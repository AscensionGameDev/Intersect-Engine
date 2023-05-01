using System;
using Intersect.Enums;

namespace Intersect.Client.Framework.Database
{
    /// <summary>
    /// User preferences database for client settings.
    /// </summary>
    public abstract partial class GameDatabase
    {
        public bool FullScreen { get; set; }

        public bool HideOthersOnWindowOpen { get; set; }

        public bool AutoToggleChatLog { get; set; }

        public bool TargetAccountDirection { get; set; }

        public int MusicVolume { get; set; }

        public int SoundVolume { get; set; }

        public int TargetFps { get; set; }

        public int TargetResolution { get; set; }

        public bool EnableLighting { get; set; }

        public bool StickyTarget { get; set; }

        public bool AutoTurnToTarget { get; set; }

        public bool FriendOverheadInfo { get; set; }

        public bool FriendOverheadHpBar { get; set; }

        public bool GuildMemberOverheadInfo { get; set; }

        public bool GuildMemberOverheadHpBar { get; set; }

        public bool MyOverheadInfo { get; set; }

        public bool MyOverheadHpBar { get; set; }

        public bool NpcOverheadInfo { get; set; }

        public bool NpcOverheadHpBar { get; set; }

        public bool PartyMemberOverheadInfo { get; set; }

        public bool PartyMemberOverheadHpBar { get; set; }

        public bool PlayerOverheadInfo { get; set; }

        public bool PlayerOverheadHpBar { get; set; }

        public bool ShowExperienceAsPercentage { get; set; }

        public bool ShowHealthAsPercentage { get; set; }

        public bool ShowManaAsPercentage { get; set; }

        public TypewriterBehavior TypewriterBehavior { get; set; }

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

            var type = typeof(T);
            if (type.IsEnum)
            {
                try
                {
                    var enumValue = Enum.Parse(type, value);
                    return (T)enumValue;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return (T)Convert.ChangeType(value, typeof(T));
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
            AutoToggleChatLog = LoadPreference(nameof(AutoToggleChatLog), false);
            TargetAccountDirection = LoadPreference(nameof(TargetAccountDirection), false);
            StickyTarget = LoadPreference(nameof(StickyTarget), false);
            AutoTurnToTarget = LoadPreference(nameof(AutoTurnToTarget), false);
            FriendOverheadInfo = LoadPreference(nameof(FriendOverheadInfo), true);
            FriendOverheadHpBar = LoadPreference(nameof(FriendOverheadHpBar), false);
            GuildMemberOverheadInfo = LoadPreference(nameof(GuildMemberOverheadInfo), true);
            GuildMemberOverheadHpBar = LoadPreference(nameof(GuildMemberOverheadHpBar), false);
            MyOverheadInfo = LoadPreference(nameof(MyOverheadInfo), true);
            MyOverheadHpBar = LoadPreference(nameof(MyOverheadHpBar), false);
            NpcOverheadInfo = LoadPreference(nameof(NpcOverheadInfo), true);
            NpcOverheadHpBar = LoadPreference(nameof(NpcOverheadHpBar), false);
            PartyMemberOverheadInfo = LoadPreference(nameof(PartyMemberOverheadInfo), true);
            PartyMemberOverheadHpBar = LoadPreference(nameof(PartyMemberOverheadHpBar), false);
            PlayerOverheadInfo = LoadPreference(nameof(PlayerOverheadInfo), true);
            PlayerOverheadHpBar = LoadPreference(nameof(PlayerOverheadHpBar), false);
            ShowExperienceAsPercentage = LoadPreference(nameof(ShowExperienceAsPercentage), true);
            ShowHealthAsPercentage = LoadPreference(nameof(ShowHealthAsPercentage), false);
            ShowManaAsPercentage = LoadPreference(nameof(ShowManaAsPercentage), false);
            TypewriterBehavior = LoadPreference(nameof(TypewriterBehavior), TypewriterBehavior.Word);
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
            SavePreference(nameof(AutoToggleChatLog), AutoToggleChatLog);
            SavePreference(nameof(TargetAccountDirection), TargetAccountDirection);
            SavePreference(nameof(StickyTarget), StickyTarget);
            SavePreference(nameof(AutoTurnToTarget), AutoTurnToTarget);
            SavePreference(nameof(FriendOverheadInfo), FriendOverheadInfo);
            SavePreference(nameof(FriendOverheadHpBar), FriendOverheadHpBar);
            SavePreference(nameof(GuildMemberOverheadInfo), GuildMemberOverheadInfo);
            SavePreference(nameof(GuildMemberOverheadHpBar), GuildMemberOverheadHpBar);
            SavePreference(nameof(MyOverheadInfo), MyOverheadInfo);
            SavePreference(nameof(MyOverheadHpBar), MyOverheadHpBar);
            SavePreference(nameof(NpcOverheadInfo), NpcOverheadInfo);
            SavePreference(nameof(NpcOverheadHpBar), NpcOverheadHpBar);
            SavePreference(nameof(PartyMemberOverheadInfo), PartyMemberOverheadInfo);
            SavePreference(nameof(PartyMemberOverheadHpBar), PartyMemberOverheadHpBar);
            SavePreference(nameof(PlayerOverheadInfo), PlayerOverheadInfo);
            SavePreference(nameof(PlayerOverheadHpBar), PlayerOverheadHpBar);
            SavePreference(nameof(ShowExperienceAsPercentage), ShowExperienceAsPercentage);
            SavePreference(nameof(ShowHealthAsPercentage), ShowHealthAsPercentage);
            SavePreference(nameof(ShowManaAsPercentage), ShowManaAsPercentage);
            SavePreference(nameof(TypewriterBehavior), TypewriterBehavior);
        }

        public abstract bool LoadConfig();

    }

}
