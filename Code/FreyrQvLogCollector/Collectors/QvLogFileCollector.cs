using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.IO;
using FreyrCollectorCommon.CollectorCore;
using FreyrCollectorCommon.LogFileReader;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;
using FreyrQvLogCollector.QvCollector;

namespace FreyrQvLogCollector.Collectors
{
    public class QvLogFileDirector
    {
        private readonly ILogger _logger;
        private CommonCollectorServiceVariables _serviceVariables;
        private readonly Action<string, MessageLevels, string> _notify;
        private readonly IFileSystem _fileSystem;
        private long _localFileCounter;
        private long _localDirCounter;
        private string _notificationKey = "LogFileDirector";
        private FolderNotificationHelper _folderNotificationHelper;


        public long FoundFileCount { get; private set; }

        public QvLogFileDirector(ILogger logger, IFileSystem filesystem, Action<string, MessageLevels, string> notify)
        {
            _logger = logger;
            _fileSystem = filesystem;
             _notify = notify;
        }

        public void Execute(CommonCollectorServiceVariables serviceVariables)
        {
            _serviceVariables = serviceVariables;
            //_serviceVariables.QvSettings.QvLogLocations = new List<QvLogLocation>();
            //_serviceVariables.QvSettings.QvLogLocations.Add(new QvLogLocation
            //{
            //    Name = $"CommonFiles - QlikTech - su-pubdev2",    
            //    Type = QvLogLocationSource.QdsClusterInfoUrl,
            //    Path = $"\\\\su-pubdev2\\c$\\ProgramData\\QlikTech",
            //    LogCollectionType = QvLogCollectionType.SettingsOnly
            //});
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            _folderNotificationHelper = new FolderNotificationHelper(_logger,_notify, _notificationKey, _serviceVariables);

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
                    _folderNotificationHelper.Analyze(_localFileCounter,_localDirCounter);
                    if (cancellationToken.IsCancellationRequested)
                        break;   
                }
            }, cancellationToken);

            _serviceVariables.QvSettings.QvLogLocations.ForEach(p =>
            {
                _folderNotificationHelper.SetCurrentFolderNames(p.Name,null);
                CrawlAllLogBaseDirectories(new DirectorySetting(p.Path), p.Name, p.IgnorePaths);
                p.AddToFileCount(_localFileCounter);
                Interlocked.Exchange(ref _localFileCounter,0);
                Interlocked.Exchange(ref _localDirCounter, 0);
            });
            cts.Cancel();
            _notify($"{FoundFileCount} logs found on {_serviceVariables.QvSettings.QvLogLocations.Count} locations.", MessageLevels.Ok, _notificationKey);
        }

        private void WriteFile(IFileInfo file, string outputFilePath)
        {
            try
            {
                if(file == null && outputFilePath == null) //hack, there are so ridiculous amount of directories we need to show customers that we are working.
                {
                    Interlocked.Add(ref _localDirCounter, +1);
                    return;
                }
                //Trace.WriteLine($"{outputFilePath} => {file?.Name}");
                Interlocked.Add(ref _localFileCounter, 1);
                _fileSystem.EnsureDirectory(outputFilePath);
                string outputFileFullPath = _fileSystem.Path.Combine(outputFilePath, file?.Name);
                if (_fileSystem.FileExists(outputFileFullPath))
                    outputFileFullPath += "_" + _fileSystem.Path.GetFileNameWithoutExtension(_fileSystem.Path.GetTempFileName());
                _fileSystem.FileCopy(file?.FullName, outputFileFullPath);
                FoundFileCount++;
            }
            catch (Exception ex)
            {
                _logger.Add("Failed writing file->ignored",ex);
            }
        }

        private void CrawlAllLogBaseDirectories(DirectorySetting baseDir, string directoryName, Func<string, bool> ignorePaths)
        {
            Trace.WriteLine(baseDir.Path);
            if (baseDir.Exists)
            {
                Crawler(directoryName, ignorePaths, baseDir.Path);
            }
        }

        private void Crawler(string directoryName, Func<string, bool> ignorePaths, string dir)
        {
            var finder = new LogFileFinder(WriteFile, _fileSystem, _logger, ignorePaths);
            string localPath = _fileSystem.Path.Combine(_serviceVariables.OutputFolderPath, directoryName, _fileSystem.Path.GetFileName(dir) + "");
            _fileSystem.EnsureDirectory(localPath);
            _folderNotificationHelper.SetCurrentFolderNames(null, dir);
            finder.FindFilesInDirectories(new DirectorySetting(dir), _serviceVariables.StartDateForLogs, _serviceVariables.StopDateForLogs, localPath);
            Debug.WriteLine("found=>" + dir);
        }

    }
}
