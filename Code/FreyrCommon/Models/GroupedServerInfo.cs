using System.Collections.Generic;

namespace FreyrCommon.Models
{
    public class GroupedServerInfo
    {
        public string LogFolderPath { get; set; }
        public List<SenseLogInfo> Logs { get; set; }
        public QlikSenseMachineInfo QlikSenseMachineInfo { get; set; } = new QlikSenseMachineInfo();
        public List<QlikSenseServiceInfo> QlikSenseServiceInfo { get; set; } = new List<QlikSenseServiceInfo>();
        public List<SuperSimpleColumnTypes.TwoColumnType> SystemInfo { get; set; } = new List<SuperSimpleColumnTypes.TwoColumnType>();
    }
}
