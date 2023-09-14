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

        public bool TryGetEntry(string id, out PackageManifestEntry entry)
        {
            return _Entries.TryGetValue(id, out entry);
        }
        
        public void AddPackageEntry(string id, string version, bool explicitlyInstalled)
        {
            _Entries.Add(id, new PackageManifestEntry(id, version, explicitlyInstalled));
            Save();
        }

        public void RemovePackageEntry(string id)
        {
            _Entries.Remove(id);
            Save();
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
            var doc = new XmlDocument();
            XmlNode root = doc.AppendChild(doc.CreateElement("packages"));
            
            foreach ((string _, PackageManifestEntry entry) in _Entries)
            {
                XmlElement xmlEntry = doc.CreateElement("package");
                xmlEntry.SetAttribute(IdKey, entry.Id);
                xmlEntry.SetAttribute(VersionKey, entry.Version);
                xmlEntry.SetAttribute(ExplicitInstallKey, entry.ExplicitlyInstalled.ToString());
                root.AppendChild(xmlEntry);
            }

            using FileStream rwStream = File.Open(_ManifestFilePath, FileMode.Create);
            doc.Save(rwStream);
        }
        
        public PackageManifestHandler(string manifestFilePath)
        {
            _ManifestFilePath = manifestFilePath;
            _Entries = new Dictionary<string, PackageManifestEntry>();
            if (!File.Exists(_ManifestFilePath))
            {
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement("packages"));
                using FileStream packageStream = File.Open(_ManifestFilePath, FileMode.Create);
                doc.Save(packageStream);
                return;
            }
            Load();
        }
    }
}