using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using FreyrViewer.Models;
using FreyrViewer.Ui.Grids.ModelFilter;

namespace FreyrViewer.Services
{
    public class GenericDataWrapperService
    {
        public List<ColumnHeaderWrapper<GenericDataWrapper>> Headers { get; } = new List<ColumnHeaderWrapper<GenericDataWrapper>>();
        public List<GenericDataWrapper> Lines { get; } = new List<GenericDataWrapper>();
        public LogFileAnalyzerService LogFileAnalyzer { get; set; }

        /// <summary>
        /// Anything over 1mill lines is iffy, Therefore we return the nr of rows and get the hell out of Dodge.
        /// </summary>
        public int LargeFileCutoffForSplit { get; set; } = 1000000;

        const Int32 BufferSize = 128;


        public string GetTextAtLine(int linePos)
        {
            var ret = "";
            if (Headers.Count == 0 || linePos >= Lines.Count) return ret;

            for (var ii = 0; ii < Headers.Count; ii++)
            {
                if (Headers[ii].ColumnType == typeof(DateTime)) continue;
                if (ii != 0)
                {
                    ret += "\t" + Headers[ii].AspectGetter.Invoke(Lines[linePos]);
                }
            }
            if (string.IsNullOrEmpty(ret)) return string.Empty;
            return ret.Substring(1);
        }

        private bool IsOldStyleQvLog(string col)
        {
            if (DateTime.TryParseExact(col, "yyyyMMdd'T'HHmmss'.'fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return true;
            if (DateTime.TryParse(col, out date))
                return true;
            Trace.WriteLine("DateColumnvalue=" + date);
            return false;
        }

        private ColumnHeaderWrapper<GenericDataWrapper> CreateStringColumn(string col, int ordinal, string modelName, PropertyInfo aspectGetter, bool showInGrid = true)
        {

            var header = new ColumnHeaderWrapper<GenericDataWrapper>
            {
                HeaderName = col,
                Ordinal = ordinal,
                ColumnNameInModel = modelName,
                AspectGetter = p => p == null ? "" : aspectGetter?.GetValue(p) as string,
                ShowInGrid = showInGrid
            };

            return header;
        }

        private ColumnHeaderWrapper<GenericDataWrapper> CreateDateColumn(string col, int ordinal)
        {

            var header = new ColumnHeaderWrapper<GenericDataWrapper>
            {
                HeaderName = col,
                Ordinal = ordinal,
                ColumnNameInModel = "DateTime1",
                ColumnType = typeof(DateTime),
            };
            return header;
        }

        public void LoadToInfo(string path)
        {
            var propList = new List<PropertyInfo>();
            LogFileAnalyzer = new LogFileAnalyzerService();
            using (StreamReader sr = File.OpenText(path))
            {
                bool isFirst = true;
                int dateColOrdinal = -1;

                var lineCounter = 0;

                string line = String.Empty;
                DateTime lineDate = DateTime.MinValue;
                while ((line = sr.ReadLine()) != null)
                {
                    lineCounter++;
                    var cols = line.Split('\t');

                    if (isFirst)
                    {
                        isFirst = false;
                        var isOldQvStuff = IsOldStyleQvLog(cols[0]);
                        //this might be a old style QV log.. Which we detect by col(0:0) = datetime.
                        propList = ReadAndSetHeaders(cols, isOldQvStuff, ref dateColOrdinal);
                        if (!isOldQvStuff)
                            continue;
                    }
                    
                    for (var i = 0; i < cols.Length; i++) // will support bad lines where the idiots have not crlf safed their error messages
                    {
                   
                            if (dateColOrdinal == i)
                            {
                                lineDate = GetDateTimeFromLog(cols[i]);
                            }

                            if (Headers[i].MaxCharCount < cols[i].Length)
                                Headers[i].MaxCharCount = cols[i].Length;
                            LogFileAnalyzer?.Analyze(Lines.Count, i, cols[i]);
                    }
                }
                LogFileAnalyzer?.OnFinished();
            }


        }

        public int LoadToJson(string path)
        {
            var propList = new List<PropertyInfo>();
            var lines = File.ReadLines(path);
            bool isFirst = true;

            int dateColOrdinal = -1;
            //var colCount = 0;
            var lineCounter = 0;
            foreach (var line in lines)
            {
                if (lineCounter >= LargeFileCutoffForSplit)
                {
                    return LargeFileCutoffForSplit;
                }

                lineCounter++;
                var cols = line.Split('\t');
                if (isFirst)
                {
                    isFirst = false;
                    var isOldQvStuff = IsOldStyleQvLog(cols[0]);
                    //this might be a old style QV log.. Which we detect by col(0:0) = datetime.
                    propList =  ReadAndSetHeaders(cols, isOldQvStuff, ref dateColOrdinal);
                    if(!isOldQvStuff)
                            continue;
                }

                var item = new GenericDataWrapper();
                string badShit = null;
                for (var i = 0; i < cols.Length; i++) // will support bad lines where the idiots have not crlf safed their error messages
                {
                    
                    if (propList.Count <= i)
                    {
                        //now this gets Bjarni Angry TABS IN A FRIGGING TAB SEPARATED LOG!!! REALLY !!!
                        badShit += "\t " + cols[i];
                    }
                    else
                    {
                        if (dateColOrdinal == i)
                        {
                            item.DateTime1 = GetDateTimeFromLog(cols[i]);
                        }

                        if (Headers[i].MaxCharCount < cols[i].Length)
                            Headers[i].MaxCharCount = cols[i].Length;
                        LogFileAnalyzer?.Analyze(Lines.Count, i, cols[i]);
                        propList[i].SetValue(item, cols[i]);
                    }

                }
                if (propList.Count < cols.Length)
                {
                    badShit = propList[propList.Count - 1].GetValue(item) +  "\t " + badShit;
                    propList[propList.Count-1].SetValue(item, badShit);
                }
                
                Lines.Add(item);
            }
            LogFileAnalyzer?.OnFinished();
            return 0; //winOk signal :)
        }

        private DateTime GetDateTimeFromLog(string str)
        {
            if (!DateTime.TryParseExact(str, "yyyyMMdd'T'HHmmss'.'fffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                if (!DateTime.TryParse(str, out date))
                {
                    Trace.WriteLine($"DateParsing this failed{str}");
                }
            }
            return date;
        }

        private List<PropertyInfo> ReadAndSetHeaders(string[] cols, bool isOldQvStuff, ref int dateColOrdinal)
        {
            int colCount = 0;
            var propList = new List<PropertyInfo>();
            foreach (var col in cols)
            {
                var modelName = $"String{colCount + 1}";
                var aspectGetter = typeof(GenericDataWrapper).GetProperty(modelName);
                propList.Add(aspectGetter); //we have to have as many aspect getters as cols.

                if (isOldQvStuff)
                {
                    if (colCount == 0) Headers.Add(CreateDateColumn("Date", colCount));
                    if (colCount == 0) Headers.Add(CreateStringColumn("Date", colCount, modelName, aspectGetter, false));
                    if (colCount == 1) Headers.Add(CreateStringColumn("Level", colCount, modelName, aspectGetter));
                    if (colCount == 2) Headers.Add(CreateStringColumn("Message", colCount, modelName, aspectGetter));
                    if (colCount > 2)
                        Headers.Add(CreateStringColumn($"(UnknownColumn{colCount - 2}", colCount, modelName, aspectGetter));
                    dateColOrdinal = 0;
                }
                else
                {
                    if (dateColOrdinal < 0 && (
                            col.Equals("date", StringComparison.InvariantCultureIgnoreCase) ||
                            col.Equals("dateTime", StringComparison.InvariantCultureIgnoreCase) ||
                            col.Equals("timestamp", StringComparison.InvariantCultureIgnoreCase))
                    )
                    {
                        Headers.Add(CreateDateColumn(col, colCount));
                        Headers.Add(CreateStringColumn("Date", colCount, modelName, aspectGetter, true));
                        dateColOrdinal = colCount;
                    }
                    else
                    {
                        Headers.Add(CreateStringColumn(col, colCount, modelName, aspectGetter));
                    }
                }


                colCount++;
            }

            return propList;
        }
    }
}
