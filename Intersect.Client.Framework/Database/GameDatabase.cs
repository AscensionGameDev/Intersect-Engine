using System;

namespace Intersect.Client.Framework.Database
{
    /// <summary>
    /// User preferences database for client settings.
    /// </summary>
    public abstract partial class GameDatabase
    {
        public bool FullScreen { get; set; }

        public bool HideOthersOnWindowOpen { get; set; }

        public bool TargetAccountDirection { get; set; }

        public int MusicVolume { get; set; }

        public int SoundVolume { get; set; }

        public int TargetFps { get; set; }

        public int TargetResolution { get; set; }

        public bool EnableLighting { get; set; }

        public bool StickyTarget { get; set; }

        public bool AutoTurnToTarget { get; set; }

        public bool FriendOverheadInfo { get; set; }

        public bool GuildMemberOverheadInfo { get; set; }

        public bool MyOverheadInfo { get; set; }

        public bool NpcOverheadInfo { get; set; }

        public bool PartyMemberOverheadInfo { get; set; }

        public bool PlayerOverheadInfo { get; set; }

        public bool ShowExperienceAsPercentage { get; set; }

        public bool ShowHealthAsPercentage { get; set; }

        public bool ShowManaAsPercentage { get; set; }

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

        /// <summary>
        /// Load all settings preferences when the game starts.
        /// </summary>
        public virtual void LoadPreferences()
        {
            MusicVolume = LoadPreference(nameof(MusicVolume), 25);
            SoundVolume = LoadPreference(nameof(SoundVolume), 25);
            TargetResolution = LoadPreference(nameof(TargetResolution), 0);
            TargetFps = LoadPreference(nameof(TargetFps), 0);
            FullScreen = LoadPreference(nameof(FullScreen), false);
            EnableLighting = LoadPreference(nameof(EnableLighting), true);
            HideOthersOnWindowOpen = LoadPreference(nameof(HideOthersOnWindowOpen), true);
            TargetAccountDirection = LoadPreference(nameof(TargetAccountDirection), false);
            StickyTarget = LoadPreference(nameof(StickyTarget), false);
            AutoTurnToTarget = LoadPreference(nameof(AutoTurnToTarget), false);
            FriendOverheadInfo = LoadPreference(nameof(FriendOverheadInfo), true);
            GuildMemberOverheadInfo = LoadPreference(nameof(GuildMemberOverheadInfo), true);
            MyOverheadInfo = LoadPreference(nameof(MyOverheadInfo), true);
            NpcOverheadInfo = LoadPreference(nameof(NpcOverheadInfo), true);
            PartyMemberOverheadInfo = LoadPreference(nameof(PartyMemberOverheadInfo), true);
            PlayerOverheadInfo = LoadPreference(nameof(PlayerOverheadInfo), true);
            ShowExperienceAsPercentage = LoadPreference(nameof(ShowExperienceAsPercentage), true);
            ShowHealthAsPercentage = LoadPreference(nameof(ShowHealthAsPercentage), false);
            ShowManaAsPercentage = LoadPreference(nameof(ShowManaAsPercentage), false);
        }

        /// <summary>
        /// Saves all settings when applying preferences.
        /// </summary>
        public virtual void SavePreferences()
        {
            SavePreference(nameof(MusicVolume), MusicVolume);
            SavePreference(nameof(SoundVolume), SoundVolume);
            SavePreference(nameof(TargetResolution), TargetResolution);
            SavePreference(nameof(TargetFps), TargetFps);
            SavePreference(nameof(FullScreen), FullScreen);
            SavePreference(nameof(EnableLighting), EnableLighting);
            SavePreference(nameof(HideOthersOnWindowOpen), HideOthersOnWindowOpen);
            SavePreference(nameof(TargetAccountDirection), TargetAccountDirection);
            SavePreference(nameof(StickyTarget), StickyTarget);
            SavePreference(nameof(AutoTurnToTarget), AutoTurnToTarget);
            SavePreference(nameof(FriendOverheadInfo), FriendOverheadInfo);
            SavePreference(nameof(GuildMemberOverheadInfo), GuildMemberOverheadInfo);
            SavePreference(nameof(MyOverheadInfo), MyOverheadInfo);
            SavePreference(nameof(NpcOverheadInfo), NpcOverheadInfo);
            SavePreference(nameof(PartyMemberOverheadInfo), PartyMemberOverheadInfo);
            SavePreference(nameof(PlayerOverheadInfo), PlayerOverheadInfo);
            SavePreference(nameof(ShowExperienceAsPercentage), ShowExperienceAsPercentage);
            SavePreference(nameof(ShowHealthAsPercentage), ShowHealthAsPercentage);
            SavePreference(nameof(ShowManaAsPercentage), ShowManaAsPercentage);
        }

        public abstract bool LoadConfig();

    }

}
