using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Eir.Common.IO;
using Gjallarhorn.Common;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    public class BaseDataMiner
    {
        //protected int NrOfErrors  { get; private set; }
        private int DateColumnIndex { get; set; } = -1;
        protected Dictionary<string, int> ColumnNames;
        protected DateTime FirstDate { get; private set; }
        protected FileMinerDto BasicDataFromCase { get; set; }
        protected DataMinerSettings DataMinerSettings { get; private set; } = new DataMinerSettings();
        protected BaseDateMinerDynamicMasterRowValues DataMinerRowValues { get; private set; } = new BaseDateMinerDynamicMasterRowValues();
        protected string CurrentFilePath { get; set; }

        private bool _inFirstLine;

        public virtual void InitializeNewFile(string headerLine, FileMinerDto basicDataFromCase, string path)
        {
            BasicDataFromCase = basicDataFromCase;
            AnalyzeHeaderBase(headerLine);
            CurrentFilePath = path;
        }

        private void AnalyzeHeaderBase(string line)
        {
            ColumnNames = new Dictionary<string, int>();
            _inFirstLine = true;
            var cols = line.Split('\t');
            for (var columnNr = 0;
                columnNr < cols.Length;
                columnNr++) // will support bad lines where the idiots have not crlf safed their error messages
            {
                var col = cols[columnNr] + "";
                ColumnNames.Add(col.ToLower(), columnNr);
                if (
                        col.Equals("date", StringComparison.InvariantCultureIgnoreCase) ||
                        col.Equals("dateTime", StringComparison.InvariantCultureIgnoreCase) ||
                        col.Equals("timestamp", StringComparison.InvariantCultureIgnoreCase)
                )
                {
                    DateColumnIndex = columnNr;
                }
            }
        }

        protected string GetMineLocation(string basePath, string myPath, IFileSystem fileSystem)
        {
            var pathToMine = fileSystem.Path.Combine(basePath, myPath);
            return fileSystem.DirectoryExists(pathToMine) ? pathToMine : "";
        }

        protected void MineFile(string line, Action<int, string> analyzeColumn)
        {
            DataMinerRowValues.ErrorLevel = BasicErrorLevel.Undefined;
            DataMinerRowValues.RowDate = DateTime.MinValue;
            //bool inErrorLine = false;

            var cols = line.Split('\t');
            for (var columnNr = 0;
                columnNr < cols.Length;
                columnNr++) // will support bad lines where the idiots have not crlf safed their error messages
            {
                var columnValue = cols[columnNr] + "";
                if (columnNr < 5)
                {


                    if (DataMinerSettings.NeedErrorLevelPerRow && DataMinerRowValues.ErrorLevel == BasicErrorLevel.Undefined)
                    {
                        if (columnValue.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                        {
                            DataMinerRowValues.ErrorLevel = BasicErrorLevel.Err;

                        }

                        if (columnValue.Equals("Warning", StringComparison.InvariantCultureIgnoreCase)) DataMinerRowValues.ErrorLevel = BasicErrorLevel.Warn;
                        if (columnValue.Equals("WARN")) DataMinerRowValues.ErrorLevel = BasicErrorLevel.Warn;
                        if (columnValue.Equals("INFO")) DataMinerRowValues.ErrorLevel = BasicErrorLevel.Info;
                    }

                    if ((_inFirstLine && columnNr == DateColumnIndex) || (DataMinerSettings.NeedDatePerRow && columnNr == DateColumnIndex))
                    {
                        if (!DateTime.TryParseExact(columnValue, "yyyyMMdd'T'HHmmss'.'fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                        {
                            if (!DateTime.TryParse(columnValue, out date))
                            {
                                Trace.WriteLine($"DateParsing this failed{columnValue}");
                            }
                        }
                        DataMinerRowValues.RowDate = date;
                        if (_inFirstLine)
                        {
                            FirstDate = date;
                            if (BasicDataFromCase.OldestLogLine == DateTime.MinValue || BasicDataFromCase.OldestLogLine > date)
                            {
                                BasicDataFromCase.OldestLogLine = date;
                            }
                            //FirstDate = DateTime.Parse("2017-01-01");
                        }
                    }
                }
                else
                {
                    analyzeColumn.Invoke(columnNr, columnValue);
                }
            }

            _inFirstLine = false;
        }
    }
}
