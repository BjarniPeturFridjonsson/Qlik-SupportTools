using System;
using System.Windows.Forms;
using FreyrCommon.Logging;

namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmResult : Form
    {
        public FrmResult(QlikViewCollectorService service,ILogger logger)
        {
            InitializeComponent();
            try
            {
                var loc = service?.ServiceVariables?.CollectorOutput?.ZipFile;
                if (string.IsNullOrEmpty(loc))
                {
                    MessageBox.Show(@"We have no files to show. You probably don't have the right access rights.", @"Failure getting output path");
                    logger.Add("No files to show in FrmResults. Probably access rights.");
                }
                    
                txtLogFile.Text = loc;
            }
            catch(Exception ex)
            {
                logger.Add("Failure to show files to show in FrmResults. Probably access rights.",ex);
                MessageBox.Show(@"We have no files to show. Something went badly wrong", @"Failure getting output path");
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{txtLogFile.Text}\"");
        }
    }
}
