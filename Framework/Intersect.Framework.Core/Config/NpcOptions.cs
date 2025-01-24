using Newtonsoft.Json;

namespace Intersect.Config;

/// <summary>
/// Contains configurable options pertaining to the way Npcs are handled by the engine.
/// </summary>
public partial class NpcOptions
{
    /// <summary>
    /// If NPCs are allowed to reset after moving out of a specified radius when starting to fight another entity.
    /// </summary>
    public bool AllowResetRadius { get; set; } = false;

    /// <summary>
    /// Configures the radius in which an NPC is allowed to move after starting to fight another entity.
    /// </summary>
    public int ResetRadius { get; set; } = 8;

    /// <summary>
    /// If the NPC is allowed to gain a new reset center point while it is still busy moving to its original reset point.
    /// NOTE: Can be used to allow the NPCs to be dragged very far away, as it constantly resets the center of its radius!!!
    /// </summary>
    public bool AllowNewResetLocationBeforeFinish { get; set; } = false;

    [JsonProperty]
    [Obsolete($"Use {nameof(ResetVitalsAndStatuses)} instead, the ResetVitalsAndStatusses property will be removed in 0.9-beta", error: true)]
    private bool ResetVitalsAndStatusses
    {
        set => ResetVitalsAndStatuses = value;
    }

    /// <summary>
    /// If the NPC should completely restore its vitals and statusses once it starts resetting.
    /// </summary>
    public bool ResetVitalsAndStatuses { get; set; }

    /// <summary>
    /// If the NPCs health should continue to reset to full and clear statuses while working its way to the reset location
    /// </summary>
    public bool ContinuouslyResetVitalsAndStatuses { get; set; } = false;

    /// <summary>
    /// If true, a NPC can be attacked while they are resetting. Their new attacker will become a target if they are within the reset radius
    /// </summary>
    public bool AllowEngagingWhileResetting { get; set; } = false;

    /// <summary>
    /// If the level of an NPC is shown next to their name.
    /// </summary>
    public bool ShowLevelByName { get; set; } = false;

    /// <summary>
    /// If true, NPCs that are resetting will walk through players
    /// </summary>
    public bool IntangibleDuringReset { get; set; } = true;

    /// <summary>
    /// If true, NPCs will go to reset state if their combat timer is exceeded
    /// </summary>
    public bool ResetIfCombatTimerExceeded { get; set; } = true;
}
