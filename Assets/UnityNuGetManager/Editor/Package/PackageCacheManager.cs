using System;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public class PackageCacheManager : IPackageCacheManager
    {
        public string PackageCacheLocation { get; private set; }

        private readonly IPackageDownloader _PackageDownloader;

        public async Task<string> GetPackageArchive(string id, string version, TaskContext context)
        {
            var fileName = $"{id}.{version}.nupkg";
            string packagePath = Path.Combine(PackageCacheLocation, fileName);
            if (File.Exists(packagePath)) return packagePath;

            if (await _PackageDownloader.TryDownloadPackage(packagePath, id, version, context)) return packagePath;

            throw new Exception($"Unable to download package {id}.{version}.");
        }
        
        public PackageCacheManager(IPackageDownloader packageDownloader, string cacheLocation)
        {
            _PackageDownloader = packageDownloader;
            PackageCacheLocation = cacheLocation;
        }
    }
}