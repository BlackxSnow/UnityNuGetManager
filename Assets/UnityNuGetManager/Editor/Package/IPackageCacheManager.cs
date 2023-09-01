using System.Threading.Tasks;

namespace UnityNuGetManager.Package
{
    public interface IPackageCacheManager
    {
        Task<string> GetPackageArchive(string id, string version);
    }
}