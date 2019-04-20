using System;
using System.Collections.Generic;

namespace Gjallarhorn.SenseLogReading
{
    public interface IBaseLog
    {
        long Read();
        Action<LogRow> OnNewRowAction { set; }
        LogFileDto LogFile { get; set; }
    }

    public class LogFileDto
    {
        public string MachineName { get; set; }
        public string LogBaseName { get; set; }
        public string DatapointBaseName { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsRolledOver { get; set; }
        public long CurrentFilePosition { get; set; }
        public string LastReadFileName { get; set; }
        public Action<LogRow> RowEmitter { get; set; }
        public List<ColumnInfo> DisplayColumns { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastWrite { get; set; }
        public SenseLogBaseTypes LogType { get; set; }
        public SenseLogSubTypes LogSubType { get; set; }
        public string SeverityFilter { get; set; }
        public bool TypeEnabled { get; set; }
    }

        public class LogRow
        {
            private readonly string[] _arr;

            /// <summary>
            /// The reference to the parent object where you can get the headers from and other info about the file being read.
            /// </summary>
            public LogFileDto LogFileDto { get; set; }

            public LogRow(int columnCount)
            {
                _arr = new string[columnCount];
            }

            public LogRow(string[] values)
            {
                _arr = values;
            }

            public string this[int columnIndex]
            {
                get => _arr[columnIndex];
                set => _arr[columnIndex] = value;
            }

            /// <summary>
            /// Returns the internal array of column data.
            /// </summary>
            /// <returns></returns>
            public string[] ToArray()
            {
                return _arr;
            }
        }

    public class ColumnInfo
    { /// <summary>
      /// Generic index for the column. All reads should go for this index.
      /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Columns move around when the file gets renamed to blablayyyyMMdd.log 
        /// <para>So this field is the true index</para>
        /// </summary>
        public int IndexInFile { get; set; }
        public string HeaderName { get; set; }
        public string DatapointName { get; set; }
        public bool IsFilterColumn { get; set; }

        public ColumnInfo()
        {
            IndexInFile = -1;
            HeaderName = string.Empty;
            DatapointName = string.Empty;
        }

        public ColumnInfo(int index, string headername, string datapointname)
        {
            Index = index;
            IndexInFile = -1;
            HeaderName = headername;
            DatapointName = datapointname;
        }

        public ColumnInfo(int index, int indexInFile, string headername, string datapointname)
        {
            Index = index;
            IndexInFile = indexInFile;
            HeaderName = headername;
            DatapointName = datapointname;
        }
        public ColumnInfo(int index, int indexInFile, string headername, string datapointname, bool isFilterColumn)
        {
            Index = index;
            IndexInFile = indexInFile;
            HeaderName = headername;
            DatapointName = datapointname;
            IsFilterColumn = isFilterColumn;
        }
    }
}
