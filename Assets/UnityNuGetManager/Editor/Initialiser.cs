using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityNuGetManager.Config;
using UnityNuGetManager.Http;
using UnityNuGetManager.NuGetApi;
using UnityNuGetManager.Package;
using UnityNuGetManager.Package.DependencyResolution;
using UnityNuGetManager.Source;

namespace UnityNuGetManager
{
    public static class Initialiser
    {
        // [InitializeOnLoadMethod]
        // public static async void Initialise()
        // {
        //     var config = new Configuration("Assets/NuGet.config");
        //     var packageSourceHandler = new PackageSourceConfigHandler(config);
        //     var credentialsHandler = new PackageSourceCredentialsConfigHandler(config);
        //     var packageInfoManager = new PackageSourceManager(packageSourceHandler, credentialsHandler);
        //
        //     var manifestHandler = new PackageManifestHandler("Assets/packages.config");
        //     var apiClient = new NugetApiClient();
        //     var packageDownloader = new PackageDownloader(apiClient, packageInfoManager);
        //     var cacheManager = new PackageCacheManager(packageDownloader, "Library/NuGetCache");
        //     var packageAccessor = new PackageAccessor(apiClient, packageInfoManager);
        //     var dependencyResolver = new DependencyResolver(packageAccessor);
        //     var packageInstaller = new PackageInstaller(manifestHandler, cacheManager, dependencyResolver, "Assets/Packages");
        //
        //     // await packageInstaller.RestorePackages();
        // }
    }
}