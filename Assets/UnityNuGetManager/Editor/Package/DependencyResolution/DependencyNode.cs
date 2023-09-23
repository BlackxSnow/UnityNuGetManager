using System.Collections.Generic;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package.DependencyResolution
{
    public class DependencyNode
    {
        public DependencyNode PrimaryParent;
        public List<DependencyNode> Parents = new();
        public string Id;
        public NugetSemanticVersion TargetVersion;
        public bool VersionIsMinimum;
        public IEnumerable<RegistrationsReponse> Registrations;
        public VersionedCatalogEntry SelectedEntry;

        public readonly Dictionary<string, DependencyNode> Dependencies = new();

        public bool TryGetNodeFromAncestors(string id, out DependencyNode node)
        {
            node = PrimaryParent?.GetNodeInTree(id);
            return node != null;
        }

        private DependencyNode GetNodeInTree(string id)
        {
            return Dependencies.TryGetValue(id, out DependencyNode node) ? node : PrimaryParent?.GetNodeInTree(id);
        }

        public DependencyNode() {}
        public DependencyNode(DependencyNode parent, string id, NugetSemanticVersion targetVersion, bool versionIsMinimum,
            IEnumerable<RegistrationsReponse> registrations)
        {
            PrimaryParent = parent;
            Parents.Add(parent);
            PrimaryParent.Dependencies.Add(id, this);
            Id = id;
            TargetVersion = targetVersion;
            VersionIsMinimum = versionIsMinimum;
            Registrations = registrations;
        }
    }
}