using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;
using Gjallarhorn.Monitors.FileMonitor;
using Gjallarhorn.Notifiers;

namespace Gjallarhorn.Monitors
{
    public class FilesMonitor : BaseMonitor, IGjallarhornMonitor
    {
        private List<string> _msgs;
        private int _maxNrOfErrMsgs;

        public FilesMonitor(Func<string,IEnumerable<INotifyerDaemon>> notifyerDaemons):base(notifyerDaemons, "FilesMonitor")
        {
        }

        public void Execute()
        {
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Failed executing monitoring files", ex);
            }
        }

        private void DoWork()
        {
            var rulenames = Settings.GetSetting($"{MonitorName}.RuleNames").Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var rules = new List<FileMonitorDto>();

            rulenames.ForEach(rulename =>
            {
                rules.Add(new FileMonitorDto
                {
                    FilePath = Settings.GetSetting($"{MonitorName}.{rulename}.FilePath").Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                    FileOverdueInHours = Settings.GetInt32($"{MonitorName}.{rulename}.FileOverdueInHours", 1),
                    Filter = Settings.GetSetting($"{MonitorName}.{rulename}.Filter").Trim(),
                    NegativeFilter = Settings.GetSetting($"{MonitorName}.{rulename}.NegativeFilter").Trim(),
                    ScanSubdirectories = Settings.GetBool($"{MonitorName}.{rulename}.ScanSubdirectories", true),
                });
            });
           

            _maxNrOfErrMsgs = Settings.GetInt32($"{MonitorName}.MaxNrOfErrorMessages", 30);

            _msgs = new List<string>();
            rules.ForEach(dto =>
            {
                foreach (var path in dto.FilePath)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        continue;
                    if (!Directory.Exists(path))
                    {
                        _msgs.Add($"FileMonitoring => Directory not found or accessible {path}");
                    }
                    else
                    {
                        CheckFolders(path, dto);
                    }
                }
            });
            Notify("FileMonitoring has found the following issues", _msgs);

        }

        private void CheckFolders(string path, FileMonitorDto dto)
        {
            
            var pathTypeDir = path.IsPathDirOrFile();
            if (pathTypeDir == null)
            {
                _msgs.Add($"FileMonitoring => Directory or file not found or accessible {path}");
                return;
            }
            if (!pathTypeDir.Value)
            {
                CheckFiles(new[] {path}, dto);
                return;
            }

            if(dto.ScanSubdirectories)
                CheckFolder(path, dto);

        }

        private void CheckFolder(string path, FileMonitorDto dto)
        {
            CheckFiles(Directory.GetFiles(path), dto);
            var dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                CheckFolders(dir, dto);
            }
        }

        private void CheckFiles(string[] files, FileMonitorDto dto)
        {
            var regex = new Regex(dto.Filter,RegexOptions.IgnoreCase);
            var negRegex = new Regex(dto.NegativeFilter, RegexOptions.IgnoreCase);

            foreach (var file in files)
            {
                if (_msgs.Count >= _maxNrOfErrMsgs)
                    return;
                var fileName = Path.GetFileName(file) + "";
                if (regex.IsMatch(fileName))
                {
                    if(!negRegex.IsMatch(file))
                        _msgs.AddRange(ValidateFileAge(file, TimeSpan.FromHours(dto.FileOverdueInHours)));
                }
            }
        }


        private List<string> ValidateFileAge(string dashPath, TimeSpan maxAge)
        {
            if (!File.Exists(dashPath))
                return new List<string> { $"{dashPath ?? "(empty path)"} not found" };

            var date = File.GetLastWriteTimeUtc(dashPath);
            if (date < (DateTimeProvider.Singleton.Time() - maxAge))
                return new List<string> {$"{dashPath} too old. Last write done on {date.ToString("yyyy-MM-dd hh:mm:ss")}"};

            return new List<string>();
        }

    }
}
