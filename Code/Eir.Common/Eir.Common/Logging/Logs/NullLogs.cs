using System;
using System.Collections.Generic;
using Eir.Common.Net.Http;

namespace Eir.Common.Logging
{
    public class NullLogs : ILogs
    {
        public static readonly NullLogs Instance = new NullLogs();

        private class NullMainLog : IMainLog
        {
#pragma warning disable 67 // "...not used"
            public event EventHandler<MainLogItemEventArgs> NewLogItem;
#pragma warning restore 67

            public void Add(string message, LogLevel logLevel, string memberName, string filePath, int lineNumber)
            {
            }

            public void AddException(string text, Exception ex, LogLevel loglevel, string memberName, string filePath, int lineNumber)
            {
            }
        }

        private class NullHttpLog : IHttpLog
        {
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
            }
        }

        private class NullWindowsEventLog : IWindowsEventLog
        {
            public void Info(string text)
            {
            }

            public void Error(string text, string memberName, string filePath, int lineNumber)
            {
            }

            public void Error(Exception exception, string memberName, string filePath, int lineNumber)
            {
            }

            public void Error(string text, Exception exception, string memberName, string filePath, int lineNumber)
            {
            }
        }

        private class NullDumpLog : IDumpLog
        {
            public void TextBlob(string data, string filename)
            {
            }
        }

        private class NullTelemetry : ITelemetry
        {
            public void Add(Func<string> getMessage, string callerFilePath = null)
            {
            }
        }

        private NullLogs()
        {
            Main = new NullMainLog();
            Http = new NullHttpLog();
            WindowsEvent = new NullWindowsEventLog();
            Dump = new NullDumpLog();
            Telemetry = new NullTelemetry();
        }

        public void Dispose()
        {
        }

        public IMainLog Main { get; }

        public IHttpLog Http { get; }

        public IWindowsEventLog WindowsEvent { get; }

        public IDumpLog Dump { get; }

        public ITelemetry Telemetry { get; }
        public void AddToDynamicLog<T>(T logItem) where T : LogItem { }
        public void InitializeDynamicLog<T>(string logName, T headerLine) where T : LogItem { }

    }
}