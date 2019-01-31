using System;
using System.Windows.Forms;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrQvLogCollector.QvCollector;

namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmConnectionIssues : Form
    {
        private readonly ILogger _logger;

        public QlikViewConnectDto ConnectDto { get; private set; }
        public FrmConnectionIssues(QlikViewConnectDto dto, ILogger logger)
        {
            InitializeComponent();
            ConnectDto = dto;
            _logger = logger;
        }

        private void ShowRunHowTo()
        {
            var dlg = new FrmConnectionIssuesInfo(ConnectDto, _logger);
            _logger.Add($"Conn issue form is showing the info form");
            dlg.ShowDialogueCenterParent(this);
            if (ConnectDto.AbortAndExit)
            {
                Close();
                DialogResult = DialogResult.Abort;
            }
        }

        private void ShowConnToRemoteHost()
        {
            _logger.Add($"Conn issue form is showing the Connect to remote server form");
            var dlg = new FrmConnectToRemoteServer(ConnectDto, _logger);
            dlg.ShowDialogueCenterParent(this);

            if (ConnectDto.QlikViewServerLocationFinderStatus != QlikViewServerLocationFinderStatus.Success)
            {
                _logger.Add($"Conn issue form is trying to access the location {ConnectDto.QmsAddress} and failing");
                return;
            }
            _logger.Add($"Conn issue form successfully accessed the new server {ConnectDto.QmsAddress}");
            Close();
            DialogResult = DialogResult.OK;
        }

        private void RunLocalOnly()
        {
            _logger.Add($"Run is going to run with dead installation choice made.");
            ConnectDto.RunWithDeadInstallation = true;
            Close();
            DialogResult = DialogResult.OK;
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            _logger.Add($"Conn issue form aborted");
            Close();
            DialogResult = DialogResult.Abort;
        }

        private void cmdConnectToRemoteHost_Click(object sender, EventArgs e)
        {
            ShowConnToRemoteHost();
        }

        private void cmdRunOnlyLocalLogs_Click(object sender, EventArgs e)
        {
            RunLocalOnly();
        }

        private void cmdShowConnectionHowTo_Click(object sender, EventArgs e)
        {
            ShowRunHowTo();
        }


    }
}
