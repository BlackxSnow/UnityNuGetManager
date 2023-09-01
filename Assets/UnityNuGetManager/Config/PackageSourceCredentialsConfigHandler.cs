using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using UnityNuGetManager.Config.Data;

namespace UnityNuGetManager.Config
{
    public class PackageSourceCredentialsConfigHandler : IPackageSourceCredentialsConfigHandler
    {
        private const string XPath = "packageSourceCredentials";
        private const string UserNameKey = "userName";
        private const string PasswordKey = "clearTextPassword";
        private readonly Configuration _Config;
        
        public event Action<PackageSourceCredentials> CredentialsAdded;
        public event Action<PackageSourceCredentials> CredentialsRemoved;
        public event Action<ICollection<PackageSourceCredentials>> CredentialsReloaded;
        public ManualResetEvent ReloadCompleted { get; } = new(false);
        private XmlNode _CredentialsElement;

        private readonly ConcurrentDictionary<string, PackageSourceCredentials> _Credentials = new();

        public PackageSourceCredentials AddCredentials(string source, string userName, string password)
        {
            ReloadCompleted.WaitOne();
            if (_Credentials.ContainsKey(source)) return null;
            var credentials = new PackageSourceCredentials(source, userName, password);
            _Credentials.TryAdd(source, credentials);

            XmlElement credentialsRoot = _Config.ConfigurationDocument.CreateElement(source);
            XmlElement userNameElement = _Config.ConfigurationDocument.CreateElement("add");
            userNameElement.SetAttribute("key", UserNameKey);
            userNameElement.SetAttribute("value", userName);
            credentialsRoot.AppendChild(userNameElement);
            XmlElement passwordElement = _Config.ConfigurationDocument.CreateElement("add");
            passwordElement.SetAttribute("key", PasswordKey);
            passwordElement.SetAttribute("value", password);
            credentialsRoot.AppendChild(passwordElement);
            _CredentialsElement.AppendChild(credentialsRoot);
            
            CredentialsAdded?.Invoke(credentials);
            _Config.Save();
            return credentials;
        }

        public void RemoveCredentials(string source)
        {
            ReloadCompleted.WaitOne();
            if (!_Credentials.TryRemove(source, out PackageSourceCredentials credentials)) return;

            _CredentialsElement.RemoveChild(_CredentialsElement.SelectSingleNode(source)!);
            
            CredentialsRemoved?.Invoke(credentials);
            _Config.Save();
        }

        public ICollection<PackageSourceCredentials> GetAllCredentials()
        {
            ReloadCompleted.WaitOne();
            return _Credentials.Values;
        }

        public PackageSourceCredentials GetCredentials(string source)
        {
            return _Credentials.GetValueOrDefault(source);
        }

        private void AddCredentialsFromXml(XmlNode credentialsXml)
        {
            string source = credentialsXml.Name;
            if (_Credentials.ContainsKey(source)) return;
            var userNameElement = credentialsXml.SelectSingleNode($"add[@key=\"{UserNameKey}\"]") as XmlElement;
            var passwordElement = credentialsXml.SelectSingleNode($"add[@key=\"{PasswordKey}\"]") as XmlElement;
            string userName = userNameElement?.GetAttribute("value");
            string password = passwordElement?.GetAttribute("value");

            _Credentials.TryAdd(source, new PackageSourceCredentials(source, userName, password));
        }
        
        private void OnConfigReload(XmlDocument document)
        {
            ReloadCompleted.Reset();
            _Credentials.Clear();
            
            _CredentialsElement = document.DocumentElement?.SelectSingleNode(XPath);
            
            foreach (XmlElement credentialsXml in _CredentialsElement!.ChildNodes)
            {
                AddCredentialsFromXml(credentialsXml);
            }
            
            ReloadCompleted.Set();
            CredentialsReloaded?.Invoke(_Credentials.Values);
        }
        
        public PackageSourceCredentialsConfigHandler(Configuration config)
        {
            _Config = config;
            OnConfigReload(_Config.ConfigurationDocument);
            _Config.Reloaded += OnConfigReload;
        }
    }
}