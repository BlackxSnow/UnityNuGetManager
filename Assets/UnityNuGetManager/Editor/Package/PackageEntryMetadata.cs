using UnityNuGetManager.Http;

namespace UnityNuGetManager.Package
{
    public class PackageEntryMetadata : IPackageIdentifier
    {
        public string Id { get; }
        public string Version { get; }
    }
}