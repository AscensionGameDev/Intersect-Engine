using Intersect.GameObjects;

namespace Intersect.Server.Framework.Items;

public interface IItem
{
    Guid ItemId { get; }
    ItemBase Descriptor { get; }
    int Quantity { get; }
    string ItemName { get; }
    double DropChance { get; }
}