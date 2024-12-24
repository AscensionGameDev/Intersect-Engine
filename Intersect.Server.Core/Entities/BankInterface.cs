using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System.Diagnostics;
using Intersect.Collections.Slotting;
using Log = Intersect.Logging.Log;

namespace Intersect.Server.Entities;

public partial class BankInterface<TSlot> : IBankInterface where TSlot : Item, ISlot
{
    private readonly Player _player;

    private readonly SlotList<TSlot> _bank;

    private readonly Guild _guild;

    private readonly object _lock;

    public BankInterface(Player player, SlotList<TSlot> bank, Guild? guild = default)
    {
        _player = player;
        _bank = bank;
        _guild = guild;
        _lock = guild?.Lock ?? new object();
    }

    public void SendOpenBank()
    {
        var slotUpdatePackets = new List<BankUpdatePacket>();

        for (var slot = 0; slot < _bank.Capacity; slot++)
        {
            var slotItem = slot < _bank.Count ? _bank[slot] : default;
            slotUpdatePackets.Add(
                new BankUpdatePacket(
                    slot,
                    slotItem?.ItemId ?? Guid.Empty,
                    slotItem?.Quantity ?? 0,
                    slotItem?.BagId,
                    slotItem?.Properties
                )
            );
        }

        _player?.SendPacket(
            new BankPacket(
                false,
                _guild != null,
                _bank.Capacity,
                slotUpdatePackets.ToArray()
            )
        );
    }

    //BankUpdatePacket
    public void SendBankUpdate(int slot, bool sendToAll = true)
    {
        if (sendToAll && _guild != null)
        {
            _guild.BankSlotUpdated(slot);
            return;
        }

        if (_bank[slot] != null && _bank[slot].ItemId != Guid.Empty && _bank[slot].Quantity > 0)
        {
            _player?.SendPacket(
                new BankUpdatePacket(
                    slot, _bank[slot].ItemId, _bank[slot].Quantity, _bank[slot].BagId,
                    _bank[slot].Properties
                )
            );
        }
        else
        {
            _player?.SendPacket(new BankUpdatePacket(slot, Guid.Empty, 0, null, null));
        }
    }

    public void SendCloseBank()
    {
        _player?.SendPacket(new BankPacket(true, false, -1, null));
    }

    public bool TryDepositItem(Item? slot, int inventorySlotIndex, int quantityHint, int bankSlotIndex = -1, bool sendUpdate = true)
    {
        //Permission Check
        if (_guild != null)
        {
            var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, _player.GuildRank))];
            if (!rank.Permissions.BankDeposit && _player.GuildRank != 0)
            {
                PacketSender.SendChatMsg(_player, Strings.Guilds.NotAllowedDeposit.ToString(_guild.Name), ChatMessageType.Bank, CustomColors.Alerts.Error);
                return false;
            }
        }

        slot ??= _player.Items[inventorySlotIndex];
        if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
        {
            PacketSender.SendChatMsg(
                _player,
                Strings.Banks.DepositInvalid,
                ChatMessageType.Bank,
                CustomColors.Alerts.Error
            );
            return false;
        }

        var canBank = (itemDescriptor.CanBank && _guild == default) ||
                      (itemDescriptor.CanGuildBank && _guild != default);

        if (!canBank)
        {
            PacketSender.SendChatMsg(_player, Strings.Items.NoBank, ChatMessageType.Bank, CustomColors.Items.Bound);
            return false;
        }

        var sourceSlots = _player.Items.ToArray();
        var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxBankStack : 1;
        var sourceQuantity = Item.FindQuantityOfItem(itemDescriptor.Id, sourceSlots);

        var targetSlots = _bank.ToArray();

        lock (_lock)
        {
            var movableQuantity = Item.FindSpaceForItem(
                itemDescriptor.Id,
                itemDescriptor.ItemType,
                maximumStack,
                bankSlotIndex,
                quantityHint < 0 ? sourceQuantity : quantityHint,
                targetSlots
            );

            if (movableQuantity < 1)
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Items.NoSpaceForItem,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var slotIndicesToFill = Item.FindCompatibleSlotsForItem(
                itemDescriptor.Id,
                itemDescriptor.ItemType,
                maximumStack,
                bankSlotIndex,
                movableQuantity,
                targetSlots
            );

            if (!Item.TryFindSourceSlotsForItem(
                    itemDescriptor.Id,
                    inventorySlotIndex,
                    movableQuantity,
                    sourceSlots,
                    out var slotIndicesToRemoveFrom
                ))
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Banks.WithdrawInvalid,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var nextSlotIndexToRemoveFrom = 0;
            var remainingQuantity = movableQuantity;
            foreach (var slotIndexToFill in slotIndicesToFill)
            {
                if (slotIndicesToRemoveFrom.Length <= nextSlotIndexToRemoveFrom)
                {
                    Log.Warn($"Ran out of slots to remove from for {_player.Id}");
                    break;
                }

                if (remainingQuantity < 1)
                {
                    break;
                }

                var slotToFill = targetSlots[slotIndexToFill];
                Debug.Assert(slotToFill != default);
                var quantityToStoreInSlot = Math.Min(remainingQuantity, maximumStack - slotToFill.Quantity);

                if (slotToFill.ItemId == default && maximumStack <= 1)
                {
                    if (slotIndicesToRemoveFrom.Length <= nextSlotIndexToRemoveFrom)
                    {
                        break;
                    }

                    var slotIndexToRemoveFrom = slotIndicesToRemoveFrom[nextSlotIndexToRemoveFrom++];
                    var sourceSlot = sourceSlots[slotIndexToRemoveFrom];
                    slotToFill.Set(sourceSlot);
                    remainingQuantity -= 1;
                    continue;
                }

                if (itemDescriptor.ItemType == ItemType.Equipment || maximumStack <= 1)
                {
                    Log.Warn($"{nameof(Item.FindCompatibleSlotsForItem)}() returned incompatible slots for {nameof(ItemBase)} {itemDescriptor.Id}");
                    break;
                }

                slotToFill.ItemId = itemDescriptor.Id;
                slotToFill.Quantity += quantityToStoreInSlot;
                remainingQuantity -= quantityToStoreInSlot;
            }

            var remainingQuantityToRemove = movableQuantity;
            foreach (var slotIndexToRemoveFrom in slotIndicesToRemoveFrom)
            {
                if (remainingQuantityToRemove < 1)
                {
                    Log.Error($"Potential inventory corruption for {_player.Id}");
                }

                var slotToRemoveFrom = sourceSlots[slotIndexToRemoveFrom];
                Debug.Assert(slotToRemoveFrom != default);
                var quantityToRemoveFromSlot = Math.Min(remainingQuantityToRemove, slotToRemoveFrom.Quantity);
                slotToRemoveFrom.Quantity -= quantityToRemoveFromSlot;

                // If the item is equipped equipment, we need to unequip it before taking it out of the inventory.
                if (itemDescriptor.ItemType == ItemType.Equipment && slotIndexToRemoveFrom > -1)
                {
                    _player.EquipmentProcessItemLoss(slotIndexToRemoveFrom);
                }

                if (slotToRemoveFrom.Quantity < 1)
                {
                    slotToRemoveFrom.Set(Item.None);
                }

                remainingQuantityToRemove -= quantityToRemoveFromSlot;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingQuantity < 0)
            {
                Log.Error($"{_player.Id} was accidentally given {-remainingQuantity}x extra {itemDescriptor.Id}");
            }
            else if (remainingQuantity > 0)
            {
                Log.Error($"{_player.Id} was not given {remainingQuantity}x {itemDescriptor.Id}");
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingQuantityToRemove < 0)
            {
                Log.Error($"{_player.Id} was scammed {-remainingQuantity}x {itemDescriptor.Id}");
            }
            else if (remainingQuantityToRemove > 0)
            {
                Log.Error($"{_player.Id} did not have {remainingQuantity}x {itemDescriptor.Id} taken");
            }

            if (sendUpdate)
            {
                foreach (var slotIndexToUpdate in slotIndicesToRemoveFrom)
                {
                    PacketSender.SendInventoryItemUpdate(_player, slotIndexToUpdate);
                }

                foreach (var slotIndexToFill in slotIndicesToFill)
                {
                    SendBankUpdate(slotIndexToFill);
                }

                if (inventorySlotIndex > -1)
                {
                    PacketSender.SendInventoryItemUpdate(_player, inventorySlotIndex);
                }
            }

            if (_guild != null)
            {
                DbInterface.Pool.QueueWorkItem(_guild.Save);
            }

            var successMessage = movableQuantity > 1
                ? Strings.Banks.DepositSuccessStackable.ToString(movableQuantity, itemDescriptor.Name)
                : Strings.Banks.DepositSuccessNonStackable.ToString(itemDescriptor.Name);

            PacketSender.SendChatMsg(_player, successMessage, ChatMessageType.Bank, CustomColors.Alerts.Success);

            return true;
        }
    }

    public bool TryDepositItem(Item item, bool sendUpdate = true) =>
        TryDepositItem(item, -1, item.Quantity, -1, sendUpdate);

    public bool TryWithdrawItem(Item? slot, int bankSlotIndex, int quantityHint, int inventorySlotIndex = -1)
    {
        //Permission Check
        if (_guild != null)
        {
            var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, _player.GuildRank))];
            if (!rank.Permissions.BankRetrieve && _player.GuildRank != 0)
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Guilds.NotAllowedWithdraw.ToString(_guild.Name),
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }
        }

        slot ??= _bank[bankSlotIndex];
        if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
        {
            PacketSender.SendChatMsg(
                _player,
                Strings.Banks.WithdrawInvalid,
                ChatMessageType.Bank,
                CustomColors.Alerts.Error
            );
            return false;
        }

        var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxInventoryStack : 1;
        var sourceSlots = _bank.ToArray();
        var sourceQuantity = Item.FindQuantityOfItem(itemDescriptor.Id, sourceSlots);

        var targetSlots = _player.Items.ToArray();

        lock (_lock)
        {
            var movableQuantity = Item.FindSpaceForItem(
                itemDescriptor.Id,
                itemDescriptor.ItemType,
                maximumStack,
                inventorySlotIndex,
                quantityHint < 0 ? sourceQuantity : quantityHint,
                targetSlots
            );

            if (movableQuantity < 1)
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Items.NoSpaceForItem,
                    ChatMessageType.Inventory,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var slotIndicesToFill = Item.FindCompatibleSlotsForItem(
                itemDescriptor.Id,
                itemDescriptor.ItemType,
                maximumStack,
                inventorySlotIndex,
                movableQuantity,
                targetSlots
            );

            if (!Item.TryFindSourceSlotsForItem(
                    itemDescriptor.Id,
                    bankSlotIndex,
                    movableQuantity,
                    sourceSlots,
                    out var slotIndicesToRemoveFrom
                ))
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Banks.WithdrawInvalid,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var nextSlotIndexToRemoveFrom = 0;
            var remainingQuantity = movableQuantity;
            foreach (var slotIndexToFill in slotIndicesToFill)
            {
                if (slotIndicesToRemoveFrom.Length <= nextSlotIndexToRemoveFrom)
                {
                    Log.Warn($"Ran out of slots to remove from for {_player.Id}");
                    break;
                }

                if (remainingQuantity < 1)
                {
                    break;
                }

                var slotToFill = targetSlots[slotIndexToFill];
                Debug.Assert(slotToFill != default);
                var quantityToStoreInSlot = Math.Min(remainingQuantity, maximumStack - slotToFill.Quantity);

                if (slotToFill.ItemId == default && maximumStack <= 1)
                {
                    if (slotIndicesToRemoveFrom.Length <= nextSlotIndexToRemoveFrom)
                    {
                        break;
                    }

                    var slotIndexToRemoveFrom = slotIndicesToRemoveFrom[nextSlotIndexToRemoveFrom];
                    var sourceSlot = sourceSlots[slotIndexToRemoveFrom];
                    if (sourceSlot.Quantity <= maximumStack)
                    {
                        nextSlotIndexToRemoveFrom++;
                    }
                    slotToFill.Set(sourceSlot);
                    slotToFill.Quantity = 1;
                    remainingQuantity -= 1;
                    continue;
                }

                if (itemDescriptor.ItemType == ItemType.Equipment || maximumStack <= 1)
                {
                    Log.Warn($"{nameof(Item.FindCompatibleSlotsForItem)}() returned incompatible slots for {nameof(ItemBase)} {itemDescriptor.Id}");
                    break;
                }

                slotToFill.ItemId = itemDescriptor.Id;
                slotToFill.Quantity += quantityToStoreInSlot;
                remainingQuantity -= quantityToStoreInSlot;
            }

            var remainingQuantityToRemove = movableQuantity;
            foreach (var slotIndexToRemoveFrom in slotIndicesToRemoveFrom)
            {
                if (remainingQuantityToRemove < 1)
                {
                    Log.Error($"Potential bank corruption for {_player.Id}");
                }

                var slotToRemoveFrom = sourceSlots[slotIndexToRemoveFrom];
                Debug.Assert(slotToRemoveFrom != default);
                var quantityToRemoveFromSlot = Math.Min(remainingQuantityToRemove, slotToRemoveFrom.Quantity);
                slotToRemoveFrom.Quantity -= quantityToRemoveFromSlot;
                if (slotToRemoveFrom.Quantity < 1)
                {
                    slotToRemoveFrom.Set(Item.None);
                }

                remainingQuantityToRemove -= quantityToRemoveFromSlot;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingQuantity < 0)
            {
                Log.Error($"{_player.Id} was accidentally given {-remainingQuantity}x extra {itemDescriptor.Id}");
            }
            else if (remainingQuantity > 0)
            {
                Log.Error($"{_player.Id} was not given {remainingQuantity}x {itemDescriptor.Id}");
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (remainingQuantityToRemove < 0)
            {
                Log.Error($"{_player.Id} was scammed {-remainingQuantity}x {itemDescriptor.Id}");
            }
            else if (remainingQuantityToRemove > 0)
            {
                Log.Error($"{_player.Id} did not have {remainingQuantity}x {itemDescriptor.Id} taken");
            }

            foreach (var slotIndexToUpdate in slotIndicesToFill)
            {
                PacketSender.SendInventoryItemUpdate(_player, slotIndexToUpdate);
            }

            foreach (var slotIndexToUpdate in slotIndicesToRemoveFrom)
            {
                SendBankUpdate(slotIndexToUpdate);
            }

            if (_guild != null)
            {
                DbInterface.Pool.QueueWorkItem(_guild.Save);
            }

            var successMessage = movableQuantity > 1
                ? Strings.Banks.WithdrawSuccessStackable.ToString(quantityHint, itemDescriptor.Name)
                : Strings.Banks.WithdrawSuccessNonStackable.ToString(itemDescriptor.Name);

            PacketSender.SendChatMsg(_player, successMessage, ChatMessageType.Bank, CustomColors.Alerts.Success);

            return true;
        }
    }

    public void SwapBankItems(int slotFrom, int slotTo)
    {
        var bank = _bank;
        if (bank == null)
        {
            Log.Error($"SwapBankItems() called on invalid bank for {_player.Id}");
            return;
        }

        if (Math.Clamp(slotFrom, 0, bank.Capacity - 1) != slotFrom || Math.Clamp(slotTo, 0, bank.Capacity - 1) != slotTo)
        {
            PacketSender.SendChatMsg(
                _player,
                Strings.Banks.InvalidSlotToSwap,
                ChatMessageType.Bank,
                CustomColors.Alerts.Error
            );
            Log.Error($"Invalid slot indices SwapBankItems({slotFrom}, {slotTo}) ({bank.Capacity}, {_player.Id})");
            return;
        }

        //Permission Check
        if (_guild != null)
        {
            var ranks = Options.Instance.Guild.Ranks;
            var rankIndex = Math.Clamp(_player.GuildRank, 0, ranks.Length - 1);
            var rank = ranks[rankIndex];
            if (_player.GuildRank != rankIndex || (!rank.Permissions.BankMove && _player.GuildRank != 0))
            {
                PacketSender.SendChatMsg(
                    _player,
                    Strings.Guilds.NotAllowedSwap.ToString(_guild.Name),
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return;
            }
        }

        lock (_lock)
        {
            try
            {
                var destinationSlot = bank[slotTo];
                var sourceSlot = bank[slotFrom];
                var temporarySlot = destinationSlot.Clone();

                if (destinationSlot.ItemId == sourceSlot.ItemId)
                {
                    if (sourceSlot.ItemId == default)
                    {
                        Log.Warn($"SwapBankItems({slotFrom}, {slotTo}) for {_player.Id} with empty item ID");
                        return;
                    }

                    /* Items are the same, move the maximum quantity */
                    if (!ItemBase.TryGet(sourceSlot.ItemId, out var itemDescriptor))
                    {
                        Log.Error($"SwapBankItems({slotFrom}, {slotTo}) for {_player.Id} failed due to missing item {sourceSlot.ItemId}");
                        return;
                    }

                    if (itemDescriptor.Stackable)
                    {
                        var moveQuantity = Math.Clamp(
                            sourceSlot.Quantity,
                            0,
                            itemDescriptor.MaxBankStack - destinationSlot.Quantity
                        );

                        destinationSlot.Quantity += moveQuantity;
                        sourceSlot.Quantity -= moveQuantity;

                        if (sourceSlot.Quantity < 1)
                        {
                            sourceSlot.Set(Item.None);
                        }
                    }
                    else
                    {
                        /* Items are not stackable, swap them */
                        destinationSlot.Set(sourceSlot);
                        sourceSlot.Set(temporarySlot);
                    }
                }
                else
                {
                    /* Items are different, swap them */
                    destinationSlot.Set(sourceSlot);
                    sourceSlot.Set(temporarySlot);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Error in SwapBankItems({slotFrom}, {slotTo}) for {_player.Id}");
                return;
            }

            if (_guild != null)
            {
                DbInterface.Pool.QueueWorkItem(_guild.Save);
            }
        }

        SendBankUpdate(slotFrom);
        SendBankUpdate(slotTo);
    }


    public void Dispose()
    {
        SendCloseBank();
        _player.GuildBank = false;
        _player.BankInterface = null;
    }
}
