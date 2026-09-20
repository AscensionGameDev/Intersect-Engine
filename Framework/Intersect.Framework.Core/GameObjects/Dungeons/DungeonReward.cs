namespace Intersect.Framework.Core.GameObjects.Dungeons;

/// <summary>
/// An item handed to every participant when a dungeon is completed.
/// </summary>
public partial class DungeonReward
{
    public DungeonReward()
    {
    }

    public DungeonReward(Guid itemId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
    }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; } = 1;
}
