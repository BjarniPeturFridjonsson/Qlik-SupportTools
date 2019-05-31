using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Logging;
using Gjallarhorn.Notifiers;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Gjallarhorn.Db;
using Gjallarhorn.Monitors.QmsApi;
using SenseApiLibrary;

namespace Gjallarhorn.Monitors
{
    public class SenseLogFileParserMonitor : BaseMonitor, IGjallarhornMonitor
    {
        private string _installationId;
        private string _licenseSerialNr;
        private static int FAKERUNCOUNT = 0;
        public SenseLogFileParserMonitor(Func<string, IEnumerable<INotifyerDaemon>> notifyerDaemons) : base(notifyerDaemons, "SenseLogFileParserMonitor") { }

        public void Execute()
        {
            try
            {
                var logFileDirector = new LogFileDirector(FileSystem.Singleton);
                var logMinerData = new FileMinerDto();
                var data = new StatisticsDto { LogFileMinerData = logMinerData, CollectionDateUtc = logMinerData.CollectionDateUtc };
                string archivedLogsLocation;

                //default we ask sense for the settings needed.
                if (String.IsNullOrWhiteSpace(Settings.GetSetting($"{MonitorName}.OverideLogFilePath", ""))
                    && String.IsNullOrWhiteSpace(Settings.GetSetting($"{MonitorName}.LicenseSerialNo", ""))
                    && String.IsNullOrWhiteSpace(Settings.GetSetting($"{MonitorName}.ServiceClusterId", "")))
                {
                    var host = Settings.GetSetting($"{MonitorName}.HostName", "(undefined)");
                    if (host.Equals("(undefined)", StringComparison.InvariantCultureIgnoreCase))
                    {
                        host = (Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower();
                    }
                    var senseApi = SenseApiSupport.Create(host);
                    var helper = new SenseApiHelper();
                    SenseEnums senseEnums = new SenseEnums(senseApi);

                    try { data.QlikSenseMachineInfos = helper.GetQlikSenseMachineInfos(senseApi, senseEnums).ToList(); } catch (Exception e) { data.Exceptions.Add(e); }
                    try { data.QlikSenseLicenseAgent = helper.ExecuteLicenseAgent(senseApi, senseEnums); } catch (Exception e) { data.Exceptions.Add(e); }
                    try { data.QlikSenseServiceInfo = helper.GetQlikSenseServiceInfos(senseApi, senseEnums).ToList(); } catch (Exception e) { data.Exceptions.Add(e); }

                    archivedLogsLocation = helper.GetQlikSenseArchivedFolderLocation(senseApi);
                    _installationId = $"{data.QlikSenseLicenseAgent?.LicenseSerialNo ?? "(unknown)"}_{data.QlikSenseServiceInfo?.FirstOrDefault()?.ServiceClusterId.ToString() ?? "(unknown)"} ";
                    _licenseSerialNr = data.QlikSenseLicenseAgent?.LicenseSerialNo ?? "(unknown)";

                    data.QlikSenseLicenseAgent = null;
                    data.QlikSenseServiceInfo = null;
                    data.QlikSenseMachineInfos = null;

                }
                else // pull from settings
                {
                    _licenseSerialNr = Settings.GetSetting($"{MonitorName}.LicenseSerialNo", "");
                    _installationId = $"{_licenseSerialNr}_{Settings.GetSetting($"{MonitorName}.ServiceClusterId", "")}";
                    archivedLogsLocation = Settings.GetSetting($"{MonitorName}.OverideLogFilePath", "");
                }

                data.InstallationId = _installationId;
                data.LogFileMinerData.LicenseSerialNo = _licenseSerialNr;
                //string archivedLogsLocation = @"C:\temp\ArchivedLogs";//@"D:\SFDCData\files\01471384\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d\SenseCollector_e8e2d1bc-3c1e-41d7-9a9c-0cac78c7539d";
                //get yesterday +1 
                var settings = new LogFileDirectorSettings
                {
                    StartDateForLogs = DateTime.Now.AddDays(-2).Date,
                    StopDateForLogs = DateTime.Now.AddDays(-1).Date.AddMilliseconds(-1),
                };
                //settings.StartDateForLogs = DateTime.Parse("2018-08-27 00:00:00");
                //settings.StopDateForLogs = DateTime.Parse("2018-08-27 23:59:59");
                settings.StartDateForLogs = DateTime.Parse("2019-03-09 00:00:00").AddDays(FAKERUNCOUNT);
                settings.StopDateForLogs = DateTime.Parse("2019-03-09 23:59:59").AddDays(FAKERUNCOUNT);

                logFileDirector.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings, logMinerData);
                //persisting current days apps and users for more analysis.
                var db = new GjallarhornDb(FileSystem.Singleton);

                CheckMontlySending(settings.StartDateForLogs.Month);

                db.AddToMontlyStats(logMinerData.TotalUniqueActiveAppsList, settings.StartDateForLogs.Year, settings.StartDateForLogs.Month, MontlyStatsType.Apps);
                db.AddToMontlyStats(logMinerData.TotalUniqueActiveUsersList, settings.StartDateForLogs.Year, settings.StartDateForLogs.Month, MontlyStatsType.Users);
                Trace.WriteLine($"{settings.StartDateForLogs.ToString("yyyy-MM-dd")} sessionCount=>{data.LogFileMinerData.TotalNrOfSessions} on FakeRun:{FAKERUNCOUNT}");
                Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");
                FAKERUNCOUNT++;
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }

        private int _debugMontlyDaySent = -1;

        private void CheckMontlySending(int logMonth)
        {
            var db = new GjallarhornDb(FileSystem.Singleton);

            //var logMinerData = new FileMinerDto();
            //logMinerData.TotalUniqueActiveAppsList = new Dictionary<string,int>();
            //logMinerData.TotalUniqueActiveUsersList = new Dictionary<string, int>();
            //for (int i = 0; i < 100; i++)
            //{
            //    logMinerData.TotalUniqueActiveAppsList.Add(Guid.NewGuid().ToString(),1);
            //    logMinerData.TotalUniqueActiveUsersList.Add(Guid.NewGuid().ToString(), 1);
            //}

            //db.AddToMontlyStats(logMinerData.TotalUniqueActiveAppsList,2019 , 5, MontlyStatsType.Apps);
            //db.AddToMontlyStats(logMinerData.TotalUniqueActiveUsersList, 2019, 5, MontlyStatsType.Users);

            var monthlyDebug = Settings.GetBool($"{MonitorName}.MonthlyDebug", false);

            var currMonthDb = db.CurrentMontlyRunInDb();
            if ((currMonthDb < 0 || currMonthDb == logMonth) && monthlyDebug == false) return;
            //for sending debug every other day.
            if (DateTime.Now.Day % 2 == 0 && monthlyDebug ) return;
            if (monthlyDebug && _debugMontlyDaySent == DateTime.Now.Day) return;

            var data = new StatisticsDto
            {
                LogFileMinerData = new FileMinerDto(),
                CollectionDateUtc = DateTime.UtcNow,
                InstallationId = _installationId
            };

            data.LogFileMinerData.LicenseSerialNo = _licenseSerialNr;
            data.LogFileMinerData.IsMonthly = true;
            data.LogFileMinerData.TotalUniqueActiveApps = db.GetToMontlyStats(MontlyStatsType.Apps);
            data.LogFileMinerData.TotalUniqueActiveUsers = db.GetToMontlyStats(MontlyStatsType.Users);

            Notify($"{MonitorName} has analyzed the following system montly", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");

            db.ResetMontlyDbTable();
            _debugMontlyDaySent = DateTime.Now.Day;
        }
    }
}
