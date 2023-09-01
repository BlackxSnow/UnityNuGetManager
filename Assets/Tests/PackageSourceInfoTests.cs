using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Mock;
using UnityEngine;
using UnityEngine.TestTools;
using UnityNuGetManager;
using UnityNuGetManager.Config.Data;
using UnityNuGetManager.Source;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class PackageSourceInfoTests
    {

        public static SourceInfoMock[] TestSources = {
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
        public async Task InitialisePackageSourceInfo([ValueSource(nameof(TestSources))] SourceInfoMock infoMock)
        {
            var info = new PackageSourceInfo(infoMock.SourceDetails, infoMock.Credentials);

            await info.InitialiseSource();
            Assert.AreEqual(infoMock.QueryUrl, info.QueryUrl);
            Assert.AreEqual(infoMock.RegistrationsUrl, info.RegistrationsUrl);
            Assert.AreEqual(infoMock.BaseAddress, info.BaseAddress);
        }
    }
}
