/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.UI;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Maps.MapList;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Networking
{
    public static class PacketHandler
    {
        public static Dictionary<ServerPackets, int> dict = new Dictionary<ServerPackets, int>();
        public static void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (ServerPackets)bf.ReadLong();
            lock (Globals.GameLock)
            {
                if (dict.ContainsKey(packetHeader))
                {
                    dict[packetHeader]++;
                }
                else
                {
                    dict.Add(packetHeader, 1);
                }

                //Globals.System.Log("Handling Packet: " + packetHeader);
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
                    case ServerPackets.SendSpellCooldown:
                        HandleSpellCooldown(bf.ReadBytes(bf.Length()));
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
                    case ServerPackets.GameObject:
                        HandleGameObject(bf.ReadBytes(bf.Length()));
                        break;
                    case ServerPackets.EntityDash:
                        HandleEntityDash(bf.ReadBytes(bf.Length()));
                        break;
                    default:
                        Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                        break;
                }
            }
        }

        private static void HandleServerConfig(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Options.LoadFromServer(bf);
            GameGraphics.InitInGame();
        }

        private static void HandleJoinGame(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.MyIndex = (int)bf.ReadLong();
            GameMain.JoinGame();
            Globals.JoiningGame = true;
        }

        private static void HandleMapData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = (int)bf.ReadLong();
            var mapLength = bf.ReadLong();
            var mapData = bf.ReadBytes((int)mapLength);
            var revision = bf.ReadInteger();
            if (MapInstance.GetMap(mapNum) != null)
            {
                if (revision == MapInstance.GetMap(mapNum).Revision)
                {
                    return;
                }
                else
                {
                    MapInstance.GetMap(mapNum).Dispose(false, false);
                }
            }
            var newMap = new MapInstance((int)mapNum);
            MapInstance.AddObject(mapNum, newMap);
            newMap.Load(mapData);
            if ((mapNum) == Globals.LocalMaps[4])
            {
                GameAudio.PlayMusic(newMap.Music, 3, 3, true);
            }
            newMap.MapGridX = bf.ReadInteger();
            newMap.MapGridY = bf.ReadInteger();
            newMap.HoldLeft = bf.ReadInteger();
            newMap.HoldRight = bf.ReadInteger();
            newMap.HoldUp = bf.ReadInteger();
            newMap.HoldDown = bf.ReadInteger();
        }

        private static void HandleEntityData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var i = (int)bf.ReadLong();
            var entityType = bf.ReadInteger();
            var mapNum = bf.ReadInteger(false);
            Entity en;
            switch (entityType)
            {
                case (int)EntityTypes.Player:
                    if (i == Globals.MyIndex)
                    {
                        en = EntityManager.AddPlayer(i);
                        en.Load(bf);
                    }
                    else
                    {
                        en = EntityManager.AddGlobalEntity(i);
                        en.Load(bf);
                    }
                    break;
                case (int)EntityTypes.GlobalEntity:
                    en = EntityManager.AddGlobalEntity(i);
                    en.Load(bf);
                    break;
                case (int)EntityTypes.Resource:
                    en = EntityManager.AddResource(i);
                    ((Resource)en).Load(bf);
                    break;
                case (int)EntityTypes.Projectile:
                    en = EntityManager.AddProjectile(i);
                    ((Projectile)en).Load(bf);
                    break;
                case (int)EntityTypes.Event:
                    en = EntityManager.AddLocalEvent(i, mapNum);
                    if (en != null)
                    {
                        ((Event)en).Load(bf);
                    }
                    break;
            }
        }

        private static void HandlePositionInfo(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var mapNum = bf.ReadInteger();
            Entity en;
            if (type != (int)EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                if (MapInstance.GetMap(mapNum) == null) return;
                if (!MapInstance.GetMap(mapNum).LocalEntities.ContainsKey(index)) { return; }
                en = MapInstance.GetMap(mapNum).LocalEntities[index];
            }
            en.CurrentMap = mapNum;
            en.CurrentX = bf.ReadInteger();
            en.CurrentY = bf.ReadInteger();
            en.Dir = bf.ReadInteger();
            en.Passable = bf.ReadInteger();
            en.HideName = bf.ReadInteger();

            if (en == Globals.Me && Globals.CurrentMap != en.CurrentMap)
            {
                Globals.CurrentMap = Globals.Entities[index].CurrentMap;
                Globals.NeedsMaps = true;
            }
        }

        private static void HandleLeave(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var map = bf.ReadInteger();
            if (index == Globals.MyIndex && type < (int)EntityTypes.Event) { return; }
            EntityManager.RemoveEntity(index, type, map);

        }

        private static void HandleMsg(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.ChatboxContent.Add(new KeyValuePair<string, Color>(bf.ReadString(), new Color((int)bf.ReadByte(), (int)bf.ReadByte(), (int)bf.ReadByte(), (int)bf.ReadByte())));

        }

        private static void HandleGameData(byte[] packet)
        {
            Globals.HasGameData = true;
        }

        private static void HandleEnterMap(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = (int)bf.ReadLong();
            if (Globals.CurrentMap != mapNum && Globals.CurrentMap != -1) return;
            for (var i = 0; i < 9; i++)
            {
                Globals.LocalMaps[i] = (int)bf.ReadLong();
                if (Globals.LocalMaps[i] <= -1) continue;
                if (MapInstance.GetMap(Globals.LocalMaps[i]) == null)
                {
                    PacketSender.SendNeedMap(Globals.LocalMaps[i]);
                }
                else
                {
                    if (i == 4)
                    {
                        GameAudio.PlayMusic(MapInstance.GetMap(Globals.LocalMaps[i]).Music, 3, 3, true);
                    }
                }
            }
        }

        private static void HandleMapList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            MapList.GetList().Load(bf, new Dictionary<int, Intersect_Library.GameObjects.Maps.MapBase>(), false);
            //If admin window is open update it
            bf.Dispose();
        }

        private static void HandleEntityMove(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var mapNum = bf.ReadInteger();
            Entity en;
            if (type < (int)EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                var gameMap = MapInstance.GetMap(mapNum);
                if (gameMap == null) return;
                if (!gameMap.LocalEntities.ContainsKey(index)) { return; }
                en = gameMap.LocalEntities[index];
            }
            if (en == null) { return; }
            var entityMap = MapInstance.GetMap(en.CurrentMap);
            if (entityMap == null) { return; }
            var map = mapNum;
            var x = bf.ReadInteger();
            var y = bf.ReadInteger();
            var dir = bf.ReadInteger();
            var correction = bf.ReadInteger();
            if ((en.CurrentMap != map || en.CurrentX != x || en.CurrentY != y) && (en != Globals.Me || (en == Globals.Me && correction == 1)))
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
            if (entityMap.Attributes[en.CurrentX, en.CurrentY] != null &&
                entityMap.Attributes[en.CurrentX, en.CurrentY].value == (int)MapAttributes.ZDimension)
            {
                if (entityMap.Attributes[en.CurrentX, en.CurrentY].data1 > 0)
                {
                    en.CurrentZ = entityMap.Attributes[en.CurrentX, en.CurrentY].data1 - 1;
                }
            }
        }

        private static void HandleVitals(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var mapNum = bf.ReadInteger();
            Entity en = null;
            if (type < (int)EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                var entityMap = MapInstance.GetMap(mapNum);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(index)) { return; }
                en = entityMap.LocalEntities[index];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                en.MaxVital[i] = bf.ReadInteger();
                en.Vital[i] = bf.ReadInteger();
            }

            //Update status effects
            var count = bf.ReadInteger();
            en.Status.Clear();
            for (int i = 0; i < count; i++)
            {
                en.Status.Add(new StatusInstance(bf.ReadInteger(), bf.ReadString()));
            }
        }

        private static void HandleStats(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var mapNum = bf.ReadInteger();
            Entity en = null;
            if (type < (int)EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                var entityMap = MapInstance.GetMap(mapNum);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(index)) { return; }
                en = entityMap.LocalEntities[index];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                en.Stat[i] = bf.ReadInteger();
            }
        }

        private static void HandleEntityDir(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var mapNum = bf.ReadInteger();
            Entity en = null;
            if (type < (int)EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                var entityMap = MapInstance.GetMap(mapNum);
                if (entityMap == null) return;
                if (!entityMap.LocalEntities.ContainsKey(index)) { return; }
                en = entityMap.LocalEntities[index];
            }
            if (en == null)
            {
                return;
            }
            en.Dir = bf.ReadInteger();
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
            ed.EventMap = bf.ReadInteger();
            ed.EventIndex = bf.ReadInteger();
            Globals.EventDialogs.Add(ed);
        }

        private static void HandleLoginError(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var error = bf.ReadString();
            GameFade.FadeIn();
            Globals.WaitingOnServer = false;
            Gui.MsgboxErrors.Add(error);
            Gui.MenuUI.Reset();
        }

        private static void HandleMapItems(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = bf.ReadInteger();
            var map = MapInstance.GetMap(mapNum);
            if (map == null) return;
            if (map != null)
            {
                map.MapItems.Clear();
                int itemCount = bf.ReadInteger();
                for (int i = 0; i < itemCount; i++)
                {
                    var item = new MapItemInstance();
                    item.Load(bf);
                    map.MapItems.Add(item);
                }
            }
            bf.Dispose();
        }

        private static void HandleMapItemUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = bf.ReadInteger();
            int index = bf.ReadInteger();
            var map = MapInstance.GetMap(mapNum);
            if (map != null)
            {
                if (bf.ReadInteger() == -1)
                {
                    if (map.MapItems.Count > index)
                    {
                        map.MapItems.RemoveAt(index);
                    }
                }
                else
                {
                    while (index >= map.MapItems.Count)
                    {
                        map.MapItems.Add(new MapItemInstance());
                    }
                    map.MapItems[index].Load(bf);
                }
            }
            bf.Dispose();
        }

        private static void HandleInventoryUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int slot = bf.ReadInteger();
            Globals.Me.Inventory[slot].Load(bf);
            bf.Dispose();
        }

        private static void HandleSpellUpdate(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int slot = bf.ReadInteger();
            Globals.Me.Spells[slot].Load(bf);
            bf.Dispose();
        }

        private static void HandlePlayerEquipment(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int entityIndex = bf.ReadInteger();
            if (Globals.Entities.ContainsKey(entityIndex))
            {
                for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    ((Player)Globals.Entities[entityIndex]).Equipment[i] = bf.ReadInteger();
                }
            }
            bf.Dispose();
        }

        private static void HandleStatPoints(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Me.StatPoints = bf.ReadInteger();
            bf.Dispose();
        }

        private static void HandleHotbarSlots(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Globals.Me.Hotbar[i].Type = bf.ReadInteger();
                Globals.Me.Hotbar[i].Slot = bf.ReadInteger();
            }
            bf.Dispose();
        }

        private static void HandleCreateCharacter()
        {
            Globals.WaitingOnServer = false;
            GameFade.FadeIn();
            Gui.MenuUI._mainMenu.NotifyOpenCharacterCreation();
        }

        private static void HandleOpenAdminWindow()
        {
            Gui.GameUI.NotifyOpenAdminWindow();
        }

        private static void HandleCastTime(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int EntityNum = bf.ReadInteger();
            int SpellNum = bf.ReadInteger();
            if (SpellBase.GetSpell(SpellNum) != null)
            {
                Globals.Entities[EntityNum].CastTime = Globals.System.GetTimeMS() +
                                                        SpellBase.GetSpell(SpellNum).CastDuration * 100;
                Globals.Entities[EntityNum].SpellCast = SpellNum;
            }
            bf.Dispose();
        }

        private static void HandleSpellCooldown(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int SpellSlot = bf.ReadInteger();
            if (SpellBase.GetSpell(Globals.Me.Spells[SpellSlot].SpellNum) != null)
            {
                Globals.Me.Spells[SpellSlot].SpellCD = Globals.System.GetTimeMS() +
                                                       (SpellBase.GetSpell(Globals.Me.Spells[SpellSlot].SpellNum).CooldownDuration * 100);
            }
            bf.Dispose();
        }

        private static void HandleExperience(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Me.Experience = bf.ReadInteger();
            Globals.Me.ExperienceToNextLevel = bf.ReadInteger();
            bf.Dispose();
        }

        private static void HandleProjectileSpawnDead(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int entityIndex = (int)bf.ReadLong();
            if (Globals.Entities.ContainsKey(entityIndex))
            {
                ((Projectile)Globals.Entities[entityIndex]).SpawnDead((int)bf.ReadLong());
            }
            bf.Dispose();
        }

        private static void HandlePlayAnimation(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int animNum = bf.ReadInteger();
            int targetType = bf.ReadInteger();
            int entityIndex = bf.ReadInteger();
            if (targetType == -1)
            {
                int mapNum = bf.ReadInteger();
                var map = MapInstance.GetMap(mapNum);
                if (map != null)
                {
                    map.AddTileAnimation(animNum, bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger());
                }
            }
            else if (targetType == 1)
            {
                bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
                int dir = bf.ReadInteger();
                if (Globals.Entities.ContainsKey(entityIndex))
                {
                    if (Globals.Entities[entityIndex] != null)
                    {
                        var animBase = AnimationBase.GetAnim(animNum);
                        if (animBase != null)
                        {
                            AnimationInstance animInstance = new AnimationInstance(animBase, false, dir == -1 ? true : false);
                            if (dir > -1) animInstance.SetDir(dir);
                            Globals.Entities[entityIndex].Animations.Add(animInstance);
                        }
                    }
                }
            }
            else if (targetType == 2)
            {
                int mapIndex = bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
                int dir = bf.ReadInteger();
                var map = MapInstance.GetMap(mapIndex);
                if (map != null)
                {
                    if (entityIndex >= 0 && entityIndex < map.LocalEntities.Count)
                    {
                        if (map.LocalEntities[entityIndex] != null)
                        {
                            var animBase = AnimationBase.GetAnim(animNum);
                            if (animBase != null)
                            {
                                AnimationInstance animInstance = new AnimationInstance(animBase, false, dir == -1 ? true : false);
                                if (dir > -1) animInstance.SetDir(dir);
                                map.LocalEntities[entityIndex].Animations.Add(animInstance);
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
            int mapNum = bf.ReadInteger();
            int eventIndex = bf.ReadInteger();
            for (int i = 0; i < Globals.EventHolds.Count; i++)
            {
                if (Globals.EventHolds[i].MapNum == mapNum && Globals.EventHolds[i].EventIndex == eventIndex)
                {
                    return; //Event already holding
                }
            }
            Globals.EventHolds.Add(new EventHold(mapNum, eventIndex));
            bf.Dispose();
        }

        private static void HandleReleasePlayer(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = bf.ReadInteger();
            int eventIndex = bf.ReadInteger();
            for (int i = 0; i < Globals.EventHolds.Count; i++)
            {
                if (Globals.EventHolds[i].MapNum == mapNum && Globals.EventHolds[i].EventIndex == eventIndex)
                {
                    Globals.EventHolds.RemoveAt(i);
                    break;
                }
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
            GameAudio.AddMapSound(sound, -1, -1, Globals.CurrentMap, false, -1);
            bf.Dispose();
        }

        private static void HandleStopSounds(byte[] packet)
        {
            GameAudio.StopAllSounds();
        }

        private static void HandleOpenShop(byte[] packet)
        {
            Globals.GameShop = new ShopBase(0);
            Globals.GameShop.Load(packet);
            Gui.GameUI.NotifyOpenShop();
        }

        private static void HandleCloseShop(byte[] packet)
        {
            Globals.GameShop = null;
            Gui.GameUI.NotifyCloseShop();
        }

        private static void HandleOpenBank(byte[] packet)
        {
            Gui.GameUI.NotifyOpenBank();
        }

        private static void HandleCloseBank(byte[] packet)
        {
            Gui.GameUI.NotifyCloseBank();
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
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
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
                case GameObject.Tileset:
                    var obj = new TilesetBase(id);
                    obj.Load(data);
                    TilesetBase.AddObject(id, obj);
                    if (Globals.HasGameData) Globals.ContentManager.LoadTilesets(DatabaseObject.GetGameObjectList(GameObject.Tileset));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            bf.Dispose();
        }

        private static void HandleEntityDash(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var range = bf.ReadInteger();
            Globals.Entities[index].Dashing = new DashInstance(index, range);
            bf.Dispose();
        }
    }
}