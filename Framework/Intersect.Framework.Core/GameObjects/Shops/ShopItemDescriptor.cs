using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Items;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class ShopItemDescriptor
{
    public Guid CostItemId { get; set; }

    public int CostItemQuantity { get; set; }

    public Guid ItemId { get; set; }

    [JsonConstructor]
    public ShopItemDescriptor(Guid itemId, Guid costItemId, int costVal)
    {
        ItemId = itemId;
        CostItemId = costItemId;
        CostItemQuantity = costVal;
    }

    [NotMapped]
    [JsonIgnore]
    public ItemDescriptor Item => ItemDescriptor.Get(ItemId);
}