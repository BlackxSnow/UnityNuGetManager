using System.IO;
using System.Net;

namespace UnityNuGetManager
{
    public struct DownloadResult
    {
        public bool Succeeded { get; }
        public Stream Data { get; }
        public HttpStatusCode StatusCode { get; }

        public DownloadResult(bool succeeded, Stream data, HttpStatusCode statusCode)
        {
            Succeeded = succeeded;
            Data = data;
            StatusCode = statusCode;
        }
    }
}