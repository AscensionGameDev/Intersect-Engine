using Intersect.Config;
using Microsoft.Win32;

namespace Intersect.Editor
{
    public static class Preferences
    {
        public static void SavePreference(string key, string value)
        {
            var regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey.CreateSubKey("IntersectEditor");
            regkey = regkey.OpenSubKey("IntersectEditor", true);
            regkey.CreateSubKey(ClientOptions.Instance.Host + ":" + ClientOptions.Instance.Port);
            regkey = regkey.OpenSubKey(ClientOptions.Instance.Host + ":" + ClientOptions.Instance.Port, true);
            regkey.SetValue(key, value);
        }

        public static string LoadPreference(string key)
        {
            var regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectEditor", false);
            if (regkey == null)
            {
                return "";
            }
            regkey = regkey.OpenSubKey(ClientOptions.Instance.Host + ":" + ClientOptions.Instance.Port);
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