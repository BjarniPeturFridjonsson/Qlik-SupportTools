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
                var senseApi = SenseApiSupport.Create(host);
                var helper = new SenseApiHelper();

                var archivedLogsLocation = helper.GetQlikSenseArchivedFolderLocation(senseApi);


                //get yesterday +1 


                // setup parser
                var a = new StreamLogDirector{ FriendlyName = "Archived Logs", NotificationKey = "ArchivedLogs" };
                Log.To.Main.Add($"Started reading Archived Logs at {archivedLogsLocation}");
                //a.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings);
                //parse logfiles
                //finalize stats
                //send stats.

                //Notify($"{MonitorName} has analyzed the following system", new List<string> { JsonConvert.SerializeObject(data, Formatting.Indented) }, "-1");
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed executing {MonitorName}", ex);
            }
        }

        public IEnumerable<IFileInfo> EnumerateLogFiles(string path, DateTime from, DateTime to)
        {
            var sw = new Stopwatch();
            sw.Start();
            var dirInfo = new System.IO.DirectoryInfo(path);

            var a = dirInfo.EnumerateFiles().Where(p =>
                p.LastWriteTime >= from && p.LastWriteTime <= to &&
                (p.Extension.Equals(".log", StringComparison.InvariantCultureIgnoreCase) || p.Extension.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            );

            var ret = new List<IFileInfo>();
            foreach (var fileInfo in a)
            {
                ret.Add(new SystemFileInfo(fileInfo));
            }
            sw.Stop();
            Debug.WriteLine($"enum time => {sw.Elapsed.TotalSeconds} for {path}");
            return ret;
        }
    }
}
