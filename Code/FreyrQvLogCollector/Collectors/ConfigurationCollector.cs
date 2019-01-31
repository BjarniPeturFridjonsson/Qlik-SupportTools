using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.Common;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using Newtonsoft.Json;

namespace FreyrQvLogCollector.Collectors
{
    public class ConfigurationCollector
    {
        public List<string> ConnectorList { get; private set; }

        private readonly AsyncCmdLineHelper _collectorHelper;
        private readonly IFileSystem _filesystem;
        private readonly ILogger _logger;

        public ConfigurationCollector(IFileSystem filesystem, ILogger logger)
        {
            _filesystem = filesystem;
            _collectorHelper = new AsyncCmdLineHelper(filesystem, logger);
            _logger = logger;
        }


        public async Task<List<CmdLineResult>> RunRemoteCmds(string hostName)
        {
            //($@"\\{hostname}\c$\Program Files\Qlik\Sense\Repository\PostgreSQL").ToList();

            string postgress = await GetPostgressPath(hostName).ConfigureAwait(false);
            ConnectorList = await GetListOfConnctorsInstalled(hostName).ConfigureAwait(false);
            var connectorsConfig = await GetConnectorsConfig(ConnectorList).ConfigureAwait(false);
            var cmds = new List<CmdLineResult>
            {
                new CmdLineResult{Name = "Postgresql.conf",Cmd =$@"{postgress}\postgresql.conf", ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "postgresql_pg_hba.conf",Cmd =$@"{postgress}\pg_hba.conf",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Engine_Settings.ini",Cmd =$@"\\{hostName}\c$\ProgramData\\QlikTech\\Sense\\Engine\\settings.ini",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Sense_Host.cfg",Cmd =$@"\\{hostName}\c$\ProgramData\QlikTech\Sense\host.cfg",ExecType = CmdLineExecType.FileContent},

                new CmdLineResult{Name = "Repository.exe.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Repository\Repository.exe.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Repository.Core.dll.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Repository\Repository.Core.dll.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Repository.Domain.dll.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Repository\Repository.Domain.dll.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Repository.Synchronization.dll.config",Cmd =$@"\{hostName}\c$\\Program Files\QlikTech\Sense\Repository\Repository.Synchronization.dll.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Repository.User.dll.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Repository\Repository.User.dll.config",ExecType = CmdLineExecType.FileContent},

                new CmdLineResult{Name = "Printing.exe.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Printing\Printing.exe.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Qlik.Printing.CefSharp.exe.config",Cmd =$@"\{hostName}\c$\\Program Files\QlikTech\Sense\Printing\Qlik.Printing.CefSharp.exe.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Qlik.Sense.Printing.dll.config",Cmd =$@"\\{hostName}\\c$\\Program Files\QlikTech\Sense\Printing\Qlik.Sense.Printing.dll.config",ExecType = CmdLineExecType.FileContent},

                new CmdLineResult{Name = "Scheduler.exe.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Scheduler\Scheduler.exe.config",ExecType = CmdLineExecType.FileContent},
                new CmdLineResult{Name = "Proxy.exe.config",Cmd =$@"\\{hostName}\c$\Program Files\QlikTech\Sense\Proxy\Proxy.exe.config",ExecType = CmdLineExecType.FileContent},

            };

            cmds.AddRange(connectorsConfig);
            cmds = await _collectorHelper.ExecCmds(cmds).ConfigureAwait(false);

            cmds.Add(new CmdLineResult { Name = "ConnectorsInstalled", Result = JsonConvert.SerializeObject(ConnectorList, Formatting.Indented) });
            return cmds;
        }

        private async Task<List<CmdLineResult>> GetConnectorsConfig(List<string> connectors)
        {
            return await Task.Run(() =>
            {
                var ret = new List<CmdLineResult>();
                connectors.ForEach(p =>
                {

                    var files = _filesystem.DirectoryGetFiles(p).ToList();
                    files.ForEach(file =>
                    {
                        if (string.Equals(_filesystem.Path.GetExtension(file), ".config", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ret.Add(new CmdLineResult { Name = $"{_filesystem.Path.GetFileName(p)}_{_filesystem.Path.GetFileName(file)}", Cmd = file, ExecType = CmdLineExecType.FileContent });
                        }
                    });
                });

                return ret;
            }).ConfigureAwait(false);
        }

        private async Task<List<string>> GetListOfConnctorsInstalled(string hostName)
        {
            return await Task.Run(() =>
            {
                string path = $@"\\{hostName}\c$\Program Files\Common Files\Qlik\Custom Data";
                try
                {
                    return _filesystem.GetDirectories(path).ToList();
                }
                catch (Exception e)
                {
                    _logger.Add($"Failed accessing GetListOfConnctorsInstalled '{path}'", e);
                    return new List<string>();
                }

            }).ConfigureAwait(false);
        }

        private async Task<string> GetPostgressPath(string hostname)
        {
            //run through the programfolders and get the one with the highest version number and hope for the best 
            return await Task.Run(() =>
            {
                string path = "";
                try
                {
                    path = $@"\\{hostname}\c$\ProgramData\Qlik\Sense\Repository\PostgreSQL";
                    var dirs = _filesystem.GetDirectories(path).ToList();
                    string dir = dirs.OrderByDescending(p => p).FirstOrDefault();
                    return dir + "";
                }
                catch (Exception e)
                {
                    _logger.Add($"Failed accessing GetPostgressPath '{path}'", e);
                    return "";
                }

            }).ConfigureAwait(false);
        }


    }
}
