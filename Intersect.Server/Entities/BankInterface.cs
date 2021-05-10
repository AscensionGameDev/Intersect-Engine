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
    public class BankInterface
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
                            mBank[slot].StatBuffs
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
                        mBank[slot].StatBuffs
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


        public bool TryDepositItem(int slot, int amount, bool sendUpdate = true)
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

            var itemBase = mPlayer.Items[slot].Descriptor;
            if (itemBase != null)
            {
                if (mPlayer.Items[slot].ItemId != Guid.Empty)
                {
                    lock (mLock)
                    {
                        if (itemBase.IsStackable)
                        {
                            if (amount >= mPlayer.Items[slot].Quantity)
                            {
                                amount = mPlayer.Items[slot].Quantity;
                            }
                        }
                        else
                        {
                            amount = 1;
                        }

                        //Find a spot in the bank for it!
                        if (itemBase.IsStackable)
                        {
                            for (var i = 0; i < mMaxSlots; i++)
                            {
                                if (mBank[i] != null && mBank[i].ItemId == mPlayer.Items[slot].ItemId)
                                {
                                    amount = Math.Min(amount, int.MaxValue - mBank[i].Quantity);
                                    mBank[i].Quantity += amount;

                                    //Remove Items from inventory send updates
                                    if (amount >= mPlayer.Items[slot].Quantity)
                                    {
                                        mPlayer.Items[slot].Set(Item.None);
                                        mPlayer.EquipmentProcessItemLoss(slot);
                                    }
                                    else
                                    {
                                        mPlayer.Items[slot].Quantity -= amount;
                                    }

                                    if (sendUpdate)
                                    {
                                        PacketSender.SendInventoryItemUpdate(mPlayer, slot);
                                        SendBankUpdate(i);
                                    }

                                    if (mGuild != null)
                                    {
                                        DbInterface.Pool.QueueWorkItem(mGuild.Save);
                                    }

                                    return true;
                                }
                            }
                        }

                        //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                        for (var i = 0; i < mMaxSlots; i++)
                        {
                            if (mBank[i] == null || mBank[i].ItemId == Guid.Empty)
                            {
                                mBank[i].Set(mPlayer.Items[slot]);
                                mBank[i].Quantity = amount;

                                //Remove Items from inventory send updates
                                if (amount >= mPlayer.Items[slot].Quantity)
                                {
                                    mPlayer.Items[slot].Set(Item.None);
                                    mPlayer.EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    mPlayer.Items[slot].Quantity -= amount;
                                }

                                if (sendUpdate)
                                {
                                    PacketSender.SendInventoryItemUpdate(mPlayer, slot);
                                    SendBankUpdate(i);
                                }

                                if (mGuild != null)
                                {
                                    DbInterface.Pool.QueueWorkItem(mGuild.Save);
                                }

                                return true;
                            }
                        }

                        PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank, CustomColors.Alerts.Error);
                    }
                }
                else
                {
                    PacketSender.SendChatMsg(mPlayer, Strings.Banks.depositinvalid, ChatMessageType.Bank, CustomColors.Alerts.Error);
                }
            }

            return false;
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
                lock (mLock)
                {
                    // Find a spot in the bank for it!
                    if (itemBase.IsStackable)
                    {
                        for (var i = 0; i < mMaxSlots; i++)
                        {
                            var bankSlot = mBank[i];
                            if (bankSlot != null && bankSlot.ItemId == item.ItemId)
                            {
                                if (item.Quantity <= int.MaxValue - bankSlot.Quantity)
                                {
                                    bankSlot.Quantity += item.Quantity;

                                    if (sendUpdate)
                                    {
                                        SendBankUpdate(i);
                                    }

                                    if (mGuild != null)
                                    {
                                        DbInterface.Pool.QueueWorkItem(mGuild.Save);
                                    }

                                    return true;
                                }
                            }
                        }
                    }

                    // Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (var i = 0; i < mMaxSlots; i++)
                    {
                        var bankSlot = mBank[i];

                        if (bankSlot == null || bankSlot.ItemId == Guid.Empty)
                        {
                            bankSlot.Set(item);

                            if (sendUpdate)
                            {
                                SendBankUpdate(i);
                            }

                            if (mGuild != null)
                            {
                                DbInterface.Pool.QueueWorkItem(mGuild.Save);
                            }

                            return true;
                        }
                    }
                }

                PacketSender.SendChatMsg(mPlayer, Strings.Banks.banknospace, ChatMessageType.Bank, CustomColors.Alerts.Error);
            }
            else
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.depositinvalid, ChatMessageType.Bank, CustomColors.Alerts.Error);
            }

            return false;
        }

        public void WithdrawItem(int slot, int amount)
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
            var inventorySlot = -1;
            if (itemBase == null)
            {
                return;
            }

            if (bankSlotItem.ItemId != Guid.Empty)
            {
                lock (mLock)
                {
                    if (itemBase.IsStackable)
                    {
                        if (amount >= bankSlotItem.Quantity)
                        {
                            amount = bankSlotItem.Quantity;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }

                    //Find a spot in the inventory for it!
                    if (itemBase.IsStackable)
                    {
                        /* Find an existing stack */
                        for (var i = 0; i < Options.MaxInvItems; i++)
                        {
                            var inventorySlotItem = mPlayer.Items[i];
                            if (inventorySlotItem == null)
                            {
                                continue;
                            }

                            if (inventorySlotItem.ItemId != bankSlotItem.ItemId)
                            {
                                continue;
                            }

                            inventorySlot = i;

                            break;
                        }
                    }

                    if (inventorySlot < 0)
                    {
                        /* Find a free slot if we don't have one already */
                        for (var j = 0; j < Options.MaxInvItems; j++)
                        {
                            if (mPlayer.Items[j] != null && mPlayer.Items[j].ItemId != Guid.Empty)
                            {
                                continue;
                            }

                            inventorySlot = j;

                            break;
                        }
                    }

                    /* If we don't have a slot send an error. */
                    if (inventorySlot < 0)
                    {
                        PacketSender.SendChatMsg(mPlayer, Strings.Banks.inventorynospace, ChatMessageType.Inventory, CustomColors.Alerts.Error);

                        return; //Panda forgot this :P
                    }

                    /* Move the items to the inventory */
                    Debug.Assert(mPlayer.Items[inventorySlot] != null, "Inventory[inventorySlot] != null");
                    amount = Math.Min(amount, int.MaxValue - mPlayer.Items[inventorySlot].Quantity);

                    if (mPlayer.Items[inventorySlot] == null ||
                        mPlayer.Items[inventorySlot].ItemId == Guid.Empty ||
                        mPlayer.Items[inventorySlot].Quantity < 0)
                    {
                        mPlayer.Items[inventorySlot].Set(bankSlotItem);
                        mPlayer.Items[inventorySlot].Quantity = 0;
                    }

                    mPlayer.Items[inventorySlot].Quantity += amount;
                    if (amount >= bankSlotItem.Quantity)
                    {
                        mBank[slot].Set(Item.None);
                    }
                    else
                    {
                        bankSlotItem.Quantity -= amount;
                    }

                    PacketSender.SendInventoryItemUpdate(mPlayer, inventorySlot);
                    SendBankUpdate(slot);

                    if (mGuild != null)
                    {
                        DbInterface.Pool.QueueWorkItem(mGuild.Save);
                    }
                }
            }
            else
            {
                PacketSender.SendChatMsg(mPlayer, Strings.Banks.withdrawinvalid, ChatMessageType.Bank, CustomColors.Alerts.Error);
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
