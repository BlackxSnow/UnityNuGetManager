using System.Threading;
using UnityEditor;
using UnityNuGetManager.Config;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Package;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;
using UnityNuGetManager.UI.Progress;

namespace UnityNuGetManager
{
    public class PackageManager
    {
        private static PackageManager _Instance;
        public static PackageManager Instance => _Instance ??= new PackageManager();

        public static SynchronizationContext UnitySyncContext { get; } = SynchronizationContext.Current;
        
        internal const string PackagePrefx = "celestemarina.unitynugetmanager";
        
        public Configuration Configuration { get; }
        public IPackageSourceConfigHandler SourceConfigHandler { get; }
        public IPackageSourceCredentialsConfigHandler SourceCredentialsConfigHandler { get; }
        public IPackageManifestHandler ManifestHandler { get; }
        
        public IPackageSourceManager SourceManager { get; }
        public INugetApiClient ApiClient { get; }
        public IPackageDownloader PackageDownloader { get; }
        public IPackageCacheManager CacheManager { get; }
        public IPackageAccessor Accessor { get; }
        public IDependencyResolver DependencyResolver { get; }
        public IPackageInstaller Installer { get; }

        [MenuItem("NuGet/Restore")]
        public static void Restore()
        {
            ProgressWindow.DoTaskWithProgress("Restoring Packages", Instance.Installer.RestorePackages);
        }

        public PackageManager()
        {
            Configuration = new Configuration("Assets/NuGet.config");
            SourceConfigHandler = new PackageSourceConfigHandler(Configuration);
            SourceCredentialsConfigHandler = new PackageSourceCredentialsConfigHandler(Configuration);
            ManifestHandler = new PackageManifestHandler("Assets/packages.config");

            SourceManager = new PackageSourceManager(SourceConfigHandler, SourceCredentialsConfigHandler);
            ApiClient = new NugetApiClient();
            PackageDownloader = new PackageDownloader(ApiClient, SourceManager);
            CacheManager = new PackageCacheManager(PackageDownloader, "Library/NuGetCache");
            Accessor = new PackageAccessor(ApiClient, SourceManager);
            DependencyResolver = new DependencyResolver(Accessor);
            Installer = new PackageInstaller(ManifestHandler, CacheManager, DependencyResolver, "Assets/Packages");
        }
    }
}