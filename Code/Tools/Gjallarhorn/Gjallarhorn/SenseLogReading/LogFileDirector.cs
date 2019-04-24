using Eir.Common.IO;
using Gjallarhorn.SenseLogReading.FileMiners;
using System.Diagnostics;

namespace Gjallarhorn.SenseLogReading
{
    public class LogFileDirector
    {
        private LogFileDirectorSettings _settings;
        private readonly IFileSystem _fileSystem;
        private long _localFileCounter;
        private long _localDirCounter;

        private BasicDataFromFileMiner _fileMinerData;

        public LogFileDirector(IFileSystem fileSystem) { _fileSystem = fileSystem;}

        public void LoadAndRead(DirectorySetting[] directories, LogFileDirectorSettings settings, BasicDataFromFileMiner fileMinerData)
        {
            _settings = settings;
            _fileMinerData = fileMinerData;
            
            var timer = Stopwatch.StartNew();
            foreach (DirectorySetting directory in directories)
            {
                CrawlAllLogBaseDirectories(directory);
            }

            foreach (var dataMiner in ActiveFileMiners.Get)
            {
                dataMiner.FinaliseStatistics();
            }
            timer.Stop();

            FinalizeStatistics(fileMinerData, timer);
        }

        private void FinalizeStatistics(BasicDataFromFileMiner data, Stopwatch stopwatch)
        {
            if (data == null) return;
            data.TotalUniqueActiveUsers = data.TotalUniqueActiveUsersList?.Count ?? -1;
            data.TotalUniqueActiveApps = data.TotalUniqueActiveAppsList?.Count ?? -1;
            _fileMinerData.TotalNrOfFiles = _localFileCounter;
            _fileMinerData.TotalNrOfDirectories = _localDirCounter;
            _fileMinerData.TotalScanTimeTakenSec = stopwatch.Elapsed.Seconds;
        }

        private DirectorySetting CrawlAllLogBaseDirectories(DirectorySetting directory)
        {
            if (directory != null && directory.Exists)
            {
                if (IsBaseLogDirecory(directory))
                {
                    foreach (var dataMiner in ActiveFileMiners.Get)
                    {
                        var mineLocation = dataMiner.MineFromThisLocation(directory.Path, _fileSystem);
                        if (!string.IsNullOrEmpty(mineLocation))
                        {
                            ScanLogs(mineLocation, dataMiner);
                        }
                    }
                }

                var i = 0;
                //FIX:on unstable networks this can return null 
                var dirs = _fileSystem.GetDirectories(directory.Path);
                while (dirs == null)
                {
                    dirs = _fileSystem.GetDirectories(directory.Path);
                    i++;
                    if (i > 10)
                        break;
                }
                foreach (string dir in dirs)
                {
                    var baseDir = CrawlAllLogBaseDirectories(new DirectorySetting(dir));
                    if (baseDir != null)
                        return baseDir;
                }
            }
            return null;
        }

        private void ScanLogs(string mineLocation, IDataMiner dataMiner)
        {
            DirectoryInfo info = new DirectoryInfo();
            Trace.WriteLine($"Log location found => {mineLocation}");
            _localDirCounter++;
            var files = info.EnumerateLogFiles(mineLocation, _settings.StartDateForLogs, _settings.StopDateForLogs);
            foreach (IFileInfo file in files)
            {
                _localFileCounter++;
                file.Refresh(); //some files are returning empty even though they are not. Shown 0 bytes in explorer until opened in notepad. 
                Trace.WriteLine("found=>" + file.FullName);
                var lines = _fileSystem.ReadLines(file.FullName);

                var lineCounter = 0;
                foreach (var line in lines)
                {
                    lineCounter++;
                    if (lineCounter == 1)
                    {
                        dataMiner.InitializeNewFile(line, _fileMinerData, mineLocation);
                        continue;
                    }

                    dataMiner.Mine(line);
                }
            }
        }

        private bool IsBaseLogDirecory(DirectorySetting directory)
        {
            return directory.ChildExists("Repository\\Trace");
        }
    }
}
