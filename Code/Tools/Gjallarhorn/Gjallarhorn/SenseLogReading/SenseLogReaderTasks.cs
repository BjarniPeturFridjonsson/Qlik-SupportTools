//using Eir.Common.IO;
//using FreyrCommon.Logging;
//using FreyrCommon.Models;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Gjallarhorn.SenseLogReading
//{
//    public class SenseLogReaderTasks
//    {
//        private readonly ILogger _logger;
//        private readonly Action<StreamLogDirector> _onFinished;
//        public int TotalNumberOfLogFiles;
//        public int TotalNumberOfRemoteLogFiles;

//        public SenseLogReaderTasks(ILogger logger, Action<StreamLogDirector> onFinished)
//        {
//            _logger = logger;
//            _onFinished = onFinished;
//        }

//        public Task ReadArchivedLogs(string archivedLogsLocation, CommonCollectorServiceVariables settings)
//        {
//            _logger.Add("Reading Archived Logs");
//            var a = new StreamLogDirector() { FriendlyName = "Archived Logs", NotificationKey = "ArchivedLogs" };
//            return Task.Run(() =>
//               {
//                   //a.OnLogDirectorFinishedReading(_onFinished);
//                   if (string.IsNullOrEmpty(archivedLogsLocation))
//                       archivedLogsLocation = $@"\\{settings.DnsHostName}\SenseShare\Archived Logs";
//                   //todo: warning fix this.....
//                   _logger.Add($"Started reading Archived Logs at {archivedLogsLocation}");
//                   a.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings);
//                   TotalNumberOfLogFiles += a.FoundFileCount;
//                   _logger.Add($"Started reading NPrinting logs locally at c:");

//               }
//            ).ContinueWith(p =>
//            {
//                _onFinished.Invoke(a);
//            });
//        }

//        public Task ReadRemoteLogs(IEnumerable<QlikSenseMachineInfo> qlikSenseMachineInfos, CommonCollectorServiceVariables settings)
//        {
//            return Task.Run(
//                () =>
//                {
//                    foreach (var info in qlikSenseMachineInfos)
//                    {
//                        var director = new StreamLogDirector() { FriendlyName = "Remote Logs " + info.Name, NotificationKey = "RemoteLogs" };
//                        var path = $@"\\{info.HostName}\c$\ProgramData\Qlik\Sense\Log";
//                        //director.OnLogDirectorFinishedReading(LogDirectorDone);
//                        _logger.Add($"Started reading remote Logs from {info.Name} at {path}");
//                        director.LoadAndRead(new[] { new DirectorySetting(path) }, settings);
//                        TotalNumberOfRemoteLogFiles += director.FoundFileCount;
//                        _logger.Add($"Started reading NPrinting logs at {$@"\\{info.HostName}\c$"}");

//                    }
//                }).ContinueWith(p =>
//            {
//                _onFinished.Invoke(new StreamLogDirector() { FriendlyName = "Remote Logs", NotificationKey = "RemoteLogs", FoundFileCount = TotalNumberOfRemoteLogFiles });
//            });
//        }
//    }
//}
