using System;

namespace UnityNuGetManager.TaskHandling
{
    public interface IJobScope<T> : IDisposable
    {
        event Action<IJobScope<T>, IJobScope<T>> ScopeCreated;
        event Action<IJobScope<T>, T> ProgressReported;
        event Action<IJobScope<T>> Disposed;
        
        string Name { get; }
        
        IJobScope<T> CreateSubScope(string name);
        void ReportProgress(T progress);
    }
}