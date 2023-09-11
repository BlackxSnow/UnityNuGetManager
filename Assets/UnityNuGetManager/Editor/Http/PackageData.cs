using System;
using Newtonsoft.Json;

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
        [JsonProperty("version")] 
        public string Version;
        public PackageVersionData[] Versions;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("summary")]
        public string Summary;
        [JsonProperty("tags")]
        [JsonConverter(typeof(ToArrayConverter<string>))]
        public string[] Tags;
        [JsonProperty("authors")]
        [JsonConverter(typeof(ToArrayConverter<string>))]
        public string[] Authors;
        [JsonProperty("iconUrl")] 
        public string IconUrl;
        [JsonProperty("owners")]
        [JsonConverter(typeof(ToArrayConverter<string>))]
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