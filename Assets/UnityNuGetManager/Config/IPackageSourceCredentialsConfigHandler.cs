using System;
using System.Collections.Generic;
using System.Threading;
using UnityNuGetManager.Config.Data;

namespace UnityNuGetManager.Config
{
    public interface IPackageSourceCredentialsConfigHandler
    {
        event Action<PackageSourceCredentials> CredentialsAdded;
        event Action<PackageSourceCredentials> CredentialsRemoved;
        event Action<ICollection<PackageSourceCredentials>> CredentialsReloaded;
        ManualResetEvent ReloadCompleted { get; }

        PackageSourceCredentials AddCredentials(string source, string userName, string password);
        void RemoveCredentials(string source);
        ICollection<PackageSourceCredentials> GetAllCredentials();
        PackageSourceCredentials GetCredentials(string source);
    }
}