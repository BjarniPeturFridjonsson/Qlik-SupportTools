using Eir.Common.IO;
using OfflineDataExporter.Db;
using OfflineDataExporter.Workers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfflineDataExporter
{
    public partial class FrmMain : Form
    {
        private readonly GjallarhornDb _db;
        private DateTime _lastRunDate;

        public FrmMain()
        {
            InitializeComponent();
            _db = new GjallarhornDb(FileSystem.Singleton);
        }

        private void DoLoad()
        {
            //Process.Start("cmd.exe", "/c del \"C:\\src\\GitHub\\Qlik-SupportTools\\Code\\Tools\\Gjallarhorn\\OfflineDataExporter\\bin\\Debug\\Gjallarhorn.sqllite\"");
            //Process.Start("cmd.exe", "/c copy /y \"C:\\src\\GitHub\\Qlik-SupportTools\\Code\\Tools\\Gjallarhorn\\Gjallarhorn\\bin\\AnyCPU\\Debug\\Gjallarhorn.sqllite\" \"C:\\src\\GitHub\\Qlik-SupportTools\\Code\\Tools\\Gjallarhorn\\OfflineDataExporter\\bin\\Debug\\\"");

            (int rowCount, DateTime lastRunDate) = _db.GetCurrentStateData();
            lblInfo.Text = $@"There are {rowCount} rows not exported." + (lastRunDate == DateTime.MinValue ? "" : $"/r/nLast export done on {lastRunDate.ToString("yyyy-MM-dd hh:mm")}");

            if (rowCount == 0) cmdExport.Enabled = false;
            _lastRunDate = lastRunDate;
            SetState(true);
        }

        private void SetState(bool enabledState)
        {
            cmdClose.Enabled = enabledState;
            cmdExport.Enabled = enabledState;
            cmdReExport.Enabled = enabledState;
            if (_lastRunDate == DateTime.MinValue) cmdReExport.Enabled = false;
        }

        private void Export(DateTime? export )
        {
            SetState(false);
            lblInfo.Text = @"Exporting";
            var path = Path.Combine(Path.GetTempPath(), $"ProactiveExpressExport_tmp_{DateTime.Now.ToString("yyyyMMddhhmmss")}");
            Directory.CreateDirectory(path);
            var zipper = new Zipper(path);
            _db.ExportData(path, export);
            var pathToZip = zipper.ZipFolder(path, $"ProactiveExport_{DateTime.Now.ToString("yyyyMMddhhmmss")}", Directory.GetParent(path).FullName);
            txtZipPath.Text = pathToZip;
            lblInfo.Text = @"Finished exporting";
            DeleteExportFolderTemp(path);
            cmdClose.Enabled = true;
        }

        private void DeleteExportFolderTemp(string path)
        {
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(p =>
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"The delete of path {path} failed. {ex}");
                }
            });
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            DoLoad();
        }

        private void cmdRun_Click(object sender, EventArgs e)
        {
            Export(null);
        }

        private void cmdReExport_Click(object sender, EventArgs e)
        {
            if (_lastRunDate == DateTime.MinValue) return;
            Export(_lastRunDate);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
