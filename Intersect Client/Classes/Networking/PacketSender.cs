namespace Intersect_Client.Classes
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.Ping);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.Login);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
            Network.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendMove()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SendMove);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentMap);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentX);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].CurrentY);
            bf.WriteInteger(Globals.Entities[Globals.MyIndex].Dir);
            Network.SendPacket(bf.ToArray());
       }

        public static void SendChatMsg(string msg)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.LocalMessage);
            bf.WriteString(msg);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendEnterMap()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EnterMap);
            bf.WriteLong(Globals.CurrentMap);
            Network.SendPacket(bf.ToArray());
            Graphics.LightsChanged = true;
        }

        public static void SendAttack(int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.TryAttack);
            bf.WriteLong(index);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendDir(int dir)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SendDir);
            bf.WriteLong(dir);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendEnterGame()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EnterGame);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendActivateEvent(int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.ActivateEvent);
            bf.WriteLong(eventIndex);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendEventResponse(int response, EventDialog ed)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EventResponse);
            bf.WriteInteger(ed.EventIndex);
            bf.WriteInteger(response);
            Globals.EventDialogs.Remove(ed);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendCreateAccount(string username, string password, string email)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.CreateAccount);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
            bf.WriteString(email.ToLower().Trim());
            Network.SendPacket(bf.ToArray());
        }

        public static void SendPickupItem(int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.PickupItem);
            bf.WriteInteger(index);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendSwapItems(int item1, int item2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SwapItems);
            bf.WriteInteger(item1);
            bf.WriteInteger(item2);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendDropItem(int slot, int amount)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.DropItems);
            bf.WriteInteger(slot);
            bf.WriteInteger(amount);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendUseItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UseItem);
            bf.WriteInteger(slot);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendSwapSpells(int spell1, int spell2)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SwapSpells);
            bf.WriteInteger(spell1);
            bf.WriteInteger(spell2);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendForgetSpell(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.ForgetSpell);
            bf.WriteInteger(slot);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendUseSpell(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UseSpell);
            bf.WriteInteger(slot);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendUnequipItem(int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UnequipItem);
            bf.WriteInteger(slot);
            Network.SendPacket(bf.ToArray());
        }

        public static void SendUpgradeStat(int stat)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.UpgradeStat);
            bf.WriteInteger(stat);
            Network.SendPacket(bf.ToArray());
        }

    }
}
