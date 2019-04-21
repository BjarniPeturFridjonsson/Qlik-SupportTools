using System.Collections.Generic;
using Eir.Common.IO;
using Eir.Common.Logging;
using FreyrCommon.Models;
using Gjallarhorn.Common;
using System.Diagnostics;
using System.Threading;
using Gjallarhorn.SenseLogReading.FileMiners;

namespace Gjallarhorn.SenseLogReading
{
    public class StreamLogDirector
    {
        private readonly SenseLogFolderFinder _folderFinder = new SenseLogFolderFinder();
        private StreamLogDirectorSettings _settings;
        private readonly IFileSystem _fileSystem = FileSystem.Singleton; //todo: inject filesystem.
        private long _localFileCounter;
        private long _localDirCounter;

        private readonly List<IDataMiner> _dataMiners = new List<IDataMiner>
        {
            new AuditActivityRepositoryMiner(),
            new AuditActivityProxyMiner()
        };

        public string FriendlyName { get; set; }
        public string NotificationKey { get; set; }
        public int FoundFileCount { get; set; }

        public void LoadAndRead(DirectorySetting[] directories, StreamLogDirectorSettings settings)
        {
            _settings = settings;

            foreach (DirectorySetting directory in directories)
            {
                CrawlAllLogBaseDirectories(directory);
            }
         
        }

        private void WriteFile(IFileInfo file, string outputFilePath)
        {
            if (file == null && outputFilePath == null) //hack, there are so ridiculous amount of directories we need to show customers that we are working.
            {
                Interlocked.Add(ref _localDirCounter, +1);
                return;
            }
            //Trace.WriteLine($"{outputFilePath} => {file?.Name}");
            Interlocked.Add(ref _localFileCounter, 1);

            Trace.WriteLine($"{outputFilePath} => {file?.Name ?? "<Undefined>"}");
            if (file == null)
            {
                Log.To.Main.Add("StreamLogDirector. File is null, this is major error in writing file");
                return;
            }

            _fileSystem.EnsureDirectory(outputFilePath);
            string outputFileFullPath = _fileSystem.Path.Combine(outputFilePath, file.Name);
            if (_fileSystem.FileExists(outputFileFullPath))
                outputFileFullPath += "_" + _fileSystem.Path.GetFileNameWithoutExtension(_fileSystem.Path.GetTempFileName());
            _fileSystem.FileCopy(file.FullName, outputFileFullPath);
            FoundFileCount++;
        }

        private DirectorySetting CrawlAllLogBaseDirectories(DirectorySetting directory)
        {
            //Trace.WriteLine(directory.Path);

            if (directory != null && directory.Exists)
            {
                if (IsBaseLogDirecory(directory))
                {
                    foreach (var dataMiner in _dataMiners)
                    {
                        var mineLocation = dataMiner.MineFromThisLocation(directory.Path, _fileSystem);
                        if (!string.IsNullOrEmpty(mineLocation))
                        {
                            DirectoryInfo info = new DirectoryInfo();
                            Trace.WriteLine($"start => {directory}");
                            var files = info.EnumerateLogFiles(directory.Path, _settings.StartDateForLogs, _settings.StopDateForLogs);
                            foreach (IFileInfo file in files)
                            {
                                file.Refresh(); //some files are returning empty even though they are not. Shown 0 bytes in explorer until opened in notepad. 
                                //dataMiner.InitializeNewFile();
                            }
                        }
                    }


                    //foreach (string dir in directory.GetDirectories())
                    //{
                    //    ////todo:The _ignored files has dual purpose, and if there is folder called all it will be ignored.
                    //    //if (_ignoredFiles.HasFlag(_folderFinder.GetSenseLogBaseTypes(new DirectorySetting(dir))))
                    //    //{//_ignoredFiles != SenseLogBaseTypes.All &&

                    //    //    Trace.WriteLine("Ignoring directory=>" + dir);
                    //    //    continue;
                    //    //}
                    //    //var finder = new LogFileFinder(WriteFile, _fileSystem, null);
                    //    //string localPath = _fileSystem.Path.Combine(_settings.OutputFolderPath, (_fileSystem.Path.GetFileName(directory.Path)).SanitizeFileName(), _fileSystem.Path.GetFileName(dir) + "");
                    //    ////_fileSystem.EnsureDirectory(localPath);
                    //    //finder.FindFilesInDirectories(new DirectorySetting(dir), _settings.StartDateForLogs, _settings.StopDateForLogs, localPath);
                    //    Trace.WriteLine("found=>" + dir);
                    //}
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

        private bool IsBaseLogDirecory(DirectorySetting directory)
        {
            return directory.ChildExists("Repository\\Trace");
        }
    }
}
