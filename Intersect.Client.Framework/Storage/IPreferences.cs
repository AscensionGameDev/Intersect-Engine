using System.Collections.Generic;

namespace Intersect.Client.Framework.Storage
{
    public interface IPreferences
    {
        bool Fullscreen { get; set; }

        bool HideOthersOnWindowOpen { get; set; }

        int MusicVolume { get; set; }

        int SoundVolume { get; set; }

        int Fps { get; set; }

        int PreferredResolution { get; set; }

        IEnumerable<KeyValuePair<string, object>> Extras { get; }

        TValue GetPreference<TValue>(string key, TValue defaultValue = default);

        bool SetPreference(string key, object value, bool autoSave);

        void AddSerializer(IPreferencesSerializer preferencesSerializer);

        void RemoveSerializer(IPreferencesSerializer preferencesSerializer);

        IEnumerable<IPreferencesSerializer> GetSerializers();

        bool Load();

        bool Save();
    }
}
