using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Version;

namespace UnityNuGetManager.Package.DependencyResolution
{
    public class DependencyTreeBuilder : IDisposable
    {
        private int _ActivePrepareTasks;
        public readonly CancellationTokenSource CancellationSource = new();
        private readonly BlockingCollection<DependencyNode> _PreparedNodes = new();

        private readonly IPackageAccessor _Accessor;

        private readonly Dictionary<string, DependencyNode> _ResolvedNodes = new();

        public async Task<DependencyNode> Build(IEnumerable<IPackageIdentifier> initialSet)
        {
            CancellationToken token = CancellationSource.Token;
            var rootNode = new DependencyNode();
            
            foreach (IPackageIdentifier id in initialSet)
            {
                var pending = new PendingNode(rootNode, new Dependency { Id = id.Id, Range = id.Version });
                StartPreparation(pending);
            }

            while (true)
            {
                DependencyNode node = await Task.Run(() => _PreparedNodes.Take(token), token);
                token.ThrowIfCancellationRequested();

                ProcessNode(node, token);
                if (IsFinished()) break;
            }

            return rootNode;
        }

        private bool IsFinished()
        {
            return _ActivePrepareTasks == 0 && !CancellationSource.IsCancellationRequested && _PreparedNodes.Count == 0;
        }
        
        private void StartPreparation(PendingNode toPrepare)
        {
            Interlocked.Increment(ref _ActivePrepareTasks);
            Task.Run(() => PrepareNode(toPrepare, CancellationSource.Token))
                .ContinueWith(t => PreparationFinished(t, CancellationSource.Token));
        }

        private void PreparationFinished(Task<DependencyNode> prepared, CancellationToken token)
        {
            if (prepared.IsFaulted)
            {
                CancellationSource.Cancel();
                return;
            }
            if (token.IsCancellationRequested) return;
                
            _PreparedNodes.Add(prepared.Result, token);
            Interlocked.Decrement(ref _ActivePrepareTasks);
        }

        private async Task<DependencyNode> PrepareNode(PendingNode node, CancellationToken token)
        {
            RegistrationsReponse registrations =
                await _Accessor.GetRegistrationsDirect(node.DependencyEntry.Registration) ??
                (await _Accessor.GetRegistrations(node.DependencyEntry.Id))?.Result;

            if (registrations == null)
            {
                CancellationSource.Cancel();
                throw new InvalidDataException($"Unable to retrieve registrations data for {node.DependencyEntry.Id}.");
            }
// TODO: fix version is minumum
            var processedNode = new DependencyNode(node.Parent, node.DependencyEntry.Id,
                NugetSemanticVersion.ParseRange(node.DependencyEntry.Range), true, registrations);
            return processedNode;
        }

        private void ProcessNode(DependencyNode node, CancellationToken token)
        {
            node.SelectedEntry = GetBestVersion(node);
            if (_ResolvedNodes.TryGetValue(node.Id, out DependencyNode previousNode))
            {
                if (ResolveNodeConflict(previousNode, node) == previousNode) return;
            }
            else _ResolvedNodes.Add(node.Id, node);
            
            QueueDependenciesForPreparation(node);
        }

        private void QueueDependenciesForPreparation(DependencyNode node)
        {
            DependencyGroup[] groups = node.SelectedEntry.Entry.DependencyGroups;
            VersionData<DependencyGroup> selectedGroup = VersionResolver.GetBestDotNetVersion(groups);
            if (selectedGroup.SelectedTarget.Dependencies == null) return;
            Dependency[] dependencies = selectedGroup.SelectedTarget.Dependencies;
            foreach (PendingNode pendingNode in dependencies.Select(d => new PendingNode(node, d)))
            {
                StartPreparation(pendingNode);                
            }
        }
        
        private DependencyNode ResolveNodeConflict(DependencyNode oldest, DependencyNode newest)
        {
            if (newest.SelectedEntry.Version > oldest.SelectedEntry.Version)
            {
                ReplaceNode(oldest, newest);
                return newest;
            }
            ReplaceNode(newest, oldest);
            return oldest;
        }
        
        private void ReplaceNode(DependencyNode old, DependencyNode replacement)
        {
            Debug.Assert(old.Id == replacement.Id);
            foreach (DependencyNode parentNode in old.Parents)
            {
                parentNode.Dependencies[old.Id] = replacement;
                replacement.Parents.Add(parentNode);
            }

            _ResolvedNodes[old.Id] = replacement;
        }

        /// <summary>
        /// Find the lowest common version of a dependency.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static VersionedCatalogEntry GetBestVersion(DependencyNode node)
        {
            CatalogEntry bestEntry = null;
            NugetSemanticVersion bestVersion = NugetSemanticVersion.Invalid;
            
            foreach (RegistrationPage page in node.Registrations.Items)
            {
                NugetSemanticVersion lower = NugetSemanticVersion.Parse(page.Lower);
                NugetSemanticVersion upper = NugetSemanticVersion.Parse(page.Upper);
                if (node.TargetVersion < lower || node.TargetVersion > upper) continue;
                
                foreach (RegistrationPageLeaf leaf in page.Items)
                {
                    NugetSemanticVersion leafVersion = NugetSemanticVersion.Parse(leaf.CatalogEntry.Version);
                    if (leafVersion < node.TargetVersion) continue;

                    if (!node.VersionIsMinimum)
                    {
                        if (leafVersion == node.TargetVersion)
                            return new VersionedCatalogEntry(leaf.CatalogEntry, leafVersion);
                        continue;
                    }

                    if (bestEntry == null)
                    {
                        bestEntry = leaf.CatalogEntry;
                        bestVersion = leafVersion;
                        continue;
                    }

                    if (leafVersion >= bestVersion) continue;
                    bestEntry = leaf.CatalogEntry;
                    bestVersion = leafVersion;
                }
            }

            return new VersionedCatalogEntry(bestEntry, bestVersion);
        }
        
        public void Dispose()
        {
            _PreparedNodes.Dispose();
        }

        public DependencyTreeBuilder(IPackageAccessor accessor)
        {
            _Accessor = accessor;
        }
    }
}