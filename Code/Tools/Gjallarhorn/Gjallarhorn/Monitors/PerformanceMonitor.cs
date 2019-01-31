using System;
using System.Collections.Generic;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Monitors.PerfMonMonitor;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Monitors
{
    //todo: replace perfMonWorker fire mechanism to adsRuleMonitor.
    public class PerformanceMonitor : BaseMonitor,IGjallarhornMonitor
    {

        private Dictionary<string, PerfMonWorker> _monitoringAgent;
        public PerformanceMonitor(Func<string,IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "PerformanceMonitor")
        {
            _monitoringAgent = new Dictionary<string,PerfMonWorker>();
        }


        public void Execute()
        {
            try
            {
                var machineNames = GetMachineNames();
                
                foreach (string machineName in machineNames)
                {
                    if(!_monitoringAgent.ContainsKey(machineName))
                        _monitoringAgent.Add(machineName,new PerfMonWorker(machineName, Notify, ResetNotification) );
                    _monitoringAgent[machineName].PerformMonitoringForMachine();
                }
                
            }
            catch (Exception e)
            {
                Log.To.Main.AddException("Failed PerfMonAgent execute",e);
                _monitoringAgent = new Dictionary<string, PerfMonWorker>();//reset
            }   
        }

        private IEnumerable<string> GetMachineNames()
        {
            var names = Settings.GetSetting("PerformanceMonitor.ComputersToMonitor").Split(new []{","},StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 0)
            {
                names = new[] {Environment.MachineName};//if all are missing just use the localhost.
            }
            return names;
        }
        //private void MessageAlert(string message)
        //{
        //    Notify("Performance monitor has found the following issues", message);
        //}

        public override void Stop()
        {
            foreach (var item in _monitoringAgent)
            {
                item.Value.Stop();
            }
        }
    }
}
