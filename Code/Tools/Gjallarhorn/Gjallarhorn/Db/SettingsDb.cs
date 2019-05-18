using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eir.Common.IO;
using Eir.Common.Logging;

namespace Gjallarhorn.Db
{
    public class SettingsDb
    {
        private readonly DynaSql _dynaSql;
        private const string TABLE_NAME = "GjallarhornSettings";


        public SettingsDb(IFileSystem fileSystem)
        {
            var dbLocation = fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gjallarhorn.Settings.sqllite");
            _dynaSql = new DynaSql($"Data Source={dbLocation};Version=3;");//will autocreate db if missing.
        }

        public void EnsureSettingsTableExists()
        {
            string cmd = null;
            
            try
            {
                if (!_dynaSql.DbTableExists(TABLE_NAME))
                {
                    cmd = $"create table if not exists {TABLE_NAME} (key text PRIMARY KEY, value text);";
                    _dynaSql.SqlExecuteNonQuery(cmd);
                }
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed ensuring monitor table. {cmd} on path {_dynaSql.ConnString}", e);
                throw;
            }

        }

        public void SaveSettings(string key, string value)
        {
            string cmd = null;
            try
            {
                cmd = $"INSERT INTO {TABLE_NAME} (key,value) VALUES (@key, @value) ON  CONFLICT (key) DO UPDATE SET value=excluded.value;";
                _dynaSql.SqlExecuteNonQuery(cmd, new List<DynaSql.DynaParameter>
                {
                    new DynaSql.DynaParameter{Name = "key",Value = key},
                    new DynaSql.DynaParameter{Name = "value",Value = value}
                });
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed saving settings data. {cmd}", e);
                throw;
            }
        }

        public string ReadSettings(string key)
        {
            string cmd = null;
            try
            {
                cmd = $"select value from {TABLE_NAME} where key = @key";
                return _dynaSql.SqlExecuteScalar(cmd, new List<DynaSql.DynaParameter>
                {
                    new DynaSql.DynaParameter{Name = "key",Value = key}
                });
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed saving settings data. {cmd}", e);
                throw;
            }

        }
    }
}
