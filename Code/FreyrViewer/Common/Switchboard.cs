using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eir.Common.IO;
using FreyrViewer.Common.Winforms;
using FreyrViewer.Services;
using FreyrViewer.Ui;
using FreyrViewer.Ui.Splashes;
using ICSharpCode.SharpZipLib.Zip;
using WeifenLuo.WinFormsUI.Docking;
using ZipFile = System.IO.Compression.ZipFile;

namespace FreyrViewer.Common
{
    public class Switchboard
    {
        private bool _initializeDone;
        private FrmMain _frmMain;
        private string _zipFilePath;
        private readonly IFileSystem _filesystem;

        public ProcessLogCollectorOutput LogCollectorOutput { get; private set; }
        public SimpleControlResize ControlResize { get; } = new SimpleControlResize();
        public ProgramSettings Settings { get; } = new ProgramSettings();
        public static Switchboard Instance { get; } = new Switchboard();
        public string GetCurrentReloadStatus { get; set; }

        public LoadPanelHelper LoadPanelHelper { get; private set; }

        private Switchboard()
        {
            _filesystem = FileSystem.Singleton;
        }

        public async Task ReloadAllData(string filePath)
        {
            await Task.Factory.StartNew(() =>
            {
                Reload(filePath);
            });
        }

        public void CloseAllOpenAndReset()
        {
            LoadPanelHelper.CloseAllOpenForms();
            if (!string.IsNullOrEmpty(_zipFilePath) && _filesystem.DirectoryExists(_zipFilePath))
            {
                var a = Mbox.Show("Do you want to remove the files from last unzipped file?", "Remove temp files",MessageBoxButtons.YesNo);
                if (a == DialogResult.Yes)
                    Task.Run(() => _filesystem.DeleteDirectory(_zipFilePath))
                        .ContinueWith(p=>SetInfoMessage(p.IsFaulted ? "Failed cleaning temp files" : "Temp files cleaned"));
            }
        }

        private void Reload(string filePath)
        {
            GetCurrentReloadStatus = "Starting";
            ValidateStartup();
            if (filePath == null ||(!_filesystem.FileExists(filePath) && !_filesystem.DirectoryExists(filePath)))
            {
                SetInfoMessage("ERROR Path is not correct. Could not load.");
                return;
            }
            
            LogCollectorOutput =null;
            _zipFilePath = string.Empty;
            if (_filesystem.Path.GetExtension(filePath).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                GetCurrentReloadStatus = "Reading zip file";
                string outputFolderPath = _filesystem.Path.Combine(_filesystem.Path.GetTempPath(), RemoveAllDots(filePath) + "_" + RemoveAllDots(_filesystem.Path.GetTempFileName()));
                if (_filesystem.DirectoryExists(outputFolderPath))
                {
                    outputFolderPath = _filesystem.Path.Combine(outputFolderPath, _filesystem.Path.GetTempFileName());
                }
                _zipFilePath = outputFolderPath;
                int fileCount = 0;
                int totalCount;
                using (var archive = ZipFile.OpenRead(filePath))
                {
                    totalCount = archive.Entries.Count;
                }
                var events = new FastZipEvents();
                FastZip fastZip = new FastZip(events);
                events.CompletedFile += (sender, args) =>
                {
                    fileCount++;
                    GetCurrentReloadStatus = $"Unzipping file # {fileCount} of {totalCount}";
                };
                
                // Will always overwrite if target filenames already exist
                fastZip.ExtractZip(filePath, outputFolderPath,null);

                if (!_filesystem.DirectoryGetFiles(outputFolderPath).Any())
                {
                    if (_filesystem.GetDirectories(outputFolderPath).Count() == 1)
                        outputFolderPath = _filesystem.GetDirectories(outputFolderPath).First();
                    else
                    {
                        GetCurrentReloadStatus = "Failed reading the zipfile";
                        throw new Exception("I can't understand the format of this zipfile.");
                    }
                }
                ParsePathForLogs(outputFolderPath);
            }
            else
            {
                ParsePathForLogs(filePath);
            }
            
            GetCurrentReloadStatus = "Reading files";
            LogCollectorOutput?.GroupedServerInfo?.ForEach(p =>
                LoadPanelHelper.AddSenseLogsForHost(
                    p.QlikSenseMachineInfo.HostName + (p.QlikSenseMachineInfo.IsCentral ? " (Central node)" : ""),
                    p.Logs
                ));
            GetCurrentReloadStatus = "";
        }

      

        public void ReceiveDragNdrop(object sender, DragEventArgs e)
        {
            _frmMain.FrmMain_DragDrop(sender, e);
        }

        public void SetInfoMessage(string msg)
        {
            _frmMain.SetInfoMessage(msg);
        }

        private string RemoveAllDots(string path)
        {
            string newPath = path;
            while (newPath.Contains("."))
            {
                newPath = _filesystem.Path.GetFileNameWithoutExtension(newPath);
            }
            return newPath;
        }

        private void ParsePathForLogs(string filePath)
        {
            LogCollectorOutput = new ProcessLogCollectorOutput(filePath, _filesystem);
        }

        private void ValidateStartup()
        {
            if (!_initializeDone)
                throw new Exception("Startup has not been called.. tsk tsk tsk.");
        }

        internal void Initialize(DockPanel ctrlDockPanelMain, FrmMain frmMain)
        {
            if (_initializeDone)
                return;
            _initializeDone = true;
            _frmMain = frmMain;
            LoadPanelHelper = new LoadPanelHelper(ctrlDockPanelMain, frmMain, new CommonResources());
            LoadPanelHelper.Start();
            SplashManager.SetMainForm(_frmMain);
        }
    }
}