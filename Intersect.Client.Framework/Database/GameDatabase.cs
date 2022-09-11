using System;

namespace Intersect.Client.Framework.Database
{
    /// <summary>
    /// User preferences database for client settings.
    /// </summary>
    public abstract partial class GameDatabase
    {
        public bool FullScreen { get; set; }

        public bool TargetAccountDirection { get; set; }

        public int MusicVolume { get; set; }

        public int SoundVolume { get; set; }

        public int TargetFps { get; set; }

        public int TargetResolution { get; set; }

        public bool EnableLighting { get; set; }

        public bool StickyTarget { get; set; }

        public bool FriendOverheadInfo { get; set; }

        public bool GuildMemberOverheadInfo { get; set; }

        public bool MyOverheadInfo { get; set; }

        public bool NpcOverheadInfo { get; set; }

        public bool PartyMemberOverheadInfo { get; set; }

        public bool PlayerOverheadInfo { get; set; }

        public bool HideOthersOnWindowOpen { get; set; }

        public bool UiExpToPercentage { get; set; }

        public bool UiHpToPercentage { get; set; }

        public bool UiMpToPercentage { get; set; }

        public abstract void DeletePreference(string key);

        public abstract bool HasPreference(string key);

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
            EnableLighting = LoadPreference("EnableLighting", true);
            HideOthersOnWindowOpen = LoadPreference("HideOthersOnWindowOpen", true);
            TargetAccountDirection = LoadPreference("TargetAccountDirection", false);
            StickyTarget = LoadPreference("StickyTarget", true);
            FriendOverheadInfo = LoadPreference("FriendOverheadInfo", true);
            GuildMemberOverheadInfo = LoadPreference("GuildMemberOverheadInfo", true);
            MyOverheadInfo = LoadPreference("MyOverheadInfo", true);
            NpcOverheadInfo = LoadPreference("NpcOverheadInfo", true);
            PartyMemberOverheadInfo = LoadPreference("PartyMemberOverheadInfo", true);
            PlayerOverheadInfo = LoadPreference("PlayerOverheadInfo", true);
            UiExpToPercentage = LoadPreference("UiExpToPercentage", true);
            UiHpToPercentage = LoadPreference("UiHpToPercentage", false);
            UiMpToPercentage = LoadPreference("UiMpToPercentage", false);
        }

        public virtual void SavePreferences()
        {
            SavePreference("MusicVolume", MusicVolume.ToString());
            SavePreference("SoundVolume", SoundVolume.ToString());
            SavePreference("Fullscreen", FullScreen.ToString());
            SavePreference("Resolution", TargetResolution.ToString());
            SavePreference("Fps", TargetFps.ToString());
            SavePreference("EnableLighting", EnableLighting.ToString());
            SavePreference("HideOthersOnWindowOpen", HideOthersOnWindowOpen.ToString());
            SavePreference("TargetAccountDirection", TargetAccountDirection.ToString());
            SavePreference("StickyTarget", StickyTarget.ToString());
            SavePreference("FriendOverheadInfo", FriendOverheadInfo.ToString());
            SavePreference("GuildMemberOverheadInfo", GuildMemberOverheadInfo.ToString());
            SavePreference("MyOverheadInfo", MyOverheadInfo.ToString());
            SavePreference("NpcOverheadInfo", NpcOverheadInfo.ToString());
            SavePreference("PartyMemberOverheadInfo", PartyMemberOverheadInfo.ToString());
            SavePreference("PlayerOverheadInfo", PlayerOverheadInfo.ToString());
            SavePreference("UiExpToPercentage", UiExpToPercentage.ToString());
            SavePreference("UiHpToPercentage", UiHpToPercentage.ToString());
            SavePreference("UiMpToPercentage", UiMpToPercentage.ToString());
        }

        public abstract bool LoadConfig();

    }

}
