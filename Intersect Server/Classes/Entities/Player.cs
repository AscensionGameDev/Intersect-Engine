/*
    Intersect Game Engine (Server)
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;
namespace Intersect_Server.Classes.Entities
{

    public class Player : Entity
    {
        public long MyId = -1;
        public bool InGame;
        public Client MyClient;
        private bool _sentMap;
        public int StatPoints;
        public int Class = 0;
        public int Gender = 0;
        public int Level = 1;
        public int Experience;
        public int[] Equipment = new int[Options.EquipmentSlots.Count];
        public Dictionary<int, bool> Switches = new Dictionary<int, bool>();
        public Dictionary<int, int> Variables = new Dictionary<int, int>();
        public Dictionary<int,QuestProgressStruct> Quests = new Dictionary<int,QuestProgressStruct>();
        public List<EventInstance> MyEvents = new List<EventInstance>();
        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];
        public ItemInstance[] Bank = new ItemInstance[Options.MaxBankSlots];
        public List<int> QuestOffers = new List<int>();

        //Temporary Values
        private object EventLock = new object();
        public bool InBank;
        public int InShop = -1;
        public int InCraft = -1;
        public int CraftIndex = -1;
        public long CraftTimer = 0;
        public long SaveTimer = Environment.TickCount;
        public List<Player> Party = new List<Player>();

        //Init
        public Player(int index, Client newClient) : base(index)
        {
            MyClient = newClient;
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                Spells.Add(new SpellInstance());
            }
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Inventory.Add(new ItemInstance(-1, 0));
            }
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Equipment[i] = -1;
            }
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
        }

        //Update
        public override void Update()
        {
            if (!InGame || CurrentMap == -1) { return; }
            var _curMapLink = CurrentMap;

            if (SaveTimer + 120000 < Environment.TickCount)
            {
                Task.Run(() => Database.SaveCharacter(this, false));
                SaveTimer = Environment.TickCount;
            }

            if (InCraft > -1 && CraftIndex > -1)
            {
                BenchBase b = BenchBase.GetCraft(InCraft);
                if (CraftTimer + b.Crafts[CraftIndex].Time < Environment.TickCount)
                {
                    CraftItem(CraftIndex);
                }
            }

            base.Update();
            //If we switched maps, lets update the maps
            if (_curMapLink != CurrentMap)
            {
                if (_curMapLink != -1)
                {
                    MapInstance.GetMap(_curMapLink).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    if (!MapInstance.GetObjects().ContainsKey(CurrentMap))
                    {
                        WarpToSpawn(true);
                    }
                    else
                    {
                        MapInstance.GetMap(CurrentMap).PlayerEnteredMap(MyClient);
                    }
                }
            }

            var currentMap = MapInstance.GetMap(CurrentMap);
            for (var i = 0; i < currentMap.SurroundingMaps.Count + 1; i++)
            {
                MapInstance map = null;
                if (i == currentMap.SurroundingMaps.Count)
                {
                    map = currentMap;
                }
                else
                {
                    map = MapInstance.GetMap(currentMap.SurroundingMaps[i]);
                }
                if (map == null) continue;
                lock (map.GetMapLock())
                {
                    //Check to see if we can spawn events, if already spawned.. update them.
                    lock (EventLock)
                    {
                        foreach (var mapEvent in map.Events.Values)
                        {
                            //Look for event
                            var foundEvent = EventExists(map.MyMapNum, mapEvent.SpawnX, mapEvent.SpawnY);
                            if (foundEvent == -1)
                            {
                                var tmpEvent = new EventInstance(MyEvents.Count, MyClient, mapEvent, map.MyMapNum)
                                {
                                    IsGlobal = mapEvent.IsGlobal == 1,
                                    MapNum = map.MyMapNum,
                                    SpawnX = mapEvent.SpawnX,
                                    SpawnY = mapEvent.SpawnY
                                };
                                MyEvents.Add(tmpEvent);
                            }
                            else
                            {
                                MyEvents[foundEvent].Update();
                            }
                        }
                    }
                }
            }
            //Check to see if we can spawn events, if already spawned.. update them.
            lock (EventLock)
            {
                for (var i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] == null) continue;
                    var eventFound = false;
                    if (MyEvents[i].MapNum == -1)
                    {
                        MyEvents[i].Update();
                        if (MyEvents[i].CallStack.Count > 0)
                        {
                            eventFound = true;
                        }
                    }
                    if (MyEvents[i].MapNum != CurrentMap)
                    {
                        foreach (var t in MapInstance.GetMap(CurrentMap).SurroundingMaps)
                        {
                            if (t == MyEvents[i].MapNum)
                            {
                                eventFound = true;
                            }
                        }
                    }
                    else
                    {
                        eventFound = true;
                    }
                    if (eventFound) continue;
                    PacketSender.SendEntityLeaveTo(MyClient, i, (int)EntityTypes.Event, MyEvents[i].MapNum);
                    MyEvents[i] = null;
                }
            }
        }

        //Sending Data
        public override byte[] Data()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(Level);
            bf.WriteInteger(Gender);
            bf.WriteInteger(Class);
            return bf.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Player;
        }

        //Spawning/Dying
        private void Respawn()
        {
            var cls = ClassBase.GetClass(Class);
            if (cls != null)
            {
                Warp(cls.SpawnMap, cls.SpawnX, cls.SpawnY, cls.SpawnDir);
            }
            else
            {
                Warp(0, 0, 0, 0);
            }
            PacketSender.SendEntityDataToProximity(this);
        }
        public override void Die(bool dropitems = false)
        {
            base.Die(dropitems);
            Reset();
            Respawn();
        }

        //Vitals
        public void RestoreVital(Vitals vital)
        {
            Vital[(int)vital] = MaxVital[(int)vital];
            PacketSender.SendEntityVitals(this);
        }
        public void AddVital(Vitals vital, int amount)
        {
            Vital[(int)vital] += amount;
            if (Vital[(int)vital] < 0) Vital[(int)vital] = 0;
            if (Vital[(int)vital] > MaxVital[(int)vital]) Vital[(int)vital] = MaxVital[(int)vital];
        }
        public override void ProcessRegen()
        {
            var myclass = ClassBase.GetClass(Class);
            var vitalAdded = false;
            if (myclass != null)
            {
                foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                {
                    if ((int)vital < (int)Vitals.VitalCount && Vital[(int)vital] != MaxVital[(int)vital])
                    {
                        AddVital(vital, (int)((float)MaxVital[(int)vital] * (myclass.VitalRegen[(int)vital] / 100f)));
                        vitalAdded = true;
                    }
                }
            }
            if (vitalAdded)
            {
                PacketSender.SendEntityVitals(this);
            }
        }

        //Leveling
        public void SetLevel(int level, bool resetExperience = false)
        {
            if (level > 0)
            {
                Level = Math.Min(Options.MaxLevel, level);
                if (resetExperience) Experience = 0;
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendExperience(MyClient);

            }
        }
        public void LevelUp(bool resetExperience = true, int levels = 1)
        {
            if (Level < Options.MaxLevel)
            {
                for (int i = 0; i < levels; i++)
                {
                    SetLevel(Level + 1, resetExperience);
                    //Let's pull up class - leveling info
                    var myclass = ClassBase.GetClass(Class);
                    if (myclass != null)
                    {
                        foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                        {
                            if ((int)vital < (int)Vitals.VitalCount)
                            {
                                var maxVital = MaxVital[(int)vital];
                                if (myclass.IncreasePercentage == 1)
                                {
                                    maxVital = (int)(MaxVital[(int)vital] * (1f + ((float)myclass.VitalIncrease[(int)vital] / 100f)));
                                }
                                else
                                {
                                    maxVital = MaxVital[(int)vital] + myclass.VitalIncrease[(int)vital];
                                }
                                var vitalDiff = maxVital - MaxVital[(int)vital];
                                MaxVital[(int)vital] = maxVital;
                                AddVital(vital, vitalDiff);
                            }
                        }

                        foreach (Stats stat in Enum.GetValues(typeof(Stats)))
                        {
                            if ((int)stat < (int)Stats.StatCount)
                            {
                                var newStat = Stat[(int)stat].Stat;
                                if (myclass.IncreasePercentage == 1)
                                {
                                    newStat = (int)(Stat[(int)stat].Stat * (1f + ((float)myclass.StatIncrease[(int)stat] / 100f)));
                                }
                                else
                                {
                                    newStat = Stat[(int)stat].Stat + myclass.StatIncrease[(int)stat];
                                }
                                var statDiff = newStat - Stat[(int)stat].Stat;
                                AddStat(stat, statDiff);
                            }
                        }
                    }
                    StatPoints += myclass.PointIncrease;
                }
            }

            PacketSender.SendPlayerMsg(MyClient, "You have leveled up! You are now level " + Level + "!", Color.Cyan, MyName);
            PacketSender.SendActionMsg(MyIndex, "LEVEL UP!", new Color(255, 0, 255, 0));
            if (StatPoints > 0)
            {
                PacketSender.SendPlayerMsg(MyClient,
                    "You have " + StatPoints + " stat points available to be spent!", Color.Cyan, MyName);
            }
            PacketSender.SendExperience(MyClient);
            PacketSender.SendPointsTo(MyClient);
            PacketSender.SendEntityDataToProximity(this);

            //Search for login activated events and run them
            foreach (var evt in EventBase.GetObjects())
            {
                StartCommonEvent(evt.Value, (int)EventPage.CommonEventTriggers.LevelUp);
            }
        }
        public void GiveExperience(int amount)
        {
            Experience += amount;
            if (!CheckLevelUp())
            {
                PacketSender.SendExperience(MyClient);
            }
        }
        private bool CheckLevelUp()
        {
            int levelCount = 0;
            while (Experience >= GetExperienceToNextLevel() && GetExperienceToNextLevel() > 0)
            {
                Experience -= GetExperienceToNextLevel();
                levelCount++;
            }
            if (levelCount > 0)
            {
                LevelUp(false, levelCount);
                return true;
            }
            return false;
        }
        public int GetExperienceToNextLevel()
        {
            if (Level >= Options.MaxLevel) return -1;
            var myclass = ClassBase.GetClass(Class);
            if (myclass != null)
            {
                return (int)(myclass.BaseExp * Math.Pow(1 + (myclass.ExpIncrease / 100f) / 1, Level));
            }
            return 1000;
        }

        //Combat
        public override void KilledEntity(Entity en)
        {
            if (en.GetType() == typeof(Npc))
            {
                if (Party.Count > 0) //If in party, split the exp.
                {
                    for (int i = 0; i < Party.Count; i++)
                    {
                        Party[i].GiveExperience(((Npc)en).MyBase.Experience / Party.Count);
                    }
                }
                else
                {
                    GiveExperience(((Npc)en).MyBase.Experience);
                }
            }
        }

        //Warping
        public override void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, 1);
        }
        public override void Warp(int newMap, int newX, int newY, int newDir)
        {
            var map = MapInstance.GetMap(newMap);
            if (map == null)
            {
                Globals.GeneralLogs.Add("Failed to warp player to new map -- warping to /spawn/.");
                WarpToSpawn(true);
                return;
            }
            CurrentX = newX;
            CurrentY = newY;
            if (newMap != CurrentMap || _sentMap == false)
            {
                var oldMap = MapInstance.GetMap(CurrentMap);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Player, CurrentMap);
                CurrentMap = newMap;
                map.PlayerEnteredMap(MyClient);
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);
                PacketSender.SendMapGrid(MyClient, map.MapGrid);
                var surroundingMaps = map.GetSurroundingMaps(true);
                foreach (var surrMap in surroundingMaps)
                {
                    PacketSender.SendMap(MyClient, surrMap.MyMapNum);
                }
                _sentMap = true;
            }
            else
            {
                PacketSender.SendEntityPositionToAll(this);
                PacketSender.SendEntityVitals(this);
                PacketSender.SendEntityStats(this);
            }

        }
        //TODO: Only works if sendWarp is true? Need to look at/refactor this
        public void WarpToSpawn(bool sendWarp = false)
        {
            int map = -1, x = 0, y = 0;
            var cls = ClassBase.GetClass(Class);
            if (cls != null)
            {
                if (MapInstance.GetObjects().ContainsKey(cls.SpawnMap))
                {
                    map = cls.SpawnMap;
                }
                x = cls.SpawnX;
                y = cls.SpawnY;
            }
            if (map == -1)
            {
                var mapenum = MapInstance.GetObjects().GetEnumerator();
                mapenum.MoveNext();
                map = mapenum.Current.Value.MyMapNum;
            }
            if (sendWarp) { Warp(map, x, y); }
        }

        //Inventory
        public bool CanGiveItem(ItemInstance item)
        {
            var itemBase = ItemBase.GetItem(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                    //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    itemBase.ItemType == (int)ItemTypes.Currency ||
                    itemBase.ItemType == (int)ItemTypes.None ||
                    itemBase.ItemType == (int)ItemTypes.Spell)
                {
                    for (int i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TryGiveItem(ItemInstance item, bool SendUpdate = true)
        {
            var itemBase = ItemBase.GetItem(item.ItemNum);
            if (itemBase != null)
            {
                if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                    //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    itemBase.ItemType == (int)ItemTypes.Currency ||
                    itemBase.ItemType == (int)ItemTypes.None ||
                    itemBase.ItemType == (int)ItemTypes.Spell)
                {
                    for (int i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i].ItemNum == item.ItemNum)
                        {
                            Inventory[i].ItemVal += item.ItemVal;
                            if (SendUpdate)
                            {
                                PacketSender.SendInventoryItemUpdate(MyClient, i);
                            }
                            return true;
                        }
                    }
                }

                //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum == -1)
                    {
                        Inventory[i] = item.Clone();
                        if (SendUpdate)
                        {
                            PacketSender.SendInventoryItemUpdate(MyClient, i);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public void SwapItems(int item1, int item2)
        {
            ItemInstance tmpInstance = Inventory[item2].Clone();
            Inventory[item2] = Inventory[item1].Clone();
            Inventory[item1] = tmpInstance.Clone();
            PacketSender.SendInventoryItemUpdate(MyClient, item1);
            PacketSender.SendInventoryItemUpdate(MyClient, item2);
            EquipmentProcessItemSwap(item1, item2);
            HotbarProcessItemSwap(item1, item2);
        }
        public void DropItems(int slot, int amount)
        {
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.ItemType == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            itemBase.ItemType == (int)ItemTypes.Currency ||
                            itemBase.ItemType == (int)ItemTypes.None ||
                            itemBase.ItemType == (int)ItemTypes.Spell)
                {
                    if (amount >= Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Inventory[slot], amount);
                    if (amount == Inventory[slot].ItemVal)
                    {
                        Inventory[slot] = new ItemInstance(-1, 0);
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        Inventory[slot].ItemVal -= amount;
                    }
                }
                else
                {
                    MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Inventory[slot], 1);
                    Inventory[slot] = new ItemInstance(-1, 0);
                    EquipmentProcessItemLoss(slot);
                }
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }
        public void UseItem(int slot)
        {
            bool equipped = false;
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                //Check if caster does not have the correct combat stats, if not exit now.
                for (var n = 0; n < (int)Stats.StatCount; n++)
                {
                    if (Stat[n].Value() < itemBase.StatsReq[n])
                    {
                        PacketSender.SendPlayerMsg(MyClient, "You do not possess the correct combat stats to use this item.");
                        return;
                    }
                }

                //Check if the caster is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, "You cannot use this item whilst stunned.");
                        return;
                    }
                }

                if (Level < itemBase.LevelReq)
                {
                    PacketSender.SendPlayerMsg(MyClient, "You are not a high enough level to use this item.");
                    return;
                }

                if (itemBase.ClassReq > 0 && itemBase.ClassReq != Class)
                {
                    PacketSender.SendPlayerMsg(MyClient, "You do not meet the class requirement to use this item.");
                    return;
                }

                if (itemBase.GenderReq - 1 != -1 && itemBase.GenderReq - 1 != Gender)
                {
                    PacketSender.SendPlayerMsg(MyClient, "You do not meet the gender requirement to use this item.");
                    return;
                }

                switch (itemBase.ItemType)
                {
                    case (int)ItemTypes.None:
                    case (int)ItemTypes.Currency:
                        PacketSender.SendPlayerMsg(MyClient, "You cannot use this item!");
                        break;
                    case (int)ItemTypes.Equipment:
                        for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                        {
                            if (Equipment[i] == slot)
                            {
                                Equipment[i] = -1;
                                equipped = true;
                            }
                        }
                        if (!equipped)
                        {
                            if (itemBase.Data1 == Options.WeaponIndex)
                            {
                                if (Options.WeaponIndex > -1)
                                {
                                    //If we are equipping a 2hand weapon, remove the shield
                                    if (Convert.ToBoolean(itemBase.Data4))
                                    {
                                        Equipment[Options.ShieldIndex] = -1;
                                    }
                                    Equipment[Options.WeaponIndex] = slot;
                                }
                            }
                            else if (itemBase.Data1 == Options.ShieldIndex)
                            {
                                if (Options.ShieldIndex > -1)
                                {
                                    if (Equipment[Options.WeaponIndex] > -1)
                                    {
                                        //If we have a 2-hand weapon, remove it to equip this new shield
                                        if (ItemBase.GetItem(Inventory[Equipment[Options.WeaponIndex]].ItemNum) != null &&
                                            Convert.ToBoolean(ItemBase.GetItem(Inventory[Equipment[Options.WeaponIndex]].ItemNum).Data4))
                                        {
                                            Equipment[Options.WeaponIndex] = -1;
                                        }
                                    }
                                    Equipment[Options.ShieldIndex] = slot;
                                }
                            }
                            else
                            {
                                Equipment[itemBase.Data1] = slot;
                            }
                        }
                        PacketSender.SendPlayerEquipmentToProximity(this);
                        break;
                    case (int)ItemTypes.Spell:
                        if (itemBase.Data1 > -1)
                        {
                            if (TryTeachSpell(new SpellInstance(itemBase.Data1)))
                            {
                                TakeItem(slot, 1);
                            }
                        }
                        break;
                    case (int)ItemTypes.Event:
                        var evt = EventBase.GetEvent(itemBase.Data1);
                        if (evt != null)
                        {
                            if (StartCommonEvent(evt))
                            {
                                TakeItem(slot, 1);
                            }
                        }
                        break;
                    default:
                        PacketSender.SendPlayerMsg(MyClient, "Use of this item type is not yet implemented.");
                        break;
                }
            }

        }
        public bool TakeItem(int slot, int amount)
        {
            bool returnVal = false;
            if (slot < 0) { return false; }
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (itemBase.ItemType == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            itemBase.ItemType == (int)ItemTypes.Currency ||
                            itemBase.ItemType == (int)ItemTypes.None ||
                            itemBase.ItemType == (int)ItemTypes.Spell)
                {
                    if (amount > Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    else
                    {
                        if (amount == Inventory[slot].ItemVal)
                        {
                            Inventory[slot] = new ItemInstance(-1, 0);
                            EquipmentProcessItemLoss(slot);
                            returnVal = true;
                        }
                        else
                        {
                            Inventory[slot].ItemVal -= amount;
                            returnVal = true;
                        }
                    }
                }
                else
                {
                    Inventory[slot] = new ItemInstance(-1, 0);
                    EquipmentProcessItemLoss(slot);
                    returnVal = true;
                }
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
            return returnVal;
        }
        public int FindItem(int itemNum, int itemVal = 1)
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                if (Inventory[i].ItemNum == itemNum && Inventory[i].ItemVal >= itemVal)
                {
                    return i;
                }
            }
            return -1;
        }

        //Shop
        public bool OpenShop(int shopNum)
        {
            if (InShop > -1 || InBank || InCraft > -1) return false;
            InShop = shopNum;
            PacketSender.SendOpenShop(MyClient, shopNum);
            return true;
        }
        public void CloseShop()
        {
            if (InShop > -1)
            {
                InShop = -1;
                PacketSender.SendCloseShop(MyClient);
            }
        }
        public void SellItem(int slot, int amount)
        {
            bool canSellItem = true;
            int rewardItemNum = -1;
            int rewardItemVal = 0;
            int sellItemNum = Inventory[slot].ItemNum;
            if (InShop == -1) return;
            ShopBase shop = ShopBase.GetShop(InShop);
            if (shop != null)
            {
                var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
                if (itemBase != null)
                {
                    for (int i = 0; i < shop.BuyingItems.Count; i++)
                    {
                        if (shop.BuyingItems[i].ItemNum == sellItemNum)
                        {
                            if (!shop.BuyingWhitelist)
                            {
                                PacketSender.SendPlayerMsg(MyClient, "This shop does not accept that item!", Color.Red);
                                return;
                            }
                            else
                            {
                                rewardItemNum = shop.BuyingItems[i].CostItemNum;
                                rewardItemVal = shop.BuyingItems[i].CostItemVal;
                                break;
                            }
                        }
                    }
                    if (rewardItemNum == -1)
                    {
                        if (shop.BuyingWhitelist)
                        {
                            PacketSender.SendPlayerMsg(MyClient, "This shop does not accept that item!", Color.Red);
                            return;
                        }
                        else
                        {
                            rewardItemNum = shop.DefaultCurrency;
                            rewardItemVal = itemBase.Price;
                        }
                    }

                    if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                        //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                        itemBase.ItemType == (int)ItemTypes.Currency ||
                        itemBase.ItemType == (int)ItemTypes.None ||
                        itemBase.ItemType == (int)ItemTypes.Spell)
                    {
                        if (amount >= Inventory[slot].ItemVal)
                        {
                            amount = Inventory[slot].ItemVal;
                        }
                        if (amount == Inventory[slot].ItemVal)
                        {
                            //Definitely can get reward.
                            Inventory[slot] = new ItemInstance(-1, 0);
                            EquipmentProcessItemLoss(slot);
                        }
                        else
                        {
                            //check if can get reward
                            if (!CanGiveItem(new ItemInstance(rewardItemNum, rewardItemVal))) canSellItem = false;
                            Inventory[slot].ItemVal -= amount;

                        }
                    }
                    else
                    {
                        Inventory[slot] = new ItemInstance(-1, 0);
                        EquipmentProcessItemLoss(slot);
                    }
                    if (canSellItem)
                    {
                        TryGiveItem(new ItemInstance(rewardItemNum, rewardItemVal * amount), true);
                    }
                    PacketSender.SendInventoryItemUpdate(MyClient, slot);
                }
            }
        }
        public void BuyItem(int slot, int amount)
        {
            bool canSellItem = true;
            int buyItemNum = -1;
            int buyItemAmt = 1;
            if (InShop == -1) return;
            ShopBase shop = ShopBase.GetShop(InShop);
            if (shop != null)
            {
                if (slot >= 0 && slot < shop.SellingItems.Count)
                {
                    var itemBase = ItemBase.GetItem(shop.SellingItems[slot].ItemNum);
                    if (itemBase != null)
                    {
                        buyItemNum = shop.SellingItems[slot].ItemNum;
                        if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                            //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            itemBase.ItemType == (int)ItemTypes.Currency ||
                            itemBase.ItemType == (int)ItemTypes.None ||
                            itemBase.ItemType == (int)ItemTypes.Spell)
                        {
                            buyItemAmt = Math.Max(1, amount);
                        }
                        if (shop.SellingItems[slot].CostItemVal == 0 || FindItem(shop.SellingItems[slot].CostItemNum, shop.SellingItems[slot].CostItemVal * buyItemAmt) > -1)
                        {
                            if (CanGiveItem(new ItemInstance(buyItemNum, buyItemAmt)))
                            {
                                if (shop.SellingItems[slot].CostItemVal > 0)
                                {
                                    TakeItem(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                }
                                TryGiveItem(new ItemInstance(buyItemNum, buyItemAmt), true);
                            }
                            else
                            {
                                if (shop.SellingItems[slot].CostItemVal * buyItemAmt ==
                                    Inventory[
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt)].ItemVal)
                                {
                                    TakeItem(
                                        FindItem(shop.SellingItems[slot].CostItemNum,
                                            shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                        shop.SellingItems[slot].CostItemVal * buyItemAmt);
                                    TryGiveItem(new ItemInstance(buyItemNum, buyItemAmt), true);
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(MyClient, "You do not have space to purchase that item!",
                                        Color.Red, MyName);
                                }
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, "Transaction failed due to insufficent funds.",
                                Color.Red, MyName);
                        }
                    }
                }
            }
        }

        //Crafting
        public bool OpenCraftingBench(int index)
        {
            if (InShop > -1 || InBank || InCraft > -1) return false;
            InCraft = index;
            PacketSender.SendOpenCraftingBench(MyClient, index);
            return true;
        }
        public void CloseCraftingBench()
        {
            if (InCraft > -1 && CraftIndex == -1)
            {
                InCraft = -1;
                PacketSender.SendCloseCraftingBench(MyClient);
            }
        }

        //Craft a new item
        public void CraftItem(int index)
        {
            if (InCraft > -1)
            {
                //Check the player actually has the items
                foreach (CraftIngredient c in BenchBase.GetCraft(InCraft).Crafts[index].Ingredients)
                {
                    int n = FindItem(c.Item);
                    int x = 0;
                    if (n > -1)
                    {
                        x = Inventory[n].ItemVal;
                        if (x == 0) { x = 1; }
                    }
                    if (x < c.Quantity)
                    {
                        return;
                    }
                }

                //Take the items
                foreach (CraftIngredient c in BenchBase.GetCraft(InCraft).Crafts[index].Ingredients)
                {
                    int n = FindItem(c.Item);
                    if (n > -1)
                    {
                        TakeItem(n, c.Quantity);
                    }
                }

                //Give them the craft
                TryGiveItem(new ItemInstance(BenchBase.GetCraft(InCraft).Crafts[index].Item, 1));
                PacketSender.SendPlayerMsg(MyClient, "You successfully crafted " + ItemBase.GetName(BenchBase.GetCraft(InCraft).Crafts[index].Item) + "!", Color.Green);
                CraftIndex = -1;
            }
        }

        //Bank
        public bool OpenBank()
        {
            if (InShop > -1 || InBank || InCraft > -1) return false;
            InBank = true;
            PacketSender.SendOpenBank(MyClient);
            return true;
        }
        public void CloseBank()
        {
            if (InBank)
            {
                InBank = false;
                PacketSender.SendCloseBank(MyClient);
            }
        }
        public void DepositItem(int slot, int amount)
        {
            if (!InBank) return;
            var itemBase = ItemBase.GetItem(Inventory[slot].ItemNum);
            if (itemBase != null)
            {
                if (Inventory[slot].ItemNum > -1)
                {
                    if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                        //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                        itemBase.ItemType == (int)ItemTypes.Currency ||
                        itemBase.ItemType == (int)ItemTypes.None ||
                        itemBase.ItemType == (int)ItemTypes.Spell)
                    {
                        if (amount >= Inventory[slot].ItemVal)
                        {
                            amount = Inventory[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }
                    //Find a spot in the bank for it!
                    if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                        //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                        itemBase.ItemType == (int)ItemTypes.Currency ||
                        itemBase.ItemType == (int)ItemTypes.None ||
                        itemBase.ItemType == (int)ItemTypes.Spell)
                    {
                        for (int i = 0; i < Options.MaxBankSlots; i++)
                        {
                            if (Bank[i] != null && Bank[i].ItemNum == Inventory[slot].ItemNum)
                            {
                                Bank[i].ItemVal += amount;
                                //Remove Items from inventory send updates
                                if (amount >= Inventory[slot].ItemVal)
                                {
                                    Inventory[slot] = new ItemInstance(-1, 0);
                                    EquipmentProcessItemLoss(slot);
                                }
                                else
                                {
                                    Inventory[slot].ItemVal -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, slot);
                                PacketSender.SendBankUpdate(MyClient, i);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (int i = 0; i < Options.MaxBankSlots; i++)
                    {
                        if (Bank[i] == null || Bank[i].ItemNum == -1)
                        {
                            Bank[i] = new ItemInstance(0, 0);
                            Bank[i] = Inventory[slot].Clone();
                            Bank[i].ItemVal = amount;
                            //Remove Items from inventory send updates
                            if (amount >= Inventory[slot].ItemVal)
                            {
                                Inventory[slot] = new ItemInstance(-1, 0);
                                EquipmentProcessItemLoss(slot);
                            }
                            else
                            {
                                Inventory[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, slot);
                            PacketSender.SendBankUpdate(MyClient, i);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, "There is no space left in your bank for that item!", Color.Red);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, "Invalid item selected to deposit!", Color.Red);
                }
            }
        }
        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank) return;
            var itemBase = ItemBase.GetItem(Bank[slot].ItemNum);
            if (itemBase != null)
            {
                if (Bank[slot] != null && Bank[slot].ItemNum > -1)
                {
                    if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                        //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                        itemBase.ItemType == (int)ItemTypes.Currency ||
                        itemBase.ItemType == (int)ItemTypes.None ||
                        itemBase.ItemType == (int)ItemTypes.Spell)
                    {
                        if (amount >= Bank[slot].ItemVal)
                        {
                            amount = Bank[slot].ItemVal;
                        }
                    }
                    else
                    {
                        amount = 1;
                    }
                    //Find a spot in the inventory for it!
                    if (itemBase.ItemType == (int)ItemTypes.Consumable ||
                        //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                        itemBase.ItemType == (int)ItemTypes.Currency ||
                        itemBase.ItemType == (int)ItemTypes.None ||
                        itemBase.ItemType == (int)ItemTypes.Spell)
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (Inventory[i] != null && Inventory[i].ItemNum == Bank[slot].ItemNum)
                            {
                                Inventory[i].ItemVal += amount;
                                //Remove Items from bank send updates
                                if (amount >= Bank[slot].ItemVal)
                                {
                                    Bank[slot] = null;
                                }
                                else
                                {
                                    Bank[slot].ItemVal -= amount;
                                }
                                PacketSender.SendInventoryItemUpdate(MyClient, i);
                                PacketSender.SendBankUpdate(MyClient, slot);
                                return;
                            }
                        }
                    }

                    //Either a non stacking item, or we couldn't find the item already existing in the players inventory
                    for (int i = 0; i < Options.MaxInvItems; i++)
                    {
                        if (Inventory[i] == null || Inventory[i].ItemNum == -1)
                        {
                            Inventory[i] = new ItemInstance(0, 0);
                            Inventory[i] = Bank[slot].Clone();
                            //Remove Items from inventory send updates
                            if (amount >= Bank[slot].ItemVal)
                            {
                                Bank[slot] = null;
                            }
                            else
                            {
                                Bank[slot].ItemVal -= amount;
                            }
                            PacketSender.SendInventoryItemUpdate(MyClient, i);
                            PacketSender.SendBankUpdate(MyClient, slot);
                            return;
                        }
                    }
                    PacketSender.SendPlayerMsg(MyClient, "There is no space left in your inventory for that item!",
                        Color.Red);
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, "Invalid item selected to withdraw!", Color.Red);
                }
            }
        }
        public void SwapBankItems(int item1, int item2)
        {
            ItemInstance tmpInstance = null;
            if (Bank[item2] != null) tmpInstance = Bank[item2].Clone();
            if (Bank[item1] != null)
            {
                Bank[item2] = Bank[item1].Clone();
            }
            else
            {
                Bank[item2] = null;
            }
            if (tmpInstance != null)
            {
                Bank[item1] = tmpInstance.Clone();
            }
            else
            {
                Bank[item1] = null;
            }
            PacketSender.SendBankUpdate(MyClient, item1);
            PacketSender.SendBankUpdate(MyClient, item2);
        }

        //Parties
        public void AddParty(Player target)
        {
            //If a new party, make yourself the leader
            if (Party.Count == 0)
            {
                Party.Add(this);
            }
            else
            {
                if (Party[0] != this)
                {
                    PacketSender.SendPlayerMsg(MyClient, "Only the party leader can send invitations to your party.", Color.Red);
                    return;
                }

                //Check for member being already in the party, if so cancel
                for (int i = 0; i < Party.Count; i++)
                {
                    if (Party[i] == target)
                    {
                        return;
                    }
                }
            }

            if (Party.Count < 4)
            {
                Party.Add(target);

                //Update all members of the party with the new list
                for (int i = 0; i < Party.Count; i++)
                {
                    Party[i].Party = Party;
                    PacketSender.SendParty(Party[i].MyClient);
                    PacketSender.SendPlayerMsg(Party[i].MyClient, target.MyName + " has joined the party!", Color.Green);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, "You have reached the maximum limit of party members. Kick another member before adding more.", Color.Red);
            }
        }
        public void KickParty(int target)
        {
            if (Party.Count > 0 && Party[0] == this)
            {
                if (target > 0 && target < Party.Count)
                {
                    var oldMember = Party[target];
                    oldMember.Party = new List<Player>();
                    PacketSender.SendParty(oldMember.MyClient);
                    PacketSender.SendPlayerMsg(oldMember.MyClient, "You have been kicked from the party!", Color.Red);
                    Party.RemoveAt(target);

                    if (Party.Count > 1) //Need atleast 2 party members to function
                    {
                        //Update all members of the party with the new list
                        for (int i = 0; i < Party.Count; i++)
                        {
                            Party[i].Party = Party;
                            PacketSender.SendParty(Party[i].MyClient);
                            PacketSender.SendPlayerMsg(Party[i].MyClient, oldMember.MyName + " has been kicked from the party!", Color.Red);
                        }
                    }
                    else if (Party.Count > 0) //Check if anyone is left on their own
                    {
                        Player remainder = Party[0];
                        remainder.Party.Clear();
                        PacketSender.SendParty(remainder.MyClient);
                        PacketSender.SendPlayerMsg(remainder.MyClient, "The party has been disbanded.", Color.Red);
                    }
                }
            }
        }
        public void LeaveParty()
        {
            if (Party.Count > 0 && Party.Contains(this))
            {
                var oldMember = this;
                Party.Remove(this);

                if (Party.Count > 1) //Need atleast 2 party members to function
                {
                    //Update all members of the party with the new list
                    for (int i = 0; i < Party.Count; i++)
                    {
                        Party[i].Party = Party;
                        PacketSender.SendParty(Party[i].MyClient);
                        PacketSender.SendPlayerMsg(Party[i].MyClient, oldMember.MyName + " has left the party!", Color.Red);
                    }
                }
                else if (Party.Count > 0) //Check if anyone is left on their own
                {
                    Player remainder = Party[0];
                    remainder.Party.Clear();
                    PacketSender.SendParty(remainder.MyClient);
                    PacketSender.SendPlayerMsg(remainder.MyClient, "The party has been disbanded.", Color.Red);
                }
            }
            Party.Clear();
            PacketSender.SendParty(MyClient);
            PacketSender.SendPlayerMsg(MyClient, "You have left the party.", Color.Red);
        }
        public bool InParty(Player member)
        {
            for (int i = 0; i < Party.Count; i++)
            {
                if (member == Party[i])
                {
                    return true;
                }
            }
            return false;
        }

        //Spells
        public bool TryTeachSpell(SpellInstance spell, bool SendUpdate = true)
        {
            if (KnowsSpell(spell.SpellNum)) { return false; }
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum <= 0)
                {
                    Spells[i] = spell.Clone();
                    if (SendUpdate)
                    {
                        PacketSender.SendPlayerSpellUpdate(MyClient, i);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool KnowsSpell(int spellnum)
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellnum) { return true; }
            }
            return false;
        }
        public int FindSpell(int spellNum)
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellNum) { return i; }
            }
            return -1;
        }
        public void SwapSpells(int spell1, int spell2)
        {
            SpellInstance tmpInstance = Spells[spell2].Clone();
            Spells[spell2] = Spells[spell1].Clone();
            Spells[spell1] = tmpInstance.Clone();
            PacketSender.SendPlayerSpellUpdate(MyClient, spell1);
            PacketSender.SendPlayerSpellUpdate(MyClient, spell2);
            HotbarProcessSpellSwap(spell1, spell2);
        }
        public void ForgetSpell(int spellSlot)
        {
            Spells[spellSlot] = new SpellInstance();
            PacketSender.SendPlayerSpellUpdate(MyClient, spellSlot);
        }
        public void UseSpell(int spellSlot, int target)
        {
            int spellNum = Spells[spellSlot].SpellNum;
            Target = target;
            if (SpellBase.Get(spellNum) != null)
            {
                var spell = SpellBase.GetSpell(spellNum);
                //Check if caster does not have the correct combat stats, if not exit now.
                for (var n = 0; n < (int)Stats.StatCount; n++)
                {
                    if (Stat[n].Value() < spell.StatReq[n])
                    {
                        PacketSender.SendPlayerMsg(MyClient, "You do not possess the correct combat stats to use this ability.");
                        return;
                    }
                }

                //Check if the caster is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Silence)
                    {
                        PacketSender.SendPlayerMsg(MyClient, "You cannot cast this ability whilst silenced.");
                        return;
                    }
                    if (Status[n].Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(MyClient, "You cannot cast this ability whilst stunned.");
                        return;
                    }
                }

                if (Level < spell.LevelReq)
                {
                    PacketSender.SendPlayerMsg(MyClient, "You are not a high enough level to use this ability.");
                    return;
                }

                if (target == -1 && ((spell.SpellType == (int)SpellTypes.CombatSpell && spell.TargetType == (int)SpellTargetTypes.Single) || spell.SpellType == (int)SpellTypes.WarpTo))
                {
                    PacketSender.SendActionMsg(MyIndex, "No Target!", new Color(255, 255, 0, 0));
                    return;
                }

                if (spell.VitalCost[(int)Vitals.Mana] <= Vital[(int)Vitals.Mana])
                {
                    if (spell.VitalCost[(int)Vitals.Health] <= Vital[(int)Vitals.Health])
                    {
                        if (Spells[spellSlot].SpellCD < Globals.System.GetTimeMs())
                        {
                            if (CastTime < Globals.System.GetTimeMs())
                            {
                                Vital[(int)Vitals.Mana] = Vital[(int)Vitals.Mana] - spell.VitalCost[(int)Vitals.Mana];
                                Vital[(int)Vitals.Health] = Vital[(int)Vitals.Health] - spell.VitalCost[(int)Vitals.Health];
                                CastTime = Globals.System.GetTimeMs() + (spell.CastDuration * 100);
                                SpellCastSlot = spellSlot;

                                if (spell.CastAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spell.CastAnimation, 1, MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
                                }

                                PacketSender.SendEntityVitals(this);
                                PacketSender.SendEntityVitals(this);
                                PacketSender.SendEntityCastTime(this, spellNum);
                            }
                            else
                            {
                                PacketSender.SendPlayerMsg(MyClient, "You are currently channeling another skill.");
                            }
                        }
                        else
                        {
                            PacketSender.SendPlayerMsg(MyClient, "This skill is on cooldown.");
                        }
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(MyClient, "Not enough Health.");
                    }
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, "Not enough mana.");
                }
            }
        }
        public override void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            var spellBase = SpellBase.GetSpell(SpellNum);
            if (spellBase != null)
            {
                if (spellBase.SpellType == (int)SpellTypes.Event)
                {
                    var evt = EventBase.GetEvent(spellBase.Data1);
                    if (evt != null)
                    {
                        StartCommonEvent(evt);
                    }
                }
                else
                {
                    base.CastSpell(SpellNum, SpellSlot);
                }
            }
        }

        //Equipment
        public void UnequipItem(int slot)
        {
            Equipment[slot] = -1;
            PacketSender.SendPlayerEquipmentToProximity(this);
        }
        public void EquipmentProcessItemSwap(int item1, int item2)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == item1)
                    Equipment[i] = item2;
                else if (Equipment[i] == item2)
                    Equipment[i] = item1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
        }
        public void EquipmentProcessItemLoss(int slot)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                    Equipment[i] = -1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
        }

        //Stats
        public void UpgradeStat(int statIndex)
        {
            if (Stat[statIndex].Stat < Options.MaxStatValue)
            {
                Stat[statIndex].Stat++;
                StatPoints--;
                PacketSender.SendEntityStats(this);
                PacketSender.SendPointsTo(MyClient);
            }
        }
        public void AddStat(Stats stat, int amount)
        {
            Stat[(int)stat].Stat += amount;
            if (Stat[(int)stat].Stat < 0) Stat[(int)stat].Stat = 0;
            if (Stat[(int)stat].Stat > Options.MaxStatValue) Stat[(int)stat].Stat = Options.MaxStatValue;
        }

        //Hotbar
        public void HotbarChange(int index, int type, int slot)
        {
            Hotbar[index].Type = type;
            Hotbar[index].Slot = slot;
        }
        public void HotbarProcessItemSwap(int item1, int item2)
        {
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item1)
                    Hotbar[i].Slot = item2;
                else if (Hotbar[i].Type == 0 && Hotbar[i].Slot == item2)
                    Hotbar[i].Slot = item1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }
        public void HotbarProcessSpellSwap(int spell1, int spell2)
        {
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell1)
                    Hotbar[i].Slot = spell2;
                else if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell2)
                    Hotbar[i].Slot = spell1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }

        //Quests
        public bool CanStartQuest(QuestBase quest)
        {
            //Check and see if the quest is already in progress, or if it has already been completed and cannot be repeated.
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].task != -1 && quest.GetTaskIndex(Quests[quest.GetId()].task) != -1)
                {
                    return false;
                }
                if (Quests[quest.GetId()].completed == 1 && quest.Repeatable == 0)
                {
                    return false;
                }
            }
            //So the quest isn't started or we can repeat it.. let's make sure that we meet requirements.
            foreach (var requirement in quest.Requirements)
            {
                if (!EventInstance.MeetsConditions(requirement, this, null))
                {
                    return false;
                }
            }
            if (quest.Tasks.Count == 0)
            {
                return false;
            }
            return true;
        }
        public bool QuestCompleted(QuestBase quest)
        {
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].completed == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool QuestInProgress(QuestBase quest, QuestProgress progress, int taskId)
        {
            if (Quests.ContainsKey(quest.GetId()))
            {
                if (Quests[quest.GetId()].task != -1 && quest.GetTaskIndex(Quests[quest.GetId()].task) != -1)
                {
                    switch (progress)
                    {
                        case QuestProgress.OnAnyTask:
                            return true;
                        case QuestProgress.BeforeTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) < quest.GetTaskIndex(Quests[quest.GetId()].task);
                            }
                            break;
                        case QuestProgress.OnTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) == quest.GetTaskIndex(Quests[quest.GetId()].task);
                            }
                            break;
                        case QuestProgress.AfterTask:
                            if (quest.GetTaskIndex(taskId) != -1)
                            {
                                return quest.GetTaskIndex(taskId) > quest.GetTaskIndex(Quests[quest.GetId()].task);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(progress), progress, null);
                    }
                }
            }
            return false;
        }
        public void OfferQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                QuestOffers.Add(quest.GetId());
                PacketSender.SendQuestOffer(this, quest.GetId());
            }
        }
        public void StartQuest(QuestBase quest)
        {
            if (CanStartQuest(quest))
            {
                if (Quests.ContainsKey(quest.GetId()))
                {
                    var questProgress = Quests[quest.GetId()];
                    questProgress.task = quest.Tasks[0].Id;
                    questProgress.taskProgress = 0;
                    Quests[quest.GetId()] = questProgress;
                }
                else
                {
                    var questProgress = new QuestProgressStruct();
                    questProgress.task = quest.Tasks[0].Id;
                    questProgress.taskProgress = 0;
                    Quests.Add(quest.GetId(), questProgress);
                }
                PacketSender.SendQuestProgress(this, quest.GetId());
            }
        }
        public void AcceptQuest(int questId)
        {
            if (QuestOffers.Contains(questId))
            {
                QuestOffers.Remove(questId);
                var quest = QuestBase.GetQuest(questId);
                if (quest != null)
                {
                    StartQuest(quest);
                }
            }
        }
        public void DeclineQuest(int questId)
        {
            if (QuestOffers.Contains(questId))
            {
                QuestOffers.Remove(questId);
            }
        }
        public void CancelQuest(int questId)
        {
            var quest = QuestBase.GetQuest(questId);
            if (quest != null)
            {
                if (QuestInProgress(quest,QuestProgress.OnAnyTask,-1))
                {
                    //Cancel the quest somehow...
                    if (quest.Quitable == 1)
                    {
                        var questProgress = Quests[questId];
                        questProgress.task = -1;
                        questProgress.taskProgress = -1;
                        Quests[questId] = questProgress;
                        PacketSender.SendQuestProgress(this, questId);
                    }
                }
            }
        }
        public void CompleteQuestTask(int questId, int taskId)
        {
            var quest = QuestBase.GetQuest(questId);
            if (quest != null)
            {
                if (Quests.ContainsKey(questId))
                {
                    var questProgress = Quests[questId];
                    if (Quests[questId].task == taskId)
                    {
                        //Let's Advance this task or complete the quest
                        for (int i = 0; i < quest.Tasks.Count; i++)
                        {
                            if (quest.Tasks[i].Id == taskId)
                            {
                                if (i == quest.Tasks.Count - 1)
                                {
                                    //Complete Quest
                                    questProgress.completed = 1;
                                    questProgress.task = -1;
                                    questProgress.taskProgress = -1;
                                }
                                else
                                {
                                    //Advance Task
                                    questProgress.task = quest.Tasks[i + 1].Id;
                                    questProgress.taskProgress = 0;
                                }
                            }
                        }
                    }
                    Quests[questId] = questProgress;
                    PacketSender.SendQuestProgress(this, questId);
                }
            }

        }


        //Event Processing Methods
        private int EventExists(int map, int x, int y)
        {
            for (var i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] == null) continue;
                if (map == MyEvents[i].MapNum && x == MyEvents[i].SpawnX && y == MyEvents[i].SpawnY)
                {
                    return i;
                }
            }
            return -1;
        }

        public EventPageInstance EventAt(int map, int x, int y, int z)
        {
            foreach (var evt in MyEvents)
            {
                if (evt != null && evt.PageInstance != null)
                {
                    if (evt.PageInstance.CurrentMap == map && evt.PageInstance.CurrentX == x && evt.PageInstance.CurrentY == y && evt.PageInstance.CurrentZ == z)
                    {
                        return evt.PageInstance;
                    }
                }
            }
            return null;
        }

        public void TryActivateEvent(int mapNum, int eventIndex)
        {
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    if (MyEvents[i].MapNum == mapNum && MyEvents[i].BaseEvent.MyIndex == eventIndex)
                    {
                        if (MyEvents[i].PageInstance == null) return;
                        if (MyEvents[i].PageInstance.Trigger != 0) return;
                        if (!IsEventOneBlockAway(i)) return;
                        if (MyEvents[i].CallStack.Count != 0) return;
                        var newStack = new CommandInstance(MyEvents[i].PageInstance.MyPage) {CommandIndex = 0, ListIndex = 0};
                        MyEvents[i].CallStack.Push(newStack);
                        if (!MyEvents[i].IsGlobal)
                        {
                            MyEvents[i].PageInstance.TurnTowardsPlayer();
                        }
                        else
                        {
                            //Turn the global event opposite of the player
                            switch (Dir)
                            {
                                case 0:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(1);
                                    break;
                                case 1:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(0);
                                    break;
                                case 2:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(3);
                                    break;
                                case 3:
                                    MyEvents[i].PageInstance.GlobalClone.ChangeDir(2);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void RespondToEvent(int mapNum, int eventIndex, int responseId)
        {
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] != null && MyEvents[i].MapNum == mapNum && MyEvents[i].BaseEvent.MyIndex == eventIndex)
                    {
                        if (MyEvents[i].CallStack.Count <= 0) return;
                        if (MyEvents[i].CallStack.Peek().WaitingForResponse != CommandInstance.EventResponse.Dialogue) return;
                        if (MyEvents[i].CallStack.Peek().ResponseType == 0)
                        {
                            MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                        }
                        else
                        {
                            var tmpStack = new CommandInstance(MyEvents[i].PageInstance.BaseEvent.MyPages[MyEvents[i].PageIndex]);
                            tmpStack.CommandIndex = 0;
                            tmpStack.ListIndex = MyEvents[i].PageInstance.BaseEvent.MyPages[MyEvents[i].PageIndex].CommandLists[MyEvents[i].CallStack.Peek().ListIndex].Commands[MyEvents[i].CallStack.Peek().CommandIndex].Ints[responseId - 1];
                            MyEvents[i].CallStack.Peek().CommandIndex++;
                            MyEvents[i].CallStack.Peek().WaitingForResponse = CommandInstance.EventResponse.None;
                            MyEvents[i].CallStack.Push(tmpStack);
                        }
                        return;
                    }
                }
            }
        }

        static bool IsEventOneBlockAway(int eventIndex)
        {
            return true;
        }

        public int FindEvent(EventPageInstance en)
        {
            int id = -1;
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] != null && MyEvents[i].PageInstance != null && (MyEvents[i].PageInstance == en || MyEvents[i].PageInstance.GlobalClone == en))
                    {
                        id = i;
                        return id;
                    }
                }
            }
            return id;
        }

        public EventInstance GetEventFromPageInstance(EventPageInstance instance)
        {
            if (FindEvent(instance) > -1)
            {
                return MyEvents[FindEvent(instance)];
            }
            else
            {
                return null;
            }
        }

        public void SendEvents()
        {
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null && MyEvents[i].PageInstance != null)
                {
                    MyEvents[i].PageInstance.SendToClient();
                }
            }
        }

        public bool StartCommonEvent(EventBase evt, int trigger = -1)
        {
            lock (EventLock)
            {
                for (int i = 0; i < MyEvents.Count; i++)
                {
                    if (MyEvents[i] != null && MyEvents[i].BaseEvent == evt) return false;
                }
                var tmpEvent = new EventInstance(MyEvents.Count, MyClient, evt, -1)
                {
                    MapNum = -1, SpawnX = -1, SpawnY = -1
                };
                MyEvents.Add(tmpEvent);
                tmpEvent.Update();
                if (tmpEvent.PageInstance != null && (trigger == -1 || tmpEvent.PageInstance.MyPage.Trigger == trigger))
                {
                    var newStack = new CommandInstance(tmpEvent.PageInstance.MyPage) {CommandIndex = 0, ListIndex = 0};
                    tmpEvent.CallStack.Push(newStack);
                }
                else
                {
                    MyEvents.RemoveAt(MyEvents.Count - 1);
                }
                return true;
            }
        }

        public override int CanMove(int moveDir)
        {
            //If crafting or locked by event return blocked 
            if (InCraft > -1 && CraftIndex > -1)
            {
                return -5;
            }
            return base.CanMove(moveDir);
        }

        public override void Move(int moveDir, Client client, bool DontUpdate = false)
        {
            int index = MyIndex;
            int oldMap = CurrentMap;
            client = MyClient;
            base.Move(moveDir, client, DontUpdate);
            // Check for a warp, if so warp the player.
            var attribute = MapInstance.GetMap(Globals.Entities[index].CurrentMap).Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY];
            if (attribute != null && attribute.value == (int) MapAttributes.Warp)
            {
                Globals.Entities[index].Warp(attribute.data1, attribute.data2, attribute.data3, Globals.Entities[index].Dir);
            }

            //Check for slide tiles
            if (attribute != null && attribute.value == (int) MapAttributes.Slide)
            {
                if (attribute.data1 > 0)
                {
                    Globals.Entities[index].Dir = attribute.data1 - 1;
                } //If sets direction, set it.
                var dash = new DashInstance(this, 1, base.Dir);
            }

            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] != null)
                {
                    if (MyEvents[i].MapNum == CurrentMap)
                    {
                        if (MyEvents[i].PageInstance != null)
                        {
                            if (MyEvents[i].PageInstance.CurrentMap == CurrentMap && MyEvents[i].PageInstance.CurrentX == CurrentX && MyEvents[i].PageInstance.CurrentY == CurrentY && MyEvents[i].PageInstance.CurrentZ == CurrentZ)
                            {
                                if (MyEvents[i].PageInstance.Trigger != 1) return;
                                if (MyEvents[i].CallStack.Count != 0) return;
                                var newStack = new CommandInstance(MyEvents[i].PageInstance.MyPage)
                                {
                                    CommandIndex = 0, ListIndex = 0
                                };
                                MyEvents[i].CallStack.Push(newStack);
                            }
                        }
                    }
                }
            }
        }
    }

    public class HotbarInstance
    {
        public int Type = -1;
        public int Slot = -1;
    }
}

