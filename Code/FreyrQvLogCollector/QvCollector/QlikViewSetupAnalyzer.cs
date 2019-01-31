//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Xml;
//using Eir.Common.IO;
//using FreyrCollectorCommon.Winform;
//using FreyrCommon.Logging;
//using FreyrCommon.Models;

//namespace FreyrQvLogCollector.QvCollector
//{
//    public class QlikViewSetupAnalyzer
//    {
//        private readonly ILogger _logger;
//        private static readonly string _eventLogStartsWith = "Events_" + Environment.MachineName;
//        private static readonly string _sessionLogStartsWith = "Sessions_" + Environment.MachineName;
//        private Action<string, MessageLevels, string> _notify;
//        private CommonCollectorServiceVariables _serviceVariables;

//        public QlikViewSetupAnalyzer(ILogger logger, Action<string, MessageLevels, string> notify,CommonCollectorServiceVariables serviceVariables)
//        {
//            _logger = logger;
//            _notify = notify;
//            _serviceVariables = serviceVariables;
//        }

//        public QlikViewSetup AnalyzeSystem()
//        {
//            var serviceInfos = new List<QlikViewServiceInfo>();

//            DirectorySetting userDocDirectory, qvsLogPath;
//            FileSetting eventLog, sessionLog;
//            if (AddIfServiceInstalled(serviceInfos, QlikViewServiceInfo.QlikView.Server))
//            {
//                userDocDirectory = GetUserDocDir();
//                qvsLogPath = GetQvsLogPath();
//                eventLog = FindNewestLogFile(_eventLogStartsWith, qvsLogPath);
//                sessionLog = FindNewestLogFile(_sessionLogStartsWith, qvsLogPath);
//            }
//            else
//            {
//                userDocDirectory = DirectorySetting.Empty;
//                qvsLogPath = DirectorySetting.Empty;
//                eventLog = FileSetting.Empty;
//                sessionLog = FileSetting.Empty;
//            }

//            AddIfServiceInstalled(serviceInfos, QlikViewServiceInfo.QlikView.ManagementService);

//            DirectorySetting qdsPath = AddIfServiceInstalled(serviceInfos, QlikViewServiceInfo.QlikView.DistributionService)
//                ? GetQdsPath()
//                : DirectorySetting.Empty;

//            AddIfServiceInstalled(serviceInfos, QlikViewServiceInfo.QlikView.Webserver);

//            AddIfServiceInstalled(serviceInfos, QlikViewServiceInfo.QlikView.DirectoryServiceConnector);

//            return new QlikViewSetup(
//                serviceInfos,
//                userDocDirectory,
//                qvsLogPath,
//                eventLog,
//                sessionLog,
//                qdsPath);
//        }

//        private bool AddIfServiceInstalled(ICollection<QlikViewServiceInfo> serviceInfos, QlikViewServiceInfo serviceInfo)
//        {
//            var serviceSupport = new ServiceSupport(_logger);
//            if (!serviceSupport.IsServiceInstalled(serviceInfo))
//            {
//                return false;
//            }

//            serviceInfos.Add(serviceInfo);
//            return true;
//        }

//        private DirectorySetting GetQvsLogPath()
//        {
//            try
//            {
//                string settingsIniPath = GetQvsSettingsIniPath();
//                if (!File.Exists(settingsIniPath))
//                {
//                    return DirectorySetting.Empty;
//                }

//                string path;

//                var iniFileSupport = new IniFileSupport(_logger);
//                if (iniFileSupport.TryFindValue(settingsIniPath, "Settings 7", "ServerLogFolder", out path) &&
//                    Directory.Exists(path))
//                {
//                    return new DirectorySetting(path);
//                }

//                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QlikTech", "QlikViewServer");
//                if (Directory.Exists(path))
//                {
//                    return new DirectorySetting(path);
//                }

//                return DirectorySetting.Empty;
//            }
//            catch (Exception ex)
//            {
//                _logger.Add($"Error when trying to determine path for QVS Log Folder: {ex.GetNestedMessages()}");
//                return DirectorySetting.Empty;
//            }
//        }

//        private DirectorySetting GetUserDocDir()
//        {
//            try
//            {
//                string settingsIniPath = GetQvsSettingsIniPath();
//                if (!File.Exists(settingsIniPath))
//                {
//                    return DirectorySetting.Empty;
//                }

//                string path;

//                var iniFileSupport = new IniFileSupport(_logger);
//                if (iniFileSupport.TryFindValue(settingsIniPath, "Settings 7", "DocumentDirectory", out path) &&
//                    Directory.Exists(path))
//                {
//                    return new DirectorySetting(path);
//                }

//                return DirectorySetting.Empty;
//            }
//            catch (Exception ex)
//            {
//                _logger.Add($"Error when determining UserDoc path: {ex.GetNestedMessages()}");
//                return DirectorySetting.Empty;
//            }
//        }

//        private static string GetQvsSettingsIniPath()
//        {
//            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QlikTech", "QlikViewServer", "Settings.ini");
//        }

//        private DirectorySetting GetQdsPath()
//        {
//            try
//            {
//                string qdsPath = TryFindQdsPathVersion1120();
//                if (Directory.Exists(qdsPath))
//                {
//                    return new DirectorySetting(qdsPath);
//                }

//                qdsPath = TryFindQdsPathVersion1100();
//                if (Directory.Exists(qdsPath))
//                {
//                    return new DirectorySetting(qdsPath);
//                }

//                return new DirectorySetting(qdsPath);
//            }
//            catch (Exception ex)
//            {
//                _logger.Add($"Error when determining QDS path: {ex.GetNestedMessages()}");
//                return DirectorySetting.Empty;
//            }
//        }

//        private string TryFindQdsPathVersion1120()
//        {
//            string qdsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QlikTech", "DistributionService");

//            if (!Directory.Exists(qdsDir))
//            {
//                qdsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QlikTech", "Distribution Service");
//            }

//            if (Directory.Exists(qdsDir))
//            {
//                return ReadSingleXmlNodeAttribute(Path.Combine(qdsDir, "config_" + Environment.MachineName + ".xml"), "Value", "root/Setting[@Name=\"ApplicationDataFolder\"]");
//            }

//            return string.Empty;
//        }

//        private string TryFindQdsPathVersion1100()
//        {
//            var qdsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "QlikView", "Distribution Service");
//            if (!Directory.Exists(qdsDir))
//            {
//                qdsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "QlikView", "Distribution Service");
//            }

//            if (Directory.Exists(qdsDir))
//            {
//                string nodeValue = ReadSingleXmlNodeAttribute(Path.Combine(qdsDir, "QVDistributionService.exe.config"), "value", "configuration/appSettings/add[@key=\"ApplicationDataFolder\"]");

//                return string.IsNullOrWhiteSpace(nodeValue)
//                    ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "QlikTech", "DistributionService")
//                    : nodeValue;
//            }

//            return string.Empty;
//        }

//        private string ReadSingleXmlNodeAttribute(string filePath, string nodeName, string nodeXpath)
//        {
//            if (!File.Exists(filePath))
//            {
//                return string.Empty;
//            }

//            var xml = new XmlDocument();

//            try
//            {
//                xml.Load(filePath);
//            }
//            catch (Exception ex)
//            {
//                _logger.Add($"The xml config file '{filePath}' is invalid. ex: {ex}");
//                return string.Empty;
//            }

//            XmlNode node = xml.SelectSingleNode(nodeXpath);
//            if (node == null)
//            {
//                _logger.Add($"The config XML node '{nodeXpath}' does not exist.");
//                return string.Empty;
//            }
//            if (node.Attributes == null)
//            {
//                _logger.Add($"The config XML node has no attributes. File:{filePath}. Node:{nodeXpath}");
//                return string.Empty;
//            }

//            var attribute = node.Attributes[nodeName];
//            if (attribute == null)
//            {
//                _logger.Add(string.Format("The config XML node has no '{2}' attribute. File:{0}. Node:{1}", filePath, nodeXpath, nodeName));
//                return string.Empty;
//            }

//            return attribute.Value;
//        }

//        private FileSetting FindNewestLogFile(string startsWith, DirectorySetting directory)
//        {
//            try
//            {
//                FileInfo[] filesFound = Directory.GetFiles(directory.Path, startsWith + "*", SearchOption.TopDirectoryOnly)
//                    .Select(f => new FileInfo(f))
//                    .OrderByDescending(f => f.LastWriteTime)
//                    .ToArray();

//                foreach (FileInfo fileInfo in filesFound)
//                {
//                    // No rotating Log selected in QVS.
//                    if (fileInfo.Name.Equals(startsWith + ".log", StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        return new FileSetting(fileInfo.FullName);
//                    }

//                    // Rotating log.
//                    if (fileInfo.Name.StartsWith(startsWith + "_", StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        return new FileSetting(fileInfo.FullName);
//                    }
//                }

//                return FileSetting.Empty;
//            }
//            catch (Exception ex)
//            {
//                _logger.Add($"Error when determining the newest log file starting with '{startsWith}' in the dir '{directory}': {ex.GetNestedMessages()}");
//                return FileSetting.Empty;
//            }
//        }
//    }
//}