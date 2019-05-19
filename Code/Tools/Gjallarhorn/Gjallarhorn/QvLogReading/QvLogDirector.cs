using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Eir.Common.Extensions;
using Eir.Common.IO;
using FreyrCommon.Logging;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;
using DirectoryInfo = System.IO.DirectoryInfo;

namespace Gjallarhorn.QvLogReading
{
    class QvLogDirector
    {

        private string _sessionLogStartsWith = "Sessions_";
        
        private readonly List<ColumnInfo> _sessionCols = new List<ColumnInfo>();
        private readonly QvLogHelper _helper = new QvLogHelper();
        private FileMinerDto _basicDataFromCase;
        private const int SESSION_LOG_TIMESTAMP_COLUMN_INDEX = 3;
        private readonly Dictionary<string, AuditActivityProxyMiner.CountTheSessions> _sessionLenght = new Dictionary<string, AuditActivityProxyMiner.CountTheSessions>();

        private void Initialize()
        {
            //const string s = "QvsLogAgent.Session.";
            _sessionCols.Add(new ColumnInfo(0, "SessionId"));//can be at the start and end of session log or not at all!!
            //_sessionCols[1] = new ColumnInfo(1, "Exe Type", s + "ExeType");
            //_sessionCols[2] = new ColumnInfo(2, "Exe Version", s + "ExeVersion");
            //_sessionCols[3] = new ColumnInfo(3, "Server Started", s + "ServerStarted");
            _sessionCols.Add(new ColumnInfo(5, "Document"));
            //_sessionCols[5] = new ColumnInfo(7, "QlikView User", s + "QlikViewUser");
            _sessionCols.Add(new ColumnInfo(8, "Exit Reason"));
            _sessionCols.Add(new ColumnInfo(9, "Session Start"));
            _sessionCols.Add(new ColumnInfo(10, "Session Duration"));
            //_sessionCols[9] = new ColumnInfo(12, "Bytes Received", s + "BytesReceived");
            //_sessionCols[10] = new ColumnInfo(13, "Bytes Sent", s + "BytesSent");
            //_sessionCols[11] = new ColumnInfo(14, "Calls", s + "Calls");
            //_sessionCols[12] = new ColumnInfo(15, "Selections", s + "Selections");
            _sessionCols.Add(new ColumnInfo(16, "Authenticated user"));
            _sessionCols.Add(new ColumnInfo(17, "Identifying user"));
            //_sessionCols[15] = new ColumnInfo(18, "Client machine identification", s + "ClientMachineIdentification");
            //_sessionCols[16] = new ColumnInfo(20, "Client Type", s + "ClientType");
            //_sessionCols[17] = new ColumnInfo(21, "Client Build Version", s + "ClientBuildVersion");
            //_sessionCols[18] = new ColumnInfo(25, "Client Address", s + "ClientAddress");
            //_sessionCols[19] = new ColumnInfo(27, "Cal Type", s + "CalType");
        }

        public void LoadAndRead(DirectorySetting archivedLogFolder, LogFileDirectorSettings settings, FileMinerDto basicDataFromCase)
        {
            _basicDataFromCase = basicDataFromCase;
            if (!archivedLogFolder.Exists)
            {
                Log.Add($"Unable to read QV logs in rootFolder ('{archivedLogFolder}') is not a valid folder.");
                return;
            }

            Initialize();

            FileSetting defaultSessionLog = GetNewestFile(archivedLogFolder);
            if (defaultSessionLog == null)
            {
                Log.Add($"No session files exists in log directory {archivedLogFolder}");
            }

            long sessionLogPosition = _helper.SessionLogPostionSetting();
            FileSetting sessionLog = _helper.SessionLogFileSetting() ?? defaultSessionLog;

            ReadLogs(sessionLog, sessionLogPosition, _sessionLogStartsWith, _sessionCols, null, SESSION_LOG_TIMESTAMP_COLUMN_INDEX);
        }

        private void ReadLogs(FileSetting currentLog, long currentLogPosition, string startsWith, List<ColumnInfo> columns, LogRowFilter filter, int timestampColIndex)
        {
            currentLogPosition = ReadLog(currentLog, currentLogPosition, columns, filter, timestampColIndex);

            _helper.SessionLogPostionSetting(currentLogPosition);
            _helper.SessionLogFileSetting(currentLog.Path);

            while (HasMoreFiles(startsWith, currentLog))
            {
                currentLog = GetNextFile(startsWith, currentLog);
                if (!currentLog.Exists)
                {   
                    return;
                }

                currentLogPosition = ReadLog(currentLog, 0, columns, filter, timestampColIndex);

                _helper.SessionLogPostionSetting(currentLogPosition);
                _helper.SessionLogFileSetting(currentLog.Path);
            }
        }
        public void FinaliseStatistics()
        {
            if (_basicDataFromCase == null || _sessionLenght == null) return;
            var sessionLengths = new List<int>();

            //var sessionsWithoutEnd = _sessionLenght.ToList().Count(p=>p.Value.SessionEnd == DateTime.MinValue);
            var sessionLenghtLocal = _sessionLenght.Where(p => p.Value.SessionEnd != DateTime.MinValue);
            //var sessionsWithoutEnd2 = _sessionLenght2.ToList().Count(p => p.Value.SessionEnd == DateTime.MinValue);
            foreach (var item in sessionLenghtLocal)
            {
                if (item.Value.SessionEnd == DateTime.MinValue)
                {
                    item.Value.SessionEnd = base.DataMinerRowValues.RowDate;
                }

                var min = (int)(item.Value.SessionEnd - item.Value.SessionStart).TotalMinutes;
                if (min == 0)
                {
                    continue;
                }
                sessionLengths.Add(min);
            }

            _basicDataFromCase.SessionLengthAvgInMinutes = (int)Math.Round(sessionLengths.Any() ? sessionLengths.Average() : 0, MidpointRounding.AwayFromZero);
            _basicDataFromCase.SessionLengthMedInMinutes = (int)Math.Round(sessionLengths.Any() ? sessionLengths.Median() : 0, MidpointRounding.AwayFromZero);
            _basicDataFromCase.TotalNrOfSessions = sessionLengths.Count;
        }
        private long ReadLog(FileSetting file, long position, List<ColumnInfo> masterCols, LogRowFilter rowFilter, int timestampColIndex)
        {
            int nrOfFailedLogLines = 0;
            int expectedMinimumColCount = 19;
            try
            {
                var timestamp = DateTime.UtcNow;
                var fi = file.GetFileInfo();
                if (position > fi.Length) position = 0;
                if (position < fi.Length)
                {
                    List<ColumnInfo> cols;
                    using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var headers = sr.ReadLine();
                            cols = Validate(headers, masterCols);
                            if (cols == null)
                            {
                                Log.Add($"ReadLog failed with evaluating headers headers: {headers}. Ignoring file: {file}. ");
                                return position;
                            }
                        }
                    }
                    using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096))
                    {
                        if (position > 0)
                            fs.Position = position;
                        using (var sr = new StreamReader(fs))
                        {
                            if (position == 0 && !sr.EndOfStream)
                            {
                                sr.ReadLine();
                            }
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine() + "";
                                var a = line.Split('\t');
                                if (a.Length < expectedMinimumColCount)
                                {
                                    nrOfFailedLogLines++;
                                    continue;
                                }
                                if (rowFilter == null || rowFilter.Matches(a))
                                {
                                    timestamp = TryParse(a[timestampColIndex], timestamp);

                                    //QvsLogAgentDatapointSet dps = QvsLogAgentDatapointSetTemplate.CreateDatapointSet();

                                    foreach (ColumnInfo c in cols.Where(c => a.Length > c.Index))
                                    {
                                        //dps.AddDatapoint(c.DatapointName, a[c.Index]);
                                    }

                                    //AddDatapointBatch(dps);
                                }
                                position = fs.Position;
                            }
                        }
                    }
                }
                if (nrOfFailedLogLines > 0)
                {
                    Log.Add($"Failed reading some lines in log: {file}. Nr of lines skipped: {nrOfFailedLogLines}.");
                }
            }
            catch (Exception ex)
            {
                Log.Add($"Failed reading and sending log: {file} on position: {position}. {ex}");
            }

            return position;
        }

        private DateTime TryParse(string value, DateTime defaultValue)
        {
            try
            {
                var style = DateTimeStyles.AdjustToUniversal;
                if (value.Length < 20)
                    style = DateTimeStyles.AssumeLocal;
                string[] format = { "yyyy-MM-dd HH:mm:ss", "yyyyMMddTHHmmss.fffzzz" };
                DateTime date;
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, style, out date))
                    return date.ToUniversalTime(); // if its already universal no conversion is done.
                return defaultValue;
            }
            catch (Exception ex)
            {
                Log.Add($"Failed to parse string date from log to UTC: {value}", ex);
                return defaultValue;
            }
        }

        //private DateTime TryParseStringToDateTimeUtc(string value, DateTime defaultValue)
        //{
        //    try
        //    {
        //        const string format = "yyyy-MM-dd hh:MM:ss";
        //        if (value.Length != format.Length) throw new Exception();
        //        int year = int.Parse(value.Substring(0, 4));    
        //        int month = int.Parse(value.Substring(5, 2));
        //        int day = int.Parse(value.Substring(8, 2));
        //        int hour = int.Parse(value.Substring(11, 2));
        //        int minute = int.Parse(value.Substring(14, 2));
        //        int second = int.Parse(value.Substring(17, 2));
        //        return new DateTime(year, month, day, hour, minute, second).ToUniversalTime();
        //    }
        //    catch
        //    {
        //        _Log.Add("Failed to parse string date from log to UTC: " + value);
        //        return defaultValue;
        //    }
        //}

        private List<ColumnInfo> Validate(string header, IList<ColumnInfo> cols)
        {
            var a = header.Split('\t');
            if (a.Length < 19)
            {
                Log.Add($"Header validation failed with incorrect length. Headers length is: {a.Length}");
                return null;
            }
            var ret = new List<ColumnInfo>();
            foreach (ColumnInfo columnInfo in cols)
            {
                var index = 0;
                ColumnInfo col = null;
                foreach (var s in a)
                {
                    if (string.Equals(s, columnInfo.HeaderName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        col= new ColumnInfo(index,s);
                        continue;
                    }

                    index++;
                }
              
                if (col == null)
                {
                    Log.Add($"Did not find header in header validation. Missing header name is: {columnInfo.HeaderName}");
                    return null;
                }
                ret.Add(col);
            }
            return ret;
        }

        private static bool HasMoreFiles(string startsWith, FileSetting current)
        {
            var files = Directory.GetFiles(current.ParentDirectory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
            FileInfo cfi = current.GetFileInfo();
            return files.Select(f => new FileInfo(f)).Any(fi => fi.LastWriteTime > cfi.LastWriteTime);
        }


        private FileSetting GetNextFile(string startsWith, FileSetting current)
        {
            /*
             * A network drive could have a disconnect and we will retry next time around.
             */
            try
            {
                string[] files = Directory.GetFiles(current.ParentDirectory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
                FileInfo cfi = current.GetFileInfo();
                cfi.Refresh();
                var fis = files.Select(f => new FileInfo(f)).ToList();
                fis.Sort((f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));
                return new FileSetting(fis.First(t => t.LastWriteTime > cfi.LastWriteTime).FullName);
            }
            catch (Exception ex)
            {
                Log.Add($"GetNextFile could not access filelist, will try again.{ex}");
                return FileSetting.Empty;
            }
        }

        private FileSetting GetNewestFile(DirectorySetting dir)
        {
            var directory = new DirectoryInfo(dir.Path);

            var myFile = directory.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();
            return new FileSetting(myFile?.FullName);
        } 

        private static FileSetting GeOldestFile(string startsWith, DirectorySetting directory)
        {
            var files = Directory.GetFiles(directory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
            var fis = files.Select(f => new FileInfo(f)).ToList();
            if (fis.Count == 0) return FileSetting.Empty;
            fis.Sort((f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));
            return new FileSetting(fis.First().FullName);
        }
    }
}