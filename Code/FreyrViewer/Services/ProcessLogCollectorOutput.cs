using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCommon.Models;
using FreyrViewer.Common;
using Newtonsoft.Json;

namespace FreyrViewer.Services
{

    public class ProcessLogCollectorOutput
    {

        private List<EventLogEntryShort> _windowsEventLogItems;
        private string _windowsEventLogPath;
        public List<GroupedServerInfo> GroupedServerInfo { get; }
        public List<SuperSimpleColumnTypes.TwoColumnType> QrsAbout { get; private set; }
        public List<SuperSimpleColumnTypes.TwoColumnType> LicenseAgent { get; private set; }
        public List<SuperSimpleColumnTypes.TwoColumnType> CalInfo { get; set; }
        public List<SuperSimpleColumnTypes.TwoColumnType> WmiWin32 { get; private set; }
        public ProcessCmdLineOutput CmdLineOutput { get; private set; }
        public string LogCollectorLog => GetLogCollectorLog();
        public CommonCollectorServiceVariables LogCollectorSettings => GetLogCollectorSettings();

        public string RecidingLogFolder { get; }
       

        private readonly IFileSystem _fileSystem;
        private readonly SimpleFileDataParser _parser;

        public ProcessLogCollectorOutput(string dirPath, IFileSystem fileSystem)
        {
            List<QlikSenseMachineInfo> qlikSenseMachineInfos = new List<QlikSenseMachineInfo>();
            List<QlikSenseServiceInfo> qlikSenseServiceInfos = new List<QlikSenseServiceInfo>();
            var servInfo = new List<GroupedServerInfo>();

            _fileSystem = fileSystem;
            _parser = new SimpleFileDataParser(_fileSystem);
            RecidingLogFolder = dirPath;
            FileAttributes attr = File.GetAttributes(dirPath);
            if (string.IsNullOrWhiteSpace(dirPath)) return;

           

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                if (!_fileSystem.DirectoryExists(dirPath)) return;
                _fileSystem.EnumerateFiles(dirPath,"*.json").ToList().ForEach(p =>
                 {

                     if (_fileSystem.Path.GetFileName(p).StartsWith("qlikSenseMachineInfo"))
                         qlikSenseMachineInfos = _parser.ParseJsonFileDataList<QlikSenseMachineInfo>(p,null);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("qlikSenseServiceInfo"))
                         qlikSenseServiceInfos = _parser.ParseJsonFileDataList< QlikSenseServiceInfo>(p,null);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("CalInfo"))
                         CalInfo = _parser.ParseJson2Column(p);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("LicenseAgent"))
                         LicenseAgent = _parser.ParseJson2Column(p);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("QrsAbout"))
                         QrsAbout = _parser.ParseJson2Column(p);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("WmiWin32"))
                         WmiWin32 = _parser.ParseJson2Column(p);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("SystemInfo"))
                         CmdLineOutput = ParseCmdLine(p);
                     else if (_fileSystem.Path.GetFileName(p).StartsWith("WindowsEvents"))
                         _windowsEventLogPath = p;
                 });
                //if (qlikSenseMachineInfos == null || !qlikSenseMachineInfos.Any())
                ParseByFolderName(dirPath, qlikSenseMachineInfos);
                if (qlikSenseMachineInfos != null && qlikSenseMachineInfos.Any())
                {

                    qlikSenseMachineInfos.ForEach(pMachineInfo =>
                    {
                        servInfo.Add(new GroupedServerInfo
                        {
                            LogFolderPath = FileSystem.Singleton.Path.Combine(RecidingLogFolder, pMachineInfo.HostName),
                            Logs = GetAllLogs(FileSystem.Singleton.Path.Combine(RecidingLogFolder, pMachineInfo.HostName)),
                            QlikSenseMachineInfo = pMachineInfo,
                            QlikSenseServiceInfo = qlikSenseServiceInfos.Where(p => pMachineInfo.HostName.Equals(p.HostName)).ToList(),
                            SystemInfo = GetSystemInfo(pMachineInfo.HostName)
                        });
                    });
                }
            }
            
          
            var logsInRoot = GetAllLogs(dirPath,false);
            if (logsInRoot.Count > 0)
            {
                servInfo.Add(new GroupedServerInfo
                {
                    LogFolderPath = dirPath,
                    Logs = logsInRoot,
                    QlikSenseMachineInfo = new QlikSenseMachineInfo { HostName = "(unknown)"}
                });
            }

            GroupedServerInfo = servInfo;
        }

        private void ConvertionProgress(string msg)
        {
            Switchboard.Instance.SetInfoMessage(msg);
        }

        public async Task<List<EventLogEntryShort>> WindowsEventLogItems(Action<string> jsonErrors)
        {
            if (string.IsNullOrEmpty(_windowsEventLogPath)) return null;
            await Task.Factory.StartNew(() =>
            {
                if (_windowsEventLogItems == null)
                {
                    if (_windowsEventLogPath.EndsWith(".evtx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _windowsEventLogItems = new ConvertEvtxToEventLogEntryShort().Convert(_windowsEventLogPath, ConvertionProgress);
                    }
                    else
                    {
                        _windowsEventLogItems = _parser.ParseJsonFileDataList<EventLogEntryShort>(_windowsEventLogPath, jsonErrors);
                    }
                        
                }
                    
                return _windowsEventLogItems;
            });
            return _windowsEventLogItems;
        }

        private List<SenseLogInfo> GetAllLogs(string path,bool recursive = true)
        {
            var ret = new List<SenseLogInfo>();
            if (File.Exists(path))
            {
                AddToFile(path, ret);
            }
            else
            {
                CrawlFolders(path, ret, recursive);
            }
            
            return ret;
        }

        private void CrawlFolders(string path, List<SenseLogInfo> info, bool recursive)
        {
            if (!_fileSystem.DirectoryExists(path))
                return; 
            if (recursive)
            {
                var dirs = _fileSystem.EnumerateDirectories(path).ToList();
                dirs.ForEach(p =>
                {
                    var dir = new SenseLogInfo
                    {
                        Name = _fileSystem.Path.GetFileName(p),
                        IsDirectory = true,
                        LogFilePath = p,
                    };
                    info.Add(dir);
                    CrawlFolders(p, dir.LogInfos, true);

                });
            }
            _fileSystem.EnumerateFiles(path, "*.*").ToList().ForEach(pfile =>
            {
                AddToFile(pfile,info);

            });
        }

        private void AddToFile(string pfile, List<SenseLogInfo> info)
        {
            if (pfile.EndsWith("location.txt", StringComparison.InvariantCultureIgnoreCase) || pfile.EndsWith("SenseCollector.log", StringComparison.InvariantCultureIgnoreCase)) return;
            var ext = _fileSystem.Path.GetExtension(pfile).ToLower();
            if (ext.Equals(".log") || ext.Equals(".txt"))
            {
                var analyze = new GenericDataWrapperService();
                analyze.LoadToInfo(pfile);
                var a = _fileSystem.GetFileInfo(pfile).LastWriteTime;
                if (a < DateTime.Parse("1970-01-01"))
                    a = DateTime.Parse("1970-01-01");
                info.Add(new SenseLogInfo
                {
                    Name = _fileSystem.Path.GetFileName(pfile),
                    IsDirectory = false,
                    LogFilePath = pfile,
                    LastModified =a,
                });
            }
            else if (ext.Equals(".evtx") && string.IsNullOrEmpty(_windowsEventLogPath))
            {
                _windowsEventLogPath = pfile;
            } 
        }

        private void ParseByFolderName(string path, List<QlikSenseMachineInfo> machineInfo)
        {
            var dirs = _fileSystem.EnumerateDirectories(path).ToList();
            dirs.ForEach(p =>
            {
                if (machineInfo.FirstOrDefault(info => info.HostName.Equals(_fileSystem.Path.GetFileName(p),StringComparison.InvariantCultureIgnoreCase)) != null)
                    return;
                machineInfo.Add(new QlikSenseMachineInfo
                {
                    HostName = _fileSystem.Path.GetFileName(p),
                    IsCentral = false,
                    NodePurpose = "Unknown",
                    ServiceClusterId = Guid.Empty
                });
            });
        }

        private ProcessCmdLineOutput ParseCmdLine(string filepath)
        {
            string data = _fileSystem.GetReader(filepath)?.ReadToEnd();
            var processor = new ProcessCmdLineOutput().ProcessJson(data);
            return processor;
        }

        private List<SuperSimpleColumnTypes.TwoColumnType> GetSystemInfo(string hostName)
        {
            var ret = new List<SuperSimpleColumnTypes.TwoColumnType>();
            var path = _fileSystem.Path.Combine(RecidingLogFolder, hostName);
            if (!_fileSystem.DirectoryExists(path))
                return ret;
            _fileSystem.EnumerateFiles(path, "*.json").ToList().ForEach(p =>
            {
                if (_fileSystem.Path.GetFileName(p).StartsWith("SystemInfo"))
                {
                    string data;
                    try
                    {
                        data = _fileSystem.GetReader(p)?.ReadToEnd();
                        ret = new ProcessCmdLineOutput().GetResutlAsSystemInfo(data);
                    }
                    catch (Exception e)
                    {
                        //create some sort of value for display.
                        Console.WriteLine(e);
                        throw;
                    }
                }
            });
            return ret;
        }

        private CommonCollectorServiceVariables GetLogCollectorSettings()
        {
            if (LogCollectorLog == null)
                GetLogCollectorLog();
            var lines = LogCollectorLog.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var line = lines.FirstOrDefault(p => p.Contains("UseOnlineDelivery") && p.Contains("CollectorOutput"));
            if (line == null)
                return null;
            string s = line.Substring(line.IndexOf("{", StringComparison.InvariantCulture));
            var ret = JsonConvert.DeserializeObject<CommonCollectorServiceVariables>(s);
            return ret;
        }

        private string GetLogCollectorLog()
        {
            string path = _fileSystem.Path.Combine(RecidingLogFolder, "SenseCollector.log");
            if(!_fileSystem.FileExists(path))
                return $"Did not find the log file {path}";

            return _fileSystem.GetReader(path)?.ReadToEnd();
        }
    }
}
