using System;

using Intersect.Client.Framework.Database;
using Intersect.Configuration;
using Intersect.Logging;

using Microsoft.Win32;

namespace Intersect.Client.MonoGame.Database
{
    public partial class MonoDatabase : GameDatabase
    {
        private RegistryKey GetInstanceKey(bool writable = false)
        {
            var regkey = Registry.CurrentUser?.OpenSubKey("Software", false);
            regkey = regkey?.OpenSubKey("IntersectClient", false);
            regkey = regkey?.OpenSubKey(ClientConfiguration.Instance.Host + ":" + ClientConfiguration.Instance.Port, writable);
            return regkey;
        }

        public override void DeletePreference(string key)
        {
            try
            {
                var instanceKey = GetInstanceKey(true);
                if (instanceKey?.GetValue(key) != default)
                {
                    instanceKey.DeleteValue(key);
                }
            }
            catch (Exception exception)
            {
#if DEBUG
                Log.Error(exception);
#endif
            }
        }

        public override bool HasPreference(string key) => GetInstanceKey()?.GetValue(key) != default;

        public override void SavePreference(string key, object value)
        {
            try
            {
                GetInstanceKey(true)?.SetValue(key, Convert.ToString(value));
            } catch (UnauthorizedAccessException)
            {
                Log.Error($"Unable to save preference {key} in the registry.");
                throw;
            }
        }

        public override string LoadPreference(string key) => GetInstanceKey()?.GetValue(key) as string ?? string.Empty;

        public override bool LoadConfig() => ClientConfiguration.LoadAndSave() != default;
    }
}
