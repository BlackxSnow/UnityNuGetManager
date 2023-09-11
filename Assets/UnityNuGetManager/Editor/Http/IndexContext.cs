using System;
using Newtonsoft.Json;

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