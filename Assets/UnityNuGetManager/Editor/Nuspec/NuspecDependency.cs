using System.Xml.Serialization;
using UnityNuGetManager.Package;

namespace UnityNuGetManager.Nuspec
{
    public class NuspecDependency
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("version")]
        public string Version { get; set; }
        [XmlAttribute("exclude")]
        public string Exclude { get; set; }
    }
}