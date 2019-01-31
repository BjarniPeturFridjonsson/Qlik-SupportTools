using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eir.Common.IO;
using Eir.Common.Net;
using FreyrViewer.Common;
using FreyrViewer.Ui.MdiForms;
using FreyrViewer.Ui.Splashes;
using Odin.Common;

namespace FreyrViewer.Ui
{
    // the dockpanelsuite/dockpanelsuite  used here uses MIT license.
    //The MIT License
    //Copyright(c) 2007 Weifen Luo(email: weifenluo @yahoo.com)
    //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


    public partial class FrmMain : Form
    {
        private readonly IFileSystem _filesystem = FileSystem.Singleton;
        private bool _isInitialized;
        public FrmMain()
        {
            InitializeComponent();
            lblStrip1.Text = "";
            CtrlDockPanelMain.DockLeftPortion = 220;
            CtrlDockPanelMain.DockTopPortion = 95;
        }

        private void DoLoad()
        {
            Application.DoEvents();
            SplashManager.SetMainForm(this);
            CheckVersion();         
        }

        [Conditional("InternalRelease"), Conditional("DEBUG")]
        private void CheckVersion()
        {
            try
            {
                Text += @" - Internal Qlik release only";
                var url = ConfigurationManager.AppSettings.Get("UpdateUrl");

                IStoreFactory storeFactory = new StoreFactory(new BaseUris(new[] { url }, MultiUriSelectionStrategyFactory.Default));

                Task.Run(() => StartupSupport.AppStartupAsync(storeFactory)).ContinueWith(p =>
                {
                    SetInfoMessage("Version check completed.");
                });

            }
            catch (Exception e)
            {
                SetInfoMessage("Failed checking for updates. ");
                Trace.WriteLine(e);
            }
           
        }

        public void SetInfoMessage(string msg)
        {
            Invoke(new Action(() =>
            {
                if (lblStrip1.Text == null || !lblStrip1.Text.Equals(msg))
                    lblStrip1.Text = msg;
            }));
        }

        private async Task DoReload(string path)
        {
            Switchboard.Instance.Initialize(CtrlDockPanelMain, this);
            var splash = SplashManager.Loader.ShowFloatingSplash(this,"Loading files...");
            
            try
            {
                if (_filesystem.FileExists(path) && !_filesystem.Path.GetExtension(path).Equals(".zip", StringComparison.InvariantCultureIgnoreCase) && _isInitialized)
                { //single file
                    
                        var res = Mbox.Show("Do you want to keep all other files open?,", "Keep files open", MessageBoxButtons.YesNo);
                        if (res != DialogResult.Yes)
                        {
                            Switchboard.Instance.CloseAllOpenAndReset();
                        }
                        await Switchboard.Instance.ReloadAllData(path);
                        await Switchboard.Instance.LoadPanelHelper.ShowLastLoadedLog();
                }
                else
                {
                    Switchboard.Instance.CloseAllOpenAndReset();
                    await Switchboard.Instance.ReloadAllData(path);
                    Switchboard.Instance.LoadPanelHelper?.ShowStartPageAfterLoad();
                }
                
                lblStrip1.Text = $@"Data load done {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception e)
            {
                MessageBox.Show(@"We did not manage to read this.\r\n" + e);

            }
            finally
            {
                splash.Dispose();
                _isInitialized = true;
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            DoLoad();
        }

        

        public async void FrmMain_DragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 0) return;
                if (files.Length > 1)
                {
                    MessageBox.Show(@"Multi file dropdown not supported");
                    return;
                }
                tmrReadingFile.Enabled = true;
                await DoReload(files[0]);
                tmrReadingFile.Enabled = false;
            }
            
        }

        private void FrmMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";

            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
            }
            if (filename != "")
            {
                await DoReload(filename);
            }
        }

        private async void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderpath = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                folderpath = fbd.SelectedPath;
            }

            if (!string.IsNullOrWhiteSpace(folderpath))
            {
                await DoReload(folderpath);
            }
          
        }
        
        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LargerFont();
        }

        private void decreaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SmallerFont();
        }

        private void SmallerFont()
        {
            Switchboard.Instance.ControlResize.Smaller();
            foreach (Form item in MdiChildren)
            {
                if (item is FrmBaseForm frmBase)
                    frmBase.ResizeAllToCurrentSize();
            }
            Switchboard.Instance.LoadPanelHelper?.RecalcMenuFont();
            Switchboard.Instance.ControlResize.ResetDelta();
        }
        private void LargerFont()
        {
            Switchboard.Instance.ControlResize.Larger();
            Switchboard.Instance.LoadPanelHelper?.RecalcMenuFont();
            foreach (Form item in MdiChildren)
            {
                if (item is FrmBaseForm frmBase)
                    frmBase.ResizeAllToCurrentSize();
            }
            Switchboard.Instance.ControlResize.ResetDelta();
        }

        private void ShowSearchForFiles()
        {
            var search = new FrmSearchForFiles();
            var ret = search.ShowDialog(this);
            if (ret != DialogResult.Cancel)
            {
                var objects = search.SelectedLogFiles;
                Switchboard.Instance.LoadPanelHelper.ActivateSenseForm(objects);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Subtract)) {SmallerFont();}
            if (keyData == (Keys.Control | Keys.Add)){LargerFont();}
            if (keyData == (Keys.Control | Keys.Shift | Keys.F)) { ShowSearchForFiles(); }
            if (keyData == (Keys.Control | Keys.F)) {(ActiveMdiChild as FrmBaseForm)?.ShowFilterIfSupported();}

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void searchForFilesShiftCtrlFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchForFiles();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var frm = new FrmAbout();
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void enablementVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://youtu.be/YcwT77qb4uE");
        }

        private void tmrReadingFile_Tick(object sender, EventArgs e)
        {
            SetInfoMessage(Switchboard.Instance.GetCurrentReloadStatus);
        }

        private void toggleParalellScrollingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Switchboard.Instance.Settings.Documents.ParallelScrollingEnabled = Switchboard.Instance.Settings.Documents.ParallelScrollingEnabled != true;
            Mbox.Show($"Parallel Scolling is now {(Switchboard.Instance.Settings.Documents.ParallelScrollingEnabled ? "Enabled" : "Disabled")}");
        }
    }
}
