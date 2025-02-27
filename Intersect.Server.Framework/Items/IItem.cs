using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Server.Framework.Items;

public interface IItem
{
    Guid ItemId { get; }
    ItemDescriptor Descriptor { get; }
    int Quantity { get; }
    string ItemName { get; }
    double DropChance { get; }
}