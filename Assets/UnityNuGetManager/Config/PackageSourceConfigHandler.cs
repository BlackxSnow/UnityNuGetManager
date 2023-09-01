using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using UnityNuGetManager.Config.Data;

namespace UnityNuGetManager.Config
{
    public class PackageSourceConfigHandler : IPackageSourceConfigHandler
    {
        private const string XPath = "packageSources";
        private readonly Configuration _Config;
        
        public event Action<PackageSourceDetails> SourceAdded;
        public event Action<PackageSourceDetails> SourceRemoved;
        public event Action<ICollection<PackageSourceDetails>> SourcesReloaded;
        public ManualResetEvent ReloadCompleted { get; } = new(false);
        private XmlNode _PackageSourcesElement;
        
        private readonly ConcurrentDictionary<string, PackageSourceDetails> _Sources = new();
        private readonly ConcurrentDictionary<string, PackageSourceDetails> _SourcesByUrl = new();

        public PackageSourceDetails AddSource(string name, string url)
        {
            ReloadCompleted.WaitOne();
            if (_Sources.ContainsKey(name) || _SourcesByUrl.ContainsKey(url)) return null;
            
            var source = new PackageSourceDetails(name, url);
            _Sources.TryAdd(name, source);
            _SourcesByUrl.TryAdd(url, source);
            
            XmlElement xmlSource = _Config.ConfigurationDocument.CreateElement("add");
            xmlSource.SetAttribute("key", name);
            xmlSource.SetAttribute("value", url);
            _PackageSourcesElement.AppendChild(xmlSource);
            
            SourceAdded?.Invoke(source);
            _Config.Save();
            return source;
        }

        public void RemoveSource(string name)
        {
            ReloadCompleted.WaitOne();
            if (!_Sources.TryRemove(name, out PackageSourceDetails removed)) return;
            _SourcesByUrl.TryRemove(removed.Url, out _);
            
            SourceRemoved?.Invoke(removed);
            XmlNode toRemove = _PackageSourcesElement.SelectSingleNode($"add[@key=\"{name}\"");
            if (toRemove == null) return;
            _PackageSourcesElement.RemoveChild(toRemove);
            
            _Config.Save();
        }

        public ICollection<PackageSourceDetails> GetSources()
        {
            ReloadCompleted.WaitOne();
            return _Sources.Values;
        }

        public PackageSourceDetails GetSource(string name)
        {
            ReloadCompleted.WaitOne();
            return _Sources.GetValueOrDefault(name);
        }

        public PackageSourceDetails GetSourceByUrl(string url)
        {
            ReloadCompleted.WaitOne();
            return _SourcesByUrl.GetValueOrDefault(url);
        }

        private void AddSourceFromXml(XmlElement sourceXml)
        {
            string name = sourceXml.GetAttribute("key");
            string url = sourceXml.GetAttribute("value");
            if (_Sources.ContainsKey(name) || _SourcesByUrl.ContainsKey(url)) return;
            var source = new PackageSourceDetails(name, url);
            _Sources.TryAdd(name, source);
            _SourcesByUrl.TryAdd(url, source);
        }
        
        private void OnConfigReload(XmlDocument document)
        {
            ReloadCompleted.Reset();
            _Sources.Clear();
            _SourcesByUrl.Clear();
            
            _PackageSourcesElement = document.DocumentElement?.SelectSingleNode(XPath);
            
            foreach (XmlElement sourceXml in _PackageSourcesElement!.ChildNodes)
            {
                AddSourceFromXml(sourceXml);
            }
            
            ReloadCompleted.Set();
            SourcesReloaded?.Invoke(_Sources.Values);
        }
        
        public PackageSourceConfigHandler(Configuration config)
        {
            _Config = config;
            OnConfigReload(_Config.ConfigurationDocument);
            _Config.Reloaded += OnConfigReload;
        }
    }
}