using System;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public class PackageDownloader : IPackageDownloader
    {
        private readonly INugetApiClient _ApiClient;
        private readonly IPackageSourceManager _SourceManager;
        
        public async Task<bool> TryDownloadPackage(string targetPath, string id, string version, TaskContext context)
        {
            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                DownloadResult downloadResult = await _ApiClient.TryDownloadPackage(source, id, version, context);
                if (!downloadResult.Succeeded) continue;

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                await using FileStream writer = File.OpenWrite(targetPath);
                await downloadResult.Data.CopyToAsync(writer);
                return true;
            }

            return false;
        }

        public async Task DownloadPackage(string targetPath, string id, string version, TaskContext context)
        {
            if (!await TryDownloadPackage(targetPath, id, version, context)) throw new Exception();
        }

        public PackageDownloader(INugetApiClient apiClient, IPackageSourceManager sourceManager)
        {
            _ApiClient = apiClient;
            _SourceManager = sourceManager;
        }
    }
}