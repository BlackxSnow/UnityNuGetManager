using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityNuGetManager.Nuspec;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.TaskHandling;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package
{
    public class PackageInstaller : IPackageInstaller
    {
        public readonly string PackageInstallPath;
        
        private readonly IPackageManifestHandler _ManifestHandler;
        private readonly IPackageCacheManager _PackageCacheManager;
        private readonly IDependencyResolver _DependencyResolver;
        
        
        public Task AddPackage(string id, string version, bool explicitlyInstalled, TaskContext context)
        {
            _ManifestHandler.AddPackageEntry(id, version, explicitlyInstalled);
            return RestorePackages(context);
        }

        public Task ModifyPackage(string id, string version, bool explicitlyInstalled, TaskContext context)
        {
            _ManifestHandler.RemovePackageEntry(id);
            _ManifestHandler.AddPackageEntry(id, version, explicitlyInstalled);
            return RestorePackages(context);
        }

        public Task RemovePackage(string id, TaskContext context)
        {
            _ManifestHandler.RemovePackageEntry(id);
            return RestorePackages(context);
        }

        private static readonly Regex[] _ArchiveIgnores = new Regex[]
        {
            new Regex(@"\[Content_Types\]\.xml"),
            new Regex("_rels/.*"),
            new Regex("ref/.*"),
            new Regex("package/.*"),
            new Regex("lib/.*"),
            new Regex("analyzers/.*")
        };

        private static bool IsFileIgnored(string path)
        {
            return _ArchiveIgnores.Any(reg => reg.IsMatch(path));
        }
        
        private async Task InstallSinglePackage(string id, string version, TaskContext context)
        {
            Directory.CreateDirectory(PackageInstallPath);
            string packagePath = await _PackageCacheManager.GetPackageArchive(id, version, context);
            Debug.Assert(File.Exists(packagePath));
            using ZipArchive archive = ZipFile.OpenRead(packagePath);

            NuspecEntry nuspec = await GetNuspec(archive, id);
            VersionData<NuspecDependencyGroup> bestFramework = VersionResolver.GetBestDotNetVersion(nuspec.Metadata.Dependencies);
            var frameworkLibRegex = new Regex($"lib/{bestFramework.LibFolderName}/.*");
            
            string installPath = Path.Combine(PackageInstallPath, Path.GetFileNameWithoutExtension(packagePath));
            Directory.CreateDirectory(installPath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (!frameworkLibRegex.IsMatch(entry.FullName) && IsFileIgnored(entry.FullName)) continue;
                string extractionPath = Path.Combine(installPath, entry.FullName);
                string extractionDir = Path.GetDirectoryName(extractionPath);
                if (!string.IsNullOrWhiteSpace(extractionDir)) Directory.CreateDirectory(extractionDir);
                
                entry.ExtractToFile(Path.Combine(installPath, entry.FullName));
            }
        }

        private static async Task<NuspecEntry> GetNuspec(ZipArchive packageArchive, string id)
        {
            try
            {
                ZipArchiveEntry nuspecZipEntry = packageArchive.GetEntry($"{id}.nuspec");
                if (nuspecZipEntry == null)
                {
                    Debug.LogWarning($"Nuspec entry retrieval '{id}.nuspec' failed");
                    nuspecZipEntry = packageArchive.Entries.First(e => e.Name.EndsWith(".nuspec"));
                    if (nuspecZipEntry == null) throw new InvalidDataException($"Unable to find .nuspec file for {id}");
                }

                await using Stream nuspecStream = nuspecZipEntry.Open();

                var serializer = new XmlSerializer(typeof(NuspecEntry));
                var nuspec = (NuspecEntry)serializer.Deserialize(new NamespaceAgnosticXmlReader(nuspecStream));
                return nuspec;
            }
            catch (Exception)
            {
                Debug.LogError($"Failed to deserialise nuspec for package {id}.");
                throw;
            }
        }

        private async Task<IEnumerable<VersionedCatalogEntry>> ResolveDependencies(TaskContext sourceContext)
        {
            using var resolveContext =
                new TaskContext(sourceContext.Scope?.CreateSubScope($"Resolve dependencies"), sourceContext.Token);
            return await Task.Run(
                () => _DependencyResolver.Resolve(_ManifestHandler.PackageEntries.Cast<IPackageIdentifier>(),
                    resolveContext), resolveContext.Token);
        }

        public async Task RestorePackages(TaskContext context)
        {
            EditorApplication.LockReloadAssemblies();
            try
            {
                Directory.CreateDirectory(PackageInstallPath);
                HashSet<string> packageDirectories = Directory.GetDirectories(PackageInstallPath)
                    .Select(Path.GetFileName).ToHashSet();

                IEnumerable<VersionedCatalogEntry> requiredPackages = await ResolveDependencies(context);
                
                foreach (VersionedCatalogEntry entry in requiredPackages)
                {
                    if (packageDirectories.Remove($"{entry.Entry.Id}.{entry.Entry.Version}")) continue;

                    using var subContext = new TaskContext(
                        context.Scope?.CreateSubScope($"Install {entry.Entry.Id}.{entry.Entry.Version}"), context.Token);
                    await InstallSinglePackage(entry.Entry.Id, entry.Entry.Version, subContext);
                }

                foreach (string packageDirectory in packageDirectories)
                {
                    AssetDatabase.DeleteAsset(Path.Combine(PackageInstallPath, packageDirectory));
                }
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        public PackageInstaller(IPackageManifestHandler manifestHandler, IPackageCacheManager packageCacheManager,
            IDependencyResolver dependencyResolver, string packageInstallPath)
        {
            _ManifestHandler = manifestHandler;
            _PackageCacheManager = packageCacheManager;
            _DependencyResolver = dependencyResolver;
            PackageInstallPath = packageInstallPath;
        }
    }
}