using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using FreyrCommon.Logging;

namespace FreyrQvLogCollector.QvCollector
{
    public class ConnectToQlikViewHelper
    {
        private readonly ILogger _logger;
        private const string ApiGroupName = "QlikView Management API";

        public ConnectToQlikViewHelper(ILogger logger)
        {
            _logger = logger;
        }

        public QlikViewConnectDto ConnectToQmsApi(QlikViewConnectDto dto)
        {
            dto = TryAccessQmsApi(dto);
            if (dto.QlikViewServerLocationFinderStatus == QlikViewServerLocationFinderStatus.Success)
                return dto;
            dto = dto.ConnectToQmsApiManuallyDlg(dto); //ha ha
            return dto;
        }

        public bool CreateQvApiGroup(string hostname)
        {
            var machineName = CreateHostnameFromUriString(hostname);
            var memberGroup = ApiGroupName;
            var ad = new DirectoryEntry("WinNT://" + machineName + ",computer");
            string userPath = $"WinNT://{WindowsIdentity.GetCurrent().Name.Replace('\\', '/')},user";
            string groupPath = $"WinNT://{machineName}/{memberGroup}";

            var server = new DirectoryEntry($"WinNT://{machineName},Computer");
            bool exists = server.Children.Cast<DirectoryEntry>().Any(d => d.SchemaClassName.Equals("Group") && d.Name.Equals(memberGroup));
            if (!exists)
            {
                using (DirectoryEntry userGroup = ad.Children.Add(memberGroup, "group"))
                {
                    if (String.IsNullOrEmpty(userGroup.SchemaClassName) || 0 != string.Compare(userGroup.SchemaClassName, "group", true, CultureInfo.CurrentUICulture))
                        return false;
                    userGroup.CommitChanges();
                    
                }
            }
            var usrGroup2 = new DirectoryEntry(groupPath);
            usrGroup2.Invoke("Add", userPath);
            usrGroup2.CommitChanges();
            return true;
        }

        private bool IsMemberOfGroup(string machineName, string memberGroup)
        {
            try
            {
                string userName = WindowsIdentity.GetCurrent().Name;
                using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain))
                using (UserPrincipal domainUsr = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName))
                {
                    using (PrincipalContext machineContext = new PrincipalContext(ContextType.Machine, machineName))
                    using (GroupPrincipal grp = GroupPrincipal.FindByIdentity(machineContext, memberGroup))
                    using (UserPrincipal localUsr = UserPrincipal.FindByIdentity(machineContext, IdentityType.SamAccountName, userName))
                    {
                        if (grp != null && domainUsr != null) //local group and domain user
                        {
                            return grp.GetMembers(true).Contains(domainUsr);
                        }
                        if (grp != null && localUsr != null) //local group and local user
                        {
                            return grp.GetMembers(true).Contains(localUsr);
                        }
                    }  
                }
            }
            catch (Exception e)
            {
                _logger.Add("Failed getting Is Member of api group",e);
            }
            return false;
        }

        private string CreateHostnameFromUriString(string s)
        {
            return new Uri(s).Host.Split('.')[0];
        }

        public bool IsPartOfApiGroup(string hostname)
        {
            try
            {
                return  IsMemberOfGroup(CreateHostnameFromUriString(hostname), ApiGroupName);
            }
            catch (Exception e)
            {
                _logger.Add($"Failed getting host from hostname {hostname}", e);
            }
            return false;
        }

        public QlikViewConnectDto TryAccessQmsApi(QlikViewConnectDto dto)
        {
            try
            {
                _logger.Add($"Trying connecting to Qms API on {dto.QmsAddress}.");
                dto.QvManagementApiGroupDetected =  IsPartOfApiGroup(dto.QmsAddress);
                using (var qmsApiService = new QMS_API.AgentsQmsApiService(dto.QmsAddress))
                {
                    dto.QlikViewServerLocationFinderStatus = QlikViewServerLocationFinderStatus.UnknownFailure;
                    if (qmsApiService.TestConnection())
                        dto.QlikViewServerLocationFinderStatus = QlikViewServerLocationFinderStatus.Success;
                }
                return dto;
            }
            catch (Exception ex)
            {
                _logger.Add($"TryAccessQmsApi failed locating api on machine {dto.QmsAddress} with status {dto.QlikViewServerLocationFinderStatus} and exception {ex}");
                if (dto.QlikViewServerLocationFinderStatus == QlikViewServerLocationFinderStatus.Undefined)
                    dto.QlikViewServerLocationFinderStatus = QlikViewServerLocationFinderStatus.UnknownFailure;
                return dto;
            }
            
        }
    }
}
