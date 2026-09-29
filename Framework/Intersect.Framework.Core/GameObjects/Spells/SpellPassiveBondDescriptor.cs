using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects;

/// <summary>
/// Configuration for a <see cref="Intersect.Enums.SpellType.PassiveBond"/> spell ("Lien Ultime"): once
/// learned, it isn't cast - it passively boosts damage when the caster and their summon hit the same
/// target within <see cref="WindowMs"/> of each other.
/// </summary>
[Owned]
public partial class SpellPassiveBondDescriptor
{
    public double BonusDamagePercent { get; set; } = 25;

    public int WindowMs { get; set; } = 1000;
}
