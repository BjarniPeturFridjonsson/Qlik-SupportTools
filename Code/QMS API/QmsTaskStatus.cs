using System;
using QMS_API.Enums;

namespace QMS_API
{
    //changes to this needs to be reflected in Valhalla.Model.QmsTaskStatus
    public class QmsTaskStatus
    {
        public Guid TaskId { get; set; }
        public string Category { get; set; }
        public string DocumentPath { get; set; }
        public string FinishedTime { get; set; }
        public string LastLogMessages { get; set; }
        public Guid Qdsid { get; set; }
        public string StartTime { get; set; }
        public string TaskSummary { get; set; }
        public QmsTaskStatusType Status { get; set; }
        public string TaskName { get; set; }
        public QmsTaskType TaskType { get; set; }
    }
}
