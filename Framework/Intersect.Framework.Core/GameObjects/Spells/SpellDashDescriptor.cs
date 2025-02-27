using Microsoft.EntityFrameworkCore;

namespace Intersect.GameObjects;

[Owned]
public partial class SpellDashDescriptor
{
    public bool IgnoreMapBlocks { get; set; }

    public bool IgnoreActiveResources { get; set; }

    public bool IgnoreInactiveResources { get; set; }

    public bool IgnoreZDimensionAttributes { get; set; }
}