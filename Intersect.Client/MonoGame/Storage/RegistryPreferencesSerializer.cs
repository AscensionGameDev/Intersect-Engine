using Intersect.Client.Framework;
using Intersect.Client.Framework.Storage;
using Intersect.Configuration;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Intersect.Client.MonoGame.Storage
{
    internal static class RegistryTypeExtensions
    {
        private static readonly IDictionary<Type, RegistryValueKind> RegistryTypeMap =
            new ReadOnlyDictionary<Type, RegistryValueKind>(
                new Dictionary<Type, RegistryValueKind>
                {
                    {typeof(byte), RegistryValueKind.DWord},
                    {typeof(sbyte), RegistryValueKind.DWord},
                    {typeof(ushort), RegistryValueKind.DWord},
                    {typeof(short), RegistryValueKind.DWord},
                    {typeof(uint), RegistryValueKind.DWord},
                    {typeof(int), RegistryValueKind.DWord},
                    {typeof(bool), RegistryValueKind.DWord},
                    {typeof(char), RegistryValueKind.DWord},
                    {typeof(ulong), RegistryValueKind.QWord},
                    {typeof(long), RegistryValueKind.QWord},
                    {typeof(float), RegistryValueKind.Binary},
                    {typeof(double), RegistryValueKind.Binary},
                    {typeof(decimal), RegistryValueKind.Binary},
                    {typeof(string), RegistryValueKind.String}
                }
            );

        public static RegistryValueKind ToRegistryType(this Type type)
        {
            if (type.IsArray)
            {
                if (type.GetElementType() == typeof(string))
                {
                    return RegistryValueKind.MultiString;
                }

                return RegistryValueKind.Binary;
            }

            if (RegistryTypeMap.TryGetValue(type, out var registryValueKind))
            {
                return registryValueKind;
            }

            return RegistryValueKind.Binary;
        }
    }

    /// <summary>
    /// Registry implementation for preferences serialization.
    /// </summary>
    public class RegistryPreferencesSerializer : IPreferencesSerializer
    {
        private const string FallbackParentKey = "IntersectClient";

        private IGameContext GameContext { get; }

        public RegistryPreferencesSerializer(IGameContext gameGameContext, bool enabled = true)
        {
            GameContext = gameGameContext;
            Enabled = enabled;
        }

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// The parent registry key to use (child of HKCU/SOFTWARE).
        /// </summary>
        public string ParentKey { get; set; } = FallbackParentKey;

        private string SanitizedParentKeyName
        {
            get
            {
                var parentKeyName = ParentKey?.Trim() ?? string.Empty;
                parentKeyName = string.IsNullOrWhiteSpace(parentKeyName) ? FallbackParentKey : parentKeyName;
                return parentKeyName;
            }
        }

        private string GetServerKeyName(IClientConfiguration clientConfiguration) =>
            $"{clientConfiguration.Host}:{clientConfiguration.Port}";

        /// <inheritdoc />
        public bool Deserialize(IPreferences destinationPreferences)
        {
            if (destinationPreferences == null)
            {
                throw new ArgumentNullException(nameof(destinationPreferences));
            }

            var softwareKey = Registry.CurrentUser?.OpenSubKey("Software", false);
            var parentKey = softwareKey?.OpenSubKey(SanitizedParentKeyName, false);
            var serverKey = parentKey?.OpenSubKey(GetServerKeyName(GameContext.Storage.Configuration), false);
            if (serverKey == null)
            {
                return false;
            }

            foreach (var name in serverKey.GetSubKeyNames())
            {
                destinationPreferences.SetPreference(name, serverKey.GetValue(name), false);
            }

            return true;
        }

        /// <inheritdoc />
        public bool Serialize(IPreferences preferences)
        {
            if (preferences == null)
            {
                throw new ArgumentNullException(nameof(preferences));
            }

            var softwareKey = Registry.CurrentUser?.OpenSubKey("Software", true);
            var parentKey = softwareKey?.CreateSubKey(SanitizedParentKeyName, true);
            var serverKey = parentKey?.CreateSubKey(GetServerKeyName(GameContext.Storage.Configuration), true);
            if (serverKey == null)
            {
                return false;
            }

            var properties = preferences.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                var value = property.GetValue(preferences);
                if (string.Equals(nameof(IPreferences.Extras), name, StringComparison.Ordinal) && value is IEnumerable<KeyValuePair<string, object>> extras)
                {
                    foreach (var pair in extras)
                    {
                        serverKey.SetValue(pair.Key, pair.Value, (pair.Value?.GetType() ?? typeof(object)).ToRegistryType());
                    }
                }
                else
                {
                    serverKey.SetValue(name, value, property.PropertyType.ToRegistryType());
                }
            }

            return true;
        }
    }
}
