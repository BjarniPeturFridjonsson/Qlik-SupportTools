namespace Gjallarhorn.Monitors.FileMonitor
{
    public class FileMonitorDto
    {
        public string RuleName { get; set; }
        public string[] FilePath { get; set; }
        public int FileOverdueInHours { get; set; }
        public string Filter { get; set; }
        public string NegativeFilter { get; set; }
        public bool FileShouldGrow { get; set; }
        public bool ScanSubdirectories { get; set; }
    }
}
