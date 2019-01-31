using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FreyrViewer.Ui.Helpers;

namespace FreyrViewer.Ui.Grids
{
    internal class ColumnHelper<TRow>
    {
        private class ObjColumn
        {
            public ObjColumn(ColumnRenderType columnRenderType)
            {
                ColumnRenderType = columnRenderType;
            }

            public ColumnRenderType ColumnRenderType { get; }
        }

        private readonly FastDataListView _grid;
        private readonly GridHelper<TRow> _gridHelper;
        private readonly List<ObjColumn> _columns = new List<ObjColumn>();
        private readonly ConcurrentDictionary<object, string> _imageKeyToNameDict = new ConcurrentDictionary<object, string>();
        private bool _hasGroupByColumn;

        public ColumnHelper(GridHelper<TRow> gridHelper)
        {
            _grid = gridHelper.Grid;
            _gridHelper = gridHelper;
        }

        public void RebuildColumns()
        {
            _grid.SuspendLayout();
            _grid.RebuildColumns();
            
            for (int i = 0; i < _columns.Count; i++)
            {
                var column = _columns[i];

                switch (column.ColumnRenderType)
                {
                    case ColumnRenderType.FillColumn:
                        _grid.AllColumns[i].FillsFreeSpace = true;
                        _grid.AllColumns[i].MinimumWidth = 5; //otherwise it can disappear
                        if (_hasGroupByColumn) _grid.AllColumns[i].Sortable = false;
                        break;

                    case ColumnRenderType.HeaderSize:
                        _grid.AllColumns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        if (_hasGroupByColumn) _grid.AllColumns[i].Sortable = false;
                        break;

                    case ColumnRenderType.ColumnContent: //Yes both
                        _grid.AllColumns[i].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        _grid.AllColumns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                        if (_hasGroupByColumn) _grid.AllColumns[i].Sortable = false;
                        break;

                    case ColumnRenderType.GroupColumn:
                        _grid.ShowGroups = true;
                        //_grid.AlwaysGroupByColumn = _grid.AllColumns[i];
                        _grid.SortGroupItemsByPrimaryColumn = false;
                        if (_gridHelper.Sorter.PrimarySortColumn == null)
                        {//this cant be null
                            _gridHelper.Sorter.PrimarySortColumn = _grid.AllColumns[i];
                            _gridHelper.Sorter.PrimarySortOrder = SortOrder.Ascending;
                        }
                        _grid.BuildGroups(_grid.AllColumns[i], SortOrder.Ascending, _gridHelper.Sorter.PrimarySortColumn, _gridHelper.Sorter.PrimarySortOrder, _gridHelper.Sorter.SecondarySortColumn, _gridHelper.Sorter.SecondarySortOrder);
                        break;
                }
            }
            _grid.ResumeLayout();
        }

        /// <summary>
        /// Basic column but with something that looks like a hyperlink.
        /// <para>you decide what you want to happen.</para>
        /// </summary>
        public OLVColumn CreateHyperlinkColumn(
            string headerText,
            Func<TRow, string> getValue,
            Action<TRow> onClicked,
            ColumnRenderType columnType = ColumnRenderType.Default)
        {
            var column = CreateColumn(headerText, getValue, null, columnType);
            _grid.UseHyperlinks = true;
            column.Hyperlink = true;
            EventHandler<HyperlinkClickedEventArgs> gridOnHyperlinkClicked = (sender, args) =>
                {
                    if(args.Column.Text.Equals(column.Text))
                        onClicked((TRow) args.Model);
                    args.Handled = true;//disables the process start default process
                };
           
            _grid.HyperlinkClicked += gridOnHyperlinkClicked;
            _gridHelper.AddDisposedAction(() => _grid.HyperlinkClicked -= gridOnHyperlinkClicked);
            return column;
        }
        

        /// <summary>
        /// Create a standard column.
        /// </summary>
        public OLVColumn CreateColumn(
            string headerText,
            Func<TRow, string> getValue,
            ColumnRenderType columnType = ColumnRenderType.Default)
        {
            return CreateColumn(headerText, getValue, null, columnType);
        }

        /// <summary>
        /// Create a standard column.
        /// </summary>
        public OLVColumn CreateColumn<TColumn>(
            string headerText,
            Func<TRow, TColumn> getValue,
            Func<TColumn, string> valueToString,
            ColumnRenderType columnType = ColumnRenderType.Default)
        {
            OLVColumn column = new OLVColumn
            {
                AspectGetter = rowObject =>
                {
                    try
                    {
                        return getValue((TRow)rowObject);
                    }
                    catch
                    {
                        return null;
                    }

                },
                Text = headerText,
                Tag = _gridHelper
            };

            if (valueToString != null)
            {
                column.AspectToStringConverter = rowObject => rowObject == null ? "(NULL)" : valueToString((TColumn)rowObject);
            }

            _columns.Add(new ObjColumn(columnType));
            _grid.AllColumns.Add(column);
            return column;
        }

        /*
         how to use! 
             private GridImageKind EnabledImageGetter(object rowObject)
            {
                return ((Analyzer)rowObject).Enabled ? GridImageKind.check : GridImageKind.cross;            
            }
             var EnabledImages = new List<GridImageKind>() {GridImageKind.check,GridImageKind.cross};
            _gridHelper.CreateImageColumn(x => x.Enabled, EnabledImageGetter, EnabledImages, "Enabled");
        */

        ///  <summary>
        ///  Regular columns but with an image.
        /// 
        ///  </summary>
        /// <param name="getSortValue"></param>
        /// <param name="headerText"></param>
        /// <param name="columnType"></param>
        /// <param name="getRowImage">a function that returns a key as string in your image list but as object</param>
        public OLVColumn CreateImageColumn(
            string headerText,
            Func<TRow, IComparable> getSortValue,
            Func<TRow, GridImageKind> getRowImage,
            ColumnRenderType columnType = ColumnRenderType.Default)
        {
            return CreateImageColumn(
                headerText,
                getSortValue,
                getRowImage,
                GridImageHelper.GetImage,
                true,
                columnType);
        }


        ///  <summary>
        ///  Regular columns but with an image.
        ///  </summary>
        public OLVColumn CreateImageColumn<TImageKey>(
            string headerText,
            Func<TRow, IComparable> getSortValue,
            Func<TRow, TImageKey> funcThatRetunsImageListKeyString,
            Func<TImageKey, Image> getImage,
            bool cacheImagesInImageList,
            ColumnRenderType columnType = ColumnRenderType.Default)
        {
            var col = CreateColumn(headerText, getSortValue, null, columnType);

            col.ImageGetter = x =>
            {
                try
                {
                    TImageKey imageKey = funcThatRetunsImageListKeyString((TRow)x);

                    if (cacheImagesInImageList)
                    {
                        return GetImageName(imageKey, () => getImage(imageKey));
                    }

                    return getImage(imageKey);
                }
                catch
                {
                    return null;
                }
            };

            col.AspectToStringConverter = x => string.Empty; // do not show string
            return col;
        }

        private string GetImageName(object imageKey, Func<Image> getImage)
        {
            return _imageKeyToNameDict.GetOrAdd(
                imageKey,
                x =>
                {
                    if (_grid.SmallImageList == null)
                    {
                        _grid.SmallImageList = new ImageList();
                    }

                    string imageName = Guid.NewGuid().ToString();

                    _grid.SmallImageList.Images.Add(imageName, getImage());

                    return imageName;
                });
        }

        /// <summary>
        /// Creates a column that will only show a group. This disables all other groups and no sorting is possible.
        /// </summary>
        public OLVColumn CreateHiddenGroupByColumn<TColumn>(string headerText, Func<TRow, TColumn> getValue)
        {
            var groupColumn = CreateColumn(headerText, getValue, null, ColumnRenderType.GroupColumn);
            groupColumn.IsVisible = false;
            groupColumn.Groupable = true;
            _hasGroupByColumn = true; //we need to disable all other columns sorting after this.
            return groupColumn;
        }

        /// <summary>
        /// Regular columns but with a checkbox.
        /// </summary>
        public OLVColumn CreateCheckboxColumn(string headerText, Func<TRow, string> getValue, ColumnRenderType columnType = ColumnRenderType.Default)
        {
            var col = CreateColumn(headerText, getValue, null, columnType);
            col.CheckBoxes = true;
            col.TriStateCheckBoxes = true;
            col.FillsFreeSpace = false;
            var size = _grid.Font.GetTextSize(headerText);
            col.Width = (int)size.Width + _grid.Margin.Left + _grid.Margin.Right;
            return col;
        }
    }
}