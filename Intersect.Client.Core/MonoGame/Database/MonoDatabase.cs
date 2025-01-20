using Intersect.Client.Framework.Database;
using Intersect.Configuration;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace Intersect.Client.MonoGame.Database;

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
            ApplicationContext.Context.Value?.Logger.LogError(
                exception,
                "Error occurred deleting preference {Key}",
                key
            );
        }
    }

    public override bool HasPreference(string key) => GetInstanceKey()?.GetValue(key) != default;

    public override void SavePreference<TValue>(string key, TValue value)
    {
        try
        {
            var instanceKey = GetInstanceKey();
            instanceKey.SetValue(key, Convert.ToString(value));
        }
        catch (UnauthorizedAccessException)
        {
            ApplicationContext.Context.Value?.Logger.LogError($"Unable to save preference {key} in the registry.");
            throw;
        }
    }

    public override string LoadPreference(string key) => GetInstanceKey()?.GetValue(key) as string ?? string.Empty;

    public override bool LoadConfig() => ClientConfiguration.LoadAndSave() != default;
}
