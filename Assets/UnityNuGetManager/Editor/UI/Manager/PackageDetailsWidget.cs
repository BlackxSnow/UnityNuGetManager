using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using UnityNuGetManager.Extensions;
using PackageInfo = UnityNuGetManager.Package.PackageInfo;
using Task = System.Threading.Tasks.Task;

namespace UnityNuGetManager.UI.Manager
{
    public class PackageDetailsWidget
    {
        public VisualElement Root { get; }

        private readonly VisualElement _PackageLogo;
        private readonly Label _PackageName;
        private readonly Label _PackageAuthor;

        private readonly VisualElement _DetailsPanel;
        private readonly Label _Description;

        private readonly DropdownField _VersionDropdown;
        private readonly Button _Install;
        private readonly Button _Modify;
        private readonly Button _Uninstall;

        private PackageInfo _Data;
        
        public void SetData(PackageInfo data)
        {
            if (data == null)
            {
                Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                return;
            }
            Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            _Data = data;
            _PackageLogo.style.backgroundImage = new StyleBackground();
            _PackageName.text = data.Id;
            _PackageAuthor.text = string.Join(", ", data.Authors);
            _Description.text = data.Description;

            List<string> choices = data.Versions.ToList();
            choices.Sort((a, b) => string.Compare(b, a, StringComparison.Ordinal));
            _VersionDropdown.choices = choices;
            _VersionDropdown.index = 0;
            VersionDropdownChanged(_VersionDropdown.value);
        }

        private void VersionDropdownChanged(ChangeEvent<string> changeEvent) =>
            VersionDropdownChanged(changeEvent.newValue);

        private void VersionDropdownChanged(string value)
        {
            if (!_Data.IsInstalled)
            {
                _Uninstall.SetDisplay(false);
                _Modify.SetDisplay(false);
                _Install.SetDisplay(true);
                return;
            }

            if (_Data.InstalledVersion == changeEvent.newValue)
            {
                _Uninstall.SetDisplay(true);
                _Modify.SetDisplay(false);
                _Install.SetDisplay(false);
                return;
            }
            
            _Uninstall.SetDisplay(false);
            _Modify.SetDisplay(true);
            _Install.SetDisplay(false);
        }
        
        
        
        private async void Install()
        {
            string version = _VersionDropdown.value;
            EditorUtility.DisplayProgressBar("Installing package", $"Installing {_Data.Id}.{version}", 0);
            await PackageManager.Instance.Installer.AddPackage(_Data.Id, version, true);
            EditorUtility.ClearProgressBar();
        }

        private async void Modify()
        {
            string version = _VersionDropdown.value;
            EditorUtility.DisplayProgressBar("Modifying package", $"Modifying to {_Data.Id}.{version}", 0);
            await PackageManager.Instance.Installer.ModifyPackage(_Data.Id, version, true);
            EditorUtility.ClearProgressBar();
        }

        private async void Uninstall()
        {
            EditorUtility.DisplayProgressBar("Removing package", $"Removing {_Data.Id}", 0);
            await PackageManager.Instance.Installer.RemovePackage(_Data.Id);
            EditorUtility.ClearProgressBar();
        }
        
        public PackageDetailsWidget(VisualElement detailsRoot)
        {
            Root = detailsRoot;

            _PackageLogo = Root.Q<VisualElement>("logo");
            _PackageName = Root.Q<Label>("name");
            _PackageAuthor = Root.Q<Label>("author");

            _DetailsPanel = Root.Q<VisualElement>("details");
            _Description = _DetailsPanel.Q<Label>("description");

            var controls = Root.Q<VisualElement>("buttonbar");
            _VersionDropdown = controls.Q<DropdownField>("version");
            _Install = controls.Q<Button>("install");
            _Modify = controls.Q<Button>("modify");
            _Uninstall = controls.Q<Button>("uninstall");

            _Install.clicked += Install;
            _Modify.clicked += Modify;
            _Uninstall.clicked += Uninstall;
            _VersionDropdown.RegisterValueChangedCallback(VersionDropdownChanged);
        }
    }
}