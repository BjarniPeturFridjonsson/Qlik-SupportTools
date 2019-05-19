using System;
using System.Collections.Generic;

namespace Gjallarhorn.QvLogReading
{
    public class QvSessionData
    {
        public List<int> SessionLenghts { get; set; } = new List<Int32>();
        public Dictionary<string, int> Users { get; set; } = new Dictionary<String, Int32>();
        public Dictionary<string, int> Apps { get; set; } = new Dictionary<String, Int32>();
        public DateTime TheDay { get; set; }    
    }
}
