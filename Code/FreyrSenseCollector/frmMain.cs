using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrSenseCollector.Dialogues;

namespace FreyrSenseCollector
{
    public partial class FrmMain : Form
    {
        private int _lblLastCreatedPos;
        private readonly ILogger _logger = new Logger("Qlik Sense Log Collector");
        private Runner _runner;
        private AnimationHelper _anime;

        public FrmMain()
        {
            InitializeComponent();
            
            DoInit();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //var a = new FrmConnectToRemoteServer(new SenseConnectDto { SenseServerLocationFinderStatus = SenseServerLocationFinderStatus.NoSertificateFound }, _logger);
            //a.ShowDialog();
        }

        private void DoInit()
        {
            _anime = new AnimationHelper(this);
            this.MakeLabelTransparent(lblGeneralInfo, picCircles);
            this.MakeLabelTransparent(lblPrivacy, picCircles);
            //MessageBox.Show(@"name is " + name);
            //name = "QlikDiagnostics_kjIZWf4yJIu.zip";
            //name = "QlikDiagnostics_cB1RhvFcpI.zip";
            try
            {
                
                _runner = new Runner(_logger, this);
                _runner.Settings.CollectorOutput.LogCollectorVersion = Application.ProductVersion;
                _runner.DoneAction = Done;
                chkSysInfo.Checked = true;//_settings.AllowMachineInfo;
                chkWinLogs.Checked = true;//_settings.AllowWindowsLogs;
                chkOnline.Checked = _runner.Settings.UseOnlineDelivery;
                dteStart.Value = _runner.Settings.StartDateForLogs;
                dteStop.Value = _runner.Settings.StopDateForLogs;
                txtCaseNr.Text = _runner.Settings.Key;
            }
            catch (Exception e)
            {
                _logger.Add("Error in startup", e);
                Notify("Failed getting output folder. Please run with higer user rights.", MessageLevels.Error);
                ResetUiFinished();
            }
        }

        private async Task Start()
        {
            if (cmdStart.Text.Equals("Close"))
                Close();
            if (!SanityCheck())
                return;
            //if (txtKey.Text == "")
            //{
            //    txtKey.BackColor = Color.FromArgb(255,255, 69, 0);
            //    return;
            //}
            ToggleLockUi(false);

            ctrlProgressbar.Visible = true;
            cmdStart.Enabled = false;
            cmdStart.Text = @"Please Wait";
            _runner.Settings.AllowWindowsLogs = chkWinLogs.Checked;
            _runner.Settings.AllowMachineInfo = chkSysInfo.Checked;
            _runner.Settings.GetLogsScripting = ChkScriptLogs.Checked;
            _runner.Settings.StartDateForLogs = dteStart.Value;
            _runner.Settings.StopDateForLogs = dteStop.Value.AddDays(1).AddTicks(-1);
            _runner.Settings.SendId = txtCaseNr.Text + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            await _runner.Run(txtCaseNr.Text, Notify).ConfigureAwait(false);
        }

        private bool SanityCheck()
        {
            if(dteStop.Value.Subtract(dteStart.Value).Days > 30)
                return MessageBox.Show(@"Large number of days may result in large zip files." + Environment.NewLine + @"Do you want to continue?",@"File size question",MessageBoxButtons.OKCancel)== DialogResult.OK;
            if (dteStop.Value.Subtract(dteStart.Value).Days < 0)
                return MessageBox.Show(@"Start date has to happen before the end date." + Environment.NewLine + @"Please fix the ""Logs From"" dates.", @"Incorrect dates", MessageBoxButtons.OK) != DialogResult.OK;
            return true;
        }

        private void Done(string header,string msg, SenseCollectorService service)
        {
            if (service.AbortAndExit)
            {
                Close();
                return;
            }
            ResetUi();
            if (!string.IsNullOrEmpty(msg))
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show(this, msg, header);
                    //MessageBox.Show(this,$@"We are now finished collecting information. {Environment.NewLine}{Environment.NewLine}Hopefully we will solve your issue shortly.", @"Qlik Diagnostics has finished");
                }));
            }

            if (!string.IsNullOrEmpty(service.ServiceVariables.CollectorOutput.ZipFile) && File.Exists(service.ServiceVariables.CollectorOutput.ZipFile))
            {
                Process.Start("c:\\windows\\explorer.exe", $"/select,\"{service.ServiceVariables.CollectorOutput.ZipFile}\"");
            }

            ResetUiFinished();
        }
       
        private void ToggleLockUi(bool enabled)
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox || control is Button || control is CheckBox || control is DateTimePicker)
                    control.Enabled = enabled;
            }
        }
        
        private void ResetUi()
        {
            cmdStart.Invoke(new Action(() =>
            {
                ctrlProgressbar.Visible = false;
                lblAttachMsg.Visible = false;
                cmdLogFile.Visible = false;
                cmdProblems.Visible = false;
                cmdStart.Text = @"Finished";
            }));
        }
        private void ResetUiFinished()
        {
            cmdStart.Invoke(new Action(() =>
            {
                cmdStart.Text = @"Close";
                lblAttachMsg.Visible = true;
                cmdStart.Enabled = true;
                cmdLogFile.Visible = true;
                cmdLogFile.Enabled = true;
                if (_runner?.Settings?.Issues?.Any() ?? false)
                {
                    cmdProblems.Visible = true;
                    cmdProblems.Enabled = true;
                }
            }));
        }

        private void Notify(string msg, MessageLevels msgLevel, string modKey = "")
        {
            if (picCheckbox.IsDisposed || picCheckbox.Disposing)
                return;

            picCheckbox.Invoke(new Action(() =>
            {
                try
                {
                    bool modKeyFound = false;
                    if (!string.IsNullOrEmpty(modKey))
                    {
                        foreach (Control ctrl in Controls)
                        {
                            if (ctrl.Tag?.ToString() == modKey)
                            {
                                if (ctrl is PictureBox inPic) _anime.ImageFromMsgLevel(inPic, msgLevel);
                                else if (ctrl is Label inLbl && inLbl.Text != msg)
                                {
                                   inLbl.Text = msg;
                                    _logger.Add($"Notified on:{msg} with key {modKey}");
                                }

                                modKeyFound = true;
                            }
                        }

                        if (modKeyFound) return;
                    }

                    if (_lblLastCreatedPos == 0) _lblLastCreatedPos = picCheckbox.Top;
                    _lblLastCreatedPos += 30;
                    var pic = picCheckbox.Clone();
                    _anime.GetFirstImage(pic, modKey, msgLevel);
                    pic.Top = _lblLastCreatedPos;
                    pic.Visible = true;
                    pic.Tag = modKey + "";
                    var lbl = lblMsg.Clone();
                    lbl.Top = _lblLastCreatedPos;
                    lbl.Text = msg;
                    lbl.Visible = true;
                    lbl.Tag = modKey + "";
                    picCircles.SendToBack();
                    _logger.Add($"Notified on:{msg} with key {modKey ?? "null"}");

                }
                catch (Exception ex)
                {
                    _logger.Add($"Notify failed  ex:{ex}");
                }
                finally
                {
                    Application.DoEvents();
                }
            }));
        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            txtCaseNr.BackColor = SystemColors.Window;
        }

        private void lblGeneralInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var a = new FrmReadMePopup();
            a.ShowDialogueCenterParent(this);
        }

        private void cmdLogFile_Click(object sender, EventArgs e)
        {
            new FrmResult(_runner.CollectorService).ShowDialogueCenterParent(this);
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            _anime.AnimationTick(); 
        }

        private async void cmdStart_Click(object sender, EventArgs e)
        {
            await Start();
        }

        private void cmdProblems_Click(object sender, EventArgs e)
        {
            new FrmProblemsFound(_runner.CollectorService.ServiceVariables.Issues).ShowDialogueCenterParent(this);
        }
    }
}
    