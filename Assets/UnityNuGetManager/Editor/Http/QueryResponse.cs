using System;
using Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class QueryResponse
    {
        [JsonProperty("@context")]
        public QueryContext Context;
        [JsonProperty("totalHits")]
        public int TotalHits;
        [JsonProperty("data")]
        public PackageData[] Data;
    }
}