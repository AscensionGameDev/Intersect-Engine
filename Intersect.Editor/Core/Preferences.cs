using Intersect.Configuration;

using Microsoft.Win32;

namespace Intersect.Editor
{

    public static partial class Preferences
    {

        public static void SavePreference(string key, string value)
        {
            var regkey = Registry.CurrentUser.OpenSubKey("Software", true);

            regkey = regkey.CreateSubKey("IntersectEditor");
            regkey = regkey.CreateSubKey($"{ClientConfiguration.Instance.Host}:{ClientConfiguration.Instance.Port}");

            regkey.SetValue(key, value);
        }

        public static string LoadPreference(string key)
        {
            var regkey = Registry.CurrentUser.OpenSubKey("Software", false);
            regkey = regkey.OpenSubKey("IntersectEditor", false);
            if (regkey == null)
            {
                return string.Empty;
            }

            regkey = regkey.OpenSubKey($"{ClientConfiguration.Instance.Host}:{ClientConfiguration.Instance.Port}");
            if (regkey == null)
            {
                return string.Empty;
            }

            var value = (string) regkey.GetValue(key);
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value;
        }

    }

}
