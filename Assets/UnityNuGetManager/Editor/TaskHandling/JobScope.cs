using System;

namespace UnityNuGetManager.TaskHandling
{
    public class JobScope<T> : IJobScope<T>
    {
        public void Dispose()
        {
            Disposed?.Invoke(this);
        }

        public event Action<IJobScope<T>, IJobScope<T>> ScopeCreated;
        public event Action<IJobScope<T>, T> ProgressReported;
        public event Action<IJobScope<T>> Disposed;
        public string Name { get; }
        
        public IJobScope<T> CreateSubScope(string name)
        {
            var scope = new JobScope<T>(name);
            ScopeCreated?.Invoke(this, scope);
            return scope;
        }

        public void ReportProgress(T progress)
        {
            ProgressReported?.Invoke(this, progress);
        }

        public JobScope(string name)
        {
            Name = name;
        }
    }
}