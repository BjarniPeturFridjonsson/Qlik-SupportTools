using System;
using System.Diagnostics;
using System.IO;
using FreyrCommon.Logging;
using System.Management;

namespace FreyrQvLogCollector.QvCollector
{
    public class ServiceSupport
    {
        private readonly ILogger _logger;

        public ServiceSupport(ILogger logger)
        {
            _logger = logger;
        }

        private void VerboseLog(string text)
        {
            _logger.Add(text);
        }

        public bool IsServiceInstalled(QlikViewServiceInfo serviceInfo)
        {
            switch (GetServiceInfoData(serviceInfo).StartMode)
            {
                case ServiceStartMode.NotInstalled:
                case ServiceStartMode.Unknown:
                case ServiceStartMode.Disabled:
                    return false;

                case ServiceStartMode.Boot:
                case ServiceStartMode.System:
                case ServiceStartMode.Auto:
                case ServiceStartMode.Manual:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ServiceInfoData GetServiceInfoData(QlikViewServiceInfo serviceInfo)
        {
            try
            {
                VerboseLog($"Checking if service '{serviceInfo.DisplayName}' is installed...");

                var query = new SelectQuery($"SELECT StartMode, ProcessId, PathName FROM Win32_Service WHERE Name='{serviceInfo.ServiceName}'");

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    using (ManagementObjectCollection result = searcher.Get())
                    {
                        using (var enumerator = result.GetEnumerator())
                        {
                            if (!enumerator.MoveNext())
                            {
                                VerboseLog($"Service '{serviceInfo.DisplayName}' is NOT installed");
                                return new ServiceInfoData(serviceInfo, ServiceStartMode.NotInstalled, null, null, null, null);
                            }

                            string startModeString = enumerator.Current.Properties["StartMode"].Value.ToString();
                            VerboseLog($"Service '{serviceInfo.DisplayName}' is installed, StartMode='{startModeString}'");

                            ServiceStartMode serviceStartMode;
                            if (!Enum.TryParse(startModeString, true, out serviceStartMode))
                            {
                                serviceStartMode = ServiceStartMode.Unknown;
                            }

                            string processIdString = enumerator.Current.Properties["ProcessId"].Value.ToString();
                            uint unsignedProcessId;
                            uint.TryParse(processIdString, out unsignedProcessId);
                            int processId = unchecked((int)unsignedProcessId);

                            string productVersion = null;
                            string fileVersion = null;
                            DateTime? exeCreationDateTime = null;
                            string pathName = enumerator.Current.Properties["PathName"].Value.ToString().Trim('\"');
                            if (File.Exists(pathName))
                            {
                                try
                                {
                                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(pathName);
                                    productVersion = versionInfo.ProductVersion;
                                    fileVersion = versionInfo.FileVersion;
                                }
                                catch
                                {
                                }

                                try
                                {
                                    exeCreationDateTime = File.GetCreationTimeUtc(pathName);
                                }
                                catch
                                {
                                }
                            }

                            return new ServiceInfoData(
                                serviceInfo,
                                serviceStartMode,
                                processId,
                                productVersion,
                                fileVersion,
                                exeCreationDateTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Add($"Error when determining if service '{serviceInfo.DisplayName}' is installed {ex.GetNestedMessages()}.");
                return new ServiceInfoData(serviceInfo, ServiceStartMode.Unknown, null, null, null, null);
            }
        }
    }
}