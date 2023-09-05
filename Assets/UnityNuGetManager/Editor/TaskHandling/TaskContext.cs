using System;
using System.Threading;
using JetBrains.Annotations;

namespace UnityNuGetManager.TaskHandling
{
    public readonly struct TaskContext : IDisposable
    {
        [CanBeNull] public readonly IJobScope<string> Scope;
        public readonly CancellationToken Token;

        public TaskContext CreateSub(string subScopeName)
        {
            return new TaskContext(Scope?.CreateSubScope(subScopeName), Token);
        }
        
        public TaskContext([CanBeNull] IJobScope<string> scope, CancellationToken token)
        {
            Scope = scope;
            Token = token;
        }

        public void Dispose()
        {
            Scope?.Dispose();
        }
    }
}