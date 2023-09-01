using System;
using System.Collections.Generic;
using System.Threading;
using UnityNuGetManager.Config.Data;

namespace UnityNuGetManager.Config
{
    public interface IPackageSourceConfigHandler
    {
        event Action<PackageSourceDetails> SourceAdded;
        event Action<PackageSourceDetails> SourceRemoved;
        event Action<ICollection<PackageSourceDetails>> SourcesReloaded;
        ManualResetEvent ReloadCompleted { get; }
        
        PackageSourceDetails AddSource(string name, string url);
        void RemoveSource(string name);
        ICollection<PackageSourceDetails> GetSources();
        PackageSourceDetails GetSource(string name);
        PackageSourceDetails GetSourceByUrl(string url);
    }
}