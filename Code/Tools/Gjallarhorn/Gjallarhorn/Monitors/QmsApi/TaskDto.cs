using System;
using System.Collections.Generic;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class TaskDto
    {
        public AppDto App { get; set; }
        public TaskOperationalDto Operational { get; set; }
        public Guid Taskid { get; set; }
        public DateTime Createddate { get; set; }
        public DateTime Modifieddate { get; set; }
        public string Modifiedbyusername { get; set; }
        public bool? Ismanuallytriggered { get; set; }
        public string Name { get; set; }
        public int Tasktype { get; set; }
        public bool? Enabled { get; set; }
        public int Tasksessiontimeout { get; set; }
        public int Maxretries { get; set; }
        public List<string> Tags { get; set; }
        public bool? Impactsecurityaccess { get; set; }
        public string Schemapath { get; set; }
    }
}
