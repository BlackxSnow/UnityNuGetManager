using System.Collections.Generic;
using System.Threading.Tasks;
using UnityNuGetManager.Package.DependencyResolution;

namespace UnityNuGetManager.Package
{
    public interface IDependencyResolver
    {
        Task<IEnumerable<VersionedCatalogEntry>> Resolve(IEnumerable<IPackageIdentifier> rootPackages);
    }
}