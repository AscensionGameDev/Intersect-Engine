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



        public static void Load()
        {
            if (File.Exists(Path.Combine("resources", "languages", "migration_lang.json")))
            {
                var strings = new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(File.ReadAllText(Path.Combine("resources", "languages", "migration_lang.json")));
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
                        if (!dict.ContainsKey(fieldInfo.Name)) continue;
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

            var languageDirectory = Path.Combine("resources", "languages");
            if (!Directory.Exists(languageDirectory))
            {
                Directory.CreateDirectory(languageDirectory);
            }
            File.WriteAllText(Path.Combine(languageDirectory, "migration_lang.json"), JsonConvert.SerializeObject(strings, Formatting.Indented));
        }
    }
}