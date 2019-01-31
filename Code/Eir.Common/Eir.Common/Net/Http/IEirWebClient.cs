using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eir.Common.Net.Http
{
    public interface IEirWebClient : IDisposable
    {
        bool UseHttpLog { get; set; }

        string UserAgent { get; set; }

        Encoding Encoding { get; set; }

        ICredentials Credentials { get; set; }

        string DownloadString(Uri uri);

        string UploadString(Uri uri, HttpMethod method, string data);

        byte[] UploadData(Uri uri, HttpMethod method, byte[] data);

        Task<string> DownloadStringAsync(Uri uri, CancellationToken ct);

        Task<string> UploadStringAsync(Uri uri, HttpMethod method, string data, CancellationToken ct);

        Task<byte[]> UploadDataAsync(Uri uri, HttpMethod method, byte[] data, CancellationToken ct);
    }
}
