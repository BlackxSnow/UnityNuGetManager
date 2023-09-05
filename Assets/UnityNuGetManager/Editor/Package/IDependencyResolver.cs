using System.Collections.Generic;
using System.Threading.Tasks;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.Package
{
    public interface IDependencyResolver
    {
        Task<IEnumerable<VersionedCatalogEntry>> Resolve(IEnumerable<IPackageIdentifier> rootPackages, TaskContext context);
    }
}