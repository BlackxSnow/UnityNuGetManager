using System;
using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class IndexContext
    {
        [JsonProperty("@vocab")]
        public string Vocab;
        [JsonProperty("@comment")]
        public string Comment;
    }
}