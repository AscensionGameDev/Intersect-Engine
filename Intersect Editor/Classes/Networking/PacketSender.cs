/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
namespace Intersect_Editor.Classes
{
    public static class PacketSender
    {

        public static void SendPing()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.Ping);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendLogin(string username, string password)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.EditorLogin);
            bf.WriteString(username);
            bf.WriteString(password);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendNeedMap(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.NeedMap);
            bf.WriteLong(mapNum);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendTilesets()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SaveTilesetArray);
            bf.WriteLong(Globals.Tilesets.Length);
            foreach (var t in Globals.Tilesets)
            {
                bf.WriteString(t);
            }
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendMap(int mapnum)
        {
            var bf = new ByteBuffer();
            var mapData = Globals.GameMaps[mapnum].Save();
            bf.WriteLong((int)Enums.ClientPackets.SaveMap);
            bf.WriteLong(mapnum);
            bf.WriteLong(mapData.Length);
            bf.WriteBytes(mapData);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendCreateMap(int location, int currentMap)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.CreateMap);
            bf.WriteLong(location);
            if (location > -1)
            {
                bf.WriteLong(currentMap);
            }
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendItemEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.OpenItemEditor);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendItem(int itemNum, byte[] itemData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SaveItem);
            bf.WriteLong(itemNum);
            bf.WriteBytes(itemData);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendNpcEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.OpenNpcEditor);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendNpc(int npcNum, byte[] npcData)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SaveNpc);
            bf.WriteInteger(npcNum);
            bf.WriteBytes(npcData);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendSpellEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.OpenSpellEditor);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendSpell(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SaveSpell);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendAnimationEditor()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.OpenAnimationEditor);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }

        public static void SendAnimation(int index, byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ClientPackets.SaveAnimation);
            bf.WriteInteger(index);
            bf.WriteBytes(data);
            Globals.GameSocket.SendPacket(bf.ToArray());
        }
    }
}
