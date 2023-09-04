using System.Linq;
using UnityNuGetManager.Http;
using UnityNuGetManager.Source;

namespace UnityNuGetManager.Package
{
    public class PackageInfo
    {
        public readonly string InfoUrl;
        public readonly string RegistrationsUrl;
        public readonly string IconUrl;
        public readonly string Id;
        public readonly string LatestVersion;
        public readonly string[] Versions;
        public readonly string Description;
        public readonly string Summary;
        public readonly string[] Tags;
        public readonly string[] Authors;
        public readonly ulong Downloads;
        public readonly IPackageSourceInfo Source;

        public bool IsInstalled;
        public string InstalledVersion;
        
        public PackageInfo(PackageData data, IPackageSourceInfo source)
        {
            InfoUrl = data.IdUrl;
            RegistrationsUrl = data.Registration;
            IconUrl = data.IconUrl;
            Id = data.Id;
            LatestVersion = data.Version;
            Versions = data.Versions.Select(v => v.Version).ToArray();
            Description = data.Description;
            Summary = data.Summary;
            Tags = data.Tags;
            Authors = data.Authors;
            Downloads = data.TotalDownloads;

            Source = source;

            IsInstalled = PackageManager.Instance.ManifestHandler.TryGetEntry(Id, out PackageManifestEntry entry);
            InstalledVersion = IsInstalled ? entry.Version : "";
        }
    }
}