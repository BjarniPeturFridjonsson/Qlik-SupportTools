using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Timers;
using FreyrCommon.Logging;
using Newtonsoft.Json;

namespace FreyrCollectorCommon.Collectors
{
    public class PerformanceCollector
    {
        private Timer _timer;
        private readonly ILogger _logger;

        public PerformanceCollector(ILogger logger)
        {
            _logger = logger;
        }

        public void StartWmiCollection()
        {
            _timer = new Timer();
            _timer.Elapsed += OnTimedEvent;
            _timer.Interval = TimeSpan.FromSeconds(15).TotalMilliseconds;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private Dictionary<string, string> GetDiskInformation()
        {
            var jsons = new Dictionary<string, string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Removable)
                {
                    
                    var driveLetter = drive.Name.Substring(0, 1);
                    jsons.Add("Win32_PerfFormattedData_PerfDisk_LogicalDisk", GetFlatWmiQuery($"SELECT DiskTransfersPersec, AvgDisksecPerRead, AvgDisksecPerWrite, AvgDisksecPerRead, AvgDisksecPerWrite, AvgDiskQueueLength FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk WHERE   Name=\"{driveLetter}:\""));
                    jsons.Add("Win32_PerfFormattedData_PerfDisk_PhysicalDisk", GetFlatWmiQuery($"SELECT CurrentDiskQueueLength, PercentIdleTime Win32_PerfFormattedData_PerfDisk_PhysicalDisk WHERE Name=\"{driveLetter}:\""));
                }
            }

            return jsons;
        }

        private string GetFlatWmiQuery(string query)
        {
            try
            {
                var s = "";
                using (var searcher = new ManagementObjectSearcher("root\\CIMV2", query))
                {
                    foreach (var o in searcher.Get())
                    {
                        //var item = new WindowsServicesInfo();
                        var mobj = (ManagementObject)o;

                        foreach (PropertyData prop in mobj.Properties)
                        {
                            s += $",\r\n{JsonConvert.ToString(prop.Name)} = {JsonConvert.ToString(prop.Value)}";
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(s) || s.Length < 3)
                    return "";
                s = $"{{{s.Substring(1)}\r\n}}";
                return s;
            }
            catch (Exception e)
            {
                _logger.Add($"Failed running wmi query => {query}", e);
                return "{}";
            }
        }
        //"SELECT  FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk WHERE   Name=\"C:\"";
        //"SELECT  FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk WHERE   Name=\"C:\"";
        //"SELECT  FROM Win32_PerfFormattedData_PerfDisk_PhysicalDisk WHERE   Name=\"0 C:\"";
        //"SELECT  FROM Win32_PerfFormattedData_PerfDisk_PhysicalDisk WHERE   Name=\"0 C:\"";

        //"SELECT AvailableMBytes FROM Win32_PerfFormattedData_PerfOS_Memory";
        //"SELECT FreeSystemPageTableEntries FROM Win32_PerfFormattedData_PerfOS_Memory";
        //"SELECT PagesInputPersec FROM Win32_PerfFormattedData_PerfOS_Memory";
        //"SELECT PagesPersec FROM Win32_PerfFormattedData_PerfOS_Memory";
        //"SELECT OutputQueueLength FROM Win32_PerfFormattedData_Tcpip_NetworkInterface WHERE   Name=\"Intel[R] 82574L Gigabit Network Connection\"";
        //"SELECT BytesTotalPersec FROM Win32_PerfFormattedData_Tcpip_NetworkInterface WHERE   Name=\"Intel[R] 82574L Gigabit Network Connection\"";
        //"SELECT CurrentBandwidth FROM Win32_PerfFormattedData_Tcpip_NetworkInterface WHERE   Name=\"Intel[R] 82574L Gigabit Network Connection\"";
        //"SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE   Name=\"_Total\"";
        //"SELECT PercentPrivilegedTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE   Name=\"_Total\"";
        //"SELECT PercentInterruptTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE   Name=\"_Total\"";
        //"SELECT ContextSwitchesPersec FROM Win32_PerfFormattedData_PerfOS_System";
        //"SELECT ProcessorQueueLength FROM Win32_PerfFormattedData_PerfOS_System";
        //"SELECT  FROM Win32_PerfFormattedData_PerfDisk_PhysicalDisk WHERE   Name=\"0 C:\"";
        //"SELECT CurrentDiskQueueLength FROM Win32_PerfFormattedData_PerfDisk_PhysicalDisk WHERE   Name=\"0 C:\"";

    }

}
