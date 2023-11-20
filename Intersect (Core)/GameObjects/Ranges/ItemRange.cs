using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects.Ranges;

/// <summary>
/// ItemRange exists to generalize rollable stats on an item
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
        InitializeRandomizer();
    }

    public int Roll()
    {
        if (Seed == default)
        {
            InitializeRandomizer();
        }
        return Randomizer.Next(LowRange, HighRange + 1);
    }

    public int LowRange { get; set; }

    public int HighRange { get; set; }

    private int Seed { get; set; }

    [NotMapped]
    private Random Randomizer { get; set; }

    private void InitializeRandomizer()
    {
        Seed = Guid.NewGuid().GetHashCode();
        Randomizer = new Random(Seed);
    }
}
