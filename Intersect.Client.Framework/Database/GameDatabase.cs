using System;

namespace Intersect.Client.Framework.Database
{

    public abstract class GameDatabase
    {
        // Registry Database for Client Settings Preferences.

        public bool FullScreen;

        public bool HideOthersOnWindowOpen;

        public bool TargetAccountDirection;

        public int MusicVolume;

        public int SoundVolume;

        public int TargetFps;

        public int TargetResolution;

        public bool StickyTarget;

        public bool FriendOverheadInfo;

        public bool GuildMemberOverheadInfo;

        public bool MyOverheadInfo;

        public bool NpcOverheadInfo;

        public bool PartyMemberOverheadInfo;

        public bool PlayerOverheadInfo;

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
            FriendOverheadInfo = LoadPreference("FriendOverheadInfo", true);
            GuildMemberOverheadInfo = LoadPreference("GuildMemberOverheadInfo", true);
            MyOverheadInfo = LoadPreference("MyOverheadInfo", true);
            NpcOverheadInfo = LoadPreference("NpcOverheadInfo", true);
            PartyMemberOverheadInfo = LoadPreference("PartyMemberOverheadInfo", true);
            PlayerOverheadInfo = LoadPreference("PlayerOverheadInfo", true);
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
            SavePreference("FriendOverheadInfo", FriendOverheadInfo.ToString());
            SavePreference("GuildMemberOverheadInfo", GuildMemberOverheadInfo.ToString());
            SavePreference("MyOverheadInfo", MyOverheadInfo.ToString());
            SavePreference("NpcOverheadInfo", NpcOverheadInfo.ToString());
            SavePreference("PartyMemberOverheadInfo", PartyMemberOverheadInfo.ToString());
            SavePreference("PlayerOverheadInfo", PlayerOverheadInfo.ToString());
        }

        public abstract bool LoadConfig();

    }

}
