using BrightIdeasSoftware;

namespace FreyrViewer.Extensions
{
    public static class OLVColumnExtensions
    {
        public static OLVColumn SetWidth(this OLVColumn column, int width, int? min = null, int? max = null)
        {
            column.Width = width;
            column.MinimumWidth = min ?? -1;
            column.MaximumWidth = max ?? -1;
            return column;
        }
    }
}