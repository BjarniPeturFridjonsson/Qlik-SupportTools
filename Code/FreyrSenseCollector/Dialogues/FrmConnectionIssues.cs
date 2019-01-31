using System;
using System.Windows.Forms;
using FreyrCollectorCommon.Winform;
using FreyrCollectorCommon.CollectorCore;
using FreyrCommon.Logging;

namespace FreyrSenseCollector.Dialogues
{
    public partial class FrmConnectionIssues : Form
    {
        private readonly ILogger _logger;

        public SenseConnectDto ConnectDto { get; private set; }
        public FrmConnectionIssues(SenseConnectDto dto, ILogger logger)
        {
            InitializeComponent();
            ConnectDto = dto;
            _logger = logger;
        }

        private void ShowRunHowTo()
        {
            var dlg = new FrmConnectionIssuesInfo(ConnectDto);
            dlg.ShowDialogueCenterParent(this);
            _logger.Add($"Conn issue form is showing the info form");
        }

        private void ShowConnToRemoteHost()
        {
            _logger.Add($"Conn issue form is showing the Connect to remote server form");
            var dlg = new FrmConnectToRemoteServer(ConnectDto, _logger);
            dlg.ShowDialogueCenterParent(this);
            ConnectDto = dlg.ConnectDto;
            if (ConnectDto.SenseServerLocationFinderStatus != SenseServerLocationFinderStatus.Success)
            {
                _logger.Add($"Conn issue form is trying to access the location {ConnectDto.SenseHostName} and failing");
                return;
            }
            _logger.Add($"Conn issue form successfully accessed the new server {ConnectDto.SenseHostName}");
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
