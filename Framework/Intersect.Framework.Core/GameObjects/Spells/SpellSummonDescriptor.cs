using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects;

[Owned]
public partial class SpellSummonDescriptor
{
    /// <summary>
    /// The NPC descriptor to spawn as a follower of the caster.
    /// </summary>
    public Guid NpcId { get; set; }

    /// <summary>
    /// How many copies of <see cref="NpcId"/> are spawned when this spell is cast.
    /// </summary>
    public int Amount { get; set; } = 1;

    /// <summary>
    /// How long (in milliseconds) the summon stays alive. 0 means it never expires on its own -
    /// it only disappears if its owner logs out, dies, or a new cast of this spell replaces it.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// If true, casting this spell again despawns any summon this caster already has from the
    /// same spell before spawning the new one(s) - used for the "one companion active at a time"
    /// style of summon (swap loadout instead of stacking followers).
    /// </summary>
    public bool ReplaceExisting { get; set; } = true;

    /// <summary>
    /// If true, the summon is removed the moment its owner dies. If false, it lingers (still bound
    /// by <see cref="Duration"/> if any) - useful for a boss's adds that should outlive the exact
    /// instant of a phase transition.
    /// </summary>
    public bool DespawnOnOwnerDeath { get; set; } = true;
}
