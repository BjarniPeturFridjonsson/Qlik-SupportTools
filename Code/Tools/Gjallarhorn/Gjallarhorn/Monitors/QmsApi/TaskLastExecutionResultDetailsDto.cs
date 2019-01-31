using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class TaskLastExecutionResultDetailsDto
    {
        public Guid DetailsId { get; set; }
        public int Detailstype { get; set; }
        public string Message { get; set; }
        public DateTime Detailcreateddate { get; set; }
    }
}

