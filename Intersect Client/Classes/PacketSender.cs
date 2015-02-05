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
            Gui._GameGui.EventDialogWindow.Hide();
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
    }
}
