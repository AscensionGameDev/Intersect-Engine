using Intersect.Configuration;

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
            regkey.CreateSubKey(ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port);
            regkey = regkey.OpenSubKey(
                ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port, true
            );

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

            regkey = regkey.OpenSubKey(ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port);
            if (regkey == null)
            {
                return "";
            }

            var value = (string) regkey.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return value;
        }

    }

}
