using System;
using System.Collections.Generic;
using System.Linq;
using Eir.Common.Extensions;
using FreyrCollectorCommon.Common;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using FreyrQvLogCollector.Models;
using QMS_API.QMSBackend;
using Exception = System.Exception;

namespace FreyrQvLogCollector.Collectors
{
    public class ApiCollector
    {
        private readonly ILogger _logger;
        private readonly Action<string, MessageLevels, string> _notify;
        private readonly CollectorHelper _collectorHelper;
        //private readonly QlikViewConnectDto _dto;
        private readonly CommonCollectorServiceVariables _settings;

        public ApiCollector(ILogger logger, Action<string, MessageLevels, string> notify, CommonCollectorServiceVariables settings)
        {
            _logger = logger;
            _collectorHelper = new CollectorHelper(settings, _logger);
            _notify = notify;
            _settings = settings;
        }

        public void CollectFromApi()
        {
            try
            {
                var collectionStatus = MessageLevels.Ok;
                _notify("Starting Collecting from API", MessageLevels.Animate, "ApiCollector");
                using (var qmsApiService = new QMS_API.AgentsQmsApiService(_settings.QvSettings.QmsAddress))
                {
                    if (!qmsApiService.TestConnection())
                    {
                        _logger.Add("Could not connect to QMS API (" + _settings.QvSettings.QmsAddress + ")!");
                        _notify("Failed Collecting from API", MessageLevels.Error, "ApiCollector");
                        return;
                    }

                    var services = qmsApiService.GetServices();
                    List<Guid> serviceIDs = services.Select(t => t.ID).ToList();


                    //List<ServiceInfo> qvsServices = qmsApiService.GetServices(ServiceTypes.QlikViewServer);
                    _collectorHelper.WriteContentToFile(qmsApiService.GetServiceStatuses(serviceIDs), "QvServiceStatuses");
                    _collectorHelper.WriteContentToFile(services, "QvServices");

                    var qvServers = services.Where(p => p.Type == ServiceTypes.QlikViewServer | p.Type == ServiceTypes.QlikViewDistributionService).ToList();
                  
                    _notify("Collecting Service info from API", MessageLevels.Animate, "ApiCollector");
                    qvServers.ForEach(p =>
                    {
                        if (p.Type == ServiceTypes.QlikViewServer)
                        {

                            QVSSettings settings =null;
                            try
                            {
                                settings = qmsApiService.GetQvsSettings(p.ID, QVSSettingsScope.All);

                                _collectorHelper.WriteContentToFile(settings, $"QvsSettings_{p.Name}");
                                _collectorHelper.WriteContentToFile(qmsApiService.GetCalConfiguration(p.ID, CALConfigurationScope.All), $"QvsCals_{p.Name}");
                                _collectorHelper.WriteContentToFile(qmsApiService.GetUserDocuments(p.ID), $"QvsUserDocuments_{p.Name}");
                                _collectorHelper.WriteContentToFile(qmsApiService.GetQvsDocumentsAndUsers(p.ID, QueryTarget.Resource), $"QvsDocumentsAndUsers_{p.Name}");
                            }
                            catch (Exception e)
                            {
                                _logger.Add($"Failed collecting API details from {p.Name ?? "UndefinedService"}", e);
                                _notify($"Failed collecting details from {p.Name}", MessageLevels.Warning, "ApiCollector");
                                collectionStatus = MessageLevels.Warning;
                            }

                            _settings.QvSettings.QvLogLocations.Add(new QvLogLocation
                            {
                                Name = $"Qvs settings - {p.Name}",
                                Type = QvLogLocationSource.QvsSetting,
                                Path = settings?.Logging?.Folder ?? "Failed retrival"
                            });

                            _settings.QvSettings.QvLogLocations.Add(new QvLogLocation
                            {
                                Name = $"Qvs Root Folder - {p.Name}",
                                Type = QvLogLocationSource.QvsSetting,
                                Path = settings?.Folders?.UserDocumentRootFolder ?? "Failed retrival"
                            });
                        }

                        if (p.Type == ServiceTypes.QlikViewDistributionService)
                        {
                            var docs = GetAllDocumentsAndFolders(qmsApiService, p);
                            _collectorHelper.WriteContentToFile(docs, $"QvsDocumentsAndFolders_{p.Name}");

                            var qdsSettings = qmsApiService.GetQdsSettings(p.ID, QDSSettingsScope.All);
                            qdsSettings.General.ClusterInfo.ForEach(clusterInfo =>
                            {
                                var hostName = clusterInfo.Url.Host;
                                _settings.QvSettings.QvLogLocations.Add(new QvLogLocation
                                {
                                    Name = $"Programdata - QlikTech - {hostName}",
                                    Type = QvLogLocationSource.QdsClusterInfoUrl,
                                    Path = $"\\\\{hostName}\\c$\\ProgramData\\QlikTech"
                                });

                                _settings.QvSettings.QvLogLocations.Add(new QvLogLocation
                                {
                                    Name = $"ProgramFiles - QlikView - {hostName}",
                                    Type = QvLogLocationSource.QdsClusterInfoUrl,
                                    Path = $"\\\\{hostName}\\c$\\Program Files\\QlikView",
                                    LogCollectionType = QvLogCollectionType.SettingsOnly
                                });

                                _settings.QvSettings.QvLogLocations.Add(new QvLogLocation
                                {
                                    Name = $"CommonFiles - QlikTech - {hostName}",
                                    Type = QvLogLocationSource.QdsClusterInfoUrl,
                                    Path = $"\\\\{hostName}\\c$\\Program Files\\Common Files\\QlikTech",
                                    LogCollectionType = QvLogCollectionType.SettingsOnly
                                });

                            });
                            _settings.QvSettings.QvLogLocations.Add(new QvLogLocation{
                                Name = $"QDS Application DataFolder - {p.Name}",
                                Type = QvLogLocationSource.QdsSettingsApplicationDataFolder,
                                Path = qdsSettings.General.ApplicationDataFolder,
                                IgnorePaths = IgnoreQdsApplicationDataFolder,
                                LogCollectionType = QvLogCollectionType.SettingsOnly
                            });
                            _collectorHelper.WriteContentToFile(qdsSettings, $"QdsSettings_{p.Name}");
                        }
                        _collectorHelper.WriteContentToFile(qmsApiService.GetLicense(p.Type == ServiceTypes.QlikViewServer ? LicenseType.QlikViewServer : LicenseType.Publisher, p.ID), $"License_{p.Name}");
                    });
                    if (collectionStatus == MessageLevels.Ok)
                    {
                        _notify("Finished collectinging from API", MessageLevels.Ok, "ApiCollector");
                    }
                    else
                    {
                        _notify("Finished collectinging from API but some errors where found.", MessageLevels.Warning, "ApiCollector");
                    }
                    
                    //Analyze(qmsApiService);
                }
            }
            catch (Exception e)
            {
                _logger.Add("Failed collecting from API", e);
                _notify("Failed collecting from API", MessageLevels.Error, "ApiCollector");
            }
        }

        private bool IgnoreQdsApplicationDataFolder(string directoryName)
        {
            if (directoryName?.Length == 8 && directoryName.IsDigitsOnly())
            {
                var folderDate = new DateTime(int.Parse(directoryName.Substring(0,4)), int.Parse(directoryName.Substring(4, 2)), int.Parse(directoryName.Substring(6, 2)));
                var isInTimeSpan = (folderDate >= _settings.StartDateForLogs.Date && folderDate <= _settings.StopDateForLogs.Date);
                return isInTimeSpan;
            }
            return true;
        }

        private List<QvDocumentAndFolders> GetAllDocumentsAndFolders(QMS_API.AgentsQmsApiService qmsApiService, ServiceInfo info)
        {
            var ret = new List<QvDocumentAndFolders>();
            try
            {
                List<DocumentFolder> sourceDocumentsFolders = qmsApiService.GetSourceDocumentFolders(info.ID, DocumentFolderScope.General | DocumentFolderScope.Services);
                foreach (DocumentFolder sourceDocumentFolder in sourceDocumentsFolders.OrderBy(x => x.General.Path))
                {
                    var folder = new QvDocumentAndFolders
                    {
                        DocumentFolder = sourceDocumentFolder
                    };
                    // print all sub nodes of the current source document folder
                    folder.DocumentNodes = SourceDocumentNodes(qmsApiService, folder, string.Empty);
                    ret.Add(folder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return ret;
        }

        private List<QvDocumentAndFolders> SourceDocumentNodes(QMS_API.AgentsQmsApiService qmsApiService, QvDocumentAndFolders sourceDocumentFolder, string relativePath)
        {
            var ret = new List<QvDocumentAndFolders>();
            try
            {
                // retrieve all source document nodes of the given folder and under the specified relative path
                List<DocumentNode> sourceDocumentNodes = qmsApiService.GetSourceDocumentNodes(sourceDocumentFolder.DocumentFolder.Services.QDSID, sourceDocumentFolder.DocumentFolder.ID, relativePath);
                foreach (DocumentNode sourceDocumentNode in sourceDocumentNodes.OrderByDescending(x => x.IsSubFolder).ThenBy(x => x.Name))
                {
                    var node = new QvDocumentAndFolders
                    {
                        DocumentNode = sourceDocumentNode
                    };
                    // print all sub nodes of the current source document node if it represents a folder
                    if (sourceDocumentNode.IsSubFolder)
                    {
                        node.DocumentNodes = SourceDocumentNodes(qmsApiService, sourceDocumentFolder, relativePath + "\\" + sourceDocumentNode.Name);
                    }
                    ret.Add(node);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return ret;
        }
    }
}
