using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Eir.Common.Net.Http;

namespace OfflineDataSpooler
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Log(string s)
        {
            txtResults.Text += s + Environment.NewLine;
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
            try
            {
                string zipFile = txtZipPath.Text;
                if (!File.Exists(zipFile))
                {
                    MessageBox.Show(@"Zip file does not exist");
                    return;
                }

                var path = Path.Combine(Path.GetTempPath(), $"ProactiveExpressImport_tmp_{DateTime.Now.ToString("yyyyMMddhhmmss")}");
                Directory.CreateDirectory(path);
                Log($"Unzipping to {path}");
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, path);
                using (var webClient = new EirWebClient())
                {
                    var uri = new Uri("https://proactive.qliktech.com/api/SenseStatistics");//"https://proactive.qliktech.com/api/SenseStatistics"); "http://localhost:8194/api/SenseStatistics"
                    var files = Directory.GetFiles(path);
                    var failedFiles = 0;
                    Log($"{files.Length} files to send.");
                    foreach (var file in files)
                    {
                        try
                        {
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
        }
    }
}
