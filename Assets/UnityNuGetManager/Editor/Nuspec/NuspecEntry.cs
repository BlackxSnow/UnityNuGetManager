using System.Xml.Serialization;

namespace UnityNuGetManager.Nuspec
{
    [XmlRoot("package")]
    public class NuspecEntry
    {
        [XmlElement("metadata")]
        public NuspecMetadata Metadata { get; set; }
    }
}