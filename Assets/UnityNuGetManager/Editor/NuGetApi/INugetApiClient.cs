using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.NuGetApi
{
    public interface INugetApiClient
    {
        public Task<QueryResponse> QueryPackages(IPackageSourceInfo source, string query, TaskContext context, bool throwOnFail = true);

        public Task<RegistrationsReponse> GetRegistrations(IPackageSourceInfo source, string id, TaskContext context, bool throwOnFail = true);
        public Task<RegistrationsReponse> GetRegistrations(string url, TaskContext context, bool throwOnFail = true);

        public Task<DownloadResult> TryDownloadPackage(IPackageSourceInfo source, string id, string version, TaskContext context);

        public Task<Stream> DownloadPackage(IPackageSourceInfo source, string id, string version, TaskContext context);
        
    }
}