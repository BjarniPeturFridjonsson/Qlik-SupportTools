using System;

namespace FreyrCommon.Models
{
    public class EventLogEntryShort
    {
        public string Level { get; set; }
        public DateTime Logged { get; set; }
        public string Source { get; set; }
        public long InstanceId { get; set; }
        public string Message { get; set; }
        public string LogName { get; set; }
        public string User { get; set; }

    }
}
