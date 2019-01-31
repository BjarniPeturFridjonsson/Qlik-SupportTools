using System;
using System.Collections.Generic;
using System.Management;
using FreyrCollectorCommon.CollectorDto;

namespace FreyrCollectorCommon.Collectors
{
    public class WmiCollector
    {
        public StandardInfo GetStandardInfo()
        {
            var ret = new StandardInfo
            {
                CurrentUser = Environment.UserName,
                PcName = Environment.MachineName,
                DomainName = Environment.UserDomainName
            };
            return ret;
        }
   
        public List<WmiPageFile> GetValuesFromWmiPagefile()
        {
            var ret = new List<WmiPageFile>();

            //var ret = new WmiPageFile();

            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PageFileUsage"))
            {

                if (searcher != null )
                {
                    foreach (var o in searcher.Get())
                    {
                        //var s = JsonConvert.SerializeObject(o);
                        
                        var mobj = (ManagementObject)o;
                        //var s = GetFlatWmiQuery(mobj);
                        var item = new WmiPageFile();
                        item.Name = GetValue(mobj, "Name");
                        item.AllocatedBaseSize = GetValue(mobj, "AllocatedBaseSize");
                        item.Caption = GetValue(mobj, "Caption");
                        item.CurrentUsage = GetValue(mobj, "CurrentUsage");
                        item.Description = GetValue(mobj, "Description");
                        item.InstallDate = GetValue(mobj, "InstallDate");
                        item.PeakUsage = GetValue(mobj, "PeakUsage");
                        item.Status = GetValue(mobj, "Status");
                        item.TempPageFile = GetValue(mobj, "TempPageFile");
                        item.Error = "0x0";
                        ret.Add(item);
                    }
                }
            }
            return ret;
        } 

        public List<WindowsServicesInfo> GetValuesFromWindowsServicesInfo()
        {
            // var toJson = new WindowsServicesInfo();
            var ret = new List<WindowsServicesInfo>();
            // var tupleList = new List<Tuple<WindowsServicesInfoCollection>>();
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT DisplayName,Name,ProcessID,Status,Started,StartMode FROM Win32_Service"))

                if (searcher != null)
                {
                    foreach (var o in searcher.Get())
                    {
                        var item = new WindowsServicesInfo();
                        var mobj = (ManagementObject)o;
                        item.DisplayName = GetValue(mobj, "DisplayName");
                        item.Name = GetValue(mobj, "Name");
                        // item.Description = GetValue(mobj, "Description");
                        item.ProcessId = GetValue(mobj, "ProcessId");
                        item.Status = GetValue(mobj, "Status");
                        item.Started = GetValue(mobj, "Started");
                        item.StartMode = GetValue(mobj, "StartMode");
                        item.Error = "0x0";
                        // tupleList.Add(Tuple.Create(toJson);
                        ret.Add(item);
                        //help getting the whole list back :(

                    }
                }
            return ret;
        }

        public Win32OsInfo GetValuesFromWin32Os()
        {
            var ret = new Win32OsInfo();
            var cpu = GetCpuInfo();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {

                foreach (var o in searcher.Get())
                {
                    var mobj = (ManagementObject) o;
                    ret.BootDevice = GetValue(mobj, "BootDevice");
                    ret.BuildNumber = GetValue(mobj, "BuildNumber");
                    ret.BuildType = GetValue(mobj, "BuildType");
                    ret.Caption = GetValue(mobj, "Caption");
                    ret.CodeSet = GetValue(mobj, "CodeSet");
                    ret.CountryCode = GetValue(mobj, "CountryCode");
                    ret.CsdVersion = GetValue(mobj, "CSDVersion");
                    ret.CsName = GetValue(mobj, "CSName");
                    ret.CurrentTimeZone = GetValue(mobj, "CurrentTimeZone");
                    ret.Description = GetValue(mobj, "Description");
                    ret.FreePhysicalMemory = GetValue(mobj, "FreePhysicalMemory");
                    ret.FreeSpaceInPagingFiles = GetValue(mobj, "FreeSpaceInPagingFiles");
                    ret.FreeVirtualMemory = GetValue(mobj, "FreeVirtualMemory");
                    ret.LocalDateTime = GetValueDate(mobj, "LocalDateTime");
                    ret.Locale = GetValue(mobj, "Locale");
                    ret.Manufacturer = GetValue(mobj, "Manufacturer");
                    ret.Name = GetValue(mobj, "Name");
                    ret.Organization = GetValue(mobj, "Organization");
                    ret.OsArchitecture = GetValue(mobj, "OSArchitecture");
                    ret.OsLanguage = GetValue(mobj, "OSLanguage");
                    ret.Organization = GetValue(mobj, "OSLanguage");
                    ret.ProductType = GetValue(mobj, "ProductType");
                    ret.ServicePackMajorVersion = GetValue(mobj, "ServicePackMajorVersion");
                    ret.ServicePackMinorVersion = GetValue(mobj, "ServicePackMinorVersion");
                    ret.SystemDevice = GetValue(mobj, "SystemDevice");
                    ret.SystemDirectory = GetValue(mobj, "SystemDirectory");
                    ret.SystemDrive = GetValue(mobj, "SystemDrive");
                    ret.TotalSwapSpaceSize = GetValue(mobj, "TotalSwapSpaceSize");
                    ret.TotalVirtualMemorySize = GetValue(mobj, "TotalVirtualMemorySize");
                    ret.TotalVisibleMemorySize = GetValue(mobj, "TotalVisibleMemorySize");
                    ret.Version = GetValue(mobj, "Version");
                    ret.DataExecutionPreventionAvailable = GetValue(mobj, "DataExecutionPrevention_Available");
                    ret.VirtualizationFirmwareEnabled = cpu.Item1;
                    ret.VmMonitorModeExtensions = cpu.Item2;
                    ret.SecondLevelAddressTranslationExtensions = cpu.Item3;
                    break;
                }
            }
            return ret;
        }

        public string GetHotFixes()
        {
            var ret = string.Empty;
            using (var searcher = new ManagementObjectSearcher("SELECT HotFIxId FROM win32_quickfixengineering"))
            {
                foreach (var o in searcher.Get())
                {
                    var mobj = (ManagementObject) o;
                    ret += GetValue(mobj, "HotFIxId") + ",";
                }
            }
            return ret.Substring(0, ret.Length > 1000 ? 1000 : ret.Length);//maxlength in database.
        }

        public Tuple<string, string, string> GetCpuInfo()
        {
            Tuple<string, string, string> ret = new Tuple<string, string, string>("", "", "");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor")) //values not supported in win2008 r2 and previous
                {
                    foreach (var o in searcher.Get())
                    {
                        var mobj = (ManagementObject)o;
                        ret = new Tuple<string, string, string>(GetValue(mobj, "VirtualizationFirmwareEnabled"), GetValue(mobj, "VMMonitorModeExtensions"), GetValue(mobj, "SecondLevelAddressTranslationExtensions"));
                        return ret; // if one cpu is virtual then all are :)
                    }
                }
            }
            catch
            {
                ret = new Tuple<string, string, string>("Win32_Processor not supported", "Win32_Processor not supported", "Win32_Processor not supported");
            }
          
            return ret;
        }
        private DateTime GetValueDate(ManagementObject mobj, string propertyName)
        {
            DateTime value;
            try
            {
                value = ManagementDateTimeConverter.ToDateTime(mobj[propertyName].ToString());
            }
            catch
            {
                value = DateTime.MinValue;
            }

            return value;
        }

        private string GetValue(ManagementObject mobj, string propertyName)
        {
            string value;
            try
            {
                value = mobj[propertyName]?.ToString();
            }
            catch (Exception ex)
            {
                value = "*** Error: " + ex.Message;
            }

            return value;
        }
    }
}
