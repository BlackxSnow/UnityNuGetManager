using System.Collections.Generic;

namespace UnityNuGetManager.Package
{
    public interface IPackageManifestHandler
    {
        IEnumerable<PackageManifestEntry> PackageEntries { get; }

        void AddPackageEntry(string id, string version, bool explicitlyInstalled);
        void RemovePackageEntry(string id, string version);
    }
}