using BrightIdeasSoftware;

namespace FreyrViewer.Ui.Grids
{
    internal class FormatRowArgs
    {
        public FormatRowArgs(
            OLVListItem item,
            int rowIndex,
            int displayIndex,
            bool useCellFormatEvents)
        {
            Item = item;
            RowIndex = rowIndex;
            DisplayIndex = displayIndex;
            UseCellFormatEvents = useCellFormatEvents;
        }

        /// <summary>
        /// Gets the item of the cell
        /// </summary>
        public OLVListItem Item { get; }

        /// <summary>
        /// Gets the row index of the cell
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the display index of the row
        /// </summary>
        public int DisplayIndex { get; }

        /// <summary>
        /// Should events be triggered for each cell in this row?
        /// </summary>
        public bool UseCellFormatEvents { get; set; }
    }
}