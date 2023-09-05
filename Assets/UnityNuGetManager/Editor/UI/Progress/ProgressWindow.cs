using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityNuGetManager.Extensions;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.UI.Progress
{
    public class ProgressWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _TreeAsset;

        private TreeView _TaskTree;
        private Button _Cancel;

        private List<TreeViewItemData<string>> _Roots;
        private CancellationTokenSource _CurrentCancellationSource;
        private int _NextId;
        private ConcurrentDictionary<IJobScope<string>, int> _ScopeIds;

        public CancellationToken AssignTask(IJobScope<string> scope, params CancellationToken[] linkedTokens)
        {
            _CurrentCancellationSource?.Cancel();
            _NextId = 0;
            _CurrentCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(linkedTokens);
            _CurrentCancellationSource.Token.Register(OnJobCancelled);
            
            _Roots = new List<TreeViewItemData<string>> { new(_NextId++, scope.Name) };
            _ScopeIds = new ConcurrentDictionary<IJobScope<string>, int>(); 
            _ScopeIds.TryAdd(scope, 0);
            RegisterCallbacks(scope);

            _TaskTree.makeItem = () => new Label();
            _TaskTree.bindItem = (element, i) => ((Label)element).text = _TaskTree.GetItemDataForIndex<string>(i);
            _TaskTree.SetRootItems(_Roots);

            return _CurrentCancellationSource.Token;
        }

        private async void OnScopeCreated(IJobScope<string> source, IJobScope<string> created)
        {
            if (!_ScopeIds.TryGetValue(source, out int sourceId)) throw new Exception();

            await PackageManager.UnitySyncContext;
            var item = new TreeViewItemData<string>(_NextId++, created.Name);
            _ScopeIds.TryAdd(created, item.id);
            _TaskTree.AddItem(item, sourceId);
            RegisterCallbacks(created);
        }

        private async void OnScopeDisposed(IJobScope<string> disposed)
        {
            if (!_ScopeIds.TryRemove(disposed, out int id)) throw new Exception();
            
            await PackageManager.UnitySyncContext;
            if (id == 0)
            {
                OnJobCompleted();
                return;
            }
            _TaskTree.TryRemoveItem(id);
        }

        private void OnProgressReport(IJobScope<string> source, string report)
        {
            
        }
        
        private void RegisterCallbacks(IJobScope<string> scope)
        {
            scope.ScopeCreated += OnScopeCreated;
            scope.Disposed += OnScopeDisposed;
            scope.ProgressReported += OnProgressReport;
        }
        
        private void Cancel()
        {
            _CurrentCancellationSource?.Cancel();
        }

        private void OnJobCancelled()
        {
            Debug.Log("Job was cancelled.");
            Close();
        }

        private void OnJobCompleted()
        {
            Debug.Log("Job completed");
            _CurrentCancellationSource.Dispose();
            Close();
        }
        
        private void CreateGUI()
        {
            _TreeAsset.CloneTree(rootVisualElement);

            _TaskTree = rootVisualElement.Q<TreeView>("task-tree");
            _Cancel = rootVisualElement.Q<Button>("cancel");
            _Cancel.clicked += Cancel;
        }
    }
}