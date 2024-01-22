using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Localization;
using Intersect.Utilities;
using Serilog;
using Log = Intersect.Logging.Log;

namespace Intersect.Server.Entities
{
    public partial class BankInterface
    {
        private Player mPlayer;

        private IList<Item> mBank;

        private Guild mGuild;

        private object mLock;

        private int mMaxSlots;

        public BankInterface(Player player, IList<Item> bank, object bankLock, Guild guild, int maxSlots)
        {
            mPlayer = player;
            mBank = bank;
            mGuild = guild;
            mLock = bankLock;
            mMaxSlots = maxSlots;
        }

        public void SendOpenBank()
        {
            var items = new List<BankUpdatePacket>();

            for (var slot = 0; slot < mMaxSlots; slot++)
            {
                if (mBank[slot] != null && mBank[slot].ItemId != Guid.Empty && mBank[slot].Quantity > 0)
                {
                    items.Add(
                        new BankUpdatePacket(
                            slot, mBank[slot].ItemId, mBank[slot].Quantity, mBank[slot].BagId,
                            mBank[slot].Properties
                        )
                    );
                }
                else
                {
                    items.Add(new BankUpdatePacket(slot, Guid.Empty, 0, null, null));
                }
            }

            mPlayer?.SendPacket(new BankPacket(false, mGuild != null, mMaxSlots, items.ToArray()));
        }

        //BankUpdatePacket
        public void SendBankUpdate(int slot, bool sendToAll = true)
        {
            if (sendToAll && mGuild != null)
            {
                mGuild.BankSlotUpdated(slot);
                return;
            }

            if (mBank[slot] != null && mBank[slot].ItemId != Guid.Empty && mBank[slot].Quantity > 0)
            {
                mPlayer?.SendPacket(
                    new BankUpdatePacket(
                        slot, mBank[slot].ItemId, mBank[slot].Quantity, mBank[slot].BagId,
                        mBank[slot].Properties
                    )
                );
            }
            else
            {
                mPlayer?.SendPacket(new BankUpdatePacket(slot, Guid.Empty, 0, null, null));
            }
        }

        public void SendCloseBank()
        {
            mPlayer?.SendPacket(new BankPacket(true, false, -1, null));
        }

        public bool TryDepositItem(Item? slot, int inventorySlotIndex, int quantityHint, int bankSlotIndex = -1, bool sendUpdate = true)
        {
            //Permission Check
            if (mGuild != null)
            {
                var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, mPlayer.GuildRank))];
                if (!rank.Permissions.BankDeposit && mPlayer.GuildRank != 0)
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Guilds.NotAllowedDeposit.ToString(mGuild.Name), ChatMessageType.Bank, CustomColors.Alerts.Error);
                    return false;
                }
            }

            slot ??= mPlayer.Items[inventorySlotIndex];
            if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
            {
                PacketSender.SendChatMsg(
                    mPlayer,
                    Strings.Banks.depositinvalid,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var canBank = (itemDescriptor.CanBank && mGuild == default) ||
                          (itemDescriptor.CanGuildBank && mGuild != default);

            if (!canBank)
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Items.nobank, ChatMessageType.Bank, CustomColors.Items.Bound);
                return false;
            }

            var sourceSlots = mPlayer.Items.ToArray();
            var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxBankStack : 1;
            var sourceQuantity = Item.FindQuantityOfItem(itemDescriptor.Id, sourceSlots);

            var targetSlots = mBank.ToArray();

            lock (mLock)
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
                        mPlayer,
                        Strings.Banks.banknospace,
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
                        mPlayer,
                        Strings.Banks.withdrawinvalid,
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
                        Log.Warn($"Ran out of slots to remove from for {mPlayer.Id}");
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
                        Log.Error($"Potential inventory corruption for {mPlayer.Id}");
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
                    Log.Error($"{mPlayer.Id} was accidentally given {-remainingQuantity}x extra {itemDescriptor.Id}");
                }
                else if (remainingQuantity > 0)
                {
                    Log.Error($"{mPlayer.Id} was not given {remainingQuantity}x {itemDescriptor.Id}");
                }

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (remainingQuantityToRemove < 0)
                {
                    Log.Error($"{mPlayer.Id} was scammed {-remainingQuantity}x {itemDescriptor.Id}");
                }
                else if (remainingQuantityToRemove > 0)
                {
                    Log.Error($"{mPlayer.Id} did not have {remainingQuantity}x {itemDescriptor.Id} taken");
                }

                if (itemDescriptor.ItemType == ItemType.Equipment)
                {
                    if (inventorySlotIndex > -1)
                    {
                        mPlayer.EquipmentProcessItemLoss(inventorySlotIndex);
                    }
                }

                if (sendUpdate)
                {
                    foreach (var slotIndexToUpdate in slotIndicesToRemoveFrom)
                    {
                        PacketSender.SendInventoryItemUpdate(mPlayer, slotIndexToUpdate);
                    }

                    foreach (var slotIndexToFill in slotIndicesToFill)
                    {
                        SendBankUpdate(slotIndexToFill);
                    }

                    if (inventorySlotIndex > -1)
                    {
                        PacketSender.SendInventoryItemUpdate(mPlayer, inventorySlotIndex);
                    }
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }

                var successMessage = movableQuantity > 1
                    ? Strings.Banks.DepositSuccessStackable.ToString(movableQuantity, itemDescriptor.Name)
                    : Strings.Banks.DepositSuccessNonStackable.ToString(itemDescriptor.Name);

                PacketSender.SendChatMsg(mPlayer, successMessage, ChatMessageType.Bank, CustomColors.Alerts.Success);

                return true;
            }
        }

        public bool TryDepositItem(Item item, bool sendUpdate = true) =>
            TryDepositItem(item, -1, item.Quantity, -1, sendUpdate);

        public bool TryWithdrawItem(Item? slot, int bankSlotIndex, int quantityHint, int inventorySlotIndex = -1)
        {
            //Permission Check
            if (mGuild != null)
            {
                var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, mPlayer.GuildRank))];
                if (!rank.Permissions.BankRetrieve && mPlayer.GuildRank != 0)
                {
                    PacketSender.SendChatMsg(
                        mPlayer,
                        Strings.Guilds.NotAllowedWithdraw.ToString(mGuild.Name),
                        ChatMessageType.Bank,
                        CustomColors.Alerts.Error
                    );
                    return false;
                }
            }

            slot ??= mBank[bankSlotIndex];
            if (!ItemBase.TryGet(slot.ItemId, out var itemDescriptor))
            {
                PacketSender.SendChatMsg(
                    mPlayer,
                    Strings.Banks.withdrawinvalid,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                return false;
            }

            var maximumStack = itemDescriptor.Stackable ? itemDescriptor.MaxInventoryStack : 1;
            var sourceSlots = mBank.ToArray();
            var sourceQuantity = Item.FindQuantityOfItem(itemDescriptor.Id, sourceSlots);

            var targetSlots = mPlayer.Items.ToArray();

            lock (mLock)
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
                        mPlayer,
                        Strings.Banks.inventorynospace,
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
                        mPlayer,
                        Strings.Banks.withdrawinvalid,
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
                        Log.Warn($"Ran out of slots to remove from for {mPlayer.Id}");
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
                        Log.Error($"Potential bank corruption for {mPlayer.Id}");
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
                    Log.Error($"{mPlayer.Id} was accidentally given {-remainingQuantity}x extra {itemDescriptor.Id}");
                }
                else if (remainingQuantity > 0)
                {
                    Log.Error($"{mPlayer.Id} was not given {remainingQuantity}x {itemDescriptor.Id}");
                }

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (remainingQuantityToRemove < 0)
                {
                    Log.Error($"{mPlayer.Id} was scammed {-remainingQuantity}x {itemDescriptor.Id}");
                }
                else if (remainingQuantityToRemove > 0)
                {
                    Log.Error($"{mPlayer.Id} did not have {remainingQuantity}x {itemDescriptor.Id} taken");
                }

                foreach (var slotIndexToUpdate in slotIndicesToFill)
                {
                    PacketSender.SendInventoryItemUpdate(mPlayer, slotIndexToUpdate);
                }

                foreach (var slotIndexToUpdate in slotIndicesToRemoveFrom)
                {
                    SendBankUpdate(slotIndexToUpdate);
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }

                var successMessage = movableQuantity > 1
                    ? Strings.Banks.WithdrawSuccessStackable.ToString(quantityHint, itemDescriptor.Name)
                    : Strings.Banks.WithdrawSuccessNonStackable.ToString(itemDescriptor.Name);

                PacketSender.SendChatMsg(mPlayer, successMessage, ChatMessageType.Bank, CustomColors.Alerts.Success);

                return true;
            }
        }

        public void SwapBankItems(int slotFrom, int slotTo)
        {
            var bank = mBank;
            if (bank == null)
            {
                Log.Error($"SwapBankItems() called on invalid bank for {mPlayer.Id}");
                return;
            }

            if (Math.Clamp(slotFrom, 0, bank.Count - 1) != slotFrom || Math.Clamp(slotTo, 0, bank.Count - 1) != slotTo)
            {
                PacketSender.SendChatMsg(
                    mPlayer,
                    Strings.Banks.InvalidSlotToSwap,
                    ChatMessageType.Bank,
                    CustomColors.Alerts.Error
                );
                Log.Error($"Invalid slot indices SwapBankItems({slotFrom}, {slotTo}) ({bank.Count}, {mPlayer.Id})");
                return;
            }

            //Permission Check
            if (mGuild != null)
            {
                var ranks = Options.Instance.Guild.Ranks;
                var rankIndex = Math.Clamp(mPlayer.GuildRank, 0, ranks.Length - 1);
                var rank = ranks[rankIndex];
                if (mPlayer.GuildRank != rankIndex || (!rank.Permissions.BankMove && mPlayer.GuildRank != 0))
                {
                    PacketSender.SendChatMsg(
                        mPlayer,
                        Strings.Guilds.NotAllowedSwap.ToString(mGuild.Name),
                        ChatMessageType.Bank,
                        CustomColors.Alerts.Error
                    );
                    return;
                }
            }

            lock (mLock)
            {
                try
                {
                    var destinationSlot = (bank[slotTo] ??= Item.None);
                    var sourceSlot = (bank[slotFrom] ??= Item.None);
                    var temporarySlot = destinationSlot.Clone();

                    if (destinationSlot.ItemId == sourceSlot.ItemId)
                    {
                        if (sourceSlot.ItemId == default)
                        {
                            Log.Warn($"SwapBankItems({slotFrom}, {slotTo}) for {mPlayer.Id} with empty item ID");
                            return;
                        }

                        /* Items are the same, move the maximum quantity */
                        if (!ItemBase.TryGet(sourceSlot.ItemId, out var itemDescriptor))
                        {
                            Log.Error($"SwapBankItems({slotFrom}, {slotTo}) for {mPlayer.Id} failed due to missing item {sourceSlot.ItemId}");
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
                    Log.Error(exception, $"Error in SwapBankItems({slotFrom}, {slotTo}) for {mPlayer.Id}");
                    return;
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }
            }

            SendBankUpdate(slotFrom);
            SendBankUpdate(slotTo);
        }


        public void Dispose()
        {
            SendCloseBank();
            mPlayer.GuildBank = false;
            mPlayer.BankInterface = null;
        }
    }
}
