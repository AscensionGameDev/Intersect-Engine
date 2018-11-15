using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.General;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Client.UI;
using Intersect.Client.UI.Game;
using Intersect.Client.UI.Game.Chat;
using Intersect.Client.UI.Menu;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;

namespace Intersect.Client.Networking
{
    public static class PacketHandler
    {
        public static long Ping = 0;
        public static long PingTime;

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
            var packetHeader = (ServerPackets) bf.ReadLong();
            sPacketCount++;
            if (sDebugPackets)
            {
                Debug.WriteLine("Handled " + packetHeader + " - " + sPacketCount);
            }
            lock (Globals.GameLock)
            {
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
                    case ServerPackets.EntityData:
                        HandleEntityData(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityPosition:
                        HandlePositionInfo(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityLeave:
                        HandleLeave(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ChatMessage:
                        HandleMsg(bf.ReadBytes(bf.Length()));
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
                    case ServerPackets.EntityMove:
                        HandleEntityMove(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityVitals:
                        HandleVitals(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityStats:
                        HandleStats(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityDir:
                        HandleEntityDir(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EventDialog:
                        HandleEventDialog(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.LoginError:
                        HandleLoginError(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.MapItems:
                        HandleMapItems(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.MapItemUpdate:
                        HandleMapItemUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.InventoryUpdate:
                        HandleInventoryUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.SpellUpdate:
                        HandleSpellUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PlayerEquipment:
                        HandlePlayerEquipment(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.StatPoints:
                        HandleStatPoints(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.HotbarSlots:
                        HandleHotbarSlots(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.CreateCharacter:
                        HandleCreateCharacter();
                        break;
                    case ServerPackets.OpenAdminWindow:
                        HandleOpenAdminWindow();
                        break;
                    case ServerPackets.CastTime:
                        HandleCastTime(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.SpellCooldown:
                        HandleSpellCooldown(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ItemCooldown:
                        HandleItemCooldown(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.Experience:
                        HandleExperience(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ProjectileSpawnDead:
                        HandleProjectileSpawnDead(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.SendPlayAnimation:
                        HandlePlayAnimation(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.HoldPlayer:
                        HandleHoldPlayer(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ReleasePlayer:
                        HandleReleasePlayer(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PlayMusic:
                        HandlePlayMusic(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.FadeMusic:
                        HandleFadeMusic(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PlaySound:
                        HandlePlaySound(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.StopSounds:
                        HandleStopSounds(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ShowPicture:
                        HandleShowPicture(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.HidePicture:
                        HandleHidePicture(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.OpenShop:
                        HandleOpenShop(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.CloseShop:
                        HandleCloseShop(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.OpenBank:
                        HandleOpenBank(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.CloseBank:
                        HandleCloseBank(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.BankUpdate:
                        HandleBankUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.OpenCraftingTable:
                        HandleOpenCraftingTable(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.CloseCraftingTable:
                        HandleCloseCraftingTable(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.GameObject:
                        HandleGameObject(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityDash:
                        HandleEntityDash(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityAttack:
                        HandleEntityAttack(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ActionMsg:
                        HandleActionMsg(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.MapGrid:
                        HandleMapGrid(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.Time:
                        HandleTime(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PartyData:
                        HandleParty(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PartyUpdate:
                        HandlePartyUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PartyInvite:
                        HandlePartyInvite(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.ChatBubble:
                        HandleChatBubble(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.MapEntities:
                        HandleMapEntities(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.QuestOffer:
                        HandleQuestOffer(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.QuestProgress:
                        HandleQuestProgress(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.TradeStart:
                        HandleTradeStart(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.TradeUpdate:
                        HandleTradeUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.TradeClose:
                        HandleTradeClose(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.TradeRequest:
                        HandleTradeRequest(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.NpcAggression:
                        HandleNpcAggression(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PlayerDeath:
                        HandlePlayerDash(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityZDimension:
                        HandleEntityZDimension(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.OpenBag:
                        HandleOpenBag(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.CloseBag:
                        HandleCloseBag(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.BagUpdate:
                        HandleBagUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.MoveRouteToggle:
                        HandleMoveRouteToggle(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.SendFriends:
                        HandleFriends(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.FriendRequest:
                        HandleFriendRequest(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.PlayerCharacters:
                        HandlePlayerCharacters(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.Shit:
                        HandleShit(bf.ReadBytes(bf.Length()));
                        break;
                    default:
                        Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                        break;
                }
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
                PingTime = Globals.System.GetTimeMs();
            }
            else
            {
                GameNetwork.Ping = (int) (Globals.System.GetTimeMs() - PingTime) / 2;
            }
        }

        private static void HandleServerConfig(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Options.LoadFromServer(bf);
            Globals.Bank = new ItemInstance[Options.MaxBankSlots];
            GameGraphics.InitInGame();
        }

        private static void HandleJoinGame(byte[] packet)
        {
            GameMain.JoinGame();
            Globals.JoiningGame = true;
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapId = bf.ReadGuid();
            bf.ReadInteger();
            var mapJson = bf.ReadString();
            var tileLength = bf.ReadInteger();
            var tileData = bf.ReadBytes(tileLength);
            var attributeLength = bf.ReadInteger();
            var attributeData = bf.ReadBytes(attributeLength);
            var revision = bf.ReadInteger();
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                if (revision == map.Revision)
                {
                    return;
                }
                else
                {
                    map.Dispose(false, false);
                }
            }
            map = new MapInstance(mapId);
            MapInstance.Lookup.Set(mapId, map);
            lock (map.MapLock)
            {
                map.Load(mapJson);
                map.LoadTileData(tileData);
                map.AttributeData = attributeData;
                if ((mapId) == Globals.Me.CurrentMap)
                {
                    GameAudio.PlayMusic(map.Music, 3, 3, true);
                }
                map.MapGridX = bf.ReadInteger();
                map.MapGridY = bf.ReadInteger();
                map.HoldLeft = bf.ReadInteger();
                map.HoldRight = bf.ReadInteger();
                map.HoldUp = bf.ReadInteger();
                map.HoldDown = bf.ReadInteger();
                map.Autotiles.InitAutotiles(map.GenerateAutotileGrid());

                if (Globals.PendingEvents.ContainsKey(mapId))
                {
                    foreach (var evt in Globals.PendingEvents[mapId])
                    {
                        map.AddEvent(evt.Key, evt.Value);
                    }

                    Globals.PendingEvents[mapId].Clear();
                }
            }
            if (MapInstance.OnMapLoaded != null) MapInstance.OnMapLoaded(map);
            Globals.Me.FetchNewMaps();
        }

        private static void HandleEntityData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var entityType = bf.ReadInteger();
            var mapId = bf.ReadGuid(false);
            if (entityType != (int) EntityTypes.Event)
            {
                var en = Globals.GetEntity(id, entityType);
                if (en != null)
                {
                    en.Load(bf);
                }
                else
                {
                    switch (entityType)
                    {
                        case (int) EntityTypes.Player:
                            Globals.Entities.Add(id, new Player(id, bf));
                            break;
                        case (int) EntityTypes.GlobalEntity:
                            Globals.Entities.Add(id, new Entity(id, bf));
                            break;
                        case (int) EntityTypes.Resource:
                            Globals.Entities.Add(id, new Resource(id, bf));
                            break;
                        case (int) EntityTypes.Projectile:
                            Globals.Entities.Add(id, new Projectile(id, bf));
                            break;
                    }
                }
            }
            else
            {
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    map?.AddEvent(id, bf);
                }
                else
                {
                    var dict = Globals.PendingEvents.ContainsKey(mapId) ? Globals.PendingEvents[mapId] : new Dictionary<Guid, ByteBuffer>();
                    if (dict.ContainsKey(id))
                    {
                        dict[id] = bf;
                    }
                    else
                    {
                        dict.Add(id,bf);
                    }
                    if (!Globals.PendingEvents.ContainsKey(mapId)) Globals.PendingEvents.Add(mapId,dict);
                }
            }
        }

        private static void HandleMapEntities(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var entityCount = bf.ReadInteger();
            for (int z = 0; z < entityCount; z++)
            {
                var id = bf.ReadGuid();
                var entityType = bf.ReadInteger();
                if (entityType != (int) EntityTypes.Event)
                {
                    var en = Globals.GetEntity(id, entityType);
                    if (en != null)
                    {
                        en.Load(bf);
                    }
                    else
                    {
                        switch (entityType)
                        {
                            case (int) EntityTypes.Player:
                                Globals.Entities.Add(id, new Player(id, bf));
                                break;
                            case (int) EntityTypes.GlobalEntity:
                                Globals.Entities.Add(id, new Entity(id, bf));
                                break;
                            case (int) EntityTypes.Resource:
                                Globals.Entities.Add(id, new Resource(id, bf));
                                break;
                            case (int) EntityTypes.Projectile:
                                Globals.Entities.Add(id, new Projectile(id, bf));
                                break;
                        }
                    }
                }
                else
                {
                    new Event(id, bf);
                }
            }
        }

        private static void HandlePositionInfo(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en;
            if (type != (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                if (MapInstance.Get(mapId) == null) return;
                if (!MapInstance.Get(mapId).LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = MapInstance.Get(mapId).LocalEntities[id];
            }
            if (en == Globals.Me &&
                (Globals.Me.DashQueue.Count > 0 || Globals.Me.DashTimer > Globals.System.GetTimeMs())) return;
            if (en == Globals.Me && Globals.Me.CurrentMap != mapId)
            {
                Globals.Me.CurrentMap = mapId;
                Globals.NeedsMaps = true;
                Globals.Me.FetchNewMaps();
            }
            else
            {
                en.CurrentMap = mapId;
            }
            en.CurrentX = bf.ReadInteger();
            en.CurrentY = bf.ReadInteger();
            en.Dir = bf.ReadInteger();
            en.Passable = bf.ReadBoolean();
            en.HideName = bf.ReadBoolean();
        }

        private static void HandleLeave(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            if (id == Globals.Me.Id && type < (int) EntityTypes.Event)
            {
                return;
            }
            if (type != (int) EntityTypes.Event)
            {
                if (Globals.Entities.ContainsKey(id))
                {
                    Globals.Entities[id].Dispose();
                    Globals.EntitiesToDispose.Add(id);
                }
            }
            else
            {
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    if (map.LocalEntities.ContainsKey(id))
                    {
                        map.LocalEntities[id].Dispose();
                        map.LocalEntities[id] = null;
                        map.LocalEntities.Remove(id);
                    }
                }
            }
        }

        private static void HandleMsg(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ChatboxMsg.AddMessage(new ChatboxMsg(bf.ReadString(),
                new Framework.GenericClasses.Color((int) bf.ReadByte(), (int) bf.ReadByte(), (int) bf.ReadByte(), (int) bf.ReadByte()),
                bf.ReadString()));
        }

        private static void HandleActionMsg(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var map = MapInstance.Get(bf.ReadGuid());
            if (map != null)
            {
                map.ActionMsgs.Add(new ActionMsgInstance(map, bf.ReadInteger(), bf.ReadInteger(),
                    bf.ReadString(),
                    new Framework.GenericClasses.Color((int) bf.ReadByte(), (int) bf.ReadByte(), (int) bf.ReadByte(), (int) bf.ReadByte())));
            }
            bf.Dispose();
        }

        private static void HandleGameData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            CustomColors.Load(bf);
            Globals.HasGameData = true;
        }

        private static void HandleEnterMap(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapId = bf.ReadGuid();
            if (Globals.Me != null && Globals.Me.CurrentMap != mapId && Globals.Me.CurrentMap != Guid.Empty) return; //TODO WTF -- why does this do nothing?
        }

        private static void HandleMapList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            MapList.GetList().JsonData = bf.ReadString();
            MapList.GetList().PostLoad(MapBase.Lookup, false, true);
            //If admin window is open update it
            bf.Dispose();
        }

        private static void HandleEntityMove(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var gameMap = MapInstance.Get(mapId);
                if (gameMap == null) return;
                if (!gameMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = gameMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }
            var entityMap = MapInstance.Get(en.CurrentMap);
            if (entityMap == null)
            {
                return;
            }
            if (en.Dashing != null || en.DashQueue.Count > 0) return;
            var map = mapId;
            var x = bf.ReadInteger();
            var y = bf.ReadInteger();
            var dir = bf.ReadInteger();
            var correction = bf.ReadInteger();
            if ((en.CurrentMap != map || en.CurrentX != x || en.CurrentY != y) &&
                (en != Globals.Me || (en == Globals.Me && correction == 1)) && en.Dashing == null)
            {
                en.CurrentMap = map;
                en.CurrentX = x;
                en.CurrentY = y;
                en.Dir = dir;
                en.IsMoving = true;

                switch (en.Dir)
                {
                    case 0:
                        en.OffsetY = Options.TileWidth;
                        en.OffsetX = 0;
                        break;
                    case 1:
                        en.OffsetY = -Options.TileWidth;
                        en.OffsetX = 0;
                        break;
                    case 2:
                        en.OffsetY = 0;
                        en.OffsetX = Options.TileWidth;
                        break;
                    case 3:
                        en.OffsetY = 0;
                        en.OffsetX = -Options.TileWidth;
                        break;
                }
            }

            // Set the Z-Dimension if the player has moved up or down a dimension.
            if (entityMap.Attributes[en.CurrentX, en.CurrentY] != null && entityMap.Attributes[en.CurrentX, en.CurrentY].Type == MapAttributes.ZDimension)
            {
                if (((MapZDimensionAttribute)entityMap.Attributes[en.CurrentX, en.CurrentY]).GatewayTo > 0)
                {
                    en.CurrentZ = ((MapZDimensionAttribute)entityMap.Attributes[en.CurrentX, en.CurrentY]).GatewayTo - 1;
                }
            }
        }

        private static void HandleVitals(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en = null;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = entityMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                en.MaxVital[i] = bf.ReadInteger();
                en.Vital[i] = bf.ReadInteger();
            }

            //Update status effects
            var count = bf.ReadInteger();
            en.Status.Clear();
            for (int i = 0; i < count; i++)
            {
                en.Status.Add(new StatusInstance(bf.ReadGuid(), bf.ReadInteger(), bf.ReadString(), bf.ReadInteger(),
                    bf.ReadInteger()));
            }

            if (Gui.GameUi != null)
            {
                //If its you or your target, update the entity box.
                if (id == Globals.Me.Id && Gui.GameUi.PlayerBox != null)
                {
                    Gui.GameUi.PlayerBox.UpdateStatuses = true;
                }
                else if (id == Globals.Me.TargetIndex && Globals.Me.TargetBox != null)
                {
                    Globals.Me.TargetBox.UpdateStatuses = true;
                }
            }
        }

        private static void HandleStats(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en = null;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = entityMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                en.Stat[i] = bf.ReadInteger();
            }
        }

        private static void HandleEntityDir(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en = null;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = entityMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }
            en.Dir = bf.ReadInteger();
        }

        private static void HandleEntityAttack(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            var attackTimer = bf.ReadInteger();

            Entity en = null;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = entityMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }

            if (attackTimer > -1 && en != Globals.Me)
            {
                en.AttackTimer = Globals.System.GetTimeMs() + attackTimer;
            }
        }

        private static void HandleEventDialog(byte[] packet)
        {
            var bf = new ByteBuffer();
            var ed = new EventDialog();
            bf.WriteBytes(packet);
            ed.Prompt = bf.ReadString();
            ed.Face = bf.ReadString();
            ed.Type = bf.ReadInteger();
            if (ed.Type == 0)
            {
            }
            else
            {
                ed.Opt1 = bf.ReadString();
                ed.Opt2 = bf.ReadString();
                ed.Opt3 = bf.ReadString();
                ed.Opt4 = bf.ReadString();
            }
            ed.EventId = bf.ReadGuid();
            Globals.EventDialogs.Add(ed);
        }

        private static void HandleLoginError(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var error = bf.ReadString();
            var header = bf.ReadString();
            GameFade.FadeIn();
            Globals.WaitingOnServer = false;
            Gui.MsgboxErrors.Add(new KeyValuePair<string, string>(header, error));
            Gui.MenuUi.Reset();
        }

        private static void HandleMapItems(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapId = bf.ReadGuid();
            var map = MapInstance.Get(mapId);
            if (map == null) return;
            map.MapItems.Clear();
            int itemCount = bf.ReadInteger();
            for (int i = 0; i < itemCount; i++)
            {
                var index = bf.ReadInteger();
                if (index != -1)
                {
                    var item = new MapItemInstance();
                    item.Load(bf);
                    map.MapItems.Add(index, item);
                }
            }
            bf.Dispose();
        }

        private static void HandleMapItemUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapId = bf.ReadGuid();
            int index = bf.ReadInteger();
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                if (bf.ReadInteger() == -1)
                {
                    map.MapItems.Remove(index);
                }
                else
                {
                    if (!map.MapItems.ContainsKey(index))
                    {
                        map.MapItems.Add(index, new MapItemInstance());
                        map.MapItems[index].Load(bf);
                    }
                    else
                    {
                        map.MapItems[index] = new MapItemInstance();
                        map.MapItems[index].Load(bf);
                    }
                }
            }
            bf.Dispose();
        }

        private static void HandleInventoryUpdate(byte[] packet)
        {
            if (Globals.Me != null)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                int slot = bf.ReadInteger();
                Globals.Me.Inventory[slot].Load(bf);
                if (Globals.Me.InventoryUpdatedDelegate != null)
                {
                    Globals.Me.InventoryUpdatedDelegate();
                }
                bf.Dispose();
            }
        }

        private static void HandleSpellUpdate(byte[] packet)
        {
            if (Globals.Me != null)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                int slot = bf.ReadInteger();
                Globals.Me.Spells[slot].Load(bf);
                bf.Dispose();
            }
        }

        private static void HandlePlayerEquipment(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var entityId = bf.ReadGuid();
            if (Globals.Entities.ContainsKey(entityId))
            {
                var entity = Globals.Entities[entityId];
                if (entity != null)
                {
                    if (entity == Globals.Me)
                    {
                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            if (entity.Equipment.Length <= i)
                            {
                                Log.Debug($"Bad equipment index, aborting ({i}/{entity.Equipment.Length}).");
                                break;
                            }
                            entity.MyEquipment[i] = bf.ReadInteger();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            if (entity.Equipment.Length <= i)
                            {
                                Log.Debug($"Bad equipment index, aborting ({i}/{entity.Equipment.Length}).");
                                break;
                            }
                            entity.Equipment[i] = bf.ReadGuid();
                        }
                    }
                }
            }
            bf.Dispose();
        }

        private static void HandleStatPoints(byte[] packet)
        {
            if (Globals.Me != null)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                Globals.Me.StatPoints = bf.ReadInteger();
                bf.Dispose();
            }
        }

        private static void HandleHotbarSlots(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                if (Globals.Me == null)
                {
                    Log.Debug("Can't set hotbar, Globals.Me is null!");
                    break;
                }

                if (Globals.Me.Hotbar == null)
                {
                    Log.Debug("Can't set hotbar, hotbar is null!");
                    break;
                }

                var hotbarEntry = Globals.Me.Hotbar[i];
                if (hotbarEntry == null)
                {
                    Log.Error(BitConverter.ToString(packet));
                }
                hotbarEntry.ItemOrSpellId = bf.ReadGuid();
                hotbarEntry.BagId = bf.ReadGuid();
                var hasStats = bf.ReadBoolean();
                hotbarEntry.PreferredStats = new int[(int)Stats.StatCount];
                if (hasStats)
                {
                    for (int s = 0; s < (int)Stats.StatCount; s++)
                        hotbarEntry.PreferredStats[s] = bf.ReadInteger();
                }
            }
            bf.Dispose();
        }

        private static void HandleCreateCharacter()
        {
            Globals.WaitingOnServer = false;
            GameFade.FadeIn();
            Gui.MenuUi.MainMenu.NotifyOpenCharacterCreation();
        }

        private static void HandleOpenAdminWindow()
        {
            Gui.GameUi.NotifyOpenAdminWindow();
        }

        private static void HandleCastTime(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var entityId = bf.ReadGuid();
            var spellId = bf.ReadGuid();
            if (SpellBase.Get(spellId) != null && Globals.Entities.ContainsKey(entityId))
            {
                Globals.Entities[entityId].CastTime = Globals.System.GetTimeMs() +
                                                       SpellBase.Get(spellId).CastDuration;
                Globals.Entities[entityId].SpellCast = spellId;
            }
            bf.Dispose();
        }

        private static void HandleSpellCooldown(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int spellSlot = bf.ReadInteger();
			decimal cooldownReduction = (1 - (decimal)(Globals.Me.GetCooldownReduction() / 100));
			if (SpellBase.Get(Globals.Me.Spells[spellSlot].SpellId) != null)
            {
                Globals.Me.Spells[spellSlot].SpellCd = Globals.System.GetTimeMs() +
                                                       (int)(SpellBase.Lookup
                                                            .Get<SpellBase>(Globals.Me.Spells[spellSlot].SpellId)
                                                            .CooldownDuration * cooldownReduction);
            }
            bf.Dispose();
        }

        private static void HandleItemCooldown(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Guid itemId = bf.ReadGuid();
            var item = ItemBase.Get(itemId);
            if (item != null)
            {
                decimal cooldownReduction = (1 - (decimal)(Globals.Me.GetCooldownReduction() / 100));
                if (Globals.Me.ItemCooldowns.ContainsKey(itemId))
                {
                    Globals.Me.ItemCooldowns[itemId] = Globals.System.GetTimeMs() + (long)(item.Cooldown * cooldownReduction);
                }
                else
                {
                    Globals.Me.ItemCooldowns.Add(itemId, Globals.System.GetTimeMs() + (long)(item.Cooldown * cooldownReduction));
                }
            }
            bf.Dispose();
        }

        private static void HandleExperience(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            if (Globals.Me != null)
            {
                Globals.Me.Experience = bf.ReadLong();
                Globals.Me.ExperienceToNextLevel = bf.ReadLong();
            }
            bf.Dispose();
        }

        private static void HandleProjectileSpawnDead(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var entityId = bf.ReadGuid();
            if (Globals.Entities.ContainsKey(entityId) &&
                Globals.Entities[entityId].GetType() == typeof(Projectile))
            {
                ((Projectile) Globals.Entities[entityId]).SpawnDead((int) bf.ReadLong());
            }
            bf.Dispose();
        }

        private static void HandlePlayAnimation(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapId = Guid.Empty;
            var animId = bf.ReadGuid();
            int targetType = bf.ReadInteger();
            var entityId = bf.ReadGuid();
            if (targetType == -1)
            {
                mapId = bf.ReadGuid();
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    map.AddTileAnimation(animId, bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger());
                }
            }
            else if (targetType == 1)
            {
                bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
                int dir = bf.ReadInteger();
                if (Globals.Entities.ContainsKey(entityId))
                {
                    if (Globals.Entities[entityId] != null && !Globals.EntitiesToDispose.Contains(entityId))
                    {
                        var animBase = AnimationBase.Get(animId);
                        if (animBase != null)
                        {
                            AnimationInstance animInstance = new AnimationInstance(animBase, false,
                                dir == -1 ? false : true,-1,Globals.Entities[entityId]);
                            if (dir > -1) animInstance.SetDir(dir);
                            Globals.Entities[entityId].Animations.Add(animInstance);
                        }
                    }
                }
            }
            else if (targetType == 2)
            {
                mapId = bf.ReadGuid();
                bf.ReadInteger();
                bf.ReadInteger();
                int dir = bf.ReadInteger();
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    if (map.LocalEntities.ContainsKey(entityId))
                    {
                        if (map.LocalEntities[entityId] != null)
                        {
                            var animBase = AnimationBase.Get(animId);
                            if (animBase != null)
                            {
                                AnimationInstance animInstance = new AnimationInstance(animBase, false,
                                    dir == -1 ? true : false,-1,map.LocalEntities[entityId]);
                                if (dir > -1) animInstance.SetDir(dir);
                                map.LocalEntities[entityId].Animations.Add(animInstance);
                            }
                        }
                    }
                }
            }
            bf.Dispose();
        }

        private static void HandleHoldPlayer(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var eventId = bf.ReadGuid();
            var mapId = bf.ReadGuid();
            if (!Globals.EventHolds.ContainsKey(eventId)) Globals.EventHolds.Add(eventId, mapId);
            bf.Dispose();
        }

        private static void HandleReleasePlayer(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Guid eventId = bf.ReadGuid();
            if (Globals.EventHolds.ContainsKey(eventId))
            {
                Globals.EventHolds.Remove(eventId);
            }
            bf.Dispose();
        }

        private static void HandlePlayMusic(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string bgm = bf.ReadString();
            GameAudio.PlayMusic(bgm, 1f, 1f, true);
            bf.Dispose();
        }

        private static void HandleFadeMusic(byte[] packet)
        {
            GameAudio.StopMusic(3f);
        }

        private static void HandlePlaySound(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string sound = bf.ReadString();
            GameAudio.AddMapSound(sound, -1, -1, Globals.Me.CurrentMap, false, 5);
            bf.Dispose();
        }

        private static void HandleStopSounds(byte[] packet)
        {
            GameAudio.StopAllSounds();
        }

        private static void HandleShowPicture(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string picture = bf.ReadString();
            int size = bf.ReadInteger();
            bool clickable = bf.ReadBoolean();
            Gui.GameUi.ShowPicture(picture, size, clickable);
            bf.Dispose();
        }

        private static void HandleHidePicture(byte[] packet)
        {
            Gui.GameUi.HidePicture();
        }

        private static void HandleOpenShop(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.GameShop = new ShopBase();
            Globals.GameShop.Load(bf.ReadString());
            Gui.GameUi.NotifyOpenShop();
        }

        private static void HandleCloseShop(byte[] packet)
        {
            Globals.GameShop = null;
            Gui.GameUi.NotifyCloseShop();
        }

        private static void HandleOpenCraftingTable(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.ActiveCraftingTable = new CraftingTableBase();
            Globals.ActiveCraftingTable.Load(bf.ReadString());
            Gui.GameUi.NotifyOpenCraftingTable();
        }

        private static void HandleCloseCraftingTable(byte[] packet)
        {
            Gui.GameUi.NotifyCloseCraftingTable();
        }

        private static void HandleOpenBank(byte[] packet)
        {
            Gui.GameUi.NotifyOpenBank();
        }

        private static void HandleCloseBank(byte[] packet)
        {
            Gui.GameUi.NotifyCloseBank();
        }

        private static void HandleBankUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int slot = bf.ReadInteger();
            int active = bf.ReadInteger();
            if (active == 0)
            {
                Globals.Bank[slot] = null;
            }
            else
            {
                Globals.Bank[slot] = new ItemInstance();
                Globals.Bank[slot].Load(bf);
            }
            bf.Dispose();
        }

        private static void HandleGameObject(byte[] packet)
        {
            using (var bf = new ByteBuffer())
            {
                bf.WriteBytes(packet);
                var type = (GameObjectType) bf.ReadInteger();
                var id = bf.ReadGuid();
                var another = Convert.ToBoolean(bf.ReadInteger());
                var deleted = Convert.ToBoolean(bf.ReadInteger());
                var json = bf.ReadString();

                switch (type)
                {
                    case GameObjectType.Map:
                        //Handled in a different packet
                        break;
                    case GameObjectType.Tileset:
                        var obj = new TilesetBase(id);
                        obj.Load(json);
                        TilesetBase.Lookup.Set(id, obj);
                        if (Globals.HasGameData && !another)
                            Globals.ContentManager.LoadTilesets(TilesetBase.GetNameList());
                        break;
                    case GameObjectType.Event:
                        //Clients don't store event data, im an idiot.
                        break;
                    default:
                        var lookup = type.GetLookup();
                        if (deleted) lookup.Get(id).Delete();
                        else
                        {
                            lookup.DeleteAt(id);
                            var item = lookup.AddNew(type.GetObjectType(), id);
                            item.Load(json);
                        }
                        break;
                }
            }
        }

        private static void HandleEntityDash(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var endMapId = bf.ReadGuid();
            var endX = bf.ReadInteger();
            var endY = bf.ReadInteger();
            var dashTime = bf.ReadInteger();
            var direction = bf.ReadInteger();
            if (Globals.Entities.ContainsKey(id))
                Globals.Entities[id].DashQueue.Enqueue(new DashInstance(Globals.Entities[id], endMapId, endX, endY,
                    dashTime, direction));
            bf.Dispose();
        }

        private static void HandleMapGrid(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MapGridWidth = bf.ReadLong();
            Globals.MapGridHeight = bf.ReadLong();
            var clearKnownMaps = bf.ReadBoolean();
            Globals.MapGrid = new Guid[Globals.MapGridWidth, Globals.MapGridHeight];
            if (clearKnownMaps)
            {
                foreach (var map in MapInstance.Lookup.Values.ToArray())
                {
                    ((MapInstance) map).Dispose();
                }
            }
            Globals.NeedsMaps = true;
            Globals.GridMaps.Clear();
            for (int x = 0; x < Globals.MapGridWidth; x++)
            {
                for (int y = 0; y < Globals.MapGridHeight; y++)
                {
                    Globals.MapGrid[x, y] = bf.ReadGuid();
                    if (Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        Globals.GridMaps.Add(Globals.MapGrid[x, y]);
                        if (MapInstance.MapRequests.ContainsKey(Globals.MapGrid[x, y]))
                        {
                            MapInstance.MapRequests[Globals.MapGrid[x, y]] = Globals.System.GetTimeMs() + 2000;
                        }
                        else
                        {
                            MapInstance.MapRequests.Add(Globals.MapGrid[x, y], Globals.System.GetTimeMs() + 2000);
                        }
                    }
                }
            }
            if (Globals.Me != null) Globals.Me.FetchNewMaps();
            GameGraphics.GridSwitched = true;
            bf.Dispose();
        }

        private static void HandleTime(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            DateTime time = DateTime.FromBinary(bf.ReadLong());
            float rate = (float) bf.ReadDouble();
            Framework.GenericClasses.Color clr = Framework.GenericClasses.Color.FromArgb(bf.ReadByte(), bf.ReadByte(), bf.ReadByte(), bf.ReadByte());
            ClientTime.LoadTime(time, clr, rate);
        }

        private static void HandleParty(byte[] packet)
        {
            if (Globals.Me == null || Globals.Me.Party == null) return;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int count = bf.ReadInteger();

            Globals.Me.Party.Clear();
            for (int i = 0; i < count; i++)
            {
                Globals.Me.Party.Add(new PartyMember(bf));
            }

            bf.Dispose();
        }

        private static void HandlePartyUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int index = bf.ReadInteger();
            if (index < Globals.Me.Party.Count)
            {
                Globals.Me.Party[index] = new PartyMember(bf);
            }
            bf.Dispose();
        }

        private static void HandlePartyInvite(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string leader = bf.ReadString();
            Guid leaderId = bf.ReadGuid();
            InputBox iBox = new InputBox(Strings.Parties.partyinvite,
                Strings.Parties.inviteprompt.ToString( leader), true, InputBox.InputType.YesNo,
                PacketSender.SendPartyAccept,
                PacketSender.SendPartyDecline, leaderId);
            bf.Dispose();
        }

        private static void HandleChatBubble(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            var type = bf.ReadInteger();
            var mapId = bf.ReadGuid();
            Entity en = null;
            if (type < (int) EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }
                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }
                en = entityMap.LocalEntities[id];
            }
            if (en == null)
            {
                return;
            }
            en.AddChatBubble(bf.ReadString());
            bf.Dispose();
        }

        private static void HandleQuestOffer(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var questId = bf.ReadGuid();
            if (!Globals.QuestOffers.Contains(questId))
            {
                Globals.QuestOffers.Add(questId);
            }
            bf.Dispose();
        }

        private static void HandleQuestProgress(byte[] packet)
        {
            if (Globals.Me != null)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                var count = bf.ReadInteger();
                for (int i = 0; i < count; i++)
                {
                    var id = bf.ReadGuid();
                    if (bf.ReadByte() == 0)
                    {
                        if (Globals.Me.QuestProgress.ContainsKey(id))
                        {
                            Globals.Me.QuestProgress.Remove(id);
                        }
                    }
                    else
                    {
                        QuestProgressStruct questProgress = new QuestProgressStruct()
                        {
                            Completed = bf.ReadBoolean(),
                            TaskId = bf.ReadGuid(),
                            TaskProgress = bf.ReadInteger()
                        };
                        if (Globals.Me.QuestProgress.ContainsKey(id))
                        {
                            Globals.Me.QuestProgress[id] = questProgress;
                        }
                        else
                        {
                            Globals.Me.QuestProgress.Add(id, questProgress);
                        }
                    }
                }
                if (Gui.GameUi != null)
                {
                    Gui.GameUi.NotifyQuestsUpdated();
                }
                bf.Dispose();
            }
        }

        private static void HandleTradeStart(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var traderId = bf.ReadGuid();

            Globals.Trade = new ItemInstance[2, Options.MaxInvItems];
            //Gotta initialize the trade values
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < Options.MaxInvItems; y++)
                {
                    Globals.Trade[x, y] = new ItemInstance();
                }
            }
            Gui.GameUi.NotifyOpenTrading(traderId);
            bf.Dispose();
        }

        private static void HandleTradeUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            int i = 0;

            if (id != Globals.Me.Id)
            {
                i = 1;
            }

            int slot = bf.ReadInteger();
            int active = bf.ReadInteger();
            if (active == 0)
            {
                Globals.Trade[i, slot] = null;
            }
            else
            {
                Globals.Trade[i, slot] = new ItemInstance();
                Globals.Trade[i, slot].Load(bf);
            }
            bf.Dispose();
        }

        private static void HandleTradeClose(byte[] packet)
        {
            Gui.GameUi.NotifyCloseTrading();
        }

        private static void HandleTradeRequest(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string partner = bf.ReadString();
            Guid partnerId = bf.ReadGuid();
            InputBox iBox = new InputBox(Strings.Trading.traderequest,
                Strings.Trading.requestprompt.ToString( partner), true, InputBox.InputType.YesNo,
                PacketSender.SendTradeRequestAccept,
                PacketSender.SendTradeRequestDecline, partnerId);
            bf.Dispose();
        }

        private static void HandleNpcAggression(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            if (Globals.Entities.ContainsKey(id))
            {
                Globals.Entities[id].Type = bf.ReadInteger();
            }
            bf.Dispose();
        }

        private static void HandlePlayerDash(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            if (Globals.Entities.ContainsKey(id))
            {
                //Clear all dashes.
                Globals.Entities[id].DashQueue.Clear();
                Globals.Entities[id].Dashing = null;
                Globals.Entities[id].DashTimer = 0;
            }
            bf.Dispose();
        }

        private static void HandleEntityZDimension(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var id = bf.ReadGuid();
            if (Globals.Entities.ContainsKey(id))
            {
                Globals.Entities[id].CurrentZ = bf.ReadInteger();
            }
            bf.Dispose();
        }

        private static void HandleOpenBag(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slots = bf.ReadInteger();
            Globals.Bag = new ItemInstance[slots];
            Gui.GameUi.NotifyOpenBag();
            bf.Dispose();
        }

        private static void HandleCloseBag(byte[] packet)
        {
            Gui.GameUi.NotifyCloseBag();
        }

        private static void HandleBagUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int slot = bf.ReadInteger();
            int active = bf.ReadInteger();
            if (active == 0)
            {
                Globals.Bag[slot] = null;
            }
            else
            {
                Globals.Bag[slot] = new ItemInstance();
                Globals.Bag[slot].Load(bf);
            }
            bf.Dispose();
        }

        private static void HandleMoveRouteToggle(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MoveRouteActive = bf.ReadBoolean();
            bf.Dispose();
        }

        private static void HandleFriends(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Me.Friends.Clear();

            //Online friends
            int count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                FriendInstance f = new FriendInstance();
                f.Name = bf.ReadString();
                f.Map = bf.ReadString();
                f.Online = true;
                Globals.Me.Friends.Add(f);
            }

            //Offline friends
            count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                FriendInstance f = new FriendInstance();
                f.Name = bf.ReadString();
                f.Map = "Offline";
                f.Online = false;
                Globals.Me.Friends.Add(f);
            }

            Gui.GameUi.UpdateFriendsList();

            bf.Dispose();
        }

        private static void HandleFriendRequest(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string partner = bf.ReadString();
            Guid partnerId = bf.ReadGuid();
            InputBox iBox = new InputBox(Strings.Friends.request,
                Strings.Friends.requestprompt.ToString( partner), true, InputBox.InputType.YesNo,
                PacketSender.SendFriendRequestAccept,
                PacketSender.SendFriendRequestDecline, partnerId);
            bf.Dispose();
        }

        private static void HandlePlayerCharacters(byte[] packet)
        {
            List<Character> characters = new List<Character>();
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);

            int charCount = bf.ReadInteger();
            bool freeSlot = bf.ReadBoolean();

            for (int i = 0; i < charCount; i++)
            {
                var id = bf.ReadGuid();
                characters.Add(new Character(id, bf.ReadString(), bf.ReadString(), bf.ReadString(), bf.ReadInteger(), bf.ReadString()));
                for (int x = 0; x < Options.EquipmentSlots.Count + 1; x++)
                {
                    characters[characters.Count - 1].Equipment[x] = bf.ReadString();
                }
            }

            if (freeSlot) characters.Add(null);

            bf.Dispose();
            Globals.WaitingOnServer = false;
            GameFade.FadeIn();
            Gui.MenuUi.MainMenu.NotifyOpenCharacterSelection(characters);
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