using System.Xml.Serialization;
using UnityNuGetManager.Package;

namespace UnityNuGetManager.Nuspec
{
    public class NuspecDependencyGroup : ITargetFramework
    {
        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }
        [XmlElement("dependency")]
        public NuspecDependency[] Dependencies { get; set; }
    }
}