using System;
using System.IO;
using System.Linq;

namespace Intersect.Localization
{
    public static class Strings
    {
        public enum IntersectComponent
        {
            Client = 0,
            Editor,
            Server,
            Migrator
        }

        private static Language DefaultLanguage;
        private static Language SelectedLanguage;

        public static void Init(IntersectComponent component, string language)
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            var langDir = Path.Combine("resources", "languages");
            if (!Directory.Exists(langDir)) Directory.CreateDirectory(langDir);
            string defaultFile = "";
            string strComponent = "";
            switch (component)
            {
                case IntersectComponent.Client:
                    strComponent = "Client";
                    defaultFile = Properties.Resources.Client_English;
                    break;
                case IntersectComponent.Editor:
                    strComponent = "Editor";
                    defaultFile = Properties.Resources.Editor_English;
                    break;
                case IntersectComponent.Server:
                    strComponent = "Server";
                    defaultFile = Properties.Resources.Server_English;
                    break;
                case IntersectComponent.Migrator:
                    strComponent = "Migrator";
                    defaultFile = Properties.Resources.Migrator_English;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(component), component, null);
            }
            //If we don't have the default language file, load it from resources
            if (!File.Exists(Path.Combine(langDir, strComponent + ".English.xml")))
            {
                //Copy Client.English.xml from resources
                File.WriteAllText(Path.Combine(langDir, strComponent + ".English.xml"), defaultFile);
            }
            DefaultLanguage = new Language(Path.Combine(langDir, strComponent + ".English.xml"),false);
            if (File.Exists(Path.Combine(langDir, strComponent + "." + language + ".xml")))
            {
                SelectedLanguage = new Language(Path.Combine(langDir, strComponent + "." + language + ".xml"),false);
            }
        }

        public static void Init(string languageXml)
        {
            SelectedLanguage = new Language(languageXml, true);
        }

        public static string Get(string section, string id, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].ToString();
            }
            if (SelectedLanguage != null && SelectedLanguage.IsLoaded && SelectedLanguage.HasString(section, id))
            {
                return SelectedLanguage.GetString(section, id, args);
            }
            if (DefaultLanguage != null && DefaultLanguage.IsLoaded && DefaultLanguage.HasString(section, id))
            {
                return DefaultLanguage.GetString(section, id, args);
            }
            return $"//{section}/{id} ({string.Join(",", args.Cast<string>().ToArray())})";
        }

        public static string Get(string section, string id)
        {
            if (SelectedLanguage != null && SelectedLanguage.IsLoaded && SelectedLanguage.HasString(section, id))
            {
                return SelectedLanguage.GetString(section, id);
            }
            if (DefaultLanguage != null && DefaultLanguage.IsLoaded && DefaultLanguage.HasString(section, id))
            {
                return DefaultLanguage.GetString(section, id);
            }
            return $"//{section}/{id}";
        }
    }
}