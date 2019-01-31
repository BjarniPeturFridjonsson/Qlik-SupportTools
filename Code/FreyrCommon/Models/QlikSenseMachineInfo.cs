using System;

namespace FreyrCommon.Models
{
    public class QlikSenseMachineInfo
    {
        public string Name { get; set; }
        public string HostName { get; set; }
        public Guid ServiceClusterId { get; set; }
        public bool IsCentral { get; set; }
        public string NodePurpose { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
