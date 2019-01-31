using System;
using System.IO;
using System.Windows.Forms;
using FreyrCommon.Logging;

namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmArchivedLogsConnectionIssues : Form
    {
        public string TheNewPath { get; private set; }

        public string NewPath()
        {
            return TheNewPath;
        }
        private readonly ILogger _logger;

        public FrmArchivedLogsConnectionIssues(ILogger logger)
        {
            InitializeComponent();
            _logger = logger;
            _logger.Add("The archived folder is not accessable. Retry form shown.");
        }

        private void cmdTry_Click(object sender, EventArgs e)
        {
            TheNewPath = txtPath.Text;
            if (!Directory.Exists(TheNewPath))
            {
                _logger.Add("The supplied archived folder is not accessable");
                MessageBox.Show(@"Please supply an existing and accessible directory", @"The directory does not exist.");
                return;
            }
            _logger.Add($"Retrying run with the folder {TheNewPath}");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog { Description = @"Archived Folder path" };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                TheNewPath = fbd.SelectedPath;
            }

            txtPath.Text = TheNewPath;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            _logger.Add($"The archived folder retry is cancelled. Run will be continued without archived folders");
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
