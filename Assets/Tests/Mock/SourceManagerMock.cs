using System;
using System.Collections.Generic;
using System.Linq;
using UnityNuGetManager.Source;

namespace Tests.Mock
{
    public class SourceManagerMock : IPackageSourceManager
    {
        private Dictionary<string, IPackageSourceInfo> _Sources;
        
        public IPackageSourceInfo GetSource(string sourceName)
        {
            return _Sources[sourceName];
        }

        public ICollection<IPackageSourceInfo> GetSources()
        {
            return _Sources.Values;
        }

        public IPackageSourceInfo AddSource(string name, string url, string userName = default, string password = default)
        {
            throw new NotSupportedException();
        }

        public void RemoveSource(string name)
        {
            throw new NotSupportedException();
        }

        public SourceManagerMock(params IPackageSourceInfo[] sources)
        {
            _Sources = sources.ToDictionary(s => s.SourceDetails.Name);
        }
    }
}