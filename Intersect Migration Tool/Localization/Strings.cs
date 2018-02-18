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



        public static void Load(string language)
        {
            if (File.Exists(Path.Combine("resources", "languages", "Migrator." + language + ".json")))
            {
                Dictionary<string, Dictionary<string, object>> strings =
                    new Dictionary<string, Dictionary<string, object>>();
                strings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(
                    File.ReadAllText(Path.Combine("resources", "languages", "Migrator." + language + ".json")));
                Type type = typeof(Strings);

                var fields = new List<Type>();
                fields.AddRange(type.GetNestedTypes(System.Reflection.BindingFlags.Static |
                                                    System.Reflection.BindingFlags.Public));

                foreach (var p in fields)
                {
                    if (strings.ContainsKey(p.Name))
                    {
                        var dict = strings[p.Name];
                        foreach (var p1 in p.GetFields(System.Reflection.BindingFlags.Static |
                                                       System.Reflection.BindingFlags.Public))
                        {
                            if (dict.ContainsKey(p1.Name))
                            {
                                if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                                {
                                    p1.SetValue(null, new LocalizedString((string)dict[p1.Name]));
                                }
                                else if (p1.GetValue(null).GetType() == typeof(LocalizedString[]))
                                {
                                    string[] values = ((JArray)dict[p1.Name]).ToObject<string[]>();
                                    List<LocalizedString> list = new List<LocalizedString>();
                                    for (int i = 0; i < values.Length; i++)
                                        list.Add(new LocalizedString(values[i]));
                                    p1.SetValue(null, list.ToArray());
                                }
                                else if (p1.GetValue(null).GetType() == typeof(Dictionary<int, LocalizedString>))
                                {
                                    var dic = new Dictionary<int, LocalizedString>();
                                    Dictionary<int, string> values = ((JObject)dict[p1.Name]).ToObject<Dictionary<int, string>>();
                                    foreach (var val in values)
                                    {
                                        dic.Add(val.Key, val.Value);
                                    }
                                    p1.SetValue(null, dic);
                                }
                                else if (p1.GetValue(null).GetType() == typeof(Dictionary<string, LocalizedString>))
                                {
                                    var dic = new Dictionary<string, LocalizedString>();
                                    Dictionary<string, string> values = ((JObject)dict[p1.Name]).ToObject<Dictionary<string, string>>();
                                    foreach (var val in values)
                                    {
                                        dic.Add(val.Key, val.Value);
                                    }
                                    p1.SetValue(null, dic);
                                }
                            }
                        }
                    }
                }
            }
            Save(language);
        }

        public static void Save(string language)
        {
            Dictionary<string, Dictionary<string, object>> strings =
                new Dictionary<string, Dictionary<string, object>>();
            Type type = typeof(Strings);
            var fields = type.GetNestedTypes(System.Reflection.BindingFlags.Static |
                                             System.Reflection.BindingFlags.Public);
            foreach (var p in fields)
            {
                var dict = new Dictionary<string, object>();
                foreach (var p1 in p.GetFields(System.Reflection.BindingFlags.Static |
                                               System.Reflection.BindingFlags.Public))
                {
                    if (p1.GetValue(null).GetType() == typeof(LocalizedString))
                    {
                        dict.Add(p1.Name, ((LocalizedString)p1.GetValue(null)).ToString());
                    }
                    else if (p1.GetValue(null).GetType() == typeof(LocalizedString[]))
                    {
                        string[] values = ((LocalizedString[])p1.GetValue(null)).Select(x => x.ToString()).ToArray();
                        dict.Add(p1.Name, values);
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
                            dic.Add(val.Key, val.Value.ToString());
                        }
                        dict.Add(p1.Name, dic);
                    }
                }
                strings.Add(p.Name, dict);
            }
            File.WriteAllText(Path.Combine("resources", "languages", "Migrator." + language + ".json"), JsonConvert.SerializeObject(strings, Formatting.Indented));
        }
    }
}