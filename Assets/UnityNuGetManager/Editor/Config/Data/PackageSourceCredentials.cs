namespace UnityNuGetManager.Config.Data
{
    public sealed class PackageSourceCredentials
    {
        public string SourceName { get; }
        public string Username { get; }
        public string ClearTextPassword { get; }

        public PackageSourceCredentials(string sourceName, string username, string clearTextPassword)
        {
            SourceName = sourceName;
            Username = username;
            ClearTextPassword = clearTextPassword;
        }
    }
}