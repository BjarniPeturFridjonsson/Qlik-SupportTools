using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.CollectorCore;
using FreyrCollectorCommon.LogFileReader;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;


namespace FreyrSenseCollector.SenseLogReading
{
    public class StreamLogDirector
    {
        private readonly SenseLogFolderFinder _folderFinder = new SenseLogFolderFinder();
        private SenseLogBaseTypes _ignoredFiles = SenseLogBaseTypes.All;
        private CommonCollectorServiceVariables _settings;
        private readonly IFileSystem _fileSystem = FileSystem.Singleton; //todo: inject filesystem.
        private readonly ILogger _logger;
        private long _localFileCounter;
        private long _localDirCounter;
        private FolderNotificationHelper _folderNotificationHelper;

        private readonly Action<string, MessageLevels, string> _notify;

        public string FriendlyName { get; set; }
        public string NotificationKey { get; set; }
        public int FoundFileCount { get; set; }
        
        public StreamLogDirector(ILogger logger, Action<string, MessageLevels, string> notify)
        {
            _logger = logger;
            _notify = notify;
            
        }

        public void LoadAndRead(DirectorySetting[] directories,CommonCollectorServiceVariables settings)
        {
            _settings = settings;
            
            if (!settings.GetLogsScripting)
                _ignoredFiles = SenseLogBaseTypes.Script;

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            _folderNotificationHelper = new FolderNotificationHelper(_logger, _notify, NotificationKey, _settings);

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                    _folderNotificationHelper.Analyze(_localFileCounter, _localDirCounter);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }, cancellationToken);


            foreach (DirectorySetting directory in directories)
            {
                CrawlAllLogBaseDirectories(directory);             
            }
            cts.Cancel();
            //_onLogDirectorFinishedReading?.Invoke(this);
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
                _logger.Add("StreamLogDirector. File is null, this is major error in writing file");
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
                if(IsBaseLogDirecory(directory))
                {
                    foreach (string dir in directory.GetDirectories())
                    {
                        //todo:The _ignored files has dual purpose, and if there is folder called all it will be ignored.
                        if ( _ignoredFiles.HasFlag(_folderFinder.GetSenseLogBaseTypes(new DirectorySetting(dir))))
                        {//_ignoredFiles != SenseLogBaseTypes.All &&
                            
                            Trace.WriteLine("Ignoring directory=>" + dir);
                            continue;
                        }
                        var finder = new LogFileFinder(WriteFile, _fileSystem, _logger, null);
                        string localPath = _fileSystem.Path.Combine(_settings.OutputFolderPath, (_fileSystem.Path.GetFileName(directory.Path)).SanitizeFileName(), _fileSystem.Path.GetFileName(dir) + "");
                        //_fileSystem.EnsureDirectory(localPath);
                        finder.FindFilesInDirectories(new DirectorySetting(dir), _settings.StartDateForLogs, _settings.StopDateForLogs, localPath);
                        Trace.WriteLine("found=>" + dir);
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


        private bool IsBaseLogDirecory(DirectorySetting directory)
        {
            return directory.ChildExists("Repository\\System");
        }     
    }
}
