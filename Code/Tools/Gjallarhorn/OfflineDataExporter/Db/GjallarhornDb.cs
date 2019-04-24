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


        public GjallarhornDb(IFileSystem fileSystem)
        {
            var dbLocation = fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gjallarhorn.sqllite");
            _dynaSql = new DynaSql($"Data Source={dbLocation};Version=3;");//will autocreate db if missing.
        }

        public (int rowCount, DateTime lastRunDate) GetCurrentStateData()
        {
            var tables = _dynaSql.GetDbTables();
            var rowCount = 0;
            var lastExportedDate = DateTime.MinValue;
            tables.ForEach(p =>
            {

                if (lastExportedDate == DateTime.MinValue)
                {
                    var sDate = _dynaSql.SqlExecuteScalar($"Select top 1 exportedDate from {p} order by exportedDate desc");
                    DateTime.TryParse(sDate, out lastExportedDate);
                }
                rowCount += int.Parse(_dynaSql.SqlExecuteScalar($"Select count(id) from {p} where exportedDate is null"));

            });

            return (rowCount, lastExportedDate);
        }

        public void ExportData(string path)
        {
            var tables = _dynaSql.GetDbTables();
            var exportDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
      
            tables.ForEach(p =>
            {

                if (!p.Equals(MONTHLY_STATS_TABLE_NAME, StringComparison.InvariantCultureIgnoreCase))
                {
                    var reader = _dynaSql.SqlReader($"Select data from {p} where exportedDate is null");
                    if (reader.Rows.Count > 0)
                    {
                        foreach (DataRow row in reader.Rows)
                        {
                            using (StreamWriter file =
                                File.CreateText(Path.Combine(path, Guid.NewGuid().ToString() + $"_{p}.json")))
                            {
                                file.Write(row[0]);
                            }
                        }

                        _dynaSql.SqlExecuteNonQuery($"update {p} set exportedDate ='@exportDate'",new List<DynaSql.DynaParameter>
                        {
                            new DynaSql.DynaParameter{Name = "exportDate",Value = exportDate}
                        });


                    }
                }
            });
          
        }
        
    }
}
