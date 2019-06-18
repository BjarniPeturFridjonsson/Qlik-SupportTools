using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Eir.Common.Net.Http;

namespace OfflineDataSpooler
{
    public partial class Main : Form
    {
        private int _logLinesCounter;
        private List<string> _waitingLogLines;
        public Main()
        {
            InitializeComponent();
        }

        private void Log(string s)
        {
            _logLinesCounter++;

            if (_logLinesCounter > 1000)
            {
                if(_waitingLogLines == null) _waitingLogLines = new List<string>();
                _waitingLogLines.Add(s);
                if (_waitingLogLines.Count > 100)
                {
                    PurgeLog();
                }
                return;
            }
            txtResults.Text += s + Environment.NewLine;

        }

        private void PurgeLog()
        {
            if (_waitingLogLines == null || !_waitingLogLines.Any()) return;
            txtResults.Text += string.Join(Environment.NewLine,_waitingLogLines);
            _waitingLogLines = new List<string>();
        }

        private void cmdGetZip_Click(object sender, EventArgs e)
        {
            
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                txtZipPath.Text = openFileDialog1.FileName;
            }
        }

        private async void cmdExport_Click(object sender, EventArgs e)
        {
            string zipFile = txtZipPath.Text;
            string path ="";
            try
            {
                cmdClose.Enabled = false;
                cmdExport.Enabled = false;
                cmdGetZip.Enabled = false;
                txtResults.Text = "";
                _waitingLogLines = new List<string>();
                
                if (!File.Exists(zipFile))
                {
                    MessageBox.Show(@"Zip file does not exist");
                    return;
                }

                path = Path.Combine(Path.GetTempPath(),$"ProactiveExpressImport_tmp_{DateTime.Now.ToString("yyyyMMddhhmmss")}");
                Directory.CreateDirectory(path);
                Log($"Unzipping to {path}");
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, path);
                using (var webClient = new EirWebClient())
                {
                    var uri = new Uri("https://proactive.qliktech.com/api/SenseStatistics"); //"https://proactive.qliktech.com/api/SenseStatistics"); "http://localhost:8194/api/SenseStatistics"
                    var files = Directory.GetFiles(path);
                    var failedFiles = 0;
                    Log($"{files.Length} files to send.");
                    var i = 0;
                    foreach (var file in files)
                    {
                        try
                        {
                            i++;

                            if (i % 10 == 0)
                            {
                                lblInfo.Text = $@"Reading file {i} of {files.Length}";
                                Application.DoEvents();
                            }
                            var data = File.ReadAllText(file);
                            try
                            {
                                await webClient.UploadStringAsync(uri, HttpMethod.Post, data, CancellationToken.None);
                            }
                            catch (Exception ex)
                            {
                                Log($"Failed sending file, will retry {file}");
                                Log(ex.ToString());
                                try
                                {
                                    await webClient.UploadStringAsync(uri, HttpMethod.Post, data, CancellationToken.None);
                                }
                                catch (Exception ex2)
                                {
                                    Log($"Totally failed sending file {file}");
                                    Log(ex2.ToString());
                                    failedFiles++;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Log($"Failed reading file");
                            Log(exception.ToString());
                            continue;
                        }

                        Log($"Sent {file}");
                    }

                    Log($"Finished with {failedFiles} failed files.");
                }

                //Directory.Delete(path);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                cmdClose.Enabled = true;
                cmdExport.Enabled = true;
                cmdGetZip.Enabled = true;
                PurgeLog();
                lblInfo.Text = $@"{lblInfo.Text}{Environment.NewLine}ZipFile:{zipFile}{Environment.NewLine}TmpPath:{path}";
                txtZipPath.Text = "";
                MessageBox.Show(@"Done");
            }
        }
    }
}
