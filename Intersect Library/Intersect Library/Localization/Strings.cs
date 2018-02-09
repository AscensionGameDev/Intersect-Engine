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

        private static Language sDefaultLanguage;
        private static Language sSelectedLanguage;

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
            sDefaultLanguage = new Language(Path.Combine(langDir, strComponent + ".English.xml"), false);
            if (File.Exists(Path.Combine(langDir, strComponent + "." + language + ".xml")))
            {
                sSelectedLanguage = new Language(Path.Combine(langDir, strComponent + "." + language + ".xml"), false);
            }
        }

        public static void Init(string languageXml)
        {
            sSelectedLanguage = new Language(languageXml, true);
        }

        public static string Get(string section, string id, params object[] args)
        {
            var argStrings = args?.Select(arg => arg?.ToString());
            if (sSelectedLanguage != null && sSelectedLanguage.IsLoaded && sSelectedLanguage.HasString(section, id))
            {
                return sSelectedLanguage.GetString(section, id, argStrings?.Cast<object>().ToArray() ?? new object[] { });
            }
            if (sDefaultLanguage != null && sDefaultLanguage.IsLoaded && sDefaultLanguage.HasString(section, id))
            {
                return sDefaultLanguage.GetString(section, id, argStrings?.Cast<object>().ToArray() ?? new object[]{});
            }
            return $"//{section}/{id} ({string.Join(",", argStrings?.ToArray() ?? new string[]{})})";
        }

        public static string Get(string section, string id)
        {
            if (sSelectedLanguage != null && sSelectedLanguage.IsLoaded && sSelectedLanguage.HasString(section, id))
            {
                return sSelectedLanguage.GetString(section, id);
            }
            if (sDefaultLanguage != null && sDefaultLanguage.IsLoaded && sDefaultLanguage.HasString(section, id))
            {
                return sDefaultLanguage.GetString(section, id);
            }
            return $"//{section}/{id}";
        }
    }
}