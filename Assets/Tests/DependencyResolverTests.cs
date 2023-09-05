using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Mock;
using UnityNuGetManager.Config.Data;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Package;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace Tests
{
    public class DependencyResolverTests
    {
        private static IPackageSourceInfo[] _TestSources = {
            new SourceInfoMock{
                SourceDetails = new PackageSourceDetails("nuget.org", "https://api.nuget.org/v3/index.json"),
                QueryUrl = "https://azuresearch-usnc.nuget.org/query",
                RegistrationsUrl = "https://api.nuget.org/v3/registration5-gz-semver2/", 
                BaseAddress = "https://api.nuget.org/v3-flatcontainer/"
            },
            new SourceInfoMock{
                SourceDetails = new PackageSourceDetails("github.com", "https://nuget.pkg.github.com/BlackxSnow/index.json"),
                Credentials = new PackageSourceCredentials("github.com", "BlackxSnow", "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"),
                QueryUrl = "https://nuget.pkg.github.com/BlackxSnow/query",
                RegistrationsUrl = "https://nuget.pkg.github.com/BlackxSnow", 
                BaseAddress = "https://nuget.pkg.github.com/BlackxSnow/download",
            }
        };

        [Test]
        public async Task Resolve()
        {
            var resolver = new DependencyResolver(new PackageAccessor(new NugetApiClient(), new SourceManagerMock(_TestSources)));
            IPackageIdentifier testPackage =
                new PackageManifestEntry("CelesteMarina.DependencyInjection", "1.1.0", true);
            var context = new TaskContext(null, new CancellationToken());
            IEnumerable<VersionedCatalogEntry> resolution =
                await resolver.Resolve(new[] { testPackage }, context);
            
            Assert.NotNull(resolution);
            Assert.Greater(resolution.Count(), 0);
        }
    }
}