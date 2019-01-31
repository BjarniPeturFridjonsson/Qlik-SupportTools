using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using FreyrSenseCollector.Collectors;

namespace FreyrSenseCollector.SenseLogReading
{
    public class SenseLogReaderTasks
    {
        private readonly ILogger _logger;
        private readonly Action<StreamLogDirector> _onFinished;
        public int TotalNumberOfLogFiles;
        public int TotalNumberOfRemoteLogFiles;
        private readonly Action<string, MessageLevels, string> _notify;

        public SenseLogReaderTasks(ILogger logger,  Action<StreamLogDirector> onFinished, Action<string,MessageLevels,string> notify)
        {
            _logger = logger;
            _onFinished = onFinished;
            _notify = notify;
        }

        public Task ReadArchivedLogs(string archivedLogsLocation, CommonCollectorServiceVariables settings)
        {
            _logger.Add("Reading Archived Logs");
            var a = new StreamLogDirector(_logger, _notify) { FriendlyName = "Archived Logs", NotificationKey = "ArchivedLogs" };
            return Task.Run(async () =>
                {
                    //a.OnLogDirectorFinishedReading(_onFinished);
                    if (string.IsNullOrEmpty(archivedLogsLocation))
                        archivedLogsLocation = $@"\\{settings.DnsHostName}\SenseShare\Archived Logs";
                    //todo: warning fix this.....
                    _logger.Add($"Started reading Archived Logs at {archivedLogsLocation}");
                    a.LoadAndRead(new[] { new DirectorySetting(archivedLogsLocation) }, settings);
                    TotalNumberOfLogFiles += a.FoundFileCount;
                    _logger.Add($"Started reading NPrinting logs locally at c:");
                    await new NPrintingCollector(FileSystem.Singleton,_logger).GetLogs(@"c:\", settings, FileSystem.Singleton.Path.Combine(settings.OutputFolderPath, settings.DnsHostName)).ConfigureAwait(false);
                    _logger.Add($"Started reading Connectors logs locally at c:");
                    await new ConnectorsLogCollector(FileSystem.Singleton, _logger).GetLogs(@"c:\", settings, FileSystem.Singleton.Path.Combine(settings.OutputFolderPath, settings.DnsHostName)).ConfigureAwait(false);
                    _logger.Add($"Finished reading archived and local logs");
                }
            ).ContinueWith(p =>
            {
                _onFinished.Invoke(a);
            });
        }

        public Task ReadRemoteLogs(IEnumerable<QlikSenseMachineInfo> qlikSenseMachineInfos, CommonCollectorServiceVariables settings)
        {
            return Task.Run(
                async () =>
                {
                    foreach (var info in qlikSenseMachineInfos)
                    {
                        var director = new StreamLogDirector(_logger,_notify) { FriendlyName = "Remote Logs " + info.Name, NotificationKey = "RemoteLogs" };
                        var path = $@"\\{info.HostName}\c$\ProgramData\Qlik\Sense\Log";
                        //director.OnLogDirectorFinishedReading(LogDirectorDone);
                        _logger.Add($"Started reading remote Logs from {info.Name} at {path}");
                        director.LoadAndRead(new[] { new DirectorySetting(path) }, settings);
                        TotalNumberOfRemoteLogFiles += director.FoundFileCount;
                        _logger.Add($"Started reading NPrinting logs at {$@"\\{info.HostName}\c$"}");
                        await new NPrintingCollector(FileSystem.Singleton, _logger).GetLogs($@"\\{info.HostName}\c$", settings, FileSystem.Singleton.Path.Combine(settings.OutputFolderPath, info.HostName)).ConfigureAwait(false);
                        _logger.Add($"Started reading Connector logs at {$@"\\{info.HostName}\c$"}");
                        await new ConnectorsLogCollector(FileSystem.Singleton, _logger).GetLogs($@"\\{info.HostName}\c$", settings, FileSystem.Singleton.Path.Combine(settings.OutputFolderPath, info.HostName)).ConfigureAwait(false);
                        _logger.Add($"Finished reading logs from {info.Name}");
                    }
                }).ContinueWith(p =>
            {
                _onFinished.Invoke(new StreamLogDirector(_logger, _notify) {FriendlyName = "Remote Logs" ,NotificationKey = "RemoteLogs", FoundFileCount = TotalNumberOfRemoteLogFiles});
            });
        }
    }
}
