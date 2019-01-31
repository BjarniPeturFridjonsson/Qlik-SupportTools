using System;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;

namespace FreyrSenseCollector.Dialogues
{
    public partial class FrmConnectionIssuesInfo : Form
    {
        private readonly string _detailedConnErrMsg;
        public FrmConnectionIssuesInfo(SenseConnectDto dto)
        {
            InitializeComponent();
            switch (dto.SenseServerLocationFinderStatus)
            {

                case SenseServerLocationFinderStatus.NoSertificateFound:
                    _detailedConnErrMsg = "We haven't found any Qlik Sense certificate on this machine. Please run this on the Qlik Sense server that has the issue, or the central node.";
                    break;
                case SenseServerLocationFinderStatus.Forbidden:
                    _detailedConnErrMsg = "We can't access Qlik Sense. You should run this application with the service account that is running Qlik Sense.";
                    break;
                default:
                    _detailedConnErrMsg = "We can't access Qlik Sense. Please run this on the Qlik Sense server that has the issue. We have no further details.";
                    break;

            }
        }

        private void ShowInfo()
        {
            var a = new ReadEmbeddedResources();
            CtrlRichTxtBox.Rtf = a.Read("FreyrCollectorCommon.DialogueFiles.How_To_Connect_To_Sense.rtf").Replace("<TheErrorMsg>", _detailedConnErrMsg);
        }
        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmConnectionIssues_Info_Load(object sender, EventArgs e)
        {
            ShowInfo();
        }
    }
}
