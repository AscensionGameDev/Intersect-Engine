using System;
using Intersect_Editor.Forms;

namespace Intersect_Editor.Classes
{
    public class PacketHandler
    {
        public void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (Enums.ServerPackets)bf.ReadLong();
            switch (packetHeader)
            {
                case Enums.ServerPackets.RequestPing:
                    PacketSender.SendPing();
                    break;
                case Enums.ServerPackets.JoinGame:
                    HandleJoinGame(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.MapData:
                    HandleMapData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.GameData:
                    HandleGameData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.TilesetArray:
                    HandleTilesets(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.EnterMap:
                    HandleEnterMap(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.MapList:
                    HandleMapList(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenItemEditor:
                    HandleItemEditor();
                    break;
                case Enums.ServerPackets.ItemData:
                    HandleItemData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.ItemList:
                    HandleItemList(bf.ReadBytes(bf.Length()));
                    break;
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                    break;
            }
        }

        private static void HandleJoinGame(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MyIndex = (int)bf.ReadLong();
            Globals.LoginForm.Hide();
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = bf.ReadLong();
            var mapLength = bf.ReadLong();
            var mapData = bf.ReadBytes((int)mapLength);
            if (mapNum > Globals.MapCount-1)
            {
                Globals.MapCount = mapNum + 1;
                var tmpMap = (Map[])Globals.GameMaps.Clone();
                Globals.GameMaps = new Map[Globals.MapCount];
                tmpMap.CopyTo(Globals.GameMaps, 0);
            }
            Globals.GameMaps[mapNum] = new Map((int)mapNum, mapData);
            Globals.ReceivedGameData++;
            if (Globals.ReceivedGameData == 3 && !Globals.InEditor)
            {
                Globals.MainForm = new FrmMain();
                Globals.MainForm.Show();
            }
            else if (Globals.InEditor)
            {
                if (mapNum != Globals.CurrentMap) return;
                if (Globals.MainForm.picMap.Visible) return;
                Globals.MainForm.picMap.Visible = true;
                if (Globals.GameMaps[mapNum].Up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Up); }
                if (Globals.GameMaps[mapNum].Down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Down); }
                if (Globals.GameMaps[mapNum].Left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Left); }
                if (Globals.GameMaps[mapNum].Right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Right); }
            }
        }

        private static void HandleGameData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MapCount = bf.ReadLong();
            Globals.GameMaps = new Map[Globals.MapCount];
            PacketSender.SendNeedMap(0);
            Globals.ReceivedGameData++;
            if (Globals.ReceivedGameData != 3 || Globals.InEditor) return;
            Globals.MainForm = new FrmMain();
            Globals.MainForm.Show();
        }

        private static void HandleTilesets(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var tilesetCount = bf.ReadLong();
            if (tilesetCount > 0)
            {
                Globals.Tilesets = new string[tilesetCount];
                for (var i = 0; i < tilesetCount; i++)
                {
                    Globals.Tilesets[i] = bf.ReadString();
                }
            }
            Globals.ReceivedGameData++;
            if (Globals.ReceivedGameData != 3 || Globals.InEditor) return;
            Globals.MainForm = new FrmMain();
            Globals.MainForm.Show();
        }

        private static void HandleEnterMap(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MainForm.EnterMap((int)bf.ReadLong());
        }

        private static void HandleMapList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapCount = bf.ReadInteger();
            Globals.MapRefs = new MapRef[mapCount];
            for (var i = 0; i < mapCount; i++)
            {
                Globals.MapRefs[i] = new MapRef {MapName = bf.ReadString(), Deleted = bf.ReadInteger()};
            }
        }

        private static void HandleItemEditor()
        {
            var tmpItemEditor = new FrmItem();
            tmpItemEditor.InitEditor();
            tmpItemEditor.Show();
        }

        private static void HandleItemData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemnum = bf.ReadLong();
            Globals.Items[itemnum] = new Item();
            Globals.Items[itemnum].LoadByte(bf.ReadBytes(bf.Length()));
        }

        private void HandleItemList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Constants.MaxItems; i++)
            {
                Globals.Items[i] = new Item();
                Globals.Items[i].Name = bf.ReadString();
            }
        }
    }
}
