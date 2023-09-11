using UnityNuGetManager.NuGetApi;

namespace UnityNuGetManager.Package.DependencyResolution
{
    public struct PendingNode
    {
        public DependencyNode Parent;
        public Dependency DependencyEntry;

        public PendingNode(DependencyNode parent, Dependency dependency)
        {
            Parent = parent;
            DependencyEntry = dependency;
        }
    }
}