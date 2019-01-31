using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Eir.Common.Logging;
using Eir.Common.Rest;
using FreyrViewer.Common;
using FreyrViewer.Extensions;
using FreyrViewer.Ui.Splashes;

/*
 * We are using exention of the basic list view called objectListView
 * 
 * http://objectlistview.sourceforge.net/cs/gettingStarted.html
 * http://objectlistview.sourceforge.net/cs/recipes.html
 * 
 * You can't use a group and allow user sorting by clicking columns.
 * this could be fixed using this method http://stackoverflow.com/questions/7661150/how-to-sort-items-in-objectlistview?rq=1
 * 
 */

namespace FreyrViewer.Ui.Grids
{
    internal class GridHelper
    {
        protected static readonly IOverlay EMPTY_LIST_MSG_OVERLAY;

        static GridHelper()
        {
            var emptyListMsgOverlay = new TextOverlay
            {
                Alignment = ContentAlignment.TopLeft,
                ReferenceCorner = ContentAlignment.TopLeft,
                AdornmentCorner = ContentAlignment.TopLeft,
                BorderWidth = 0,
                BorderColor = Color.Transparent,
                TextColor = Color.FromArgb(244,133,24),
                BackColor = Color.Transparent,
                Text = @"No data was found",
                CornerRounding = 0,
                Transparency = 0,
                Font = new Font("Segoe UI", 10f),
                InsetX = 0,
                InsetY = 0
            };

            EMPTY_LIST_MSG_OVERLAY = emptyListMsgOverlay;
        }

        protected GridHelper(FastDataListView grid)
        {
            Grid = grid;
        }

        public FastDataListView Grid { get; }
        public bool AutoResizeGrid { get; set; } = true;
    }

    internal class GridHelper<TRow> : GridHelper
    {
        private IEqualityComparer<TRow> _comparer;
        private bool _isLoaded;
        private ILoaderWithTimer _loaderWithTimer;
        private Splash _splash;
        private readonly Color _defaultAlternatingBackColor = Color.GhostWhite;
        private readonly ComponentDisposalHelper _disposalHelper;
        

        public GridSortHelper Sorter { get; } = new GridSortHelper();
        /// <summary>
        /// All things column wise is here.
        /// </summary>
        public ColumnHelper<TRow> Columns { get; }

        /// <summary>
        /// this is a callback if you need to access/change the data after the loader is finished.
        /// <para>(good for simple gui stuff)</para>
        /// </summary>
        public event Action<TRow[]> AfterLoad;

        /// <summary>
        /// This you can use fx to color the rows in different manner.
        /// </summary>
        public Action<TRow, FormatRowArgs> FormatRowAction { private get; set; }

        /// <summary>
        /// This can be used to manipulate a single cell in the grid
        /// </summary>
        public Action<TRow, OLVColumn, FormatCellArgs> FormatCellAction { private get; set; }

        public bool SelectFullRow { get; set; }

        public bool UseAlternatingBackColor { get; set; }

        /// <summary>
        /// Helper for the objectListView.
        /// </summary>
        /// <param name="grid">reference to the grid in your form</param>
        /// <param name="dataLoader">This is a function that returns the data.</param>
        /// <param name="disposalHelper"></param>
        public GridHelper(FastDataListView grid, Func<Task<IEnumerable<TRow>>> dataLoader, ComponentDisposalHelper disposalHelper)
            : this(grid, disposalHelper)
        {
            _loaderWithTimer = new LoaderWithTimer<IEnumerable<TRow>>(
                grid,
                dataLoader,
                OnAfterLoadData,
                HandleException);
        }

        /// <summary>
        /// Helper for the objectListView.
        /// </summary>
        /// <param name="grid">reference to the grid in your form</param>
        /// <param name="dataLoader">This is a function that returns the data.</param>
        /// <param name="disposalHelper"></param>
        public GridHelper(FastDataListView grid, Func<Task<ResponseObject<IEnumerable<TRow>>>> dataLoader, ComponentDisposalHelper disposalHelper)
            : this(grid, disposalHelper)
        {
            _loaderWithTimer = new LoaderWithTimer<ResponseObject<IEnumerable<TRow>>>(
                grid,
                dataLoader,
                x => OnAfterLoadData(x.Value),
                HandleException);
        }

        
        private GridHelper(FastDataListView grid, ComponentDisposalHelper disposalHelper)
            : base(grid)
        {
            SelectFullRow = true;
            Columns = new ColumnHelper<TRow>(this);

            _splash = SplashManager.ShowEmbeddedSplash(grid, SplashKind.Mini);
            _disposalHelper = disposalHelper;
            Grid.ColumnClick += Grid_ColumnClick;
        

            AddDisposedAction(() => Grid.ColumnClick -= Grid_ColumnClick);
        }
        public void ChangeData(Func<Task<IEnumerable<TRow>>> dataLoader )
        {

            _loaderWithTimer.Manualreload(dataLoader);
        }

        public void AddDisposedAction(Action action)
        {
            _disposalHelper.AddDisposedAction(action);
        }
        
        private void Grid_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter.PrimarySortColumn = Grid.PrimarySortColumn;
            Sorter.PrimarySortOrder = Grid.PrimarySortOrder;
            Sorter.SecondarySortColumn = Grid.SecondarySortColumn;
            Sorter.SecondarySortOrder = Grid.SecondarySortOrder;
        }

        public void SetComparer(IEqualityComparer<TRow> comparer)
        {
            _comparer = comparer;
        }

        public void SetComparer(Func<TRow, Guid> getRowId)
        {
            _comparer = new GenericEqualityComparer<TRow>(getRowId);
        }

        /// <summary>
        /// Will format the grid and load the data. (And start a timer if automatic reload is chosen)
        /// </summary>
        public void ReloadPeriodically(TimeSpan reloadInterval)
        {
            _loaderWithTimer.SetReloadPeriodically(reloadInterval);
        }

        public void ReloadOnce(bool showSplash = false)
        {
            if (showSplash)
            {
                _splash?.Dispose();
                _splash = SplashManager.ShowEmbeddedSplash(Grid, SplashKind.Mini);
            }

            _loaderWithTimer.SetReloadOnce();
        }

        /// <summary>
        /// Call before Reload!
        /// </summary>
        public void SetInitialSort(OLVColumn sortColumn, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (sortColumn != null)
            {
                Sorter.PrimarySortColumn = sortColumn;
                Sorter.PrimarySortOrder = sortOrder;
                Sorter.SecondarySortColumn = null;
                Sorter.SecondarySortOrder = SortOrder.Ascending;
            }
            else
            {
                Sorter.PrimarySortColumn = Grid.PrimarySortColumn;
                Sorter.PrimarySortOrder = Grid.PrimarySortOrder;
                Sorter.SecondarySortColumn = Grid.SecondarySortColumn;
                Sorter.SecondarySortOrder = Grid.SecondarySortOrder;
            }
        }

        public void CellClick(Action<TRow, CellClickEventArgs> onClick, Action onClickOutside = null)
        {
            EventHandler<CellClickEventArgs> gridOnCellClick = null;
            Grid.CellClick += gridOnCellClick = (sender, e) =>
            {
                if (e.ClickCount != 1)
                {
                    return;
                }

                if (e.RowIndex < 0)
                {
                    onClickOutside?.Invoke();
                    return;
                }

                onClick((TRow)e.Model, e);
            };

            AddDisposedAction(() => Grid.CellClick += gridOnCellClick);
        }
        
        public void CellDoubleClick(Action<TRow, CellClickEventArgs> onDoubleClick, Action onDoubleClickOutside = null)
        {
            EventHandler<CellClickEventArgs> gridOnCellClick = null;
            Grid.CellClick += gridOnCellClick = (sender, e) =>
            {
                if (e.ClickCount != 2)
                {
                    return;
                }

                if (e.RowIndex < 0)
                {
                    onDoubleClickOutside?.Invoke();
                    return;
                }

                onDoubleClick((TRow)e.Model, e);
            };

            AddDisposedAction(() => Grid.CellClick -= gridOnCellClick);
        }

        public IEnumerable<TRow> GetObjects()
        {
            return Grid.Objects.OfType<TRow>();
        }

        public void SelectObject(TRow row)
        {
            if (row != null)
            {
                Grid.SelectObjects(new[] { row });
            }
            else
            {
                Grid.SelectObjects(new ArrayList());
            }
        }

        public void SelectObjects(IEnumerable<TRow> rows)
        {
            Grid.SelectObjects(rows.ToList());
        }

        public IEnumerable<TRow> GetSelectedObjects()
        {
            return Grid.SelectedObjects.OfType<TRow>().ToArray();
        }

        /// <summary>
        /// set search string to empty to clear search
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="type"></param>
        public void Search(string searchValue, GridSearchType type)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                Grid.ModelFilter = null;
                return;
            }
            switch (type)
            {
                case GridSearchType.Contains:
                    Grid.ModelFilter = TextMatchFilter.Contains(Grid, searchValue);
                    break;
                case GridSearchType.Regex:
                    Grid.ModelFilter = TextMatchFilter.Regex(Grid, searchValue);
                    break;
                case GridSearchType.BeginsWith:
                    Grid.ModelFilter = TextMatchFilter.Prefix(Grid, searchValue);
                    break;
            }
        }

        private void HandleException(Exception exception)
        {
            var webException = exception.InnerException as WebException;
            //if ((webException?.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
            //{
            //    //ok we are not authorized. we could have been in hibernation and our 2 part auth is timed out.
            //    var frmMain = Application.OpenForms["FrmMain"];

            //    (frmMain as FrmMain)?.ShowSelectCustomerForm();
            //}
            

            if (!_loaderWithTimer.ReloadInterval.HasValue)
            {
                _splash.Dispose();
            }

            Grid.SetException(exception);
        }

        private void FormatGrid(IEnumerable collection)
        {
            if (FormatRowAction != null)
            {
                EventHandler<FormatRowEventArgs> gridOnFormatRow;
                Grid.FormatRow += gridOnFormatRow = (sender, e) =>
                {
                    var formatRowArgs = new FormatRowArgs(
                        e.Item,
                        e.RowIndex,
                        e.DisplayIndex,
                        e.UseCellFormatEvents);

                    FormatRowAction((TRow)e.Model, formatRowArgs);

                    e.UseCellFormatEvents = formatRowArgs.UseCellFormatEvents;
                };

                AddDisposedAction(() => Grid.FormatRow -= gridOnFormatRow);
            }

            if (FormatCellAction != null)
            {
                EventHandler<FormatCellEventArgs> gridOnFormatCell;
                Grid.FormatCell += gridOnFormatCell = (sender, e) =>
                {
                    var formatCellArgs = new FormatCellArgs(
                        e.SubItem,
                        e.RowIndex,
                        e.DisplayIndex,
                        e.ColumnIndex,
                        e.CellValue);

                    FormatCellAction((TRow)e.Model, e.Column, formatCellArgs);
                };

                AddDisposedAction(() => Grid.FormatCell -= gridOnFormatCell);
            }

            if (UseAlternatingBackColor)
            {
                Grid.AlternateRowBackColor = _defaultAlternatingBackColor;
                Grid.UseAlternatingBackColors = true;
            }

            Grid.SetObjects(collection);
            Grid.UseFiltering = true;
            Grid.HideSelection = false;
            Grid.FullRowSelect = SelectFullRow;
            Grid.EmptyListMsgOverlay = EMPTY_LIST_MSG_OVERLAY;
            SetSelectionSettings();
            Columns.RebuildColumns();

            Sort();
        }

        private void Sort()
        {
            if (Sorter.SecondarySortColumn != null)
            {
                Grid.Sort(Sorter.SecondarySortColumn, Sorter.SecondarySortOrder);
            }

            if (Sorter.PrimarySortColumn != null)
            {
                Grid.Sort(Sorter.PrimarySortColumn, Sorter.PrimarySortOrder);
            }
        }

        private void OnAfterLoadData(IEnumerable<TRow> data)
        {
            _splash.Dispose();

            if (!Grid.IsHandleCreated || Grid.IsDisposed)
            {
                return;
            }

            TRow[] array = data.ToArray();

            try
            {
                if (Grid.IsDisposed)
                {
                    return;
                }

                Grid.OnUiThread(() => SetGridData(array));
            }
            catch (Exception ex)
            {
                Log.To.Main.Add($"Failed invoke in OnAfterLoadData. FastDataListViewHelper<{typeof(TRow)}> with ex:{ex}");
            }

            AfterLoad?.Invoke(array);
        }

        private void SetGridData(IEnumerable<TRow> data)
        {
            if (Grid.IsDisposed) return;
            if (!_isLoaded)
            {
                FormatGrid(data);
                _isLoaded = true;
            }
            else
            {
                //todo: support for first load and subsequent loads with param.


                try
                {
                    Grid.BeginUpdate();

                    var fastObjectListDataSource = (FastObjectListDataSource)Grid.VirtualListDataSource;

                    if (_comparer == null)
                    {
                        fastObjectListDataSource.SetObjects(data);
                        Grid.SelectedItem = null;
                    }
                    else
                    {
                        List<TRow> newObjects = data.ToList();
                        var removedObjects = new List<TRow>();

                        TRow[] currentObjects = fastObjectListDataSource.ObjectList.Cast<TRow>().ToArray();
                        List<TRow> selectedObjects = Grid.SelectedObjects.OfType<TRow>().ToList();

                        // Iterate over the current rows
                        for (int i = currentObjects.Length - 1; i >= 0; i--)
                        {
                            TRow currentObject = currentObjects[i];

                            TRow newObject = newObjects.FirstOrDefault(x => _comparer.Equals(x, currentObject));
                            if (newObject == null)
                            {
                                // This row doesn't exist anymore.
                                removedObjects.Add(currentObject);
                                selectedObjects.Remove(currentObject);
                            }
                            else
                            {
                                newObjects.Remove(newObject);

                                if (!ReferenceEquals(currentObject, newObject))
                                {
                                    // This row still exist in the new set of data. Replace the object with the new one.
                                    fastObjectListDataSource.UpdateObject(i, newObject);

                                    if (selectedObjects.Remove(currentObject))
                                    {
                                        selectedObjects.Add(newObject);
                                    }
                                }
                            }
                        }

                        // Remove removed rows
                        fastObjectListDataSource.RemoveObjects(removedObjects);

                        // Add new rows
                        fastObjectListDataSource.AddObjects(newObjects);

                        // Set selection
                        Grid.SelectObjects(selectedObjects);
                    }

                    Grid.UpdateVirtualListSize();
                }
                finally
                {
                    Grid.EndUpdate();
                }

                // ----------------------------
                // Hall of Failed Trials.
                // ----------------------------
                // (Comment depth reveals trial #.)

                //var collection = data as ICollection;
                //Grid.ClearObjects();
                //Grid.AddObjects(collection);

                ////rows.AddRange(_rows);
                ////var rows = _grid.Objects as ArrayList;
                ////var collection = data as ICollection;
                ////if (rows != null)
                ////{
                ////    rows.Clear();
                ////    if (collection != null) rows.AddRange(collection);
                ////}
                //////else
                //////{
                //////    _olvFast.AddObjects(collection);
                //////}
                ////Grid.RefreshObject(rows);
            }

            Sort();

            if(AutoResizeGrid)
                Grid.AutoResizeColumns();
        }

        private void SetSelectionSettings()
        {
            var render = new HighlightTextRenderer { FillBrush = new SolidBrush(Color.FromArgb(0, 242, 11)) };
            Grid.DefaultRenderer = render;
        }
    }
}