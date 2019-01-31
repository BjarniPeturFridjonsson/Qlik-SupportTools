using System;
using System.Drawing;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;
using FreyrCommon.Logging;
using FreyrQvLogCollector.QvCollector;

namespace FreyrQvLogCollector.Dialogues
{
    public partial class FrmConnectToRemoteServer : Form
    {

        
        private readonly ILogger _logger;
        private QlikViewConnectDto _connectDto;

        public FrmConnectToRemoteServer(QlikViewConnectDto connectDto, ILogger logger)
        {
            InitializeComponent();
            _connectDto = connectDto;
            _logger = logger;
            AcceptButton = cmdConnect;
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            txtAddress.Text = txtAddress.Text.Trim();
            ToggleUi(false);
            if (txtAddress.Text.StartsWith("http://"))
            {
                txtAddress.Text = txtAddress.Text.Substring(8);
            }

            if (txtAddress.Text.StartsWith("https://"))
            {
                txtAddress.Text = txtAddress.Text.Substring(8);
            }

            lblErrorMessage.Text = @"Trying to connect to server.";
            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                txtAddress.BackColor = Color.FromArgb(255, 255, 69, 0);
                ToggleUi(true);
                return;
            }

            
            Application.DoEvents();
            _connectDto.QmsAddress = $"http://{txtAddress.Text}:4799/QMS/Service";
            _connectDto = new ConnectToQlikViewHelper(_logger).TryAccessQmsApi(_connectDto);
            if (_connectDto.QlikViewServerLocationFinderStatus == QlikViewServerLocationFinderStatus.Success)
            {
                Close();
                DialogResult = DialogResult.OK;
                lblErrorMessage.Text = "";
                return;
            }

            lblErrorMessage.Text = _connectDto.QlikViewServerLocationFinderStatus.GetDescription() + @"!";
            ToggleUi(true);

        }

        private void ToggleUi(bool enableState)
        {
            txtAddress.Enabled = enableState;
            cmdConnect.Enabled = enableState;
            cmdExit.Enabled = enableState;
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            txtAddress.BackColor = SystemColors.Window;
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }

        private void FrmConnectToRemoteServer_Load(object sender, EventArgs e)
        {
            ActiveControl = txtAddress;
        }
    }
}
