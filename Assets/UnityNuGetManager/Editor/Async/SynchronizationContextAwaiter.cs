using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnityNuGetManager.Async
{
    public class SynchronizationContextAwaiter : INotifyCompletion
    {
        private static readonly SendOrPostCallback _Callback = state => ((Action)state)();
        private readonly SynchronizationContext _Context;

        public bool IsCompleted => _Context == SynchronizationContext.Current;
        
        public void OnCompleted(Action continuation) => _Context.Post(_Callback, continuation);

        public void GetResult()
        {
            
        }

        public SynchronizationContextAwaiter(SynchronizationContext context)
        {
            _Context = context;
        }
    }
}