using System;
using System.Management;

namespace Gjallarhorn.Common
{
    public class WmiSystemInfo
    {

        public WmiSystemInfoDto GetValuesFromWin32Os()
        {
            var ret = new WmiSystemInfoDto();
            var cpu = GetCpuInfo();
            var computerSystem = GetComputerSystem();
            var baseBoard = GetBaseBoard();
            var bios = GetBios();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {

                foreach (var o in searcher.Get())
                {
                    ret.VirtualizationFirmwareEnabled = cpu.Item1;
                    ret.VmMonitorModeExtensions = cpu.Item2;
                    ret.SecondLevelAddressTranslationExtensions = cpu.Item3;
                    ret.ComputerSystemManufacturer = computerSystem.Item1;
                    ret.ComputerSystemModel = computerSystem.Item2;
                    ret.BaseBoardManufacturer = baseBoard.Item1;
                    ret.BaseBoardProduct = baseBoard.Item2;
                    ret.BiosManufacturer = bios.Item1;
                    ret.BiosName = bios.Item2;
                    ret.BiosSerialNumber = bios.Item3;
                    ret.BiosVersion = bios.Item4;

                    var mobj = (ManagementObject)o;
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

                    break;
                }
            }
            return ret;
        }

        private Tuple<string, string, string, string> GetBios()
        {
            Tuple<string, string, string, string> ret = new Tuple<string, string, string, string>("", "", "", "");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mobj = (ManagementObject)o;
                        ret = new Tuple<string, string, string, string>(GetValue(mobj, "Manufacturer"), GetValue(mobj, "Name"), GetValue(mobj, "SerialNumber"), GetValue(mobj, "Version"));
                        return ret;
                    }
                }
            }
            catch
            {
                ret = new Tuple<string, string, string, string>("Win32_BIOS not supported", "Win32_BIOS not supported", "Win32_BIOS not supported", "Win32_BIOS not supported");
            }
            return ret;
        }


        private Tuple<string, string> GetBaseBoard()
        {
            Tuple<string, string> ret = new Tuple<string, string>("", "");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM win32_baseboard"))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mobj = (ManagementObject)o;
                        ret = new Tuple<string, string>(GetValue(mobj, "Manufacturer"), GetValue(mobj, "Product"));
                        return ret;
                    }
                }
            }
            catch
            {
                ret = new Tuple<string, string>("win32_baseboard not supported", "win32_baseboard not supported");
            }

            return ret;
        }

        private Tuple<string, string> GetComputerSystem()
        {
            Tuple<string, string> ret = new Tuple<string, string>("", "");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer,Model FROM Win32_ComputerSystem")) 
                {
                    foreach (var o in searcher.Get())
                    {
                        var mobj = (ManagementObject)o;
                        ret = new Tuple<string, string>(GetValue(mobj, "Manufacturer"), GetValue(mobj, "Model"));
                        return ret; 
                    }
                }
            }
            catch
            {
                ret = new Tuple<string, string>("Win32_ComputerSystem not supported", "Win32_ComputerSystem not supported");
            }

            return ret;
        }

        private Tuple<string, string, string> GetCpuInfo()
        {
            Tuple<string, string, string> ret = new Tuple<string, string, string>("", "", "");
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor")) //values not supported in win2008 r2 and previous
                {
                    foreach (var o in searcher.Get())
                    {//"select numberOfCores, numberOfLogicalProcessors,maxclockspeed,addressWidth, VirtualizationFirmwareEnabled, VMMonitorModeExtensions, SecondLevelAddressTranslationExtensions from Win32_Processor"
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
