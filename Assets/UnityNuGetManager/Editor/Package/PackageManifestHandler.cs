using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Unity.Properties;

namespace UnityNuGetManager.Package
{
    public class PackageManifestHandler : IPackageManifestHandler
    {
        public IEnumerable<PackageManifestEntry> PackageEntries => _Entries.Values;

        private const string IdKey = "id";
        private const string VersionKey = "version";
        private const string ExplicitInstallKey = "explicitInstall";
        
        private readonly string _ManifestFilePath;
        private readonly Dictionary<string, PackageManifestEntry> _Entries;
        
        public void AddPackageEntry(string id, string version, bool explicitlyInstalled)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePackageEntry(string id, string version)
        {
            throw new System.NotImplementedException();
        }

        private void Load()
        {
            var doc = new XmlDocument();
            doc.Load(_ManifestFilePath);
            XmlElement root = doc.DocumentElement;
            
            foreach (XmlElement packageElement in root.ChildNodes)
            {
                string id = packageElement.GetAttribute(IdKey);
                string version = packageElement.GetAttribute(VersionKey);
                bool explicitlyInstalled = bool.Parse(packageElement.GetAttribute(ExplicitInstallKey));
                _Entries.Add(id, new PackageManifestEntry(id, version, explicitlyInstalled));
            }
        }

        private void Save()
        {
            throw new NotImplementedException();
        }
        
        public PackageManifestHandler(string manifestFilePath)
        {
            _ManifestFilePath = manifestFilePath;
            if (!File.Exists(_ManifestFilePath)) throw new InvalidPathException(manifestFilePath);
            _Entries = new Dictionary<string, PackageManifestEntry>();
            Load();
        }
    }
}