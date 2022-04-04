using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Enums
{
    public enum SpellCastFailureReason
    {
        None,

        InvalidSpell,

        InsufficientHP,

        InsufficientMP,

        InvalidTarget,

        Silenced,

        Stunned,

        Asleep,

        InvalidProjectile,

        InsufficientItems,

        Snared,

        OutOfRange,

        ConditionsNotMet,

        OnCooldown
    }
}
