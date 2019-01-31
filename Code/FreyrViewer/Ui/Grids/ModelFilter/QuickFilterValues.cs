using System.Drawing;

namespace FreyrViewer.Ui.Grids.ModelFilter
{
    public class QuickFilterValues
    {
        public string FriendlyName { get; set; }
        /// <summary>
        /// makes a NOT condition.
        /// </summary>
        public bool NegativeFilter { get; set; }
        public bool ColorFilter { get; set; }
        /// <summary>
        /// makes an AND group, the last item in the group shall have false IsGroup to signal end of the group.
        /// </summary>
        public bool IsGroup { get; set; }
        public Color RowColor { get; set; }
        public string ColumnName { get; set; }
        public string FilterValue { get; set; }
        public bool ToBeModifiedInFilterEditor { get; set; } = false;
    }
}
