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
using System.Collections.Generic;
using Intersect_Editor.Forms;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Microsoft.Xna.Framework.Graphics;
using Options = Intersect_Editor.Classes.General.Options;

namespace Intersect_Editor.Classes
{
    public static class PacketHandler
    {
        public static void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (ServerPackets)bf.ReadLong();
            switch (packetHeader)
            {
                case ServerPackets.RequestPing:
                    PacketSender.SendPing();
                    break;
                case ServerPackets.ServerConfig:
                    HandleServerConfig(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.JoinGame:
                    HandleJoinGame(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.MapData:
                    HandleMapData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.GameData:
                    HandleGameData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.TilesetArray:
                    HandleTilesets(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.EnterMap:
                    HandleEnterMap(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.MapList:
                    HandleMapList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.LoginError:
                    HandleLoginError(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenItemEditor:
                    HandleItemEditor();
                    break;
                case ServerPackets.ItemData:
                    HandleItemData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.ItemList:
                    HandleItemList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenNpcEditor:
                    HandleNpcEditor();
                    break;
                case ServerPackets.NpcData:
                    HandleNpcData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.NpcList:
                    HandleNpcList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenSpellEditor:
                    HandleSpellEditor();
                    break;
                case ServerPackets.SpellData:
                    HandleSpellData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.SpellList:
                    HandleSpellList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenAnimationEditor:
                    HandleAnimationEditor();
                    break;
                case ServerPackets.AnimationData:
                    HandleAnimationData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.AnimationList:
                    HandleAnimationList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenResourceEditor:
                    HandleResourceEditor();
                    break;
                case ServerPackets.ResourceData:
                    HandleResourceData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenClassEditor:
                    HandleClassEditor();
                    break;
                case ServerPackets.ClassData:
                    HandleClassData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenQuestEditor:
                    HandleQuestEditor();
                    break;
                case ServerPackets.QuestData:
                    HandleQuestData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.MapGrid:
                    HandleMapGrid(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenProjectileEditor:
                    HandleProjectileEditor();
                    break;
                case ServerPackets.ProjectileData:
                    HandleProjectileData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.SendAlert:
                    HandleAlert(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenCommonEventEditor:
                    HandleOpenCommonEventEditor();
                    break;
                case ServerPackets.CommonEventData:
                    HandleCommonEventData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenSwitchVariableEditor:
                    HandleOpenSwitchVariableEditor();
                    break;
                case ServerPackets.SwitchVariableData:
                    HandleSwitchVariableData(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.OpenShopEditor:
                    HandleOpenShopEditor();
                    break;
                case ServerPackets.ShopData:
                    HandleShopData(bf.ReadBytes(bf.Length()));
                    break;
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                    break;
            }
        }

        public static void HandleServerConfig(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Options.LoadServerConfig(bf);
            Database.InitDatabase();
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
            if (Globals.GameMaps.ContainsKey(mapNum))
            {
                Globals.GameMaps[mapNum].Dispose();
                Globals.GameMaps.Remove(mapNum);
            }
            var map = new MapInstance((int) mapNum);
            Globals.GameMaps.Add(mapNum,map);
            map.Load(mapData);
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
                        using (var fs = new FileStream("resources/mapcache/" + mapNum + "_" + Globals.GameMaps[mapNum].Revision + ".png", FileMode.OpenOrCreate))
                        {
                            RenderTarget2D screenshotTexture = EditorGraphics.ScreenShotMap(true);
                            screenshotTexture.SaveAsPng(fs, screenshotTexture.Width, screenshotTexture.Height);
                        }
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
                    Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
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
            MapList.GetList().Load(bf, new Dictionary<int,Intersect_Library.GameObjects.Maps.MapStruct>(), false);
            if (Globals.CurrentMap == -1)
            {
                Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
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
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Item);
        }

        private static void HandleItemData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadInteger();
            Globals.GameItems[itemNum] = new ItemStruct();
            Globals.GameItems[itemNum].Load(bf.ReadBytes(bf.Length()),itemNum);
            bf.Dispose();
        }

        private static void HandleItemList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Options.MaxItems; i++)
            {
                Globals.GameItems[i] = new ItemStruct();
                Globals.GameItems[i].Name = bf.ReadString();
            }
            bf.Dispose();
        }

        private static void HandleNpcEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Npc);
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
            for (int i = 0; i < Options.MaxNpcs; i++)
            {
                Globals.GameNpcs[i] = new NpcStruct();
                Globals.GameNpcs[i].Name = bf.ReadString();
            }
            bf.Dispose();
        }

        private static void HandleSpellEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Spell);
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
            for (int i = 0; i < Options.MaxSpells; i++)
            {
                Globals.GameSpells[i] = new SpellStruct();
                Globals.GameSpells[i].Name = bf.ReadString();
            }
            bf.Dispose();
        }

        private static void HandleAnimationEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Animation);
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
            for (int i = 0; i < Options.MaxAnimations; i++)
            {
                Globals.GameAnimations[i] = new AnimationStruct();
                Globals.GameAnimations[i].Name = bf.ReadString();
            }
            bf.Dispose();
        }

        private static void HandleResourceEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Resource);
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
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Class);
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
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Quest);
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
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Projectile);
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

        private static void HandleOpenCommonEventEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.CommonEvent);
        }

        private static void HandleCommonEventData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.CommonEvents[index] = new EventStruct(index,bf,true);
            bf.Dispose();
        }

        private static void HandleOpenSwitchVariableEditor()
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.SwitchVariable);
        }

        private static void HandleSwitchVariableData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = bf.ReadInteger();
            var index = bf.ReadInteger();
            switch (type)
            {
                case (int)SwitchVariableTypes.PlayerSwitch:
                    Globals.PlayerSwitches[index] = bf.ReadString();
                    break;
                case (int)SwitchVariableTypes.PlayerVariable:
                    Globals.PlayerVariables[index] = bf.ReadString();
                    break;
                case (int)SwitchVariableTypes.ServerSwitch:
                    Globals.ServerSwitches[index] = bf.ReadString();
                    Globals.ServerSwitchValues[index] = Convert.ToBoolean(bf.ReadByte());
                    break;
                case (int)SwitchVariableTypes.ServerVariable:
                    Globals.ServerVariables[index] = bf.ReadString();
                    Globals.ServerVariableValues[index] = bf.ReadInteger();
                    break;
            }
            bf.Dispose();
        }

        private static void HandleOpenShopEditor() { 
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, (int)EditorTypes.Shop);
        }

        private static void HandleShopData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameShops[index] = new ShopStruct();
            Globals.GameShops[index].Load(bf.ReadBytes(bf.Length()), index);
            bf.Dispose();
        }
    }
}
