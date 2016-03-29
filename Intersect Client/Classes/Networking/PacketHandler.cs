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
using System.CodeDom;
using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Sys;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.Game_Objects;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.UI;

namespace Intersect_Client.Classes.Networking
{
    public static class PacketHandler
    {
        public static Dictionary<Enums.ServerPackets, int> dict = new Dictionary<Enums.ServerPackets,int>();  
        public static void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (Enums.ServerPackets)bf.ReadLong();
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
                    case Enums.ServerPackets.RequestPing:
                        PacketSender.SendPing();
                        break;
                    case Enums.ServerPackets.ServerConfig:
                        HandleServerConfig(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.JoinGame:
                        HandleJoinGame(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.MapData:
                        HandleMapData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityData:
                        HandleEntityData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityPosition:
                        HandlePositionInfo(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityLeave:
                        HandleLeave(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ChatMessage:
                        HandleMsg(bf.ReadBytes(bf.Length()));
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
                    case Enums.ServerPackets.EntityMove:
                        HandleEntityMove(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityVitals:
                        HandleVitals(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityStats:
                        HandleStats(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EntityDir:
                        HandleEntityDir(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.EventDialog:
                        HandleEventDialog(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.LoginError:
                        HandleLoginError(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ItemData:
                        HandleItemData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.NpcData:
                        HandleNpcData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.SpellData:
                        HandleSpellData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.AnimationData:
                        HandleAnimationData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ResourceData:
                        HandleResourceData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.MapItems:
                        HandleMapItems(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.MapItemUpdate:
                        HandleMapItemUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.InventoryUpdate:
                        HandleInventoryUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.SpellUpdate:
                        HandleSpellUpdate(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.PlayerEquipment:
                        HandlePlayerEquipment(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.StatPoints:
                        HandleStatPoints(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.HotbarSlots:
                        HandleHotbarSlots(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ClassData:
                        HandleClassData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.CreateCharacter:
                        HandleCreateCharacter();
                        break;
                    case Enums.ServerPackets.QuestData:
                        HandleQuestData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.OpenAdminWindow:
                        HandleOpenAdminWindow();
                        break;
                    case Enums.ServerPackets.ProjectileData:
                        HandleProjectileData(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.CastTime:
                        HandleCastTime(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.SendSpellCooldown:
                        HandleSpellCooldown(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.Experience:
                        HandleExperience(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ProjectileSpawnDead:
                        HandleProjectileSpawnDead(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.SendPlayAnimation:
                        HandlePlayAnimation(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.HoldPlayer:
                        HandleHoldPlayer(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.ReleasePlayer:
                        HandleReleasePlayer(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.PlayMusic:
                        HandlePlayMusic(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.FadeMusic:
                        HandleFadeMusic(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.PlaySound:
                        HandlePlaySound(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.StopSounds:
                        HandleStopSounds(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.OpenShop:
                        HandleOpenShop(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.CloseShop:
                        HandleCloseShop(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.OpenBank:
                        HandleOpenBank(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.CloseBank:
                        HandleCloseBank(bf.ReadBytes(bf.Length()));
                        break;
                    case Enums.ServerPackets.BankUpdate:
                        HandleBankUpdate(bf.ReadBytes(bf.Length()));
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
            Options.LoadServerConfig(bf);
            //Load Each of the Main Game Object Types
            Globals.GameItems = new ItemStruct[Options.MaxItems];
            Globals.GameNpcs = new NpcStruct[Options.MaxNpcs];
            Globals.GameAnimations = new AnimationStruct[Options.MaxAnimations];
            Globals.GameSpells = new SpellStruct[Options.MaxSpells];
            Globals.GameResources = new ResourceStruct[Options.MaxResources];
            Globals.GameClasses = new ClassStruct[Options.MaxClasses];
            Globals.GameQuests = new QuestStruct[Options.MaxQuests];
            Globals.GameProjectiles = new ProjectileStruct[Options.MaxProjectiles];
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
            if (Globals.GameMaps.ContainsKey(mapNum))
            {
                if (revision == Globals.GameMaps[mapNum].Revision)
                {
                    return;
                }
                else
                {
                    Globals.GameMaps[mapNum].Dispose(false,false);
                }
            }
            Globals.GameMaps.Add(mapNum, new MapStruct((int)mapNum, mapData));
            if ((mapNum) == Globals.LocalMaps[4]) { 
                GameAudio.PlayMusic(Globals.GameMaps[mapNum].Music, 3,3, true); }
            Globals.GameMaps[mapNum].MapGridX = bf.ReadInteger();
            Globals.GameMaps[mapNum].MapGridY = bf.ReadInteger();
            Globals.GameMaps[mapNum].HoldLeft = bf.ReadInteger();
            Globals.GameMaps[mapNum].HoldRight = bf.ReadInteger();
            Globals.GameMaps[mapNum].HoldUp = bf.ReadInteger();
            Globals.GameMaps[mapNum].HoldDown = bf.ReadInteger();
            Globals.System.Log("Loaded map " + mapNum);
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
                case (int)Enums.EntityTypes.Player:
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
                case (int)Enums.EntityTypes.GlobalEntity:
                    en = EntityManager.AddGlobalEntity(i);
                    en.Load(bf);
                    break;
                case (int)Enums.EntityTypes.Resource:
                    en = EntityManager.AddResource(i);
                    ((Resource)en).Load(bf);
                    break;
                case (int)Enums.EntityTypes.Projectile:
                    en = EntityManager.AddProjectile(i);
                    ((Projectile)en).Load(bf);
                    break;
                case (int)Enums.EntityTypes.Event:
                    en = EntityManager.AddLocalEvent(i,mapNum);
                    if (en != null)
                    {
                        ((Event) en).Load(bf);
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
            if (type != (int)Enums.EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                if (!Globals.GameMaps.ContainsKey(mapNum)) return;
                if (!Globals.GameMaps[mapNum].LocalEntities.ContainsKey(index)) { return; }
                en = Globals.GameMaps[mapNum].LocalEntities[index];
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
            }
        }

        private static void HandleLeave(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var type = bf.ReadInteger();
            var map = bf.ReadInteger();
            if (index == Globals.MyIndex && type < (int)Enums.EntityTypes.Event) { return; }
            EntityManager.RemoveEntity(index, type, map);

        }

        private static void HandleMsg(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.ChatboxContent.Add(new KeyValuePair<string, Color>(bf.ReadString(),new Color((int)bf.ReadByte(),(int)bf.ReadByte(),(int)bf.ReadByte(),(int)bf.ReadByte())));

        }

        private static void HandleGameData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapCount = (int)bf.ReadLong();
            Globals.MapCount = mapCount;
            var BorderMode = bf.ReadInteger();

            if (BorderMode == 1)
            {
                Globals.GameBorderStyle = 1;
                //TODO Make sure window isn't too large.
                //GameGraphics.FixResolution();
            }
            //Database.LoadMapRevisions();
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
                if (!Globals.GameMaps.ContainsKey(Globals.LocalMaps[i]))
                {
                    PacketSender.SendNeedMap(Globals.LocalMaps[i]);
                }
                else
                {
                    if (i == 4)
                    {
                        GameAudio.PlayMusic(Globals.GameMaps[Globals.LocalMaps[i]].Music, 3,3, true);
                    }
                }
            }
            Globals.System.Log("Got enter map packet");
        }

        private static void HandleMapList(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.OrderedMaps.Clear();
            Globals.MapStructure.Load(bf);
            Globals.OrderedMaps.Sort();
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
            if (type < (int)Enums.EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)){return;}
                en = Globals.Entities[index];
            }
            else
            {
                if (!Globals.GameMaps.ContainsKey(mapNum)) return;
                if (!Globals.GameMaps[mapNum].LocalEntities.ContainsKey(index)) { return; }
                en = Globals.GameMaps[mapNum].LocalEntities[index];
            }
            if (en == null) {return;}
            if (!Globals.GameMaps.ContainsKey(en.CurrentMap)) { return; }
            var map = mapNum;
            var x = bf.ReadInteger();
            var y = bf.ReadInteger();
            var dir = bf.ReadInteger();
            var correction = bf.ReadInteger();
            if ((en.CurrentMap != map || en.CurrentX != x || en.CurrentY != y) && (en != Globals.Me || (en == Globals.Me && correction ==1)))
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
            if (Globals.GameMaps[en.CurrentMap].Attributes[en.CurrentX, en.CurrentY].value == (int)Enums.MapAttributes.ZDimension)
            {
                if (Globals.GameMaps[en.CurrentMap].Attributes[en.CurrentX, en.CurrentY].data1 > 0)
                {
                    en.CurrentZ = Globals.GameMaps[en.CurrentMap].Attributes[en.CurrentX, en.CurrentY].data1 - 1;
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
            if (type < (int)Enums.EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                if(!Globals.GameMaps.ContainsKey(mapNum)) return;
                if (!Globals.GameMaps[mapNum].LocalEntities.ContainsKey(index)) { return; }
                en = Globals.GameMaps[mapNum].LocalEntities[index];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                en.MaxVital[i] = bf.ReadInteger();
                en.Vital[i] = bf.ReadInteger();
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
            if (type < (int)Enums.EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                if (!Globals.GameMaps.ContainsKey(mapNum)) return;
                if (!Globals.GameMaps[mapNum].LocalEntities.ContainsKey(index)) { return; }
                en = Globals.GameMaps[mapNum].LocalEntities[index];
            }
            if (en == null)
            {
                return;
            }
            for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
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
            if (type < (int)Enums.EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(index)) { return; }
                en = Globals.Entities[index];
            }
            else
            {
                if (!Globals.GameMaps.ContainsKey(mapNum)) return;
                if (!Globals.GameMaps[mapNum].LocalEntities.ContainsKey(index)) { return; }
                en = Globals.GameMaps[mapNum].LocalEntities[index];
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

        private static void HandleItemData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadInteger();
            Globals.GameItems[itemNum] = new ItemStruct();
            Globals.GameItems[itemNum].Load(bf.ReadBytes(bf.Length()),itemNum);
        }

        private static void HandleNpcData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var npcNum = bf.ReadInteger();
            Globals.GameNpcs[npcNum] = new NpcStruct();
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()),npcNum);
        }

        private static void HandleSpellData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameSpells[index] = new SpellStruct();
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()),index);
        }

        private static void HandleAnimationData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameAnimations[index] = new AnimationStruct();
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()),index);
        }

        private static void HandleResourceData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var resourceNum = bf.ReadInteger();
            Globals.GameResources[resourceNum] = new ResourceStruct();
            Globals.GameResources[resourceNum].Load(bf.ReadBytes(bf.Length()),resourceNum);
        }

        private static void HandleMapItems(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = bf.ReadInteger();
            if (Globals.GameMaps.ContainsKey(mapNum) && Globals.GameMaps[mapNum] != null)
            {
                Globals.GameMaps[mapNum].MapItems.Clear();
                int itemCount = bf.ReadInteger();
                for (int i = 0; i < itemCount; i++)
                {
                    var item = new MapItemInstance();
                    item.Load(bf);
                    Globals.GameMaps[mapNum].MapItems.Add(item);
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
            if (Globals.GameMaps[mapNum] != null)
            {
                if (bf.ReadInteger() == -1)
                {
                    Globals.GameMaps[mapNum].MapItems.RemoveAt(index);
                }
                else
                {
                    while (index >= Globals.GameMaps[mapNum].MapItems.Count)
                    {
                        Globals.GameMaps[mapNum].MapItems.Add(new MapItemInstance());
                    }
                    Globals.GameMaps[mapNum].MapItems[index].Load(bf);
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
                    ((Player) Globals.Entities[entityIndex]).Equipment[i] = bf.ReadInteger();
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

        private static void HandleClassData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var classNum = bf.ReadInteger();

            if (classNum == 0) //Initilise the classes if the first one.
            {
                Globals.GameClasses = new ClassStruct[Options.MaxClasses];
            }

            Globals.GameClasses[classNum] = new ClassStruct();
            Globals.GameClasses[classNum].Load(bf.ReadBytes(bf.Length()),classNum);
            bf.Dispose();
        }

        private static void HandleCreateCharacter()
        {
            Globals.WaitingOnServer = false;
            GameFade.FadeIn();
            Gui.MenuUI._mainMenu.NotifyOpenCharacterCreation();
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

        private static void HandleOpenAdminWindow()
        {
            Gui.GameUI.NotifyOpenAdminWindow();
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

        private static void HandleCastTime(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int EntityNum = bf.ReadInteger();
            int SpellNum = bf.ReadInteger();
            Globals.Entities[EntityNum].CastTime = Globals.System.GetTimeMS() + Globals.GameSpells[SpellNum].CastDuration;
            Globals.Entities[EntityNum].SpellCast = SpellNum;
            bf.Dispose();
        }

        private static void HandleSpellCooldown(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int SpellSlot = bf.ReadInteger();
            Globals.Me.Spells[SpellSlot].SpellCD = Globals.System.GetTimeMS() + (Globals.GameSpells[Globals.Me.Spells[SpellSlot].SpellNum].CooldownDuration * 100);
            bf.Dispose();
        }

        private static void HandleExperience(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Me.Experience = bf.ReadInteger();
            bf.Dispose();
        }

        private static void HandleProjectileSpawnDead(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int entityIndex = (int)bf.ReadLong();
            if (Globals.Entities.ContainsKey(entityIndex))
            {
                ((Projectile) Globals.Entities[entityIndex]).SpawnDead((int)bf.ReadLong());
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
                if (Globals.GameMaps.ContainsKey(mapNum))
                {
                    Globals.GameMaps[mapNum].AddTileAnimation(animNum, bf.ReadInteger(), bf.ReadInteger(),
                        bf.ReadInteger());
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
                        AnimationInstance animInstance = new AnimationInstance(Globals.GameAnimations[animNum], false, dir == -1 ? true: false);
                        if (dir > -1) animInstance.SetDir(dir);
                        Globals.Entities[entityIndex].Animations.Add(animInstance);
                    }
                }
            }
            else if (targetType == 2)
            {
                int mapIndex = bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
                int dir = bf.ReadInteger();
                if (Globals.GameMaps.ContainsKey(mapIndex))
                {
                    if (entityIndex >= 0 && entityIndex < Globals.GameMaps[mapIndex].LocalEntities.Count)
                    {
                        if (Globals.GameMaps[mapIndex].LocalEntities[entityIndex] != null)
                        {
                            AnimationInstance animInstance = new AnimationInstance(Globals.GameAnimations[animNum], false, dir == -1 ? true : false);
                            if (dir > -1) animInstance.SetDir(dir);
                            Globals.GameMaps[mapIndex].LocalEntities[entityIndex].Animations.Add(animInstance);
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
            Globals.EventHolds.Add(new EventHold(mapNum,eventIndex));
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
            GameAudio.AddMapSound(sound, -1, -1, Globals.CurrentMap, true, -1);
            bf.Dispose();
        }

        private static void HandleStopSounds(byte[] packet)
        {
            GameAudio.StopAllSounds();
        }

        private static void HandleOpenShop(byte[] packet)
        {
            Globals.GameShop = new ShopStruct();
            Globals.GameShop.Load(packet,0);
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


    }
}