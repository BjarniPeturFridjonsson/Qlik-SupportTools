using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FreyrCommon.Models;
using FreyrViewer.Common;
using FreyrViewer.Common.Winforms;
using FreyrViewer.Ui.Grids;
using FreyrViewer.Ui.Grids.ModelFilter;

namespace FreyrViewer.Ui.MdiForms
{
    public partial class FrmSearchForFiles : Form
    {

        private string _searchString;
        private List<SenseLogInfo> _logs = new List<SenseLogInfo>();
        private DateTime _dteTo;
        private DateTime _dteFrom;
        private readonly DateTimePickerHelper _dateTimePickerHelper = new DateTimePickerHelper();
        private readonly GridAndDataWrapper<SenseLogInfo> _gridAndData = new GridAndDataWrapper<SenseLogInfo>();


        public List<SenseLogInfo> SelectedLogFiles { get; private set; }

        public FrmSearchForFiles()
        {
            InitializeComponent();
        }

        private List<SenseLogInfo> GetAllSelectedLogs()
        {
            var ret = new List<SenseLogInfo>();
            foreach (var item in _gridAndData.GridHelper.Grid.SelectedObjects)
            {

                var log = ((SenseLogInfo)item);
                ret.Add(log);
            }
            return ret;
        }

        private void DoLoad()
        {
            dteTo.Value = DateTime.Parse("1970-01-01");
            dteFrom.Value = DateTime.Now;
            _dteTo = dteTo.Value;
            _dteFrom = dteFrom.Value;

            if (Switchboard.Instance.LogCollectorOutput == null)
            {
                lblError.Text = @"There are no logs loaded";
                lblError.Visible = true;
                cmdApplyAndClose.Enabled = false;
                return;
            }
            Switchboard.Instance.LogCollectorOutput.GroupedServerInfo.ForEach(p =>
            {
                FlattenLogList(p.Logs);
            });
            _logs = _logs.OrderBy(p => p.Name).ToList();
            _dteTo = dteTo.Value;
            _dteFrom = dteFrom.Value;

            _gridAndData.GridHelper = CreateGrid();
            _gridAndData.GridInvoked = true;
            _gridAndData.DateColumnGetter = GetDateColumnGetter;
            _gridAndData.Headers = CreateHeaders();
        }

        private DateTime GetDateColumnGetter(SenseLogInfo model)
        {
            return model.LastModified;
        }

        private Dictionary<string, ColumnHeaderWrapper<SenseLogInfo>> CreateHeaders()
        {
            var helper = new ModelFilterHelper();
            var headers = new Dictionary<string, ColumnHeaderWrapper<SenseLogInfo>>
            {
                {"Name", helper.CreateColumnHeaderWrapper<SenseLogInfo, string>("Name", "Name")},
                {"LogFilePath", helper.CreateColumnHeaderWrapper<SenseLogInfo, string>("Path", "LogFilePath")},
                {"LastModified",helper.CreateColumnHeaderWrapper<SenseLogInfo, DateTime?>("Modified date", "LastModified")}
            };
            return headers;
        }

        private GridHelper<SenseLogInfo> CreateGrid()
        {
            var disposalHelper = new ComponentDisposalHelper(this);
            var gridHelper = new GridHelper<SenseLogInfo>(CtrlGrid, () => Task.FromResult(_logs as IEnumerable<SenseLogInfo>), disposalHelper);
            gridHelper.UseAlternatingBackColor = true;
            gridHelper.Columns.CreateColumn("Name", x => x?.Name);
            gridHelper.Columns.CreateColumn("Modified date", x => x?.LastModified, x => x?.ToString("yyyy-MM-dd HH:mm:ss"));
            gridHelper.Columns.CreateColumn("Path", x => x?.LogFilePath);
            gridHelper.ReloadOnce(true);
            return gridHelper;
        }


        private void FlattenLogList(List<SenseLogInfo> logs)
        {
            logs.ForEach(p =>
            {
                if (!p.IsDirectory)
                {
                    _logs.Add(p);
                    if (dteFrom.Value > p.LastModified)
                        dteFrom.Value = p.LastModified;
                    if (dteTo.Value < p.LastModified)
                        dteTo.Value = p.LastModified;
                }
                p.LogInfos.ForEach(p2 => FlattenLogList(p2.LogInfos));
               
            });
        }

        private void FilterList(bool dateChanged)
        {

            if (_gridAndData.GridHelper == null || txtFilter.Text.Equals(_searchString) && dateChanged == false) return;
            _searchString = txtFilter.Text;

            Regex regex = null;
            try
            {
                regex = new Regex(_searchString, RegexOptions.IgnoreCase);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"regexFailure {regex} =>{e}");
                return;
            }

            if (CtrlGrid == null) return;
            _gridAndData.Filter.DateFrom = dteFrom.Value.Date;
            _gridAndData.Filter.DateTo = dteTo.Value.Date.AddDays(1).AddMilliseconds(-1);
            _gridAndData.Filter.UseDate = true;
            _gridAndData.Filter.QuickFilters = new List<QuickFilterValues> { new QuickFilterValues { FilterValue = _searchString } };
            var fiterer = new ModelFilterService<SenseLogInfo>(_gridAndData.Filter, _gridAndData.Headers, _gridAndData.DateColumnGetter);
            _gridAndData.GridHelper.Grid.ModelFilter = fiterer;
        }

        private void cmdFilterCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdApplyAndClose_Click(object sender, EventArgs e)
        {
            SelectedLogFiles = GetAllSelectedLogs();
            if (SelectedLogFiles.Count == 0)
            {
                Mbox.Show("There are no selected files.\r\n\r\nUse Ctrl-A to select all files\r\n\r\nUse Ctrl/Shift And click on row for individual select.", "No selected files", MessageBoxButtons.OK);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }


        private void FrmSearchForFiles_Load(object sender, EventArgs e)
        {
            DoLoad();
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            tmrKeystrokes.Enabled = true;
        }

        private void tmrKeystrokes_Tick(object sender, EventArgs e)
        {
            FilterList(false);
        }

        private void dteFrom_ValueChanged(object sender, EventArgs e)
        {
            dteFrom.Font = new Font(dteFrom.Font.FontFamily, dteFrom.Font.Size);
            FilterList(true);
        }

        private void dteTo_ValueChanged(object sender, EventArgs e)
        {
            dteTo.Font = new Font(dteTo.Font.FontFamily, dteTo.Font.Size);
            FilterList(true);
        }

        private void cmdClearDate_Click(object sender, EventArgs e)
        {
            dteTo.Enabled = false;
            dteFrom.Enabled = false;
            dteFrom.Value = _dteFrom;
            dteTo.Value = _dteTo;
        }

        private void FrmSearchForFiles_MouseDown(object sender, MouseEventArgs e)
        {
            EnableControlOnMouseDown(e, dteTo);
            EnableControlOnMouseDown(e, dteFrom);
        }

        private void EnableControlOnMouseDown(MouseEventArgs e, Control ctrl)
        {
            if (ctrl.Enabled) return;
            if (e.X > ctrl.Location.X && e.X < ctrl.Location.X + ctrl.Width && e.Y > ctrl.Location.Y && e.Y < ctrl.Location.Y + ctrl.Height)
            {
                ctrl.Enabled = true;
                _dateTimePickerHelper.Open(ctrl as DateTimePicker);
            }
        }

        //private void chkFlip_CheckedChanged(object sender, EventArgs e)
        //{
        //    if(CtrlGrid.SelectedObjects.Count == 0)
        //        CtrlGrid.SelectObjects(CtrlGrid.Objects.Cast<SenseLogInfo>().ToList());
        //    else
        //        CtrlGrid.SelectObjects(new List<SenseLogInfo>());
        //}
    }
}
