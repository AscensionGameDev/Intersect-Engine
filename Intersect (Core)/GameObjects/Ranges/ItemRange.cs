using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects.Ranges;

/// <summary>
/// TODO: ItemRange exists to generalize rollable stats on an item. After adding the ability to set specific ranges
/// per-STAT on items, I want to go back and add the ability to do the same thing for Bonus Effects - Day
/// </summary>
[Owned]
public partial class ItemRange
{
    public ItemRange(int lowRange, int highRange) : this()
    {
        LowRange = lowRange;
        HighRange = highRange;
    }

    public ItemRange()
    {
        Seed = Guid.NewGuid().GetHashCode();
        Randomizer = new Random(Seed);
    }

    public int Roll()
    {
        return Randomizer.Next(LowRange, HighRange + 1);
    }

    public int LowRange { get; set; }

    public int HighRange { get; set; }

    private int Seed { get; set; }

    [NotMapped]
    private Random Randomizer { get; set; }
}
