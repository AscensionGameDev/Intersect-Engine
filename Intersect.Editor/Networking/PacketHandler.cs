using System;
using System.Linq;
using System.Windows.Forms;

using Intersect.Core;
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
    internal sealed partial class PacketHandler
    {
        private sealed class VirtualPacketSender : IPacketSender
        {
            public IApplicationContext ApplicationContext { get; }

            public VirtualPacketSender(IApplicationContext applicationContext) =>
                ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));

            #region Implementation of IPacketSender

            /// <inheritdoc />
            public bool Send(IPacket packet)
            {
                if (packet is IntersectPacket intersectPacket)
                {
                    Network.SendPacket(intersectPacket);

                    return true;
                }

                return false;
            }

            #endregion
        }

        public delegate void GameObjectUpdated(GameObjectType type);

        public delegate void MapUpdated();

        public static GameObjectUpdated GameObjectUpdatedDelegate;

        public static MapUpdated MapUpdatedDelegate;

        public IApplicationContext Context { get; }

        public Logger Logger { get; }

        public PacketHandlerRegistry Registry { get; }

        public IPacketSender VirtualSender { get; }

        public PacketHandler(IApplicationContext context, PacketHandlerRegistry packetHandlerRegistry)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Registry = packetHandlerRegistry ?? throw new ArgumentNullException(nameof(packetHandlerRegistry));

            if (!Registry.TryRegisterAvailableMethodHandlers(GetType(), this, false) || Registry.IsEmpty)
            {
                throw new InvalidOperationException("Failed to register method handlers, see logs for more details.");
            }

            VirtualSender = new VirtualPacketSender(context);
        }

        public bool HandlePacket(IConnection connection, IPacket packet)
        {
            if (!(packet is IntersectPacket))
            {
                return false;
            }

            if (!Registry.TryGetHandler(packet, out HandlePacketGeneric handler))
            {
                Logger.Error($"No registered handler for {packet.GetType().FullName}!");

                return false;
            }

            if (Registry.TryGetPreprocessors(packet, out var preprocessors))
            {
                if (!preprocessors.All(preprocessor => preprocessor.Handle(VirtualSender, packet)))
                {
                    // Preprocessors are intended to be silent filter functions
                    return false;
                }
            }

            if (Registry.TryGetPreHooks(packet, out var preHooks))
            {
                if (!preHooks.All(hook => hook.Handle(VirtualSender, packet)))
                {
                    // Hooks should not fail, if they do that's an error
                    Logger.Error($"PreHook handler failed for {packet.GetType().FullName}.");
                    return false;
                }
            }

            if (!handler(VirtualSender, packet))
            {
                return false;
            }

            if (Registry.TryGetPostHooks(packet, out var postHooks))
            {
                if (!postHooks.All(hook => hook.Handle(VirtualSender, packet)))
                {
                    // Hooks should not fail, if they do that's an error
                    Logger.Error($"PostHook handler failed for {packet.GetType().FullName}.");
                    return false;
                }
            }

            return true;
        }

        //PingPacket
        public void HandlePacket(IPacketSender packetSender, PingPacket packet)
        {
            if (packet.RequestingReply)
            {
                PacketSender.SendPing();
            }
        }

        //ConfigPacket
        public void HandlePacket(IPacketSender packetSender, ConfigPacket packet)
        {
            Options.LoadFromServer(packet.Config);
        }

        //JoinGamePacket
        public void HandlePacket(IPacketSender packetSender, JoinGamePacket packet)
        {
            Globals.LoginForm.TryRemembering();
            Globals.LoginForm.HideSafe();
        }

        //MapPacket
        public void HandlePacket(IPacketSender packetSender, MapPacket packet)
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
        public void HandlePacket(IPacketSender packetSender, GameDataPacket packet)
        {
            foreach (var obj in packet.GameObjects)
            {
                HandlePacket(packetSender, obj);
            }

            Globals.HasGameData = true;
            if (!Globals.InEditor && Globals.HasGameData && Globals.CurrentMap != null)
            {
                Globals.LoginForm.BeginInvoke(Globals.LoginForm.EditorLoopDelegate);
            }

            GameContentManager.LoadTilesets();
        }

        //EnterMapPacket
        public void HandlePacket(IPacketSender packetSender, EnterMapPacket packet)
        {
            Globals.MainForm.BeginInvoke((Action) (() => Globals.MainForm.EnterMap(packet.MapId)));
        }

        //MapListPacket
        public void HandlePacket(IPacketSender packetSender, MapListPacket packet)
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
        public void HandlePacket(IPacketSender packetSender, ErrorMessagePacket packet)
        {
            MessageBox.Show(packet.Error, packet.Header, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //MapGridPacket
        public void HandlePacket(IPacketSender packetSender, MapGridPacket packet)
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
        public void HandlePacket(IPacketSender packetSender, GameObjectPacket packet)
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
        public void HandlePacket(IPacketSender packetSender, OpenEditorPacket packet)
        {
            Globals.MainForm.BeginInvoke(Globals.MainForm.EditorDelegate, packet.Type);
        }

        //TimeDataPacket
        public void HandlePacket(IPacketSender packetSender, TimeDataPacket packet)
        {
            TimeBase.GetTimeBase().LoadFromJson(packet.TimeJson);
        }
    }
}
