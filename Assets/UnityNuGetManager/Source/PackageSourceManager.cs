using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityNuGetManager.Config;
using UnityNuGetManager.Config.Data;

// TODO: handle config changes/reloads
namespace UnityNuGetManager.Source
{
    public class PackageSourceManager : IPackageSourceManager
    {
        private ConcurrentDictionary<string, IPackageSourceInfo> _Sources;

        private IPackageSourceConfigHandler _SourceConfigHandler;
        private IPackageSourceCredentialsConfigHandler _CredentialsConfigHandler;
    
        public IPackageSourceInfo GetSource(string sourceName)
        {
            return _Sources.GetValueOrDefault(sourceName);
        }

        public ICollection<IPackageSourceInfo> GetSources()
        {
            return _Sources.Values;
        }

        public IPackageSourceInfo AddSource(string name, string url, string userName = default, string password = default)
        {
            PackageSourceCredentials credentials = null;
            if (!string.IsNullOrWhiteSpace(userName) || !string.IsNullOrWhiteSpace(password))
            {
                credentials = _CredentialsConfigHandler.AddCredentials(name, userName, password);
            }

            PackageSourceDetails sourceDetails = _SourceConfigHandler.AddSource(name, url);
            var sourceInfo = new PackageSourceInfo(sourceDetails, credentials);
            _Sources.TryAdd(name, sourceInfo);
            return sourceInfo;
        }

        private PackageSourceInfo BuildSourceInfo(PackageSourceDetails sourceDetails)
        {
            return new PackageSourceInfo(sourceDetails, _CredentialsConfigHandler.GetCredentials(sourceDetails.Name));
        }
    
        private PackageSourceInfo BuildSourceInfo(string name)
        {
            return new PackageSourceInfo(_SourceConfigHandler.GetSource(name), _CredentialsConfigHandler.GetCredentials(name));
        }

        public void RemoveSource(string name)
        {
            if (!_Sources.TryRemove(name, out IPackageSourceInfo infoToRemove)) return;
        
            _SourceConfigHandler.RemoveSource(name);
            _CredentialsConfigHandler.RemoveCredentials(name);
        }

        private void OnSourcesReload(ICollection<PackageSourceDetails> packageSources)
        {
            _Sources.Clear();
            foreach (PackageSourceDetails packageSource in packageSources)
            {
                PackageSourceInfo info = BuildSourceInfo(packageSource);
                _ = info.InitialiseSource();
                _Sources.TryAdd(packageSource.Name, info);
            }
        }
    
        public PackageSourceManager(IPackageSourceConfigHandler sourceHandler,
            IPackageSourceCredentialsConfigHandler credentialsHandler)
        {
            _SourceConfigHandler = sourceHandler;
            _CredentialsConfigHandler = credentialsHandler;
            _Sources = new ConcurrentDictionary<string, IPackageSourceInfo>();

            _SourceConfigHandler.SourcesReloaded += OnSourcesReload;
            OnSourcesReload(_SourceConfigHandler.GetSources());
        }
    }
}