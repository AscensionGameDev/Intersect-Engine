
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.Maps;
using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;


namespace Intersect_Editor.Classes
{
    public static class PacketHandler
    {
        public delegate void GameObjectUpdated(GameObject type);
        public static  GameObjectUpdated GameObjectUpdatedDelegate;

        public delegate void MapUpdated();
        public static MapUpdated MapUpdatedDelegate;

        public static void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);

            //Compressed?
            if (bf.ReadByte() == 1)
            {
                packet = bf.ReadBytes(bf.Length());
                var data = Compression.DecompressPacket(packet);
                bf = new ByteBuffer();
                bf.WriteBytes(data);
            }

            var packetHeader = (ServerPackets)bf.ReadLong();
            switch (packetHeader)
            {
                case ServerPackets.Ping:
                    HandlePing(bf.ReadBytes(bf.Length()));
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
                case ServerPackets.TimeBase:
                    HandleTimeBase(bf.ReadBytes(bf.Length()));
                    break;
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                    break;
            }
        }

        private static void HandlePing(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            if (Convert.ToBoolean(bf.ReadInteger()) == true) //request
            {
                PacketSender.SendPing();
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
            Globals.LoginForm.TryRemembering();
            Globals.LoginForm.Hide();
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            int deleted = bf.ReadInteger();
            if (deleted == 1)
            {
                if (MapInstance.GetMap(mapNum) != null)
                {
                    if (Globals.CurrentMap == MapInstance.GetMap(mapNum))
                    {
                        Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
                    }
                    MapInstance.GetMap(mapNum).Delete();
                }
            }
            else
            {
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
                map.MapGridX = bf.ReadInteger();
                map.MapGridY = bf.ReadInteger();
                map.InitAutotiles();
                map.UpdateAdjacentAutotiles();
                if (!Globals.InEditor && Globals.HasGameData)
                {
                    Globals.CurrentMap = map;
                    Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
                }
                else if (Globals.InEditor)
                {
                    if (Globals.FetchingMapPreviews || Globals.CurrentMap == map)
                    {
                        int currentmap = ((DatabaseObject) Globals.CurrentMap).Id;
                        if (Database.LoadMapCacheLegacy(mapNum, map.Revision) == null && !Globals.MapsToScreenshot.Contains(mapNum)) Globals.MapsToScreenshot.Add(mapNum);
                        if (Globals.FetchingMapPreviews)
                        {
                            if (Globals.MapsToFetch.Contains(mapNum))
                            {
                                Globals.MapsToFetch.Remove(mapNum);
                                if (Globals.MapsToFetch.Count == 0)
                                {
                                    Globals.FetchingMapPreviews = false;
                                    Globals.PreviewProgressForm.Dispose();
                                }
                                else
                                {
                                    Globals.PreviewProgressForm.SetProgress("Fetching Maps: " + (Globals.FetchCount - Globals.MapsToFetch.Count) + "/" + Globals.FetchCount, (int)(((float)(Globals.FetchCount - Globals.MapsToFetch.Count) / (float)Globals.FetchCount) * 100f), false);
                                }
                            }
                        }
                        Globals.CurrentMap = MapInstance.GetMap(currentmap);
                    }
                    if (mapNum != Globals.LoadingMap) return;
                    Globals.CurrentMap = MapInstance.GetMap(Globals.LoadingMap);
                    MapUpdatedDelegate();
                    if (map.Up > -1) { PacketSender.SendNeedMap(map.Up); }
                    if (map.Down > -1) { PacketSender.SendNeedMap(map.Down); }
                    if (map.Left > -1) { PacketSender.SendNeedMap(map.Left); }
                    if (map.Right > -1) { PacketSender.SendNeedMap(map.Right); }
                }
                if (Globals.CurrentMap.Id == mapNum && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].mapnum > -1) PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].mapnum);
                            }
                        }
                    }
                }
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
            Globals.MainForm.BeginInvoke((Action)(() => Globals.MainForm.EnterMap((int)bf.ReadLong())));
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
            Globals.MapListWindow.BeginInvoke(Globals.MapListWindow.mapTreeList.MapListDelegate, -1, null);
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
            if (Globals.MapGrid != null)
            {
                Globals.MapGrid.Load(bf);
                if (Globals.CurrentMap != null &&  Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].mapnum > -1) PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].mapnum);
                            }
                        }
                    }
                }
            }
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
            var another = Convert.ToBoolean(bf.ReadInteger());
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
                case GameObject.Bench:
                    if (deleted)
                    {
                        var cft = BenchBase.GetCraft(id);
                        cft.Delete();
                    }
                    else
                    {
                        var cft = new BenchBase(id);
                        cft.Load(data);
                        BenchBase.AddObject(id, cft);
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
                    if (Globals.HasGameData && !another) GameContentManager.LoadTilesets();
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

        private static void HandleTimeBase(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            TimeBase.GetTimeBase().LoadTimeBase(packet);
            Globals.MainForm.BeginInvoke(Globals.MainForm.TimeDelegate);
            bf.Dispose();
        }
    }
}
