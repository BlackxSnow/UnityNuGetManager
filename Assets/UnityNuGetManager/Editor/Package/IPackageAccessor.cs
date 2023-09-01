using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;

namespace UnityNuGetManager.Package
{
    public class PackageAccessorResult<T>
    {
        public T Result;
        public IPackageSourceInfo Source;

        public PackageAccessorResult(T result, IPackageSourceInfo source)
        {
            Result = result;
            Source = source;
        }
    }
    
    public interface IPackageAccessor
    {
        public Task<Dictionary<IPackageSourceInfo, QueryResponse>> QueryPackages(string query);

        public Task<PackageAccessorResult<RegistrationsReponse>> GetRegistrations(string id);
        public Task<RegistrationsReponse> GetRegistrationsDirect(string url);

        public Task<PackageAccessorResult<DownloadResult>> TryDownloadPackage(string id, string version);

        public Task<PackageAccessorResult<Stream>> DownloadPackage(string id, string version);
    }
}