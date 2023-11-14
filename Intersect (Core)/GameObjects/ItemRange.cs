using Intersect.Utilities;

namespace Intersect.GameObjects;

/// <summary>
/// TODO: ItemRange exists to generalize rollable stats on an item. After adding the ability to set specific ranges
/// per-STAT on items, I want to go back and add the ability to do the same thing for Bonus Effects - Day
/// </summary>
public abstract partial class ItemRange
{
    public ItemRange(int lowRange, int highRange)
    {
        LowRange = lowRange;
        HighRange = highRange;
    }

    public ItemRange()
    {
    }

    public int Roll()
    {
        return Randomization.Next(LowRange, HighRange + 1);
    }

    public int LowRange { get; set; }

    public int HighRange { get; set; }
}
