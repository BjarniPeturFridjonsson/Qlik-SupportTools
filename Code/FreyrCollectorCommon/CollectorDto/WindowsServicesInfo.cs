namespace FreyrCollectorCommon.CollectorDto
{
    public class WindowsServicesInfo
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        public string ProcessId { get; set; }
        public string Started { get; set; }
        public string StartMode { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
