using Intersect.Server.Database;

namespace Intersect.Server.Entities;

public interface IBankInterface : IDisposable
{
    void SendOpenBank();
    void SendBankUpdate(int slot, bool sendToAll = true);
    void SendCloseBank();
    bool TryDepositItem(Item slot, int inventorySlotIndex, int quantityHint, int bankSlotIndex = -1, bool sendUpdate = true);
    bool TryDepositItem(Item item, bool sendUpdate = true);
    bool TryWithdrawItem(Item slot, int bankSlotIndex, int quantityHint, int inventorySlotIndex = -1);
    void SwapBankItems(int slotFrom, int slotTo);
}