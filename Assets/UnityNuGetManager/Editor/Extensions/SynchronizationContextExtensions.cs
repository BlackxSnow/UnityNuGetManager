using System.Threading;
using UnityNuGetManager.Async;

namespace UnityNuGetManager.Extensions
{
    public static class SynchronizationContextExtensions
    {
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
        {
            return new SynchronizationContextAwaiter(context);
        }
    }
}