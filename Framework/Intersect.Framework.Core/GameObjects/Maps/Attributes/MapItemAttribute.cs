using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects.Annotations;

namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

public partial class MapItemAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Item;

    [EditorLabel("Attributes", "Item")]
    [EditorReference(typeof(ItemDescriptor), nameof(ItemDescriptor.Name))]
    public Guid ItemId { get; set; }

    [EditorLabel("Attributes", "Quantity")]
    [EditorDisplay]
    public int Quantity { get; set; }

    public long RespawnTime { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapItemAttribute) base.Clone();
        att.ItemId = ItemId;
        att.Quantity = Quantity;
        att.RespawnTime = RespawnTime;

        return att;
    }
}