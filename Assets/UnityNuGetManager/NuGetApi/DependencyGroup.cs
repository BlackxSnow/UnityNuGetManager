using UnityNuGetManager.Package;

namespace UnityNuGetManager.NuGetApi
{
    public class DependencyGroup : ITargetFramework
    {
        public string TargetFramework { get; set; }
        public Dependency[] Dependencies { get; set; }
    }
}