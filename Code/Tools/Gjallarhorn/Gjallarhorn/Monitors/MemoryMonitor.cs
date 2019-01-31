using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Monitors
{
    public class MemoryMonitor: BaseMonitor,IGjallarhornMonitor
    {
        private PerformanceCounter _perfmonCounter;
        private AdsRuleMonitor _ruleMonitor;
        

        public MemoryMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons): base(notifyerDaemons, "MemoryMonitor")
        {
            _ruleMonitor = new AdsRuleMonitor(new AdsRuleSetting (MonitorName, "MemoryUsed"), RuleTriggered, ResetNotification);
        }

        private void RuleTriggered(AdsRuleSetting setting)
        {
            Notify("Memory Monitor has fired a warning", new List<string> {$"The Rule '{setting.AdsRuleName}' has triggered with high memory usage used on machine {setting.MachineName}."});
        }
       
        public void Execute()
        {
            try
            {
                if (_perfmonCounter == null)
                {
                    _perfmonCounter = new PerformanceCounter("Memory", "Available MBytes");
                }

                double totalMem = GetTotalMemoryInMb();
                double usedMem = (totalMem - _perfmonCounter.NextValue()); //used memory

                _ruleMonitor.Analyze(usedMem/totalMem);

            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed {MonitorName} execute", e);
            }
        }

        private static double GetTotalMemoryInMb()
        {
            double totalMem = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem")
                .Get()
                .Cast<ManagementBaseObject>()
                .Sum(item => double.Parse(item["TotalPhysicalMemory"].ToString()));

            return totalMem / (1024 * 1024);
        }
    }
}
