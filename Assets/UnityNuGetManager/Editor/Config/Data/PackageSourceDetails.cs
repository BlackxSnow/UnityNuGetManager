
namespace UnityNuGetManager.Config.Data
{
    public class PackageSourceDetails
    {
        public string Name { get; }
        public string Url { get; }

        public PackageSourceDetails(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
