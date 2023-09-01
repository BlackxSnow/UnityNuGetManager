using NUnit.Framework;
using UnityNuGetManager.Package;
using UnityNuGetManager.Version;

namespace Tests
{
    public class VersionResolverTests
    {
        private class Target : ITargetFramework
        {
            public string TargetFramework { get; set; }

            public override string ToString() => TargetFramework;

            public Target(string framework)
            {
                TargetFramework = framework;
            }
        }
        [Test]
        public void VersionPriority()
        {
            var targets = new Target[]
            {
                new Target(".NETFramework4.6.2"),
                new Target(".NETFramework4.8"),
                new Target(".NETFramework4"),
                new Target(".NETFramework3.5"),
            };

            VersionData<Target> bestVersion = VersionResolver.GetBestDotNetVersion(targets);
            
            Assert.AreEqual(targets[1], bestVersion.SelectedTarget);
        }
        
        [Test]
        public void FrameworkPriority()
        {
            var targets = new Target[]
            {
                new Target(".NETFramework2.0"),
                new Target(".NETStandard2.1")
            };

            VersionData<Target> bestVersion = VersionResolver.GetBestDotNetVersion(targets);
            
            Assert.AreEqual(targets[0], bestVersion.SelectedTarget);
        }
        
        [Test]
        public void UnhandledFrameworks()
        {
            var targets = new Target[]
            {
                new Target(".NETStandard2.0"),
                new Target(".NETCore3.5"),
                new Target(".NETFramework4.7.2"),
            };

            VersionData<Target> bestVersion = VersionResolver.GetBestDotNetVersion(targets);
            
            Assert.AreEqual(targets[2], bestVersion.SelectedTarget);
        }
    }
}