// ReSharper disable InconsistentNaming
//these names match the correct names from WMI.
namespace FreyrCommon.Models
{
    public class WindowsSystemInfo
    {
        public string HostName { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string OSManufacturer { get; set; }
        public string OSConfiguration { get; set; }
        public string OSBuildType { get; set; }
        public string RegisteredOwner { get; set; }
        public string RegisteredOrganization { get; set; }
        public string ProductID { get; set; }
        public string OriginalInstallDate { get; set; }
        public string SystemBootTime { get; set; }
        public string SystemManufacturer { get; set; }
        public string SystemModel { get; set; }
        public string SystemType { get; set; }
        public string Processors { get; set; }
        public string BIOSVersion { get; set; }
        public string WindowsDirectory { get; set; }
        public string SystemDirectory { get; set; }
        public string BootDevice { get; set; }
        public string SystemLocale { get; set; }
        public string InputLocale { get; set; }
        public string TimeZone { get; set; }
        public string TotalPhysicalMemory { get; set; }
        public string AvailablePhysicalMemory { get; set; }
        public string VirtualMemoryMaxSize { get; set; }
        public string VirtualMemoryAvailable { get; set; }
        public string VirtualMemoryInUse { get; set; }
        public string PageFileLocations { get; set; }
        public string Domain { get; set; }
        public string LogonServer { get; set; }
        public string Hotfixs { get; set; }
        public string NetworkCards { get; set; }
        public string HyperVRequirements { get; set; }
    }
}
