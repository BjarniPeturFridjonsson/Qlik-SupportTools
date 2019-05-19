using System;
using System.Collections.Generic;

namespace Gjallarhorn.QvLogReading
{
    public class QvSessionData
    {
        public List<int> SessionLenghts { get; set; }
        public Dictionary<string, string> Users { get; set; }
        public Dictionary<string, string> Apps { get; set; }
        public DateTime TheDay { get; set; }
    }
}
