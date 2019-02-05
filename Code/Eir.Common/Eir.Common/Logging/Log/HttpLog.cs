using System;
using Eir.Common.IO;
using Eir.Common.Net.Http;
using Eir.Common.Time;

namespace Eir.Common.Logging
{
    public class HttpLog : IHttpLog, IDisposable
    {
        private readonly FileWriterLogItemHandler<HttpLogItem> _fileWriterLogItemHandler;
        private readonly QueuedLogItemHandler<HttpLogItem> _queuedLogItemHandler;
        private readonly ITrigger _trigger;
         
        public HttpLog(string logDir, LogFileNameComposer logFileNameComposer, IFileSystem fileSystem)
        {
            _fileWriterLogItemHandler = new FileWriterLogItemHandler<HttpLogItem>(logDir, logFileNameComposer, fileSystem, HttpLogItem.Header);

            _trigger = new PauseTrigger(() => TimeSpan.FromSeconds(1));

            _queuedLogItemHandler = new QueuedLogItemHandler<HttpLogItem>(_fileWriterLogItemHandler, _trigger);
        }

        public void Dispose()
        {
            _trigger.Dispose();
            _queuedLogItemHandler.Dispose();
            _fileWriterLogItemHandler.Dispose();
        }

        public void Add(
            HttpMethod requestHttpMethod,
            string requestRawUrl,
            string requestUserAgent,
            string requestRemoteEndPointAddress,
            long responseTimeInMs,
            long requestContentLength,
            long responseLengthInBytes,
            int statusCode)
        {
            var logItem = new HttpLogItem(
                DateTime.UtcNow,
                LogLevel.Info,
                requestHttpMethod,
                requestRawUrl,
                requestUserAgent,
                requestRemoteEndPointAddress,
                statusCode,
                requestContentLength,
                responseLengthInBytes,
                responseTimeInMs);

            _queuedLogItemHandler.Add(logItem);
        }
    }
}