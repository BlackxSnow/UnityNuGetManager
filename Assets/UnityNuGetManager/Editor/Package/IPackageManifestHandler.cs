using System.Collections.Generic;

namespace UnityNuGetManager.Package
{
    public interface IPackageManifestHandler
    {
        IEnumerable<PackageManifestEntry> PackageEntries { get; }

        public bool TryGetEntry(string id, out PackageManifestEntry entry);
        void AddPackageEntry(string id, string version, bool explicitlyInstalled);
        void RemovePackageEntry(string id);
    }
}