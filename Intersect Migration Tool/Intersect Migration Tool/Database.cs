using System;
using System.IO;
using System.Xml;
using Intersect.Localization;
using Intersect.Logging;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_2;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_3;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_4;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_6;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7;
using Mono.Data.Sqlite;

namespace Intersect_Migration_Tool
{
    public static class Database
    {
        public const int DbVersion = 8;
        private const string DbFilename = "resources/intersect.db";

        //Database Variables
        private const string INFO_TABLE = "info";
        private const string DB_VERSION = "dbversion";
        private static SqliteConnection _dbConnection;
        private static Object _dbLock = new Object();

        //Config Info
        public static string GetLanguageFromConfig()
        {
            if (File.Exists("resources/config.xml"))
            {
                var options = new XmlDocument();
                var ConfigXml = File.ReadAllText("resources/config.xml");
                try
                {
                    options.LoadXml(ConfigXml);
                    return GetXmlStr(options, "//Config/Language");
                }
                catch (Exception exception)
                {
                    Log.Trace(exception);
                }
            }
            return "English";
        }

        private static string GetXmlStr(XmlDocument xmlDoc, string xmlPath)
        {
            var selectSingleNode = xmlDoc.SelectSingleNode(xmlPath);
            string returnVal = "";
            if (selectSingleNode == null)
            {
            }
            else
            {
                returnVal = selectSingleNode.InnerText;
            }
            return returnVal;
        }

        //Database setup, version checking
        public static bool InitDatabase()
        {
            lock (_dbLock)
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ";Version=3");
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
                version = (long) createCommand.ExecuteScalar();
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
            File.Copy("resources/intersect.db",
                "resources/intersect_v" + GetDatabaseVersion() + "_" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") +
                ".db");
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection = null;
                _dbConnection = new SqliteConnection("Data Source=" + DbFilename + ";Version=3");
                _dbConnection.Open();
            }
            var startingVersion = GetDatabaseVersion();
            var currentVersion = GetDatabaseVersion();
            while (currentVersion < DbVersion)
            {
                switch (currentVersion)
                {
                    case 1:
                        var upgrade1 = new Upgrade1(_dbConnection);
                        upgrade1.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 2:
                        var upgrade2 = new Upgrade2(_dbConnection);
                        upgrade2.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 3:
                        var upgrade3 = new Upgrade3(_dbConnection);
                        upgrade3.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 4:
                        var upgrade4 = new Upgrade4(_dbConnection);
                        upgrade4.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 5:
                        var upgrade5 = new Upgrade5(_dbConnection);
                        upgrade5.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 6:
                        var upgrade6 = new Upgrade6(_dbConnection);
                        upgrade6.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 7:
                        var upgrade7 = new Upgrade7(_dbConnection);
                        upgrade7.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    default:
                        throw new Exception(Strings.Get("upgrade", "noinstructions"));
                }
                currentVersion = GetDatabaseVersion();
            }

            _dbConnection.Close();
            _dbConnection = null;
            Console.WriteLine(Strings.Get("upgrade", "updated", currentVersion));
            Console.WriteLine(Strings.Get("upgrade", "backupinfo", startingVersion, startingVersion,
                DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")));
        }
    }
}