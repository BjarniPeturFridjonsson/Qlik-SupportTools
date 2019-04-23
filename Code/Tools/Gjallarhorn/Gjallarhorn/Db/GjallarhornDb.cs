using Eir.Common.IO;
using System;
using System.Collections.Generic;

namespace Gjallarhorn.Db
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

        public void EnsureMonitorTableExists(string tableName)
        {
            if (!_dynaSql.DbTableExists(tableName))
            {
                var cmd = $"create table if not exists {tableName} (id text PRIMARY KEY, created text not null, sentDate text, data text);";
                _dynaSql.SqlExecuteNonQuery(cmd);
            }
        }

        public void SaveMonitorData(string monitorName, string text)
        {
            var cmd = $"insert into {monitorName} (id,created,data) values(@id,@created,@data)";
            _dynaSql.SqlExecuteNonQuery(cmd, new List<DynaSql.DynaParameter>
            {
                new DynaSql.DynaParameter{Name = "id",Value = Guid.NewGuid().ToString()},
                new DynaSql.DynaParameter{Name = "created",Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss")},
                new DynaSql.DynaParameter{Name = "data",Value = text}
            });
        }

        private bool _montlyTableChecked;
        private void EnsureMontlyStatsTableExists()
        {
            if (!_dynaSql.DbTableExists(MONTHLY_STATS_TABLE_NAME))
            {
                var cmd = $"create table if not exists {MONTHLY_STATS_TABLE_NAME} (id text PRIMARY KEY, year integer, month integer, idType integer);";
                _dynaSql.SqlExecuteNonQuery(cmd);
            }

            _montlyTableChecked = true;
        }

        public void AddToMontlyStats(Dictionary<string, int> values, int year, int month, MontlyStatsType idType)
        {
            if (!_montlyTableChecked)
            {
                EnsureMontlyStatsTableExists();
            }

            using (var conn = _dynaSql.ConnectionGet())
            {
                conn.Open();
                _dynaSql.SqlExecuteNonQuery(conn,"BEGIN TRANSACTION;");
                var cmd = $"insert or ignore into {MONTHLY_STATS_TABLE_NAME} (id, year, month, idType) values(@id, @year, @month, @idType)";
                foreach (var val in values)
                {
                    _dynaSql.SqlExecuteNonQuery(conn, cmd, new List<DynaSql.DynaParameter>
                    {
                        new DynaSql.DynaParameter{Name = "id", Value = val.Key},
                        new DynaSql.DynaParameter{Name = "year", Value = year.ToString()},
                        new DynaSql.DynaParameter{Name = "month", Value = month.ToString()},
                        new DynaSql.DynaParameter{Name = "idType",Value = ((int)idType).ToString()}
                    });
                }
                _dynaSql.SqlExecuteNonQuery(conn,"COMMIT TRANSACTION;");
                conn.Close();
            }
           
        }
    }
}
