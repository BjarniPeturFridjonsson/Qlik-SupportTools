using Eir.Common.IO;
using System;
using System.Collections.Generic;

namespace Gjallarhorn.Db
{
    public class GjallarhornDb
    {
        private readonly DynaSql _dynaSql;

        public GjallarhornDb(IFileSystem fileSystem)
        {
            var dbLocation = fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gjallarhorn.sqllite");
            _dynaSql = new DynaSql($"Data Source={dbLocation};Version=3;");//will autocreate db if missing.
        }

        public void EnsureMonitorTableExists(string tableName)
        {
            if (!_dynaSql.DbTableExists(tableName))
            {
                var cmd = $"create table if not exists {tableName} (id text PRIMARY KEY, created text not null, sentDate text, data text)";
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



    }
}
