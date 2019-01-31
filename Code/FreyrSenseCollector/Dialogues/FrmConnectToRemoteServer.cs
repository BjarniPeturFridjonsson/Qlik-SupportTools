using System;
using System.Drawing;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;
using FreyrCommon.Logging;

namespace FreyrSenseCollector.Dialogues
{
    public partial class FrmConnectToRemoteServer : Form
    {

        public SenseConnectDto ConnectDto { get; private set; }    
        private readonly ILogger _logger;

        public FrmConnectToRemoteServer(SenseConnectDto connectDto, ILogger logger)
        {
            InitializeComponent();
            ConnectDto = connectDto;
            _logger = logger;
            AcceptButton = cmdConnect;
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            txtAddress.Text = txtAddress.Text.Trim();

            if (txtAddress.Text.StartsWith("http://"))
            { 
                lblErrorMessage.Text = @"Sorry only https is accepted.";
                return;
            }

            if (txtAddress.Text.StartsWith("https://"))
            {
                txtAddress.Text = txtAddress.Text.Substring(8);
            }

            lblErrorMessage.Text = @"Trying to connect to server.";
            if (string.IsNullOrEmpty(txtAddress.Text))
            {
                txtAddress.BackColor = Color.FromArgb(255, 255, 69, 0);
                return;
            }

            Application.DoEvents();
            ConnectDto.SenseHostName = txtAddress.Text;
            ConnectDto = new ConnectToSenseHelper(_logger).TryAccessSenseApi(ConnectDto);
            if (ConnectDto.SenseServerLocationFinderStatus == SenseServerLocationFinderStatus.Success)
            {
                Close();
                DialogResult = DialogResult.OK;
            }

            lblErrorMessage.Text = ConnectDto.SenseServerLocationFinderStatus.GetDescription() + @"!";
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
