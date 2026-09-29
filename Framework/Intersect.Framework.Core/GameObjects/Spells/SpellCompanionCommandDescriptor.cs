using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects;

[Owned]
public partial class SpellCompanionCommandDescriptor
{
    /// <summary>
    /// If true, this spell orders the caster's summons to disengage and return to following them
    /// ("Ordre : Repli"). If false, it orders them onto the caster's current target ("Ordre : Charge").
    /// </summary>
    public bool Retreat { get; set; }
}
