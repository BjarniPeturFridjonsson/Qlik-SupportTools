using System;

namespace FreyrCommon.Models
{
    public class QlikSenseServiceInfo
    {
        public string ServiceType { get; set; }
        public string HostName { get; set; }
        public Guid ServiceClusterId { get; set; }
        public string ServiceState { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
