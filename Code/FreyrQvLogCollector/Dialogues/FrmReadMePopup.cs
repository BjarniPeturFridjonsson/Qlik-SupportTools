using System;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;

namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmReadMePopup : Form
    {
        public FrmReadMePopup()
        {
            InitializeComponent();
            richTextBox1.Enabled = false;
        }

        private void FrmReadMePopup_Load(object sender, EventArgs e)
        {
            var a = new ReadEmbeddedResources();
            richTextBox1.Rtf = a.Read("FreyrCollectorCommon.DialogueFiles.What_do_we_collect.rtf");
        }


        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
