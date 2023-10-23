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


        public bool IsSlotOpen(int slot)
        {
            if (slot >= 0 && slot < mMaxSlots)
            {
                if (mBank[slot].ItemId == Guid.Empty)
                {
                    return true;
                }
            }

            return false;
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
        /// Retrieves a list of open bank slots for this instance.
        /// </summary>
        /// <returns>Returns the first open bank slot of this instance, or -1 if none are found.</returns>
        public int FindOpenSlot()
        {
            var slots = FindOpenSlots();
            if (slots.Count >= 1)
            {
                return slots.First();
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Finds a bank slot matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>Returns a slot that contains the item, or -1 if none are found.</returns>
        public int FindItemSlot(Guid itemId, int quantity = 1)
        {
            var slots = FindItemSlots(itemId, quantity);
            if (slots.Count >= 1)
            {
                return slots.First();
            }
            else
            {
                return -1;
            }
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
        /// Checks whether or not the bank instance can store this item.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check for.</param>
        /// <returns>Returns whether or not the bank instance can store this item.</returns>
        public bool CanStoreItem(Item item)
        {
            // We don't want to store null items !
            if (item.Descriptor == null)
            {
                return false;
            }

            // If this item isn't stackable we just need to find an open slot.
            if (!item.Descriptor.Stackable)
            {
                return FindOpenSlot() != -1;
            }

            // At this point, we assume that the item is stackable, let's fill up what we can.
            if (Math.Ceiling((double)item.Quantity / item.Descriptor.MaxBankStack) <=
                FindAvailableBankSpaceForItem(item.ItemId, item.Quantity))
            {
                return true;
            }

            return FindOpenSlot() != -1;
        }

        /// <summary>
        /// Checks whether or not the bank instance can store this item in a specific slot.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check for.</param>
        /// <param name="slot">The bank slot to check against.</param>
        /// <returns>Returns whether or not the bank instance can store this item, if an invalid slot is provided it will instead check with <see cref="CanStoreItem(Item)"/></returns>
        public bool CanStoreItem(Item item, int slot)
        {
            // Is this a valid item and slot?
            if (slot >= 0 && slot < mMaxSlots)
            {
                // Is this slot empty?
                if (IsSlotOpen(slot) || item.ItemId == mBank[slot].ItemId)
                {
                    // It is! Can we store the full quantity of this item though?
                    return CanStoreItem(item);
                }
            }
            else
            {
                // Not a valid slot, just treat it as a normal query.
                return CanStoreItem(item);
            }

            return false;
        }

        /// <summary>
        /// Finds the amount of available space in the bank for items.
        /// This method calculates the space available in the bank by checking
        /// the number of slots that have the same item ID as the provided item, and
        /// adding up the remaining quantity space in each of those slots. It also
        /// checks for any empty slots in the bank, and adds the maximum stack size
        /// of the provided item to the available space for each empty slot found.
        /// </summary>
        /// <param name="itemId">The ID of the item to find available space for.</param>
        /// <param name="amount">The requested amount of the item to deposit.</param>
        /// <returns>The maximum amount of the item that can be deposited in the bank.</returns>
        private int FindAvailableBankSpaceForItem(Guid itemId, int amount)
        {
            if (!ItemBase.TryGet(itemId, out var itemDescriptor))
            {
                return 0;
            }

            int spaceLeft = 0;
            int maxBankStack = itemDescriptor.MaxBankStack;

            if (spaceLeft >= amount)
            {
                return amount;
            }

            var bankSlots = mBank.ToArray();
            foreach (var bankSlot in bankSlots)
            {
                if (bankSlot != null && bankSlot.ItemId == itemId)
                {
                    spaceLeft += maxBankStack - bankSlot.Quantity;
                }
                else if (bankSlot == null || bankSlot.ItemId == Guid.Empty)
                {
                    spaceLeft += maxBankStack;
                }
            }

            return Math.Min(amount, spaceLeft);
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

        /// <summary>
        /// Checks whether or not this bank instance has enough items to accept a take operation.
        /// </summary>
        /// <param name="itemId">The ItemId to see if it can be taken away.</param>
        /// <param name="quantity">The quantity of above item to see if we can take away.</param>
        /// <returns>Whether or not the item can be taken away from the bank in the requested quantity.</returns>
        public bool CanTakeItem(Guid itemId, int quantity) => FindItemQuantity(itemId) >= quantity;

        public bool TryDepositItem(int slot, int amount, int destSlot = -1, bool sendUpdate = true)
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

            var inventoryItem = mPlayer.Items[slot];
            var inventoryItemItemId = inventoryItem.ItemId;

            if (inventoryItem == null || inventoryItemItemId == Guid.Empty)
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.depositinvalid, ChatMessageType.Bank,
                    CustomColors.Alerts.Error);
                return false;
            }

            if ((!inventoryItem.Descriptor.CanBank && mGuild == null) ||
                (!inventoryItem.Descriptor.CanGuildBank && mGuild != null))
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Items.nobank, ChatMessageType.Bank, CustomColors.Items.Bound);
                return false;
            }

            // Calculate the maximum amount that can be deposited, based on quantity and bank space.
            var inventoryQuantity = mPlayer.FindInventoryItemQuantity(inventoryItemItemId);
            amount = Math.Min(amount, inventoryQuantity);
            var availableStorageSpace = FindAvailableBankSpaceForItem(inventoryItemItemId, amount);
            var maxAmount = Math.Min(inventoryQuantity, availableStorageSpace);
            amount = Math.Min(amount, maxAmount);

            if (amount < 1)
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank,
                    CustomColors.Alerts.Error);
                return false;
            }

            if (amount > 1)
            {
                if (destSlot < 0 || mBank?[destSlot]?.ItemId != Guid.Empty)
                {
                    // Try to find an open slot with the same item type and not at maximum stack size
                    for (int i = 0; i < mMaxSlots; i++)
                    {
                        var bankItem = mBank[i];
                        if (bankItem.ItemId == Guid.Empty ||
                            (bankItem.ItemId != Guid.Empty && bankItem.ItemId == inventoryItemItemId &&
                             amount + bankItem.Quantity < bankItem.Descriptor.MaxBankStack))
                        {
                            destSlot = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (destSlot < 0 || mBank?[destSlot]?.ItemId != Guid.Empty)
                {
                    destSlot = FindOpenSlot();
                }

                if (destSlot < 0)
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank,
                        CustomColors.Alerts.Error);
                    return false;
                }
            }

            lock (mLock)
            {
                if (CanStoreItem(new Item(inventoryItemItemId, amount), destSlot))
                {
                    string successMessage;
                    Item itemToPut;

                    if (amount > 1)
                    {
                        successMessage = Strings.Banks.DepositSuccessStackable.ToString(amount, inventoryItem.ItemName);
                        itemToPut = new Item(inventoryItemItemId, amount);
                        mPlayer.TryTakeItem(inventoryItemItemId, amount, ItemHandling.Normal, sendUpdate);
                    }
                    else
                    {
                        successMessage = Strings.Banks.DepositSuccessNonStackable.ToString(inventoryItem.ItemName);
                        itemToPut = new Item(inventoryItemItemId, 1);
                        inventoryItem.Set(Item.None);
                        mPlayer.EquipmentProcessItemLoss(slot);
                    }

                    PacketSender.SendChatMsg(mPlayer, successMessage, ChatMessageType.Bank,
                        CustomColors.Alerts.Success);
                    PutItem(itemToPut, destSlot, sendUpdate);

                    if (sendUpdate)
                    {
                        PacketSender.SendInventoryItemUpdate(mPlayer, slot);
                    }

                    if (mGuild != null)
                    {
                        DbInterface.Pool.QueueWorkItem(mGuild.Save);
                    }

                    return true;
                }

                PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank,
                    CustomColors.Alerts.Error);
                return false;
            }
        }

        public bool TryDepositItem(Item item, bool sendUpdate = true)
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

            var itemBase = item.Descriptor;

            if (itemBase == null)
            {
                return false;
            }

            if (item.ItemId != Guid.Empty)
            {
                if ((!itemBase.CanBank && mGuild == null) || (!itemBase.CanGuildBank && mGuild != null))
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Items.nobank, ChatMessageType.Bank, CustomColors.Items.Bound);
                    return false;
                }

                lock (mLock)
                {
                    if (CanStoreItem(item))
                    {
                        PutItem(item, -1, sendUpdate);
                        return true;
                    }
                    else
                    {
                        PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank, CustomColors.Alerts.Error);
                    }
                }
            }
            else
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.depositinvalid, ChatMessageType.Bank, CustomColors.Alerts.Error);
            }

            return false;
        }

        /// <summary>
        /// Puts an item in the bank. NOTE: This method MAKES ZERO CHECKS to see if this is possible!
        /// Use TryDepositItem where possible!
        /// </summary>
        /// <param name="item"></param>
        /// <param name="destSlot">Set to -1 to ignore</param>
        /// <param name="sendUpdate"></param>
        private void PutItem(Item item, int destSlot, bool sendUpdate)
        {
            // Decide how we're going to handle this item.
            var existingSlots = FindItemSlots(item.Descriptor.Id);
            var updateSlots = new List<int>();
            if (item.Descriptor.Stackable && existingSlots.Count > 0) // Stackable, but already exists in the bank.
            {
                // So this gets complicated.. First let's hand out the quantity we can hand out before we hit a stack limit.
                var toGive = item.Quantity;
                foreach (var slot in existingSlots)
                {
                    if (toGive == 0)
                    {
                        break;
                    }

                    if (mBank[slot].Quantity >= item.Descriptor.MaxBankStack)
                    {
                        continue;
                    }

                    var canAdd = item.Descriptor.MaxBankStack - mBank[slot].Quantity;
                    if (canAdd > toGive)
                    {
                        mBank[slot].Quantity += toGive;
                        updateSlots.Add(slot);
                        toGive = 0;
                    }
                    else
                    {
                        mBank[slot].Quantity += canAdd;
                        updateSlots.Add(slot);
                        toGive -= canAdd;
                    }
                    
                }

                // Is there anything left to hand out? If so, hand out max stacks and what remains until we run out!
                if (toGive > 0)
                {
                    // Are we trying to put the item into a specific slot? If so, put as much in as possible!
                    if (destSlot != -1)
                    {
                        if (toGive > item.Descriptor.MaxBankStack)
                        {
                            mBank[destSlot].Set(new Item(item.ItemId, item.Descriptor.MaxBankStack));
                            updateSlots.Add(destSlot);
                            toGive -= item.Descriptor.MaxBankStack;
                        }
                        else
                        {
                            
                            mBank[destSlot].Set(new Item(item.ItemId, toGive));
                            updateSlots.Add(destSlot);
                            toGive = 0;
                        }
                    }

                    var openSlots = FindOpenSlots();
                    var total = toGive; // Copy this as we're going to be editing toGive.
                    for (var slot = 0; slot < Math.Ceiling((double)total / item.Descriptor.MaxBankStack); slot++)
                    {
                        var quantity = item.Descriptor.MaxBankStack <= toGive ?
                            item.Descriptor.MaxBankStack :
                            toGive;

                        toGive -= quantity;
                        mBank[openSlots[slot]].Set(new Item(item.ItemId, quantity));
                        updateSlots.Add(openSlots[slot]);
                    }
                }
            }
            else if (!item.Descriptor.Stackable && item.Quantity > 1) // Not stackable, but multiple items.
            {
                var toGive = item.Quantity;
                if (destSlot != -1)
                {
                    mBank[destSlot].Set(new Item(item.ItemId, 1));
                    updateSlots.Add(destSlot);
                    toGive -= 1;
                }

                var openSlots = FindOpenSlots();
                for (var slot = 0; slot < toGive; slot++)
                {
                    mBank[openSlots[slot]].Set(new Item(item.ItemId, 1));
                    updateSlots.Add(openSlots[slot]);
                }
            }
            else // Hand out without any special treatment. Either a single item or a stackable item we don't have yet.
            {
                // If the item is not stackable, or the amount is below our stack cap just blindly hand it out.
                if (!item.Descriptor.Stackable || item.Quantity < item.Descriptor.MaxBankStack)
                {
                    if (destSlot != -1)
                    {
                        mBank[destSlot].Set(item);
                        updateSlots.Add(destSlot);
                    }
                    else
                    {
                        var newSlot = FindOpenSlot();
                        mBank[newSlot].Set(item);
                        updateSlots.Add(newSlot);
                    }
                    
                }
                // The item is above our stack cap.. Let's start handing them phat stacks out!
                else
                {
                    var toGive = item.Quantity;

                    if (destSlot != -1)
                    {
                        mBank[destSlot].Set(new Item(item.ItemId, item.Descriptor.MaxBankStack));
                        updateSlots.Add(destSlot);
                        toGive -= item.Descriptor.MaxBankStack;
                    }

                    var openSlots = FindOpenSlots();
                    for (var slot = 0; slot < Math.Ceiling((double)item.Quantity / item.Descriptor.MaxBankStack); slot++)
                    {
                        var quantity = item.Descriptor.MaxBankStack <= toGive ?
                            item.Descriptor.MaxBankStack :
                            toGive;

                        toGive -= quantity;
                        mBank[openSlots[slot]].Set(new Item(item.ItemId, quantity));
                        updateSlots.Add(openSlots[slot]);
                    }
                }

            }

            // Do we need to update the bank display?
            if (sendUpdate)
            {
                foreach (var slot in updateSlots)
                {
                    SendBankUpdate(slot);
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }
            }
        }

        public void WithdrawItem(int slot, int amount, int invSlot = -1)
        {
            //Permission Check
            if (mGuild != null)
            {
                var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(Options.Instance.Guild.Ranks.Length - 1, mPlayer.GuildRank))];
                if (!rank.Permissions.BankRetrieve && mPlayer.GuildRank != 0)
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Guilds.NotAllowedWithdraw.ToString(mGuild.Name), ChatMessageType.Bank, CustomColors.Alerts.Error);
                    return;
                }
            }

            Debug.Assert(ItemBase.Lookup != null, "ItemBase.Lookup != null");
            Debug.Assert(mBank != null, "Bank != null");
            Debug.Assert(mPlayer.Items != null, "Inventory != null");

            var bankSlotItem = mBank[slot];
            if (bankSlotItem == null)
            {
                return;
            }

            var itemBase = bankSlotItem.Descriptor;
            if (itemBase == null)
            {
                return;
            }

            if (bankSlotItem.ItemId == default)
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.withdrawinvalid, ChatMessageType.Bank, CustomColors.Alerts.Error);
                return;
            }

            lock (mLock)
            {
                if (amount > 1)
                {
                    if (!CanTakeItem(itemBase.Id, amount))
                    {
                        amount = FindItemQuantity(itemBase.Id);
                    }
                }
                else
                {
                    amount = 1;
                }

                if (!mPlayer.CanGiveItem(itemBase.Id, amount, invSlot) || invSlot < 0)
                {
                    invSlot = mPlayer.FindOpenInventorySlot().Slot;
                }

                if (invSlot < 0)
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Banks.inventorynospace, ChatMessageType.Inventory, CustomColors.Alerts.Error);
                    return;
                }

                var toTake = amount;
                if (amount > 1)
                {
                    mPlayer.TryGiveItem(itemBase.Id, amount, ItemHandling.Normal, false, invSlot, true);

                    // Go through our bank and take what we need!
                    foreach (var s in FindItemSlots(itemBase.Id))
                    {
                        // Do we still have items to take? If not leave the loop!
                        if (toTake == 0)
                        {
                            break;
                        }

                        if (mBank[s].Quantity >= toTake)
                        {
                            TakeItem(s, toTake, true);
                            toTake = 0;
                        }
                        else // Take away the entire quantity of the item and lower our items that we still need to take!
                        {
                            toTake -= mBank[s].Quantity;
                            TakeItem(s, mBank[s].Quantity, true);
                        }
                    }
                }
                else
                {
                    mPlayer.TryGiveItem(mBank[slot], invSlot);
                    mBank[slot].Set(Item.None);
                    SendBankUpdate(slot);
                }

                if (mGuild != null)
                {
                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                }

                string successMessage;
                if (amount > 1)
                {
                    successMessage = Strings.Banks.WithdrawSuccessStackable.ToString(amount, itemBase.Name);
                }
                else
                {
                    successMessage = Strings.Banks.WithdrawSuccessNonStackable.ToString(itemBase.Name);
                }

                PacketSender.SendChatMsg(mPlayer, successMessage, ChatMessageType.Bank, CustomColors.Alerts.Success);
            }
        }

        /// <summary>
        /// Take an item away from the bank, or an amount of it if they have more. NOTE: This method MAKES ZERO CHECKS to see if this is possible!
        /// Use WithdrawItem where possible!
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        /// <param name="sendUpdate"></param>
        private void TakeItem(int slot, int amount, bool sendUpdate = true)
        {
            if (mBank[slot].Quantity > amount) // This slot contains more than what we're trying to take away here. Update the quantity.
            {
                mBank[slot].Quantity -= amount;
            }
            else // Take the entire thing away!
            {
                mBank[slot].Set(Item.None);
            }

            if (sendUpdate)
            {
                SendBankUpdate(slot);
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
