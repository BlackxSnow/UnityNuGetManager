using System.Threading.Tasks;

namespace UnityNuGetManager.Package
{
    public interface IPackageDownloader
    {
        Task<bool> TryDownloadPackage(string targetPath, string id, string version);
        Task DownloadPackage(string targetPath, string id, string version);
    }
}