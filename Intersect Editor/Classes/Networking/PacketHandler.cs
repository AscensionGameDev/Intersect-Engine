/*
    Intersect Game Engine (Editor)
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
using System.IO;
using System.Windows.Forms;

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
                case Enums.ServerPackets.LoginError:
                    HandleLoginError(bf.ReadBytes(bf.Length()));
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
                case Enums.ServerPackets.OpenClassEditor:
                    HandleClassEditor();
                    break;
                case Enums.ServerPackets.ClassData:
                    HandleClassData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenQuestEditor:
                    HandleQuestEditor();
                    break;
                case Enums.ServerPackets.QuestData:
                    HandleQuestData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.MapGrid:
                    HandleMapGrid(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.OpenProjectileEditor:
                    HandleProjectileEditor();
                    break;
                case Enums.ServerPackets.ProjectileData:
                    HandleProjectileData(bf.ReadBytes(bf.Length()));
                    break;
                case Enums.ServerPackets.SendAlert:
                    HandleAlert(bf.ReadBytes(bf.Length()));
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
            int mapNum = (int)bf.ReadLong();
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
                Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
            }
            else if (Globals.InEditor)
            {
                if (Globals.FetchingMapPreviews || Globals.CurrentMap == mapNum)
                {
                    int currentmap = Globals.CurrentMap;
                    if (!Directory.Exists("resources/mapcache")) Directory.CreateDirectory("resources/mapcache");
                    if (!File.Exists("resources/mapcache/" + mapNum + "_" + Globals.GameMaps[mapNum].Revision + ".png"))
                    {
                        Globals.CurrentMap = (int)mapNum;
                        EditorGraphics.ScreenShotMap(true).SaveToFile("resources/mapcache/" + mapNum + "_" + Globals.GameMaps[mapNum].Revision + ".png");
                    }
                    if (Globals.FetchingMapPreviews)
                    {
                        if (Globals.MapsToFetch.Contains(mapNum))
                        {
                            Globals.MapsToFetch.Remove(mapNum);
                            if (Globals.MapsToFetch.Count == 0) {
                                Globals.FetchingMapPreviews = false;
                                Globals.PreviewProgressForm.Dispose();
                            }
                            else {
                                Globals.PreviewProgressForm.SetProgress("Fetching Maps: " + (Globals.FetchCount - Globals.MapsToFetch.Count) + "/" + Globals.FetchCount, (int)(((float)(Globals.FetchCount - Globals.MapsToFetch.Count)/(float)Globals.FetchCount) * 100f),false);
                            }
                        }
                    }
                    Globals.CurrentMap = currentmap;
                }
                if (mapNum != Globals.CurrentMap) return;
                if (Globals.GameMaps[mapNum].Deleted == 1)
                {
                    Globals.CurrentMap = -1;
                    Globals.MainForm.EnterMap(Database.MapStructure.FindFirstMap());
                }
                else
                {
                    Globals.MapPropertiesWindow.Init((int)mapNum);
                    if (Globals.MapEditorWindow.picMap.Visible) return;
                    Globals.MapEditorWindow.picMap.Visible = true;
                    if (Globals.GameMaps[mapNum].Up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Up); }
                    if (Globals.GameMaps[mapNum].Down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Down); }
                    if (Globals.GameMaps[mapNum].Left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Left); }
                    if (Globals.GameMaps[mapNum].Right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[mapNum].Right); }
                }
            }
        }

        private static void HandleGameData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MapCount = bf.ReadLong();
            Globals.GameMaps = new MapStruct[Globals.MapCount];
            PacketSender.SendNeedMap(0);
            Globals.ReceivedGameData++;
            if (Globals.ReceivedGameData != 3 || Globals.InEditor) return;
            Globals.MainForm = new frmMain();
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
            Globals.MainForm = new frmMain();
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
            Database.OrderedMaps.Clear();
            Database.MapStructure.Load(bf);
            Database.OrderedMaps.Sort();
            if (Globals.CurrentMap == -1)
            {
                Globals.MainForm.EnterMap(Database.MapStructure.FindFirstMap());
            }
            Globals.MapListWindow.BeginInvoke(Globals.MapListWindow.mapTreeList.MapListDelegate,-1);
            bf.Dispose();
        }

        private static void HandleLoginError(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string error = bf.ReadString();
            System.Windows.Forms.MessageBox.Show(error, "Login Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            bf.Dispose();
        }

        private static void HandleItemEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Item);
        }

        private static void HandleItemData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadInteger();
            Globals.GameItems[itemNum] = new ItemStruct();
            Globals.GameItems[itemNum].LoadItem(bf.ReadBytes(bf.Length()),itemNum);
            bf.Dispose();
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
            bf.Dispose();
        }

        private static void HandleNpcEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Npc);
        }

        private static void HandleNpcData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var npcNum = bf.ReadInteger();
            Globals.GameNpcs[npcNum] = new NpcStruct();
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()), npcNum);
            bf.Dispose();
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
            bf.Dispose();
        }

        private static void HandleSpellEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Spell);
        }

        private static void HandleSpellData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameSpells[index] = new SpellStruct();
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()),index);
            bf.Dispose();
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
            bf.Dispose();
        }

        private static void HandleAnimationEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Animation);
        }

        private static void HandleAnimationData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameAnimations[index] = new AnimationStruct();
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()),index);
            bf.Dispose();
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
            bf.Dispose();
        }

        private static void HandleResourceEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Resource);
        }

        private static void HandleResourceData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var resourceNum = bf.ReadInteger();
            Globals.GameResources[resourceNum] = new ResourceStruct();
            Globals.GameResources[resourceNum].Load(bf.ReadBytes(bf.Length()),resourceNum);
            bf.Dispose();
        }

        private static void HandleClassEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Class);
        }

        private static void HandleClassData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var classNum = bf.ReadInteger();
            Globals.GameClasses[classNum] = new ClassStruct();
            Globals.GameClasses[classNum].Load(bf.ReadBytes(bf.Length()),classNum);
            bf.Dispose();
        }

        private static void HandleQuestEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Quest);
        }

        private static void HandleQuestData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var questNum = bf.ReadInteger();
            Globals.GameQuests[questNum] = new QuestStruct();
            Globals.GameQuests[questNum].Load(bf.ReadBytes(bf.Length()),questNum);
            bf.Dispose();
        }

        private static void HandleMapGrid(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MapGridWindow.InitGrid(bf);
            bf.Dispose();
        }

        private static void HandleProjectileEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)Enums.EditorTypes.Projectile);
        }

        private static void HandleProjectileData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var projectileNum = bf.ReadInteger();
            Globals.GameProjectiles[projectileNum] = new ProjectileStruct();
            Globals.GameProjectiles[projectileNum].Load(bf.ReadBytes(bf.Length()),projectileNum);
            bf.Dispose();
        }

        private static void HandleAlert(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var title = bf.ReadString();
            var text = bf.ReadString();
            MessageBox.Show(text, title);
            bf.Dispose();
        }
    }
}
