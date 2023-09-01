using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.NuGetApi
{
    public class CatalogEntry
    {
        [JsonConverter(typeof(ToArrayConverter<string>))]
        public string[] Authors { get; set; }
        public DependencyGroup[] DependencyGroups { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string Id { get; set; }
        public string Language { get; set; }
        public string LicenseUrl { get; set; }
        public string LicenseExpression { get; set; }
        public string MinClientVersion { get; set; }
        public string PackageContent { get; set; }
        public string ProjectUrl { get; set; }
        public string Published { get; set; }
        public string ReadmeUrl { get; set; }
        public bool RequireLicenseAcceptance { get; set; }
        public string Summary { get; set; }
        [JsonConverter(typeof(ToArrayConverter<string>))]
        public string[] Tags { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
    }
}