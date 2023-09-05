using System.Threading.Tasks;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public interface IPackageCacheManager
    {
        Task<string> GetPackageArchive(string id, string version, TaskContext context);
    }
}