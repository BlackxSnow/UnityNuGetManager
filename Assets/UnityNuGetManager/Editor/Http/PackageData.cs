using System;
using Unity.Plastic.Newtonsoft.Json;

namespace UnityNuGetManager.Http
{
    [Serializable]
    public class PackageData
    {
        [JsonProperty("@id")]
        public string IdUrl;
        [JsonProperty("@type")]
        public string Type;
        [JsonProperty("registration")]
        public string Registration;
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("summary")]
        public string Summary;
        [JsonProperty("tags")]
        public string[] Tags;
        [JsonProperty("authors")]
        public string[] Authors;
        [JsonProperty("owners")]
        public string[] Owners;
        [JsonProperty("totalDownloads")]
        public ulong TotalDownloads;
        [JsonProperty("verified")]
        public bool Verified;
        [JsonProperty("packageTypes")]
        public PackageTypeData[] PackageTypes;
        [JsonProperty("vulnerabilities")]
        public string[] Vulnerabilities;
    }
}