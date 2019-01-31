using System;
using System.Diagnostics;
using System.Linq;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using Newtonsoft.Json;

namespace FreyrCollectorCommon.Collectors
{
    public class WindowsEventLogs
    {
        private long _errCount = 0;
        private readonly ILogger _logger;

        public WindowsEventLogs(ILogger logger)
        {
            _logger = logger;
        }

        public void ReadEvents(DateTime fromTimestamp, DateTime toTimestamp, Common.CollectorHelper collectorHelper)
        {
            string[] eventNames = "Application,System,Security".Split(',');
            //string[] sourceFilters = "QlikSense...."
            string[] levelFilters = new string[0];// "Warning,Error".Split(',');//
            var newFrom = fromTimestamp.AddDays(-2);
            var name = collectorHelper.CreateUniqueFileName("WindowsEventLogs");
            bool firstLine = true;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(name))
            {
                file.WriteLine("[");
                foreach (string eventName in eventNames)
                {
                    using (var eventlog = new EventLog(eventName))
                    {
                        foreach (EventLogEntry item in eventlog.Entries)
                        {
                            if ((item.TimeGenerated >= newFrom) &&
                                (item.TimeGenerated < toTimestamp) &&
                                ((levelFilters.Length == 0) || levelFilters.Any(t => t.Equals(item.EntryType.ToString(), StringComparison.OrdinalIgnoreCase))))
                            {
                                try
                                {
                                    var a = new EventLogEntryShort
                                    {
                                        InstanceId = item.InstanceId,
                                        Level = item.EntryType.ToString(),
                                        Logged = item.TimeGenerated,
                                        Source = item.Source,
                                        Message = item.Message,
                                        LogName = eventName,
                                        User = item.UserName

                                    };
                                    file.WriteLine((firstLine ? "": "," ) + JsonConvert.SerializeObject(a)); //todo: this is just a temp hack to adust to the fact we have out of memory errors in the old version bfr - 2018-12-19.
                                }
                                catch (Exception ex)
                                {
                                    _errCount++;
                                    Trace.WriteLine(ex);
                                }
                                firstLine = false;
                            }
                        }
                    }
                    if (_errCount > 0)
                    {
                        _logger.Add($"Windows Event log {eventName} failed reading {_errCount} events");
                        _errCount = 0;
                    }
                }
                file.WriteLine("]");
            }
        }
    }
}
