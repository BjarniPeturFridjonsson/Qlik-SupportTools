using System;
using System.Collections.Generic;

namespace FreyrCommon.Models
{
    public class SenseLogInfo
    {
        public string LogFilePath { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public DateTime LastModified { get; set; }
        public List<SenseLogInfo> LogInfos { get; set; } = new List<SenseLogInfo>();
    }
}
