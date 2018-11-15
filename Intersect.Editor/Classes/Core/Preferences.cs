using Intersect.Config;
using Microsoft.Win32;

namespace Intersect.Editor
{
    public static class Preferences
    {
        public static void SavePreference(string key, string value)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey.CreateSubKey("IntersectEditor");
            regkey = regkey.OpenSubKey("IntersectEditor", true);
            regkey.CreateSubKey(ClientOptions.ServerHost + ":" + ClientOptions.ServerPort);
            regkey = regkey.OpenSubKey(ClientOptions.ServerHost + ":" + ClientOptions.ServerPort, true);
            regkey.SetValue(key, value);
        }

        public static string LoadPreference(string key)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectEditor", false);
            if (regkey == null)
            {
                return "";
            }
            regkey = regkey.OpenSubKey(ClientOptions.ServerHost + ":" + ClientOptions.ServerPort);
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
    }
}