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
                var logMinerData = new FileMinerDto();
                var data = new StatisticsDto { LogFileMinerData = logMinerData, CollectionDateUtc = logMinerData.CollectionDateUtc };
                var settings = new LogFileDirectorSettings
                {
                    StartDateForLogs = DateTime.Now.AddDays(-2).Date,
                    StopDateForLogs = DateTime.Now.AddDays(-1).Date.AddMilliseconds(-1),
                };

                settings.StartDateForLogs = DateTime.Parse("2019-04-30 00:00:00").AddDays(FAKERUNCOUNT);
                settings.StopDateForLogs = DateTime.Parse("2019-04-30 23:59:59").AddDays(FAKERUNCOUNT);
                var archivedLogsLocation = new DirectorySetting(@"C:\ProgramData\QlikTech\QlikViewServer");
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
