using System;
using System.IO;
using System.Xml;

namespace UnityNuGetManager.Config
{
    public class Configuration
    {
        public string SourcePath { get; }
        public event Action<XmlDocument> Reloaded;

        private FileSystemWatcher _ConfigurationWatcher;
        
        private object _ConfigLock = new object();
        private XmlDocument _ConfigurationDocument;
        public XmlDocument ConfigurationDocument { 
            get {
                lock (_ConfigLock)
                {
                    return _ConfigurationDocument;
                }
            }
        }

        public void Save()
        {
            lock (_ConfigLock)
            {
                _ConfigurationDocument.Save(SourcePath);
            }
        }
        
        private void OnConfigurationChanged(object sender, FileSystemEventArgs e)
        {
            lock (_ConfigLock)
            {
                _ConfigurationDocument.Load(SourcePath);
                Reloaded?.Invoke(ConfigurationDocument);   
            }
        }
        
        public Configuration(string path)
        {
            if (!File.Exists(path)) throw new ArgumentException("Invalid configuration path.");

            _ConfigurationDocument = new XmlDocument();
            ConfigurationDocument.Load(path);
            
            _ConfigurationWatcher = new FileSystemWatcher(Path.GetDirectoryName(path)!);
            _ConfigurationWatcher.Filter = Path.GetFileName(path);
            _ConfigurationWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _ConfigurationWatcher.Changed += OnConfigurationChanged;
            _ConfigurationWatcher.EnableRaisingEvents = true;
        }


    }
}