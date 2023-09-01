using System;
using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class IndexResource
    {
        [JsonProperty("@id")]
        public string Id;
        [JsonProperty("@type")]
        public string Type;
        [JsonProperty("comment")]
        public string Comment;
    }
}