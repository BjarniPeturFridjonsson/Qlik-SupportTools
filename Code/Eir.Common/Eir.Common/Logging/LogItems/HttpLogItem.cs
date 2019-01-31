using System;
using Eir.Common.Net.Http;

namespace Eir.Common.Logging
{
    public class HttpLogItem : LogItem
    {
        public static readonly HttpLogItem Header = new HttpLogItem();

        private HttpLogItem()
            : base("Timestamp", "LogLevel", "Method", "Url", "Agent", "Ip", "ResponseTime", "RequestLength", "ResponseLength", "StatusCode")
        {
        }

        public HttpLogItem(
            DateTime timestamp,
            LogLevel logLevel,
            HttpMethod requestHttpMethod,
            string requestRawUrl,
            string requestUserAgent,
            string requestRemoteEndPointAddress,
            int statusCode,
            long requestContentLength,
            long responseLengthInBytes,
            long responseTimeInMs)
            : base(
                timestamp.ToString("O"),
                logLevel.ToString(),
                requestHttpMethod.ToString(),
                requestRawUrl,
                requestUserAgent,
                requestRemoteEndPointAddress,
                responseTimeInMs.ToString(),
                requestContentLength.ToString(),
                responseLengthInBytes.ToString(),
                statusCode.ToString())
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            RequestHttpMethod = requestHttpMethod;
            RequestRawUrl = requestRawUrl;
            RequestUserAgent = requestUserAgent;
            RequestRemoteEndPointAddress = requestRemoteEndPointAddress;
            StatusCode = statusCode;
            RequestContentLength = requestContentLength;
            ResponseLengthInBytes = responseLengthInBytes;
            ResponseTimeInMs = responseTimeInMs;
        }

        public override DateTime Timestamp { get; }

        public override LogLevel LogLevel { get; }

        public HttpMethod RequestHttpMethod { get; }

        public string RequestRawUrl { get; }

        public string RequestUserAgent { get; }

        public string RequestRemoteEndPointAddress { get; }

        public int StatusCode { get; }
        public long ResponseLengthInBytes { get; }
        public long RequestContentLength { get; }

        public long ResponseTimeInMs { get; }
    }
}