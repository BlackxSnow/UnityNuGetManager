using System.Collections.Generic;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package
{
    public interface INugetPackage : IPackageIdentifier
    {
        ICollection<NugetSemanticVersion> AvailableVersions { get; }
        
    }
}