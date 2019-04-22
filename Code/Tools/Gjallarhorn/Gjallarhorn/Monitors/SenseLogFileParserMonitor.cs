using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Monitors.QmsApi;
using Gjallarhorn.Notifiers;
using Newtonsoft.Json;
using SenseApiLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Eir.Common.IO;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;

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

                string archivedLogsLocation = @"D:\SFDCData\files\01471384\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d";
                //get yesterday +1 
                var settings = new LogFileDirectorSettings
                {
                    OutputFolderPath = @"c:\temp\temp2",
                    StartDateForLogs = DateTime.Now.AddDays(-2).Date,
                    StopDateForLogs = DateTime.Now.AddDays(-1).Date.AddMilliseconds(-1),
                };
                settings.StartDateForLogs = DateTime.Parse("2018-08-27 00:00:00");
                settings.StopDateForLogs = DateTime.Parse("2018-08-27 23:59:59");
                // setup parser
                var a = new LogFileDirector{ FriendlyName = "Archived Logs", NotificationKey = "ArchivedLogs" };
                Log.To.Main.Add($"Started reading Archived Logs at {archivedLogsLocation}");
                var data = new BasicDataFromFileMiner();
                a.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings, data);
                //parse logfiles
                //finalize stats
                //send stats.

               Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }
    }
}
