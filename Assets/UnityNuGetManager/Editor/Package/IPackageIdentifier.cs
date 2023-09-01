namespace UnityNuGetManager.Package
{
    public interface IPackageIdentifier
    {
        string Id { get; }
        string Version { get; }
    }
}