using System.Windows.Forms;
using BrightIdeasSoftware;

namespace FreyrViewer.Ui.Grids
{
    public class GridSortHelper
    {
        public OLVColumn PrimarySortColumn { get; set; }
        public OLVColumn SecondarySortColumn { get; set; }
        public SortOrder PrimarySortOrder { get; set; }
        public SortOrder SecondarySortOrder { get; set; }
    }
}
