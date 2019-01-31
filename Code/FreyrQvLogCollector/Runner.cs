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
using FreyrQvLogCollector.Dialogues;
using FreyrQvLogCollector.QvCollector;

namespace FreyrQvLogCollector
{
    public class Runner
    {
        public Action<string, string, QlikViewCollectorService> DoneAction { get; set; }
        public CommonCollectorServiceVariables Settings { get; }
        public QlikViewCollectorService CollectorService { get; private set; }

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
            Settings.ApplicatonBaseName = "QlikViewCollector";
            var resultPathRutine = new GetOutputFolder().FallbackRutine(Settings.ApplicatonBaseName);
            Settings.LogFilePath = Path.Combine(resultPathRutine, "QvCollector.log");
            Settings.OutputFolderPath = resultPathRutine;
            Log.Init(Settings.LogFilePath);//todo: remove and move into local log implimentation. Warning init with empty string will create a device zero logging.
            _logger.Add($"started with version {Application.ProductVersion}");
            _notify("Connecting to QlikView Installation", MessageLevels.Animate, "Connecting");

            CollectorService = new QlikViewCollectorService(_logger, _notify, DoneAction)
            {
                ConnectToApiManuallyDlg = ShowConnectionFailuresDialogue,
                PathToLogFolderDlg = ShowLogFolderDialogue,
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
            Settings.QvSettings.QmsAddress = $"http://{Settings.DnsHostName}:4799/QMS/Service";

            try
            {
                await CollectorService.Start(Settings).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

                Log.Add("Failed accessing installation.", ex);
                DoneAction.Invoke(@"Failed accessing installation.", @"We unfortunately had a problem reading the QlikView installation. You will have to manually send us your logs.", CollectorService);
            }
        }

     

        //todo: Fix for QlikView connection
            private QlikViewConnectDto ShowLogFolderDialogue(QlikViewConnectDto dto)
        {
            string pathToFolder = string.Empty;
            string previousPath = @"C:\ProgramData\QlikTech";
            DialogResult dlgRes = DialogResult.OK;
            while (string.IsNullOrEmpty(pathToFolder) && dlgRes == DialogResult.OK)
            {
                string invalidPathString = string.Empty;


                _owner.Invoke(new Action(() =>
                {
                    var a = new SuperInputDialogue(
                        title: "Path to QlikView Log Folder",
                        text: invalidPathString + "Please write the correct path to the QlikView logs.",
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
                    else
                    {
                        dto.AbortAndExit = true;
                    }
                    invalidPathString = "Invalid Path. ";
                }));

            }
            dto.PathToLocalLogFolder = pathToFolder;
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

        private QlikViewConnectDto ShowConnectionFailuresDialogue(QlikViewConnectDto dto)
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
