using System;

namespace Eir.Common.Logging
{
    public class LogFileNameComposer : ILogFileNameComposer
    {
        private const string FILENAME_TEMPLATE = "{0}-{1}-{2}{3}"; // "yyyyMMdd-APP-LOG.EXT"
        private const string ROLLED_FILENAME_TEMPLATE = "{0}-{1}-{2}{3}.{4}"; // "yyyyMMdd-APP-LOG.EXT.ROLL
        private const string FILENAME_DATE_FORMAT = "yyyyMMdd";

        public LogFileNameComposer(string applicationName, string logName, string fileExtension)
        {
            if (applicationName.Contains("-"))
            {
                throw new ArgumentException($"The {nameof(applicationName)} must not contain any '-' character!");
            }

            if (logName.Contains("-"))
            {
                throw new ArgumentException($"The {nameof(logName)} must not contain any '-' character!");
            }

            if (fileExtension.Contains("-"))
            {
                throw new ArgumentException($"The {nameof(fileExtension)} must not contain any '-' character!");
            }

            ApplicationName = applicationName;
            LogName = logName;
            FileExtension = fileExtension;
        }

        public string ApplicationName { get; }

        public string LogName { get; }

        public string FileExtension { get; }

        public string GetDatePart(DateTime timestamp)
        {
            return timestamp.ToString(FILENAME_DATE_FORMAT);
        }

        public string GetFileName(string datePart)
        {
            return string.Format(FILENAME_TEMPLATE, datePart, ApplicationName, LogName, FileExtension);
        }

        public string GetFileName(string datePart, string roll)
        {
            return string.Format(ROLLED_FILENAME_TEMPLATE, datePart, ApplicationName, LogName, FileExtension, roll);
        }

        public bool TryParse(string fileName, out string datePart, out string roll)
        {
            // 20010203-MyApp-MyLog.MyExt
            // 20010203-MyApp-MyLog.MyExt.1

            string[] parts1 = fileName.Split('-');
            // parts1[0] = 20010203
            // parts1[1] = MyApp
            // parts1[2] = MyLog.MyExt.1
            if (parts1.Length != 3)
            {
                datePart = roll = null;
                return false;
            }

            string[] parts2 = parts1[2].Split('.');
            // parts2[0] = MyLog
            // parts2[1] = MyExt
            // parts2[2] = 1     (Optional)
            if ((parts2.Length != 2) && (parts2.Length != 3))
            {
                datePart = roll = null;
                return false;
            }

            datePart = parts1[0];
            roll = parts2.Length == 3 ? parts2[2] : null;
            return true;
        }
    }
}