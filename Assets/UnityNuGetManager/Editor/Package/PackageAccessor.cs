using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public class PackageAccessor : IPackageAccessor
    {
        private readonly IPackageSourceManager _SourceManager;
        private readonly INugetApiClient _Client; 
        
        public async Task<Dictionary<IPackageSourceInfo, QueryResponse>> QueryPackages(string query, TaskContext context)
        {
            Dictionary<IPackageSourceInfo, QueryResponse> responses = new();

            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                QueryResponse queryResponse = await _Client.QueryPackages(source, query, context, false);
                if (queryResponse == null) continue;
                responses.Add(source, queryResponse);
            }

            return responses;
        }

        public async Task<PackageAccessorResult<RegistrationsReponse>> GetRegistrations(string id, TaskContext context)
        {
            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                RegistrationsReponse response = await _Client.GetRegistrations(source, id, context, false);
                if (response == null) continue;

                return new PackageAccessorResult<RegistrationsReponse>(response, source);
            }

            return null;
        }

        public async Task<IEnumerable<RegistrationsReponse>> GetAllRegistrations(string id, TaskContext context)
        {
            var results = new List<RegistrationsReponse>();
            foreach (IPackageSourceInfo source in _SourceManager.GetSources())
            {
                RegistrationsReponse response = await _Client.GetRegistrations(source, id, context, false);
                if (response == null) continue;
                results.Add(response);
            }

            return results;
        }

        public Task<RegistrationsReponse> GetRegistrationsDirect(string url, TaskContext context)
        {
            return _Client.GetRegistrations(url, context, false);
        }

        public Task<PackageAccessorResult<DownloadResult>> TryDownloadPackage(string id, string version, TaskContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task<PackageAccessorResult<Stream>> DownloadPackage(string id, string version, TaskContext context)
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