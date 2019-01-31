using System;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;

namespace FreyrSenseCollector.Dialogues
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
            richTextBox1.Rtf = a.Read("FreyrCollectorCommon.DialogueFiles.How_To_Connect_To_Sense.rtf");
        }


        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
