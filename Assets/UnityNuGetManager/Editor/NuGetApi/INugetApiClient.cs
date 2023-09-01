using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.Source;

namespace UnityNuGetManager.NuGetApi
{
    public interface INugetApiClient
    {
        public Task<QueryResponse> QueryPackages(IPackageSourceInfo source, string query, bool throwOnFail = true);

        public Task<RegistrationsReponse> GetRegistrations(IPackageSourceInfo source, string id, bool throwOnFail = true);
        public Task<RegistrationsReponse> GetRegistrations(string url, bool throwOnFail = true);

        public Task<DownloadResult> TryDownloadPackage(IPackageSourceInfo source, string id, string version);

        public Task<Stream> DownloadPackage(IPackageSourceInfo source, string id, string version);
        
    }
}