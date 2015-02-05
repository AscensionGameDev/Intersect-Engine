using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor
{
    public static class PacketSender
    {

        public static void SendPing()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(0);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(5);
            bf.WriteString(username);
            bf.WriteString(password);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(2);
            bf.WriteLong(mapNum);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendTilesets()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(6);
            bf.WriteLong(Globals.tilesets.Length);
            for (int i = 0; i < Globals.tilesets.Length; i++)
            {
                bf.WriteString(Globals.tilesets[i]);
            }
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendMap(int mapnum)
        {
            ByteBuffer bf = new ByteBuffer();
            byte[] mapData = Globals.GameMaps[mapnum].Save();
            bf.WriteLong(7);
            bf.WriteLong(mapnum);
            bf.WriteLong(mapData.Length);
            bf.WriteBytes(mapData);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendCreateMap(int location, int currentMap)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(8);
            bf.WriteLong(location);
            if (location > -1)
            {
                bf.WriteLong(currentMap);
            }
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendItemEditor()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(16);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendItem(int ItemNum, byte[] ItemData)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(17);
            bf.WriteLong(ItemNum);
            bf.WriteBytes(ItemData);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }
    }
}
