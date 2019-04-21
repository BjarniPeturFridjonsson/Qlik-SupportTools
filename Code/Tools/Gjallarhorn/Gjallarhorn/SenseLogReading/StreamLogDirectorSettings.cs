using System;

namespace Gjallarhorn.SenseLogReading
{
    public class StreamLogDirectorSettings
    {
        public String OutputFolderPath { get; set; }
        public DateTime StartDateForLogs { get; set; }
        public DateTime StopDateForLogs { get; set; }
    }
}
