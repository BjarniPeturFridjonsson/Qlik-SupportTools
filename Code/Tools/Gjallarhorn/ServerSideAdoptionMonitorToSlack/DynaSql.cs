using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ServerSideAdoptionMonitorToSlack
{
    public class DynaSql
    {
        /// <summary>
        /// Default timeout is 15 seconds, Put 0 to wait indefinitely.
        /// </summary>
        public int SqlCommandTimeout = 15;

        public DynaSql()
        {
            SqlSearchStringMaxLength = 100;
        }

        public DynaSql(string connectionString)
        {
            SqlSearchStringMaxLength = 100;
            ConnString = connectionString;
        }
        public MySqlConnection ConnectionGet()
        {
            return new MySqlConnection(ConnString);
        }

        public string ConnString { get; set; }


        /// <summary>
        /// Executes an sql string and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlString">The sql string <para>exec myStp @myField</para></param>
        /// <param name="sqlParameters">Sql parameters for sql injection protection<para>new List&lt;DynaSql.DynaParameter&gt;{new DynaSql.DynaParameter {Name = "myField", Value = "-1"}}</para></param>
        /// <returns>number of rows or -1 if there was no execution</returns>
        public int SqlExecuteNonQuery(string sqlString, List<DynaParameter> sqlParameters = null)
        {
            if (string.IsNullOrEmpty(sqlString))
                return -1;

            using (var conn = ConnectionGet())
            {
                var cmd = PrepCmd(conn, sqlString, sqlParameters);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                conn.Close();
                return rows;
            }

        }

        /// <summary>
        /// Returns a dictionary of the 2 first fields in the recordset dictionary[rst[0],rst[1]].
        /// The first field is the key and second field is the value
        /// <para>If the key exists from before it will be ignored.</para> 
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public Dictionary<string, string> SqlDictionary(string sqlString, List<DynaParameter> sqlParameters = null)
        {
            var aReturn = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(sqlString))
                return aReturn;

            using (var conn = ConnectionGet())
            {

                if (!string.IsNullOrEmpty(sqlString))
                {
                    var cmd = PrepCmd(conn, sqlString, sqlParameters);
                    conn.Open();
                    var rst = cmd.ExecuteReader();
                    if (rst.FieldCount < 2)
                        throw new Exception("Recordset needs to contain 2 fields");
                    while (rst.Read())
                    {
                        string key = rst[0].ToString();
                        if (!aReturn.ContainsKey(key))
                            aReturn.Add(key, rst[1].ToString());
                    }
                    rst.Close();
                    conn.Close();
                }

            }
            return aReturn;
        }

        /// <summary>
        /// Similar to sqlScalar but returns a list of the first field in the sql statement (rst[0])
        /// <para>Iterates over a recordset taking the first field in every row and adds it to a list of string</para>
        /// </summary>
        /// <param name="sqlString">The sql string <para>select * from myTable where myField=@myField</para></param>
        /// <param name="sqlParameters">Sql parameters for sql injection protection<para>new List&lt;DynaSql.DynaParameter&gt;{new DynaSql.DynaParameter {Name = "MasterEmployeeId", Value = "-1"}}</para></param>
        /// <returns></returns>
        public List<string> SqlList(string sqlString, List<DynaParameter> sqlParameters = null)
        {
            var aReturn = new List<string>();
            if (string.IsNullOrEmpty(sqlString))
                return aReturn;

            using (var conn = ConnectionGet())
            {
                if (!string.IsNullOrEmpty(sqlString))
                {
                    var cmd = PrepCmd(conn, sqlString, sqlParameters);
                    conn.Open();
                    var rst = cmd.ExecuteReader();
                    while (rst.Read())
                    {
                        aReturn.Add(rst[0].ToString());
                    }
                    rst.Close();
                    conn.Close();
                }

            }
            return aReturn;
        }

        /// <summary>
        /// Returns a datatable based on the sql you are sending in.
        /// Remember that all the rows and all the fields are in the recordset
        /// so it can be slow. 
        /// </summary>
        /// <param name="sql">The sql string <para>select * from myTable where myField=@myField</para></param>
        /// <param name="sqlParameters">Sql parameters for sql injection protection<para>new List&lt;DynaSql.DynaParameter&gt;{new DynaSql.DynaParameter {Name = "MasterEmployeeId", Value = "-1"}}</para></param>
        /// <returns></returns>
        public DataTable SqlReader(string sql, List<DynaParameter> sqlParameters = null)
        {
            var rst = new DataTable();
            if (string.IsNullOrEmpty(sql))
                return rst;

            using (var conn = ConnectionGet())
            {
                var cmd = PrepCmd(conn, sql, sqlParameters);
                var oAdapter = GetDataAdapter(cmd);
                oAdapter.Fill(rst);
                conn.Close();
            }
            return rst;
        }

        private MySqlDataAdapter GetDataAdapter(MySqlCommand cmd)
        {
            return new MySqlDataAdapter(cmd);
        }

        private MySqlCommand PrepCmd(MySqlConnection conn, string sql, IEnumerable<DynaParameter> sqlParameters)
        {
            var cmd = new MySqlCommand(sql, conn);
            if (sqlParameters != null)
            {
                foreach (var param in sqlParameters)
                {
                    cmd.Parameters.AddWithValue(param.Name, (object)param.Value == null ? DBNull.Value : (object)param.Value.Trim());
                }
            }
            cmd.CommandTimeout = SqlCommandTimeout;
            return cmd;
        }




        /// <summary>
        /// Same as scalar but returns an dictionary (hashtable, indexed array)
        /// For each column in the first row of the recordset.
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public Dictionary<string, string> SqlExecuteScalarSuper(string sqlString, List<DynaParameter> sqlParameters = null)
        {
            var aReturn = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var conn = ConnectionGet())
            {
                if (!string.IsNullOrEmpty(sqlString))
                {
                    var cmd = PrepCmd(conn, sqlString, sqlParameters);
                    conn.Open();
                    var rst = cmd.ExecuteReader();
                    if (rst.HasRows)
                    {
                        rst.Read();
                        int i;
                        for (i = 0; i <= rst.FieldCount - 1; i++)
                        {
                            aReturn.Add(rst.GetName(i), rst[i].ToString());
                        }
                    }
                    rst.Close();
                    conn.Close();
                }

            }
            return aReturn;
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns>Value as string null is returned as empty string</returns>
        public string SqlExecuteScalar(string sql, List<DynaParameter> sqlParameters = null)
        {
            string value;

            using (var conn = ConnectionGet())
            {
                var cmd = PrepCmd(conn, sql, sqlParameters);
                var res = cmd.ExecuteScalar();
                if (res == null)
                    return "";
                value = res.ToString();
                conn.Close();
            }

            return value;
        }


        /// <summary>
        /// Use when youve got xml data. Like for xml statements
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public string SqlXmlGet(string sql, List<DynaParameter> sqlParameters = null)
        {
            string value;
            using (var conn = new SqlConnection(ConnString))
            {
                var cmd = new SqlCommand(sql, conn);
                if (sqlParameters != null)
                {
                    foreach (var param in sqlParameters)
                    {
                        cmd.Parameters.AddWithValue(param.Name, param.Value);
                    }
                }
                conn.Open();
                System.Xml.XmlReader rdr = cmd.ExecuteXmlReader();
                try
                {
                    var sb = new System.Text.StringBuilder();

                    rdr.Read(); while (!rdr.EOF) { sb.Append(rdr.ReadOuterXml()); }

                    value = sb.ToString();
                }
                finally { rdr.Close(); }
                conn.Close();
            }

            return value;
        }

        /// <summary>
        /// This is the max length of the search string and will be truncated without warning
        /// Don't make it too long for the sql execution string has a max length.
        /// </summary>
        public int SqlSearchStringMaxLength { get; set; }

        /// <summary>
        /// Used when creating dynamic sql statements
        /// <para>&#160;</para>
        /// <para>ImportanceWeight</para>
        /// <para> if not zero(default) will put the whole search string (not split by space) for this field only and it will be 
        /// sorted(higher number higher in the list) and added on top of the result with SQL UNION
        /// </para> <para>&#160;</para>
        /// <para>SortByOrder</para>
        /// <para>The field will be added to the sortBy part if value is larger than zero(default), 1 will be first,2 second and so forth.</para>
        /// <para>&#160;</para>
        /// <para>ExcludeFromWhere</para>
        /// <para>Will not be used in the where, only in the sort by part but if using ImportanceWeight it will not be ignored.</para>
        /// </summary>
        public struct SqlDynaField
        {
            public string FieldName;
            public int ImportanceWeight;
            public int SortByOrder;
            public bool ExcludeFromWhere;
        }

        /// <summary>
        /// creates a dynamic sql search for using in search lists. Can be used with EF.
        /// </summary>
        /// <param name="searchstring">The where part split by space and applied to all the fields in the search</param>
        /// <param name="tableName">Yup. its the name of the sql table</param>
        /// <param name="whereFields">The fields you want to search in</param>
        /// <param name="returnTop">the sql TOP statement, Use null if you want to return all results</param>
        /// <param name="selectOutputFields">default is all (select * ) otherwise a sql compliant list of sql fields.  
        /// <para>WARNING</para>
        /// <para>The fieldlist is not sql safed. MAKE SURE YOU SQL SAFE IT. That is CRITICAL if the fieldlist is dynamic.</para>
        /// </param>
        /// <returns>The sql string you can execute</returns>
        public string SqlSearchCreate(string searchstring, string tableName, List<SqlDynaField> whereFields, int? returnTop = 50, string selectOutputFields = "*")
        {
            if (string.IsNullOrEmpty(selectOutputFields))
                selectOutputFields = "*";

            //we want to make sure this does not crash because someone pasted shit long text into search boxes.
            if (searchstring.Length > SqlSearchStringMaxLength)
                searchstring = searchstring.Substring(0, SqlSearchStringMaxLength);
            const string weightColumName = "sortWeight_092F0FDE29A9482FB87665F593946C5B";
            var aSearch = searchstring.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //building the standard Sql statement
            string sqlWhereParam = aSearch.Aggregate("", (current, s) => current + (" or {0} like '%" + SqlSafe(s) + "%'")).Substring(4);
            string sqlWhere = "";
            string orderBy = "";
            string sqlTop = (returnTop != null) ? "Top " + returnTop.ToString() : "";
            var specialSearch = new List<string>();

            //we ordering so the orderBy statement is in correct order
            whereFields.Sort((s1, s2) => s1.SortByOrder.CompareTo(s2.SortByOrder));
            foreach (var field in whereFields)
            {
                if (!field.ExcludeFromWhere)
                {
                    sqlWhere += " or (" + string.Format(sqlWhereParam, SqlSafeFieldName(field.FieldName)) + ")\r\n";
                }
                if (field.ImportanceWeight > 0)
                {
                    //and now we build the special sort/search cases where the whole search string
                    //will be used on some of the search fields using UNION
                    specialSearch.Add("\r\n    Select " + sqlTop + " " + selectOutputFields + "," + field.ImportanceWeight + " as " + weightColumName + "  From " + tableName + " where " + SqlSafeFieldName(field.FieldName) + " like '%" + SqlSafe(searchstring) + "%'");
                }
                if (field.SortByOrder > 0)
                {
                    orderBy += "," + SqlSafeFieldName(field.FieldName);
                }
            }

            //now we are building the sql text
            string sSqlTotal = "";
            sSqlTotal += "select " + sqlTop + " * from (\r\n";
            sSqlTotal = specialSearch.Aggregate(sSqlTotal, (current, spec) => current + (spec + "\r\n union \r\n"));
            sSqlTotal += "    Select " + selectOutputFields + ",0 as " + weightColumName + "  From " + tableName + " where \r\n" + sqlWhere.Substring(4);
            sSqlTotal += "\r\n) as report order by " + weightColumName + orderBy;
            return sSqlTotal;
        }

        private string SqlSafeFieldName(string fieldName)
        {
            return "[" + SqlSafe(fieldName).Replace("[", "").Replace("]", "") + "]";
        }

        /// <summary>
        /// Makes a string sql safe.
        /// </summary>
        /// <param name="sqlValue"></param>
        /// <returns>The sql field that is sql safe.</returns>
        public string SqlSafe(string sqlValue)
        {
            return sqlValue.Replace("'", "''").Replace("{", "").Replace("}", "");
        }


        /// <summary>
        /// Takes the name of a stored procedure or insert and executes it and returns a 
        /// the rows affected. You need insert into or Exec in your sql string
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="sqlParameters">List of named params</param>
        /// <returns># of records affected.</returns>
        public int Execute(string sqlString, List<DynaParameter> sqlParameters)
        {


            var connection = new SqlConnection(ConnString);
            int affected;
            using (var command = new SqlCommand(sqlString, connection))
            {

                foreach (var param in sqlParameters)
                {
                    command.Parameters.AddWithValue(param.Name, param.Value);
                }

                connection.Open();
                affected = command.ExecuteNonQuery();
                connection.Close();
            }
            return affected;
        }

        /// <summary>
        /// Takes a connection string and returns the same connection string but connection to the other database.
        /// </summary>
        /// <param name="connString">Your current connection string</param>
        /// <param name="newDatabaseName">The database to connect to</param>
        /// <returns></returns>
        public string MoveToAnotherDb(string connString, string newDatabaseName)
        {

            // ReSharper disable once CollectionNeverQueried.Local
            var connBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = connString,
                InitialCatalog = newDatabaseName
            };
            return connBuilder.ConnectionString;
        }



        /// <summary>
        /// Checks if you can open the connection based on the current connection string
        /// </summary>
        /// <returns>Returns true if there was no error in opening the connection string.</returns>
        public bool ConnectionCheck()
        {
            try
            {
                using (var conn = ConnectionGet())
                {
                    conn.Open();
                    conn.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// List all the databases existing within the context of the current connection string.
        /// </summary>
        /// <returns>array of names of the databases.</returns>
        public List<string> DatabaseList()
        {
            var dbList = new List<string>();
            using (var conn = ConnectionGet())
            {
                conn.Open();
                var databases = conn.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    dbList.Add(database.Field<String>("database_name"));
                }
                conn.Close();
            }
            return dbList;
        }

        /// <summary>
        /// The parameters to be used in any sql statement.
        /// <para>example</para>
        ///  new List &lt;DynaSql.DynaParameter> { new DynaSql.DynaParameter() { Name = "@bookingId", Value = bookingId.ToString() } })
        /// </summary>
        public class DynaParameter
        {
            private string _name = "";
            /// <summary>
            /// The name of the param. can be both with and without @ param syntax. It will fix the missing @ automatically
            /// </summary>
            public string Name
            {
                get { return _name; }
                set { _name = value.Substring(0, 1) != "@" ? "@" + value : value; }
            }

            /// <summary>
            /// array of values for where in (val1,val2) type of syntax
            /// </summary>
            public List<string> Values { get; set; }
            /// <summary>
            /// single value for a parameters
            /// </summary>
            public string Value { get; set; }
        }

    }
}