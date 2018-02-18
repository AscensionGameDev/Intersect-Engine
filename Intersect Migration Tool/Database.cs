using System;
using System.IO;
using System.Xml;
using Intersect.Migration.Localization;
using Intersect.Migration.UpgradeInstructions.Upgrade_1;
using Intersect.Migration.UpgradeInstructions.Upgrade_10;
using Intersect.Migration.UpgradeInstructions.Upgrade_2;
using Intersect.Migration.UpgradeInstructions.Upgrade_3;
using Intersect.Migration.UpgradeInstructions.Upgrade_4;
using Intersect.Migration.UpgradeInstructions.Upgrade_5;
using Intersect.Migration.UpgradeInstructions.Upgrade_6;
using Intersect.Migration.UpgradeInstructions.Upgrade_7;
using Intersect.Migration.UpgradeInstructions.Upgrade_8;
using Intersect.Migration.UpgradeInstructions.Upgrade_9;
using Mono.Data.Sqlite;
using Log = Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Logging.Log;

namespace Intersect.Migration
{
    public static class Database
    {
        public const int DbVersion = 11;
        private const string DB_FILENAME = "resources/intersect.db";

        //Database Variables
        private const string INFO_TABLE = "info";

        private const string DB_VERSION = "dbversion";
        private static SqliteConnection sDbConnection;
        private static object sDbLock = new object();

        //Config Info
        public static string GetLanguageFromConfig()
        {
            if (File.Exists("resources/config.xml"))
            {
                var options = new XmlDocument();
                var configXml = File.ReadAllText("resources/config.xml");
                try
                {
                    options.LoadXml(configXml);
                    return GetXmlStr(options, "//Config/Language");
                }
                catch (Exception exception)
                {
                    Log.Trace(exception);
                }
            }
            else if (File.Exists("resources/config.json"))
            {
                //TODO: Make sure migration tool can load language from new config.json
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
            lock (sDbLock)
            {
                if (sDbConnection == null)
                {
                    sDbConnection = new SqliteConnection("Data Source=" + DB_FILENAME + ";Version=3");
                    sDbConnection.Open();
                }
                return true;
            }
        }

        public static long GetDatabaseVersion()
        {
            long version = -1;
            var cmd = "SELECT " + DB_VERSION + " from " + INFO_TABLE + ";";
            using (var createCommand = sDbConnection.CreateCommand())
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
            using (var createCommand = sDbConnection.CreateCommand())
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
            if (sDbConnection != null)
            {
                sDbConnection.Close();
                sDbConnection = null;
                sDbConnection = new SqliteConnection("Data Source=" + DB_FILENAME + ";Version=3");
                sDbConnection.Open();
            }
            var startingVersion = GetDatabaseVersion();
            var currentVersion = GetDatabaseVersion();
            while (currentVersion < DbVersion)
            {
                switch (currentVersion)
                {
                    case 1:
                        var upgrade1 = new Upgrade1(sDbConnection);
                        upgrade1.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 2:
                        var upgrade2 = new Upgrade2(sDbConnection);
                        upgrade2.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 3:
                        var upgrade3 = new Upgrade3(sDbConnection);
                        upgrade3.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 4:
                        var upgrade4 = new Upgrade4(sDbConnection);
                        upgrade4.Upgrade();
                        IncrementDatabaseVersion();
                        break;
                    case 5:
                        var upgrade5 = new Upgrade5(sDbConnection);
                        upgrade5.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 6:
                        var upgrade6 = new Upgrade6(sDbConnection);
                        upgrade6.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 7:
                        var upgrade7 = new Upgrade7(sDbConnection);
                        upgrade7.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 8:
                        var upgrade8 = new Upgrade8(sDbConnection);
                        upgrade8.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 9:
                        var upgrade9 = new Upgrade9(sDbConnection);
                        upgrade9.Upgrade();
                        currentVersion++;
                        IncrementDatabaseVersion();
                        break;
                    case 10:
                        sDbConnection.Close();
                        sDbConnection = null;
                        var upgrade10 = new Upgrade10();
                        upgrade10.Upgrade();
                        //currentVersion++;
                        //IncrementDatabaseVersion(); //TOOD: increment both db's
                        break;
                    default:
                        throw new Exception(Strings.Upgrade.noinstructions);
                }
                if (sDbConnection == null) break;
                currentVersion = GetDatabaseVersion();
            }
            if (sDbConnection != null)
            {
                sDbConnection.Close();
                sDbConnection = null;
            }
            Console.WriteLine(Strings.Upgrade.updated.ToString( currentVersion));
            Console.WriteLine(Strings.Upgrade.backupinfo.ToString( startingVersion, startingVersion,
                DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")));
        }
    }
}