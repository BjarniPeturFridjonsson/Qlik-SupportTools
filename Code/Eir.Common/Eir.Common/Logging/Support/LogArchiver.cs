using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.IO;
using Eir.Common.Time;
using ICSharpCode.SharpZipLib.Zip;

namespace Eir.Common.Logging
{
    public class LogArchiver : IDisposable
    {
        private class LogFileInfo
        {
            public LogFileInfo(string prefix, IFileInfo fileInfo)
            {
                Prefix = prefix;
                FileInfo = fileInfo;
            }

            public string Prefix { get; }

            public IFileInfo FileInfo { get; }
        }

        private readonly string _logDir;
        private readonly int _daysToIgnore;
        private readonly IFileSystem _fileSystem;
        private readonly ILogFileNameComposer _logFileNameComposer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ITrigger _trigger;

        public LogArchiver(
            string logDir,
            int daysToIgnore,
            IFileSystem fileSystem,
            ILogFileNameComposer logFileNameComposer,
            IDateTimeProvider dateTimeProvider,
            ITrigger trigger)
        {
            _logDir = logDir;
            _daysToIgnore = daysToIgnore;
            _fileSystem = fileSystem;
            _logFileNameComposer = logFileNameComposer;
            _dateTimeProvider = dateTimeProvider;
            _trigger = trigger;

            _trigger.RegisterAction(PerformArchiving);
        }

        public void Dispose()
        {
            _trigger.UnregisterAction(PerformArchiving);
        }

        private Task PerformArchiving()
        {
            try
            {
                IEnumerable<string> ignoredPrefixes = GetIgnoredPrefixes().ToArray();

                IEnumerable<LogFileInfo> logFileInfos = LogFilePathsToArchive(ignoredPrefixes).ToArray();

                ArchiveLogFiles(logFileInfos);
            }
            catch (Exception ex)
            {
                Log.To.WindowsEvent.Error($"{nameof(LogArchiver)}.{nameof(PerformArchiving)}", ex);
            }

            return Task.FromResult(false);
        }

        private IEnumerable<LogFileInfo> LogFilePathsToArchive(IEnumerable<string> ignoredPrefixes)
        {
            var ignoredPrefixHashSet = new HashSet<string>(ignoredPrefixes, StringComparer.OrdinalIgnoreCase);

            string[] filePaths = _fileSystem.EnumerateFiles(_logDir, _logFileNameComposer.GetFileName("*"))
                .Concat(_fileSystem.EnumerateFiles(_logDir, _logFileNameComposer.GetFileName("*", "*")))
                .ToArray();

            foreach (string filePath in filePaths)
            {
                string datePart;
                string roll;

                if (_logFileNameComposer.TryParse(
                    Path.GetFileName(filePath),
                    out datePart,
                    out roll))
                {
                    if (!ignoredPrefixHashSet.Contains(datePart))
                    {
                        IFileInfo fileInfo = _fileSystem.GetFileInfo(filePath);
                        yield return new LogFileInfo(datePart, fileInfo);
                    }
                }
            }
        }

        private IEnumerable<string> GetIgnoredPrefixes()
        {
            DateTime ignoreDate = _dateTimeProvider.Time();

            for (var i = _daysToIgnore; i > 0; i--)
            {
                yield return _logFileNameComposer.GetDatePart(ignoreDate);

                ignoreDate = ignoreDate.Add(TimeSpan.FromDays(-1));
            }
        }

        private void ArchiveLogFiles(IEnumerable<LogFileInfo> logFileInfos)
        {
            var prefixGroups = logFileInfos.GroupBy(x => x.Prefix, StringComparer.OrdinalIgnoreCase);

            foreach (var prefixGroup in prefixGroups)
            {
                string prefix = prefixGroup.Key;

                ArchiveLogFiles(prefix, prefixGroup.Select(x => x.FileInfo).ToArray());
            }
        }

        private class FuncStreamStaticDataSource : IStaticDataSource
        {
            private readonly Func<Stream> _getStream;

            public FuncStreamStaticDataSource(Func<Stream> getStream)
            {
                _getStream = getStream;
            }

            public Stream GetSource() => _getStream();
        }

        private void ArchiveLogFiles(string prefix, IFileInfo[] logFileInfos)
        {
            string zipPath = Path.Combine(_logDir, $"Archived_{prefix}.zip");

            var archivedLogFileInfos = new List<IFileInfo>();

            try
            {
                if (_fileSystem.FileExists(zipPath))
                {
                    Dictionary<string, ZipEntry> zipEntries = GetZipEntries(zipPath)
                        .ToDictionary(x => x.Name);

                    IFileInfo[] logFilesNotInZip = logFileInfos
                        .Where(fileInfo =>
                        {
                            string name = Path.GetFileName(fileInfo.FullName);
                            ZipEntry existingZipEntry;
                            if (!zipEntries.TryGetValue(name, out existingZipEntry))
                            {
                                return true;
                            }

                            if (existingZipEntry.Size != fileInfo.Length)
                            {
                                return true;
                            }

                            double secondsDiff = Math.Abs(existingZipEntry.DateTime.Subtract(fileInfo.LastWriteTimeUtc).TotalSeconds);
                            if (secondsDiff > 2)
                            {
                                return true;
                            }

                            return false;
                        })
                        .ToArray();

                    if (!logFilesNotInZip.Any())
                    {
                        archivedLogFileInfos.AddRange(logFileInfos);
                    }
                    else
                    {
                        using (Stream stream = _fileSystem.GetStream(zipPath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            var zipEntryFactory = new ZipEntryFactory(ZipEntryFactory.TimeSetting.Fixed);

                            var zipFile = new ZipFile(stream)
                            {
                                EntryFactory = zipEntryFactory
                            };

                            zipFile.BeginUpdate();

                            foreach (IFileInfo fileInfo in logFilesNotInZip)
                            {
                                Func<Stream> getStreamFunc =
                                    () => _fileSystem.GetStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

                                var staticDataSource = new FuncStreamStaticDataSource(getStreamFunc);

                                zipEntryFactory.FixedDateTime = fileInfo.LastWriteTimeUtc;
                                zipFile.Add(staticDataSource, Path.GetFileName(fileInfo.FullName));

                                archivedLogFileInfos.Add(fileInfo);
                            }

                            zipFile.CommitUpdate();
                        }
                    }
                }
                else
                {
                    using (Stream stream = _fileSystem.GetStream(zipPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (var zipOutputStream = new ZipOutputStream(stream))
                        {
                            foreach (IFileInfo fileInfo in logFileInfos)
                            {
                                AddFileToZip(fileInfo, zipOutputStream);
                                archivedLogFileInfos.Add(fileInfo);
                            }
                        }
                    }
                }
            }
            catch
            {
                archivedLogFileInfos.Clear(); // Don't delete any file!

                if (_fileSystem.FileExists(zipPath))
                {
                    _fileSystem.DeleteFile(zipPath);
                }
            }

            DeleteFiles(archivedLogFileInfos);
        }

        private void AddFileToZip(IFileInfo fileInfo, ZipOutputStream zipStream)
        {
            var newEntry = new ZipEntry(Path.GetFileName(fileInfo.FullName))
            {
                DateTime = fileInfo.LastWriteTimeUtc,
                Size = fileInfo.Length
            };

            zipStream.PutNextEntry(newEntry);

            using (Stream fileStream = _fileSystem.GetStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(zipStream);
            }

            zipStream.CloseEntry();
        }

        private IEnumerable<ZipEntry> GetZipEntries(string zipPath)
        {
            using (Stream stream = _fileSystem.GetStream(zipPath, FileMode.Open, FileAccess.Read))
            {
                using (var zipInputStream = new ZipInputStream(stream))
                {
                    while (true)
                    {
                        ZipEntry zipEntry = zipInputStream.GetNextEntry();
                        if (zipEntry == null)
                        {
                            yield break;
                        }

                        yield return zipEntry;
                    }
                }
            }
        }

        private void DeleteFiles(IEnumerable<IFileInfo> fileInfos)
        {
            foreach (IFileInfo fileInfo in fileInfos)
            {
                try
                {
                    _fileSystem.DeleteFile(fileInfo.FullName);
                }
                catch
                {
                }
            }
        }
    }
}