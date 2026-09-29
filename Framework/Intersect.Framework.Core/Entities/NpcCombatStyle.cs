namespace Intersect.Enums;

/// <summary>
/// Drives type-specific combat behavior on top of an NPC's base stats - lets monsters feel distinct from
/// each other instead of just being bigger or smaller stat blocks.
/// </summary>
public enum NpcCombatStyle
{
    /// <summary>No special behavior - just its raw stats.</summary>
    Standard = 0,

    /// <summary>Hits much harder once it drops to a low fraction of its own health.</summary>
    Brute,

    /// <summary>Chance to immediately counter-attack whenever it's hit.</summary>
    Guardian,

    /// <summary>Flees earlier and gets a burst of speed while fleeing, making it hard to run down once it decides to bolt.</summary>
    Skirmisher,

    /// <summary>Prioritizes casting its spells over melee.</summary>
    Caster,
}
