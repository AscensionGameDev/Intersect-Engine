using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects;

[Owned]
public partial class SpellWarpDescriptor
{
    public Guid MapId { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Dir { get; set; }
}