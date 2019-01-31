using System;

namespace Bifrost.Model
{
    public class ImportJobStatus
    {
        public Guid Id { get; set; }
        public long Processed { get; set; }
        public long Failed { get; set; }
        public long Total { get; set; }
        public ImportJobState State { get; set; }
    }

    public enum ImportJobState
    {
        None,
        Pending,
        Extracting,
        Ongoing,
        FinishedSuccess,
        FinishedFailure
    }
}
