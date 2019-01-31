using System.Collections.Generic;
using QMS_API.QMSBackend;

namespace FreyrQvLogCollector.Models
{
    public class QvDocumentAndFolders
    {
        public DocumentFolder DocumentFolder { get; set; }
        public DocumentNode DocumentNode { get; set; }
        public List<QvDocumentAndFolders> DocumentNodes { get; set; }
    }
}