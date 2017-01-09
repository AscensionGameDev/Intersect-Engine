using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Intersect_Library.Localization
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
            if (!Directory.Exists(langDir))  Directory.CreateDirectory(langDir);
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
                File.WriteAllText(Path.Combine(langDir, strComponent + ".English.xml"),defaultFile);
            }
            DefaultLanguage = new Language(Path.Combine(langDir, strComponent + ".English.xml"));
            if (File.Exists(Path.Combine(langDir, strComponent + "." + language + ".xml")))
            {
                SelectedLanguage = new Language(Path.Combine(langDir, strComponent + "." + language + ".xml"));
            }
        }

        public static string Get(string section, string id, params object[] args)
        {
            if (SelectedLanguage != null && SelectedLanguage.Loaded() && SelectedLanguage.HasString(section, id))
            {
                return SelectedLanguage.GetString(section, id, args);
            }
            if (DefaultLanguage != null && DefaultLanguage.Loaded() && DefaultLanguage.HasString(section, id))
            {
                return DefaultLanguage.GetString(section, id, args);
            }
            return "Not Found";
        }
    }
}
