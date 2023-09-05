using System.Collections.Generic;
using System.Threading.Tasks;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.TaskHandling;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package.DependencyResolution
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IPackageAccessor _Accessor;
        
        public async Task<IEnumerable<VersionedCatalogEntry>> Resolve(IEnumerable<IPackageIdentifier> rootPackages, 
            TaskContext context)
        {
            DependencyNode root = await BuildDependencyTree(rootPackages, context);
            return FlattenDependencyTree(root);
        }

        private static IEnumerable<VersionedCatalogEntry> FlattenDependencyTree(DependencyNode rootNode)
        {
            Dictionary<string, VersionedCatalogEntry> flattenedDependencies = new();
            HashSet<DependencyNode> explored = new();
            Queue<DependencyNode> exploreQueue = new(rootNode.Dependencies.Values);

            while (exploreQueue.TryDequeue(out DependencyNode currentNode))
            {
                if (explored.Contains(currentNode)) continue;
                explored.Add(currentNode);
                flattenedDependencies.Add(currentNode.Id, currentNode.SelectedEntry);
                foreach ((string _, DependencyNode node) in currentNode.Dependencies)
                {
                    exploreQueue.Enqueue(node);
                }
            }

            return flattenedDependencies.Values;
        }

        private async Task<DependencyNode> BuildDependencyTree(IEnumerable<IPackageIdentifier> packages, TaskContext context)
        {
            using TaskContext buildContext = context.CreateSub($"Build dependency tree");
            var builder = new DependencyTreeBuilder(_Accessor);
            return await builder.Build(packages, buildContext);
        }

        public DependencyResolver(IPackageAccessor accessor)
        {
            _Accessor = accessor;
        }
    }

    public class VersionedCatalogEntry
    {
        public CatalogEntry Entry;
        public NugetSemanticVersion Version;

        public VersionedCatalogEntry(CatalogEntry entry, NugetSemanticVersion version)
        {
            Entry = entry;
            Version = version;
        }
    }
}