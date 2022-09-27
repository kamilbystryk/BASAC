using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using System.IO;
using System.Reflection;
using System.Threading;
using BASAC.Devices;
using System.Data;

namespace BASAC.Database
{
    public class IotDevicesDataBase
    {
        private static IotDevicesDataBase _instance;
        private static readonly object Lock = new object();
        private string _DbFileName;
        private string _DbFilePath;

        public static IotDevicesDataBase Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new IotDevicesDataBase();
                }
                return _instance;
            }
        }

        public IotDevicesDataBase()
        {
            _DbFileName = IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.IotDevdatabasename);

            _DbFilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()).ToString();
            //_DbFilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            CreateIotDevicesBase();

        }

        public void CreateIotDevicesBase()
        {
            var path = Path.Combine(_DbFilePath, _DbFileName);

            bool exists = File.Exists(path);
            if (!exists)
            {
                SqliteConnection.CreateFile(path);
                CreateTable();
            }
        }

        public void CreateTable()
        {
            var path = Path.Combine(_DbFilePath, _DbFileName);

            bool exists = File.Exists(path);
            if (!exists)
            {

                SqliteConnection.CreateFile(path);
            }
            var connection = GetConnection();

            string sqlins = "CREATE TABLE " + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableDev)
                + " ("
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_ID) + " int, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_MAC) + " nvarchar(32), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_RegisterId) + " int, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_Name) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_Desc) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_CoilType) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_value) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_deleted) + " boolean, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_acq) + " boolean, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_local) + " int, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_devicetype) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_supply) + " int, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_CommCh) + " int"
                + ")";
            ExecuteNonQuery(connection, sqlins);
            sqlins = "CREATE INDEX " + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_ID) + " ON "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableDev)
                + "("
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_ID) + ")";
            ExecuteNonQuery(connection, sqlins);

            //tabela pomocnicza lokalizacji
            sqlins = "CREATE TABLE " + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2Nam)
                + " (" 
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamId) + " int, "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamName) + " nvarchar(255), "
                + IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamDesc) + " nvarchar(255)"
                + ")";
            ExecuteNonQuery(connection, sqlins);

            DBAddLocal(new DeviceLocal(0, "Home", "default"));

        }

        private int DBAddLocal(DeviceLocal deviceLocal)
        {
            int id = DBGetLocalMaxId() + 1;
            String _tablename = IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2Nam);
            String _id = IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamId);
            string _nam = IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamName);
            string _comm = IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamDesc);
            string sqlins = string.Format("INSERT INTO {0} ( {1}, {2}, {3}) " +
                                                           "VALUES ('{4}', '{5}', '{6}')",
                                           _tablename, _id, _nam, _comm,
                                          id, deviceLocal.Name, deviceLocal.Description);
            var connection = GetConnection();
            ExecuteNonQuery(connection, sqlins);
            return id;
        }

        public int DBGetLocalMaxId()
        {
            var query = String.Format("SELECT MAX({1}) FROM {0}",
                                      IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2Nam),
                                      IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableLoc2NamId));
            return QueryTableInt(query);
        }

        public List<IoTDevice> DBGetAllIoTDevices()
        {
            int i = 1;
            var ob = DBGetObjIDList();
            List<IoTregister> oblist = new List<IoTregister>();
            List<IoTDevice> devlist = new List<IoTDevice>();
            foreach (var item in ob)
            {
                oblist.Add(DBGetRegisterById(item));
            }

            foreach (var item in oblist)
            {
                if (item.Name.ToUpper().StartsWith("DEV"))
                {
                    IoTDevice temp = new IoTDevice(item.Id, item.MAC, 0, false, new List<IoTregister>(), item.Localisation, item.DeviceType, item.Supply);
                    List<IoTregister> tempo = new List<IoTregister>();
                    foreach (var items in oblist)
                    {
                        if (items.MAC.Equals(item.MAC))
                        {
                            tempo.Add(items);
                        }
                    }
                    temp.Registers = tempo;
                    devlist.Add(temp);
                    i++;
                }
            }
            return devlist;
        }

        public List<long> DBGetObjIDList()
        {
            String query = String.Format("SELECT {1} FROM {0} WHERE {2} IS NOT 'True'",
                IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableDev),
                IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_ID),
                IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_deleted),
                "False");
            return QueryTable(query);
        }

   
        public IoTregister DBGetRegisterById(long id)
        {
            IoTregister an = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM {0} WHERE {2} = {1}",
                        IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.TableDev),
                        id,
                        IotDevicesDataBaseModifier.GetName(IotDevicesDataBaseModifierCode.Dev_ID));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            an = new IoTregister(reader.GetString(2), reader.GetInt32(1), reader.GetInt32(3),
                                    reader.GetString(4), reader.GetString(5), reader.GetString(6),
                                    ((reader.GetString(7) as string == "True") ? true : false), reader.GetString(8), 
                                    ((reader.GetString(9) as string == "True") ? true : false),
                            reader.GetInt32(10), (BASAC.Devices.DeviceType)reader.GetInt32(11), (
                                    BASAC.Devices.SupplyEnumeration)reader.GetInt32(12), (BASAC.Devices.ComChEnumeration)reader.GetInt32(13));
                        }
                        reader.Close();
                    }
                }
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (an != null && an.Id != id)
                an = null;


            return an;
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
        private int QueryTableInt(string query)
        {
            int list = 0;
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
        private List<string> QueryTableListString(string query)
        {
            var list = new List<string>();
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
                            list.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return list;
        }
        private String QueryTableString(string query)
        {
            String list = "";
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
                                list = (reader.GetString(0));
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
                return "";
            }
        }


        private List<long> QueryTable(string query)
        {
            var list = new List<long>();
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
                                list.Add(reader.GetInt32(0));
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
            { return null; }
        }

    }
}