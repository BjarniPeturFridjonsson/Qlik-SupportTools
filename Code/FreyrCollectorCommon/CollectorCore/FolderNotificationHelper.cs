using System;
using System.Collections.Generic;
using FreyrCollectorCommon.Winform;
using FreyrCommon.Logging;
using FreyrCommon.Models;

namespace FreyrCollectorCommon.CollectorCore
{
    public class FolderNotificationHelper
    {
        private readonly Action<string, MessageLevels, string> _notify;
        private readonly ILogger _logger;
        private long _currentDirId = -1;
        private long _secCounter;
        private long _longFolder;

        private string _msg = "Starting Collecting Logs";
        private readonly string _notificationKey;
        private readonly object _lock = new object();
        private string _currentLogFolder;
        private readonly List<string> _badFolders = new List<string>();
        private readonly CommonCollectorServiceVariables _serviceVariables;

        public FolderNotificationHelper(ILogger logger, Action<string, MessageLevels, string> notify, string notificationKey, CommonCollectorServiceVariables serviceVariables)
        {
            _logger = logger;
            _notify = notify;
            _notificationKey = notificationKey;
            _serviceVariables = serviceVariables;
            _notify(_msg, MessageLevels.Animate, _notificationKey);
        }


        public void SetCurrentFolderNames(string machineName, string currentLogFolder)
        {
            try
            {
                lock (_lock)
                {
                    if (machineName != null)
                        _msg = $"Collecting Logs from {machineName}";
                    if (currentLogFolder != null)
                        _currentLogFolder = currentLogFolder;
                }

                _notify(_msg, MessageLevels.Animate, _notificationKey);
            }
            catch (Exception e)
            {
                _logger.Add("Failed in Folder Notification helper SetCurrentFolderNames", e);
            }
        }

        public void Analyze(long localFileCounter, long localDirCounter )
        {
            try
            {
                if (_secCounter > 60)
                {
                    if (!_badFolders.Contains(_currentLogFolder))
                    {
                        _badFolders.Add(_currentLogFolder);
                        _longFolder++;
                        _serviceVariables.Issues.Add(new IssueRegister{Name = $"It can be an issue if you have too many files in a folder {_currentLogFolder}"});
                        _notify($"There might be {_longFolder} folders that need cleaning up.", MessageLevels.Warning, "TooManyFilesInFolderWarning.");
                    }
                }

                _notify($"{_msg} ({localFileCounter}F-{localDirCounter}D)", MessageLevels.Animate, _notificationKey);

                if (localDirCounter == _currentDirId)
                {
                    _secCounter += 3;
                }
                else
                {
                    _secCounter = 0;
                    _currentDirId = localDirCounter;
                }
            }
            catch (Exception e)
            {
               _logger.Add("Failed in Folder Notification helper Analyze",e);
            }
          
           
        }
    }
}
