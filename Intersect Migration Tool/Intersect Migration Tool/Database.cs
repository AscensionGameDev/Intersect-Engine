using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1;
using Mono.Data.Sqlite;

namespace Intersect_Migration_Tool
{
    public static class Database
    {
        private static SqliteConnection _dbConnection;
        private static Object _dbLock = new Object();
        public const int DbVersion = 2;
        private const string DbFilename = "resources/intersect.db";

        //Database Variables
        private const string INFO_TABLE = "info";
        private const string DB_VERSION = "dbversion";

        //Database setup, version checking
        public static bool InitDatabase()
        {
            lock (_dbLock)
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ",Version=3");
                    _dbConnection.Open();
                }
                return true;
            }
        }
        public static long GetDatabaseVersion()
        {
            long version = -1;
            var cmd = "SELECT " + DB_VERSION + " from " + INFO_TABLE + ";";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                version = (long)createCommand.ExecuteScalar();
            }
            return version;
        }

        private static void IncrementDatabaseVersion()
        {
            long version = GetDatabaseVersion();
            var cmd = "UPDATE " + INFO_TABLE + " SET " + DB_VERSION + " = " + (version + 1) + ";";
            using (var createCommand = _dbConnection.CreateCommand())
            {
                createCommand.CommandText = cmd;
                createCommand.ExecuteNonQuery();
            }
        }

        public static void Upgrade()
        {
            File.Copy("resources/intersect.db", "resources/intersect_v" + GetDatabaseVersion() + "_" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".db");
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection = null;
                _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ",Version=3");
                _dbConnection.Open();
            }
            var startingVersion = GetDatabaseVersion();
            var currentVersion = GetDatabaseVersion();
            while (currentVersion < DbVersion)
            {
                switch (currentVersion)
                {
                    case 1:
                        var upgrader = new Upgrade1(_dbConnection);
                        upgrader.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    default:
                        throw new Exception("Upgrade instructions could not be found!");
                }
                currentVersion = GetDatabaseVersion();
            }
            _dbConnection.Close();
            _dbConnection = null;
            Console.WriteLine("Database successfully updated to version " + currentVersion);
            Console.WriteLine("Version " + startingVersion + " backup is located at resources/intersect_v" + startingVersion + "_" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".db in case of problems.");
        }
    }
}
