using Eir.Common.Net.Http;

namespace Eir.Common.Logging
{
    public interface IHttpLog
    {
        void Add(
            HttpMethod requestHttpMethod,
            string requestRawUrl,
            string requestUserAgent,
            string requestRemoteEndPointAddress,
            long responseTimeInMs,
            long requestContentLength,
            long responseLengthInBytes,
            int statusCode);
    }
}