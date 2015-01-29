using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectEditor
{
    public static class PacketSender
    {

        public static void SendPing()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(0);
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(5);
            bf.WriteString(username);
            bf.WriteString(password);
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(2);
            bf.WriteLong(mapNum);
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendTilesets()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong(6);
            bf.WriteLong(GlobalVariables.tilesets.Length);
            for (int i = 0; i < GlobalVariables.tilesets.Length; i++)
            {
                bf.WriteString(GlobalVariables.tilesets[i]);
            }
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendMap(int mapnum)
        {
            ByteBuffer bf = new ByteBuffer();
            byte[] mapData = GlobalVariables.GameMaps[mapnum].Save();
            bf.WriteLong(7);
            bf.WriteLong(mapnum);
            bf.WriteLong(mapData.Length);
            bf.WriteBytes(mapData);
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
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
            GlobalVariables.GameSocket.SendPacket(bf.ToArray());
        }
    }
}
