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

        /// <summary>
        /// Retrieves a list of open bank slots for this instance.
        /// </summary>
        /// <returns>Returns a list of open slots in this bank instance.</returns>
        public List<int> FindOpenSlots()
        {
            var slots = new List<int>();
            for (var i = 0; i < mMaxSlots; i++)
            {
                var bankSlot = mBank[i];

                if (bankSlot != null && bankSlot.ItemId == Guid.Empty)
                {
                    slots.Add(i);
                }
            }

            return slots;
        }

        /// <summary>
        /// Finds all bank slots matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>Returns a list of slots containing the requested item.</returns>
        public List<int> FindItemSlots(Guid itemId, int quantity = 1)
        {
            var slots = new List<int>();
            if (mBank == null)
            {
                return slots;
            }

            for (var i = 0; i < mMaxSlots; i++)
            {
                var item = mBank[i];
                if (item?.ItemId != itemId)
                {
                    continue;
                }

                if (item.Quantity >= quantity)
                {
                    slots.Add(i);
                }
            }

            return slots;
        }
        
        /// <summary>
        /// Find the amount of a specific item this bank instance has.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <returns>The amount of the requested item this bank instance contains.</returns>
        public int FindItemQuantity(Guid itemId)
        {
            if (mBank == null)
            {
                return 0;
            }

            long itemCount = 0;
            for (var i = 0; i < mMaxSlots; i++)
            {
                var item = mBank[i];
                if (item.ItemId == itemId)
                {
                    itemCount = item.Descriptor.Stackable ? itemCount += item.Quantity : itemCount += 1;
                }
            }

            // TODO: Stop using Int32 for item quantities
            return itemCount >= Int32.MaxValue ? Int32.MaxValue : (int)itemCount;
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

                var remainingQuantity = movableQuantity;
                foreach (var slotIndexToFill in slotIndicesToFill)
                {
                    if (remainingQuantity < 1)
                    {
                        break;
                    }

                    var slotToFill = targetSlots[slotIndexToFill];
                    Debug.Assert(slotToFill != default);
                    var quantityToStoreInSlot = Math.Min(remainingQuantity, maximumStack - slotToFill.Quantity);
                    slotToFill.ItemId = itemDescriptor.Id;
                    slotToFill.Quantity += quantityToStoreInSlot;
                    remainingQuantity -= quantityToStoreInSlot;
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

                var remainingQuantity = movableQuantity;
                foreach (var slotIndexToFill in slotIndicesToFill)
                {
                    if (remainingQuantity < 1)
                    {
                        break;
                    }

                    var slotToFill = targetSlots[slotIndexToFill];
                    Debug.Assert(slotToFill != default);
                    var quantityToStoreInSlot = Math.Min(remainingQuantity, maximumStack - slotToFill.Quantity);
                    slotToFill.ItemId = itemDescriptor.Id;
                    slotToFill.Quantity += quantityToStoreInSlot;
                    remainingQuantity -= quantityToStoreInSlot;
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

        public void SwapBankItems(int item1, int item2)
        {
            //Permission Check
            if (mGuild != null)
            {
                var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, mPlayer.GuildRank))];
                if (!rank.Permissions.BankMove && mPlayer.GuildRank != 0)
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Guilds.NotAllowedSwap.ToString(mGuild.Name), ChatMessageType.Bank, CustomColors.Alerts.Error);
                    return;
                }
            }

            Item tmpInstance = null;
            lock (mLock)
            {
                if (mBank != null)
                {
                    tmpInstance = mBank[item2].Clone();
                }

                if (mBank[item1] != null)
                {
                    mBank[item2].Set(mBank[item1]);
                }
                else
                {
                    mBank[item2].Set(Item.None);
                }

                if (tmpInstance != null)
                {
                    mBank[item1].Set(tmpInstance);
                }
                else
                {
                    mBank[item1].Set(Item.None);
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }
            }

            SendBankUpdate(item1);
            SendBankUpdate(item2);
        }


        public void Dispose()
        {
            SendCloseBank();
            mPlayer.GuildBank = false;
            mPlayer.BankInterface = null;
        }
    }
}
