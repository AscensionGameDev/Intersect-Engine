namespace Intersect.Config;

public partial class CombatOptions
{
    public int BlockingSlow { get; set; } = 30; //Slow when moving with a shield. Default 30%

    public int CombatTime { get; set; } = 10000; //10 seconds

    public int MaxAttackRate { get; set; } = 200; //5 attacks per second

    public int MaxDashSpeed { get; set; } = 200;

    public int MinAttackRate { get; set; } = 500; //2 attacks per second

    //Combat
    public int RegenTime { get; set; } = 3000; //3 seconds

    public bool EnableCombatChatMessages { get; set; } = false; // Enables or disables combat chat messages.

    //Spells

    /// <summary>
    /// If enabled this allows spell casts to stop/be canceled if the player tries to move around (WASD)
    /// </summary>
    public bool MovementCancelsCast { get; set; } = false;

    // Cooldowns

    /// <summary>
    /// Configures whether cooldowns within cooldown groups should match.
    /// </summary>
    public bool MatchGroupCooldowns { get; set; } = true;

    /// <summary>
    /// Only used when <seealso cref="MatchGroupCooldowns"/> is enabled!
    /// Configures whether cooldowns are being matched to the highest cooldown within a cooldown group when true, or are matched to the current item or spell being used when false.
    /// </summary>
    public bool MatchGroupCooldownHighest { get; set; } = true;

    /// <summary>
    /// Only used when <seealso cref="MatchGroupCooldowns"/> is enabled!
    /// Configures whether cooldown groups between items and spells are shared.
    /// </summary>
    public bool LinkSpellAndItemCooldowns { get; set; } = true;

    /// <summary>
    /// Configures whether or not using a spell or item should trigger a global cooldown.
    /// </summary>
    public bool EnableGlobalCooldowns { get; set; } = false;

    /// <summary>
    /// Configures the duration (in milliseconds) which the global cooldown lasts after each ability.
    /// Only used when <seealso cref="EnableGlobalCooldowns"/> is enabled!
    /// </summary>
    public int GlobalCooldownDuration { get; set; } = 1500;

    /// <summary>
    /// Configures the maximum distance a target is allowed to be from the player when auto targetting.
    /// </summary>
    public int MaxPlayerAutoTargetRadius { get; set; } = 15;

    /// <summary>
    /// If enabled this allows regenerate vitals in combat
    /// </summary>
    public bool RegenVitalsInCombat { get; set; } = false;

    /// <summary>
    /// If enabled, this allows entities to turn around while casting
    /// </summary>
    public bool EnableTurnAroundWhileCasting { get; set; } = false;

    /// <summary>
    /// If enabled, the target window will be shown to players whenever they target an entity
    /// </summary>
    public bool EnableTargetWindow { get; set; } = true;

    /// <summary>
    /// If enabled, this makes it so a player casting a friendly spell on a hostile target instead casts the spell upon themselves
    /// </summary>
    public bool EnableAutoSelfCastFriendlySpellsWhenTargetingHostile { get; set; } = false;

    /// <summary>
    /// If enabled, this allows players to cast friendly spells on players who aren't in their guild or party
    /// </summary>
    public bool EnableAllPlayersFriendlyInSafeZone { get; set; } = false;

    /// <summary>
    /// Percentage of healing reduction when Grievous Wounds effect is applied. Default 50%.
    /// </summary>
    public int GrievousWoundsHealingReduction { get; set; } = 50;

    /// <summary>
    /// Percentage of healing increase when Healing Boost effect is applied. Default 50%.
    /// </summary>
    public int HealingBoostPercentage { get; set; } = 50;
}
