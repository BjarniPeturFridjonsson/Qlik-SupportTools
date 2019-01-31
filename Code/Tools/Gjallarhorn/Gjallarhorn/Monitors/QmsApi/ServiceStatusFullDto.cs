using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    class ServiceStatusFullDto
    {
        public string ServerNodeName { get; set; }
        public string ServerNodeHostName { get; set; }
        public string ServiceTypeName { get; set; }
        public int ServiceStateId { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceStateName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
