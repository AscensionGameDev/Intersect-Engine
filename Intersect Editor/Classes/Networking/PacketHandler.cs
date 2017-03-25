using System;
using System.Windows.Forms;
using Intersect;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.Maps;

namespace Intersect_Editor.Classes
{
    public static class PacketHandler
    {
        public delegate void GameObjectUpdated(GameObjectType type);

        public delegate void MapUpdated();

        public static GameObjectUpdated GameObjectUpdatedDelegate;
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

            var packetHeader = (ServerPackets) bf.ReadLong();
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
            int mapNum = (int) bf.ReadLong();
            int deleted = bf.ReadInteger();
            if (deleted == 1)
            {
                if (MapInstance.Lookup.Get(mapNum) != null)
                {
                    if (Globals.CurrentMap == MapInstance.Lookup.Get(mapNum))
                    {
                        Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
                    }
                    MapInstance.Lookup.Get(mapNum).Delete();
                }
            }
            else
            {
                var mapLength = bf.ReadLong();
                var mapData = bf.ReadBytes((int) mapLength);
                var map = new MapInstance((int) mapNum);
                if (MapInstance.Lookup.Get(mapNum) != null)
                {
                    if (Globals.CurrentMap == MapInstance.Lookup.Get(mapNum))
                        Globals.CurrentMap = map;
                    MapInstance.Lookup.Get(mapNum).Delete();
                }
                MapInstance.Lookup.Set(mapNum, map);
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
                        int currentmap = ((IDatabaseObject) Globals.CurrentMap).Id;
                        if (Database.LoadMapCacheLegacy(mapNum, map.Revision) == null &&
                            !Globals.MapsToScreenshot.Contains(mapNum)) Globals.MapsToScreenshot.Add(mapNum);
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
                                    Globals.PreviewProgressForm.SetProgress(
                                        "Fetching Maps: " + (Globals.FetchCount - Globals.MapsToFetch.Count) + "/" +
                                        Globals.FetchCount,
                                        (int)
                                        (((float) (Globals.FetchCount - Globals.MapsToFetch.Count) /
                                          (float) Globals.FetchCount) * 100f), false);
                                }
                            }
                        }
                        Globals.CurrentMap = MapInstance.Lookup.Get(currentmap);
                    }
                    if (mapNum != Globals.LoadingMap) return;
                    Globals.CurrentMap = MapInstance.Lookup.Get(Globals.LoadingMap);
                    MapUpdatedDelegate();
                    if (map.Up > -1)
                    {
                        PacketSender.SendNeedMap(map.Up);
                    }
                    if (map.Down > -1)
                    {
                        PacketSender.SendNeedMap(map.Down);
                    }
                    if (map.Left > -1)
                    {
                        PacketSender.SendNeedMap(map.Left);
                    }
                    if (map.Right > -1)
                    {
                        PacketSender.SendNeedMap(map.Right);
                    }
                }
                if (Globals.CurrentMap.Id == mapNum && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.Lookup.Get(Globals.MapGrid.Grid[x, y].mapnum);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].mapnum > -1)
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].mapnum);
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
            Globals.MainForm.BeginInvoke((Action) (() => Globals.MainForm.EnterMap((int) bf.ReadLong())));
        }

        private static void HandleMapList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            MapList.GetList().Load(bf, MapBase.Lookup, false, true);
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
                if (Globals.CurrentMap != null && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.Lookup.Get(Globals.MapGrid.Grid[x, y].mapnum);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].mapnum > -1)
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].mapnum);
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
            var type = (GameObjectType) bf.ReadInteger();
            var id = bf.ReadInteger();
            var another = Convert.ToBoolean(bf.ReadInteger());
            var deleted = Convert.ToBoolean(bf.ReadInteger());
            var data = bf.ReadBytes(bf.Length());
            switch (type)
            {
                case GameObjectType.Animation:
                    if (deleted)
                    {
                        var anim = AnimationBase.Lookup.Get(id);
                        anim.Delete();
                    }
                    else
                    {
                        var anim = new AnimationBase(id);
                        anim.Load(data);
                        AnimationBase.Lookup.Set(id, anim);
                    }
                    break;
                case GameObjectType.Class:
                    if (deleted)
                    {
                        var cls = ClassBase.Lookup.Get(id);
                        cls.Delete();
                    }
                    else
                    {
                        var cls = new ClassBase(id);
                        cls.Load(data);
                        ClassBase.Lookup.Set(id, cls);
                    }
                    break;
                case GameObjectType.Item:
                    if (deleted)
                    {
                        var itm = ItemBase.Lookup.Get(id);
                        itm.Delete();
                    }
                    else
                    {
                        var itm = new ItemBase(id);
                        itm.Load(data);
                        ItemBase.Lookup.Set(id, itm);
                    }
                    break;
                case GameObjectType.Npc:
                    if (deleted)
                    {
                        var npc = NpcBase.Lookup.Get(id);
                        npc.Delete();
                    }
                    else
                    {
                        var npc = new NpcBase(id);
                        npc.Load(data);
                        NpcBase.Lookup.Set(id, npc);
                    }
                    break;
                case GameObjectType.Projectile:
                    if (deleted)
                    {
                        var proj = ProjectileBase.Lookup.Get(id);
                        proj.Delete();
                    }
                    else
                    {
                        var proj = new ProjectileBase(id);
                        proj.Load(data);
                        ProjectileBase.Lookup.Set(id, proj);
                    }
                    break;
                case GameObjectType.Quest:
                    if (deleted)
                    {
                        var qst = QuestBase.Lookup.Get(id);
                        qst.Delete();
                    }
                    else
                    {
                        var qst = new QuestBase(id);
                        qst.Load(data);
                        QuestBase.Lookup.Set(id, qst);
                    }
                    break;
                case GameObjectType.Resource:
                    if (deleted)
                    {
                        var res = ResourceBase.Lookup.Get(id);
                        res.Delete();
                    }
                    else
                    {
                        var res = new ResourceBase(id);
                        res.Load(data);
                        ResourceBase.Lookup.Set(id, res);
                    }
                    break;
                case GameObjectType.Shop:
                    if (deleted)
                    {
                        var shp = ShopBase.Lookup.Get(id);
                        shp.Delete();
                    }
                    else
                    {
                        var shp = new ShopBase(id);
                        shp.Load(data);
                        ShopBase.Lookup.Set(id, shp);
                    }
                    break;
                case GameObjectType.Spell:
                    if (deleted)
                    {
                        var spl = SpellBase.Lookup.Get(id);
                        spl.Delete();
                    }
                    else
                    {
                        var spl = new SpellBase(id);
                        spl.Load(data);
                        SpellBase.Lookup.Set(id, spl);
                    }
                    break;
                case GameObjectType.Bench:
                    if (deleted)
                    {
                        var cft = BenchBase.Lookup.Get(id);
                        cft.Delete();
                    }
                    else
                    {
                        var cft = new BenchBase(id);
                        cft.Load(data);
                        BenchBase.Lookup.Set(id, cft);
                    }
                    break;
                case GameObjectType.Map:
                    //Handled in a different packet
                    break;
                case GameObjectType.CommonEvent:
                    if (deleted)
                    {
                        var evt = EventBase.Lookup.Get(id);
                        evt.Delete();
                    }
                    else
                    {
                        var evt = new EventBase(id, -1, -1, true);
                        evt.Load(data);
                        EventBase.Lookup.Set(id, evt);
                    }
                    break;
                case GameObjectType.PlayerSwitch:
                    if (deleted)
                    {
                        var pswtch = PlayerSwitchBase.Lookup.Get(id);
                        pswtch.Delete();
                    }
                    else
                    {
                        var pswtch = new PlayerSwitchBase(id);
                        pswtch.Load(data);
                        PlayerSwitchBase.Lookup.Set(id, pswtch);
                    }
                    break;
                case GameObjectType.PlayerVariable:
                    if (deleted)
                    {
                        var pvar = PlayerVariableBase.Lookup.Get(id);
                        pvar.Delete();
                    }
                    else
                    {
                        var pvar = new PlayerVariableBase(id);
                        pvar.Load(data);
                        PlayerVariableBase.Lookup.Set(id, pvar);
                    }
                    break;
                case GameObjectType.ServerSwitch:
                    if (deleted)
                    {
                        var sswtch = ServerSwitchBase.Lookup.Get(id);
                        sswtch.Delete();
                    }
                    else
                    {
                        var sswtch = new ServerSwitchBase(id);
                        sswtch.Load(data);
                        ServerSwitchBase.Lookup.Set(id, sswtch);
                    }
                    break;
                case GameObjectType.ServerVariable:
                    if (deleted)
                    {
                        var svar = ServerVariableBase.Lookup.Get(id);
                        svar.Delete();
                    }
                    else
                    {
                        var svar = new ServerVariableBase(id);
                        svar.Load(data);
                        ServerVariableBase.Lookup.Set(id, svar);
                    }
                    break;
                case GameObjectType.Tileset:
                    var obj = new TilesetBase(id);
                    obj.Load(data);
                    TilesetBase.Lookup.Set(id, obj);
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
            var type = (GameObjectType) bf.ReadInteger();
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