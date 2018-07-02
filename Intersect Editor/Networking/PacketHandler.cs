using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;

namespace Intersect.Editor.Networking
{
    public static class PacketHandler
    {
        public delegate void GameObjectUpdated(GameObjectType type);

        public delegate void MapUpdated();

        public static GameObjectUpdated GameObjectUpdatedDelegate;
        public static MapUpdated MapUpdatedDelegate;

        private static List<ShitMeasurement> sMeasurements = new List<ShitMeasurement>();

        private static int sHitstaken;
        private static long sTimespentshitting;
        private static long sTotalshitsize;
        private static Stopwatch sShitTimer = new Stopwatch();

        private static TextWriter sWriter;

        public static bool HandlePacket(IPacket packet)
        {
            var binaryPacket = packet as BinaryPacket;

            var bf = binaryPacket?.Buffer;

            if (packet == null || bf == null) return false;

            //Compressed?
            if (bf.ReadByte() == 1)
            {
                var data = Compression.DecompressPacket(bf.ReadBytes(bf.Length()));
                bf = new ByteBuffer();
                bf.WriteBytes(data);
            }

            HandlePacket(bf);
            return true;
        }

        private static int sPacketCount = 0;
        private static bool sDebugPackets = false;
        public static void HandlePacket(ByteBuffer bf)
        {
            if (bf == null || bf.Length() == 0) return;

            var packetHeader = (ServerPackets) bf.ReadLong();
            sPacketCount++;
            if (sDebugPackets)
            {
                Console.WriteLine("Handled packet " + packetHeader + " - " + sPacketCount);
            }
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
                case ServerPackets.Shit:
                    HandleShit(bf.ReadBytes(bf.Length()));
                    break;
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                    break;
            }
        }

        private static void HandleShit(byte[] packet)
        {
            if (sWriter == null)
            {
                sWriter = new StreamWriter(
                    new FileStream($"shits{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.csv", FileMode.Create,
                        FileAccess.Write), Encoding.UTF8);
            }

            using (var bf = new ByteBuffer())
            {
                bf.WriteBytes(packet);
                var shitting = bf.ReadBoolean();
                var packetNum = bf.ReadInteger();
                if (packetNum > -1)
                {
                    var isData = bf.ReadBoolean();
                    if (isData)
                    {
                        //Console.WriteLine($"START PACKET #{packetNum}");
                        //Console.WriteLine($"SHIT LENGTH: {bf.ReadString().Length}");
                        var shitSize = bf.ReadInteger();
                        //Console.WriteLine($"SHIT SIZE: {shitSize} bytes.");
                        //Console.WriteLine($"END PACKET #{packetNum}");
                        sTotalshitsize += shitSize;
                    }
                    else
                    {
                        var isStarting = bf.ReadBoolean();
                        if (isStarting)
                        {
                            //Console.WriteLine($"Starting timer...");
                            sShitTimer.Restart();
                        }
                        else
                        {
                            sShitTimer.Stop();
                            //Console.WriteLine($"Timer done. {ShitTimer.ElapsedMilliseconds}ms elapsed.");
                            sTimespentshitting += sShitTimer.ElapsedTicks;
                            sHitstaken++;
                            sMeasurements.Add(new ShitMeasurement
                            {
                                Elapsed = sShitTimer.ElapsedTicks,
                                Taken = 1,
                                Totalsize = 0
                            });
                        }
                    }
                }
                else
                {
                    switch (packetNum)
                    {
                        case -2:
                            foreach (var m in sMeasurements)
                            {
                                if (m.Taken < 2) continue;
                                Console.WriteLine(
                                    $"Shits: {m.Taken}, Shitrate: {m.ShitRate}s/s, Datarate: {m.DataRate / 1048576}MiB/s");
                            }
                            break;
                        case -3:
                            sWriter.Close();
                            sWriter.Dispose();
                            sWriter = null;
                            break;
                        default:
                            if (shitting)
                            {
                                sHitstaken = 0;
                                sTimespentshitting = 0;
                                sTotalshitsize = 0;
                                //Console.WriteLine("Starting to shit...");
                            }
                            else
                            {
                                var diff = 1000.0 * TimeSpan.TicksPerMillisecond;
                                //Console.WriteLine("Just flushed the toilet.");
                                //Console.WriteLine($"I took {shitstaken} shit(s).");
                                //Console.WriteLine($"It took me a total of {timespentshitting / diff}s to shit.");
                                //Console.WriteLine($"Each shit took {timespentshitting / (diff * shitstaken)}s per shit.");
                                //Console.WriteLine($"I shit at approximately {(totalshitsize / (timespentshitting / diff)) / 1024}KiB/s.");
                                sMeasurements.Add(new ShitMeasurement
                                {
                                    Elapsed = sTimespentshitting,
                                    Taken = sHitstaken,
                                    Totalsize = sTotalshitsize
                                });
                                if (sHitstaken > 0)
                                {
                                    sWriter.WriteLine($"{sTimespentshitting},{sHitstaken},{sTotalshitsize}");
                                    sWriter.Flush();
                                }
                            }
                            break;
                    }
                }
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

        private static void HandleServerConfig(byte[] packet)
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
            Globals.LoginForm.HideSafe();
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Guid mapId = bf.ReadGuid();
            int deleted = bf.ReadInteger();
            if (deleted == 1)
            {
                if (MapInstance.Get(mapId) != null)
                {
                    if (Globals.CurrentMap == MapInstance.Get(mapId))
                    {
                        Globals.MainForm.EnterMap(MapList.GetList().FindFirstMap());
                    }
                    MapInstance.Get(mapId).Delete();
                }
            }
            else
            {
                var mapJson = bf.ReadString();
                var tileLength = bf.ReadInteger();
                var tileData = bf.ReadBytes(tileLength);
                var attributeDataLength = bf.ReadInteger();
                var attributeData = bf.ReadBytes(attributeDataLength);
                var map = new MapInstance(mapId);
                map.Load(mapJson);
                map.LoadTileData(tileData);
                map.LoadAttributes(attributeData);
                map.MapGridX = bf.ReadInteger();
                map.MapGridY = bf.ReadInteger();
                map.SaveStateAsUnchanged();
                map.InitAutotiles();
                map.UpdateAdjacentAutotiles();
                if (MapInstance.Get(mapId) != null)
                {
                    lock (MapInstance.Get(mapId).MapLock)
                    {
                        if (Globals.CurrentMap == MapInstance.Get(mapId))
                        {
                            Globals.CurrentMap = map;
                        }
                        MapInstance.Get(mapId).Delete();
                    }
                }
                MapInstance.Lookup.Set(mapId, map);
                if (!Globals.InEditor && Globals.HasGameData)
                {
                    Globals.CurrentMap = map;
                    Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
                }
                else if (Globals.InEditor)
                {
                    if (Globals.FetchingMapPreviews || Globals.CurrentMap == map)
                    {
                        Guid currentMapId = Globals.CurrentMap.Id;
                        var img = Database.LoadMapCacheLegacy(mapId, map.Revision);
                        if (img == null &&  !Globals.MapsToScreenshot.Contains(mapId)) Globals.MapsToScreenshot.Add(mapId);
                        img?.Dispose();
                        if (Globals.FetchingMapPreviews)
                        {
                            if (Globals.MapsToFetch.Contains(mapId))
                            {
                                Globals.MapsToFetch.Remove(mapId);
                                if (Globals.MapsToFetch.Count == 0)
                                {
                                    Globals.FetchingMapPreviews = false;
                                    Globals.PreviewProgressForm.BeginInvoke((MethodInvoker)delegate { Globals.PreviewProgressForm.Dispose(); });
                                }
                                else
                                {
                                    //TODO Localize
                                    Globals.PreviewProgressForm.SetProgress(
                                        "Fetching Maps: " + (Globals.FetchCount - Globals.MapsToFetch.Count) + "/" +
                                        Globals.FetchCount,
                                        (int)
                                        (((float) (Globals.FetchCount - Globals.MapsToFetch.Count) /
                                          (float) Globals.FetchCount) * 100f), false);
                                }
                            }
                        }
                        Globals.CurrentMap = MapInstance.Get(currentMapId);
                    }
                    if (mapId != Globals.LoadingMap) return;
                    Globals.CurrentMap = MapInstance.Get(Globals.LoadingMap);
                    MapUpdatedDelegate();
                    if (map.Up != Guid.Empty)
                    {
                        PacketSender.SendNeedMap(map.Up);
                    }
                    if (map.Down != Guid.Empty)
                    {
                        PacketSender.SendNeedMap(map.Down);
                    }
                    if (map.Left != Guid.Empty)
                    {
                        PacketSender.SendNeedMap(map.Left);
                    }
                    if (map.Right != Guid.Empty)
                    {
                        PacketSender.SendNeedMap(map.Right);
                    }
                }
                if (Globals.CurrentMap.Id == mapId && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].MapId);
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
            Globals.MainForm.BeginInvoke((Action) (() => Globals.MainForm.EnterMap(bf.ReadGuid())));
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
            Globals.MapListWindow.BeginInvoke(Globals.MapListWindow.mapTreeList.MapListDelegate, Guid.Empty, null);
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
                                var needMap = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].MapId);
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
            var id = bf.ReadGuid();
            var another = Convert.ToBoolean(bf.ReadInteger());
            var deleted = Convert.ToBoolean(bf.ReadInteger());
            var json = "";
            if (!deleted)
                json = bf.ReadString();
            switch (type)
            {
                case GameObjectType.Animation:
                    if (deleted)
                    {
                        var anim = AnimationBase.Get(id);
                        anim.Delete();
                    }
                    else
                    {
                        var anim = new AnimationBase(id);
                        anim.Load(json);
                        try
                        {
                            AnimationBase.Lookup.Set(id, anim);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"Another mystery NPE. [Lookup={AnimationBase.Lookup}]");
                            if (exception.InnerException != null) Log.Error(exception.InnerException);
                            Log.Error(exception);
                            Log.Error($"{nameof(id)}={id},{nameof(anim)}={anim}");
                            throw;
                        }
                    }
                    break;
                case GameObjectType.Class:
                    if (deleted)
                    {
                        var cls = ClassBase.Get(id);
                        cls.Delete();
                    }
                    else
                    {
                        var cls = new ClassBase(id);
                        cls.Load(json);
                        ClassBase.Lookup.Set(id, cls);
                    }
                    break;
                case GameObjectType.Item:
                    if (deleted)
                    {
                        var itm = ItemBase.Get(id);
                        itm.Delete();
                    }
                    else
                    {
                        var itm = new ItemBase(id);
                        itm.Load(json);
                        ItemBase.Lookup.Set(id, itm);
                    }
                    break;
                case GameObjectType.Npc:
                    if (deleted)
                    {
                        var npc = NpcBase.Get(id);
                        npc.Delete();
                    }
                    else
                    {
                        var npc = new NpcBase(id);
                        npc.Load(json);
                        NpcBase.Lookup.Set(id, npc);
                    }
                    break;
                case GameObjectType.Projectile:
                    if (deleted)
                    {
                        var proj = ProjectileBase.Get(id);
                        proj.Delete();
                    }
                    else
                    {
                        var proj = new ProjectileBase(id);
                        proj.Load(json);
                        ProjectileBase.Lookup.Set(id, proj);
                    }
                    break;
                case GameObjectType.Quest:
                    if (deleted)
                    {
                        var qst = QuestBase.Get(id);
                        qst.Delete();
                    }
                    else
                    {
                        var qst = new QuestBase(id);
                        qst.Load(json);
                        QuestBase.Lookup.Set(id, qst);
                    }
                    break;
                case GameObjectType.Resource:
                    if (deleted)
                    {
                        var res = ResourceBase.Get(id);
                        res.Delete();
                    }
                    else
                    {
                        var res = new ResourceBase(id);
                        res.Load(json);
                        ResourceBase.Lookup.Set(id, res);
                    }
                    break;
                case GameObjectType.Shop:
                    if (deleted)
                    {
                        var shp = ShopBase.Get(id);
                        shp.Delete();
                    }
                    else
                    {
                        var shp = new ShopBase(id);
                        shp.Load(json);
                        ShopBase.Lookup.Set(id, shp);
                    }
                    break;
                case GameObjectType.Spell:
                    if (deleted)
                    {
                        var spl = SpellBase.Get(id);
                        spl.Delete();
                    }
                    else
                    {
                        var spl = new SpellBase(id);
                        spl.Load(json);
                        SpellBase.Lookup.Set(id, spl);
                    }
                    break;
                case GameObjectType.CraftTables:
                    if (deleted)
                    {
                        var cft = CraftingTableBase.Get(id);
                        cft.Delete();
                    }
                    else
                    {
                        var cft = new CraftingTableBase(id);
                        cft.Load(json);
                        CraftingTableBase.Lookup.Set(id, cft);
                    }
                    break;
                case GameObjectType.Crafts:
                    if (deleted)
                    {
                        var cft = CraftBase.Get(id);
                        cft.Delete();
                    }
                    else
                    {
                        var cft = new CraftBase(id);
                        cft.Load(json);
                        CraftBase.Lookup.Set(id, cft);
                    }
                    break;
                case GameObjectType.Map:
                    //Handled in a different packet
                    break;
                case GameObjectType.Event:
                    if (deleted)
                    {
                        var evt = EventBase.Get(id);
                        evt.Delete();
                    }
                    else
                    {
                        var evt = new EventBase(id,true);
                        evt.Load(json);
                        EventBase.Lookup.Set(id, evt);
                    }
                    break;
                case GameObjectType.PlayerSwitch:
                    if (deleted)
                    {
                        var pswtch = PlayerSwitchBase.Get(id);
                        pswtch.Delete();
                    }
                    else
                    {
                        var pswtch = new PlayerSwitchBase(id);
                        pswtch.Load(json);
                        PlayerSwitchBase.Lookup.Set(id, pswtch);
                    }
                    break;
                case GameObjectType.PlayerVariable:
                    if (deleted)
                    {
                        var pvar = PlayerVariableBase.Get(id);
                        pvar.Delete();
                    }
                    else
                    {
                        var pvar = new PlayerVariableBase(id);
                        pvar.Load(json);
                        PlayerVariableBase.Lookup.Set(id, pvar);
                    }
                    break;
                case GameObjectType.ServerSwitch:
                    if (deleted)
                    {
                        var sswtch = ServerSwitchBase.Get(id);
                        sswtch.Delete();
                    }
                    else
                    {
                        var sswtch = new ServerSwitchBase(id);
                        sswtch.Load(json);
                        ServerSwitchBase.Lookup.Set(id, sswtch);
                    }
                    break;
                case GameObjectType.ServerVariable:
                    if (deleted)
                    {
                        var svar = ServerVariableBase.Get(id);
                        svar.Delete();
                    }
                    else
                    {
                        var svar = new ServerVariableBase(id);
                        svar.Load(json);
                        ServerVariableBase.Lookup.Set(id, svar);
                    }
                    break;
                case GameObjectType.Tileset:
                    var obj = new TilesetBase(id);
                    obj.Load(json);
                    TilesetBase.Lookup.Set(id, obj);
                    if (Globals.HasGameData && !another) GameContentManager.LoadTilesets();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            GameObjectUpdatedDelegate?.Invoke(type);
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

        private struct ShitMeasurement
        {
            public int Taken;
            public long Totalsize;
            public long Elapsed;

            public double ShitRate => Taken / (Elapsed / (double) TimeSpan.TicksPerSecond);
            public double DataRate => Totalsize / (Elapsed / (double) TimeSpan.TicksPerSecond);
        }
    }
}