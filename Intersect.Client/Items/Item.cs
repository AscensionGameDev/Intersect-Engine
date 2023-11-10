using System.Diagnostics;
using Intersect.Client.Framework.Items;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Items;

public class Item : IItem
{
    public Guid? BagId { get; set; }

    public Guid ItemId { get; set; }

    public int Quantity { get; set; }

    public ItemProperties ItemProperties { get; set; }

    public ItemBase Base => ItemBase.Get(ItemId);

    public void Load(Guid id, int quantity, Guid? bagId, ItemProperties itemProperties)
    {
        ItemId = id;
        Quantity = quantity;
        BagId = bagId;
        ItemProperties = itemProperties;
    }

    public static int FindQuantityOfItem<TItem>(Guid itemDescriptorId, TItem?[] slots) where TItem : IItem
    {
        return slots.Where(slot => slot?.ItemId == itemDescriptorId)
            .Aggregate(0, (totalQuantity, slot) => totalQuantity + slot.Quantity);
    }

    public static int FindFirstPartialSlot<TItem>(Guid itemDescriptorId, TItem?[] slots, int maximumStack) where TItem : IItem
    {
        for (var slotIndex = 0; slotIndex < slots.Length; ++slotIndex)
        {
            var slot = slots[slotIndex];
            if (slot?.ItemId != itemDescriptorId)
            {
                continue;
            }

            if (slot.Quantity < maximumStack)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    public static int FindSpaceForItem<TItem>(
        Guid itemDescriptorId,
        ItemType itemType,
        int maximumStack,
        int slotHint,
        int searchQuantity,
        TItem?[] slots
    ) where TItem : IItem
    {
        Debug.Assert(itemDescriptorId != default);
        Debug.Assert(slots != default);

        var availableQuantity = 0;

        for (var slotIndex = 0; availableQuantity < searchQuantity && slotIndex < slots.Length; ++slotIndex)
        {
            var slot = slots[(slotIndex + Math.Max(0, slotHint)) % slots.Length];
            if (slot == null || slot.ItemId == default)
            {
                availableQuantity += maximumStack;
            }
            else if (itemType == ItemType.Equipment)
            {
                // Equipment slots are not valid target slots because they can have randomized stats
                continue;
            }
            else if (slot.ItemId == itemDescriptorId)
            {
                availableQuantity += maximumStack - slot.Quantity;
            }
        }

        return Math.Min(availableQuantity, searchQuantity);
    }
}