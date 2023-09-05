using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

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
        public Task<Dictionary<IPackageSourceInfo, QueryResponse>> QueryPackages(string query, TaskContext context);

        public Task<PackageAccessorResult<RegistrationsReponse>> GetRegistrations(string id, TaskContext context);
        public Task<RegistrationsReponse> GetRegistrationsDirect(string url, TaskContext context);

        public Task<PackageAccessorResult<DownloadResult>> TryDownloadPackage(string id, string version, TaskContext context);

        public Task<PackageAccessorResult<Stream>> DownloadPackage(string id, string version, TaskContext context);
    }
}