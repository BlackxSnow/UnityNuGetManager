using System;
using Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class IndexResponse
    {
        [JsonProperty("version")]
        public string Version;
        [JsonProperty("resources")]
        public IndexResource[] Resources;
        [JsonProperty("@context")]
        public IndexContext Context;
    }
}