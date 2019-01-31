using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.Common;
using FreyrCommon.Logging;
using FreyrCommon.Models;

namespace FreyrCollectorCommon.Collectors
{
    public class CmdLineAgents
    {
        private readonly AsyncCmdLineHelper _collectorHelper;

        public CmdLineAgents(IFileSystem filesystem, ILogger logger)
        {
            _collectorHelper = new AsyncCmdLineHelper(filesystem, logger);
        }

   

        public async Task<List<CmdLineResult>> RunRemoteCmds(string hostName, TimeSpan timeout)
        {
            var cmds = new List<CmdLineResult>
            {
                new CmdLineResult
                {
                    Name = "System Information",
                    Cmd = $"C:\\Windows\\System32\\systeminfo.exe /S {hostName} /FO CSV"
                }
            };

            cmds = await _collectorHelper.ExecCmds(cmds, timeout).ConfigureAwait(false);
            return cmds;
        }

        public async Task<List<CmdLineResult>> RunLocalCmds()
        {
            AppDomain myDomain = Thread.GetDomain();
            myDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            //removed due to returning too many strange false positives (bfr 2017-10-01)
            //WindowsPrincipal myPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            //var a = myPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            //if (a == false)
            //    throw new Exception("this program needs to run as Administrator");

            var cmds = new List<CmdLineResult>();
            cmds.Add(new CmdLineResult { Name = "WhoAmI", Cmd = "C:\\Windows\\System32\\whoami.exe" });
            cmds.Add(new CmdLineResult { Name = "Port status", Cmd = "C:\\Windows\\System32\\netstat.exe -anob" });
            cmds.Add(new CmdLineResult { Name = "Process Info", Cmd = "C:\\Windows\\System32\\tasklist.exe /v" });
            cmds.Add(new CmdLineResult { Name = "Firewall Info", Cmd = "C:\\Windows\\System32\\netsh.exe advfirewall show allprofiles" });
            cmds.Add(new CmdLineResult { Name = "Network Info", Cmd = "C:\\Windows\\System32\\ipconfig.exe /all" });
            cmds.Add(new CmdLineResult { Name = "IIS Status", Cmd = "C:\\Windows\\System32\\iisreset.exe  /status" });
            cmds.Add(new CmdLineResult { Name = "Proxy Activated", Cmd = "C:\\Windows\\System32\\reg.exe query \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" | find /i \"ProxyEnable\"" });
            cmds.Add(new CmdLineResult { Name = "Proxy Server", Cmd = "C:\\Windows\\System32\\reg.exe query \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" | find /i \"proxyserver\" " });
            cmds.Add(new CmdLineResult { Name = "Proxy AutoConfig", Cmd = "C:\\Windows\\System32\\reg.exe query \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" | find /i \"AutoConfigURL\" " });
            cmds.Add(new CmdLineResult { Name = "Proxy Override", Cmd = "C:\\Windows\\System32\\reg.exe query \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\" | find /i \"ProxyOverride\" " });
            cmds.Add(new CmdLineResult { Name = "Internet Connection", Cmd = "C:\\Windows\\System32\\ping.exe google.com" });
            cmds.Add(new CmdLineResult { Name = "Drive Mappings", Cmd = "C:\\Windows\\System32\\net.exe use" });
            cmds.Add(new CmdLineResult { Name = "Drive Info", Cmd = "C:\\Windows\\System32\\wbem\\wmic.exe /OUTPUT:STDOUT logicaldisk get size,freespace,caption" });
            cmds.Add(new CmdLineResult { Name = "Localgroup Administrators", Cmd = "C:\\Windows\\System32\\net.exe localgroup \"Administrators\"" });
            cmds.Add(new CmdLineResult { Name = "Localgroup Sense Service Users", Cmd = "C:\\Windows\\System32\\net.exe localgroup \"Qlik Sense Service Users\"" });
            cmds.Add(new CmdLineResult { Name = "Localgroup Performance Monitor Users", Cmd = "C:\\Windows\\System32\\net.exe localgroup \"Performance Monitor users\"" });
            cmds.Add(new CmdLineResult { Name = "Localgroup Qv Administrators", Cmd = "C:\\Windows\\System32\\net.exe localgroup \"QlikView Administrators\"" });
            cmds.Add(new CmdLineResult { Name = "Localgroup Qv Api", Cmd = "C:\\Windows\\System32\\net.exe localgroup \"QlikView Management API\"" });
            cmds.Add(new CmdLineResult { Name = "System Information", Cmd = "C:\\Windows\\System32\\systeminfo.exe" });
            cmds.Add(new CmdLineResult { Name = "Program List", Cmd = "C:\\Windows\\System32\\wbem\\wmic.exe /OUTPUT:STDOUT product get name,version,vendor" });
            cmds.Add(new CmdLineResult { Name = "Service List", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"gwmi win32_service | select Started, name, startname\"" });
            cmds.Add(new CmdLineResult { Name = "Group Policy", Cmd = "C:\\Windows\\System32\\gpresult.exe /z" });
            cmds.Add(new CmdLineResult { Name = "Local Policies - User Rights Assignment", Cmd = "C:\\Windows\\System32\\secedit.exe /export /areas USER_RIGHTS /cfg" });
            cmds.Add(new CmdLineResult { Name = "Local Policies - Security Options", Cmd = "C:\\Windows\\System32\\secedit.exe /export /areas" });
            cmds.Add(new CmdLineResult { Name = "Hosts file", Cmd = "C:\\Windows\\System32\\drivers\\etc\\hosts", ExecType = CmdLineExecType.FileContent });
            cmds.Add(new CmdLineResult { Name = "Lef file QlikTech", Cmd = "C:\\ProgramData\\QlikTech\\lef.txt", ExecType = CmdLineExecType.FileContent });
            cmds.Add(new CmdLineResult { Name = "Lef file QlikView", Cmd = "C:\\ProgramData\\QlikTech\\QlikView\\lef.txt", ExecType = CmdLineExecType.FileContent });
            cmds.Add(new CmdLineResult { Name = "Certificate - Current User(Personal)", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"Get-ChildItem -Recurse Cert:\\currentuser\\my | Format-list\"" });
            cmds.Add(new CmdLineResult { Name = "Certificate - Current User(Trusted Root)", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"Get-ChildItem -Recurse Cert:\\currentuser\\Root | Format-list\"" });
            cmds.Add(new CmdLineResult { Name = "Certificate - Local Computer(Personal)", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"Get-ChildItem -Recurse Cert:\\localmachine\\my | Format-list\"" });
            cmds.Add(new CmdLineResult { Name = "Certificate - Local Computer(Trusted Root)", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"Get-ChildItem -Recurse Cert:\\localmachine\\Root | Format-list\"" });
            cmds.Add(new CmdLineResult { Name = "HotFixes", Cmd = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -command \"$FormatEnumerationLimit=-1; $Session = New-Object -ComObject \\\"Microsoft.Update.Session\\\";$Searcher = $Session.CreateUpdateSearcher();$historyCount = $Searcher.GetTotalHistoryCount();$Searcher.QueryHistory(0, $historyCount) | Select-Object Title, Description, Date, @{name=\\\"Operation\\\"; expression={switch($_.operation){1 {\\\"Installation\\\"}; 2 {\\\"Uninstallation\\\"}; 3 {\\\"Other\\\"}}}} | out-string -Width 1024\"" });
            cmds.Add(new CmdLineResult { Name = "UrlAclList", Cmd = "C:\\Windows\\System32\\netsh.exe http show urlacl" });
            cmds.Add(new CmdLineResult { Name = "PortCertList", Cmd = "C:\\Windows\\System32\\netsh.exe http show sslcert" });

            //cmds.Add(new CmdLineResult { Name = "SystemNfoExecResults", Cmd = $"C:\\Windows\\System32\\msinfo32.exe /nfo {OutputFolderPath}\\msinfo32.nfo" });

            cmds = await _collectorHelper.ExecCmds(cmds).ConfigureAwait(false);
            return cmds;
        }
   }
}
