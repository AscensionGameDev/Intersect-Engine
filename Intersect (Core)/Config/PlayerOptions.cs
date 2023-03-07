namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable options pertaining to the way Players are handled by the engine.
    /// </summary>
    public partial class PlayerOptions
    {
        /// <summary>
        /// Default value for initial amount of player's bank slots.
        /// </summary>
        private const int DefaultInitialBankSlots = 100;

        /// Unlinks the timers for combat and movement to facilitate complex combat (e.g. kiting)
        /// </summary>
        public bool AllowCombatMovement { get; set; } = true;

        /// <summary>
        /// Sets the delay (milliseconds) for the feature 'Auto-turn to target'.
        /// </summary>
        public ushort AutoTurnToTargetDelay { get; set; } = 500;

        /// <summary>
        /// When enabled, 'Auto-turn to target' ignores entities behind players.
        /// </summary>
        public bool AutoTurnToTargetIgnoresEntitiesBehind { get; set; } = false;

        /// <summary>
        /// Enables the client feature 'Auto-turn to target'.
        /// </summary>
        public bool EnableAutoTurnToTarget { get; set; } = true;

        /// <summary>
        /// Enables or disables friend login notifications when a user joins the game.
        /// </summary>
        public bool EnableFriendLoginNotifications { get; set; } = true;

        /// <summary>
        /// If true, it will remove the associated exp, otherwise you will lose the exp based on the exp required to level up.
        /// </summary>
        public bool ExpLossFromCurrentExp { get; set; } = true;

        /// <summary>
        /// A percentage between 0 and 100 which determines the experience that players will lose when they die.
        /// </summary>
        public int ExpLossOnDeathPercent { get; set; } = 0;

        /// <summary>
        /// Number of hotbar slots a player has.
        /// </summary>
        public int HotbarSlotCount { get; set; } = 10;

        /// <summary>
        /// Number of bank slots a player has.
        /// </summary>
        public int InitialBankslots { get; set; } = DefaultInitialBankSlots;

        /// <summary>
        /// A percentage between 0 and 100 which determines the chance in which they will lose any given item in their inventory when killed.
        /// </summary>
        public int ItemDropChance { get; set; } = 0;

        /// <summary>
        /// Number of characters an account may create.
        /// </summary>
        public int MaxCharacters { get; set; } = 1;

        /// <summary>
        /// Number of inventory slots a player has.
        /// </summary>
        public int MaxInventory { get; set; } = 35;

        /// <summary>
        /// Max level a player can achieve.
        /// </summary>
        public int MaxLevel { get; set; } = 100;

        /// <summary>
        /// Number of spell slots a player has.
        /// </summary>
        public int MaxSpells { get; set; } = 35;

        /// <summary>
        /// The highest value a single stat can be for a player.
        /// </summary>
        public int MaxStat { get; set; } = 255;

        /// <summary>
        /// How long a player must wait before sending a trade/party/friend.
        /// </summary>
        public int RequestTimeout { get; set; } = 300000;

        /// <summary>
        /// Configures whether or not the level of a player is shown next to their name.
        /// </summary>
        public bool ShowLevelByName { get; set; } = false;

        /// <summary>
        /// Configures whether or not the game client skips the character select window upon login or going back
        /// to characters when the max number of characters allowed per account is one.
        /// </summary>
        public bool SkipCharacterSelect { get; set; } = false;

        /// <summary>
        /// Distance (in tiles) between players in which a trade offer can be sent and accepted.
        /// </summary>
        public int TradeRange { get; set; } = 6;
    }
}
