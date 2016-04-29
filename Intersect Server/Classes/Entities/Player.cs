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
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;
using Options = Intersect_Server.Classes.General.Options;

namespace Intersect_Server.Classes.Entities
{

    public class Player : Entity
    {
        public bool InGame;
        public Client MyClient;
        private bool _sentMap;
        public List<EventInstance> MyEvents = new List<EventInstance>();
        public bool[] Switches;
        public int[] Variables;
        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];
        public int[] Equipment = new int[Options.EquipmentSlots.Count];
        public int StatPoints = 0;
        public int Class = 0;
        public int Gender = 0;
        public int Level = 0;
        public int Experience = 0;
        public ItemInstance[] Bank = new ItemInstance[Options.MaxBankSlots];

        //Temporary Values
        private int _curMapLink = -1;
        private object EventLock = new object();
        public bool InBank;
        public int InShop = -1;

        //Init
        public Player(int index, Client newClient) : base(index)
        {
            MyClient = newClient;
            Switches = new bool[Options.MaxPlayerSwitches];
            Variables = new int[Options.MaxPlayerVariables];
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
            base.Update();
            //If we switched maps, lets update the maps
            if (_curMapLink != CurrentMap)
            {
                if (_curMapLink != -1)
                {
                    Globals.GameMaps[_curMapLink].RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    if (!Globals.GameMaps.ContainsKey(CurrentMap))
                    {
                        WarpToSpawn(true);
                    }
                    else
                    {
                        Globals.GameMaps[CurrentMap].AddEntity(this);
                    }
                }
                _curMapLink = CurrentMap;
            }

            //Check to see if we can spawn events, if already spawned.. update them.
            lock (EventLock)
            {
                for (var i = 0; i < Globals.GameMaps[CurrentMap].SurroundingMaps.Count + 1; i++)
                {
                    int mapNum;
                    if (i == Globals.GameMaps[CurrentMap].SurroundingMaps.Count)
                    {
                        mapNum = CurrentMap;
                    }
                    else
                    {
                        mapNum = Globals.GameMaps[CurrentMap].SurroundingMaps[i];
                    }
                    if (mapNum <= -1) continue;
                    foreach (var mapEvent in Globals.GameMaps[mapNum].Events)
                    {
                        if (mapEvent.Deleted == 0)
                        {
                            //Look for event
                            var foundEvent = EventExists(mapNum, mapEvent.SpawnX, mapEvent.SpawnY);
                            if (foundEvent == -1)
                            {
                                var tmpEvent = new EventInstance(MyEvents.Count, MyClient, mapEvent, mapNum)
                                {
                                    IsGlobal = mapEvent.IsGlobal == 1,
                                    MapNum = mapNum,
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
            return bf.ToArray();
        }

        //Spawning/Dying
        private void Respawn()
        {
            Warp(Globals.GameClasses[Class].SpawnMap, Globals.GameClasses[Class].SpawnX, Globals.GameClasses[Class].SpawnY, Globals.GameClasses[Class].SpawnDir);
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
            PacketSender.SendEntityVitals(MyIndex, (int)EntityTypes.Player, this);
        }

        //Leveling
        public void SetLevel(int level, bool resetExperience = false)
        {
            if (level > 0)
            {
                Level = Math.Min(Options.MaxLevel,level);
                if (resetExperience) Experience = 0;
                PacketSender.SendEntityDataToProximity(MyIndex, (int)EntityTypes.Player, Data(), this);
                PacketSender.SendExperience(MyClient);
            }
        }
        public void LevelUp(bool resetExperience = true, int levels = 1)
        {
            if (Level < Options.MaxLevel)
            {
                SetLevel(Level + levels, resetExperience);
                PacketSender.SendPlayerMsg(MyClient, "You have leveled up! You are now level " + Level + "!", Color.Blue);
                //ToDo, add stat points based on class I guess?
                StatPoints += 5*levels;
                if (StatPoints > 0)
                {
                    PacketSender.SendPlayerMsg(MyClient,
                        "You have " + StatPoints + " stat points available to be spent!", Color.Blue);
                }
                PacketSender.SendExperience(MyClient);
                PacketSender.SendEntityDataToProximity(MyIndex, (int) EntityTypes.Player, Data(), this);
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
            while (Experience > GetExperienceToNextLevel() && GetExperienceToNextLevel() > 0)
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
            //TODO: Program this
            if (Level >= Options.MaxLevel) return -1;
            return 1000;
        }


        //Warping
        public override void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, 1);
        }
        public override void Warp(int newMap, int newX, int newY, int newDir)
        {
            if (!Globals.GameMaps.ContainsKey(newMap))
            {
                Globals.GeneralLogs.Add("Failed to warp player to new map -- warping to /spawn/.");
                WarpToSpawn(true);
                return;
            }
            CurrentX = newX;
            CurrentY = newY;
            if (newMap != CurrentMap || _sentMap == false)
            {
                PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.Player, CurrentMap);
                CurrentMap = newMap;
                PacketSender.SendEntityDataToProximity(MyIndex, (int)EntityTypes.Player, Data(), Globals.Entities[MyIndex]);
                PacketSender.SendEntityPositionToAll(MyIndex, (int)EntityTypes.Player, Globals.Entities[MyIndex]);
                PacketSender.SendMap(MyClient, newMap);
                PacketSender.SendEnterMap(MyClient, newMap);
                _sentMap = true;
            }
            else
            {
                PacketSender.SendEntityPositionToAll(MyIndex, (int)EntityTypes.Player, Globals.Entities[MyIndex]);
                PacketSender.SendEntityVitals(MyIndex, (int)EntityTypes.Player, Globals.Entities[MyIndex]);
                PacketSender.SendEntityStats(MyIndex, (int)EntityTypes.Player, Globals.Entities[MyIndex]);
            }

        }
        public void WarpToSpawn(bool sendWarp = false)
        {
            int map = 0, x = 0, y = 0;
            if (Globals.GameMaps.ContainsKey(Globals.GameClasses[Class].SpawnMap))
            {
                map = Globals.GameClasses[Class].SpawnMap;
            }
            else
            {
                map = Globals.GameMaps.GetEnumerator().Current.Value.MyMapNum;
            }
            x = Globals.GameClasses[Class].SpawnX;
            y = Globals.GameClasses[Class].SpawnY;
            if (sendWarp) { Warp(map, x, y); }
        }

        //Inventory
        public bool CanGiveItem(ItemInstance item)
        {
            if (Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Currency ||
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.None ||
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Spell)
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

            return false;
        }
        public bool TryGiveItem(ItemInstance item, bool SendUpdate = true)
        {
            if (Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Currency ||
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.None ||
                Globals.GameItems[item.ItemNum].Type == (int)ItemTypes.Spell)
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
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.None ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Spell)
                {
                    if (amount >= Inventory[slot].ItemVal)
                    {
                        amount = Inventory[slot].ItemVal;
                    }
                    Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[slot], amount);
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
                    Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[slot], 1);
                    Inventory[slot] = new ItemInstance(-1, 0);
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
                            if (Globals.GameItems[Inventory[slot].ItemNum].Data1 == Options.WeaponIndex)
                            {
                                if (Options.WeaponIndex > -1)
                                {
                                    if (Convert.ToBoolean(Globals.GameItems[Inventory[slot].ItemNum].Data4))
                                    {
                                        Equipment[Options.ShieldIndex] = -1;
                                    }
                                    Equipment[Options.WeaponIndex] = slot;
                                }
                            }
                            else if (Globals.GameItems[Inventory[slot].ItemNum].Data1 == Options.ShieldIndex)
                            {
                                if (Options.ShieldIndex > -1)
                                {
                                    if (Equipment[Options.WeaponIndex] > -1)
                                    {
                                        if (Convert.ToBoolean(Globals.GameItems[Inventory[Equipment[Options.WeaponIndex]].ItemNum].Data4))
                                        {
                                            Equipment[Options.WeaponIndex] = -1;
                                        }
                                    }
                                    Equipment[Options.ShieldIndex] = slot;
                                }
                            }
                            else
                            {
                                Equipment[Globals.GameItems[Inventory[slot].ItemNum].Data1] = slot;
                            }
                        }
                        PacketSender.SendPlayerEquipmentToProximity(this);
                        break;
                    case (int)ItemTypes.Spell:
                        if (Globals.GameItems[Inventory[slot].ItemNum].Data1 > -1)
                        {
                            if (TryTeachSpell(new SpellInstance(Globals.GameItems[Inventory[slot].ItemNum].Data1)))
                            {
                                TakeItem(slot, 1);
                            }
                        }
                        break;
                    case (int)ItemTypes.Event:

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
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.None ||
                            Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Spell)
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
            if (InShop > -1 || InBank) return false;
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
            ShopStruct shop = Globals.GameShops[InShop];
            if (Inventory[slot].ItemNum > -1)
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
                        rewardItemVal = Globals.GameItems[sellItemNum].Price;
                    }
                }

                if (Globals.GameItems[sellItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            Globals.GameItems[sellItemNum].Type == (int)ItemTypes.Currency ||
                            Globals.GameItems[sellItemNum].Type == (int)ItemTypes.None ||
                            Globals.GameItems[sellItemNum].Type == (int)ItemTypes.Spell)
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
        public void BuyItem(int slot, int amount)
        {
            bool canSellItem = true;
            int buyItemNum = -1;
            int buyItemAmt = 1;
            if (InShop == -1) return;
            ShopStruct shop = Globals.GameShops[InShop];
            if (slot >= 0 && slot < shop.SellingItems.Count)
            {
                buyItemNum = shop.SellingItems[slot].ItemNum;
                if (Globals.GameItems[buyItemNum].Type == (int)ItemTypes.Consumable ||
                    //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    Globals.GameItems[buyItemNum].Type == (int)ItemTypes.Currency ||
                    Globals.GameItems[buyItemNum].Type == (int)ItemTypes.None ||
                    Globals.GameItems[buyItemNum].Type == (int)ItemTypes.Spell)
                {
                    buyItemAmt = Math.Min(1, amount);
                }
                if (FindItem(shop.SellingItems[slot].CostItemNum, shop.SellingItems[slot].CostItemVal * buyItemAmt) > -1)
                {
                    if (CanGiveItem(new ItemInstance(buyItemNum, buyItemAmt)))
                    {
                        TakeItem(
                                FindItem(shop.SellingItems[slot].CostItemNum,
                                    shop.SellingItems[slot].CostItemVal * buyItemAmt),
                                shop.SellingItems[slot].CostItemVal * buyItemAmt);
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
                            PacketSender.SendPlayerMsg(MyClient, "You do not have space to purchase that item!", Color.Red);
                        }
                    }
                }
                else
                {
                    PacketSender.SendPlayerMsg(MyClient, "Transaction failed due to insufficent funds.", Color.Red);
                }

            }
        }

        //Bank
        public bool OpenBank()
        {
            if (InShop > -1 || InBank) return false;
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
            if (Inventory[slot].ItemNum > -1)
            {
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Consumable ||
                    //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                    Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.None ||
                    Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Spell)
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
                if (Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                    Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.None ||
                     Globals.GameItems[Inventory[slot].ItemNum].Type == (int)ItemTypes.Spell)
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

        public void WithdrawItem(int slot, int amount)
        {
            if (!InBank) return;
            if (Bank[slot] != null && Bank[slot].ItemNum > -1)
            {
                if (Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Consumable ||
                    //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                    Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.None ||
                    Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Spell)
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
                if (Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                    Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Currency ||
                    Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.None ||
                     Globals.GameItems[Bank[slot].ItemNum].Type == (int)ItemTypes.Spell)
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
                PacketSender.SendPlayerMsg(MyClient, "There is no space left in your inventory for that item!", Color.Red);
            }
            else
            {
                PacketSender.SendPlayerMsg(MyClient, "Invalid item selected to withdraw!", Color.Red);
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

        //Skills
        public bool TryTeachSpell(SpellInstance spell, bool SendUpdate = true)
        {
            if (KnowsSpell(spell.SpellNum)) { return false; }
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
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
            if (spellNum > -1)
            {
                //Check if caster does not have the correct combat stats, if not exit now.
                for (var n = 0; n < (int)Stats.StatCount; n++)
                {
                    if (Stat[n].Value() < Globals.GameSpells[spellNum].StatReq[n])
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

                if (Globals.GameSpells[spellNum].VitalCost[(int)Vitals.Mana] <= Vital[(int)Vitals.Mana])
                {
                    if (Globals.GameSpells[spellNum].VitalCost[(int)Vitals.Health] <= Vital[(int)Vitals.Health])
                    {
                        if (Spells[spellSlot].SpellCD < Environment.TickCount)
                        {
                            if (CastTime < Environment.TickCount)
                            {
                                Vital[(int)Vitals.Mana] = Vital[(int)Vitals.Mana] - Globals.GameSpells[spellNum].VitalCost[(int)Vitals.Mana];
                                Vital[(int)Vitals.Health] = Vital[(int)Vitals.Health] - Globals.GameSpells[spellNum].VitalCost[(int)Vitals.Health];
                                CastTime = Environment.TickCount + (Globals.GameSpells[spellNum].CastDuration * 100);
                                SpellCastSlot = spellSlot;

                                if (Globals.GameSpells[spellNum].CastAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(Globals.GameSpells[spellNum].CastAnimation, 1, MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
                                }

                                PacketSender.SendEntityVitals(MyIndex, (int)Vitals.Health, Globals.Entities[MyIndex]);
                                PacketSender.SendEntityVitals(MyIndex, (int)Vitals.Mana, Globals.Entities[MyIndex]);
                                PacketSender.SendEntityCastTime(MyIndex, spellNum);
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
                PacketSender.SendEntityStats(MyIndex, (int)EntityTypes.Player, this);
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
                        var newStack = new CommandInstance(MyEvents[i].PageInstance.MyPage) { CommandIndex = 0, ListIndex = 0 };
                        MyEvents[i].CallStack.Push(newStack);
                        if (!MyEvents[i].IsGlobal) MyEvents[i].PageInstance.TurnTowardsPlayer();
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
                    if (MyEvents[i] != null && MyEvents[i].MapNum == mapNum &&
                        MyEvents[i].BaseEvent.MyIndex == eventIndex)
                    {
                        if (MyEvents[i].CallStack.Count <= 0) return;
                        if (MyEvents[i].CallStack.Peek().WaitingForResponse != 1) return;
                        if (MyEvents[i].CallStack.Peek().ResponseType == 0)
                        {
                            MyEvents[i].CallStack.Peek().WaitingForResponse = 0;
                        }
                        else
                        {
                            var tmpStack = new CommandInstance(MyEvents[i].PageInstance.BaseEvent.MyPages[MyEvents[i].PageIndex]);
                            tmpStack.CommandIndex = 0;
                            tmpStack.ListIndex =
                                MyEvents[i].PageInstance.BaseEvent.MyPages[MyEvents[i].PageIndex].CommandLists[
                                    MyEvents[i].CallStack.Peek().ListIndex].Commands[
                                        MyEvents[i].CallStack.Peek().CommandIndex].Ints[responseId - 1];
                            MyEvents[i].CallStack.Peek().CommandIndex++;
                            MyEvents[i].CallStack.Peek().WaitingForResponse = 0;
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
            for (int i = 0; i < MyEvents.Count; i++)
            {
                if (MyEvents[i].PageInstance == en)
                {
                    id = i;
                    return id;
                }
            }
            return id;
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
    }

    public class HotbarInstance
    {
        public int Type = -1;
        public int Slot = -1;
    }
}

