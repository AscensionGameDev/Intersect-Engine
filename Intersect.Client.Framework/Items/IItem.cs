using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Framework.Items;

public interface IItem
{
    Guid? BagId { get; set; }
    ItemDescriptor Descriptor { get; }
    Guid ItemId { get; set; }
    int Quantity { get; set; }
    ItemProperties ItemProperties { get; set; }

    void Load(Guid id, int quantity, Guid? bagId, ItemProperties itemProperties);
}
