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
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Microsoft.Xna.Framework.Graphics;


namespace Intersect_Editor.Classes
{
    public static class PacketHandler
    {
        public delegate void GameObjectUpdated(GameObject type);
        public static  GameObjectUpdated GameObjectUpdatedDelegate;

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
                case ServerPackets.EnterMap:
                    HandleEnterMap(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.MapList:
                    HandleMapList(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.LoginError:
                    HandleLoginError(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.MapGrid:
                    HandleMapGrid(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.SendAlert:
                    HandleAlert(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.GameObject:
                    HandleGameObject(bf.ReadBytes(bf.Length()));
                    break;
                case ServerPackets.GameObjectEditor:
                    HandleOpenEditor(bf.ReadBytes(bf.Length()));
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
            Options.LoadFromServer(bf);
        }

        private static void HandleJoinGame(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MyIndex = (int)bf.ReadLong();
            Globals.LoginForm.TryRemembering();
            Globals.LoginForm.Hide();
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            var mapLength = bf.ReadLong();
            var mapData = bf.ReadBytes((int)mapLength);
            var map = new MapInstance((int)mapNum);
            if (MapInstance.GetMap(mapNum) != null)
            {
                if (Globals.CurrentMap == MapInstance.GetMap(mapNum))
                    Globals.CurrentMap = map;
                MapInstance.GetMap(mapNum).Delete();
            }
            MapInstance.AddObject(mapNum, map);
            map.Load(mapData);
            if (!Globals.InEditor && Globals.HasGameData)
            {
                Globals.CurrentMap = map;
                Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
            }
            else if (Globals.InEditor)
            {
                if (Globals.FetchingMapPreviews || Globals.CurrentMap == map)
                {
                    int currentmap = Globals.CurrentMap.GetId();
                    if (!Directory.Exists("resources/mapcache")) Directory.CreateDirectory("resources/mapcache");
                    if (!File.Exists("resources/mapcache/" + mapNum + "_" + MapInstance.GetMap(mapNum).Revision + ".png"))
                    {
                        Globals.CurrentMap = MapInstance.GetMap(mapNum);
                        using (var fs = new FileStream("resources/mapcache/" + mapNum + "_" + MapInstance.GetMap(mapNum).Revision + ".png", FileMode.OpenOrCreate))
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
                    Globals.CurrentMap = MapInstance.GetMap(currentmap);
                }
                if (mapNum != Globals.LoadingMap) return;
                Globals.CurrentMap = MapInstance.GetMap(Globals.LoadingMap);
                //TODO HANDLE DELETED MAP BEING SENT
                //if (Globals.GameMaps[mapNum].Deleted == 1)
                //{
                //    Globals.CurrentMap = null;
                    //Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
                //}
                //else
                //{
                    Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
                    if (Globals.MapEditorWindow.picMap.Visible) return;
                    Globals.MapEditorWindow.picMap.Visible = true;
                    if (map.Up > -1) { PacketSender.SendNeedMap(map.Up); }
                    if (map.Down > -1) { PacketSender.SendNeedMap(map.Down); }
                    if (map.Left > -1) { PacketSender.SendNeedMap(map.Left); }
                    if (map.Right > -1) { PacketSender.SendNeedMap(map.Right); }
                //}
            }
        }

        private static void HandleGameData(byte[] packet)
        {
            Globals.HasGameData = true;
            if (!Globals.InEditor && Globals.HasGameData && Globals.CurrentMap != null)
            {
                Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
            }
            GameContentManager.LoadTilesets();
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
            MapList.GetList().Load(bf, MapBase.GetObjects(), false, true);
            if (Globals.CurrentMap == null)
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
            MessageBox.Show(error, "Login Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            bf.Dispose();
        }

        private static void HandleMapGrid(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MapGridWindow.InitGrid(bf);
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

        private static void HandleGameObject(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject) bf.ReadInteger();
            var id = bf.ReadInteger();
            var deleted = Convert.ToBoolean(bf.ReadInteger());
            var data = bf.ReadBytes(bf.Length());
            switch (type)
            {
                case GameObject.Animation:
                    if (deleted)
                    {
                        var anim = AnimationBase.GetAnim(id);
                        anim.Delete();
                    }
                    else
                    {
                        var anim = new AnimationBase(id);
                        anim.Load(data);
                        AnimationBase.AddObject(id, anim);
                    }
                    break;
                case GameObject.Class:
                    if (deleted)
                    {
                        var cls = ClassBase.GetClass(id);
                        cls.Delete();
                    }
                    else
                    {
                        var cls = new ClassBase(id);
                        cls.Load(data);
                        ClassBase.AddObject(id, cls);
                    }
                    break;
                case GameObject.Item:
                    if (deleted)
                    {
                        var itm = ItemBase.GetItem(id);
                        itm.Delete();
                    }
                    else
                    {
                        var itm = new ItemBase(id);
                        itm.Load(data);
                        ItemBase.AddObject(id, itm);
                    }
                    break;
                case GameObject.Npc:
                    if (deleted)
                    {
                        var npc = NpcBase.GetNpc(id);
                        npc.Delete();
                    }
                    else
                    {
                        var npc = new NpcBase(id);
                        npc.Load(data);
                        NpcBase.AddObject(id, npc);
                    }
                    break;
                case GameObject.Projectile:
                    if (deleted)
                    {
                        var proj = ProjectileBase.GetProjectile(id);
                        proj.Delete();
                    }
                    else
                    {
                        var proj = new ProjectileBase(id);
                        proj.Load(data);
                        ProjectileBase.AddObject(id, proj);
                    }
                    break;
                case GameObject.Quest:
                    if (deleted)
                    {
                        var qst = QuestBase.GetQuest(id);
                        qst.Delete();
                    }
                    else
                    {
                        var qst = new QuestBase(id);
                        qst.Load(data);
                        QuestBase.AddObject(id, qst);
                    }
                    break;
                case GameObject.Resource:
                    if (deleted)
                    {
                        var res = ResourceBase.GetResource(id);
                        res.Delete();
                    }
                    else
                    {
                        var res = new ResourceBase(id);
                        res.Load(data);
                        ResourceBase.AddObject(id, res);
                    }
                    break;
                case GameObject.Shop:
                    if (deleted)
                    {
                        var shp = ShopBase.GetShop(id);
                        shp.Delete();
                    }
                    else
                    {
                        var shp = new ShopBase(id);
                        shp.Load(data);
                        ShopBase.AddObject(id, shp);
                    }
                    break;
                case GameObject.Spell:
                    if (deleted)
                    {
                        var spl = SpellBase.GetSpell(id);
                        spl.Delete();
                    }
                    else
                    {
                        var spl = new SpellBase(id);
                        spl.Load(data);
                        SpellBase.AddObject(id, spl);
                    }
                    break;
                case GameObject.Map:
                    //Handled in a different packet
                    break;
                case GameObject.CommonEvent:
                    if (deleted)
                    {
                        var evt = EventBase.GetEvent(id);
                        evt.Delete();
                    }
                    else
                    {
                        var evt = new EventBase(id, -1, -1, true);
                        evt.Load(data);
                        EventBase.AddObject(id, evt);
                    }
                    break;
                case GameObject.PlayerSwitch:
                    if (deleted)
                    {
                        var pswtch = PlayerSwitchBase.GetSwitch(id);
                        pswtch.Delete();
                    }
                    else
                    {
                        var pswtch = new PlayerSwitchBase(id);
                        pswtch.Load(data);
                        PlayerSwitchBase.AddObject(id, pswtch);
                    }
                    break;
                case GameObject.PlayerVariable:
                    if (deleted)
                    {
                        var pvar = PlayerVariableBase.GetVariable(id);
                        pvar.Delete();
                    }
                    else
                    {
                        var pvar = new PlayerVariableBase(id);
                        pvar.Load(data);
                        PlayerVariableBase.AddObject(id, pvar);
                    }
                    break;
                case GameObject.ServerSwitch:
                    if (deleted)
                    {
                        var sswtch = ServerSwitchBase.GetSwitch(id);
                        sswtch.Delete();
                    }
                    else
                    {
                        var sswtch = new ServerSwitchBase(id);
                        sswtch.Load(data);
                        ServerSwitchBase.AddObject(id, sswtch);
                    }
                    break;
                case GameObject.ServerVariable:
                    if (deleted)
                    {
                        var svar = ServerVariableBase.GetVariable(id);
                        svar.Delete();
                    }
                    else
                    {
                        var svar = new ServerVariableBase(id);
                        svar.Load(data);
                        ServerVariableBase.AddObject(id, svar);
                    }
                    break;
                case GameObject.Tileset:
                    var obj = new TilesetBase(id);
                    obj.Load(data);
                    TilesetBase.AddObject(id, obj);
                    if (Globals.HasGameData) GameContentManager.LoadTilesets();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (GameObjectUpdatedDelegate != null) GameObjectUpdatedDelegate(type);
            bf.Dispose();
        }

        private static void HandleOpenEditor(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, type);
            bf.Dispose();
        }
    }
}
