using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Logging;
using Gjallarhorn.Notifiers;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using Gjallarhorn.Db;

namespace Gjallarhorn.Monitors
{
    public class SenseLogFileParserMonitor : BaseMonitor, IGjallarhornMonitor
    {
        public SenseLogFileParserMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "SenseLogFileParserMonitor") { }

        public void Execute()
        {
            try
            {
                //get the path to folder

                var host = Settings.GetSetting($"{MonitorName}.HostName", "(undefined)");
                if (host.Equals("(undefined)", StringComparison.InvariantCultureIgnoreCase))
                {
                    host = (Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower();
                }
                //var senseApi = SenseApiSupport.Create(host);
                //var helper = new SenseApiHelper();

                //var archivedLogsLocation = helper.GetQlikSenseArchivedFolderLocation(senseApi);

                string archivedLogsLocation = @"C:\temp\ArchivedLogs";//@"D:\SFDCData\files\01471384\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d";
                //get yesterday +1 
                var settings = new LogFileDirectorSettings
                {
                    OutputFolderPath = @"c:\temp\temp2",
                    StartDateForLogs = DateTime.Now.AddDays(-2).Date,
                    StopDateForLogs = DateTime.Now.AddDays(-1).Date.AddMilliseconds(-1),
                };
                //settings.StartDateForLogs = DateTime.Parse("2018-08-27 00:00:00");
                //settings.StopDateForLogs = DateTime.Parse("2018-08-27 23:59:59");
                settings.StartDateForLogs = DateTime.Parse("2019-04-22 00:00:00");
                settings.StopDateForLogs = DateTime.Parse("2018-04-22 23:59:59");
                var a = new LogFileDirector(FileSystem.Singleton);
                var data = new BasicDataFromFileMiner();
                a.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings, data);
                //persisting current days apps and users for more analysis.
                var db = new GjallarhornDb(FileSystem.Singleton);
                db.AddToMontlyStats(data.TotalUniqueActiveAppsList, settings.StartDateForLogs.Year, settings.StartDateForLogs.Month,MontlyStatsType.Apps);
                db.AddToMontlyStats(data.TotalUniqueActiveUsersList, settings.StartDateForLogs.Year, settings.StartDateForLogs.Month, MontlyStatsType.Users);
                Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }
    }
}
