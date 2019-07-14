using System;
using System.Security.Cryptography;
using System.Text;

using Intersect.Admin.Actions;
using Intersect.Client.Entities.Events;
using Intersect.Client.General;
using Intersect.Client.Maps;
using Intersect.Client.UI.Game;
using Intersect.Enums;
using Intersect.Network.Packets.Client;

namespace Intersect.Client.Networking
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            GameNetwork.SendPacket(new PingPacket());
        }

        public static void SendLogin(string username, string password)
        {
            GameNetwork.SendPacket(new LoginPacket(username, password));
        }

        public static void SendLogout(bool characterSelect = false)
        {
            GameNetwork.SendPacket(new LogoutPacket(characterSelect));
        }

        public static void SendNeedMap(Guid mapId)
        {
            GameNetwork.SendPacket(new NeedMapPacket(mapId));
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
            GameNetwork.SendPacket(new MovePacket(Globals.Me.CurrentMap,Globals.Me.X, Globals.Me.Y, Globals.Me.Dir));
        }

        public static void SendChatMsg(string msg, byte channel)
        {
            GameNetwork.SendPacket(new ChatMsgPacket(msg,channel));
        }

        public static void SendAttack(Guid targetId)
        {
            GameNetwork.SendPacket(new AttackPacket(targetId));
        }

        public static void SendBlock(bool blocking)
        {
            GameNetwork.SendPacket(new BlockPacket(blocking));
        }

        public static void SendDirection(byte dir)
        {
            GameNetwork.SendPacket(new DirectionPacket(dir));
        }

        public static void SendEnterGame()
        {
            GameNetwork.SendPacket(new EnterGamePacket());
        }

        public static void SendActivateEvent(Guid eventId)
        {
            GameNetwork.SendPacket(new ActivateEventPacket(eventId));
        }

        public static void SendEventResponse(byte response, EventDialog ed)
        {
            Globals.EventDialogs.Remove(ed);
            GameNetwork.SendPacket(new EventResponsePacket(ed.EventId,response));
        }

        public static void SendCreateAccount(string username, string password, string email)
        {
            GameNetwork.SendPacket(new CreateAccountPacket(username.Trim(),password.Trim(),email.Trim()));
        }

        public static void SendCreateCharacter(string name, Guid classId, int sprite)
        {
            GameNetwork.SendPacket(new CreateCharacterPacket(name,classId,sprite));
        }

        public static void SendPickupItem(int index)
        {
            GameNetwork.SendPacket(new PickupItemPacket(index));
        }

        public static void SendSwapInvItems(int item1, int item2)
        {
            GameNetwork.SendPacket(new SwapInvItemsPacket(item1,item2));
        }

        public static void SendDropItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new DropItemPacket(slot,amount));
        }

        public static void SendUseItem(int slot)
        {
            GameNetwork.SendPacket(new UseItemPacket(slot));
        }

        public static void SendSwapSpells(int spell1, int spell2)
        {
            GameNetwork.SendPacket(new SwapSpellsPacket(spell1,spell2));
        }

        public static void SendForgetSpell(int slot)
        {
            GameNetwork.SendPacket(new ForgetSpellPacket(slot));
        }

        public static void SendUseSpell(int slot, Guid targetId)
        {
            GameNetwork.SendPacket(new UseSpellPacket(slot,targetId));
        }

        public static void SendUnequipItem(int slot)
        {
            GameNetwork.SendPacket(new UnequipItemPacket(slot));
        }

        public static void SendUpgradeStat(byte stat)
        {
            GameNetwork.SendPacket(new UpgradeStatPacket(stat));
        }

        public static void SendHotbarUpdate(byte hotbarSlot, sbyte type, int itemIndex)
        {
            GameNetwork.SendPacket(new HotbarUpdatePacket(hotbarSlot,type,itemIndex));
        }

        public static void SendHotbarSwap(byte index, byte swapIndex)
        {
            GameNetwork.SendPacket(new HotbarSwapPacket(index,swapIndex));
        }

        public static void SendOpenAdminWindow()
        {
            GameNetwork.SendPacket(new OpenAdminWindowPacket());
        }

        //Admin Action Packet Should be Here

        public static void SendSellItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new SellItemPacket(slot,amount));
        }

        public static void SendBuyItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new BuyItemPacket(slot,amount));
        }

        public static void SendCloseShop()
        {
            GameNetwork.SendPacket(new CloseShopPacket());
        }

        public static void SendDepositItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new DepositItemPacket(slot,amount));
        }

        public static void SendWithdrawItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new WithdrawItemPacket(slot,amount));
        }

        public static void SendCloseBank()
        {
            GameNetwork.SendPacket(new CloseBankPacket());
        }

        public static void SendCloseCrafting()
        {
            GameNetwork.SendPacket(new CloseCraftingPacket());
        }

        public static void SendMoveBankItems(int slot1, int slot2)
        {
            GameNetwork.SendPacket(new SwapBankItemsPacket(slot1,slot2));
        }

        public static void SendCraftItem(Guid id)
        {
            GameNetwork.SendPacket(new CraftItemPacket(id));
        }

        public static void SendPartyInvite(Guid targetId)
        {
            GameNetwork.SendPacket(new PartyInvitePacket(targetId));
        }

        public static void SendPartyKick(Guid targetId)
        {
            GameNetwork.SendPacket(new PartyKickPacket(targetId));
        }

        public static void SendPartyLeave()
        {
            GameNetwork.SendPacket(new PartyLeavePacket());
        }

        public static void SendPartyAccept(object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new PartyInviteResponsePacket((Guid)((InputBox)sender).UserData,true));
        }

        public static void SendPartyDecline(object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new PartyInviteResponsePacket((Guid)((InputBox)sender).UserData, false));
        }

        public static void SendAcceptQuest(Guid questId)
        {
            GameNetwork.SendPacket(new QuestResponsePacket(questId,true));
        }

        public static void SendDeclineQuest(Guid questId)
        {
            GameNetwork.SendPacket(new QuestResponsePacket(questId,false));
        }

        public static void SendAbandonQuest(Guid questId)
        {
            GameNetwork.SendPacket(new AbandonQuestPacket(questId));
        }

        public static void SendTradeRequest(Guid targetId)
        {
            GameNetwork.SendPacket(new TradeRequestPacket(targetId));
        }

        public static void SendOfferTradeItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new OfferTradeItemPacket(slot,amount));
        }

        public static void SendRevokeTradeItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new RevokeTradeItemPacket(slot,amount));
        }

        public static void SendAcceptTrade()
        {
            GameNetwork.SendPacket(new AcceptTradePacket());
        }

        public static void SendDeclineTrade()
        {
            GameNetwork.SendPacket(new DeclineTradePacket());
        }

        public static void SendTradeRequestAccept(object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new TradeRequestResponsePacket((Guid)((InputBox)sender).UserData,true));
        }

        public static void SendTradeRequestDecline(object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new TradeRequestResponsePacket((Guid)((InputBox)sender).UserData, false));
        }

        public static void SendStoreBagItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new StoreBagItemPacket(slot,amount));
        }

        public static void SendRetrieveBagItem(int slot, int amount)
        {
            GameNetwork.SendPacket(new RetrieveBagItemPacket(slot,amount));
        }

        public static void SendCloseBag()
        {
            GameNetwork.SendPacket(new CloseBagPacket());
        }

        public static void SendMoveBagItems(int slot1, int slot2)
        {
            GameNetwork.SendPacket(new SwapBagItemsPacket(slot1,slot2));
        }

        public static void SendRequestFriends()
        {
            GameNetwork.SendPacket(new RequestFriendsPacket());
        }

        public static void SendAddFriend(string name)
        {
            GameNetwork.SendPacket(new UpdateFriendsPacket(name,true));
        }

        public static void SendRemoveFriend(string name)
        {
            GameNetwork.SendPacket(new UpdateFriendsPacket(name,false));
        }

        public static void SendFriendRequestAccept(Object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new FriendRequestResponsePacket((Guid)((InputBox)sender).UserData,true));
        }

        public static void SendFriendRequestDecline(Object sender, EventArgs e)
        {
            GameNetwork.SendPacket(new FriendRequestResponsePacket((Guid)((InputBox)sender).UserData,false));
        }

        public static void SendSelectCharacter(Guid charId)
        {
            GameNetwork.SendPacket(new SelectCharacterPacket(charId));
        }

        public static void SendDeleteCharacter(Guid charId)
        {
            GameNetwork.SendPacket(new DeleteCharacterPacket(charId));
        }

        public static void SendNewCharacter()
        {
            GameNetwork.SendPacket(new NewCharacterPacket());
        }

        public static void SendRequestPasswordReset(string nameEmail)
        {
            GameNetwork.SendPacket(new RequestPasswordResetPacket(nameEmail));
        }

        public static void SendResetPassword(string nameEmail, string code, string hashedPass)
        {
            GameNetwork.SendPacket(new ResetPasswordPacket(nameEmail,code,hashedPass));
        }

        public static void SendAdminAction(AdminAction action)
        {
            GameNetwork.SendPacket(new AdminActionPacket(action));
        }

        public static void SendBumpEvent(Guid mapId, Guid eventId)
        {
            GameNetwork.SendPacket(new BumpPacket(mapId, eventId));
        }
    }
}