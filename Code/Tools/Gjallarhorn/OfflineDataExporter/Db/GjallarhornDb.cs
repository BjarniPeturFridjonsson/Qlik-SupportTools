using Eir.Common.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace OfflineDataExporter.Db
{
    public class GjallarhornDb
    {
        private readonly DynaSql _dynaSql;
        private const string MONTHLY_STATS_TABLE_NAME = "MonthlyStats";
        private const string DATE_TIME_FORMAT_STRING = "yyyy-MM-dd hh:mm:ss";

        public string DatabaseLocation { get;}  

        public GjallarhornDb(IFileSystem fileSystem)
        {
            DatabaseLocation = fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gjallarhorn.sqllite");
            _dynaSql = new DynaSql($"Data Source={DatabaseLocation};Version=3;");//will autocreate db if missing.
        }

        public (int rowCount, DateTime lastRunDate) GetCurrentStateData()
        {
            var tables = _dynaSql.GetDbTables();
            var rowCount = 0;
            var lastExportedDate = DateTime.MinValue;
            tables.ForEach(p =>
            {
                if (p.Equals(MONTHLY_STATS_TABLE_NAME, StringComparison.InvariantCultureIgnoreCase)) return;
                    
                if (lastExportedDate == DateTime.MinValue)
                {
                    var sDate = _dynaSql.SqlExecuteScalar($"Select exportedDate from {p} order by exportedDate desc limit 1");
                    DateTime.TryParse(sDate, out lastExportedDate);
                }
                rowCount += int.Parse(_dynaSql.SqlExecuteScalar($"Select count(id) from {p} where exportedDate is null"));

            });

            return (rowCount, lastExportedDate);
        }

        public void ExportData(string path, DateTime? lastRunDate = null)
        {
            var tables = _dynaSql.GetDbTables();
            var exportDate = DateTime.Now.ToString(DATE_TIME_FORMAT_STRING);
            //var a = _dynaSql.SqlExecuteScalar("Select count(data) from SenseLogFileParserMonitor where exportedDate isnull;");
            //var b = _dynaSql.SqlExecuteScalar("Select count(data) from SenseLogFileParserMonitor;");
            //var c = _dynaSql.SqlExecuteScalar("Select count(data) from SenseLogFileParserMonitor where exportedDate is not null;");
            //var d = _dynaSql.SqlList("Select exportedDate from SenseLogFileParserMonitor");
            var where = "is null";
            if (lastRunDate.GetValueOrDefault() != DateTime.MinValue)
            {
                where = $"='{lastRunDate.GetValueOrDefault().ToString(DATE_TIME_FORMAT_STRING)}'";
            }

            tables.ForEach(p =>
            {

                if (!p.Equals(MONTHLY_STATS_TABLE_NAME, StringComparison.InvariantCultureIgnoreCase))
                {
                    var reader = _dynaSql.SqlReader($"Select data from {p} where exportedDate {where};");
                    if (reader.Rows.Count > 0)
                    {
                        foreach (DataRow row in reader.Rows)
                        {
                            using (StreamWriter file = File.CreateText(Path.Combine(path, Guid.NewGuid().ToString() + $"_{p}.json")))
                            {
                                file.Write(row[0]);
                            }
                        }
                        _dynaSql.SqlExecuteNonQuery($"update {p} set exportedDate = @exportDate", new List<DynaSql.DynaParameter>
                        {
                            new DynaSql.DynaParameter{Name = "exportDate",Value = exportDate}
                        });
                    }
                }
            });
          
        }
    }
}
