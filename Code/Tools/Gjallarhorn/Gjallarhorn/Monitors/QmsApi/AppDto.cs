using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class AppDto
    {
        public StreamDto Stream { get; set; }
        public Guid AppId { get; set; }
        public string Name {get;set;}
        public DateTime Publishtime { get; set; }
        public bool Published { get; set; }
        public string Savedinproductversion { get; set; }
        public string Migrationhash { get; set; }
        public int Availabilitystatus { get; set; }
    }
}
