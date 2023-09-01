using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;

namespace UnityNuGetManager.Package
{
    public class PackageAccessor : IPackageAccessor
    {
        private readonly IPackageSourceManager _SourceManager;
        private readonly INugetApiClient _Client; 
        
        public async Task<Dictionary<IPackageSourceInfo, QueryResponse>> QueryPackages(string query)
        {
            Dictionary<IPackageSourceInfo, QueryResponse> responses = new();

            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                QueryResponse queryResponse = await _Client.QueryPackages(source, query, false);
                if (queryResponse == null) continue;
                responses.Add(source, queryResponse);
            }

            return responses;
        }

        public async Task<PackageAccessorResult<RegistrationsReponse>> GetRegistrations(string id)
        {
            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                RegistrationsReponse response = await _Client.GetRegistrations(source, id, false);
                if (response == null) continue;

                return new PackageAccessorResult<RegistrationsReponse>(response, source);
            }

            return null;
        }

        public Task<RegistrationsReponse> GetRegistrationsDirect(string url)
        {
            return _Client.GetRegistrations(url, false);
        }

        public Task<PackageAccessorResult<DownloadResult>> TryDownloadPackage(string id, string version)
        {
            throw new System.NotImplementedException();
        }

        public Task<PackageAccessorResult<Stream>> DownloadPackage(string id, string version)
        {
            throw new System.NotImplementedException();
        }

        public PackageAccessor(INugetApiClient client, IPackageSourceManager sourceManager)
        {
            _Client = client;
            _SourceManager = sourceManager;
        }
    }
}