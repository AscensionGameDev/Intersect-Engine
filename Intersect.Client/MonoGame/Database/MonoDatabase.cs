using System;
using System.Diagnostics;
using System.Linq;

using Intersect.Client.Framework.Database;
using Intersect.Configuration;
using Intersect.Logging;

using Microsoft.Win32;

namespace Intersect.Client.MonoGame.Database
{
    public partial class MonoDatabase : GameDatabase
    {
        private RegistryKey OpenOrCreateKeyPath(RegistryKey root, params string[] keyPath) =>
            keyPath.Aggregate(root, (current, nextName) => current?.CreateSubKey(nextName, true));

        private RegistryKey GetInstanceKey() => OpenOrCreateKeyPath(
            Registry.CurrentUser,
            "Software",
            "IntersectClient",
            $"{ClientConfiguration.Instance.Host}:{ClientConfiguration.Instance.Port}"
        );

        public override void DeletePreference(string key)
        {
            try
            {
                var instanceKey = GetInstanceKey();
                instanceKey?.DeleteValue(key);
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
                var instanceKey = GetInstanceKey();
                instanceKey.SetValue(key, Convert.ToString(value));
            }
            catch (UnauthorizedAccessException)
            {
                Log.Error($"Unable to save preference {key} in the registry.");
                throw;
            }
        }

        public override string LoadPreference(string key) => GetInstanceKey()?.GetValue(key) as string ?? string.Empty;

        public override bool LoadConfig() => ClientConfiguration.LoadAndSave() != default;
    }
}
