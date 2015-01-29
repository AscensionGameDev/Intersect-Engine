using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectEditor
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
            }
        }

        private void HandleJoinGame(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            GlobalVariables.myIndex = (int)bf.ReadLong();
            GlobalVariables.loginForm.Hide();
        }

        private void HandleMapData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long mapNum = bf.ReadLong();
            long mapLength = bf.ReadLong();
            byte[] mapData = bf.ReadBytes((int)mapLength);
            if (mapNum > GlobalVariables.mapCount-1)
            {
                GlobalVariables.mapCount = mapNum + 1;
                Map[] tmpMap = (Map[])GlobalVariables.GameMaps.Clone();
                GlobalVariables.GameMaps = new Map[GlobalVariables.mapCount];
                tmpMap.CopyTo(GlobalVariables.GameMaps, 0);
            }
            GlobalVariables.GameMaps[mapNum] = new Map((int)mapNum, mapData);
            GlobalVariables.receivedGameData++;
            if (GlobalVariables.receivedGameData == 3 && !GlobalVariables.inEditor)
            {
                GlobalVariables.mainForm = new frmMain();
                GlobalVariables.mainForm.Show();
            }
            else if (GlobalVariables.inEditor)
            {
                if (mapNum == GlobalVariables.mainForm.currentMap)
                {
                    if (GlobalVariables.mainForm.picMap.Visible == false)
                    {
                        GlobalVariables.mainForm.picMap.Visible = true;
                        if (GlobalVariables.GameMaps[mapNum].up > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[mapNum].up); }
                        if (GlobalVariables.GameMaps[mapNum].down > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[mapNum].down); }
                        if (GlobalVariables.GameMaps[mapNum].left > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[mapNum].left); }
                        if (GlobalVariables.GameMaps[mapNum].right > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[mapNum].right); }
                    }
                }
            }
        }

        private void HandleGameData(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            GlobalVariables.mapCount = bf.ReadLong();
            GlobalVariables.GameMaps = new Map[GlobalVariables.mapCount];
            PacketSender.SendNeedMap(0);
            GlobalVariables.receivedGameData++;
            if (GlobalVariables.receivedGameData == 3 && !GlobalVariables.inEditor)
            {
                GlobalVariables.mainForm = new frmMain();
                GlobalVariables.mainForm.Show();
            }
        }

        private void HandleTilesets(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long tilesetCount = bf.ReadLong();
            if (tilesetCount > 0)
            {
                GlobalVariables.tilesets = new string[tilesetCount];
                for (int i = 0; i < tilesetCount; i++)
                {
                    GlobalVariables.tilesets[i] = bf.ReadString();
                }
            }
            GlobalVariables.receivedGameData++;
            if (GlobalVariables.receivedGameData == 3 && !GlobalVariables.inEditor)
            {
                GlobalVariables.mainForm = new frmMain();
                GlobalVariables.mainForm.Show();
            }
        }

        private void HandleEnterMap(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            GlobalVariables.mainForm.EnterMap((int)bf.ReadLong());
        }

        private void HandleMapList(byte[] packet)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapCount = bf.ReadInteger();
            GlobalVariables.MapRefs = new MapRef[mapCount];
            for (int i = 0; i < mapCount; i++)
            {
                GlobalVariables.MapRefs[i] = new MapRef();
                GlobalVariables.MapRefs[i].MapName = bf.ReadString();
                GlobalVariables.MapRefs[i].deleted = bf.ReadInteger();
            }
        }
    }
}
