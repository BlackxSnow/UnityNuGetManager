using System;
using Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class QueryContext
    {
        [JsonProperty("@vocab")]
        public string Vocab;
        [JsonProperty("@base")]
        public string Base;
    }
}