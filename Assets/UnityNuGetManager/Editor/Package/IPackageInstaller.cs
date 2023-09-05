using System.Threading.Tasks;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public interface IPackageInstaller
    {
        Task AddPackage(string id, string version, bool explicitlyInstalled, TaskContext context);
        Task RemovePackage(string id, TaskContext context);
        Task ModifyPackage(string id, string version, bool explicitlyInstalled, TaskContext context);
        Task RestorePackages(TaskContext context);
    }
}