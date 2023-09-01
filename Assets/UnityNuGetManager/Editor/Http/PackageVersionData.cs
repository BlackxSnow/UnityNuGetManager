using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    public class PackageVersionData
    {
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("downloads")]
        public int Downloads { get; set; }
        [JsonProperty("@id")]
        public string Id { get; set; }
    }
}