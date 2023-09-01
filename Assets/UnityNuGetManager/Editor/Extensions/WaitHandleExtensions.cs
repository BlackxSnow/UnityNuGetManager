using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityNuGetManager.Extensions
{
    public static class WaitHandleExtensions
    {
        public static Task<bool> AsTask(this WaitHandle handle) =>
            AsTask(handle, Timeout.InfiniteTimeSpan, CancellationToken.None);
        public static Task<bool> AsTask(this WaitHandle handle, CancellationToken token) =>
            AsTask(handle, Timeout.InfiniteTimeSpan, token);
        public static Task<bool> AsTask(this WaitHandle handle, TimeSpan timeout) =>
            AsTask(handle, timeout, CancellationToken.None);
        public static Task<bool> AsTask(this WaitHandle handle, TimeSpan timeout, CancellationToken token)
        {
            bool isSignalled = handle.WaitOne(0);
            if (isSignalled) return Task.FromResult(true);
            if (timeout == TimeSpan.Zero) return Task.FromResult(false);
            if (token.IsCancellationRequested) return Task.FromCanceled<bool>(token);

            return AsTaskAsync(handle, timeout, token);
        }

        private static async Task<bool> AsTaskAsync(WaitHandle handle, TimeSpan timeout, CancellationToken token)
        {
            var completionSource = new TaskCompletionSource<bool>();
            using (new ThreadPoolRegistration(handle, timeout, completionSource))
            {
                await using (token.Register(state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                                 completionSource, false))
                {
                    return await completionSource.Task.ConfigureAwait(false);
                }
            }
        }
        
        private sealed class ThreadPoolRegistration : IDisposable
        {
            private readonly RegisteredWaitHandle _RegisteredHandle;

            public ThreadPoolRegistration(WaitHandle handle, TimeSpan timeout,
                TaskCompletionSource<bool> completionSource)
            {
                _RegisteredHandle = ThreadPool.RegisterWaitForSingleObject(handle,
                    (state, timedout) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedout),
                    completionSource, timeout, true);
            }

            public void Dispose() => _RegisteredHandle.Unregister(null);
        }
    }
}