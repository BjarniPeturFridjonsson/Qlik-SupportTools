using System;

namespace Bifrost.Model.Metadata
{
    public class DatapointObservation
    {
        public string Hostname { get; set; }
        public string DatapointName { get; set; }
        public long Count { get; set; }
        public DateTime LatestObservationTime { get; set; }
    }
}