using System;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;
using FreyrCommon.Logging;
using FreyrQvLogCollector.QvCollector;


namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmConnectionIssuesInfo : Form
    {
        private readonly ILogger _logger;
        private readonly QlikViewConnectDto _dto;

        public FrmConnectionIssuesInfo(QlikViewConnectDto dto, ILogger logger)
        {
            InitializeComponent();
            _logger = logger;
            _dto = dto;
            var a = new ConnectToQlikViewHelper(logger);
            if (!a.IsPartOfApiGroup(dto.QmsAddress))
            {
                cmdCreateApiAccount.Visible = true;
            }        
        }

        private void ShowInfo()
        {
            var a = new ReadEmbeddedResources();
            CtrlRichTxtBox.Rtf = a.Read("FreyrCollectorCommon.DialogueFiles.How_To_Connect_To_QlikView.rtf");
        }
        private void cmdExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FrmConnectionIssues_Info_Load(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private void cmdCreateApiAccount_Click(object sender, EventArgs e)
        {
            cmdCreateApiAccount.Enabled = false;
            cmdExit.Enabled = false;
            var a = new ConnectToQlikViewHelper(_logger);
            a.CreateQvApiGroup(_dto.QmsAddress);

            if (!a.IsPartOfApiGroup(_dto.QmsAddress))
            {
                MessageBox.Show(this,@"Failed creating the Api account. Please do so manually.", @"Api Account was not created");
                _logger.Add($"Failed creating the Api account automatically{ _dto.QmsAddress}");
            }
            else
            {
                MessageBox.Show(this, $@"We have created the api group and added you as a member of that group.{Environment.NewLine} {Environment.NewLine}Unfortunately you need to logoff and on again for the changes to be applied. {Environment.NewLine}If you still have a problem you need to restart the QlikView Server service also.", @"Api Account created");
                _logger.Add($"Successfully creating the Api account automatically on { _dto.QmsAddress}");
            }

            _dto.AbortAndExit = true;
            cmdExit.Enabled = true;
            Close();
            DialogResult = DialogResult.OK;
        }

        private void lnkArticle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://qliksupport.force.com/articles/000003621");
        }
    }
}
