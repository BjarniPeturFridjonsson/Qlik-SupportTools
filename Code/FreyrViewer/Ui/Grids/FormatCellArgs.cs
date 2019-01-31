using BrightIdeasSoftware;

namespace FreyrViewer.Ui.Grids
{
    internal class FormatCellArgs
    {
        public FormatCellArgs(
            OLVListSubItem item,
            int rowIndex,
            int displayIndex,
            int columnIndex,
            object cellValue)
        {
            RowIndex = rowIndex;
            DisplayIndex = displayIndex;
            ColumnIndex = columnIndex;
            Item = item;
            CellValue = cellValue;
        }

        /// <summary>
        /// Gets the row index of the cell
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the display index of the row
        /// </summary>
        public int DisplayIndex { get; }

        /// <summary>
        /// Gets the column index of the cell
        /// </summary>
        /// 
        /// <remarks>
        /// This is -1 when the view is not in details view.
        /// </remarks>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets the subitem of the cell
        /// </summary>
        /// 
        /// <remarks>
        /// This is null when the view is not in details view
        /// </remarks>
        public OLVListSubItem Item { get; }

        /// <summary>
        /// Gets the model value that is being displayed by the cell.
        /// </summary>
        /// 
        /// <remarks>
        /// This is null when the view is not in details view
        /// </remarks>
        public object CellValue { get; }
    }
}