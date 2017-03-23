using System.IO;
using IntersectClientExtras.Database;
using Microsoft.Win32;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Database
{
    public class MonoDatabase : GameDatabase
    {
        public override void SavePreference(string key, string value)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey.CreateSubKey("IntersectClient");
            regkey = regkey.OpenSubKey("IntersectClient", true);
            regkey.CreateSubKey(ServerHost + ":" + ServerPort);
            regkey = regkey.OpenSubKey(ServerHost + ":" + ServerPort, true);
            regkey.SetValue(key, value);
        }

        public override string LoadPreference(string key)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectClient", false);
            if (regkey == null)
            {
                return "";
            }
            regkey = regkey.OpenSubKey(ServerHost + ":" + ServerPort);
            if (regkey == null)
            {
                return "";
            }
            string value = (string) regkey.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return value;
        }

        public override bool LoadConfig()
        {
            if (!File.Exists(Path.Combine("resources", "config.xml")))
            {
                File.WriteAllText(Path.Combine("resources", "config.xml"), GetDefaultConfig());
                return LoadConfig();
            }
            else
            {
                string xmldata = File.ReadAllText(Path.Combine("resources", "config.xml"));
                return LoadConfigFromXml(xmldata);
            }
        }
    }
}