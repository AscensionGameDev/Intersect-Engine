using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class EquipmentPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public EquipmentPacket()
    {
    }

    //We send item ids for the equipment of others, but we send the correlating inventory slots for wearers because we want the equipped icons to show in the inventory.
    public EquipmentPacket(Guid entityId, int[] invSlots, Guid[] itemIds, byte[] enchantLevels = null)
    {
        EntityId = entityId;
        InventorySlots = invSlots;
        ItemIds = itemIds;
        EnchantLevels = enchantLevels ?? [];
    }

    [Key(0)]
    public Guid EntityId { get; set; }

    [Key(1)]
    public int[] InventorySlots { get; set; }

    [Key(2)]
    public Guid[] ItemIds { get; set; }

    /// <summary>Enchantment level of the item worn in each equipment slot (0 = none).</summary>
    [Key(3)]
    public byte[] EnchantLevels { get; set; } = [];

}
