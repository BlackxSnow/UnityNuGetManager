using System.Threading.Tasks;

namespace UnityNuGetManager.Package
{
    public interface IPackageInstaller
    {
        Task AddPackage(string id, string version, bool explicitlyInstalled);
        Task RemovePackage(string id);
        Task ModifyPackage(string id, string version, bool explicitlyInstalled);
        Task RestorePackages();
    }
}