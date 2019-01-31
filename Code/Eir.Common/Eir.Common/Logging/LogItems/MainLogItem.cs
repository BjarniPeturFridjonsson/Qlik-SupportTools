using System;
using Eir.Common.Extensions;

namespace Eir.Common.Logging
{
    public class MainLogItem : LogItem
    {
        public static readonly MainLogItem Header = new MainLogItem();

        private MainLogItem()
            : base(
                  "Timestamp",
                  "LogLevel",
                  "Thread",
                  "File path",
                  "Line",
                  "Method",
                  "Message",
                  "Exception")
        {
        }

        public MainLogItem(
            DateTime timestamp,
            LogLevel logLevel,
            string threadName,
            string filePath,
            string line,
            string method,
            string message,
            Exception exception)
            : base(
                  timestamp.ToString("O"),
                  logLevel.ToString(),
                  threadName,
                  filePath,
                  line,
                  method,
                  message.ReplaceControlChars(),
                  exception?.ToLogLine() ?? string.Empty)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            ThreadName = threadName;
            FilePath = filePath;
            Line = line;
            Method = method;
            Message = message;
            Exception = exception;
        }

        public override DateTime Timestamp { get; }

        public override LogLevel LogLevel { get; }

        public string ThreadName { get; }

        public string FilePath { get; }

        public string Line { get; }

        public string Method { get; }

        public string Message { get; }

        public Exception Exception { get; }
    }
}