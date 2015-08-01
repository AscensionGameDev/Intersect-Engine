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
using Intersect_Client.Classes.Animations;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Spells;
using System;

namespace Intersect_Client.Classes
{
    public static class PacketHandler
    {
        public static void HandlePacket(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (Enums.ServerPackets)bf.ReadLong();
            lock (Globals.GameLock)
            {
                switch (packetHeader)
                {
                    case Enums.ServerPackets.RequestPing:
                        PacketSender.SendPing();
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
                    case Enums.ServerPackets.GameTime:
                        HandleGameTime(bf.ReadBytes(bf.Length()));
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
                    default:
                        Console.WriteLine(@"Non implemented packet received: " + packetHeader);
                        break;
                }
            }
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
            var mapNum = bf.ReadLong();
            var mapLength = bf.ReadLong();
            var mapData = bf.ReadBytes((int)mapLength);
            if (Globals.GameMaps[mapNum] != null) { Globals.GameMaps[mapNum].Dispose(); }
            Globals.GameMaps[mapNum] = new MapStruct((int)mapNum, mapData);
            if ((mapNum) == Globals.LocalMaps[4]) { 
                Sounds.PlayMusic(Globals.GameMaps[mapNum].Music, 3,3, true); }

        }

        private static void HandleEntityData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var i = (int)bf.ReadLong();
            var entityType = bf.ReadInteger();
            Entity en;
           
            if (entityType == 1)
            {
                 en = EntityManager.AddEvent(i);
                ((Event)en).Load(bf);
            }
            else if (entityType == 3)
            {
                en = EntityManager.AddResource(i);
                ((Resource)en).Load(bf);
            }
            else
            {
                if (i == Globals.MyIndex)
                {
                    en = EntityManager.AddPlayer(i);
                }
                else
                {
                    en = EntityManager.AddEntity(i);
                }
                en.Load(bf);
            }
        }

        private static void HandlePositionInfo(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            if (isEvent != 1)
            {
                if (index >= Globals.Entities.Count) { return; }
                if (Globals.Entities[index] == null) { return; }
                Globals.Entities[index].CurrentMap = bf.ReadInteger();
                Globals.Entities[index].CurrentX = bf.ReadInteger();
                Globals.Entities[index].CurrentY = bf.ReadInteger();
                Globals.Entities[index].Dir = bf.ReadInteger();
                Globals.Entities[index].Passable = bf.ReadInteger();
                Globals.Entities[index].HideName = bf.ReadInteger();
                if (index != Globals.MyIndex) return;
                if (Globals.CurrentMap == Globals.Entities[index].CurrentMap) return;
                Globals.CurrentMap = Globals.Entities[index].CurrentMap;
                //Initiate loading screen, we got probz
                Graphics.FadeStage = 2;
                Graphics.FadeAmt = 255.0f;
                Globals.GameLoaded = false;
                //Globals.LocalMaps[4] = -1;
            }
            else
            {
                if (index >= Globals.Events.Count) { return; }
                if (Globals.Events[index] == null) { return; }
                Globals.Events[index].CurrentMap = bf.ReadInteger();
                Globals.Events[index].CurrentX = bf.ReadInteger();
                Globals.Events[index].CurrentY = bf.ReadInteger();
                Globals.Events[index].Dir = bf.ReadInteger();
                Globals.Events[index].Passable = bf.ReadInteger();
                Globals.Events[index].HideName = bf.ReadInteger();
            }
        }

        private static void HandleLeave(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            EntityManager.RemoveEntity(index, isEvent);

        }

        private static void HandleMsg(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.ChatboxContent.Add(bf.ReadString());

        }

        private static void HandleGameData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapCount = (int)bf.ReadLong();
            Globals.GameMaps = new MapStruct[mapCount];
            Globals.MapCount = mapCount;

            //Load Each of the Main Game Object Types
            Globals.GameItems = new ItemStruct[Constants.MaxItems];
            Globals.GameNpcs = new NpcStruct[Constants.MaxNpcs];
            Globals.GameAnimations = new AnimationStruct[Constants.MaxAnimations];
            Globals.GameSpells = new SpellStruct[Constants.MaxSpells];
            Globals.GameResources = new ResourceStruct[Constants.MaxResources];
            Globals.GameClasses = new ClassStruct[Constants.MaxClasses];

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
                Graphics.LoadTilesets(Globals.Tilesets);
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
                if (Globals.GameMaps[Globals.LocalMaps[i]] == null)
                {
                    PacketSender.SendNeedMap(Globals.LocalMaps[i]);
                }
                else
                {
                    if (i == 4)
                    {
                        Sounds.PlayMusic(Globals.GameMaps[Globals.LocalMaps[i]].Music, 3,3, true);
                    }
                }
            }
            for (var i = 0; i < Globals.GameMaps.Length; i++)
            {
                if (Globals.GameMaps[i] == null) continue;
                if (Globals.GameMaps[i].CacheCleared) continue;
                for (var x = 0; x < 9; x++)
                {
                    if (Globals.LocalMaps[x] == i)
                    {
                        break;
                    }
                    if (x == 8)
                    {
                        //Globals.GameMaps[i].ClearCache();
                    }
                }
            }
        }

        private static void HandleEntityMove(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            if (isEvent != 1)
            {
                if (index >= Globals.Entities.Count) { return; }
                if (Globals.Entities[index] == null) { return; }
                if (Globals.GameMaps[Globals.Entities[index].CurrentMap] == null) { return; }
                int map = bf.ReadInteger();
                int x = bf.ReadInteger();
                int y = bf.ReadInteger();
                int dir = bf.ReadInteger();
                if (Globals.Entities[index].CurrentMap != map || Globals.Entities[index].CurrentX != x || Globals.Entities[index].CurrentY != y)
                {
                    Globals.Entities[index].CurrentMap = map;
                    Globals.Entities[index].CurrentX = x;
                    Globals.Entities[index].CurrentY = y;
                    Globals.Entities[index].Dir = dir;
                    Globals.Entities[index].IsMoving = true;

                    switch (Globals.Entities[index].Dir)
                    {
                        case 0:
                            Globals.Entities[index].OffsetY = Constants.TileWidth;
                            Globals.Entities[index].OffsetX = 0;
                            break;
                        case 1:
                            Globals.Entities[index].OffsetY = -Constants.TileWidth;
                            Globals.Entities[index].OffsetX = 0;
                            break;
                        case 2:
                            Globals.Entities[index].OffsetY = 0;
                            Globals.Entities[index].OffsetX = Constants.TileWidth;
                            break;
                        case 3:
                            Globals.Entities[index].OffsetY = 0;
                            Globals.Entities[index].OffsetX = -Constants.TileWidth;
                            break;
                    }
                }

                // Set the Z-Dimension if the player has moved up or down a dimension.
                if (Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].value == (int)Enums.MapAttributes.ZDimension)
                {
                    if (Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].data1 > 0)
                    {
                        Globals.Entities[index].CurrentZ = Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].data1 - 1;
                    }
                }
                
            }
            else
            {
                if (index >= Globals.Events.Count) { return; }
                if (Globals.Events[index] == null) { return; }
                Globals.Events[index].CurrentMap = bf.ReadInteger();
                Globals.Events[index].CurrentX = bf.ReadInteger();
                Globals.Events[index].CurrentY = bf.ReadInteger();
                Globals.Events[index].Dir = bf.ReadInteger();
                Globals.Events[index].IsMoving = true;
                switch (Globals.Events[index].Dir)
                {
                    case 0:
                        Globals.Events[index].OffsetY = Constants.TileWidth;
                        Globals.Events[index].OffsetX = 0;
                        break;
                    case 1:
                        Globals.Events[index].OffsetY = -Constants.TileWidth;
                        Globals.Events[index].OffsetX = 0;
                        break;
                    case 2:
                        Globals.Events[index].OffsetY = 0;
                        Globals.Events[index].OffsetX = Constants.TileWidth;
                        break;
                    case 3:
                        Globals.Events[index].OffsetY = 0;
                        Globals.Events[index].OffsetX = -Constants.TileWidth;
                        break;
                }
            }
        }

        private static void HandleVitals(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            if (isEvent != 1)
            {
                if (index >= Globals.Entities.Count) { return; }
                if (Globals.Entities[index] == null) { return; }
                for (var i = 0; i < (int) Enums.Vitals.VitalCount; i++)
                {
                    Globals.Entities[index].MaxVital[i] = bf.ReadInteger();
                    Globals.Entities[index].Vital[i] = bf.ReadInteger();
                }
            }
            else
            {
                if (index >= Globals.Events.Count) { return; }
                if (Globals.Events[index] == null) { return; }
                for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
                {
                    Globals.Events[index].MaxVital[i] = bf.ReadInteger();
                    Globals.Events[index].Vital[i] = bf.ReadInteger();
                }
            }
        }

        private static void HandleStats(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            if (isEvent != 1)
            {
                if (index >= Globals.Entities.Count) { return; }
                if (Globals.Entities[index] == null) { return; }
                for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
                {
                    Globals.Entities[index].Stat[i] = bf.ReadInteger();
                }
            }
            else
            {
                if (index >= Globals.Events.Count) { return; }
                if (Globals.Events[index] == null) { return; }
                for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
                {
                    Globals.Events[index].Stat[i] = bf.ReadInteger();
                }
            }
        }

        private static void HandleEntityDir(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = (int)bf.ReadLong();
            var isEvent = bf.ReadInteger();
            if (isEvent != 1)
            {
                if (index >= Globals.Entities.Count) { return; }
                if (Globals.Entities[index] == null) { return; }
                Globals.Entities[index].Dir = bf.ReadInteger();
            }
            else
            {
                if (index >= Globals.Events.Count) { return; }
                if (Globals.Events[index] == null) { return; }
                Globals.Events[index].Dir = bf.ReadInteger();
            }

        }

        private static void HandleEventDialog(byte[] packet)
        {
            var bf = new ByteBuffer();
            var ed = new EventDialog();
            bf.WriteBytes(packet);
            ed.Prompt = bf.ReadString();
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
            ed.EventIndex = bf.ReadInteger();
            Globals.EventDialogs.Add(ed);
        }

        private static void HandleLoginError(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var error = bf.ReadString();
            Graphics.FadeStage = 1;
            Graphics.FadeAmt = 250;
            Globals.WaitingOnServer = false;
            Gui.MsgboxErrors.Add(error);
            Gui._MenuGui.Reset();
        }

        private static void HandleGameTime(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.GameTime = bf.ReadInteger();
        }

        private static void HandleItemData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemnum = bf.ReadLong();
            Globals.GameItems[itemnum] = new ItemStruct();
            Globals.GameItems[itemnum].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleNpcData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var npcNum = bf.ReadInteger();
            Globals.GameNpcs[npcNum] = new NpcStruct();
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleSpellData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameSpells[index] = new SpellStruct();
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleAnimationData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameAnimations[index] = new AnimationStruct();
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleResourceData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var resourceNum = bf.ReadInteger();
            Globals.GameResources[resourceNum] = new ResourceStruct();
            Globals.GameResources[resourceNum].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleMapItems(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = bf.ReadInteger();
            if (Globals.GameMaps[mapNum] != null)
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
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++ )
            {
                ((Player)Globals.Entities[entityIndex]).Equipment[i] = bf.ReadInteger();
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
            for (int i = 0; i < Constants.MaxHotbar; i++)
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
                Globals.GameClasses = new ClassStruct[Constants.MaxClasses];
            }

            Globals.GameClasses[classNum] = new ClassStruct();
            Globals.GameClasses[classNum].Load(bf.ReadBytes(bf.Length()));
        }

        private static void HandleCreateCharacter()
        {
            Globals.WaitingOnServer = false;
            Graphics.FadeStage = 1;
            Gui._MenuGui._mainMenu.CreateCharacterCreation();
        }
    }
}