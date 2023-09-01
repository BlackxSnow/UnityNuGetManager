using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Mock;
using UnityNuGetManager;
using UnityNuGetManager.Config.Data;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;

namespace Tests
{
    public class NugetApiClientTests
    {
        private IPackageSourceInfo _TestSourceInfo = new SourceInfoMock
        {
            SourceDetails = new PackageSourceDetails("nuget.org", "https://api.nuget.org/v3/index.json"),
            QueryUrl = "https://azuresearch-usnc.nuget.org/query",
            RegistrationsUrl = "https://api.nuget.org/v3/registration5-gz-semver2/",
            BaseAddress = "https://api.nuget.org/v3-flatcontainer/"
        };
        
        [Test]
        public async Task QueryPackages()
        {
            var client = new NugetApiClient();
            QueryResponse queryResult = await client.QueryPackages(_TestSourceInfo, "Microsoft");
            
            Assert.NotNull(queryResult);
            Assert.NotNull(queryResult.Data);
            Assert.Greater(queryResult.TotalHits, 0);
            Assert.Greater(queryResult.Data.Length, 0);
        }

        [Test]
        public async Task GetRegistrations()
        {
            var client = new NugetApiClient();

            const string diID = "microsoft.extensions.dependencyinjection";
            RegistrationsReponse registrationsReponse =
                await client.GetRegistrations(_TestSourceInfo, diID);
            
            Assert.Greater(registrationsReponse.Count, 0);
            Assert.Greater(registrationsReponse.Items.Length, 0);
            Assert.Greater(registrationsReponse.Items[0].Items.Length, 0);
            Assert.AreEqual(diID, registrationsReponse.Items[0].Items[0].CatalogEntry);
        }

        [Test]
        public async Task GetRegistrationLeaf()
        {
            
        }
        
        [Test]
        public async Task DownloadPackage()
        {
            var client = new NugetApiClient();
            DownloadResult result = await client.TryDownloadPackage(_TestSourceInfo, "Microsoft.Extensions.DependencyInjection", "7.0.0");
            
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Greater(result.Data.Length, 0);
        }
    }
}