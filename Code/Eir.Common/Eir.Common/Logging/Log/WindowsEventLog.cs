using System;
using System.Diagnostics;
using Eir.Common.Extensions;

namespace Eir.Common.Logging
{
    public class WindowsEventLog : IWindowsEventLog
    {
        private readonly string _eventLogCategoryName;

        public WindowsEventLog(string eventLogCategoryName) 
        {
            _eventLogCategoryName = eventLogCategoryName;
        }

        public void Info(string text)
        {
            AddToEventLog(text, EventLogEntryType.Information);
        }

        public void Error(string text, string memberName, string filePath, int lineNumber)
        {
            AddToEventLog(
                Compose(text, memberName, filePath, lineNumber),
                EventLogEntryType.Error);
        }

        public void Error(Exception exception, string memberName, string filePath, int lineNumber)
        {
            AddToEventLog(
                Compose(exception.ToLogLine(false), memberName, filePath, lineNumber),
                EventLogEntryType.Error);
        }

        public void Error(string text, Exception exception, string memberName, string filePath, int lineNumber)
        {
            AddToEventLog(
                Compose(text + "\r\n" + exception.ToLogLine(false), memberName, filePath, lineNumber),
                EventLogEntryType.Error);
        }

        private static string Compose(string text, string memberName, string filePath, int lineNumber)
        {
            return string.Join("\r\n", text, "Method: " + memberName, "File: " + filePath, "Line: " + lineNumber);
        }

        private void AddToEventLog(string text, EventLogEntryType type)
        {
            try
            {
                EventLog.WriteEntry(_eventLogCategoryName, text, type);
            }
            catch
            {
                try
                {
                    // If this also fails, well, then the process should fail
                    EventLog.WriteEntry("Application", text, type);
                }
                catch
                {
                    // Give up...
                }
            }
        }
    }
}