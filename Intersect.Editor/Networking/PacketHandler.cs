using System;
using System.Windows.Forms;

using Intersect.Editor.Content;
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
using Intersect.Network.Packets.Server;

namespace Intersect.Editor.Networking
{

    public static class PacketHandler
    {

        public delegate void GameObjectUpdated(GameObjectType type);

        public delegate void MapUpdated();

        public static GameObjectUpdated GameObjectUpdatedDelegate;

        public static MapUpdated MapUpdatedDelegate;

        public static bool HandlePacket(IConnection connection, IPacket packet)
        {
            if (packet is CerasPacket)
            {
                HandlePacket((dynamic) packet);
            }

            return true;
        }

        //PingPacket
        private static void HandlePacket(PingPacket packet)
        {
            if (packet.RequestingReply)
            {
                PacketSender.SendPing();
            }
        }

        //ConfigPacket
        private static void HandlePacket(ConfigPacket packet)
        {
            Options.LoadFromServer(packet.Config);
        }

        //JoinGamePacket
        private static void HandlePacket(JoinGamePacket packet)
        {
            Globals.LoginForm.TryRemembering();
            Globals.LoginForm.HideSafe();
        }

        //MapPacket
        private static void HandlePacket(MapPacket packet)
        {
            if (packet.Deleted)
            {
                if (MapInstance.Get(packet.MapId) != null)
                {
                    if (Globals.CurrentMap == MapInstance.Get(packet.MapId))
                    {
                        Globals.MainForm.EnterMap(MapList.List.FindFirstMap());
                    }

                    MapInstance.Get(packet.MapId).Delete();
                }
            }
            else
            {
                var map = new MapInstance(packet.MapId);
                map.Load(packet.Data);
                map.LoadTileData(packet.TileData);
                map.AttributeData = packet.AttributeData;
                map.MapGridX = packet.GridX;
                map.MapGridY = packet.GridY;
                map.SaveStateAsUnchanged();
                map.InitAutotiles();
                map.UpdateAdjacentAutotiles();
                if (MapInstance.Get(packet.MapId) != null)
                {
                    lock (MapInstance.Get(packet.MapId).MapLock)
                    {
                        if (Globals.CurrentMap == MapInstance.Get(packet.MapId))
                        {
                            Globals.CurrentMap = map;
                        }

                        MapInstance.Get(packet.MapId).Delete();
                    }
                }

                MapInstance.Lookup.Set(packet.MapId, map);
                if (!Globals.InEditor && Globals.HasGameData)
                {
                    Globals.CurrentMap = map;
                    Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
                }
                else if (Globals.InEditor)
                {
                    if (Globals.FetchingMapPreviews || Globals.CurrentMap == map)
                    {
                        var currentMapId = Globals.CurrentMap.Id;
                        var img = Database.LoadMapCacheLegacy(packet.MapId, map.Revision);
                        if (img == null && !Globals.MapsToScreenshot.Contains(packet.MapId))
                        {
                            Globals.MapsToScreenshot.Add(packet.MapId);
                        }

                        img?.Dispose();
                        if (Globals.FetchingMapPreviews)
                        {
                            if (Globals.MapsToFetch.Contains(packet.MapId))
                            {
                                Globals.MapsToFetch.Remove(packet.MapId);
                                if (Globals.MapsToFetch.Count == 0)
                                {
                                    Globals.FetchingMapPreviews = false;
                                    Globals.PreviewProgressForm.BeginInvoke(
                                        (MethodInvoker) delegate { Globals.PreviewProgressForm.Dispose(); }
                                    );
                                }
                                else
                                {
                                    //TODO Localize
                                    Globals.PreviewProgressForm.SetProgress(
                                        "Fetching Maps: " +
                                        (Globals.FetchCount - Globals.MapsToFetch.Count) +
                                        "/" +
                                        Globals.FetchCount,
                                        (int) ((float) (Globals.FetchCount - Globals.MapsToFetch.Count) /
                                               (float) Globals.FetchCount *
                                               100f), false
                                    );
                                }
                            }
                        }

                        Globals.CurrentMap = MapInstance.Get(currentMapId);
                    }

                    if (packet.MapId != Globals.LoadingMap)
                    {
                        return;
                    }

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

                if (Globals.CurrentMap.Id == packet.MapId && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (var y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                {
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].MapId);
                                }
                            }
                        }
                    }
                }
            }
        }

        //GameDataPacket
        private static void HandlePacket(GameDataPacket packet)
        {
            foreach (var obj in packet.GameObjects)
            {
                HandlePacket((dynamic) obj);
            }

            Globals.HasGameData = true;
            if (!Globals.InEditor && Globals.HasGameData && Globals.CurrentMap != null)
            {
                Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
            }

            GameContentManager.LoadTilesets();
        }

        //EnterMapPacket
        private static void HandlePacket(EnterMapPacket packet)
        {
            Globals.MainForm.BeginInvoke((Action) (() => Globals.MainForm.EnterMap(packet.MapId)));
        }

        //MapListPacket
        private static void HandlePacket(MapListPacket packet)
        {
            MapList.List.JsonData = packet.MapListData;
            MapList.List.PostLoad(MapBase.Lookup, false, true);
            if (Globals.CurrentMap == null)
            {
                Globals.MainForm.EnterMap(MapList.List.FindFirstMap());
            }

            Globals.MapListWindow.BeginInvoke(Globals.MapListWindow.mapTreeList.MapListDelegate, Guid.Empty, null);
            Globals.MapPropertiesWindow?.BeginInvoke(Globals.MapPropertiesWindow.UpdatePropertiesDelegate);
        }

        //ErrorMessagePacket
        private static void HandlePacket(ErrorMessagePacket packet)
        {
            MessageBox.Show(packet.Error, packet.Header, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //MapGridPacket
        private static void HandlePacket(MapGridPacket packet)
        {
            if (Globals.MapGrid != null)
            {
                Globals.MapGrid.Load(packet.EditorGrid);
                if (Globals.CurrentMap != null && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    for (var y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                    {
                        for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                        {
                            if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                            {
                                var needMap = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                if (needMap == null && Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                {
                                    PacketSender.SendNeedMap(Globals.MapGrid.Grid[x, y].MapId);
                                }
                            }
                        }
                    }
                }
            }
        }

        //GameObjectPacket
        private static void HandlePacket(GameObjectPacket packet)
        {
            var id = packet.Id;
            var deleted = packet.Deleted;
            var json = "";
            if (!packet.Deleted)
            {
                json = packet.Data;
            }

            switch (packet.Type)
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
                            if (exception.InnerException != null)
                            {
                                Log.Error(exception.InnerException);
                            }

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
                        foreach (var tsk in qst.Tasks)
                        {
                            qst.OriginalTaskEventIds.Add(tsk.Id, tsk.CompletionEventId);
                        }

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
                    var wasCommon = false;
                    if (deleted)
                    {
                        var evt = EventBase.Get(id);
                        wasCommon = evt.CommonEvent;
                        evt.Delete();
                    }
                    else
                    {
                        var evt = new EventBase(id);
                        evt.Load(json);
                        wasCommon = evt.CommonEvent;
                        EventBase.Lookup.Set(id, evt);
                    }

                    if (!wasCommon)
                    {
                        return;
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
                    if (Globals.HasGameData && !packet.AnotherFollowing)
                    {
                        GameContentManager.LoadTilesets();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameObjectUpdatedDelegate?.Invoke(packet.Type);
        }

        //OpenEditorPacket
        private static void HandlePacket(OpenEditorPacket packet)
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, packet.Type);
        }

        //TimeDataPacket
        private static void HandlePacket(TimeDataPacket packet)
        {
            TimeBase.GetTimeBase().LoadFromJson(packet.TimeJson);
        }

    }

}
