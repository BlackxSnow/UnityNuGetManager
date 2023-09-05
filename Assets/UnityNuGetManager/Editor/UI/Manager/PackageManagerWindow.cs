using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityNuGetManager.Http;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;
using PackageInfo = UnityNuGetManager.Package.PackageInfo;

namespace UnityNuGetManager.UI.Manager
{
    public class PackageManagerWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _WindowTreeAsset;

        private TextField _QueryField;
        private ListView _PackageListView;
        private PackageDetailsWidget _DetailsWidget;

        private List<PackageInfo> _Packages;

        private bool _IsRefreshing;

        [SerializeField, HideInInspector] private string _LastQuery;

        internal const string WindowDataKey = PackageManager.PackagePrefx + ".packagemanagerwindow";

        [MenuItem("NuGet/Package Manager")]
        private static void CreateWindow()
        {
            var window = GetWindow<PackageManagerWindow>();
            window.titleContent = new GUIContent("NuGet Package Manager");
        }

        private void OnEnable()
        {
            string data = EditorPrefs.GetString(WindowDataKey);
            if (string.IsNullOrWhiteSpace(data)) return;
            JsonUtility.FromJsonOverwrite(data, this);
        }

        private void OnDisable()
        {
            string data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(WindowDataKey, data);
        }

        public void CreateGUI()
        {
            _WindowTreeAsset.CloneTree(rootVisualElement);

            _QueryField = rootVisualElement.Q<TextField>("query");
            _PackageListView = rootVisualElement.Q<ListView>("packages");
            _DetailsWidget = new PackageDetailsWidget(rootVisualElement.Q("detailspanel"));

            if (_Packages == null)
            {
                var context = new TaskContext(new JobScope<string>("Refreshing packages"), new CancellationToken());
                
                RefreshPackages(context, _LastQuery);
            }

            _PackageListView.makeItem = () => new PackageInfoWidget();
            _PackageListView.bindItem = (element, index) =>
            {
                PackageInfo item = _Packages[index];
                var infoWidget = (PackageInfoWidget)element;
                infoWidget.SetData(item);
            };
            _PackageListView.itemsSource = _Packages;
            _PackageListView.selectionChanged += OnPackageSelected;

            _QueryField.RegisterCallback<KeyUpEvent>(OnQuerySubmit);

            rootVisualElement.Q<LoadingSpinner>().SetEnabled(false);
        }

        private void OnQuerySubmit(KeyUpEvent keyUp)
        {
            if (keyUp.keyCode != KeyCode.Return) return;
            var context = new TaskContext(new JobScope<string>("Refreshing packages"), new CancellationToken());
            _LastQuery = _QueryField.value;
            RefreshPackages(context, _LastQuery);
        }
        
        private void OnPackageSelected(IEnumerable<object> selections)
        {
            var selectedInfo = (PackageInfo)_PackageListView.selectedItem;
            _DetailsWidget.SetData(selectedInfo);
        }

        private async void RefreshPackages(TaskContext context, string query = "")
        {
            _IsRefreshing = true;
            _DetailsWidget.SetData(null);
            _QueryField.SetEnabled(false);
            
            if (_Packages == null) _Packages = new List<PackageInfo>();
            else _Packages.Clear();
            
            var spinner = rootVisualElement.Q<LoadingSpinner>();
            var packagesList = rootVisualElement.Q<ListView>("packages");
            
            packagesList.style.display = DisplayStyle.None;
            spinner.Enable();

            await GetPackages(context, query);
            
            packagesList.style.display = DisplayStyle.Flex;
            spinner.Disable();
            _PackageListView.RefreshItems();
            _QueryField.SetEnabled(true);
            _IsRefreshing = false;
        }
        
        private async Task GetPackages(TaskContext context, string query = "")
        {
            Dictionary<IPackageSourceInfo, QueryResponse> responses =
                await PackageManager.Instance.Accessor.QueryPackages(query, context);

            _Packages.AddRange(responses.SelectMany(pair =>
            {
                return pair.Value.Data.Select(data => new PackageInfo(data, pair.Key));
            }));
        }
    }
}