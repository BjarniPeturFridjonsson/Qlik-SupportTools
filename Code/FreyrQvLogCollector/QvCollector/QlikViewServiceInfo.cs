using System.Collections.Generic;

namespace FreyrQvLogCollector.QvCollector
{
    public class QlikViewServiceInfo
    {
        private QlikViewServiceInfo(string serviceName, string displayName, string shortName = "")
        {
            ServiceName = serviceName;
            DisplayName = displayName;
            ShortName = shortName;
        }

        public string ServiceName { get; }

        public string DisplayName { get; }

        public string ShortName { get; }

        public override string ToString() => DisplayName;

        public static class QlikView
        {
            public static readonly QlikViewServiceInfo Server = new QlikViewServiceInfo("QlikviewServer", "QlikView Server", "QVS");

            public static readonly QlikViewServiceInfo ManagementService = new QlikViewServiceInfo("QlikviewManagementService", "QlikView Management Service", "QMS");

            public static readonly QlikViewServiceInfo DistributionService = new QlikViewServiceInfo("QlikViewDistributionService", "QlikView Distribution Service", "QDS");
            
            public static readonly QlikViewServiceInfo Webserver = new QlikViewServiceInfo("QlikviewWebserver", "QlikView Webserver", "QVWS");

            public static readonly QlikViewServiceInfo DirectoryServiceConnector = new QlikViewServiceInfo("QlikviewDirectoryServiceConnector", "QlikView Directory Service Connector", "DSC");

            public static IEnumerable<QlikViewServiceInfo> All
            {
                get
                {
                    yield return Server;
                    yield return ManagementService;
                    yield return DistributionService;
                    yield return Webserver;
                    yield return DirectoryServiceConnector;
                }
            }
        }
    }
}
