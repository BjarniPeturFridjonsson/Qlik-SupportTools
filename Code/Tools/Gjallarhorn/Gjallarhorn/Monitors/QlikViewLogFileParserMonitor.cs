using Eir.Common.IO;
using Eir.Common.Logging;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using Gjallarhorn.QvLogReading;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Eir.Common.Common;
using QMS_API.QMSBackend;
using Exception = System.Exception;

namespace Gjallarhorn.Monitors
{
    public class QlikViewLogFileParserMonitor : BaseMonitor, IGjallarhornMonitor
    {
        //private string _installationId;
        //private string _licenseSerialNr;
        private static int FAKERUNCOUNT = 0;

        public QlikViewLogFileParserMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "QlikViewLogFileParserMonitor") { }


        public void Execute()
        {
            try
            {
                var qmsAddress = Settings.GetSetting($"{MonitorName}.QmsAddress", "(undefined)");
                DirectorySetting archivedLogsLocation;
                if (qmsAddress.Equals("(undefined)", StringComparison.InvariantCultureIgnoreCase))
                {
                    qmsAddress = $"http://{(Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower()}:4799/QMS/Service";
                }

                using (var qmsApiService = new QMS_API.AgentsQmsApiService(qmsAddress))
                {
                    if (!qmsApiService.TestConnection())
                    {
                        Log.To.Main.Add($"Could not connect to QMS API {qmsAddress} in {MonitorName}");
                        return;
                    }
                    List<ServiceInfo> qvsServices = qmsApiService.GetServices(ServiceTypes.QlikViewServer);
                    QVSSettings qvsSettings = qmsApiService.GetQvsSettings(qvsServices[0].ID, QVSSettingsScope.Logging);
                    var folder = qvsSettings.Logging.Folder;
                    if (!Directory.Exists(folder))
                    {
                        return;
                    }
                    archivedLogsLocation = new DirectorySetting(folder);
                }

                var logMinerData = new FileMinerDto();
                var data = new StatisticsDto { LogFileMinerData = logMinerData, CollectionDateUtc = logMinerData.CollectionDateUtc };
                var settings = new LogFileDirectorSettings
                {
                    StartDateForLogs = DateTime.Now.AddDays(-2).Date,
                    StopDateForLogs = DateTime.Now.AddDays(-1).Date.AddMilliseconds(-1),
                };

                //settings.StartDateForLogs = DateTime.Parse("2019-04-30 00:00:00").AddDays(FAKERUNCOUNT);
                //settings.StopDateForLogs = DateTime.Parse("2019-04-30 23:59:59").AddDays(FAKERUNCOUNT);
                //archivedLogsLocation = new DirectorySetting(@"C:\ProgramData\QlikTech\QlikViewServer");
                var logFileDirector = new QvLogDirector();

                logFileDirector.LoadAndRead(archivedLogsLocation, settings, logMinerData);
                Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");
                FAKERUNCOUNT++;
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }
    }
}
