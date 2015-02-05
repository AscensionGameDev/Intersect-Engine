using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor
{
    public class PacketHandler
    {
        public PacketHandler()
        {

        }

        public void HandlePacket(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long packetHeader = bf.ReadLong();
            switch (packetHeader)
            {
                case 0:
                    PacketSender.SendPing();
                    break;
                case 1:
                    HandleJoinGame(bf.ReadBytes(bf.Length()));
                    break;

                case 2:
                    HandleMapData(bf.ReadBytes(bf.Length()));
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    HandleGameData(bf.ReadBytes(bf.Length()));
                    break;
                case 8:
                    HandleTilesets(bf.ReadBytes(bf.Length()));
                    break;
                case 9:
                    HandleEnterMap(bf.ReadBytes(bf.Length()));
                    break;
                case 15:
                    HandleMapList(bf.ReadBytes(bf.Length()));
                    break;
                case 18:
                    HandleItemEditor();
                    break;
                case 19:
                    HandleItemData(bf.ReadBytes(bf.Length()));
                    break;
            }
        }

        private void HandleJoinGame(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.myIndex = (int)bf.ReadLong();
            Globals.loginForm.Hide();
        }

        private void HandleMapData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long mapNum = bf.ReadLong();
            long mapLength = bf.ReadLong();
            byte[] mapData = bf.ReadBytes((int)mapLength);
            if (mapNum > Globals.mapCount-1)
            {
                Globals.mapCount = mapNum + 1;
                Map[] tmpMap = (Map[])Globals.GameMaps.Clone();
                Globals.GameMaps = new Map[Globals.mapCount];
                tmpMap.CopyTo(Globals.GameMaps, 0);
            }
            Globals.GameMaps[mapNum] = new Map((int)mapNum, mapData);
            Globals.receivedGameData++;
            if (Globals.receivedGameData == 3 && !Globals.inEditor)
            {
                Globals.mainForm = new frmMain();
                Globals.mainForm.Show();
            }
            else if (Globals.inEditor)
            {
                if (mapNum == Globals.currentMap)
                {
                    if (Globals.mainForm.picMap.Visible == false)
                    {
                        Globals.mainForm.picMap.Visible = true;
                        if (Globals.GameMaps[mapNum].up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].up); }
                        if (Globals.GameMaps[mapNum].down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].down); }
                        if (Globals.GameMaps[mapNum].left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].left); }
                        if (Globals.GameMaps[mapNum].right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].right); }
                    }
                }
            }
        }

        private void HandleGameData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.mapCount = bf.ReadLong();
            Globals.GameMaps = new Map[Globals.mapCount];
            PacketSender.SendNeedMap(0);
            Globals.receivedGameData++;
            if (Globals.receivedGameData == 3 && !Globals.inEditor)
            {
                Globals.mainForm = new frmMain();
                Globals.mainForm.Show();
            }
        }

        private void HandleTilesets(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long tilesetCount = bf.ReadLong();
            if (tilesetCount > 0)
            {
                Globals.tilesets = new string[tilesetCount];
                for (int i = 0; i < tilesetCount; i++)
                {
                    Globals.tilesets[i] = bf.ReadString();
                }
            }
            Globals.receivedGameData++;
            if (Globals.receivedGameData == 3 && !Globals.inEditor)
            {
                Globals.mainForm = new frmMain();
                Globals.mainForm.Show();
            }
        }

        private void HandleEnterMap(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.mainForm.EnterMap((int)bf.ReadLong());
        }

        private void HandleMapList(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapCount = bf.ReadInteger();
            Globals.MapRefs = new MapRef[mapCount];
            for (int i = 0; i < mapCount; i++)
            {
                Globals.MapRefs[i] = new MapRef();
                Globals.MapRefs[i].MapName = bf.ReadString();
                Globals.MapRefs[i].deleted = bf.ReadInteger();
            }
        }

        private void HandleItemEditor()
        {
            frmItem tmpItemEditor;
            tmpItemEditor = new frmItem();
            tmpItemEditor.initEditor();
            tmpItemEditor.Show();
        }

        private void HandleItemData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long itemnum = bf.ReadLong();
            Globals.Items[itemnum] = new Item();
            Globals.Items[itemnum].LoadByte(bf.ReadBytes(bf.Length()));
        }
    }
}
