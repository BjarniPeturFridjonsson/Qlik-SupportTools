using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class TaskOperationalDto
    {
        public Guid OperationalId { get; set; }
        public TaskLastExecutionResultDto LastExecutionResult { get; set; }
        public DateTime NextExecution { get; set; }
    }
}
