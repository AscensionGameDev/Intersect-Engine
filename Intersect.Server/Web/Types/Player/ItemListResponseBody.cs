using Intersect.Server.Database.PlayerData.Players;

namespace Intersect.Server.Web.Types.Player;

public readonly struct ItemListResponse(IEnumerable<BankSlot> bankItems, IEnumerable<InventorySlot> inventoryItems)
{
    public IEnumerable<BankSlot> Bank { get; init; } = bankItems;

    public IEnumerable<InventorySlot> Inventory { get; init; } = inventoryItems;
}