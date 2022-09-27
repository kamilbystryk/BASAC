using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using Mono.Data.Sqlite;

namespace BASAC.Database
{
    public class EventDataBase
    {
        private string _DbFileName;
        private string _DbFilePath;
        private static readonly object Lock = new object();
        private static EventDataBase _instance;


        public static EventDataBase Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new EventDataBase();
                }
                return _instance;
            }
        }


        public EventDataBase()
        {
            _DbFileName = EventDataBaseModifier.GetName(EventDataBaseModifierCode.EventDataBaseName);
            _DbFilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()).ToString();
            CreateDB();
        }

        private void CreateDB()
        {
            var path = Path.Combine(_DbFilePath, _DbFileName);

            bool exists = File.Exists(path);
            if (!exists)
            {

                SqliteConnection.CreateFile(path);
            }
            var connection = GetConnection();

            if (!exists)
            {
                string sql_user = "CREATE TABLE " + EventDataBaseModifier.GetName(EventDataBaseModifierCode.TableEvent)+ " (" 
                    + EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Id) + " int, "
                    + EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Type) + " int, "
                    + EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Date) + " datetime, "
                    + EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Local) + " int, "
                    + EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Value) + " nvarchar(255)"
                    + ")";
                ExecuteNonQuery(connection, sql_user);
            }
        }

        internal void AddEvent(int type, string date, int location, string value)
        {
            int id = (int)getLastId() + 1;
            string _tablename = EventDataBaseModifier.GetName(EventDataBaseModifierCode.TableEvent);
            string _id = EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Id);
            string _type = EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Type);
            string _date = EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Date);
            string _local = EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Local);
            string _value = EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Value);


            string sqlins = string.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}, {5}) VALUES ('{6}', '{7}', '{8}', '{9}', '{10}')",
                _tablename,
                _id, _type, _date, _local, _value,
                id, type, date, location, value);
            var connection = GetConnection();
            ExecuteNonQuery(connection, sqlins);
        }

        private long getLastId()
        {
            var query = String.Format("SELECT MAX({1}) FROM {0}", EventDataBaseModifier.GetName(EventDataBaseModifierCode.TableEvent), EventDataBaseModifier.GetName(EventDataBaseModifierCode.Event_Id));
            return QueryTableLong(query);

        }

        private DateTime Makedata(string sdata)
        {
            DateTime dtdate = new DateTime();
            try
            {
                dtdate = DateTime.ParseExact(sdata, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                dtdate = DateTime.ParseExact("1900-01-01 00:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            return dtdate;
        }


        private long QueryTableLong(string query)
        {
            long list = 0;
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list = (reader.GetInt32(0));
                            }
                            reader.Close();
                        }
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                return list;
            }
            catch
            {
                return 0;
            }

        }
        public SqliteConnection GetConnection()
        {
            var path = Path.Combine(_DbFilePath, _DbFileName);
            var conn = new SqliteConnection("Data Source=" + path);
            return conn;
        }

        protected void ExecuteNonQuery(SqliteConnection connection, string sql)
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

    }
}
