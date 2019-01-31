using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Eir.Common.Extensions;
using FreyrCommon.Models;
using FreyrViewer.Common;
using FreyrViewer.Common.Winforms;
using FreyrViewer.Models;
using FreyrViewer.Services;
using FreyrViewer.Ui.Controls;
using FreyrViewer.Ui.Controls.TextPreview;
using FreyrViewer.Ui.Grids;
using FreyrViewer.Ui.Grids.ModelFilter;
using FreyrViewer.Ui.Helpers;
using FreyrViewer.Ui.Splashes;
using Color = System.Windows.Media.Color;

namespace FreyrViewer.Ui.MdiForms
{
    //when it comes to making better tabs... https://dotnetrix.co.uk/tabcontrol.htm#tip16
    //todo: when closing tab - remove the data from the DataWrapper
    public partial class FrmSenseLogs : FrmBaseForm
    {
        private class DataWrapper
        {
            public GenericDataWrapperService GenericDataWrapperService { get; } = new GenericDataWrapperService();
            public GridAndDataWrapper<GenericDataWrapper> GridAndDataWrapper { get; } = new GridAndDataWrapper<GenericDataWrapper>();
            public GridWrapperControl<GenericDataWrapper> GridWrapperControl { get; set; }
            public string LogFilePath { get; set; }
            public string Name { get; set; }
            public bool IsDirectory { get; set; }
            public DateTime LastModified { get; set; }
        }

        private readonly Dictionary<string, DataWrapper> _gridAndData = new Dictionary<string, DataWrapper>();
        private readonly List<AnimatedCloseTab> _closeTabHelper = new List<AnimatedCloseTab>();
        private readonly List<DragReorderTabControl> _dragTabsHelper = new List<DragReorderTabControl>();
        private TabControl _currentTab;
        private readonly Dictionary<string, FastDataListView> _paralellScroller = new Dictionary<string, FastDataListView>();
        private FastDataListView _currentMasterScroller;

        public FrmSenseLogs(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
            HideLogPanel2();
            ctrlTab2.SelectedIndexChanged += CtrlTabXOnSelectedIndexChanged;
            ctrlTab1.SelectedIndexChanged += CtrlTabXOnSelectedIndexChanged;
            _currentTab = ctrlTab1;
            _closeTabHelper.Add(new AnimatedCloseTab(ctrlTab1, OnTabClose) {OnHeaderRightClick = OnHeaderRightClick, OnTabMouseOver = OnTabMouseOver});
            _closeTabHelper.Add(new AnimatedCloseTab(ctrlTab2, OnTabClose) {OnHeaderRightClick = OnHeaderRightClick, OnTabMouseOver = OnTabMouseOver });
            _dragTabsHelper.Add(new DragReorderTabControl(ctrlTab1));
            _dragTabsHelper.Add(new DragReorderTabControl(ctrlTab2));
            ResizeAllToCurrentSize();
            WireDragNdrop();
            ctrlPreview.OnMouseClickRowAction += OnMouseClickRowAction;
            
        }
       
        private void OnMouseClickRowAction(int rowNr)
        {
            var grid = GetCurrentGrid();
            if(grid?.GridWrapperControl?.Grid != null)
                grid.GridWrapperControl.Grid.TopItemIndex = rowNr;
        }

        private void OnTabMouseOver(TabPage page)
        {
            var key = (string) page.Tag;
            var service = _gridAndData[key].GenericDataWrapperService;
            ctrlPreview.ShowDisplay(key, new TextFilePreviewOptions
            {
                TextPreviewer = ctrlLinePreview,
                DateaWrapperService = service,
                NrOfTextRowsForTextPreview = 20,
                TextColor = Color.FromRgb(62, 62, 65),
                WhiteSpaceColor = Color.FromRgb(174, 174, 174),
                CurrentActiveTab =  _currentTab,
            });
        }

        private void CtrlTabXOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            ctrlPreview.HideDisplay();
            if (!(sender is TabControl tab)) return;
            if (tab.SelectedTab == null)
            {
                _paralellScroller.Remove(tab.Name);
                return;
            }
                
            _paralellScroller[tab.Name] = _gridAndData[(string)tab.SelectedTab.Tag].GridWrapperControl.Grid;
        }

        private void HideLogPanel2()
        {
           
            ctrlSplitLogs.Panel2Collapsed = true;
            ctrlSplitLogs.Panel2.Hide();
        }

        private void OnHeaderRightClick(TabPage tabPage)
        {
            mnuContext.Show(tabPage, tabPage.PointToClient(Cursor.Position));
        }

        public override void ManualActivateEvent(object param)
        {
            if (param == null )
                return;

            if (param is SenseLogInfo logInfo  && !logInfo.IsDirectory)
            {
                if (!_gridAndData.ContainsKey(logInfo.LogFilePath))
                    AddNewLog(logInfo, logInfo.LogFilePath);

                ActivateTab(logInfo.LogFilePath);
            }

            if (param is List<SenseLogInfo> logInfos && logInfos.Count > 0)
            {
                logInfos.ForEach(p =>
                {
                    if (!p.IsDirectory && !_gridAndData.ContainsKey(p.LogFilePath))
                        AddNewLog(p, p.LogFilePath);
                });
                ActivateTab(logInfos.First().LogFilePath);
            }
        }

        private void ActivateTab(string tabKey)
        {
            if (tabKey == null) return;
            TabControl tab = ctrlTab1.TabPages.ContainsKey(tabKey) ? ctrlTab1 : ctrlTab2;
            
            if (!((string)tab.SelectedTab.Tag).Equals(tabKey))
                tab.SelectedTab = tab.TabPages[tabKey];
            _paralellScroller[tab.Name] = _gridAndData[(string)tab.SelectedTab.Tag].GridWrapperControl.Grid;

        }

        private void AddNewLog(SenseLogInfo logInfo, string key)
        {
            var splash = SplashManager.Loader.ShowFloatingSplash(this, "Opening log");
            try
            {
                if (_gridAndData.ContainsKey(key))
                {
                    return;
                }
                _gridAndData.Add(key, new DataWrapper
                {
                    Name = logInfo.Name,
                    IsDirectory = logInfo.IsDirectory,
                    LogFilePath = logInfo.LogFilePath,
                    LastModified = logInfo.LastModified
                });

                ctrlTab1.SuspendLayout();
                ctrlTab1.TabPages.Add(key, logInfo.Name);


                var tab = ctrlTab1.TabPages[key];
                tab.Tag = key;
                tab.ImageIndex = 0;


                var gridCtrl = new GridWrapperControl<GenericDataWrapper>(RecalcFilterBox, _gridAndData[key].GridAndDataWrapper);
                gridCtrl.Grid.Scroll += GridCtrlOnScroll;
                gridCtrl.Grid.MouseMove += GridOnMouseMove;
                gridCtrl.Grid.MouseDown += (sender, args) => { ctrlPreview.HideDisplay(); };
                gridCtrl.RightClikFilterGenerator = GenerateRightClickMenu;
                gridCtrl.RightClikFilterDateColumnGetter = RightClikFilterDateColumnGetter;

                gridCtrl.ClearFiltersInAllGrids = ClearFiltersInAllGrids;
                gridCtrl.Dock = DockStyle.Fill;
                tab.Controls.Add(gridCtrl);

                _gridAndData[key].GenericDataWrapperService.LogFileAnalyzer = new LogFileAnalyzerService();

                var fileCutoff = _gridAndData[key].GenericDataWrapperService.LoadToJson(logInfo.LogFilePath);
                _gridAndData[key].GridAndDataWrapper.GridHelper = CreateGrid(gridCtrl.Grid, key, _gridAndData[key]);
                _gridAndData[key].GridAndDataWrapper.GridInvoked = true;
                _gridAndData[key].GridAndDataWrapper.GridHelper.Grid.FormatRow += GridOnFormatRow;
                _gridAndData[key].GridAndDataWrapper.DateColumnGetter = GetDateColumnGetter;
                _gridAndData[key].GridWrapperControl = gridCtrl;


                ResizeAllToCurrentSize(gridCtrl.Controls);
                _closeTabHelper[0].SetTabHeader(tab, _gridAndData[key].GenericDataWrapperService.LogFileAnalyzer.SimplifiedFailureLevel);

                ctrlTab1.SelectedTab = tab;
                ctrlTab1.ResumeLayout();
                if (ctrlTab1.TabPages.Count > 1 || ctrlTab1.TabPages.Count > 1) picDown.Visible = true;
                splash.Dispose();
                if (fileCutoff > 0)
                {
                    Mbox.Show("This file is too large and has been capped at 1 million lines.", "I'm sorry about this");
                }
            }
            catch (Exception e)
            {
                splash.Dispose();
                Mbox.Show($"Failed opening log \r\n{e}", "Failure");

            }
            
        }

        
        private void GridOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!(sender is FastDataListView sentBy)) return;
            _currentMasterScroller = sentBy;
        }

        
        private void GridCtrlOnScroll(object sender, ScrollEventArgs scrollEventArgs)
        {
            if (scrollEventArgs.ScrollOrientation != ScrollOrientation.VerticalScroll || !Switchboard.Instance.Settings.Documents.ParallelScrollingEnabled) return;
           
            if (!(sender is FastDataListView sentBy)) return;
            if (_currentMasterScroller == null) _currentMasterScroller = sentBy;
            _paralellScroller.Values.ToList().ForEach(p =>
            {
                if (p == _currentMasterScroller) return;
                if (sentBy.TopItemIndex != p.TopItemIndex)
                    p.TopItemIndex = sentBy.TopItemIndex;
            });
        }

        private GridHelper<GenericDataWrapper> CreateGrid(FastDataListView grid, string name, DataWrapper wrapper)
        {
            var headers = new Dictionary<string, ColumnHeaderWrapper<GenericDataWrapper>>();
            wrapper.GridAndDataWrapper.Headers = headers;
            var gridHelper = new GridHelper<GenericDataWrapper>(grid, () => LoadStaticData(name), DisposalHelper);
            gridHelper.AutoResizeGrid = false;
            gridHelper.UseAlternatingBackColor = true;
            wrapper.GenericDataWrapperService.Headers.ForEach(p =>
            {
                if (headers.ContainsKey(p.HeaderName)) // log files might have the headers called the same thing
                {
                    for (var i = 1; i < 99; i++)
                    {
                        var header = $"{p.HeaderName} ({i})";
                        if (!headers.ContainsKey(header))
                        {
                            p.HeaderName = header;
                            break;
                        }
                    }
                }

                if (p.AspectGetter != null)
                {
                    if (!p.ShowInGrid) return;
                    var col = gridHelper.Columns.CreateColumn(p.HeaderName, p.AspectGetter); //x => x.String1
                    var headerWidth = (int)grid.Font.GetTextSize(p.HeaderName).Width + 5;
                    var textWidth = (int)grid.Font.GetTextSize(new string('e', p.MaxCharCount)).Width;
                    textWidth = textWidth > 400 ? 400 : textWidth;
                    col.Width = headerWidth > textWidth ? headerWidth : textWidth;
                }
                else
                {
                    var col = gridHelper.Columns.CreateColumn(p.HeaderName, x=> x.DateTime1, x => x.ToString("yyyy-MM-dd HH:mm:ss")); //x => x.String1
                    col.Width = (int)grid.Font.GetTextSize("9999-19-99 99:99:99").Width;
                }
                headers.Add(p.HeaderName, p);
                
            });

            gridHelper.ReloadOnce(true);
            return gridHelper;
        }

        private DataWrapper GetCurrentGrid()
        {
            if (_gridAndData == null || _gridAndData.Count  == 0 )
                return null;

            if (_currentTab.SelectedTab?.Tag?.ToString() == null)
                return _gridAndData?.FirstOrDefault().Value;

            _gridAndData.TryGetValue(_currentTab.SelectedTab?.Tag?.ToString(), out DataWrapper selected);
            return selected;
        }

        private void GridOnFormatRow(object sender, FormatRowEventArgs e)
        {
            var selected = GetCurrentGrid();
            if (selected == null || !selected.GridAndDataWrapper.GridInvoked)
                return;

            if (selected.GridAndDataWrapper.ColorFilterer == null)
                selected.GridAndDataWrapper.ColorFilterer = new ModelFilterService<GenericDataWrapper>(selected.GridAndDataWrapper.ColorFilter, selected.GridAndDataWrapper.Headers, selected.GridAndDataWrapper.DateColumnGetter);

            selected.GridAndDataWrapper.ColorFilterer.ColorFilter(e.Model, e.Item);
        }

        private DateTime GetDateColumnGetter(GenericDataWrapper model)
        {
            return model.DateTime1;
        }

        private void ChangeLog()
        {
            ctrlOpenFiles.Visible = false;
            _currentMasterScroller = null;
            var selected = GetCurrentGrid();
            if (selected == null || selected.GridAndDataWrapper.GridInvoked == false)
                return;

            RecalcFilterBox(selected.GridAndDataWrapper.Filter);
            ApplyFilter(GetCurrentGrid());
        }

        private Task<IEnumerable<GenericDataWrapper>> LoadStaticData(string name)
        {
            return Task.FromResult(_gridAndData[name].GenericDataWrapperService.Lines.AsEnumerable());
        }

        private DateTime RightClikFilterDateColumnGetter(GenericDataWrapper model, object o, OLVColumn olvColumn)
        {
            return model.DateTime1;
        }

        private List<Tuple<string, List<QuickFilterValues>>> GenerateRightClickMenu(GenericDataWrapper model, object value, OLVColumn eColumn)
        {
            var ret = new List<Tuple<string, List<QuickFilterValues>>>();

            var quickPositive = new QuickFilterValues { NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} = '{value}'" };
            var quickNegative = new QuickFilterValues { NegativeFilter = true, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} <> '{value}'" };
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By {eColumn.Text} = '{value}'", new List<QuickFilterValues> { quickPositive }));
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Quick Filter By {eColumn.Text} <> '{value}'", new List<QuickFilterValues> { quickNegative }));
            ret.Add(new Tuple<string, List<QuickFilterValues>>($"Create new filter containing {eColumn.Text} = '{value}'", new List<QuickFilterValues> { new QuickFilterValues { NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(),ToBeModifiedInFilterEditor = true}}));

            return ret;
        }

        private void ShowFilter()
        {
            if (_currentTab.SelectedTab == null) return;
            var wrapper = _gridAndData[(string)_currentTab.SelectedTab.Tag];
            var frm = new FrmFilter(wrapper.GridAndDataWrapper.Filter, wrapper.GridAndDataWrapper.Headers.Select(p=> p.Value.HeaderName).ToList())
            {
                StartPosition = FormStartPosition.CenterParent
            };
            frm.ShowDialog(this);
            RecalcFilterBox(wrapper.GridAndDataWrapper.Filter);
            wrapper.GridWrapperControl.ApplyFilter(wrapper.GridAndDataWrapper.Filter);
        }

        private void RecalcFilterBox(ModelFilterValues filter)
        {
            txtFilter.Text = "";
            bool showFilterDlg = false;
            filter.QuickFilters.ForEach(p =>
            {
                if (p.ToBeModifiedInFilterEditor)
                {
                    showFilterDlg = true;
                    return;
                }
                txtFilter.Text += p.FriendlyName + Environment.NewLine;
            });

            if(showFilterDlg)
                ShowFilter();

            dteFrom.Value = filter.DateFrom.Date;
            dteFromTime.Value = filter.DateFrom;
            dteTo.Value = filter.DateTo.Date;
            dteToTime.Value = filter.DateTo;
            chkUseDate.Checked = filter.UseDate;
            
        }

        private void ApplyFilter(DataWrapper gridWrap)
        {
            if (gridWrap == null)
                return;

            gridWrap.GridAndDataWrapper.Filter.DateFrom = dteFrom.Value.Date + dteFromTime.Value.TimeOfDay;
            gridWrap.GridAndDataWrapper.Filter.DateTo = dteTo.Value.Date + dteToTime.Value.TimeOfDay;
            gridWrap.GridAndDataWrapper.Filter.UseDate = chkUseDate.Checked;
            gridWrap.GridAndDataWrapper.Filter.FilterLevel = new List<EventLogFilterLevel>();
            var fiterer = new ModelFilterService<GenericDataWrapper>(gridWrap.GridAndDataWrapper.Filter, gridWrap.GridAndDataWrapper.Headers, gridWrap.GridAndDataWrapper.DateColumnGetter);
            gridWrap.GridAndDataWrapper.GridHelper.Grid.ModelFilter = fiterer;
        }

        private void OnTabClose(TabPage tab)
        {
            if (tab.Tag is string key)
            {
                _gridAndData[key].GridAndDataWrapper.GridHelper?.Grid.Dispose();
                _gridAndData.Remove(key);
                
                _paralellScroller.Remove(tab.Parent.Name);
            }
            if (tab.Parent == ctrlTab2 && ctrlTab2.TabPages.Count < 2 ) //its not closed yet.
            {
                HideLogPanel2();
            }
        }

        public override void ShowFilterIfSupported()
        {
            ShowFilter();
        }

        private void ClearFiltersInAllGrids()
        {
            foreach (var item in _gridAndData)
            {
                item.Value.GridAndDataWrapper.Filter = new ModelFilterValues();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                _closeTabHelper?.ForEach(p => p.Dispose());
                _dragTabsHelper?.ForEach(p=> p.Dispose());
                
            }
            base.Dispose(disposing);
        }

        private void cmdApplyAll_Click(object sender, EventArgs e)
        {
            var filter = GetCurrentGrid().GridAndDataWrapper.Filter;
            foreach (var item in _gridAndData)
            {
                item.Value.GridAndDataWrapper.Filter = filter.DeepCopy();
            }
        }

        private void cmdFilterApply_Click(object sender, EventArgs e)
        {
            //apply filter to all visible tabs.
            if(_gridAndData.ContainsKey(ctrlTab1.SelectedTab?.Tag +""))
                ApplyFilter(_gridAndData[ctrlTab1.SelectedTab?.Tag+""]);
            if (ctrlTab2.TabCount > 0 &&  _gridAndData.ContainsKey(ctrlTab2.SelectedTab?.Tag + ""))
                ApplyFilter(_gridAndData[ctrlTab2.SelectedTab?.Tag + ""]);
        }

      

        private void picDown_Click(object sender, EventArgs e)
        {
            ctrlOpenFiles.Items.Clear();
            foreach (var grid in _gridAndData)
            {
                var item = new ListViewItem(new[] {
                    grid.Value.GenericDataWrapperService.LogFileAnalyzer.ErrorCount[LogFailureLevel.Error].ToString(),
                    grid.Value.GenericDataWrapperService.LogFileAnalyzer.ErrorCount[LogFailureLevel.Warning].ToString(),
                    grid.Value.Name// logInfo.Name
                });
                item.Tag = grid.Key;
                ctrlOpenFiles.Items.Add(item);
            }

            ctrlOpenFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            ctrlOpenFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ctrlOpenFiles.Visible = !ctrlOpenFiles.Visible;
            ctrlOpenFiles.Focus();
        }

        private void ctrlOpenFiles_DoubleClick(object sender, EventArgs e)
        {
            var nameValue = ctrlOpenFiles.SelectedItems[0].Tag as string;
            ctrlOpenFiles.Visible = false;
            ActivateTab(nameValue);
        }

        private void CloseTabLocally(TabControl ctrlTab,TabPage p)
        {
            OnTabClose(p);
            ctrlTab.TabPages.Remove(p);
            p.Dispose();
            if (ctrlTab1.TabPages.Count < 2 && ctrlTab1.TabPages.Count < 2) picDown.Visible = false;
        }

        private void CloseTabs(object sender, int typeOfClose)
        {
            if (!(((ContextMenuStrip) ((ToolStripMenuItem) sender).Owner).SourceControl is TabPage tabPage)) return;
            if (!(tabPage.Parent is TabControl ctrlTab) ) return;

            var tabIndex = ctrlTab.TabPages.IndexOf(tabPage);
            var tabsToClose = new List<TabPage>();
            foreach (TabPage item in ctrlTab.TabPages)
            {
                switch (typeOfClose)
                {
                    case 1://Clowns to the left of me
                        if (ctrlTab.TabPages.IndexOf(item) > tabIndex)
                            tabsToClose.Add(item);
                        break;
                    case 2: //jokers to the right
                        if (ctrlTab.TabPages.IndexOf(item) < tabIndex)
                            tabsToClose.Add(item);
                        break;
                    case 3: // stuck in the middle with you
                        if (ctrlTab.TabPages.IndexOf(item) != tabIndex)
                            tabsToClose.Add(item);
                        break;
                    case 4: // start out with nothing.
                        tabsToClose.Add(item);
                        break;
                    case 5: //close all empty
                            if(!_gridAndData[item.Tag as string +""].GenericDataWrapperService.Lines.Any())
                                tabsToClose.Add(item);
                        break;
                }                
            }
            tabsToClose.ForEach(p => CloseTabLocally(ctrlTab, p));

        }

        private void moveToSplitTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl is TabPage tabPage)) return;
            if (ctrlSplitLogs.Panel2Collapsed)
            {
                ctrlSplitLogs.Panel2Collapsed = false;
                ctrlSplitLogs.Panel2.Show();
            }
            ctrlTab2.TabPages.Add(tabPage);
            _paralellScroller[ctrlTab2.Name] = _gridAndData[(string)tabPage.Tag].GridWrapperControl.Grid;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e) {CloseTabs(sender, 3);}
        private void closeAllLogsToolStripMenuItem_Click(object sender, EventArgs e){CloseTabs(sender, 4);}
        private void closeAllOnRightToolStripMenuItem_Click(object sender, EventArgs e){CloseTabs(sender, 1);}
        private void closeAllOnLeftToolStripMenuItem_Click(object sender, EventArgs e){CloseTabs(sender, 2);}
        private void closeAllEmptyLogsToolStripMenuItem_Click(object sender, EventArgs e) {CloseTabs(sender, 5); }

        private void ctrlTab1_Enter(object sender, EventArgs e){_currentTab = ctrlTab1;}
        private void ctrlTab2_Enter(object sender, EventArgs e) { _currentTab = ctrlTab2;}
        private void ctrlTabLogFiles_SelectedIndexChanged(object sender, EventArgs e) { ChangeLog(); }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e) {ShowFilter();}
        private void txtFilter_Click(object sender, EventArgs e){ShowFilter();}

 
        private void ctrlOpenFiles_Leave(object sender, EventArgs e){ctrlOpenFiles.Visible = false;}
    }
}
