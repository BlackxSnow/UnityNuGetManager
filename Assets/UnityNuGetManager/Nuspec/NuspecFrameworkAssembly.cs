using System.Xml.Serialization;
using UnityNuGetManager.Package;

namespace UnityNuGetManager.Nuspec
{
    public class NuspecFrameworkAssembly : ITargetFramework
    {
        [XmlAttribute("assemblyName")]
        public string AssemblyName { get; set; }
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }
    }
}