using System.Xml.Serialization;

namespace UnityNuGetManager.Nuspec
{
    public class NuspecMetadata
    {
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("version")]
        public string Version { get; set; }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("authors")]
        public string Authors { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("releaseNotes")]
        public string ReleaseNotes { get; set; }
        [XmlElement("tags")]
        public string Tags { get; set; }
        [XmlElement("repository")]
        public NuspecRepository Repository { get; set; }
        [XmlArray("dependencies")]
        [XmlArrayItem("group")]
        public NuspecDependencyGroup[] Dependencies { get; set; }
        [XmlArray("frameworkAssemblies")]
        [XmlArrayItem("frameworkAssembly")]
        public NuspecFrameworkAssembly[] FrameworkAssemblies { get; set; }
    }
}