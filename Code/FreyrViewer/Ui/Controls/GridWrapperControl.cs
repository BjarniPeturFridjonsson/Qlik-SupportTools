using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FreyrViewer.Common;
using FreyrViewer.Ui.Grids;
using FreyrViewer.Ui.Grids.ModelFilter;

namespace FreyrViewer.Ui.Controls
{
    public partial class GridWrapperControl<T> : UserControl
    {
        private enum FilterRightClickActionType
        {
            RemAllFilters, RemAllQuickFilters, RemColorFilter,RemDateFilter,ExternalAction,CopyRowToClipboard,CopyCellToClipboard
        }

        private class DateFilter
        {
            public int Minutes { get; set; }
            public DateTime OrgDate { get; set; }
        }

        public FastDataListView Grid => CtrlGrid;
        public Func<T, object, OLVColumn, List<Tuple<string, List<QuickFilterValues>>>> RightClikFilterGenerator { get; set; }
        public Func<T, object, OLVColumn, DateTime> RightClikFilterDateColumnGetter { get; set; }
        public Action ClearFiltersInAllGrids { get; set; }

        private readonly GridAndDataWrapper<T> _gridAndData;
        private readonly Action<ModelFilterValues> _recalcFilter;

        internal GridWrapperControl(Action<ModelFilterValues> filterRecalcAction, GridAndDataWrapper<T> gridAndData)
        {
            InitializeComponent();
            _recalcFilter = filterRecalcAction;
            _gridAndData = gridAndData ?? throw new Exception("Grid and data can't be null");
        }
        
        
        public void ApplyFilter(ModelFilterValues filter)
        {
            _gridAndData.Filter = filter;
            CtrlGrid.ModelFilter = new ModelFilterService<T>(_gridAndData.Filter, _gridAndData.Headers, _gridAndData.DateColumnGetter);
        }
        
        private void CtrlGrid_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            e.MenuStrip = DecideRightClickMenu((T)e.Model, e.SubItem?.ModelValue, e.Column);
        }
        
        private ContextMenuStrip DecideRightClickMenu(T model, object value, OLVColumn eColumn)
        {
            ctrlContext.Items.Clear();
            if (model != null)
            {
                if (eColumn.DataType == typeof(DateTime))
                {
                    if (RightClikFilterDateColumnGetter != null)
                    {
                        var date = RightClikFilterDateColumnGetter.Invoke(model, value, eColumn);
                        var groupItem = new ToolStripMenuItem("Filter by date");
                        groupItem.DropDownItemClicked += ctrlContext_ItemClicked;
                        groupItem.DropDownItems.Add("+- 5 Minutes").Tag = new DateFilter { Minutes = 5, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 15 Minutes").Tag = new DateFilter { Minutes = 15, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 30 Minutes").Tag = new DateFilter { Minutes = 30, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 1 hour").Tag = new DateFilter { Minutes = 60, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 6 hour").Tag = new DateFilter { Minutes = 6 * 60, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 12 hour").Tag = new DateFilter { Minutes = 12 * 60, OrgDate = date };
                        groupItem.DropDownItems.Add("+- 24 hour").Tag = new DateFilter { Minutes = 24 * 60, OrgDate = date };
                        ctrlContext.Items.Add(groupItem);
                    }
                }
                else
                {
                    //var quickPositive = new QuickFilterValues {NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} = '{value}'" };
                    //var quickNegative = new QuickFilterValues { NegativeFilter = true, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} <> '{value}'" };
                    //ctrlContext.Items.Add($"Quick Filter By {eColumn.Text} = '{value}'").Tag = new List<QuickFilterValues> { quickPositive };
                    //ctrlContext.Items.Add($"Quick Filter By {eColumn.Text} <> '{value}'").Tag = new List<QuickFilterValues> { quickNegative };

                    //var quickPositive2 = new QuickFilterValues {IsGroup = true, NegativeFilter = false, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} = '{value}'" };
                    //var quickNegative2 = new QuickFilterValues { IsGroup = true, NegativeFilter = true, ColumnName = eColumn.Text, FilterValue = value.ToString(), FriendlyName = $"Quick Filter By {eColumn.Text} <> '{value}'" };
                    //ctrlContext.Items.Add($"Quick Filter By {eColumn.Text} = '{value}' & Source = '{model.Source}'").Tag = new List<QuickFilterValues> { quickPositive2, new QuickFilterValues { NegativeFilter = false, ColumnName = "Source", FilterValue = model.Source, FriendlyName = $"Quick Filter By Source = '{model.Source}'" } };
                    //ctrlContext.Items.Add($"Quick Filter By NOT ({eColumn.Text} = '{value}' & Source = '{model.Source}')").Tag = new List<QuickFilterValues> { quickNegative2, new QuickFilterValues { NegativeFilter = true, ColumnName = "Source", FilterValue = model.Source, FriendlyName = $"Quick Filter By Source <> '{model.Source}'" } };
                    if (RightClikFilterGenerator != null)
                    {
                        var extra = RightClikFilterGenerator?.Invoke(model, value, eColumn);
                        extra?.ForEach(p =>
                        {
                            ctrlContext.Items.Add(new ToolStripMenuItem(p.Item1) { Tag = p.Item2 });
                        });
                    }

                    ctrlContext.Items.Add("-");
                    var colorItem = new ToolStripMenuItem($"Colorize By {eColumn.Text} = '{value}'");
                    colorItem.DropDownItemClicked += ctrlContext_ColorItemClicked;
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Green") { BackColor = Color.FromArgb(175, 215, 117), Tag = CreateColorTag(Color.FromArgb(175, 215, 117), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Dark Green") { BackColor = Color.FromArgb(82,160,23), Tag = CreateColorTag(Color.FromArgb(82,160,23), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Blue") { BackColor = Color.FromArgb(104,198,236), Tag = CreateColorTag(Color.FromArgb(104, 198, 236), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Light Blue") { BackColor = Color.FromArgb(127, 206, 207), Tag = CreateColorTag(Color.FromArgb(127, 206, 207), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Violet") { BackColor = Color.FromArgb(202, 181, 213), Tag = CreateColorTag(Color.FromArgb(202, 181, 213), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Pink") { BackColor = Color.FromArgb(247,168,202), Tag = CreateColorTag(Color.FromArgb(247, 168, 202), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Salmon") { BackColor = Color.FromArgb(248, 168, 151), Tag = CreateColorTag(Color.FromArgb(248, 168, 151), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Yellow") { BackColor = Color.FromArgb(244, 209, 98), Tag = CreateColorTag(Color.FromArgb(244, 209, 98), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Orange") { BackColor = Color.FromArgb(244, 183, 109), Tag = CreateColorTag(Color.FromArgb(244, 183, 109), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Red") { BackColor = Color.FromArgb(239,108,90), Tag = CreateColorTag(Color.FromArgb(239, 108, 90), eColumn.Text, value) });
                    colorItem.DropDownItems.Add(new ToolStripMenuItem($"Color Gray") { BackColor = Color.FromArgb(192, 192, 192), Tag = CreateColorTag(Color.FromArgb(192, 192, 192), eColumn.Text, value) });

                    ctrlContext.Items.Add(colorItem);
                    ctrlContext.Items.Add("Remove all color highlights").Tag = FilterRightClickActionType.RemColorFilter;
                }

            }
            ctrlContext.Items.Add("-");
            ctrlContext.Items.Add("Copy row to Clipboard").Tag = FilterRightClickActionType.CopyRowToClipboard;
            //ctrlContext.Items.Add("Copy cell to Clipboard").Tag = FilterRightClickActionType.CopyRowToClipboard;
            ctrlContext.Items.Add("-");
            ctrlContext.Items.Add("Remove filters and hightlights").Tag = FilterRightClickActionType.RemAllFilters;
            ctrlContext.Items.Add("Remove filters").Tag = FilterRightClickActionType.RemAllQuickFilters;
            ctrlContext.Items.Add("Remove date filter").Tag = FilterRightClickActionType.RemDateFilter;
            if(ClearFiltersInAllGrids!=null)
                ctrlContext.Items.Add("Remove filters in all open pages").Tag = FilterRightClickActionType.ExternalAction;
            return ctrlContext;
        }

        private void ctrlContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is FilterRightClickActionType action)
            {
                switch (action)
                {
                    case FilterRightClickActionType.RemAllFilters:
                        _gridAndData.Filter.QuickFilters = new List<QuickFilterValues>();
                        _gridAndData.ColorFilter.QuickFilters = new List<QuickFilterValues>();
                        _gridAndData.Filter.UseDate = false;
                        break;
                    case FilterRightClickActionType.RemAllQuickFilters:
                        _gridAndData.Filter.QuickFilters = new List<QuickFilterValues>();
                        break;
                    case FilterRightClickActionType.RemColorFilter:
                        _gridAndData.ColorFilter.QuickFilters = new List<QuickFilterValues>();
                        break;
                    case FilterRightClickActionType.RemDateFilter:
                        _gridAndData.Filter.UseDate = false;
                        break;
                    case FilterRightClickActionType.ExternalAction:
                        ClearFiltersInAllGrids?.Invoke();
                        break;
                    case FilterRightClickActionType.CopyCellToClipboard:
                        break;
                    case FilterRightClickActionType.CopyRowToClipboard:
                        var str = "";
                        var header = "";
                        foreach (object model in Grid.SelectedObjects)
                        {
                            for(var i = 0; i < Grid.Columns.Count;i++)
                            {
                                header += Grid.GetColumn(i).Name + "\t";
                                str += Grid.GetColumn(i).GetStringValue(model) + "\t";
                            }
                        }
                            
                        Clipboard.Clear();
                        Clipboard.SetText(header + Environment.NewLine + str);
                        Switchboard.Instance.SetInfoMessage("Copied to clipboard");
                        break;

                }
                RecalcFilterBox(_gridAndData.Filter);
            }
            if (e.ClickedItem.Tag is DateFilter dateFilter)
            {
                _gridAndData.Filter.DateFrom = dateFilter.OrgDate - TimeSpan.FromMinutes(dateFilter.Minutes);
                _gridAndData.Filter.DateTo = dateFilter.OrgDate + TimeSpan.FromMinutes(dateFilter.Minutes);
                _gridAndData.Filter.UseDate = true;
                RecalcFilterBox(_gridAndData.Filter);
            }
            if (e.ClickedItem.Tag is List<QuickFilterValues> val)
            {
                _gridAndData.Filter.QuickFilters.AddRange(val);
                RecalcFilterBox(_gridAndData.Filter);
            }

            ApplyFilter(_gridAndData.Filter);
        }

        private void RecalcFilterBox(ModelFilterValues currentGridFilter)
        {
            _recalcFilter.Invoke(currentGridFilter);
        }

        private QuickFilterValues CreateColorTag(Color color, string text, object value)
        {
            return new QuickFilterValues
            {
                ColorFilter = true,
                ColumnName = text,
                RowColor = color,
                FilterValue = value.ToString(),
                FriendlyName = $"Quick Filter By {text} = '{value}'"
            };
        }
        private void ctrlContext_ColorItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag is QuickFilterValues val)
            {
                _gridAndData.ColorFilter.QuickFilters.Add(val);

            }
        }

        private void CtrlGrid_FormatRow(object sender, FormatRowEventArgs e)
        {            
            if (_gridAndData.ColorFilterer == null)
                _gridAndData.ColorFilterer = new ModelFilterService<T>(_gridAndData.ColorFilter, _gridAndData.Headers, _gridAndData.DateColumnGetter);

            _gridAndData.ColorFilterer.ColorFilter(e.Model, e.Item);
        }  
    }
}
