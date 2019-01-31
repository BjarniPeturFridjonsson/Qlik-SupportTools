using System;
using System.Windows.Forms;

namespace FreyrSenseCollector.Dialogues
{
    public partial class FrmResult : Form
    {
        public FrmResult(SenseCollectorService service)
        {
            InitializeComponent();
            try
            {
                var loc = service?.ServiceVariables?.CollectorOutput?.ZipFile;
                if (string.IsNullOrEmpty(loc))
                    MessageBox.Show(@"We have no files to show. You probably don't have the right access rights.", @"Failure getting output path");
                txtLogFile.Text = loc;
            }
            catch 
            {
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
