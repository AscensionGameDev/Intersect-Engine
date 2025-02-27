using Microsoft.EntityFrameworkCore;

namespace Intersect.Framework.Core.GameObjects.Items;

[Owned]
public partial class ConsumableData
{
    public ConsumableType Type { get; set; }

    public long Value { get; set; }

    public int Percentage { get; set; }
}