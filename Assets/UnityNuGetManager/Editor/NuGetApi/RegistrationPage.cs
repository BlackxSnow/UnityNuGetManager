using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.NuGetApi
{
    public class RegistrationPage
    {
        [JsonProperty("@id")]
        public string PageUrl { get; set; }
        public long Count { get; set; }
        public RegistrationPageLeaf[] Items { get; set; }
        public string Lower { get; set; }
        public string Parent { get; set; }
        public string Upper { get; set; }
    }
}