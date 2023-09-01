using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityNuGetManager.Nuspec;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package
{
    public class PackageInstaller : IPackageInstaller
    {
        public readonly string PackageInstallPath;
        
        private readonly IPackageManifestHandler _ManifestHandler;
        private readonly IPackageCacheManager _PackageCacheManager;
        private readonly IDependencyResolver _DependencyResolver;
        
        
        public Task AddPackage(string id, string version, bool explicitlyInstalled)
        {
            _ManifestHandler.AddPackageEntry(id, version, explicitlyInstalled);
            throw new NotImplementedException();
        }

        public Task RemovePackage(string id, string version)
        {
            _ManifestHandler.RemovePackageEntry(id, version);
            throw new NotImplementedException();
        }

        private static readonly Regex[] _ArchiveIgnores = new Regex[]
        {
            new Regex(@"\[Content_Types\]\.xml"),
            new Regex("_rels/.*"),
            new Regex("package/.*"),
            new Regex("lib/.*"),
            new Regex("analyzers/.*")
        };

        private static bool IsFileIgnored(string path)
        {
            return _ArchiveIgnores.Any(reg => reg.IsMatch(path));
        }
        
        private async Task InstallSinglePackage(string id, string version)
        {
            Directory.CreateDirectory(PackageInstallPath);
            string packagePath = await _PackageCacheManager.GetPackageArchive(id, version);
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
                ZipArchiveEntry nuspecZipEntry = packageArchive.GetEntry($"{id}.nupsec");
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

        public async Task RestorePackages()
        {
            Directory.CreateDirectory(PackageInstallPath);
            HashSet<string> packageDirectories = Directory.GetDirectories(PackageInstallPath)
                .Select(Path.GetFileName).ToHashSet();

            IEnumerable<VersionedCatalogEntry> remaining = await Task.Run(
                () => _DependencyResolver.Resolve(_ManifestHandler.PackageEntries.Cast<IPackageIdentifier>()));

            foreach (VersionedCatalogEntry entry in remaining)
            {
                if (packageDirectories.Remove($"{entry.Entry.Id}.{entry.Entry.Version}")) continue;
                await InstallSinglePackage(entry.Entry.Id, entry.Entry.Version);
            }

            foreach (string packageDirectory in packageDirectories)
            {
                Directory.Delete(packageDirectory, true);
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