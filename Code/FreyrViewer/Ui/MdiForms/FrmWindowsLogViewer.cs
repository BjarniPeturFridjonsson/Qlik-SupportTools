using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FreyrCommon.Models;
using FreyrViewer.Common;
using FreyrViewer.Models;
using FreyrViewer.Ui.Controls;
using FreyrViewer.Ui.Grids;
using FreyrViewer.Ui.Grids.ModelFilter;
using FreyrViewer.Ui.Splashes;

namespace FreyrViewer.Ui.MdiForms
{
    public partial class FrmWindowsLogViewer : FrmBaseForm
    {
        private Dictionary<string, DataWrapper> _gridAndData;
        private GridAndDataWrapper<EventLogEntryShort> _currentGrid;

        private class DataWrapper
        {
            //public GenericDataWrapperService GenericDataWrapperService { get; } = new GenericDataWrapperService();
            public GridAndDataWrapper<EventLogEntryShort> GridAndDataWrapper { get; } = new GridAndDataWrapper<EventLogEntryShort>();
            //public GridWrapperControl<EventLogEntryShort> GridWrapperControl { get; set; }
        }

        public FrmWindowsLogViewer(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
        }

        private void InvokeGrid(TabPage tabPage)
        {
            string name = tabPage.Text;
            if (!_gridAndData.ContainsKey(name))
                return;
            if (!_gridAndData[name].GridAndDataWrapper.GridInvoked)
            {
                tabPage.Controls[0].Visible = false;
                var grid = LoadGridControl(name, tabPage);
               
                grid.SelectionChanged += Grid_SelectionChanged;
                _gridAndData[name].GridAndDataWrapper.GridHelper = CreateGrid(grid, name); 
                _gridAndData[name].GridAndDataWrapper.GridInvoked = true;
                _gridAndData[name].GridAndDataWrapper.GridHelper.Grid.FormatRow += GridOnFormatRow;
                _gridAndData[name].GridAndDataWrapper.DateColumnGetter = GetDateColumnGetter;
                ResizeAllToCurrentSize();
            }
            _currentGrid = _gridAndData[name].GridAndDataWrapper;
            RecalcFilterBox(_currentGrid.Filter);
            ApplyFilter();
        }

        private void GridOnFormatRow(object sender, FormatRowEventArgs e)
        {
            if(_currentGrid.ColorFilterer == null)
                _currentGrid.ColorFilterer = new ModelFilterService<EventLogEntryShort>(_currentGrid.ColorFilter, _currentGrid.Headers, _currentGrid.DateColumnGetter);

            _currentGrid.ColorFilterer.ColorFilter(e.Model, e.Item);
        }

        private async Task<bool> LoadStuff()
        {
            var helper = new ModelFilterHelper();
            var headers = new Dictionary<string, ColumnHeaderWrapper<EventLogEntryShort>>
            {
                {"Level", helper.CreateColumnHeaderWrapper<EventLogEntryShort, string>("Level", "Level")},
                {"Source", helper.CreateColumnHeaderWrapper<EventLogEntryShort, string>("Source", "Source")},
                {"Event Id", helper.CreateColumnHeaderWrapper<EventLogEntryShort, long?>("Event Id", "InstanceId")},
                {"Message", helper.CreateColumnHeaderWrapper<EventLogEntryShort, string>("Message", "Message").IgnoreInGrid()},
                {"User", helper.CreateColumnHeaderWrapper<EventLogEntryShort, string>("User", "User").IgnoreInGrid()},
                {"Date and time",helper.CreateColumnHeaderWrapper<EventLogEntryShort, DateTime?>("Date and time", "Logged")}
            };


            List<EventLogEntryShort> res = new List<EventLogEntryShort>();
            _gridAndData = new Dictionary<string, DataWrapper>();
            
           
           
            if (Switchboard.Instance.LogCollectorOutput?.GroupedServerInfo != null)
            {
                //var splash = SplashManager.Loader.ShowFloatingSplash(this,"Opening Windows logs..");
                ShowSplash("Opening Windows logs..");
                try
                {
                    _validationErrors = new List<string>();
                    res = await Switchboard.Instance.LogCollectorOutput.WindowsEventLogItems(JsonErrors);
                    if (_validationErrors.Any())
                    {
                        string err = _validationErrors.Take(5).Aggregate((curr, next) => curr + "\r\n" + next);
                        Mbox.Show($"We had problems parsing this Windows event log. We found {_validationErrors.Count} errors\r\n\r\n{err}","Problems in parsing Windows event log");
                    }
                }
                catch (Exception e)
                {
                    Mbox.Show($"The Windows log is invalid.\r\n\r\n{e}", "Sorry we can't read this");
                }
                finally
                {
                    HideSplash();
                }
  
            }
            
            if (res == null) return false;
            foreach (var item in res)
            {
                if (item.LogName == null) continue;
                if (!_gridAndData.ContainsKey(item.LogName))
                {
                    _gridAndData.Add(item.LogName, new DataWrapper());
                    _gridAndData[item.LogName].GridAndDataWrapper.Data = new List<EventLogEntryShort>();
                    _gridAndData[item.LogName].GridAndDataWrapper.Headers = headers;
                }
                _gridAndData[item.LogName].GridAndDataWrapper.Data.Add(item);
            }
            InvokeGrid(CtrlGridTabs.SelectedTab);
            return true;
        }

        List<string> _validationErrors = new List<string>();
        private void JsonErrors(string errMsg)
        {
            _validationErrors.Add(errMsg);
        }

        private GridHelper<EventLogEntryShort> CreateGrid(FastDataListView grid, string name)
        {

            var gridHelper = new GridHelper<EventLogEntryShort>(grid, () => LoadStaticData(name), DisposalHelper);
            gridHelper.UseAlternatingBackColor = true;
            gridHelper.Columns.CreateColumn("Level", x => x?.Level);
            gridHelper.Columns.CreateColumn("Date and time", x => x?.Logged, x => x?.ToString("yyyy-MM-dd HH:mm:ss"));
            gridHelper.Columns.CreateColumn("Source", x => x?.Source);
            gridHelper.Columns.CreateColumn("Event Id", x => x?.InstanceId.ToString());

            gridHelper.ReloadOnce(true);
            return gridHelper;


        }

        private FastDataListView LoadGridControl(string key,TabPage tabPage)
        {
            tabPage.SuspendLayout();
            var gridCtrl = new GridWrapperControl<EventLogEntryShort>(RecalcFilterBox, _gridAndData[key].GridAndDataWrapper);
            gridCtrl.RightClikFilterGenerator = GenerateRightClickMenu;
            gridCtrl.RightClikFilterDateColumnGetter = RightClikFilterDateColumnGetter;
            gridCtrl.Dock = DockStyle.Fill;
            tabPage.Controls.Add(gridCtrl);
            //_gridAndData[key].GridWrapperControl = gridCtrl;
            tabPage.ResumeLayout();
            return gridCtrl.Grid;
        }


        private DateTime RightClikFilterDateColumnGetter(EventLogEntryShort model, object o, OLVColumn olvColumn)
        {
            return model.Logged;
        }

        private List<Tuple<string, List<QuickFilterValues>>> GenerateRightClickMenu(EventLogEntryShort model, object value, OLVColumn eColumn)
        {
            var ret = new List<Tuple<string, List<QuickFilterValues>>>();

            var quickPositive = new QuickFilterValues { NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} = '{value}'" };
            var quickNegative = new QuickFilterValues { NegativeFilter = true, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} <> '{value}'" };
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By {eColumn.Text} = '{value}'",new List<QuickFilterValues> {quickPositive}));

            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By {eColumn.Text} <> '{value}'",new List<QuickFilterValues> { quickNegative}));

            var quickPositive2 = new QuickFilterValues { IsGroup = true, NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} = '{value}'" };
            var quickNegative2 = new QuickFilterValues { IsGroup = true, NegativeFilter = true, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} <> '{value}'" };
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By {eColumn.Text} = '{value}' & Source = '{model.Source}'", new List<QuickFilterValues> { quickPositive2, new QuickFilterValues { NegativeFilter = false, ColumnName = "Source", FilterValue = model.Source, FriendlyName = $"Quick Filter By Source = '{model.Source}'" }}));
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By NOT ({eColumn.Text} = '{value}' & Source = '{model.Source}')", new List<QuickFilterValues> { quickNegative2, new QuickFilterValues { NegativeFilter = true, ColumnName = "Source", FilterValue = model.Source, FriendlyName = $"Quick Filter By Source <> '{model.Source}'" }}));

            return ret;
        }

        private async void FrmWindowsLogViewer_Load(object sender, EventArgs e)
        {
            await LoadStuff();
        }

       

        private void ApplyFilter()
        {
            if (_currentGrid == null) return;
            _currentGrid.Filter.DateFrom = dteFrom.Value.Date + dteFromTime.Value.TimeOfDay;
            _currentGrid.Filter.DateTo = dteTo.Value.Date + dteToTime.Value.TimeOfDay;
            _currentGrid.Filter.UseDate = chkUseDate.Checked;
            _currentGrid.Filter.FilterLevel = new List<EventLogFilterLevel>();
            var fiterer = new ModelFilterService<EventLogEntryShort>(_currentGrid.Filter, _currentGrid.Headers, _currentGrid.DateColumnGetter);
            _currentGrid.GridHelper.Grid.ModelFilter = fiterer;
        }

        private DateTime GetDateColumnGetter(EventLogEntryShort model)
        {
            return model.Logged;
        }

        private Task<IEnumerable<EventLogEntryShort>> LoadStaticData(string name)
        {
            return Task.FromResult(_gridAndData[name].GridAndDataWrapper.Data.AsEnumerable());
        }
        
        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            var send = sender as ObjectListView;
            
            txtInfo.Text = (send?.SelectedItem?.RowObject as EventLogEntryShort)?.Message;
        }
      
        private void cmdFilterApply_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void CtrlGridTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtInfo.Text = "";
            if (CtrlGridTabs.SelectedTab == null) return;
            InvokeGrid(CtrlGridTabs.SelectedTab);
        }

        private void dteFrom_MouseDown(object sender, MouseEventArgs e)
        {
            chkUseDate.Checked = true;
        }


        private void RecalcFilterBox(ModelFilterValues filter)
        {
            txtFilter.Text = "";
            
            var qfDisplay = new ModelFilterHelper().CreateDisplayOfQuickFilters(filter);
            qfDisplay.ForEach(p =>
            {
                txtFilter.Text += p +Environment.NewLine;
            });


            dteFrom.Value = filter.DateFrom.Date;
            dteFromTime.Value = filter.DateFrom;
            dteTo.Value = filter.DateTo.Date;
            dteToTime.Value = filter.DateTo;
            chkUseDate.Checked = filter.UseDate;
            _currentGrid.Filter = filter;
        }

        public override void ShowFilterIfSupported()
        {
            ShowFilter();
        }

        private void ShowFilter()
        {
            if (_currentGrid == null) return;
            var frm = new FrmFilter(_currentGrid.Filter, _currentGrid.Headers.Select(p => p.Value.HeaderName).ToList())
            {
                StartPosition = FormStartPosition.CenterParent
            };
            frm.ShowDialog(this);
            RecalcFilterBox(_currentGrid.Filter);
            ApplyFilter();
        }

        private void txtFilter_Click(object sender, EventArgs e)
        {
            ShowFilter();
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            ShowFilter();
        }
    }
}

