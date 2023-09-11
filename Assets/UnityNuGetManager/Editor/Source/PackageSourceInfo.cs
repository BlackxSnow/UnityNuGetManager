using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityNuGetManager.Config.Data;
using UnityNuGetManager.Http;

namespace UnityNuGetManager.Source
{
    public class PackageSourceInfo : IPackageSourceInfo
    {
        public PackageSourceDetails SourceDetails { get; }
        public PackageSourceCredentials Credentials { get; }

        public ManualResetEvent Initialised { get; private set; }
        public bool IsDisposed { get; private set; }
        private CancellationTokenSource _DisposedTokenSource = new CancellationTokenSource();
        public CancellationToken DisposedToken => _DisposedTokenSource.Token;
        public string QueryUrl { get; private set; }
        public string RegistrationsUrl { get; private set; }
        public string BaseAddress { get; private set; }

        public async Task InitialiseSource()
        {
            Initialised = new ManualResetEvent(false);
            const string queryType = "SearchQueryService";
            const string primaryRegistrationsType = "RegistrationsBaseUrl/3.6.0";
            const string secondaryRegistrationsType = "RegistrationsBaseUrl";
            const string baseAddressType = "PackageBaseAddress/3.0.0";

            try
            {
                using var client = new HttpClient();
                
                var request = new HttpRequestMessage(HttpMethod.Get, SourceDetails.Url);
                if (Credentials != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                        $"{Credentials.Username}:{Credentials.ClearTextPassword}");
                }

                HttpResponseMessage response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"Source initialisation for {SourceDetails.Name} @ {SourceDetails.Url} failed with status code {response.StatusCode}.");

                var indexResponse =
                    JsonConvert.DeserializeObject<IndexResponse>(await response.Content.ReadAsStringAsync());

                QueryUrl = indexResponse.Resources.First(res => res.Type == queryType).Id;
                RegistrationsUrl =
                    indexResponse.Resources.FirstOrDefault(res => res.Type == primaryRegistrationsType)?.Id ??
                    indexResponse.Resources.FirstOrDefault(res => res.Type == secondaryRegistrationsType)?.Id;
                BaseAddress = indexResponse.Resources.First(res => res.Type == baseAddressType).Id;


                if (QueryUrl == null || RegistrationsUrl == null) throw new InvalidDataException();
                Initialised.Set();
            }
            catch (Exception e)
            {
                Dispose();
                Debug.Log(e);
                throw;
            }
        }
    
        public PackageSourceInfo(PackageSourceDetails sourceDetails, PackageSourceCredentials credentials)
        {
            SourceDetails = sourceDetails;
            Credentials = credentials;
        }

        public void Dispose()
        {
            IsDisposed = true;
            _DisposedTokenSource.Cancel();
            Initialised?.Dispose();
        }
    }
}