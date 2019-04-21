using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    [DataContract]
    public class BasicDataFromCase
    {
        [DataMember] public string CaseNumber { get; set; }
        [DataMember] public long TotalNrOfFiles { get; set; }
        [DataMember] public long TotalNrOfLines { get; set; }
        [DataMember] public string LicenseSerialNo { get; set; }
        [DataMember] public int NumberOfNodes { get; set; }
        [DataMember] public string LogCollectorRunDate { get; set; }
        [DataMember] public long ServiceSchedulerFailures { get; set; }
        [DataMember] public long ServiceSchedulerSuccess { get; set; }
        [DataMember] public int TotalUniqueActiveUsers { get; set; }
        [DataMember] public int TotalUniqueActiveApps { get; set; }
        [DataMember] public int TotalNrOfSessions { get; set; }
        [DataMember] public int SessionLengthMedInMinutes { get; set; }
        [DataMember] public int SessionLengthAvgInMinutes { get; set; }

        public DateTime OldestLogLine { get; set; }


        public Dictionary<string, int> TotalUniqueActiveUsersList { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> TotalUniqueActiveAppsList { get; set; } = new Dictionary<string, int>();
    }
}