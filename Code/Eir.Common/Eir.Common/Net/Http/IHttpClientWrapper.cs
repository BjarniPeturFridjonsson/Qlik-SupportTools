using System.Net.Http;
using System.Threading.Tasks;

namespace Eir.Common.Net.Http
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> Post(string url, string stringData);
    }
}
