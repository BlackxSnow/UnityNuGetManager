using System.Xml.Serialization;

namespace UnityNuGetManager.Nuspec
{
    public class NuspecRepository
    {
        [XmlAttribute("url")]
        public string Url { get; set; }        
    }
}