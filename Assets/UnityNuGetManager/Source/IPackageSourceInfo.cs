using System;
using System.Threading;
using UnityNuGetManager.Config.Data;

namespace UnityNuGetManager.Source
{
    public interface IPackageSourceInfo : IDisposable
    {
        public PackageSourceDetails SourceDetails { get; }
        public PackageSourceCredentials Credentials { get; }

        public ManualResetEvent Initialised { get; }
        public CancellationToken DisposedToken { get; }
        public bool IsDisposed { get; }
        public string QueryUrl { get; }
        public string RegistrationsUrl { get; }
        public string BaseAddress { get; }
    }
}