using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Migration.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Migration.Localization
{
    public struct LocalizedString
    {
        private string _mValue;

        public LocalizedString(string value)
        {
            _mValue = value;
        }

        public static implicit operator LocalizedString(string value)
        {
            return new LocalizedString(value);
        }

        public static implicit operator string(LocalizedString str)
        {
            return str._mValue;
        }

        public override string ToString()
        {
            return _mValue;
        }

        public string ToString(params object[] args)
        {
            try
            {
                if (args.Length == 0) return _mValue;
                return string.Format(_mValue, args);
            }
            catch (FormatException)
            {
                return "Format Exception!";
            }
        }
    }
    public static class Strings
    {
        public struct Characters
        {
            public static LocalizedString no = @"n";
            public static LocalizedString yes = @"y";
        }

        public struct Exceptions
        {
            public static LocalizedString errorcaught = @"The Intersect Migration tool has encountered an error and must close. Error information can be found in resources/migration_errors.log. Press any key to exit.";
        }

        public struct Intro
        {
            public static LocalizedString cancelling = @"Cancelling Upgrade";
            public static LocalizedString confirmdirectory = @"Make sure you are launching this tool from the server directory.";
            public static LocalizedString confirmupgrade = @"Do you want to Upgrade? ({00}/{01})";
            public static LocalizedString nodatabase = @"Failed to find an Intersect database to Upgrade.";
            public static LocalizedString outofdate = @"Out-of-date database found. Version: {00} Latest Version: {01}";
            public static LocalizedString purpose = @"Use this to update your database when deploying newer versions of Intersect.";
            public static LocalizedString starting = @"Starting Upgrade Process";
            public static LocalizedString support = @"For help, support, and updates visit: https://ascensiongamedev.com";
            public static LocalizedString tooloutofdate = @"Is this migration tool up to date?";
            public static LocalizedString uptodate = @"Database does not appear to be out of date. Version: {00} Latest Version: {01}";
        }

        public struct Main
        {
            public static LocalizedString exit = @"Press any key to exit.";
            public static LocalizedString tagline = @"                          free 2d orpg engine";
            public static LocalizedString title = @"Intersect Migration Tool";
            public static LocalizedString version = @"Version {00}";
        }

        public struct Upgrade
        {
            public static LocalizedString backupinfo = @"Version {00} backup is located at resources/intersect_v{01}_{02}.db in case of problems.";
            public static LocalizedString noinstructions = @"Upgrade instructions could not be found!";
            public static LocalizedString updated = @"Database successfully updated to version {00}.";
        }

        public struct Migration
        {
            public static LocalizedString newdatabase = @"Starting in Beta 5, Intersect introduces several database changes.
 - Player data and server data are now being split into multiple databases.
 - You may choose Sqlite or Mysql as the engine for each database.

We highly recommend choosing Sqlite (a local file) for both databases. 

Using a networked Mysql instance for player data is okay but Mysql is highly discouraged for game data.

The following prompts will walk you through required configuration steps and bring your new databases online.";

            public static LocalizedString presstocontinue = @"Press any key to continue...";
            public static LocalizedString playerdb = @"Let's start with the player database, which database engine would you like to use?";
            public static LocalizedString playerdbsqlite = @"   [1] Sqlite (Local, easy, no setup required!)";
            public static LocalizedString playerdbmysql = @"   [2] Mysql (Networked, and for advanced users only!)";
            public static LocalizedString gamedbsqlite = @"   [1] Sqlite (Local, easy, no setup required!)";
            public static LocalizedString gamedbmysql = @"   [2] Mysql (Networked, for advanced users only, and strongly discouraged for game data!)";
            public static LocalizedString gamedb = @"Now we need to setup the game database, this contains big chunks of data like maps. Which database engine would you like to use? (Sqlite highly recommended!)";
            public static LocalizedString invalidinput = @"Invalid input!";
            public static LocalizedString entermysqlinfo = @"Please enter your Mysql connection parameters:";
            public static LocalizedString mysqlhost = @"Host: ";
            public static LocalizedString mysqlport = @"Port: ";
            public static LocalizedString mysqldatabase = @"Database: ";
            public static LocalizedString mysqluser = @"User: ";
            public static LocalizedString mysqlpass = @"Password: ";
            public static LocalizedString mysqlconnecting = @"Please wait, attempting to connect to database...  ";
            public static LocalizedString mysqlconnected = @"Connected!";
            public static LocalizedString mysqlnotempty = @"Database must be empty before migration! Please delete any tables before proceeding!";
            public static LocalizedString mysqlconnectionerror = @"Error opening db connection! Error: {00}";
            public static LocalizedString mysqltryagain = @"Would you like to try entering your connection info again? (y/n)  ";
            public static LocalizedString tryagaincharacter = @"y";
            public static LocalizedString mysqlsetupcancelled = @"Mysql setup cancelled.";
            public static LocalizedString migratingpleasewait = @"Starting migration, please wait! This might take several minutes....";
            public static LocalizedString upgradecomplete = @"Database migrated successfully! Feel free to delete this migration tool. Press any key to exit!";
        }


        public static void Load()
        {
            if (File.Exists(Path.Combine("resources", "migrator_strings.json")))
            {
                var strings = new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(Path.Combine("resources", "migrator_strings.json")));
                var type = typeof(Strings);

                var fields = new List<Type>();
                fields.AddRange(type.GetNestedTypes(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));

                foreach (var p in fields)
                {
                    if (!strings.ContainsKey(p.Name)) continue;
                    var dict = strings[p.Name];
                    foreach (var fieldInfo in p.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                    {
                        var fieldValue = fieldInfo.GetValue(null);
                        if (!dict.ContainsKey(fieldInfo.Name.ToLower())) continue;
                        if (fieldValue is LocalizedString)
                        {
                            fieldInfo.SetValue(null, new LocalizedString((string)dict[fieldInfo.Name.ToLower()]));
                        }
                        else if (fieldValue is Dictionary<int, LocalizedString>)
                        {
                            var existingDict = (Dictionary<int, LocalizedString>)fieldInfo.GetValue(null);
                            var values = ((JObject)dict[fieldInfo.Name]).ToObject<Dictionary<int, string>>();
                            var dic = values.ToDictionary<KeyValuePair<int, string>, int, LocalizedString>(val => val.Key, val => val.Value);
                            foreach (var val in dic)
                            {
                                existingDict[val.Key] = val.Value;
                            }
                        }
                        else if (fieldValue is Dictionary<string, LocalizedString>)
                        {
                            var existingDict = (Dictionary<string, LocalizedString>)fieldInfo.GetValue(null);
                            var pairs = ((JObject)dict[fieldInfo.Name])?.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
                            foreach (var pair in pairs)
                            {
                                if (pair.Key == null) continue;
                                existingDict[pair.Key.ToLower()] = pair.Value;
                            }
                        }
                    }
                }
            }
            Save();
        }

        public static void Save()
        {
            var strings = new Dictionary<string, Dictionary<string, object>>();
            var type = typeof(Strings);
            var fields = type.GetNestedTypes(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var p in fields)
            {
                var dict = new Dictionary<string, object>();
                foreach (var p1 in p.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                {
                    if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                    {
                        dict.Add(p1.Name.ToLower(), ((LocalizedString)p1.GetValue(null)).ToString());
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<int, LocalizedString>))
                    {
                        var dic = new Dictionary<int, string>();
                        foreach (var val in (Dictionary<int, LocalizedString>)p1.GetValue(null))
                        {
                            dic.Add(val.Key, val.Value.ToString());
                        }
                        dict.Add(p1.Name, dic);
                    }
                    else if (p1.GetValue(null).GetType() == typeof(Dictionary<string, LocalizedString>))
                    {
                        var dic = new Dictionary<string, string>();
                        foreach (var val in (Dictionary<string, LocalizedString>)p1.GetValue(null))
                        {
                            dic.Add(val.Key.ToLower(), val.Value.ToString());
                        }
                        dict.Add(p1.Name, dic);
                    }
                }
                strings.Add(p.Name, dict);
            }

            var languageDirectory = Path.Combine("resources");
            if (!Directory.Exists(languageDirectory))
            {
                Directory.CreateDirectory(languageDirectory);
            }
            File.WriteAllText(Path.Combine(languageDirectory, "migrator_strings.json"), JsonConvert.SerializeObject(strings, Formatting.Indented));
        }
    }
}