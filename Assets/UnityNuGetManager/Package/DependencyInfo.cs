using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Source;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package
{
    public class DependencyInfo
    {
        public string PackageId { get; set; }
        public RegistrationPage RegistrationPage { get; set; }
        public PackageSourceInfo Source { get; set; }
        public NugetSemanticVersion MinVersion { get; set; }
        public NugetSemanticVersion MaxVersion { get; set; }
    }
}