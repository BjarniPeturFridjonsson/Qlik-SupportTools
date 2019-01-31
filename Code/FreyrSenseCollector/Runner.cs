using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrCollectorCommon.CollectorCore;
using FreyrCollectorCommon.Winform;
using FreyrCommon;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using FreyrSenseCollector.Dialogues;

namespace FreyrSenseCollector
{
    public class Runner
    {
        public CommonCollectorServiceVariables Settings { get; }
        public SenseCollectorService CollectorService { get; private set; }
        public Action<string, string, SenseCollectorService> DoneAction { get; set; }

        private readonly ILogger _logger;
        private Action<string, MessageLevels, string> _notify;
        private readonly Form _owner;

        public Runner(ILogger logger, Form owner)
        {
            _logger = logger;
            _owner = owner;
            var name = Process.GetCurrentProcess().ProcessName;

            try
            {
                Settings = new FileNamingProcessor().DecodeFileName(name);
            }
            catch (Exception e)
            {
                Settings = new CommonCollectorServiceVariables();
                _logger.Add($"The filenaming processor failed decoding variables for name {name} and got {e}");
            }
        }

        public async Task Run(string txtCaseNr, Action<string, MessageLevels, string> notify)
        {
            _notify = notify;
            var caseNr = txtCaseNr?.Trim().Replace("_", "") + "";
            if (!string.IsNullOrEmpty(caseNr))
                caseNr += "_";
            Settings.ApplicatonBaseName = "SenseCollector";
            var resultPathRutine = new GetOutputFolder().FallbackRutine(Settings.ApplicatonBaseName);
            Settings.LogFilePath = Path.Combine(resultPathRutine, "SenseCollector.log");
            Settings.OutputFolderPath = resultPathRutine;
            Log.Init(Settings.LogFilePath);//todo: remove and move into local log implimentation. Warning init with empty string will create a device zero logging.
            _logger.Add($"started with version {Application.ProductVersion}");
            _notify("Connecting to Qlik Sense Installation", MessageLevels.Animate, "Connecting");//,ServiceRunStatus.Running, "Connection");

            CollectorService = new SenseCollectorService(_logger, _notify, DoneAction)
            {
                ConnectToSenseApiManuallyDlg = ShowConnectionFailuresDialogue,
                PathToSenseLogFolderDlg = ShowSenseLogFolderDialogue,
                PathToArchivedLogsDlg = ShowArchivedLogsFolderDialogue,
            };
            //_settings.StartDateForLogs = _startDate;
            //_settings.StopDateForLogs = _stopDate;
            Settings.UseOnlineDelivery = false; //chkOnline.Checked;
            Settings.AllowRemoteLogs = FreyrCollectorCommon.Common.Settings.AllowRemoteLogs;
            Settings.AllowArchivedLogs = FreyrCollectorCommon.Common.Settings.AllowArchivedLogs;

            Settings.AllowSenseInfo = FreyrCollectorCommon.Common.Settings.AllowSenseInfo;
            Settings.DnsHostName = (Dns.GetHostEntry(Dns.GetHostName()).HostName).ToLower();
            Settings.CustomerKey = "";
            Settings.Key = caseNr;

            try
            {
                await CollectorService.Start(Settings).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Log.Add("Failed accessing installation.", ex);
                DoneAction.Invoke(@"Failed accessing installation.", @"We unfortunately had a problem reading the Qlik Sense installation. You will have to manually send us your logs.", CollectorService);
            }
        }

        //todo: brake out to fancy winforms broker
        private SenseConnectDto ShowSenseLogFolderDialogue(SenseConnectDto dto)
        {
            string pathToFolder = string.Empty;
            string previousPath = @"C:\ProgramData\Qlik\Sense\Log";
            DialogResult dlgRes = DialogResult.OK;
            while (string.IsNullOrEmpty(pathToFolder) && dlgRes == DialogResult.OK)
            {
                string invalidPathString = string.Empty;


                _owner.Invoke(new Action(() =>
                {
                    var a = new SuperInputDialogue(
                        title: "Path to Qlik Sense Log Folder",
                        text: invalidPathString + "Please write the correct path to the Qlik Sense logs.",
                        defaultValue: previousPath
                    )
                    { StartPosition = FormStartPosition.CenterParent };
                    a.ShowDialog(_owner);
                    previousPath = a.InputTextValue;
                    dlgRes = a.DialogResult;
                    if (dlgRes == DialogResult.OK)
                    {
                        if (Directory.Exists(a.InputTextValue))
                        {
                            pathToFolder = a.InputTextValue;
                        }
                    }

                    invalidPathString = "Invalid Path. ";
                }));

            }
            dto.PathToLocalSenseLogFolder = pathToFolder;
            return dto;
        }

        //todo: break out to fancy winforms broker
        private void ShowDialogue(Form frmDlg)
        {
            if (_owner.IsDisposed) return;
            _owner.Invoke(new Action(() =>
            {
                if (_owner.IsDisposed) return;
                frmDlg.ShowDialogueCenterParent(_owner);
            }));
        }

        private string ShowArchivedLogsFolderDialogue(string inPath)
        {
            var dlg = new FrmArchivedLogsConnectionIssues(_logger);
            ShowDialogue(dlg);
            return dlg.DialogResult == DialogResult.OK ? dlg.TheNewPath : inPath;
        }

        private SenseConnectDto ShowConnectionFailuresDialogue(SenseConnectDto dto)
        {
            var dlg = new FrmConnectionIssues(dto, _logger);
            ShowDialogue(dlg);

            if (dlg.DialogResult == DialogResult.Abort)
                dlg.ConnectDto.AbortAndExit = true;
            dto = dlg.ConnectDto;
            return dto;
        }
    }
}
