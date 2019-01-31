using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Notifiers;
using QMS_API.QMSBackend;
using Exception = System.Exception;

namespace Gjallarhorn.Monitors
{
    public class QmsMonitor : BaseMonitor, IGjallarhornMonitor
    {
        //private string _lastQmsName;
        //private AdsRuleMonitor _ruleMonitor;
        private readonly GroupedNotifyerWorker _groupedNotifyer;

        public QmsMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "QmsMonitor")
        {
            //_ruleMonitor = new AdsRuleMonitor(new AdsRuleSetting(MonitorName, "MemoryUsed"), RuleTriggered, ResetNotification);
            _groupedNotifyer = new GroupedNotifyerWorker(MonitorName, Notify, null);
        }

        public void Execute()
        {
            try
            {


                string qmsAddress = Settings.GetSetting($"{MonitorName}.Address");
                //var qmsHost = new Uri(qmsAddress).Host;

                using (var qmsApiService = new QMS_API.AgentsQmsApiService(qmsAddress))
                {
                    
                    if (!qmsApiService.TestConnection())
                    {
                        Log.To.Main.Add("Could not connect to QMS API (" + qmsAddress + ")!");
                        //SendAlert(qmsAddress, qmsHost);
                        return;
                    }

                    Analyze(qmsApiService);
                }


                //Notify($"{MonitorName} has found the following issues", msgs);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed {MonitorName} execute", e);
            }
        }
        private void Analyze(QMS_API.AgentsQmsApiService qmsAgentsQmsApi)
        {
            List<Guid> serviceIDs = qmsAgentsQmsApi.GetServices().Select(t => t.ID).ToList();

            if (serviceIDs.Count == 0)
                return;
            //List<ServiceInfo> qvsServices = qmsAgentsQmsApi.GetServices(ServiceTypes.QlikViewServer);
            //var abb = qmsAgentsQmsApi.GetQdsSettings(serviceIDs[2], QDSSettingsScope.General);
            List<ServiceStatus> serviceStatuses = qmsAgentsQmsApi.GetServiceStatuses(serviceIDs);
            

            serviceStatuses.ForEach(svc => svc.MemberStatusDetails.ForEach(msd =>
            {
                try
                {
                    //var a = qmsAgentsQmsApi.GetQdsSettings(svc.ID, QDSSettingsScope.General);
                    //if (!a.General.ShowAlerts)
                    //    return;
                    var key = $"{svc.Name}_{msd.Host}_{msd.ID}";
                    _groupedNotifyer.AddIfMissing(key, $"{svc.Name} service down on {msd.Host}","");
                    _groupedNotifyer.Analyze(key, (int)msd.Status);
                }
                catch (Exception ex)
                {
                    Log.To.Main.AddException("Failed QmsMonitor analyze",ex);
                }
            }));
            _groupedNotifyer.AnalyzeRoundFinished();
        }
        //private void RuleTriggered(AdsRuleSetting setting)
        //{
        //    Notify("Memory Monitor has fired a warning", $"The Rule '{setting.AdsRuleName}' has triggered with high memory usage used on machine {setting.MachineName}.");
        //}
    }
}
