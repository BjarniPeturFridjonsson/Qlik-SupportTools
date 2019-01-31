using System;
using System.IO;
using System.Text;
using Eir.Common.IO;

namespace Eir.Common.Logging
{
    public class FileWriterLogItemHandler<TLogItem> : ILogItemHandler<TLogItem>, IDisposable
        where TLogItem : LogItem
    {
        private const int DEFAULT_ROLL_FILE_SIZE_THRESHOLD = 10 * 1024 * 1024; // 10 MB
        private const string COLUMN_SEPARATOR = "\t";

        private readonly object _syncObj = new object();
        private readonly string _logDir;
        private readonly LogFileNameComposer _logFileNameComposer;
        private readonly IFileSystem _fileSystem;
        private readonly TLogItem _headerLogItem;
        private readonly int _rollFileSizeThreshold;

        private StreamWriter _streamWriter;
        private string _currentPrefix;
        private string _currentPath;
        private bool _headersAreVerified;

        public FileWriterLogItemHandler(
            string logDir,
            LogFileNameComposer logFileNameComposer,
            IFileSystem fileSystem,
            TLogItem headerLogItem,
            int rollFileSizeThreshold = DEFAULT_ROLL_FILE_SIZE_THRESHOLD)
        {
            _logDir = logDir;
            _logFileNameComposer = logFileNameComposer;
            _fileSystem = fileSystem;
            _headerLogItem = headerLogItem;
            _rollFileSizeThreshold = rollFileSizeThreshold;
        }

        public void Dispose()
        {
            lock (_syncObj)
            {
                CloseStreamWriter();
            }
        }

        public void Add(TLogItem logItem)
        {
            lock (_syncObj)
            {
                try
                {
                    EnsureFileOpen(logItem.Timestamp);
                    _streamWriter.WriteLine(GetLogItemString(logItem));
                }
                catch
                {
                }
            }
        }

        private void EnsureFileOpen(DateTime timestamp)
        {
            string prefix = _logFileNameComposer.GetDatePart(timestamp);

            var lazyPath = new Lazy<string>(() => GetLogFilePath(prefix));

            if (_currentPrefix != prefix)
            {
                if (_streamWriter != null)
                {
                    CloseStreamWriter();
                }

                _currentPrefix = prefix;
                _currentPath = lazyPath.Value;
            }

            VerifyHeaders(lazyPath.Value, prefix);

            if (_streamWriter == null)
            {
                _streamWriter = CreateStreamWriter(lazyPath.Value);
            }

            if (_streamWriter.BaseStream.Length > _rollFileSizeThreshold)
            {
                CloseStreamWriter();
                RollFile(prefix);
                _streamWriter = CreateStreamWriter(lazyPath.Value);
            }
        }

        private void VerifyHeaders(string filename, string prefix)
        {
            lock (_syncObj)
            {
                if (_headersAreVerified)
                {
                    return;
                }
                bool forceRollFile = false;
                using (var reader = CreateStreamReader(filename))
                {
                    if (reader == null)
                    {
                        _headersAreVerified = true;
                        return;
                    }

                    var existingHeaderLine = reader.ReadLine();
                    var expectedHeaderLine = GetLogItemString(_headerLogItem);

                    forceRollFile = !expectedHeaderLine.Equals(existingHeaderLine);                    
                    _headersAreVerified = true;
                }

                if (forceRollFile)
                {
                    RollFile(prefix);
                }
            }
        }

        private string GetLogFilePath(string prefix)
        {
            return Path.Combine(_logDir, _logFileNameComposer.GetFileName(prefix));
        }

        private string GetLogFilePath(string prefix, int rollNumber)
        {
            return Path.Combine(_logDir, _logFileNameComposer.GetFileName(prefix, rollNumber.ToString()));
        }

        private static string GetLogItemString(TLogItem logItem)
        {
            return string.Join(COLUMN_SEPARATOR, logItem.Values);
        }

        private StreamWriter CreateStreamWriter(string filename)
        {
            var directoryName = Path.GetDirectoryName(filename);
            _fileSystem.EnsureDirectory(directoryName);

            Stream stream = _fileSystem.GetStream(filename, FileMode.Append, FileAccess.Write);

            var streamWriter = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            if (stream.Length <= 5) // Ignore the BOM characters (which can be up to 5 depending on encoding)
            {
                streamWriter.WriteLine(GetLogItemString(_headerLogItem));
            }

            return streamWriter;
        }

        private StreamReader CreateStreamReader(string filename)
        {
            if (!_fileSystem.FileExists(filename))
            {
                return null;
            }

            Stream stream = _fileSystem.GetStream(filename, FileMode.Open, FileAccess.Read);

            return new StreamReader(stream, Encoding.UTF8);
        }

        private void CloseStreamWriter()
        {
            try
            {
                _streamWriter?.Close();
                _streamWriter = null;
            }
            catch
            {
            }
        }

        private int GetHighestRollNumber(string prefix)
        {
            int rollNumber = 0;

            while (true)
            {
                rollNumber++;
                string path = GetLogFilePath(prefix, rollNumber);
                if (!_fileSystem.FileExists(path))
                {
                    return rollNumber - 1;
                }
            }
        }

        private void RollFile(string prefix)
        {
            int highestRollNumber = GetHighestRollNumber(prefix);
            if (highestRollNumber > 0)
            {
                for (var i = highestRollNumber; i > 0; i--)
                {
                    string sourcePath = GetLogFilePath(prefix, i);
                    string destPath = GetLogFilePath(prefix, i + 1);

                    try
                    {
                        Log.To.Telemetry.Add(() => $"Renaming '{sourcePath}' to '{destPath}'");
                        _fileSystem.MoveFile(sourcePath, destPath);
                    }
                    catch (Exception ex)
                    {
                        Log.To.Telemetry.Add(() => $"Failed to rename '{sourcePath}' to '{destPath}': {ex}");
                    }
                }
            }

            string currentPath = GetLogFilePath(prefix);
            string firstRollPath = GetLogFilePath(prefix, 1);

            Log.To.Telemetry.Add(() => $"Renaming active log file '{currentPath}' to '{firstRollPath}'");
            _fileSystem.MoveFile(currentPath, firstRollPath);
        }
    }
}