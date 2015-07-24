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

namespace Intersect_Server.Classes
{

	public class Player : Entity
	{
		public bool InGame;
        public Client MyClient;
        private bool _sentMap;
        public List<EventIndex> MyEvents = new List<EventIndex>();
        public bool[] Switches;
        public int[] Variables;
        public SpellInstance[] Spells = new SpellInstance[Constants.MaxPlayerSkills];
        public HotbarInstance[] Hotbar = new HotbarInstance[Constants.MaxHotbar];
        public int[] Equipment = new int[Enums.EquipmentSlots.Count];
        public int StatPoints = 0;
        public string MyAccount = "";
        public int Class = 0;
        public int Gender = 0;
        public int Level = 0;
        public int Experience = 0;

        //Temporary Values
        private int _curMapLink = -1;

        //Init
		public Player (int index, Client newClient) : base(index)
		{
            MyClient = newClient;
            Switches = new bool[Constants.SwitchCount];
            Variables = new int[Constants.VariableCount];
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
            {
                Spells[i] = new SpellInstance();
            }
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                Equipment[i] = -1;
            }
            for (int i = 0; i < Constants.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
		}

        //Update
        public void Update()
        {
            if (!InGame || CurrentMap == -1) { return; }
            //If we switched maps, lets update the maps
            if (_curMapLink != CurrentMap)
            {
                if (_curMapLink != -1)
                {
                    Globals.GameMaps[_curMapLink].RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    Globals.GameMaps[CurrentMap].AddEntity(this);
                }
                _curMapLink = CurrentMap;
            }
            //Check to see if we can spawn events, if already spawned.. update them.
            for (var i = 0; i < Globals.GameMaps[CurrentMap].SurroundingMaps.Count + 1; i++)
            {
                int mapNum;
                if (i == Globals.GameMaps[CurrentMap].SurroundingMaps.Count) { mapNum = CurrentMap; } else { mapNum = Globals.GameMaps[CurrentMap].SurroundingMaps[i]; }
                if (mapNum <= -1) continue;
                foreach (var t in Globals.GameMaps[mapNum].Events)
                {
                    int foundEvent;
                    if (CanSpawnEvent(t, mapNum))
                    {
                        //if event isnt spawned, spawn it
                        foundEvent = EventExists(mapNum, t.SpawnX, t.SpawnY);
                        if (foundEvent == -1)
                        {
                            var pageIndex = SpawnEventPage(t);
                            var tmpEvent = new EventIndex(MyEvents.Count, MyClient)
                            {
                                IsGlobal = false,
                                MapNum = mapNum,
                                PageIndex = pageIndex,
                                SpawnX = t.SpawnX,
                                SpawnY = t.SpawnY
                            };
                            MyEvents.Add(tmpEvent);
                            tmpEvent.EventInstance = new Event(t, pageIndex,
                                MyEvents.Count - 1, mapNum)
                            {
                                Dir = t.MyPages[pageIndex].Graphicy
                            };
                            //Send Spawn Event to Player
                            PacketSender.SendEntityData(MyClient, MyEvents.Count - 1, 1, MyEvents[MyEvents.Count - 1].EventInstance);
                            PacketSender.SendEntityPositionTo(MyClient, MyEvents.Count - 1, 1, MyEvents[MyEvents.Count - 1].EventInstance);

                        }
                        else
                        {
                            MyEvents[foundEvent].Update();
                        }
                    }
                    else
                    {
                        //if event is up and running, destroy itttt
                        foundEvent = EventExists(mapNum, t.SpawnX, t.SpawnY);
                        if (foundEvent <= -1 || foundEvent >= MyEvents.Count) continue;
                        MyEvents[foundEvent] = null;
                        PacketSender.SendEntityLeaveTo(MyClient, foundEvent, 1);
                    }
                }
            }
            for (var i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i] == null) continue;
                var eventFound = false;
                if (MyEvents[i].MapNum != CurrentMap)
                {
                    foreach (var t in Globals.GameMaps[CurrentMap].SurroundingMaps)
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
                MyEvents[i] = null;
                PacketSender.SendEntityLeaveTo(MyClient, i, 1);
            }
        }

        //Spawning/Dying
        private void Respawn()
        {
            Warp(Constants.SpawnMap, Constants.SpawnX, Constants.SpawnY, 1);
        }
        public override void Die()
        {
            base.Die();
            Reset();
            Respawn();
        }

        //Warping
        public override void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY);
        }
        public override void Warp(int newMap, int newX, int newY, int newDir)
        {
            
            CurrentX = newX;
            CurrentY = newY;
            if (newMap != CurrentMap || _sentMap == false)
            {
                Console.WriteLine("Sending warp to player.");
                PacketSender.SendEntityLeave(MyIndex,0,CurrentMap);
                CurrentMap = newMap;
                PacketSender.SendEntityDataToProximity(MyIndex, 0, Globals.Entities[MyIndex]);
                PacketSender.SendEntityPositionToAll(MyIndex,0,Globals.Entities[MyIndex]);
                PacketSender.SendMap(MyClient,newMap);
                PacketSender.SendEnterMap(MyClient,newMap);
                _sentMap = true;
                for (var i = 0; i < Globals.Entities.Count; i++)
                {
                    if (i != MyIndex)
                    {
                        PacketSender.SendEntityPositionTo(MyClient, i,0, Globals.Entities[i]);
                    }
                }
            }
            else
            {
                PacketSender.SendEntityPositionToAll(MyIndex,0,Globals.Entities[MyIndex]);
                PacketSender.SendEntityVitals(MyIndex, IsEvent, Globals.Entities[MyIndex]);
                PacketSender.SendEntityStats(MyIndex, IsEvent, Globals.Entities[MyIndex]);
            }
            
        }

        //Inventory
        public bool TryGiveItem(ItemInstance item, bool SendUpdate = true)
        {
            if (Globals.GameItems[item.ItemNum].Type == (int)Enums.ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                Globals.GameItems[item.ItemNum].Type == (int)Enums.ItemTypes.Currency || 
                Globals.GameItems[item.ItemNum].Type == (int)Enums.ItemTypes.None || 
                Globals.GameItems[item.ItemNum].Type == (int)Enums.ItemTypes.Spell)
            {
                for (int i = 0; i < Constants.MaxInvItems; i++)
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
            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                if (Inventory[i].ItemNum == -1){
                    Inventory[i] = item.Clone();
                    if (SendUpdate)
                    {
                        PacketSender.SendInventoryItemUpdate(MyClient, i);
                    }
                    return true;
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
            if (Inventory[slot].ItemNum > -1)
            {
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Currency ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.None ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Spell)
                {
                    if (amount >= Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[slot], amount);
                    if (amount == Inventory[slot].ItemVal)
                    {
                        Inventory[slot] = new ItemInstance();
                        EquipmentProcessItemLoss(slot);
                    }
                    else
                    {
                        Inventory[slot].ItemVal -= amount;
                    }
                }
                else
                {
                    Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[slot], 1);
                    Inventory[slot] = new ItemInstance();
                    EquipmentProcessItemLoss(slot);
                }
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
        }
        public void UseItem(int slot)
        {
            bool equipped = false;
            //TO be implemented.
            if (Inventory[slot].ItemNum > -1)
            {
                //TO DO - CHECK REQUIREMENTS
                switch (Globals.GameItems[Inventory[slot].ItemNum].Type)
                {
                    case (int)Enums.ItemTypes.None:
                    case(int)Enums.ItemTypes.Currency:
                        PacketSender.SendPlayerMsg(MyClient, "You cannot use this item!");
                        break;
                    case (int)Enums.ItemTypes.Equipment:
                        for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
                        {
                            if (Equipment[i] == slot)
                            {
                                Equipment[i] = -1;
                                equipped = true;
                            }
                        }
                        if (!equipped){
                            switch (Globals.GameItems[Inventory[slot].ItemNum].Data1)
                            {
                                case Enums.WeaponIndex:
                                    if (Convert.ToBoolean(Globals.GameItems[Inventory[slot].ItemNum].Data4))
                                    {
                                        Equipment[Enums.ShieldIndex] = -1;
                                    }
                                    Equipment[Enums.WeaponIndex] = slot;
                                    break;
                                case Enums.ShieldIndex:
                                    if (Equipment[Enums.WeaponIndex] > -1)
                                    {
                                        if (Convert.ToBoolean(Globals.GameItems[Inventory[Equipment[Enums.WeaponIndex]].ItemNum].Data4))
                                        {
                                            Equipment[Enums.WeaponIndex] = -1;
                                        }
                                    }
                                    Equipment[Enums.ShieldIndex] = slot;
                                    break;
                                default:
                                    Equipment[Globals.GameItems[Inventory[slot].ItemNum].Data1] = slot;
                                    break;
                            }
                        }
                        PacketSender.SendPlayerEquipmentToProximity(this);
                        break;
                    case (int)Enums.ItemTypes.Spell:
                        if (Globals.GameItems[Inventory[slot].ItemNum].Data1 > -1)
                        {
                            if (TryTeachSpell(new SpellInstance(Globals.GameItems[Inventory[slot].ItemNum].Data1)))
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
            if (Inventory[slot].ItemNum > -1)
            {
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Currency ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.None ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)Enums.ItemTypes.Spell)
                {
                    if (amount > Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    else
                    {
                        if (amount == Inventory[slot].ItemVal)
                        {
                            Inventory[slot] = new ItemInstance();
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
                    Inventory[slot] = new ItemInstance();
                    EquipmentProcessItemLoss(slot);
                    returnVal = true;
                }
                PacketSender.SendInventoryItemUpdate(MyClient, slot);
            }
            return returnVal;
        }

        //Skills
        public bool TryTeachSpell(SpellInstance spell, bool SendUpdate = true)
        {
            if (KnowsSpell(spell.SpellNum)) { return false; }
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == -1)
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
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
            {
                if (Spells[i].SpellNum == spellnum) { return true; }
            }
            return false;
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
        public void ForgetSpell(int spell)
        {
            Spells[spell] = new SpellInstance();
            PacketSender.SendPlayerSpellUpdate(MyClient, spell);
        }
        public void UseSpell(int spell)
        {
            PacketSender.SendPlayerMsg(MyClient, "The use of spells is not yet implemented.");
        }

        //Equipment
        public void UnequipItem(int slot)
        {
            Equipment[slot] = -1;
            PacketSender.SendPlayerEquipmentToProximity(this);
        }
        public void EquipmentProcessItemSwap(int item1, int item2)
        {
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
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
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                    Equipment[i] = -1;
            }
            PacketSender.SendPlayerEquipmentToProximity(this);
        }


        //Stats
        public void UpgradeStat(int statIndex)
        {
            if (Stat[statIndex] < Constants.MaxStatValue)
            {
                Stat[statIndex]++;
                StatPoints--;
                PacketSender.SendEntityStats(MyIndex, 0, this);
                PacketSender.SendPointsTo(MyClient);
            }
        }

        //Hotbar
        public void HotbarChange(int index, int type, int slot)
        {
            Hotbar[index].Type = type;
            Hotbar[index].Slot = slot;
        }
        public void HotbarProcessItemSwap(int item1, int item2)
        {
            for (int i = 0; i < Constants.MaxHotbar; i++)
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
            for (int i = 0; i < Constants.MaxHotbar; i++)
            {
                if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell1)
                    Hotbar[i].Slot = spell2;
                else if (Hotbar[i].Type == 1 && Hotbar[i].Slot == spell2)
                    Hotbar[i].Slot = spell1;
            }
            PacketSender.SendHotbarSlots(MyClient);
        }


        //Event Processing Methods
        public bool MeetsConditions(EventConditions ec, Event ei){
            if (ec.Switch1 - 1 > -1)
            {
                if (Switches[ec.Switch1 - 1] != ec.Switch1Val)
                {
                    return false;
                }
            }
            if (ec.Switch2 - 1 <= -1) return true;
            if (Switches[ec.Switch2 - 1] == ec.Switch2Val) return true;
            return false;
        }
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
        public void TryActivateEvent(int eventIndex)
        {
            if (eventIndex <= -1 || eventIndex >= MyEvents.Count) return;
            if (MyEvents[eventIndex] == null) return;
            if (MyEvents[eventIndex].EventInstance.Trigger != 0) return;
            if (!IsEventOneBlockAway(eventIndex)) return;
            if (MyEvents[eventIndex].CallStack.Count != 0) return;
            var newStack = new EventStack { CommandIndex = 0, ListIndex = 0 };
            MyEvents[eventIndex].CallStack.Push(newStack);
        }
        public void RespondToEvent(int eventIndex, int responseId)
        {
            if (eventIndex <= -1 || eventIndex >= MyEvents.Count) return;
            if (MyEvents[eventIndex] == null) return;
            if (MyEvents[eventIndex].CallStack.Count <= 0) return;
            if (MyEvents[eventIndex].CallStack.Peek().WaitingForResponse != 1) return;
            if (MyEvents[eventIndex].CallStack.Peek().ResponseType == 0)
            {
                MyEvents[eventIndex].CallStack.Peek().WaitingForResponse = 0;
            }
            else
            {
                var tmpStack = new EventStack();
                MyEvents[eventIndex].CallStack.Peek().WaitingForResponse = 0;
                tmpStack.CommandIndex = 0;
                tmpStack.ListIndex = MyEvents[eventIndex].EventInstance.BaseEvent.MyPages[MyEvents[eventIndex].PageIndex].CommandLists[MyEvents[eventIndex].CallStack.Peek().ListIndex].Commands[MyEvents[eventIndex].CallStack.Peek().CommandIndex].Ints[responseId - 1];
                MyEvents[eventIndex].CallStack.Peek().CommandIndex++;
                MyEvents[eventIndex].CallStack.Push(tmpStack);
            }
        }
        static bool IsEventOneBlockAway(int eventIndex)
        {

            return true;
        }
        private static int SpawnEventPage(EventStruct myEvent)
        {
            return 0;
        }
        private bool CanSpawnEvent(EventStruct myEvent, int myMap)
        {
            if (MeetsConditions(myEvent.MyPages[0].MyConditions, new Event(myEvent, 0, 0, myMap))) return true;
            return false;
        }
        


	}

    public class HotbarInstance
    {
        public int Type = -1;
        public int Slot = -1;
    }
}

