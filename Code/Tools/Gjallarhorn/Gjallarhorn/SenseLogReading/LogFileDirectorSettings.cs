using System;

namespace Gjallarhorn.SenseLogReading
{
    public class LogFileDirectorSettings
    {
        public String OutputFolderPath { get; set; }
        public DateTime StartDateForLogs { get; set; }
        public DateTime StopDateForLogs { get; set; }
    }
}
