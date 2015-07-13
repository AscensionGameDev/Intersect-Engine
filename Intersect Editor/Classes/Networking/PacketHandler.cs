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
using System;
using Intersect_Editor.Forms;
using System.Threading;

namespace Intersect_Editor.Classes
{
    public static class PacketHandler
    {
        public static void HandlePacket(byte[] packet)
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
                case Enums.ServerPackets.OpenNpcEditor:
                    HandleNpcEditor();
                    break;
                case Enums.ServerPackets.NpcData:
                    HandleNpcData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.NpcList:
                    HandleNpcList(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenSpellEditor:
                    HandleSpellEditor();
                    break;
                case Enums.ServerPackets.SpellData:
                    HandleSpellData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.SpellList:
                    HandleSpellList(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenAnimationEditor:
                    HandleAnimationEditor();
                    break;
                case Enums.ServerPackets.AnimationData:
                    HandleAnimationData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.AnimationList:
                    HandleAnimationList(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenResourceEditor:
                    HandleResourceEditor();
                    break;
                case Enums.ServerPackets.ResourceData:
                    HandleResourceData(bf.ReadBytes(bf.Length()));
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
            Database.InitDatabase();
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
                var tmpMap = (MapStruct[])Globals.GameMaps.Clone();
                Globals.GameMaps = new MapStruct[Globals.MapCount];
                tmpMap.CopyTo(Globals.GameMaps, 0);
            }
            Globals.GameMaps[mapNum] = new MapStruct((int)mapNum, mapData);
            Globals.ReceivedGameData++;
            if (Globals.ReceivedGameData == 3 && !Globals.InEditor)
            {
                Globals.EditorThread = new Thread(EditorLoop.StartLoop);
                Globals.EditorThread.Start();
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
            Globals.Ma
                = bf.ReadLong();
            Globals.GameMaps = new MapStruct[Globals.MapCount];
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
            Globals.GameItems[itemnum] = new ItemStruct();
            Globals.GameItems[itemnum].LoadByte(bf.ReadBytes(bf.Length()));
        }

        private static void HandleItemList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Constants.MaxItems; i++)
            {
                Globals.GameItems[i] = new ItemStruct();
                Globals.GameItems[i].Name = bf.ReadString();
            }
        }

        private static void HandleNpcEditor()
        {
            var tmpNpcEditor = new frmNpc();
            tmpNpcEditor.InitEditor();
            tmpNpcEditor.Show();
        }

        private static void HandleNpcData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var npcNum = bf.ReadInteger();
            Globals.GameNpcs[npcNum] = new NpcStruct();
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleNpcList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Constants.MaxNpcs; i++)
            {
                Globals.GameNpcs[i] = new NpcStruct();
                Globals.GameNpcs[i].Name = bf.ReadString();
            }
        }

        private static void HandleSpellEditor()
        {
            var tmpEditor = new frmSpell();
            tmpEditor.InitEditor();
            tmpEditor.Show();
        }

        private static void HandleSpellData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameSpells[index] = new SpellStruct();
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleSpellList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Constants.MaxSpells; i++)
            {
                Globals.GameSpells[i] = new SpellStruct();
                Globals.GameSpells[i].Name = bf.ReadString();
            }
        }

        private static void HandleAnimationEditor()
        {
            var tmpEditor = new frmAnimation();
            tmpEditor.InitEditor();
            tmpEditor.Show();
        }

        private static void HandleAnimationData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameAnimations[index] = new AnimationStruct();
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleAnimationList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Constants.MaxAnimations; i++)
            {
                Globals.GameAnimations[i] = new AnimationStruct();
                Globals.GameAnimations[i].Name = bf.ReadString();
            }
        }

        private static void HandleResourceEditor()
        {
            if (Globals.ResourceEditor == null)
            {
                Globals.ResourceEditor = new frmResource();
                Globals.ResourceEditor.InitEditor();
                Globals.ResourceEditor.Show();
            }
        }

        private static void HandleResourceData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var resourceNum = bf.ReadInteger();
            Globals.GameResources[resourceNum] = new ResourceStruct();
            Globals.GameResources[resourceNum].Load(bf.ReadBytes(bf.Length()));
        }
    }
}
