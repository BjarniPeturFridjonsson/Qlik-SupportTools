using System.Collections.Generic;

namespace FreyrCommon.Models
{
    public class QlikSenseAppListShort
    {
        public string Id { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string PublishTime { get; set; }
        public string Published { get; set; }
        public string FileSize { get; set; }
        public string LastReloadTime { get; set; }
        public long SheetObjects { get; set; }
        public long AppObjects { get; set; }
    }
}
