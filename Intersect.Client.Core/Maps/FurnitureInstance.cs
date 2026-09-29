using Intersect.Enums;

namespace Intersect.Client.Maps;

/// <summary>
/// Client-side mirror of a single piece of guild hall furniture - just enough data to draw a static
/// sprite at (X, Y) with the given facing. See <see cref="Intersect.Framework.Core.GameObjects.Furniture.FurnitureDescriptor"/>
/// for the catalog entry (sprite name, passability) this references.
/// </summary>
public class FurnitureInstance
{
    public FurnitureInstance(Guid id, Guid descriptorId, byte x, byte y, Direction direction)
    {
        Id = id;
        DescriptorId = descriptorId;
        X = x;
        Y = y;
        Direction = direction;
    }

    public Guid Id { get; }

    public Guid DescriptorId { get; set; }

    public byte X { get; set; }

    public byte Y { get; set; }

    public Direction Direction { get; set; }
}
