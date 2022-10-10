﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Managers.MySQLManager
{
    static class SqlDatabaseManager
    {

        public static string SERVER = "127.0.0.1";
        public static string UID = "ancient";
        public static string PWD = "XcSshE5fTGuBtJ9f";
        public static string DB = "ancient";
        public static uint PORT = 3309;
        public static bool Initialized = false;


        public static void Initialize()
        {
            GenerateConnectionString();
            Initialized = true;
            using (var client = GetClient())
            {
                client.ExecuteNonQuery("SELECT 1");
            }
        }

        public static SqlDatabaseClient GetClient()
        {
            var Connection = new MySqlConnection(GenerateConnectionString());
            Connection.Open();
            return new SqlDatabaseClient(Connection);
        }

        public static string GenerateConnectionString()
        {
            if (ConnectionString == "")
            {
                MySqlConnectionStringBuilder ConnectionStringBuilder = new MySqlConnectionStringBuilder();
                ConnectionStringBuilder.Server = SERVER;
                ConnectionStringBuilder.Port = PORT;
                ConnectionStringBuilder.UserID = UID;
                ConnectionStringBuilder.Password = PWD;
                ConnectionStringBuilder.Database = DB;
                ConnectionStringBuilder.ConvertZeroDateTime = true;
                ConnectionStringBuilder.Pooling = true;
                ConnectionStringBuilder.MaximumPoolSize = 150;
                ConnectionStringBuilder.SslMode = MySqlSslMode.None;
                ConnectionString = ConnectionStringBuilder.ToString();
            }
            return ConnectionString;
        }

        public static string ConnectionString = "";

    }
}