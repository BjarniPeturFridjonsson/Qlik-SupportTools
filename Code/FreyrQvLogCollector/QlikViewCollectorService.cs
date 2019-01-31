using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.Collectors;
using FreyrCollectorCommon.Common;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using FreyrQvLogCollector.Collectors;
using FreyrQvLogCollector.QvCollector;
using Newtonsoft.Json;


namespace FreyrQvLogCollector
{
    public class QlikViewCollectorService
    {
        public bool AbortAndExit { get; private set; }

        private readonly ILogger _logger;
        private readonly Action<string, MessageLevels, string> _notify;
        private readonly IFileSystem _filesystem = FileSystem.Singleton;
        private CollectorHelper _collectorHelper;


        public CommonCollectorServiceVariables ServiceVariables { get; private set; }
        public Func<QlikViewConnectDto, QlikViewConnectDto> ConnectToApiManuallyDlg;
        public Func<QlikViewConnectDto, QlikViewConnectDto> PathToLogFolderDlg;
        public Func<string, string> PathToArchivedLogsDlg;
        private readonly Action<string, string, QlikViewCollectorService> _onFinished;
        //private ApiCollector _apiCollector;


        public QlikViewCollectorService(ILogger logger, Action<string, MessageLevels, string> notify, Action<string, string, QlikViewCollectorService> onFinished)
        {
            _logger = logger;
            _notify = notify;
            _onFinished = onFinished;
        }

        public async Task<bool> Start(CommonCollectorServiceVariables settings)
        {
            ServiceVariables = settings;
            ValidateFuncs();
            _collectorHelper = new CollectorHelper(ServiceVariables, _logger);

            return await RunCollectionFlow().ConfigureAwait(false);
        }

        private void ValidateFuncs()
        {
            if (ConnectToApiManuallyDlg == null || PathToLogFolderDlg == null || _onFinished == null ||
                PathToArchivedLogsDlg == null)
                throw new Exception("Developer failure. Dialogues are not set.");
        }


        private async Task<bool> RunCollectionFlow()
        {
           
            _logger.Add($"Setting up connection to QlikView Server on {ServiceVariables.QvSettings.QmsAddress}");

            var dto = new QlikViewConnectDto
            {
                ConnectToQmsApiManuallyDlg = ConnectToApiManuallyDlg,
                QmsAddress = ServiceVariables.QvSettings.QmsAddress
            };

            await Task.Run(() =>
            {
                var helper = new ConnectToQlikViewHelper(_logger);
                dto = helper.ConnectToQmsApi(dto);
            }).ConfigureAwait(false);

            if (dto == null || dto.AbortAndExit)
            {
                AbortAndExit = true;
                _logger.Add("Aborting connection requested.");
                _notify("Failed Connecting to QlikView Installation", MessageLevels.Error, "Connecting");
                _notify("Aborting.", MessageLevels.Error, null);
                _onFinished(null, null, this);
                return false;
            }
            try
            {
                var taskList = new List<Task>();
                ServiceVariables.QvSettings.QmsAddress = dto.QmsAddress;
                //_apiCollector = new ApiCollector(_logger, _notify, ServiceVariables,dto);

                if (dto.RunWithDeadInstallation)
                {
                    _notify("Not connected to QlikView", MessageLevels.Warning, "Connecting");
                    dto = PathToLogFolderDlg.Invoke(dto);
                    if (dto.AbortAndExit)
                    {
                        _notify("Failed Connecting to QlikView Installation", MessageLevels.Error, "Connecting");
                        _notify("Aborting.", MessageLevels.Error, null);
                        _onFinished(null, null, this);
                        return false;
                    }
                    ServiceVariables.QvSettings.QvLogLocations.Add(new QvLogLocation{LogCollectionType = QvLogCollectionType.All, Name = "BasicLogs", Path = dto.PathToLocalLogFolder});
                    taskList.AddRange(GetTasksForWindows());
                    taskList.AddRange(GetDeadLogTasks());
                }
                else
                {
                    _notify("Connected to QlikView Installation", MessageLevels.Ok, "Connecting");
                    taskList.AddRange(GetApiTasks());

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

                        AbortAndExit = false;
                        _onFinished(null, null, this);
                    }
                ).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }

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
                        ba.ReadEvents(ServiceVariables.StartDateForLogs, ServiceVariables.StopDateForLogs, new CollectorHelper(ServiceVariables, _logger));
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

        private async Task<bool> RunCmdLineAgents()
        {
            var cmds = new CmdLineAgents(_filesystem, _logger);
            var res = await cmds.RunLocalCmds().ConfigureAwait(false);
            _collectorHelper.WriteContentToFile(res, "SystemInfo");
            return true;
        }

        private List<Task> GetDeadLogTasks()
        {
            var taskList = new List<Task>();
            taskList.Add(Task.Run(() =>
            {
                try
                {
                    var logCollector = new QvLogFileDirector(_logger, _filesystem, _notify);
                    logCollector.Execute(ServiceVariables);

                }
                catch (Exception e)
                {
                    _logger.Add("Failed getting logs",e);
                }

            }));
            return taskList;
        }



        private List<Task> GetApiTasks()
        {
            var taskList = new List<Task>();
            taskList.Add(Task.Run(()=>
            {
                try
                {
                    new ApiCollector(_logger, _notify, ServiceVariables).CollectFromApi();
                    var logCollector = new QvLogFileDirector(_logger, _filesystem, _notify);
                    logCollector.Execute(ServiceVariables);
                }
                catch (Exception e)
                {
                    _logger.Add("Failed Api collection", e);
                }
             
            }));
            return taskList;
        }
     
        
      
    }
}
