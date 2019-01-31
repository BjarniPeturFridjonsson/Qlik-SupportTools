using System;
using System.Collections.Generic;
using Eir.Common.Net.Http;

namespace Eir.Common.Logging
{
    public class ConsoleLogs : ILogs
    {
        public static readonly ConsoleLogs Instance = new ConsoleLogs();

        private class ConsoleMainLog : IMainLog
        {
#pragma warning disable 67 // "...not used"
            public event EventHandler<MainLogItemEventArgs> NewLogItem;
#pragma warning restore 67

            public void Add(string message, LogLevel logLevel, string memberName, string filePath, int lineNumber)
            {
                Console.WriteLine(message);
            }

            public void AddException(string text, Exception ex, LogLevel loglevel, string memberName, string filePath, int lineNumber)
            {
                Console.WriteLine(text);
                Console.WriteLine(ex);
            }
        }

        private class ConsoleHttpLog : IHttpLog
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

        private class ConsoleWindowsEventLog : IWindowsEventLog
        {
            public void Info(string text)
            {
                Console.WriteLine(text);
            }

            public void Error(string text, string memberName, string filePath, int lineNumber)
            {
                Console.WriteLine(text);
            }

            public void Error(Exception exception, string memberName, string filePath, int lineNumber)
            {
                Console.WriteLine(exception);
            }

            public void Error(string text, Exception exception, string memberName, string filePath, int lineNumber)
            {
                Console.WriteLine(text);
                Console.WriteLine(exception);
            }
        }

        private class ConsoleDumpLog : IDumpLog
        {
            public void TextBlob(string data, string filename)
            {
            }
        }

        private class ConsoleTelemetry : ITelemetry
        {
            public void Add(Func<string> getMessage, string callerFilePath = null)
            {
                Console.WriteLine(getMessage());
            }
        }

        private ConsoleLogs()
        {
            Main = new ConsoleMainLog();
            Http = new ConsoleHttpLog();
            WindowsEvent = new ConsoleWindowsEventLog();
            Dump = new ConsoleDumpLog();
            Telemetry = new ConsoleTelemetry();
            
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