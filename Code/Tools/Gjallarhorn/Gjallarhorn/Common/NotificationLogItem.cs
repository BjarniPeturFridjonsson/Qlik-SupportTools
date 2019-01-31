using System;
using Eir.Common.Logging;

namespace Gjallarhorn.Common
{
    public class NotificationLogItem : LogItem
    {
        public static readonly NotificationLogItem Header = new NotificationLogItem();
        public const string NotificationLogName = "NotificationLog";

        private NotificationLogItem()
            : base("Timestamp", "LogLevel", "MonitorName", "Message")
        {
        }
        public NotificationLogItem(
            DateTime timestamp,
            LogLevel logLevel,
            string monitorName,
             string message
       )
            : base(
                timestamp.ToString("O"),
                logLevel.ToString(),
                monitorName,
                message)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            MonitorName = monitorName;

            Message = message;
        }

        public override DateTime Timestamp { get; }
        public override LogLevel LogLevel { get; }
        public string MonitorName { get; }
        public string Message { get; }

    }
}
