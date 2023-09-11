using System;
using Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class PackageTypeData
    {
        [JsonProperty("name")]
        public string Name;
    }
}