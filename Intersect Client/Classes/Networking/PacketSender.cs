using System;
using System.Security.Cryptography;
using System.Text;
using Intersect;
using Intersect.Enums;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.UI.Game;

namespace Intersect_Client.Classes.Networking
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.Ping);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password, string version)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.Login);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
			bf.WriteString(version);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendLogout()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ClientPackets.Logout);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.NeedMap);
            bf.WriteGuid(mapId);
            GameNetwork.SendPacket(bf.ToArray());
            if (MapInstance.MapRequests.ContainsKey(mapId))
            {
                MapInstance.MapRequests[mapId] = Globals.System.GetTimeMs() + 3000;
            }
            else
            {
                MapInstance.MapRequests.Add(mapId, Globals.System.GetTimeMs() + 3000);
            }
        }

        public static void SendMove()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SendMove);
            bf.WriteGuid(Globals.Me.CurrentMap);
            bf.WriteInteger(Globals.Me.CurrentX);
            bf.WriteInteger(Globals.Me.CurrentY);
            bf.WriteInteger(Globals.Me.Dir);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendChatMsg(string msg, int channel)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.LocalMessage);
            bf.WriteString(msg);
            bf.WriteInteger(channel);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAttack(Guid targetId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TryAttack);
            bf.WriteGuid(targetId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendBlock(int blocking)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TryBlock);
            bf.WriteInteger(blocking);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDir(int dir)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SendDir);
            bf.WriteLong(dir);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendEnterGame()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EnterGame);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendActivateEvent(Guid eventId, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.ActivateEvent);
            bf.WriteGuid(eventId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendEventResponse(int response, EventDialog ed)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.EventResponse);
            bf.WriteGuid(ed.EventId);
            bf.WriteInteger(response);
            Globals.EventDialogs.Remove(ed);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCreateAccount(string username, string password, string email)
        {
            var bf = new ByteBuffer();
            var sha = new SHA256Managed();
            bf.WriteLong((int) ClientPackets.CreateAccount);
            bf.WriteString(username.Trim());
            bf.WriteString(BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password.Trim())))
                .Replace("-", ""));
            bf.WriteString(email.Trim());
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCreateCharacter(string name, Guid classId, int sprite)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CreateCharacter);
            bf.WriteString(name.Trim());
            bf.WriteGuid(classId);
            bf.WriteInteger(sprite);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPickupItem(int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PickupItem);
            bf.WriteInteger(index);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendSwapItems(int item1, int item2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SwapItems);
            bf.WriteInteger(item1);
            bf.WriteInteger(item2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDropItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DropItems);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUseItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UseItem);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendSwapSpells(int spell1, int spell2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SwapSpells);
            bf.WriteInteger(spell1);
            bf.WriteInteger(spell2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendForgetSpell(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.ForgetSpell);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUseSpell(int slot, Guid targetId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UseSpell);
            bf.WriteInteger(slot);
            bf.WriteGuid(targetId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUnequipItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UnequipItem);
            bf.WriteInteger(slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendUpgradeStat(int stat)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.UpgradeStat);
            bf.WriteInteger(stat);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendHotbarChange(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.HotbarChange);
            bf.WriteInteger(slot);
            bf.WriteInteger(Globals.Me.Hotbar[slot].Type);
            bf.WriteInteger(Globals.Me.Hotbar[slot].Slot);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendOpenAdminWindow()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.OpenAdminWindow);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAdminAction(int action, string val1 = "", string val2 = "", string val3 = "",
            string val4 = "", Guid val5 = new Guid())
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.AdminAction);
            bf.WriteInteger(action);
            bf.WriteString(val1);
            bf.WriteString(val2);
            bf.WriteString(val3);
            bf.WriteString(val4);
            bf.WriteGuid(val5);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendSellItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.SellItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendBuyItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.BuyItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCloseShop()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CloseShop);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDepositItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DepositItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendWithdrawItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.WithdrawItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCloseBank()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CloseBank);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCloseCraftingTable()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CloseCraftingTable);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendMoveBankItems(int slot1, int slot2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MoveBankItem);
            bf.WriteInteger(slot1);
            bf.WriteInteger(slot2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCraftItem(Guid id)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CraftItem);
            bf.WriteGuid(id);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPartyInvite(Guid targetId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PartyInvite);
            bf.WriteGuid(targetId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPartyKick(Guid targetId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PartyKick);
            bf.WriteGuid(targetId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPartyLeave()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PartyLeave);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPartyAccept(object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PartyAcceptInvite);
            bf.WriteGuid((Guid) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendPartyDecline(object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PartyDeclineInvite);
            bf.WriteGuid((Guid) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAcceptQuest(Guid questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.AcceptQuest);
            bf.WriteGuid(questId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDeclineQuest(Guid questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DeclineQuest);
            bf.WriteGuid(questId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCancelQuest(Guid questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CancelQuest);
            bf.WriteGuid(questId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendTradeRequest(Guid targetId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeRequest);
            bf.WriteGuid(targetId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendOfferItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeOffer);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendRevokeItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeRevoke);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendAcceptTrade()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeAccept);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendDeclineTrade()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeDecline);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendTradeRequestAccept(object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeRequestAccept);
            bf.WriteGuid((Guid) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendTradeRequestDecline(object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.TradeRequestDecline);
            bf.WriteGuid((Guid) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendStoreBagItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.StoreBagItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendRetreiveBagItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.RetreiveBagItem);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendCloseBag()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CloseBag);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendMoveBagItems(int slot1, int slot2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.MoveBagItem);
            bf.WriteInteger(slot1);
            bf.WriteInteger(slot2);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void RequestFriends()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.RequestFriends);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void AddFriend(string name)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.AddFriend);
            bf.WriteString(name);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void RemoveFriend(string name)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.RemoveFriend);
            bf.WriteString(name);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendFriendRequestAccept(Object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.FriendRequestAccept);
            bf.WriteInteger((int) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void SendFriendRequestDecline(Object sender, EventArgs e)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.FriendRequestDecline);
            bf.WriteInteger((int) ((InputBox) sender).UserData);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void PlayGame(Guid charId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.PlayGame);
            bf.WriteGuid(charId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void DeleteChar(Guid charId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.DeleteChar);
            bf.WriteGuid(charId);
            GameNetwork.SendPacket(bf.ToArray());
        }

        public static void CreateNewCharacter()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ClientPackets.CreateNewChar);
            GameNetwork.SendPacket(bf.ToArray());
        }
    }
}