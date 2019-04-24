using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eir.Common.IO;
using OfflineDataExporter.Db;

namespace OfflineDataExporter
{
    public partial class FrmMain : Form
    {
        private GjallarhornDb _db;

        public FrmMain()
        {
            InitializeComponent();
            _db = new GjallarhornDb(FileSystem.Singleton);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            (int rowCount, DateTime lastRunDate) = _db.GetCurrentStateData();
            lblInfo.Text = $"There are {rowCount} rows not exported." + (lastRunDate == DateTime.MinValue ? "" : $"/r/nLast export done on {lastRunDate.ToString("yyyy-MM-dd hh:mm")}");

            if (rowCount == 0) cmdExport.Enabled = false;
            if (lastRunDate == DateTime.MinValue) cmdReExport.Enabled = false;
        }
        
        private void cmdRun_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Path.GetTempPath(),$"ProactiveExpressExport_{DateTime.Now.ToString("yyyyMMddhhmmss")}");
            Directory.CreateDirectory(path);
            _db.ExportData(path);
        }

        private void cmdReExport_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Path.GetTempPath(), $"ProactiveExpressExport_{DateTime.Now.ToString("yyyyMMddhhmmss")}");
            Directory.CreateDirectory(path);
            _db.ExportData(path);
        }
    }
}
