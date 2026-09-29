using Intersect.Enums;

namespace Intersect.Server.Entities;

/// <summary>
/// A single, purely decorative piece of furniture spawned inside a guild hall <see cref="Maps.MapInstance"/>.
/// Mirrors the DB-persisted <see cref="Database.PlayerData.Players.GuildFurniturePlacement"/> it was spawned
/// from, but lives only in memory for the lifetime of the instance.
/// </summary>
public class FurnitureInstance
{
    public FurnitureInstance(Guid placementId, Guid descriptorId, byte x, byte y, Direction direction)
    {
        PlacementId = placementId;
        DescriptorId = descriptorId;
        X = x;
        Y = y;
        Direction = direction;
    }

    public Guid PlacementId { get; }

    public Guid DescriptorId { get; set; }

    public byte X { get; set; }

    public byte Y { get; set; }

    public Direction Direction { get; set; }
}
