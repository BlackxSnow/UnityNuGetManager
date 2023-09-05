using System.Threading.Tasks;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public interface IPackageDownloader
    {
        Task<bool> TryDownloadPackage(string targetPath, string id, string version, TaskContext context);
        Task DownloadPackage(string targetPath, string id, string version, TaskContext context);
    }
}