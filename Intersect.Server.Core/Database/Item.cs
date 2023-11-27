using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.Server.Database;

public class Item
{
    [JsonIgnore][NotMapped] public double DropChance = 100;

    public Item()
    {
        Properties = new ItemProperties();
    }

    public Item(Guid itemId, int quantity, ItemProperties properties = null) : this(
        itemId,
        quantity,
        null,
        null,
        properties
    )
    {
    }

    public Item(
        Guid itemId,
        int quantity,
        Guid? bagId,
        Bag bag,
        ItemProperties properties = null
    )
    {
        ItemId = itemId;
        Quantity = quantity;
        BagId = bagId;
        Bag = bag;
        Properties = properties ?? new ItemProperties();

        var descriptor = ItemBase.Get(ItemId);
        if (descriptor == null || properties != null)
        {
            return;
        }

        if (descriptor.ItemType != ItemType.Equipment)
        {
            return;
        }

        foreach (Stat stat in Enum.GetValues<Stat>())
        {
            if (descriptor.TryGetRangeFor(stat, out var range))
            {
                Properties.StatModifiers[(int)stat] = range.Roll();
            }
        }
    }

    public Item(Item item) : this(item.ItemId, item.Quantity, item.BagId, item.Bag)
    {
        Properties = new ItemProperties(item.Properties);
        DropChance = item.DropChance;
    }

    // TODO: THIS SHOULD NOT BE A NULLABLE. This needs to be fixed.
    public Guid? BagId { get; set; }

    [JsonIgnore] public virtual Bag Bag { get; set; }

    public Guid ItemId { get; set; } = Guid.Empty;

    [NotMapped] public string ItemName => ItemBase.GetName(ItemId);

    public int Quantity { get; set; }

    [NotMapped] public ItemProperties Properties { get; set; }

    [Column("ItemProperties")]
    [JsonIgnore]
    public string ItemPropertiesJson
    {
        get => JsonConvert.SerializeObject(Properties);
        set =>
            Properties = JsonConvert.DeserializeObject<ItemProperties>(value ?? string.Empty) ?? new ItemProperties();
    }

    [JsonIgnore][NotMapped] public ItemBase Descriptor => ItemBase.Get(ItemId);

    public static Item None => new();

    public Item Clone()
    {
        return new Item(this);
    }

    public string Data()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static bool TryFindSourceSlotsForItem<TItem>(
        Guid itemDescriptorId,
        int slotHint,
        int searchQuantity,
        TItem?[] slots,
        [NotNullWhen(true)] out int[] sourceSlotIndices
    ) where TItem : Item
    {
        Debug.Assert(itemDescriptorId != default);
        Debug.Assert(slots != default);

        if (searchQuantity < 1)
        {
            sourceSlotIndices = default;
            return false;
        }

        var remainingQuantity = searchQuantity;
        List<int> compatibleSlots = new();

        if (slotHint > -1)
        {
            var slot = slots[slotHint];
            if (slot?.ItemId == itemDescriptorId)
            {
                var remainingQuantityInSlot = Math.Min(remainingQuantity, slot.Quantity);
                if (remainingQuantityInSlot > 0)
                {
                    remainingQuantity -= remainingQuantityInSlot;
                    compatibleSlots.Add(slotHint);
                }
            }
        }

        for (var slotIndex = 0; 0 < remainingQuantity && slotIndex < slots.Length; ++slotIndex)
        {
            var slot = slots[slotIndex];
            if (slotIndex == slotHint)
            {
                // If slotHint is < 0 this will never be hit
                // If slotHint is >= 0 we already accounted for the current slot in the if-block above
                continue;
            }

            if (slot?.ItemId != itemDescriptorId)
            {
                continue;
            }

            var remainingQuantityInSlot = Math.Min(remainingQuantity, slot.Quantity);
            if (remainingQuantityInSlot <= 0)
            {
                continue;
            }

            remainingQuantity -= remainingQuantityInSlot;
            compatibleSlots.Add(slotIndex);
        }

        if (remainingQuantity != 0 || compatibleSlots.Count < 1)
        {
            sourceSlotIndices = default;
            return false;
        }

        sourceSlotIndices = compatibleSlots.ToArray();
        return true;
    }

    public static TItem[] FindCompatibleSlotsForItem<TItem>(
        ItemBase itemDescriptor,
        int maximumStack,
        int slotHint,
        int searchQuantity,
        TItem?[] slots,
        bool excludeEmpty = false
    ) where TItem : Item
    {
        Debug.Assert(itemDescriptor != default);
        Debug.Assert(slots != default);

        if (excludeEmpty && itemDescriptor.ItemType == ItemType.Equipment)
        {
            return Array.Empty<TItem>();
        }

        var availableQuantity = 0;
        List<TItem> compatibleSlots = new();

        if (slotHint > -1)
        {
            var slot = slots[slotHint];
            if (slot == null || slot.ItemId == default)
            {
                if (!excludeEmpty)
                {
                    availableQuantity += maximumStack;
                    compatibleSlots.Add(slot);
                }
            }
            else if (slot.ItemId == itemDescriptor.Id)
            {
                var availableQuantityInSlot = maximumStack - slot.Quantity;
                if (availableQuantityInSlot > 0)
                {
                    availableQuantity += availableQuantityInSlot;
                    compatibleSlots.Add(slot);
                }
            }
        }

        for (var slotIndex = 0; availableQuantity < searchQuantity && slotIndex < slots.Length; ++slotIndex)
        {
            var slot = slots[slotIndex];
            if (slotIndex == slotHint)
            {
                // If slotHint is < 0 this will never be hit
                // If slotHint is >= 0 we already accounted for the current slot in the if-block above
                continue;
            }

            if (slot == null || slot.ItemId == default)
            {
                if (excludeEmpty)
                {
                    continue;
                }

                availableQuantity += maximumStack;
                compatibleSlots.Add(slot);
                continue;
            }

            if (itemDescriptor.ItemType == ItemType.Equipment)
            {
                // Equipment slots are not valid target slots because they can have randomized stats
                continue;
            }

            if (slot.ItemId != itemDescriptor.Id)
            {
                continue;
            }

            var availableQuantityInSlot = maximumStack - slot.Quantity;
            if (availableQuantityInSlot <= 0)
            {
                continue;
            }

            availableQuantity += availableQuantityInSlot;
            compatibleSlots.Add(slot);
        }

        return compatibleSlots.ToArray();
    }

    public static int[] FindCompatibleSlotsForItem<TItem>(
        Guid itemDescriptorId,
        ItemType itemType,
        int maximumStack,
        int slotHint,
        int searchQuantity,
        TItem?[] slots
    ) where TItem : Item
    {
        Debug.Assert(itemDescriptorId != default);
        Debug.Assert(slots != default);

        var availableQuantity = 0;
        List<int> compatibleSlots = new();

        if (slotHint > -1)
        {
            var slot = slots[slotHint];
            if (slot == null || slot.ItemId == default)
            {
                availableQuantity += maximumStack;
                compatibleSlots.Add(slotHint);
            }
            else if (slot.ItemId == itemDescriptorId)
            {
                var availableQuantityInSlot = maximumStack - slot.Quantity;
                if (availableQuantityInSlot > 0)
                {
                    availableQuantity += availableQuantityInSlot;
                    compatibleSlots.Add(slotHint);
                }
            }
        }

        for (var slotIndex = 0; availableQuantity < searchQuantity && slotIndex < slots.Length; ++slotIndex)
        {
            var slot = slots[slotIndex];
            if (slotIndex == slotHint)
            {
                // If slotHint is < 0 this will never be hit
                // If slotHint is >= 0 we already accounted for the current slot in the if-block above
                continue;
            }

            if (slot == null || slot.ItemId == default)
            {
                availableQuantity += maximumStack;
                compatibleSlots.Add(slotIndex);
            }
            else if (itemType == ItemType.Equipment)
            {
                // Equipment slots are not valid target slots because they can have randomized stats
                continue;
            }
            else if (slot.ItemId == itemDescriptorId)
            {
                var availableQuantityInSlot = maximumStack - slot.Quantity;
                if (availableQuantityInSlot <= 0)
                {
                    continue;
                }

                availableQuantity += availableQuantityInSlot;
                compatibleSlots.Add(slotIndex);
            }
        }

        return compatibleSlots.ToArray();
    }

    public static int FindQuantityOfItem<TItem>(Guid itemDescriptorId, TItem?[] slots) where TItem : Item
    {
        return slots.Where(slot => slot?.ItemId == itemDescriptorId)
            .Aggregate(0, (totalQuantity, slot) => totalQuantity + slot.Quantity);
    }

    public static int FindSpaceForItem<TItem>(
        Guid itemDescriptorId,
        ItemType itemType,
        int maximumStack,
        int slotHint,
        int searchQuantity,
        TItem?[] slots
    ) where TItem : Item
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

    public virtual void Set(Item item)
    {
        ItemId = item.ItemId;
        Quantity = item.Quantity;
        BagId = item.BagId;
        Bag = item.Bag;
        Properties = new ItemProperties(item.Properties);
    }

    /// <summary>
    ///     Try to get the bag, with an additional attempt to load it if it is not already loaded (it should be if this is even
    ///     a bag item).
    /// </summary>
    /// <param name="bag">the bag if there is one associated with this <see cref="Item" /></param>
    /// <returns>if <paramref name="bag" /> is not <see langword="null" /></returns>
    public bool TryGetBag(out Bag bag)
    {
        bag = Bag;

        if (bag == null)
        {
            var descriptor = Descriptor;
            if (descriptor?.ItemType == ItemType.Bag)
            {
                bag = Bag.GetBag(BagId ?? Guid.Empty);
                bag?.ValidateSlots();
                Bag = bag;
            }
        }
        else
        {
            // Remove any items from this bag that have been removed from the game
            foreach (var slot in bag.Slots)
            {
                if (ItemBase.Get(slot.ItemId) == default)
                {
                    slot.Set(None);
                }
            }
        }

        return default != bag;
    }
}