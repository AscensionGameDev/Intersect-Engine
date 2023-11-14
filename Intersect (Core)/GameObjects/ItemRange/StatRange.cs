using Intersect.Enums;
using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects.ItemRange;

[Owned]
public partial class StatRange : ItemRange
{
    public StatRange(Stat statAffected, int lowRange, int highRange) : base(lowRange, highRange)
    {
        StatAffected = statAffected;
    }

    public Stat StatAffected { get; set; }
}
