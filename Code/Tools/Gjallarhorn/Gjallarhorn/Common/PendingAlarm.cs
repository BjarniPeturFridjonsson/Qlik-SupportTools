using System;
using System.Text;

namespace Gjallarhorn.Common
{
    public class PendingAlarm
    {
        public DateTime ThresholdTime { get; }
        public StringBuilder StringBuilder { get; }

        public PendingAlarm(DateTime thresholdTime)
        {
            ThresholdTime = thresholdTime;
            StringBuilder = new StringBuilder();
        }
    }
}
