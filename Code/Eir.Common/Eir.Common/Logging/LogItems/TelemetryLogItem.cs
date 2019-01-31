using System;
using Eir.Common.Extensions;

namespace Eir.Common.Logging
{
    public class TelemetryLogItem : LogItem
    {
        public static readonly TelemetryLogItem Header = new TelemetryLogItem();

        private TelemetryLogItem()
            : base("Timestamp", "LogLevel", "Message")
        {
        }

        public TelemetryLogItem(DateTime timestamp, LogLevel logLevel, string message, string callerFilePath)
            : base(timestamp.ToString("O"), logLevel.ToString(), message.ReplaceControlChars(), callerFilePath)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            Message = message;
            CallerFilePath = callerFilePath;
        }

        public override DateTime Timestamp { get; }

        public override LogLevel LogLevel { get; }

        public string Message { get; }

        public string CallerFilePath { get; }
    }
}