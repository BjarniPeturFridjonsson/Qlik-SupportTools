using System;
using System.Collections.Generic;

namespace FreyrCommon.Models
{
    public class CommonCollectorOutput
    {
        public string ZipFile { get; set; }
        public string LogCollectorVersion { get; set; }
        public int LogFileCount { get; set; }
    }

    public class QvLogCollectorSettings
    {
        public string QmsAddress { get; set; }
        public List<QvLogLocation> QvLogLocations { get; set; } = new List<QvLogLocation>();
    }

    public class CommonCollectorServiceVariables
    {

        public string ApplicatonBaseName { get; set; }
        public bool UseOnlineDelivery { get; set; } = false;
        public bool AllowMachineInfo { get; set; } = false;
        public bool AllowWindowsLogs { get; set; } = false;
        public bool AllowSenseInfo { get; set; } = false;
        public bool AllowArchivedLogs { get; set; } = false;
        public bool AllowRemoteLogs { get; set; } = false;
        public bool GetLogsMain { get; set; } = true;
        public bool GetLogsScripting { get; set; } = false;
        public bool GetLogsPrinting { get; set; } = false;
        public DateTime StartDateForLogs { get; set; } = DateTime.Now.Date - TimeSpan.FromDays(1);
        public DateTime StopDateForLogs { get; set; } = DateTime.Now.Date;
        public string DnsHostName { get; set; }
        public Uri OnlineReccever { get; set; }
        public string SendId { get; set; } = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        public string CustomerKey { get; set; }
        public string Key { get; set; }
        public string LogFilePath { get; set; }
        public string OutputFolderPath { get; set; }
        public CommonCollectorOutput CollectorOutput { get; set; } = new CommonCollectorOutput();
        public QvLogCollectorSettings QvSettings { get; set; } = new QvLogCollectorSettings();
        public List<IssueRegister> Issues { get; set; } = new List<IssueRegister>();
    }
}
