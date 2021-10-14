using System;

namespace Intersect.Client.Framework.Database
{

    public abstract class GameDatabase
    {

        public bool FullScreen { get; set; }

        public bool HideOthersOnWindowOpen { get; set; }

        public bool TargetAccountDirection { get; set; }

        //Preferences
        public int MusicVolume { get; set; }

        public int SoundVolume { get; set; }

        public int TargetFps { get; set; }

        public int TargetResolution { get; set; }

        public bool StickyTarget { get; set; }

        // TODO: Expose through client options
        public bool EnableContextMenus { get; set; }

        //Saving password, other stuff we don't want in the games directory
        public abstract void SavePreference(string key, object value);

        public abstract string LoadPreference(string key);

        public T LoadPreference<T>(string key, T defaultValue)
        {
            var value = LoadPreference(key);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return (T) Convert.ChangeType(value, typeof(T));
        }

        //Load all preferences when the game starts
        public virtual void LoadPreferences()
        {
            MusicVolume = LoadPreference("MusicVolume", 25);
            SoundVolume = LoadPreference("SoundVolume", 25);
            TargetResolution = LoadPreference("Resolution", 0);
            TargetFps = LoadPreference("Fps", 0);
            FullScreen = LoadPreference("Fullscreen", false);
            HideOthersOnWindowOpen = LoadPreference("HideOthersOnWindowOpen", true);
            TargetAccountDirection = LoadPreference("TargetAccountDirection", false);
            StickyTarget = LoadPreference("StickyTarget", true);
            EnableContextMenus = LoadPreference("EnableContextMenus", true);
        }

        public virtual void SavePreferences()
        {
            SavePreference("MusicVolume", MusicVolume.ToString());
            SavePreference("SoundVolume", SoundVolume.ToString());
            SavePreference("Fullscreen", FullScreen.ToString());
            SavePreference("Resolution", TargetResolution.ToString());
            SavePreference("Fps", TargetFps.ToString());
            SavePreference("HideOthersOnWindowOpen", HideOthersOnWindowOpen.ToString());
            SavePreference("TargetAccountDirection", TargetAccountDirection.ToString());
            SavePreference("StickyTarget", StickyTarget.ToString());
            SavePreference("EnableContextMenus", EnableContextMenus.ToString());
        }

        public abstract bool LoadConfig();

    }

}
