using Intersect.Reflection;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Framework.Storage
{
    public class GamePreferences : IPreferences
    {
        [JsonProperty(nameof(Extras))]
        private Dictionary<string, object> ExtrasStore { get; } = new Dictionary<string, object>();

        private List<IPreferencesSerializer> Serializers { get; } = new List<IPreferencesSerializer>();

        /// <inheritdoc />
        public bool Fullscreen { get; set; } = false;

        /// <inheritdoc />
        public bool HideOthersOnWindowOpen { get; set; } = true;

        /// <inheritdoc />
        public int MusicVolume { get; set; } = 10;

        /// <inheritdoc />
        public int SoundVolume { get; set; } = 10;

        /// <inheritdoc />
        public int Fps { get; set; } = 60;

        /// <inheritdoc />
        public int PreferredResolution { get; set; } = 0;

        [JsonIgnore]
        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, object>> Extras => ExtrasStore.AsEnumerable();

        public TValue GetPreference<TValue>(string key, TValue defaultValue = default)
        {
            if (!ExtrasStore.TryGetValue(key, out var genericValue))
            {
                var property = typeof(GamePreferences).FindProperty(key);

                if (property == default)
                {
                    return default;
                }

                return (TValue) property.GetValue(this);
            }

            return (TValue) genericValue;
        }

        public bool SetPreference(string key, object value, bool autoSave = false)
        {
            var property = typeof(GamePreferences).FindProperty(key);

            if (property == default)
            {
                ExtrasStore[key] = value;
            }
            else
            {
                property.SetValue(this, value);
            }

            return autoSave ? Save() : true;
        }

        /// <inheritdoc />
        public void AddSerializer(IPreferencesSerializer preferencesSerializer)
        {
            lock (Serializers)
            {
                if (Serializers.Contains(preferencesSerializer))
                {
                    return;
                }

                Serializers.Add(preferencesSerializer);
            }
        }

        /// <inheritdoc />
        public void RemoveSerializer(IPreferencesSerializer preferencesSerializer)
        {
            lock (Serializers)
            {
                if (Serializers.Contains(preferencesSerializer))
                {
                    Serializers.Remove(preferencesSerializer);
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<IPreferencesSerializer> GetSerializers() => Serializers.AsReadOnly();

        /// <inheritdoc />
        public virtual bool Load() =>
            Serializers.FirstOrDefault(serializer => serializer?.Deserialize(this) ?? false) != null;

        /// <inheritdoc />
        public virtual bool Save() =>
            Serializers.FirstOrDefault(serializer => serializer?.Serialize(this) ?? false) != null;
    }
}
