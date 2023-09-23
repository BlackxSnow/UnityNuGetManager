using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityNuGetManager.Extensions;
using UnityNuGetManager.Http;
using UnityNuGetManager.Source;
using UnityNuGetManager.TaskHandling;

namespace UnityNuGetManager.NuGetApi
{
    public class NugetApiClient : INugetApiClient
    {
        private readonly HttpClient _Client;

        private static void ThrowIfSourceDisposed(IPackageSourceInfo source)
        {
            if (source.IsDisposed) throw new ObjectDisposedException(nameof(PackageSourceInfo));
        }
        
        public async Task<QueryResponse> QueryPackages(IPackageSourceInfo source, string query, TaskContext context, bool throwOnFail = true)
        {
            ThrowIfSourceDisposed(source);
            await source.Initialised.AsTask(source.DisposedToken);
            
            var queryTarget = $"{source.QueryUrl}?semVerLevel=2.0.0&q={query}";
            return await GetAsObject<QueryResponse>(source, queryTarget, context, throwOnFail);
        }

        public async Task<RegistrationsReponse> GetRegistrations(IPackageSourceInfo source, string id, TaskContext context, bool throwOnFail = true)
        {
            ThrowIfSourceDisposed(source);
            await source.Initialised.AsTask(source.DisposedToken);

            string registrationTarget = Path.Combine(source.RegistrationsUrl, id.ToLower(), "index.json");
            return await GetAsObject<RegistrationsReponse>(source, registrationTarget, context, throwOnFail);
        }

        public async Task<RegistrationsReponse> GetRegistrations(string url, TaskContext context, bool throwOnFail = true)
        {
            if (url == null) return throwOnFail ? throw new ArgumentNullException() : null;
            return await GetAsObject<RegistrationsReponse>(url, context);
        }
        
        public async Task<DownloadResult> TryDownloadPackage(IPackageSourceInfo source, string id, string version, TaskContext context)
        {
            ThrowIfSourceDisposed(source);
            await source.Initialised.AsTask(source.DisposedToken);
            
            string downloadAddress = Path.Combine(source.BaseAddress, id, version, $"{id}.{version}.nupkg");
            HttpRequestMessage request = BuildRequest(source, HttpMethod.Get, downloadAddress);
            HttpResponseMessage response =
                await _Client.SendAsync(request, context.Token);
            return response.IsSuccessStatusCode
                ? new DownloadResult(true, await response.Content.ReadAsStreamAsync(), response.StatusCode)
                : new DownloadResult(false, null, response.StatusCode);
        }

        public async Task<Stream> DownloadPackage(IPackageSourceInfo source, string id, string version, TaskContext context)
        {
            DownloadResult result = await TryDownloadPackage(source, id, version, context);
            return result.Succeeded
                ? result.Data
                : throw new HttpRequestException(
                    $"Download of {id}.{version} failed with status {result.StatusCode}.");
        }

        private async Task<T> GetAsObject<T>(IPackageSourceInfo source, string url, TaskContext context, bool throwOnFail = true)
        {
            HttpRequestMessage request = BuildRequest(source, HttpMethod.Get, url);
            HttpResponseMessage response = await _Client.SendAsync(request, context.Token);
            if (!response.IsSuccessStatusCode)
            {
                return throwOnFail
                    ? throw new HttpRequestException(
                        $"GET of resource {typeof(T)} from source {source.SourceDetails.Name} failed with status code {response.StatusCode}. Url: {url}")
                    : default(T);
            }

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
        private async Task<T> GetAsObject<T>(string url, TaskContext context, bool throwOnFail = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _Client.SendAsync(request, context.Token);
            if (!response.IsSuccessStatusCode)
            {
                return throwOnFail
                    ? throw new HttpRequestException(
                        $"GET of resource {typeof(T)} failed with status code {response.StatusCode}. Url: {url}")
                    : default(T);
            }

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private static HttpRequestMessage BuildRequest(IPackageSourceInfo source, HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            
            if (source.Credentials == null) return request;
            
            var credentialsValue = $"{source.Credentials.Username}:{source.Credentials.ClearTextPassword}";
            string credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentialsValue));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64);

            return request;
        }
        
        public NugetApiClient()
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            _Client = new HttpClient(handler);
        }
    }
}