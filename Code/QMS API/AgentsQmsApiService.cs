using System;
using System.Collections.Generic;
using QMS_API.QMSBackend;

namespace QMS_API
{
    /// <summary>
    /// Used by the QvMonitorService.
    /// </summary>
    public class AgentsQmsApiService : QmsApiService
    {
        public AgentsQmsApiService(string address)
            : base(address)
        {
        }

        public List<ServiceInfo> GetServices(ServiceTypes serviceType = ServiceTypes.All)
        {
            return Client.GetServices(serviceType);
        }

        public QVSSettings GetQvsSettings(Guid qvsId, QVSSettingsScope scope)
        {
            return Client.GetQVSSettings(qvsId, scope);
        }

        public QDSSettings GetQdsSettings(Guid qvsId, QDSSettingsScope scope)
        {
            return Client.GetQDSSettings(qvsId, scope);
        }
        public List<ServiceStatus> GetServiceStatuses(List<Guid> svcGuids)
        {
            return Client.GetServiceStatuses(svcGuids);
        }

        public Dictionary<string, List<string>> GetQvsDocumentsAndUsers(Guid qvsId, QueryTarget target)
        {
            return Client.GetQVSDocumentsAndUsers(qvsId, target);
        }

        public List<DocumentNode> GetUserDocuments(Guid qvsId)
        {
            return Client.GetUserDocuments(qvsId);
        }

        public CALConfiguration GetCalConfiguration(Guid qvsId, CALConfigurationScope scope)
        {
            return Client.GetCALConfiguration(qvsId, scope);
        }

        public License GetLicense(LicenseType licenseType, Guid serviceId)
        {
            return Client.GetLicense(licenseType, serviceId);
        }

        public List<DocumentFolder> GetSourceDocumentFolders(Guid qvsId, DocumentFolderScope scope)
        {
            return Client.GetSourceDocumentFolders(qvsId, scope);
        }
        public List<DocumentNode> GetSourceDocumentNodes(Guid qvsId, Guid folderId, string relativePath)
        {
            return Client.GetSourceDocumentNodes(qvsId, folderId, relativePath);
        }
    }
}
