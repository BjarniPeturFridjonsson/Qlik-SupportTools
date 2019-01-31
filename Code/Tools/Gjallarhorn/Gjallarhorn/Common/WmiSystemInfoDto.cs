using System;

namespace Gjallarhorn.Common
{
    public class WmiSystemInfoDto
    {
        public string VirtualizationFirmwareEnabled { get; set; }
        public string VmMonitorModeExtensions { get; set; }
        public string SecondLevelAddressTranslationExtensions { get; set; }
        public string BootDevice { get; set; }
        public string BuildNumber { get; set; }
        public string BuildType { get; set; }
        public string Caption { get; set; }
        public string CodeSet { get; set; }
        public string CountryCode { get; set; }
        public string CsdVersion { get; set; }
        public string CsName { get; set; }
        public string CurrentTimeZone { get; set; }
        public string Description { get; set; }
        public string FreePhysicalMemory { get; set; }
        public string FreeSpaceInPagingFiles { get; set; }
        public string FreeVirtualMemory { get; set; }
        public DateTime LocalDateTime { get; set; }
        public string Locale { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string OsArchitecture { get; set; }
        public string OsLanguage { get; set; }
        public string OsProductSuite { get; set; }
        public string ProductType { get; set; }
        public string ServicePackMajorVersion { get; set; }
        public string ServicePackMinorVersion { get; set; }
        public string SystemDevice { get; set; }
        public string SystemDirectory { get; set; }
        public string SystemDrive { get; set; }
        public string TotalSwapSpaceSize { get; set; }
        public string TotalVirtualMemorySize { get; set; }
        public string TotalVisibleMemorySize { get; set; }
        public string Version { get; set; }
        public string DataExecutionPreventionAvailable { get; set; }
        public string ComputerSystemManufacturer { get; set; }
        public string ComputerSystemModel { get; set; }
        public string BaseBoardManufacturer { get; set; }
        public string BaseBoardProduct { get; set; }
        public string BiosManufacturer { get; set; }
        public string BiosName { get; set; }
        public string BiosSerialNumber { get; set; }
        public string BiosVersion { get; set; }
    }
}
