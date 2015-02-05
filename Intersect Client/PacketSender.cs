using System;

namespace Intersect_Client
{
    public static class PacketSender
    {
        public static void SendPing()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(0);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(1);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
            Network.sendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(2);
            bf.WriteLong(mapNum);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendMove()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(3);
            bf.WriteInteger(Globals.entities[Globals.myIndex].currentMap);
            bf.WriteInteger(Globals.entities[Globals.myIndex].currentX);
            bf.WriteInteger(Globals.entities[Globals.myIndex].currentY);
            bf.WriteInteger(Globals.entities[Globals.myIndex].dir);
            Network.sendPacket(bf.ToArray());
            //Debug.Log("Send Player Coords: x: " + (double)Globals.players[Globals.myIndex].SimulationState.position.x + " y: " + (double)Globals.players[Globals.myIndex].SimulationState.position.y);
        }

        public static void SendChatMsg(string msg)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(4);
            bf.WriteString(msg);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendEnterMap()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(9);
            bf.WriteLong(Globals.currentMap);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendAttack(int index)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(10);
            bf.WriteLong(index);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendDir(int dir)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(11);
            bf.WriteLong(dir);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendEnterGame()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(12);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendActivateEvent(int eventIndex)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(13);
            bf.WriteLong(eventIndex);
            Network.sendPacket(bf.ToArray());
        }

        public static void SendEventResponse(int response, EventDialog ed)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(14);
            bf.WriteInteger(ed.eventIndex);
            bf.WriteInteger(response);
            Globals.EventDialogs.Remove(ed);
            GUI.g_eventDialogWindow.Hide();
            Network.sendPacket(bf.ToArray());
        }

        public static void SendCreateAccount(string username, string password, string email)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(15);
            bf.WriteString(username.ToLower().Trim());
            bf.WriteString(password.Trim());
            bf.WriteString(email.ToLower().Trim());
            Network.sendPacket(bf.ToArray());
        }
    }
}
