namespace UnityNuGetManager.Package
{
    public struct PackageManifestEntry : IPackageIdentifier
    {
        public string Id { get; }
        public string Version { get; }
        public bool ExplicitlyInstalled { get; }

        public PackageManifestEntry(string id, string version, bool explicitlyInstalled)
        {
            Id = id;
            Version = version;
            ExplicitlyInstalled = explicitlyInstalled;
        }
    }
}