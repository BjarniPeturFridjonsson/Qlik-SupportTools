using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Eir.Common.Extensions;
using Eir.Common.IO;
using Eir.Common.Logging;
using Gjallarhorn.SenseLogReading;
using Gjallarhorn.SenseLogReading.FileMiners;
using DirectoryInfo = System.IO.DirectoryInfo;

namespace Gjallarhorn.QvLogReading
{
    class QvLogDirector
    {

        private string _sessionLogStartsWith = "Sessions_";
        
        private readonly List<ColumnInfo> _sessionCols = new List<ColumnInfo>();
        //private readonly QvLogHelper _helper = new QvLogHelper();
        private FileMinerDto _basicDataFromCase;
        private QvSessionData _sessionData;
        


        private void Initialize()
        {
            //const string s = "QvsLogAgent.Session.";
            //_sessionCols.Add(new ColumnInfo(0, "SessionId"));//can be at the start and end of session log or not at all!!
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
            //Read root folder and one directory down.
            _sessionData = new QvSessionData();
            _basicDataFromCase = basicDataFromCase;
            if (!archivedLogFolder.Exists)
            {
                 Log.To.Main.Add($"Unable to read QV logs in rootFolder ('{archivedLogFolder}') is not a valid folder.");
                return;
            }

            Initialize();
            Log.To.Main.Add($"Reading log folder {archivedLogFolder.Path}");
            var logs = EnumerateLogFiles(archivedLogFolder.Path, settings.StartDateForLogs)?.ToList() ?? new List<IFileInfo>();
            if (!logs.Any())
            {
                 Log.To.Main.Add($"No session files exists for {settings.StartDateForLogs} - {settings.StopDateForLogs} in log directory {archivedLogFolder}");
            }

            ReadLogs(logs, _sessionLogStartsWith, _sessionCols, settings);

            _basicDataFromCase.TotalUniqueActiveApps = _sessionData.Apps.Count;
            _basicDataFromCase.TotalUniqueActiveUsers = _sessionData.Users.Count;
            _basicDataFromCase.TotalUniqueActiveUsersList = _sessionData.Users;
            _basicDataFromCase.TotalUniqueActiveAppsList = _sessionData.Apps;

            _basicDataFromCase.SessionLengthAvgInMinutes = (int)Math.Round(_sessionData.SessionLenghts.Any() ? _sessionData.SessionLenghts.Average() : 0, MidpointRounding.AwayFromZero);
            _basicDataFromCase.SessionLengthMedInMinutes = (int)Math.Round(_sessionData.SessionLenghts.Any() ? _sessionData.SessionLenghts.Median() : 0, MidpointRounding.AwayFromZero);
            _basicDataFromCase.TotalNrOfSessions = _sessionData.SessionLenghts.Count;
        }

        private void ReadLogs(IEnumerable<IFileInfo> logFiles, string startsWith, List<ColumnInfo> columns, LogFileDirectorSettings settings)
        {
            foreach (var file in logFiles)
            {
                if (file.Name.Contains(startsWith, StringComparison.InvariantCultureIgnoreCase))
                     ReadLog(file, columns, settings);
            }
        }

        private void ReadLog(IFileInfo file, List<ColumnInfo> masterCols, LogFileDirectorSettings settings)
        {
            Log.To.Main.Add( $"Reading log {file.FullName}");
            var nrOfFailedLogLines = 0;
            var nrOfFailedLogLinesDate = 0;
            var expectedMinimumColCount = 19;
            var lineCounter = 0;
            try
            {
                {
                    Dictionary<string,ColumnInfo> cols;
                   
                    using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096))
                    {
                      
                        using (var sr = new StreamReader(fs))
                        {
                            if (sr.EndOfStream) return;//empty file
                         
                            var headers = sr.ReadLine();
                            cols = Validate(headers, masterCols);
                            if (cols == null)
                            {
                                Log.To.Main.Add($"ReadLog failed with evaluating headers headers: {headers}. Ignoring file: {file}. ");
                                return;
                            }
                            
                            while (!sr.EndOfStream)
                            {
                                lineCounter++;
                                var line = sr.ReadLine() + "";
                                var a = line.Split('\t');
                                if (a.Length < expectedMinimumColCount)
                                {
                                    nrOfFailedLogLines++;
                                    continue;
                                }
                              
                               var timestamp = TryParse(a[cols["timestamp"].Index], DateTime.MinValue);
                               if(timestamp == DateTime.MinValue)
                               {
                                   nrOfFailedLogLinesDate++;
                                   continue;
                               }
                               if (timestamp.Date < settings.StartDateForLogs.Date)
                               {
                                   continue;//inefficient but effective. If we store postion we need to get the private variables from the streamreader or create our own.
                                }
                                //we should not continue reading into the day that is already active.
                                if (timestamp.Date > settings.StopDateForLogs.Date)
                               {
                                   Trace.WriteLine($"stopping reading log due to date {timestamp}");
                                   break;
                               }

                                var user = a[cols["Authenticated user"].Index];
                                if (_sessionData.Users.ContainsKey(user))
                                    _sessionData.Users[user]++;
                                else
                                    _sessionData.Users[user] = 1;


                                var docs = a[cols["Document"].Index];
                                if (_sessionData.Apps.ContainsKey(docs))
                                    _sessionData.Apps[docs]++;
                                else
                                    _sessionData.Apps[docs] = 1;


                                if (TimeSpan.TryParseExact((a[cols["Session Duration"].Index]), @"hh\:mm\:ss", CultureInfo.InvariantCulture,out var timespan))
                                {
                                    _sessionData.SessionLenghts.Add((int)timespan.TotalMinutes);
                                }
                                

                                //QvsLogAgentDatapointSet dps = QvsLogAgentDatapointSetTemplate.CreateDatapointSet();

                                //foreach (ColumnInfo c in cols.Where(c => a.Length > c.Index))
                                //{
                                //    //dps.AddDatapoint(c.DatapointName, a[c.Index]);
                                //}

                                //AddDatapointBatch(dps);

                             
                            }
                        }
                    }
                }
                if (nrOfFailedLogLines > 0 || nrOfFailedLogLinesDate > 0)
                {
                     Log.To.Main.Add($"Failed reading some lines in log: {file}. Nr of lines skipped: {nrOfFailedLogLines}. Datetime failures are :{nrOfFailedLogLinesDate} and lines read {lineCounter}");
                }
            }
            catch (Exception ex)
            {
                 Log.To.Main.Add($"Failed reading and sending log: {file} on position: {lineCounter}. {ex}");
            }

          
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
                 Log.To.Main.AddException($"Failed to parse string date from log to UTC: {value}", ex);
                return defaultValue;
            }
        }

        private Dictionary<string,ColumnInfo> Validate(string header, IList<ColumnInfo> cols)
        {
            var a = header.Split('\t');
            if (a.Length < 19)
            {
                 Log.To.Main.Add($"Header validation failed with incorrect length. Headers length is: {a.Length}");
                return null;
            }
            var ret = new Dictionary<string, ColumnInfo>();
            var firstPass = true;

            foreach (ColumnInfo columnInfo in cols)
            {
                var index = 0;
                ColumnInfo col = null;
                foreach (var s in a)
                {
                    if (string.Equals(s, columnInfo.HeaderName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        col= new ColumnInfo(index,s);
                        
                        if(!firstPass) continue;
                    }
                   
                    if (firstPass &&(//find the datetime header.(we dont trust that the timestamp column keeps its name so we take the Sense datetime names too
                        s.Equals("date", StringComparison.InvariantCultureIgnoreCase) ||
                        s.Equals("dateTime", StringComparison.InvariantCultureIgnoreCase) ||
                        s.Equals("timestamp", StringComparison.InvariantCultureIgnoreCase))
                    )
                    {
                        if(!ret.ContainsKey("timestamp"))
                            ret.Add("timestamp",new ColumnInfo(index,"timestamp"));
                    }
                    index++;
                }
              
                if (col == null)
                {
                     Log.To.Main.Add($"Did not find header in header validation. Missing header name is: {columnInfo.HeaderName}");
                    return null;
                }
                Trace.WriteLine(columnInfo.HeaderName);
                ret[col.HeaderName] = col;
                firstPass = false;
            }

            
            if (!ret.ContainsKey("timestamp"))
            {
                 Log.To.Main.Add($"Did not find timestamp header in header validation. ignoring file");
                return null;
            }

            return ret;
        }

        //private static bool HasMoreFiles(string startsWith, FileSetting current)
        //{
        //    var files = Directory.GetFiles(current.ParentDirectory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
        //    FileInfo cfi = current.GetFileInfo();
        //    return files.Select(f => new FileInfo(f)).Any(fi => fi.LastWriteTime > cfi.LastWriteTime);
        //}


        //private FileSetting GetNextFile(string startsWith, FileSetting current)
        //{
        //    /*
        //     * A network drive could have a disconnect and we will retry next time around.
        //     */
        //    try
        //    {
        //        string[] files = Directory.GetFiles(current.ParentDirectory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
        //        FileInfo cfi = current.GetFileInfo();
        //        cfi.Refresh();
        //        var fis = files.Select(f => new FileInfo(f)).ToList();
        //        fis.Sort((f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));
        //        return new FileSetting(fis.First(t => t.LastWriteTime > cfi.LastWriteTime).FullName);
        //    }
        //    catch (Exception ex)
        //    {
        //         Log.To.Main.Add($"GetNextFile could not access filelist, will try again.{ex}");
        //        return FileSetting.Empty;
        //    }
        //}

        //private FileSetting GetNewestFile(DirectorySetting dir)
        //{
        //    var directory = new DirectoryInfo(dir.Path);

        //    var myFile = directory.GetFiles()
        //        .OrderByDescending(f => f.LastWriteTime)
        //        .FirstOrDefault();
        //    return new FileSetting(myFile?.FullName);
        //} 

        //private static FileSetting GeOldestFile(string startsWith, DirectorySetting directory)
        //{
        //    var files = Directory.GetFiles(directory.Path, startsWith + "*", SearchOption.TopDirectoryOnly);
        //    var fis = files.Select(f => new FileInfo(f)).ToList();
        //    if (fis.Count == 0) return FileSetting.Empty;
        //    fis.Sort((f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));
        //    return new FileSetting(fis.First().FullName);
        //}

        private IEnumerable<IFileInfo> EnumerateLogFiles(string path, DateTime from)
        {
             Log.To.Main.Add($"Searching for logs with LastWriteTime => {from.ToString("yyyy-MM-dd hh:mm")} to {DateTime.Now.ToString("yyyy-MM-dd hh:mm")} in {path} ");
            var sw = new Stopwatch();
            sw.Start();
            var dirInfo = new DirectoryInfo(path);

            var a = dirInfo.EnumerateFiles().Where(p =>
                p.LastWriteTime >= from &&
                (p.Extension.Equals(".log", StringComparison.InvariantCultureIgnoreCase) || p.Extension.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            );

        

            var ret = new List<IFileInfo>();
            foreach (var fileInfo in a)
            {
                ret.Add(new SystemFileInfo(fileInfo));
            }
            sw.Stop();
            //Debug.WriteLine($"enum time => {sw.Elapsed.TotalSeconds} for {path}");
            return ret;
        }
    }
}