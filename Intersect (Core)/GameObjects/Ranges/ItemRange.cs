using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects.Ranges;

/// <summary>
/// ItemRange exists to generalize rollable stats on an item
/// </summary>
[Owned]
public partial class ItemRange
{
    public ItemRange()
    {
    }

    public ItemRange(int lowRange, int highRange) : this()
    {
        LowRange = lowRange;
        HighRange = highRange;
    }

    private Random GetRandomInstance(int? seed = default) => seed == default ? Random.Shared : new Random(seed.Value);

    public int Roll(int? seed = default)
    {
        var random = GetRandomInstance(seed);
        return random.Next(LowRange, HighRange + 1);
    }

    public int LowRange { get; set; }

    public int HighRange { get; set; }
}
