using System;
using System.Threading;
using UnityNuGetManager.Config.Data;
using UnityNuGetManager.Source;

namespace Tests.Mock
{
    public class SourceInfoMock : IPackageSourceInfo
    {
        public PackageSourceDetails SourceDetails { get; set; }
        public PackageSourceCredentials Credentials { get; set; }
        public string QueryUrl { get; set; }
        public string RegistrationsUrl { get; set; }
        public string BaseAddress { get; set; }

        public override string ToString()
        {
            return $"{SourceDetails.Name} @ {SourceDetails.Url}";
        }

        public void Dispose() => throw new NotSupportedException();
        public ManualResetEvent Initialised { get; } = new ManualResetEvent(true);
        public CancellationToken DisposedToken => new CancellationToken(false);
        public bool IsDisposed { get; } = false;

    }
}