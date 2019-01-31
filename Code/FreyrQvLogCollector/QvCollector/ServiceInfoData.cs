using System;

namespace FreyrQvLogCollector.QvCollector
{
    public class ServiceInfoData
    {
        public ServiceInfoData(
            QlikViewServiceInfo serviceInfo,
            ServiceStartMode startMode,
            int? pid,
            string productVersion,
            string fileVersion,
            DateTime? exeCreationDateTime)
        {
            ServiceInfo = serviceInfo;
            StartMode = startMode;
            Pid = pid;
            ProductVersion = productVersion;
            FileVersion = fileVersion;
            ExeCreationDateTime = exeCreationDateTime;
        }

        public QlikViewServiceInfo ServiceInfo { get; }

        public ServiceStartMode StartMode { get; }

        public int? Pid { get; }

        public string ProductVersion { get; }

        public string FileVersion { get; }

        public DateTime? ExeCreationDateTime { get; }
    }
}