using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Models;
using FreyrCollectorCommon.CollectorCore;
using FreyrCollectorCommon.Collectors;
using FreyrSenseCollector.Collectors;
using FreyrCollectorCommon.Common;
using FreyrCommon.Logging;
using FreyrSenseCollector.SenseLogReading;
using Newtonsoft.Json;
using SenseApiLibrary;

namespace FreyrSenseCollector
{
    public class SenseCollectorService
    {
        public bool AbortAndExit { get; private set; }

        private readonly ILogger _logger;
        private readonly Action<string, MessageLevels, string> _notify;
        private readonly IFileSystem _filesystem = FileSystem.Singleton;
        private CollectorHelper _collectorHelper;
        public CommonCollectorServiceVariables ServiceVariables { get; private set; } = new CommonCollectorServiceVariables();
        public Func<SenseConnectDto, SenseConnectDto> ConnectToSenseApiManuallyDlg;
        public Func<SenseConnectDto, SenseConnectDto> PathToSenseLogFolderDlg;
        public Func<string,string> PathToArchivedLogsDlg;
        private readonly Action<string, string, SenseCollectorService> _onFinished;


        public SenseCollectorService(ILogger logger, Action<string, MessageLevels, string> notify, Action<string, string, SenseCollectorService> onFinished)
        {
            _logger = logger;
            _notify = notify;
            _onFinished = onFinished;
        }

        public async Task<bool> Start(CommonCollectorServiceVariables settings)
        {
            ServiceVariables = settings;
            _collectorHelper = new CollectorHelper(settings,_logger);
            ValidateFuncs();
            return await RunCollectionFlow().ConfigureAwait(false);
        }

        private void ValidateFuncs()
        {
            if (ConnectToSenseApiManuallyDlg == null || PathToSenseLogFolderDlg == null || _onFinished == null || PathToArchivedLogsDlg == null)
                throw new Exception("Developer failure. Dialogues are not set.");
        }


        private async Task<bool> RunCollectionFlow()
        {
            List<Task> taskList;
            _logger.Add($"Setting up connection to Sense Server on {ServiceVariables.DnsHostName}");
            var dto = new SenseConnectDto { SenseHostName = ServiceVariables.DnsHostName, ConnectToSenseApiManuallyDlg = ConnectToSenseApiManuallyDlg };

            await Task.Run(() =>
            {
                var a = new ConnectToSenseHelper(_logger).ConnectToSenseApi(dto);
                dto = a;
                _notify("Connected to Qlik Sense Installation", MessageLevels.Ok, "Connecting");
            }).ConfigureAwait(false);

            if (dto == null || dto.AbortAndExit)
            {
                AbortAndExit = true;
                _logger.Add("Aborting connection requested.");
                _notify("Aborting.", MessageLevels.Error, null);
                _onFinished(null, null, this);
                return false;
            }


            if (dto.RunWithDeadInstallation)
            {
                _logger.Add("Running with local folders only");
                taskList = GetTasksForDeadInstallation(dto);
                if (taskList == null)
                    return false;
                taskList.AddRange(GetTasksForWindows());
            }
            else
            {
                taskList = await GetTasksForFullRun(dto).ConfigureAwait(false);
                taskList.AddRange(GetTasksForWindows());
            }

            await Task.WhenAll(taskList).ContinueWith(p =>
                {
                    _logger.Add("Finished collection part");
                    _logger.Add(JsonConvert.SerializeObject(ServiceVariables));
                    Log.Shutdown();

                    Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                   
                    var pathToZip = _collectorHelper.CreateZipFile(ServiceVariables);
                    ServiceVariables.CollectorOutput.ZipFile = pathToZip;
                    _notify(pathToZip, MessageLevels.Ok, null);
                    try
                    {
                        Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(task =>
                        {// give the system time to flush the filesystem.
                            FileSystem.Singleton.DeleteDirectory(ServiceVariables.OutputFolderPath);
                        });
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);//nothing to log home about :( the log is already zipped.
                    }
                     
                    //if (Settings.UseOnlineDelivery)
                    //        var transport = new Sender(Settings.Key, Settings.SendId);
                    //        transport.Transport(Settings.OnlineReccever, Settings.CollectorOutput.ZipFile);

                    AbortAndExit = false;
                    _onFinished(null, null, this);
                }
            ).ConfigureAwait(false);
            return true;
        }

        private List<Task> GetTasksForWindows()
        {
            var taskList = new List<Task>();
            if (ServiceVariables.AllowWindowsLogs)
            {
                taskList.Add(Task.Run(() =>
                {

                    _notify("Collecting Windows event log files.", MessageLevels.Animate, "WindowsEventLogs");
                    try
                    {
                        var ba = new WindowsEventLogs(_logger);
                        ba.ReadEvents(ServiceVariables.StartDateForLogs, ServiceVariables.StopDateForLogs, _collectorHelper);
                        //_collectorHelper.WriteContentToFile(c, "WindowsEvents");
                        _notify("Collected Windows event log files.", MessageLevels.Ok, "WindowsEventLogs");
                    }
                    catch (Exception e)
                    {
                        _logger.Add("Failed collecting Windows event log files", e);
                        _notify("Failed collecting Windows event log files.", MessageLevels.Error, "WindowsEventLogs");
                    }
                }));
            }

            if (ServiceVariables.AllowMachineInfo)
            {

                taskList.Add(Task.Run(async () =>
                {
                    try
                    {
                        _notify("Collecting machine information", MessageLevels.Animate, "MachineInfo");

                        var wmi = new WmiCollector();
                        await RunCmdLineAgents().ConfigureAwait(false);
                        _collectorHelper.WriteContentToFile(wmi.GetValuesFromWin32Os(), "WmiWin32");
                        _collectorHelper.WriteContentToFile(wmi.GetHotFixes(), "HotFixes");
                        _collectorHelper.WriteContentToFile(wmi.GetStandardInfo(), "StdInfo");
                        _collectorHelper.WriteContentToFile(wmi.GetValuesFromWmiPagefile(), "PageFile");
                        _collectorHelper.WriteContentToFile(wmi.GetValuesFromWindowsServicesInfo(), "Services");
                        _collectorHelper.WriteContentToFile(wmi.GetValuesFromWindowsServicesInfo(), "WindowsServiceInfovi");

                        _notify("Collected machine information", MessageLevels.Ok, "MachineInfo");
                    }
                    catch (Exception e)
                    {
                        _logger.Add("Failed collecting machine information", e);
                        _notify("Failed collecting machine information", MessageLevels.Error, "MachineInfo");
                    }
                }));
            }
            return taskList;
        }

        private List<Task> GetTasksForDeadInstallation(SenseConnectDto dto)
        {
            var taskList = new List<Task>();
            if (!ServiceVariables.AllowArchivedLogs)
                return taskList;

            try
            {
                var readerTaskHelper = new SenseLogReaderTasks(_logger, LogDirectorDone, _notify);
                dto = PathToSenseLogFolderDlg(dto);
                if (string.IsNullOrEmpty(dto.PathToLocalSenseLogFolder))
                {
                    _logger.Add("No Sense log folder selected. Tool has to run again. Ending.");
                    _notify("No Sense Log folder. Please run tool again.", MessageLevels.Error, null);
                    _onFinished(null, null, this);
                    return null;
                }
                taskList.Add(readerTaskHelper.ReadArchivedLogs(dto.PathToLocalSenseLogFolder, ServiceVariables));
            }
            catch (Exception ex)
            {
                _logger.Add("Failed retreiving Archived folder location from user. " + ex);
            }
            return taskList;
        }

        private async Task<List<Task>> GetTasksForFullRun(SenseConnectDto dto)
        {
            var senseApi = new SenseApiCollector(_logger);
            var taskList = new List<Task>();

            _logger.Add("Sense api connected");
            SenseEnums statusEnums = null;
            QlikSenseMachineInfo[] qlikSenseMachineInfos = null;

            await Task.Run(() => { statusEnums = new SenseEnums(dto.SenseApiSupport); }).ConfigureAwait(false);
            _logger.Add("Sense Machine infos collected");

            var archivedLogsLocation = string.Empty;
            if (statusEnums == null)
                throw new Exception("Failed accessing sense");
            await Task.Run(() => qlikSenseMachineInfos = GetQlikSenseMachineInfos(senseApi, dto.SenseApiSupport, statusEnums)).ConfigureAwait(false);

            try
            {
                archivedLogsLocation = senseApi.GetQlikSenseArchivedFolderLocation(dto.SenseApiSupport);
                _logger.Add($"Accessing archived logs at {archivedLogsLocation}.");
                if (!_filesystem.DirectoryExists(archivedLogsLocation))
                {
                    archivedLogsLocation = PathToArchivedLogsDlg.Invoke(archivedLogsLocation);
                }
            }
            catch (Exception ex)
            {
                _logger.Add("Failed retreiving Archived folder location from API " + ex);
            }

            taskList.Add(Task.Run(async () =>
            {
               if (ServiceVariables.AllowSenseInfo)
               {
                   try
                   {
                       var nodeCount = qlikSenseMachineInfos?.Length ?? 0;
                       _notify($"Collecting Information from {nodeCount} Qlik Sense {(nodeCount > 1 ? "nodes" : "node")}.", MessageLevels.Animate, "SenseInfo");
                       _logger.Add($"Running CommandLineRemoteTasks.");
                        await RunCommandLineRemoteTasks(qlikSenseMachineInfos).ConfigureAwait(false);
                       _logger.Add($"Running GettingAllSettingsFiles.");
                        await RunGettingAllSettingsFiles(qlikSenseMachineInfos).ConfigureAwait(false);

                       _notify("Collecting Qlik Sense Information.", MessageLevels.Animate, "SenseInfo");
                       _logger.Add($"Running Get all api info.");
                        _collectorHelper.WriteContentToFile(qlikSenseMachineInfos, "qlikSenseMachineInfo");
                       _collectorHelper.RunAction(() => senseApi.GetQlikSenseServiceInfos(dto.SenseApiSupport, statusEnums).ToArray(), "qlikSenseServiceInfo");
                       _collectorHelper.RunAction(() => senseApi.GetQrsAbout(dto.SenseApiSupport, statusEnums), "QrsAbout");
                       _collectorHelper.RunAction(() => senseApi.ExecuteCalAgent(dto.SenseApiSupport, statusEnums), "CalInfo");
                       _collectorHelper.RunAction(() => senseApi.ExecuteLicenseAgent(dto.SenseApiSupport, statusEnums), "LicenseAgent");
                       _collectorHelper.RunAction(() => senseApi.GetAboutSystemInfo(dto.SenseApiSupport, statusEnums), "AboutSystemInfo");
                       _collectorHelper.RunAction(() => senseApi.GetAboutComponents(dto.SenseApiSupport, statusEnums), "AboutComponents");
                       _collectorHelper.RunAction(() => senseApi.GetQrsDataconnections(dto.SenseApiSupport, statusEnums), "DataConnections");
                       _collectorHelper.RunAction(() => senseApi.GetQrsServiceCluster(dto.SenseApiSupport, statusEnums), "ServiceCluster");
                       _collectorHelper.RunAction(() => senseApi.GetQrsProxyService(dto.SenseApiSupport, statusEnums), "ProxyService");
                       _collectorHelper.RunAction(() => senseApi.GetQrsAppList(dto.SenseApiSupport, statusEnums), "AppList");
                       _collectorHelper.RunAction(() => senseApi.GetQrsDataConnections(dto.SenseApiSupport, statusEnums), "dataconnectionList");

                       _logger.Add($"Finished senseInfo");
                        _notify("Collected Qlik Sense Information.", MessageLevels.Ok, "SenseInfo");

                   }
                   catch (Exception e)
                   {
                       _logger.Add("Failed getting sense info", e);
                       _notify("Failed collecting Qlik Sense Information.", MessageLevels.Error, "SenseInfo");
                   }
               }

            }));
            var readerTaskHelper = new SenseLogReaderTasks(_logger, LogDirectorDone, _notify);
            if (ServiceVariables.AllowRemoteLogs)
                taskList.Add(readerTaskHelper.ReadRemoteLogs(qlikSenseMachineInfos, ServiceVariables));

            if (ServiceVariables.AllowArchivedLogs)
                taskList.Add(readerTaskHelper.ReadArchivedLogs(archivedLogsLocation, ServiceVariables));

            return taskList;
        }

        private async Task<bool> RunGettingAllSettingsFiles(QlikSenseMachineInfo[] infos)
        {
            try
            {
                foreach (var info in infos)
                {
                    try
                    {
                        var cls = new ConfigurationCollector(FileSystem.Singleton, _logger);
                        var res = await cls.RunRemoteCmds(info.HostName).ConfigureAwait(false);
                        res.ForEach(p =>
                        {
                            _collectorHelper.WriteContentToFile(p.Result, _filesystem.Path.Combine(ServiceVariables.OutputFolderPath, info.HostName, p.Name));
                            p.Result = "<removed>";
                        });
                        _collectorHelper.WriteContentToFile(res, _filesystem.Path.Combine(ServiceVariables.OutputFolderPath, info.HostName, "SettingsFileCrawlerResults"));
                    }
                    catch (Exception e)
                    {
                        _logger.Add($"Failed Getting Settings info from {info.HostName}", e);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Add("Failed running RunGettingAllSettingsFiles", ex);
                return false;
            }
        }

        private QlikSenseMachineInfo[] GetQlikSenseMachineInfos(SenseApiCollector senseApi, SenseApiSupport senseApiSupport, SenseEnums statusEnums)
        {
            try
            {
                return senseApi.GetQlikSenseMachineInfos(senseApiSupport, statusEnums).ToArray();
            }
            catch (Exception e)
            {
                _logger.Add("Failed accessing Sense machine Infos", e);
                throw;
            }
        }

        private async Task<bool> RunCommandLineRemoteTasks(QlikSenseMachineInfo[] infos)
        {
            try
            {
                foreach (var info in infos)
                {
                    var cmds = new CmdLineAgents(_filesystem, _logger);
                    try
                    {
                        var res = await cmds.RunRemoteCmds(info.HostName, TimeSpan.FromMinutes(10)).ConfigureAwait(false);
                        _collectorHelper.WriteContentToFile(res, _filesystem.Path.Combine(ServiceVariables.OutputFolderPath, info.HostName, "SystemInfo.csv"));
                    }
                    catch (Exception e)
                    {
                        _logger.Add($"Failed accessing remote SystemInfo or write on {info?.HostName ?? "UNKNOWN"}",e);
                    }
                   
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Add("Failed running RunCommandLineRemoteTasks", ex);
                return false;
            }
        }

        private async Task<bool> RunCmdLineAgents()
        {
            var cmds = new CmdLineAgents(_filesystem, _logger);
            var res = await cmds.RunLocalCmds().ConfigureAwait(false);
            _collectorHelper.WriteContentToFile(res, "SystemInfo");
            return true;
        }

        private void LogDirectorDone(StreamLogDirector director)
        {
            ServiceVariables.CollectorOutput.LogFileCount += director.FoundFileCount;
            if (director.FoundFileCount == 0)
                _notify($"{director.FriendlyName} found no log files!", MessageLevels.Warning, director.NotificationKey);
            else
                _notify($"{director.FriendlyName} Done. Found {director.FoundFileCount} log files", MessageLevels.Ok, director.NotificationKey);
        }
    }
}
