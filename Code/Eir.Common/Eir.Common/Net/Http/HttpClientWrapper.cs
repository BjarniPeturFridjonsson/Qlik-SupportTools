using System.Net.Http;
using System.Threading.Tasks;

namespace Eir.Common.Net.Http
{
    /// <summary>
    /// HttpClient according to Microsoft should be static.
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private static readonly HttpClientWrapper _instance = new HttpClientWrapper();

        public static HttpClientWrapper Instance => _instance;

        public Task<HttpResponseMessage> Post(string url, string stringData)
        {
            var a = new HttpClient();
            return a.PostAsync(url, new StringContent(stringData));
        }
    }
}
