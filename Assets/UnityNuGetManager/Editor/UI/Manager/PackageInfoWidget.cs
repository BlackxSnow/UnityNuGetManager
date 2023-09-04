using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityNuGetManager.UI.Manager
{
    public class PackageInfoWidget : VisualElement
    {
        private const string VisualTreeAssetPath = "PackageInfo";
        
        private readonly VisualElement _Logo;
        private readonly Label _Id;
        private readonly Label _Author;
        private readonly Label _Version;
        private readonly Label _Source;
        
        
        public void SetData(Package.PackageInfo data)
        {
            _Logo.style.backgroundImage = new StyleBackground();
            _Id.text = data.Id;
            _Author.text = data.Authors[0];
            _Version.text = data.LatestVersion;
            _Source.text = data.Source.SourceDetails.Name;
        }
        
        public PackageInfoWidget()
        {
            var treeAsset = Resources.Load<VisualTreeAsset>(VisualTreeAssetPath);
            treeAsset.CloneTree(this);

            _Logo = this.Q<VisualElement>("logo");
            _Id = this.Q<Label>("name");
            _Author = this.Q<Label>("author");
            _Version = this.Q<Label>("version");
            _Source = this.Q<Label>("source");
        }
    }
}
