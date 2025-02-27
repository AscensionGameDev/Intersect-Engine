using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Framework.Core.GameObjects.Crafting;

public partial class CraftingRecipeIngredient
{
    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public CraftingRecipeIngredient(Guid itemId, int quantity)
    {
        ItemId = itemId;
        Quantity = quantity;
    }

    public ItemDescriptor GetItem()
    {
        return ItemDescriptor.Get(ItemId);
    }
}