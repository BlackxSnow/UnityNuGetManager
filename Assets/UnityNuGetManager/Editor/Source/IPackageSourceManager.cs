using System.Collections.Generic;

namespace UnityNuGetManager.Source
{
    public interface IPackageSourceManager
    {
        IPackageSourceInfo GetSource(string sourceName);
        ICollection<IPackageSourceInfo> GetSources();

        IPackageSourceInfo AddSource(string name, string url, string userName = default, string password = default);
        void RemoveSource(string name);
    }
}