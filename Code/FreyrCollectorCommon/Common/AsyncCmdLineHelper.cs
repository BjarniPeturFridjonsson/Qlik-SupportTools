using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCommon.Logging;
using FreyrCommon.Models;

namespace FreyrCollectorCommon.Common
{
    public class AsyncCmdLineHelper
    {
        private readonly IFileSystem _filesystem;
        private readonly ILogger _logger;

        public AsyncCmdLineHelper(IFileSystem filesystem, ILogger logger)
        {
            _filesystem = filesystem;
            _logger = logger;
        }

        public async Task<List<CmdLineResult>> ExecCmds(List<CmdLineResult> cmds,TimeSpan timeout = new TimeSpan())
        {
            var sw = new Stopwatch();
            sw.Start();
            cmds.ForEach(async p => await RunProcessAsync(p).ConfigureAwait(false));
            bool allFinished = false;
            while (!allFinished)
            {
                await Task.Delay(200).ConfigureAwait(false);
                allFinished = true;
                foreach (var itm in cmds)
                {
                    if (itm.RunComplete == false)
                    {
                        allFinished = false;
                        break;
                    }
                }

                if (timeout.TotalSeconds > 0 &&  sw.Elapsed.TotalSeconds > timeout.TotalSeconds)
                {
                    //todo: This leaves the process still running. Rewrite the async cmd line to be able to cancel. 
                    break;
                }
            }

            return cmds;
        }

        private async Task<int> RunProcessAsync(CmdLineResult cmdLine)
        {
            if (cmdLine.ExecType == CmdLineExecType.FileContent)
            {
                if (_filesystem.FileExists(cmdLine.Cmd))
                {
                    try
                    {
                        cmdLine.Result = await _filesystem.ReadFileContentAsync(cmdLine.Cmd).ConfigureAwait(false);
                        cmdLine.RunComplete = true;
                        cmdLine.CmdExitCode = 0;
                        return cmdLine.CmdExitCode;
                    }
                    catch (Exception e)
                    {
                        cmdLine.Error = e.Message;
                        cmdLine.CmdExitCode = e.HResult;
                        cmdLine.RunComplete = true;
                        return cmdLine.CmdExitCode;
                    }
                }
                cmdLine.Error = "Couldn't find the file.";
                cmdLine.RunComplete = true;
                cmdLine.CmdExitCode = 666;
                return cmdLine.CmdExitCode;
            }

            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = "C:\\Windows\\System32\\cmd.exe",
                    Arguments = "/c " + cmdLine.Cmd,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage),
                    StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage),
                },
                EnableRaisingEvents = true
            })
            {
                return await RunProcessAsync(process, cmdLine).ConfigureAwait(false);
            }
        }

        private Task<int> RunProcessAsync(Process process, CmdLineResult cmdLine)
        {
            //Notify("started " + cmdLine.Name);
            var tcs = new TaskCompletionSource<int>();
            var output = new StringBuilder();
            var error = new StringBuilder();
            
            try
            {
                process.Exited += (s, ea) =>
                {
                    cmdLine.Result = output.ToString();
                    cmdLine.Error = error.ToString();
                    cmdLine.RunComplete = true;
                    cmdLine.CmdExitCode = process.ExitCode;
                    tcs.SetResult(process.ExitCode);
                };
                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        output.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        error.AppendLine(e.Data);
                };

                bool started = process.Start();
                if (!started)
                {
                    //you may allow for the process to be re-used (started = false) 
                    //but I'm not sure about the guarantees of the Exited event in such a case
                    _logger.Add($"cmd line cmd failed starting");
                    throw new InvalidOperationException("Could not start process: " + process);
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                return tcs.Task;
            }
            catch (Exception e)
            {
                _logger.Add($"cmd line ex : {e}");
                tcs.SetResult(-1);
                return tcs.Task;
            }
        }
    }
}
