using System;
using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class PackageTypeData
    {
        [JsonProperty("name")]
        public string Name;
    }
}