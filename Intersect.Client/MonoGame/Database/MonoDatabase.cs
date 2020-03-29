using System;

using Intersect.Client.Framework.Database;
using Intersect.Configuration;

using Microsoft.Win32;

namespace Intersect.Client.MonoGame.Database
{

    public class MonoDatabase : GameDatabase
    {

        public override void SavePreference(string key, object value)
        {
            var regkey = Registry.CurrentUser?.OpenSubKey("Software", true);

            regkey?.CreateSubKey("IntersectClient");
            regkey = regkey?.OpenSubKey("IntersectClient", true);
            regkey?.CreateSubKey(ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port);
            regkey = regkey?.OpenSubKey(
                ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port, true
            );

            regkey?.SetValue(key, Convert.ToString(value));
        }

        public override string LoadPreference(string key)
        {
            var regkey = Registry.CurrentUser?.OpenSubKey("Software", false);
            regkey = regkey?.OpenSubKey("IntersectClient", false);
            regkey = regkey?.OpenSubKey(ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port);

            return regkey?.GetValue(key) as string ?? "";
        }

        public override bool LoadConfig()
        {
            ClientConfiguration.LoadAndSave();

            return true;
        }

    }

}
