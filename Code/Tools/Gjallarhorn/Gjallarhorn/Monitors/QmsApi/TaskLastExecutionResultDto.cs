using System;
using System.Collections.Generic;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class TaskLastExecutionResultDto
    {
        public List<TaskLastExecutionResultDetailsDto> LastExecutionResultDetails { get; set; }
        public Guid LastexecutionresultId { get; set; }
        public string Executingnodename { get; set; }
        public int Status { get; set; }
        public string StatusName{ get; set; }
        public DateTime Starttime { get; set; }
        public DateTime Stoptime { get; set; }
        public long Duration { get; set; }
        public Guid Filereferenceid { get; set; }
        public bool Scriptlogavailable { get; set; }
    }
}
