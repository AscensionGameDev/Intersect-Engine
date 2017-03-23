using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.Spells;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Entities
{
    public class Player : Entity
    {
        private ItemDescWindow _itemTargetBox;
        private EntityBox _targetBox;

        public int _targetIndex = -1;
        public int _targetType;

        public int Class = -1;
        public int Experience = 0;
        public int ExperienceToNextLevel = 0;
        public HotbarInstance[] Hotbar = new HotbarInstance[Options.MaxHotbar];

        private List<int> mParty = null;

        public bool NoClip = false;
        public Dictionary<int, QuestProgressStruct> QuestProgress = new Dictionary<int, QuestProgressStruct>();
        public int StatPoints = 0;

        public Player(int index, long spawnTime, ByteBuffer bf) : base(index, spawnTime, bf)
        {
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
        }

        public bool IsInParty()
        {
            return Party.Count > 0;
        }

        public List<int> Party
        {
            get
            {
                if (mParty == null)
                {
                    mParty = new List<int>();
                }

                return mParty;
            }
        }

        public override int CurrentMap
        {
            get { return base.CurrentMap; }
            set
            {
                if (value != base.CurrentMap)
                {
                    var oldMap = MapInstance.GetMap(base.CurrentMap);
                    var newMap = MapInstance.GetMap(value);
                    base.CurrentMap = value;
                    if (Globals.Me == this)
                    {
                        if (MapInstance.GetMap(Globals.Me.CurrentMap) != null)
                            GameAudio.PlayMusic(MapInstance.GetMap(Globals.Me.CurrentMap).Music, 3, 3, true);
                        if (newMap != null && oldMap != null)
                        {
                            newMap.CompareEffects(oldMap);
                        }
                    }
                }
            }
        }

        public bool IsInMyParty(Entity entity)
        {
            if (EntityTypes.Player == entity.GetEntityType())
            {
                return Party.Contains(entity.MyIndex);
            }

            return false;
        }

        public bool IsBusy()
        {
            return
                !(Globals.EventHolds.Count == 0 && !Globals.MoveRouteActive && Globals.GameShop == null &&
                  Globals.InBank == false && Globals.InCraft == false && Globals.InTrade == false &&
                  !Gui.HasInputFocus());
        }

        public override bool Update()
        {
            bool returnval = base.Update();
            HandleInput();
            if (!IsBusy())
            {
                if (this == Globals.Me && IsMoving == false)
                {
                    ProcessDirectionalInput();
                }
            }
            if (_targetBox != null)
            {
                _targetBox.Update();
            }
            return returnval;
        }

        //Loading
        public override void Load(ByteBuffer bf)
        {
            base.Load(bf);
            Level = bf.ReadInteger();
            Gender = bf.ReadInteger();
            Class = bf.ReadInteger();

            //The server send entitiy to packet might tack on an extra 1 if the entity being sent is our player
            if (bf.Length() >= 4)
            {
                var isMe = Convert.ToBoolean(bf.ReadInteger());
                if (isMe)
                {
                    Globals.Me = this;
                }
            }
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Player;
        }

        //Item Processing
        public void SwapItems(int item1, int item2)
        {
            ItemInstance tmpInstance = Inventory[item2].Clone();
            Inventory[item2] = Inventory[item1].Clone();
            Inventory[item1] = tmpInstance.Clone();
        }

        public void TryDropItem(int index)
        {
            if (ItemBase.GetItem(Inventory[index].ItemNum) != null)
            {
                if (Inventory[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("inventory", "dropitem"),
                        Strings.Get("inventory", "dropitemprompt", ItemBase.GetItem(Inventory[index].ItemNum).Name),
                        true, DropItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendDropItem(index, 1);
                }
            }
        }

        private void DropItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendDropItem(((InputBox) sender).Slot, value);
            }
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

        public void TryUseItem(int index)
        {
            if (Globals.GameShop == null && Globals.InBank == false & Globals.InTrade == false)
            {
                PacketSender.SendUseItem(index);
            }
        }

        public bool IsEquipped(int slot)
        {
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot)
                {
                    return true;
                }
            }
            return false;
        }

        public void TrySellItem(int index)
        {
            if (ItemBase.GetItem(Inventory[index].ItemNum) != null)
            {
                int foundItem = -1;
                for (int i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
                {
                    if (Globals.GameShop.BuyingItems[i].ItemNum == Inventory[index].ItemNum)
                    {
                        foundItem = i;
                        break;
                    }
                }
                if ((foundItem > -1 && Globals.GameShop.BuyingWhitelist) ||
                    (foundItem == -1 && !Globals.GameShop.BuyingWhitelist))
                {
                    if (Inventory[index].ItemVal > 1)
                    {
                        InputBox iBox = new InputBox(Strings.Get("shop", "sellitem"),
                            Strings.Get("shop", "sellitemprompt", ItemBase.GetItem(Inventory[index].ItemNum).Name), true,
                            SellItemInputBoxOkay, null, index, true);
                    }
                    else
                    {
                        PacketSender.SendSellItem(index, 1);
                    }
                }
                else
                {
                    InputBox iBox = new InputBox(Strings.Get("shop", "sellitem"), Strings.Get("shop", "cannotsell"),
                        true, null, null, -1, false);
                }
            }
        }

        private void SellItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendSellItem(((InputBox) sender).Slot, value);
            }
        }

        //bank
        public void TryDepositItem(int index)
        {
            if (ItemBase.GetItem(Inventory[index].ItemNum) != null)
            {
                if (Inventory[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("bank", "deposititem"),
                        Strings.Get("bank", "deposititemprompt", ItemBase.GetItem(Inventory[index].ItemNum).Name), true,
                        DepositItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendDepositItem(index, 1);
                }
            }
        }

        private void DepositItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendDepositItem(((InputBox) sender).Slot, value);
            }
        }

        public void TryWithdrawItem(int index)
        {
            if (Globals.Bank[index] != null && ItemBase.GetItem(Globals.Bank[index].ItemNum) != null)
            {
                if (Globals.Bank[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("bank", "withdrawitem"),
                        Strings.Get("bank", "withdrawitemprompt", ItemBase.GetItem(Globals.Bank[index].ItemNum).Name),
                        true, WithdrawItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendWithdrawItem(index, 1);
                }
            }
        }

        private void WithdrawItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendWithdrawItem(((InputBox) sender).Slot, value);
            }
        }

        //Bag
        public void TryStoreBagItem(int index)
        {
            if (ItemBase.GetItem(Inventory[index].ItemNum) != null)
            {
                if (Inventory[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("bags", "storeitem"),
                        Strings.Get("bags", "storeitemprompt", ItemBase.GetItem(Inventory[index].ItemNum).Name), true,
                        StoreBagItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendStoreBagItem(index, 1);
                }
            }
        }

        private void StoreBagItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendStoreBagItem(((InputBox) sender).Slot, value);
            }
        }

        public void TryRetreiveBagItem(int index)
        {
            if (Globals.Bag[index] != null && ItemBase.GetItem(Globals.Bag[index].ItemNum) != null)
            {
                if (Globals.Bag[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("bags", "retreiveitem"),
                        Strings.Get("bags", "retreiveitemprompt", ItemBase.GetItem(Globals.Bag[index].ItemNum).Name),
                        true, RetreiveBagItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendRetreiveBagItem(index, 1);
                }
            }
        }

        private void RetreiveBagItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendRetreiveBagItem(((InputBox) sender).Slot, value);
            }
        }

        //Trade
        public void TryTradeItem(int index)
        {
            if (ItemBase.GetItem(Inventory[index].ItemNum) != null)
            {
                if (Inventory[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("trading", "offeritem"),
                        Strings.Get("trading", "offeritemprompt", ItemBase.GetItem(Inventory[index].ItemNum).Name), true,
                        TradeItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendOfferItem(index, 1);
                }
            }
        }

        private void TradeItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendOfferItem(((InputBox) sender).Slot, value);
            }
        }

        public void TryRevokeItem(int index)
        {
            if (Globals.Trade[0, index] != null && ItemBase.GetItem(Globals.Trade[0, index].ItemNum) != null)
            {
                if (Globals.Trade[0, index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox(Strings.Get("trading", "revokeitem"),
                        Strings.Get("trading", "revokeitemprompt",
                            ItemBase.GetItem(Globals.Trade[0, index].ItemNum).Name), true, RevokeItemInputBoxOkay, null,
                        index, true);
                }
                else
                {
                    PacketSender.SendRevokeItem(index, 1);
                }
            }
        }

        private void RevokeItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendRevokeItem(((InputBox) sender).Slot, value);
            }
        }

        //Spell Processing
        public void SwapSpells(int spell1, int spell2)
        {
            SpellInstance tmpInstance = Spells[spell2].Clone();
            Spells[spell2] = Spells[spell1].Clone();
            Spells[spell1] = tmpInstance.Clone();
        }

        public void TryForgetSpell(int index)
        {
            if (SpellBase.GetSpell(Spells[index].SpellNum) != null)
            {
                InputBox iBox = new InputBox(Strings.Get("spells", "forgetspell"),
                    Strings.Get("spells", "forgetspellprompt", SpellBase.GetSpell(Spells[index].SpellNum).Name), true,
                    ForgetSpellInputBoxOkay, null, index, false);
            }
        }

        private void ForgetSpellInputBoxOkay(Object sender, EventArgs e)
        {
            PacketSender.SendForgetSpell(((InputBox) sender).Slot);
        }

        public void TryUseSpell(int index)
        {
            if (Spells[index].SpellNum >= 0 && Spells[index].SpellCD < Globals.System.GetTimeMS())
            {
                PacketSender.SendUseSpell(index, _targetIndex);
            }
        }

        //Hotbar Processing
        public void AddToHotbar(int hotbarSlot, int itemType, int itemSlot)
        {
            Hotbar[hotbarSlot].Type = itemType;
            Hotbar[hotbarSlot].Slot = itemSlot;
            PacketSender.SendHotbarChange(hotbarSlot);
        }

        // Change the dimension if the player is on a gateway
        private void TryToChangeDimension()
        {
            if (CurrentX < Options.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Options.MapHeight && CurrentY >= 0)
                {
                    if (MapInstance.GetMap(CurrentMap) != null &&
                        MapInstance.GetMap(CurrentMap).Attributes[CurrentX, CurrentY] != null)
                    {
                        if (MapInstance.GetMap(CurrentMap).Attributes[CurrentX, CurrentY].value ==
                            (int) MapAttributes.ZDimension)
                        {
                            if (MapInstance.GetMap(CurrentMap).Attributes[CurrentX, CurrentY].data1 > 0)
                            {
                                CurrentZ = MapInstance.GetMap(CurrentMap).Attributes[CurrentX, CurrentY].data1 - 1;
                            }
                        }
                    }
                }
            }
        }

        //Input Handling
        private void HandleInput()
        {
            var movex = 0f;
            var movey = 0f;
            if (Gui.HasInputFocus())
            {
                return;
            }
            if (Globals.InputManager.KeyDown(Keys.W) || Globals.InputManager.KeyDown(Keys.Up))
            {
                movey = 1;
            }
            if (Globals.InputManager.KeyDown(Keys.S) || Globals.InputManager.KeyDown(Keys.Down))
            {
                movey = -1;
            }
            if (Globals.InputManager.KeyDown(Keys.A) || Globals.InputManager.KeyDown(Keys.Left))
            {
                movex = -1;
            }
            if (Globals.InputManager.KeyDown(Keys.D) || Globals.InputManager.KeyDown(Keys.Right))
            {
                movex = 1;
            }
            Globals.Me.MoveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                if (movey < 0)
                {
                    Globals.Me.MoveDir = 1;
                }
                if (movey > 0)
                {
                    Globals.Me.MoveDir = 0;
                }
                if (movex < 0)
                {
                    Globals.Me.MoveDir = 2;
                }
                if (movex > 0)
                {
                    Globals.Me.MoveDir = 3;
                }
            }
        }

        public bool TryBlock()
        {
            if (AttackTimer > Globals.System.GetTimeMS())
            {
                return false;
            }

            if (Options.ShieldIndex > -1 && Globals.Me.Equipment[Options.ShieldIndex] > -1)
            {
                var item = ItemBase.GetItem(Globals.Me.Inventory[Globals.Me.Equipment[Options.ShieldIndex]].ItemNum);
                if (item != null)
                {
                    PacketSender.SendBlock(1);
                    Blocking = true;
                    return true;
                }
            }

            return false;
        }

        public void StopBlocking()
        {
            if (Blocking)
            {
                Blocking = false;
                PacketSender.SendBlock(0);
                AttackTimer = Globals.System.GetTimeMS() + CalculateAttackTime();
            }
        }

        public bool TryAttack()
        {
            if (AttackTimer > Globals.System.GetTimeMS() || Blocking)
            {
                return false;
            }

            var x = Globals.Me.CurrentX;
            var y = Globals.Me.CurrentY;
            var map = Globals.Me.CurrentMap;
            switch (Globals.Me.Dir)
            {
                case 0:
                    y--;
                    break;
                case 1:
                    y++;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    x++;
                    break;
            }
            if (GetRealLocation(ref x, ref y, ref map))
            {
                foreach (var eventMap in MapInstance.GetObjects().Values)
                {
                    foreach (var en in eventMap.LocalEntities)
                    {
                        if (en.Value == null) continue;
                        if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                        {
                            if (en.Value.GetType() == typeof(Event))
                            {
                                //Talk to Event
                                PacketSender.SendActivateEvent(en.Value.CurrentMap, en.Key);
                                AttackTimer = Globals.System.GetTimeMS() + CalculateAttackTime();
                                return true;
                            }
                        }
                    }
                }
                if (Options.WeaponIndex > -1 && Globals.Me.Equipment[Options.WeaponIndex] > -1)
                {
                    var item = ItemBase.GetItem(Globals.Me.Inventory[Globals.Me.Equipment[Options.WeaponIndex]].ItemNum);
                    if (item != null && item.Projectile >= 0)
                    {
                        PacketSender.SendAttack(-1);
                        AttackTimer = Globals.System.GetTimeMS() + CalculateAttackTime();
                        return true;
                    }
                }
                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null) continue;
                    if (en.Value != Globals.Me)
                    {
                        if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                        {
                            //ATTACKKKKK!!!
                            PacketSender.SendAttack(en.Key);
                            AttackTimer = Globals.System.GetTimeMS() + CalculateAttackTime();
                            return true;
                        }
                    }
                }
            }
            //If has a weapon with a projectile equiped, attack anyway
            if (Options.WeaponIndex > -1 && Globals.Me.Equipment[Options.WeaponIndex] > -1)
            {
                var item = ItemBase.GetItem(Globals.Me.Inventory[Globals.Me.Equipment[Options.WeaponIndex]].ItemNum);
                if (item != null && item.Projectile >= 0)
                {
                    PacketSender.SendAttack(_targetIndex);
                    AttackTimer = Globals.System.GetTimeMS() + CalculateAttackTime();
                    return true;
                }
            }
            return false;
        }

        public bool GetRealLocation(ref int x, ref int y, ref int map)
        {
            var tmpX = x;
            var tmpY = y;
            var tmpI = -1;
            if (MapInstance.GetMap(map) != null)
            {
                var gridX = MapInstance.GetMap(map).MapGridX;
                var gridY = MapInstance.GetMap(map).MapGridY;

                if (x < 0)
                {
                    tmpX = (Options.MapWidth) - (x * -1);
                    gridX--;
                }
                if (y < 0)
                {
                    tmpY = (Options.MapHeight) - (y * -1);
                    gridY--;
                }
                if (y > (Options.MapHeight - 1))
                {
                    tmpY = y - (Options.MapHeight);
                    gridY++;
                }
                if (x > (Options.MapWidth - 1))
                {
                    tmpX = x - (Options.MapWidth);
                    gridX++;
                }

                if (gridX >= 0 && gridX < Globals.MapGridWidth && gridY >= 0 && gridY < Globals.MapGridHeight)
                {
                    if (MapInstance.GetMap(Globals.MapGrid[gridX, gridY]) != null)
                    {
                        x = tmpX;
                        y = tmpY;
                        map = Globals.MapGrid[gridX, gridY];
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryTarget()
        {
            var x = (int) Math.Floor(Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left);
            var y = (int) Math.Floor(Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top);

            foreach (var map in MapInstance.GetObjects().Values)
            {
                if (x >= map.GetX() && x <= map.GetX() + (Options.MapWidth * Options.TileWidth))
                {
                    if (y >= map.GetY() && y <= map.GetY() + (Options.MapHeight * Options.TileHeight))
                    {
                        //Remove the offsets to just be dealing with pixels within the map selected
                        x -= (int) map.GetX();
                        y -= (int) map.GetY();

                        //transform pixel format to tile format
                        x /= Options.TileWidth;
                        y /= Options.TileHeight;
                        int mapNum = map.Id;

                        if (GetRealLocation(ref x, ref y, ref mapNum))
                        {
                            foreach (var en in Globals.Entities)
                            {
                                if (en.Value == null) continue;
                                if (en.Value.CurrentMap == mapNum && en.Value.CurrentX == x && en.Value.CurrentY == y &&
                                    !en.Value.IsStealthed())
                                {
                                    if (en.Value.GetType() != typeof(Projectile) &&
                                        en.Value.GetType() != typeof(Resource))
                                    {
                                        if (_targetBox != null)
                                        {
                                            _targetBox.Dispose();
                                            _targetBox = null;
                                        }
                                        if (en.Value != Globals.Me)
                                            _targetBox = new EntityBox(Gui.GameUI.GameCanvas, en.Value, 4, 122);
                                        if (_targetType == 0 && _targetIndex == en.Value.MyIndex)
                                        {
                                            ClearTarget();
                                            return true;
                                        }
                                        if (en.Value.GetType() == typeof(Player))
                                        {
                                            //Select in admin window if open
                                            if (Gui.GameUI.AdminWindowOpen())
                                            {
                                                Gui.GameUI.AdminWindowSelectName(en.Value.MyName);
                                            }
                                        }
                                        _targetType = 0;
                                        _targetIndex = en.Value.MyIndex;
                                        return true;
                                    }
                                }
                            }
                            foreach (var eventMap in MapInstance.GetObjects().Values)
                            {
                                foreach (var en in eventMap.LocalEntities)
                                {
                                    if (en.Value == null) continue;
                                    if (en.Value.CurrentMap == mapNum && en.Value.CurrentX == x &&
                                        en.Value.CurrentY == y && ((Event) en.Value).DisablePreview == 0 &&
                                        !en.Value.IsStealthed())
                                    {
                                        if (_targetBox != null)
                                        {
                                            _targetBox.Dispose();
                                            _targetBox = null;
                                        }
                                        _targetBox = new EntityBox(Gui.GameUI.GameCanvas, en.Value, 4, 122);
                                        if (_targetType == 1 && _targetIndex == en.Value.MyIndex)
                                        {
                                            ClearTarget();
                                            return true;
                                        }
                                        _targetType = 1;
                                        _targetIndex = en.Value.MyIndex;
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        private void ClearTarget()
        {
            if (_targetBox != null)
            {
                _targetBox.Dispose();
                _targetBox = null;
            }
            _targetIndex = -1;
            _targetType = -1;
            if (_itemTargetBox != null)
            {
                _itemTargetBox.Dispose();
                _itemTargetBox = null;
            }
        }

        public bool TryPickupItem()
        {
            var map = MapInstance.GetMap(CurrentMap);
            if (map == null)
            {
                return false;
            }
            foreach (var item in map.MapItems)
            {
                if (item.Value.X == CurrentX && item.Value.Y == CurrentY)
                {
                    PacketSender.SendPickupItem(item.Key);
                    return true;
                }
            }
            return false;
        }

        //Forumlas
        public int GetNextLevelExperience()
        {
            return ExperienceToNextLevel;
        }

        //Movement Processing
        private void ProcessDirectionalInput()
        {
            var didMove = false;
            var tmpI = -1;

            //Check if player is crafting
            if (Globals.InCraft == true)
            {
                return;
            }

            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Stun || Status[n].Type == (int) StatusTypes.Snare)
                {
                    return;
                }
            }

            //Check if the player is dashing, if so don't let them move.
            if (Dashing != null || DashQueue.Count > 0 || DashTimer > Globals.System.GetTimeMS())
            {
                return;
            }

            if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
            {
                //Try to move if able and not casting spells.
                if (!IsMoving && MoveTimer < Globals.System.GetTimeMS() && CastTime < Globals.System.GetTimeMS())
                {
                    switch (MoveDir)
                    {
                        case 0:
                            if (IsTileBlocked(CurrentX, CurrentY - 1, CurrentZ, CurrentMap) == -1)
                            {
                                CurrentY--;
                                Dir = 0;
                                IsMoving = true;
                                OffsetY = Options.TileHeight;
                                OffsetX = 0;
                                TryToChangeDimension();
                            }
                            break;
                        case 1:
                            if (IsTileBlocked(CurrentX, CurrentY + 1, CurrentZ, CurrentMap) == -1)
                            {
                                CurrentY++;
                                Dir = 1;
                                IsMoving = true;
                                OffsetY = -Options.TileHeight;
                                OffsetX = 0;
                                TryToChangeDimension();
                            }
                            break;
                        case 2:
                            if (IsTileBlocked(CurrentX - 1, CurrentY, CurrentZ, CurrentMap) == -1)
                            {
                                CurrentX--;
                                Dir = 2;
                                IsMoving = true;
                                OffsetY = 0;
                                OffsetX = Options.TileWidth;
                                TryToChangeDimension();
                            }
                            break;
                        case 3:
                            if (IsTileBlocked(CurrentX + 1, CurrentY, CurrentZ, CurrentMap) == -1)
                            {
                                CurrentX++;
                                Dir = 3;
                                IsMoving = true;
                                OffsetY = 0;
                                OffsetX = -Options.TileWidth;
                                TryToChangeDimension();
                            }
                            break;
                    }

                    if (IsMoving)
                    {
                        MoveTimer = Globals.System.GetTimeMS() + GetMovementTime();
                        didMove = true;
                        if (CurrentX < 0 || CurrentY < 0 || CurrentX > (Options.MapWidth - 1) ||
                            CurrentY > (Options.MapHeight - 1))
                        {
                            var gridX = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridX;
                            var gridY = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridY;
                            if (CurrentX < 0)
                            {
                                gridX--;
                                CurrentX = (Options.MapWidth - 1);
                            }
                            if (CurrentY < 0)
                            {
                                gridY--;
                                CurrentY = (Options.MapHeight - 1);
                            }
                            if (CurrentX >= Options.MapWidth)
                            {
                                CurrentX = 0;
                                gridX++;
                            }
                            if (CurrentY >= Options.MapHeight)
                            {
                                CurrentY = 0;
                                gridY++;
                            }
                            if (CurrentMap != Globals.MapGrid[gridX, gridY])
                            {
                                CurrentMap = Globals.MapGrid[gridX, gridY];
                                FetchNewMaps();
                            }
                        }
                    }
                    else
                    {
                        if (MoveDir != Dir)
                        {
                            Dir = MoveDir;
                            PacketSender.SendDir(Dir);
                        }
                    }
                }
            }
            Globals.MyX = CurrentX;
            Globals.MyY = CurrentY;
            if (didMove)
            {
                PacketSender.SendMove();
            }
        }

        public void FetchNewMaps()
        {
            if (Globals.MapGridWidth == 0 || Globals.MapGridHeight == 0) return;
            if (MapInstance.GetMap(Globals.Me.CurrentMap) != null)
            {
                var gridX = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridX;
                var gridY = MapInstance.GetMap(Globals.Me.CurrentMap).MapGridY;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                {
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        if (x >= 0 && x < Globals.MapGridWidth && y >= 0 && y < Globals.MapGridHeight &&
                            Globals.MapGrid[x, y] != -1)
                        {
                            if (MapInstance.GetMap(Globals.MapGrid[x, y]) == null)
                            {
                                PacketSender.SendNeedMap(Globals.MapGrid[x, y]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Returns -5 if the tile is completely out of bounds.
        ///     Returns -4 if a tile is blocked because of a local event.
        ///     Returns -3 if a tile is blocked because of a Z dimension tile
        ///     Returns -2 if a tile does not exist or is blocked by a map attribute.
        ///     Returns -1 is a tile is passable.
        ///     Returns any value zero or greater matching the entity index that is in the way.
        /// </summary>
        /// <returns></returns>
        public int IsTileBlocked(int x, int y, int z, int map)
        {
            var mapInstance = MapInstance.GetMap(map);
            if (mapInstance == null) return -2;
            var gridX = mapInstance.MapGridX;
            var gridY = mapInstance.MapGridY;
            try
            {
                int tmpX = x;
                int tmpY = y;
                int tmpMap;
                if (x < 0)
                {
                    gridX--;
                    tmpX = (Options.MapWidth) - (x * -1);
                }
                if (y < 0)
                {
                    gridY--;
                    tmpY = (Options.MapHeight) - (y * -1);
                }
                if (x > (Options.MapWidth - 1))
                {
                    gridX++;
                    tmpX = x - (Options.MapWidth);
                }
                if (y > (Options.MapHeight - 1))
                {
                    gridY++;
                    tmpY = y - (Options.MapHeight);
                }

                if (gridX < 0 || gridY < 0 || gridX >= Globals.MapGridWidth || gridY >= Globals.MapGridHeight)
                    return -2;

                var gameMap = MapInstance.GetMap(Globals.MapGrid[gridX, gridY]);
                if (gameMap != null)
                {
                    if (gameMap.Attributes[tmpX, tmpY] != null)
                    {
                        if (gameMap.Attributes[tmpX, tmpY].value == (int) MapAttributes.Blocked && !NoClip)
                        {
                            return -2;
                        }
                        else if (gameMap.Attributes[tmpX, tmpY].value == (int) MapAttributes.ZDimension && !NoClip)
                        {
                            if (gameMap.Attributes[tmpX, tmpY].data2 - 1 == z)
                            {
                                return -3;
                            }
                        }
                    }
                    tmpMap = Globals.MapGrid[gridX, gridY];
                }
                else
                {
                    return -5;
                }

                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null) continue;
                    if (en.Value == Globals.Me)
                    {
                        continue;
                    }
                    else
                    {
                        if (en.Value.CurrentMap == tmpMap && en.Value.CurrentX == tmpX && en.Value.CurrentY == tmpY &&
                            en.Value.CurrentZ == CurrentZ && !NoClip)
                        {
                            if (en.Value.GetType() != typeof(Projectile))
                            {
                                if (en.Value.GetType() == typeof(Resource))
                                {
                                    var resourceBase = ((Resource) en.Value).GetResourceBase();
                                    if (resourceBase != null)
                                    {
                                        if ((resourceBase.WalkableAfter && ((Resource) en.Value).IsDead) ||
                                            (resourceBase.WalkableBefore && !((Resource) en.Value).IsDead))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else if (en.Value.GetType() == typeof(Player))
                                {
                                    //Return the entity key as this should block the player.  Only exception is if the MapZone this entity is on is passable.
                                    var entityMap = MapInstance.GetMap(en.Value.CurrentMap);
                                    if (Options.PlayerPassable[(int) entityMap.ZoneType]) continue;
                                }
                                return en.Key;
                            }
                        }
                    }
                }
                if (MapInstance.GetMap(tmpMap) != null)
                {
                    foreach (var en in MapInstance.GetMap(tmpMap).LocalEntities)
                    {
                        if (en.Value == null) continue;
                        if (en.Value.CurrentMap == tmpMap && en.Value.CurrentX == tmpX && en.Value.CurrentY == tmpY &&
                            en.Value.CurrentZ == CurrentZ && en.Value.Passable == 0 && !NoClip)
                        {
                            return -4;
                        }
                    }
                }
                return -1;
            }
            catch
            {
                return -2;
            }
        }

        public override void DrawEquipment(string filename, int alpha)
        {
            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Transform)
                {
                    return;
                }
            }

            base.DrawEquipment(filename, alpha);
        }

        //Override of the original function, used for rendering the color of a player based on rank
        public override void DrawName(Color color)
        {
            if (type == 1)
            {
                base.DrawName(new Color(0, 70, 255)); //blue
            }
            else if (type == 2)
            {
                base.DrawName(Color.Red); //red
            }
            else
            {
                base.DrawName(new Color(205, 133, 63)); //light brown
            }
        }

        public void DrawTargets()
        {
            foreach (var en in Globals.Entities)
            {
                if (en.Value == null) continue;
                if (!en.Value.IsStealthed())
                {
                    if (en.Value.GetType() != typeof(Projectile) && en.Value.GetType() != typeof(Resource))
                    {
                        if (_targetType == 0 && _targetIndex == en.Value.MyIndex)
                        {
                            en.Value.DrawTarget((int) TargetTypes.Selected);
                        }
                    }
                }
            }
            foreach (var eventMap in MapInstance.GetObjects().Values)
            {
                foreach (var en in eventMap.LocalEntities)
                {
                    if (en.Value == null) continue;
                    if (en.Value.CurrentMap == eventMap.Id && ((Event) en.Value).DisablePreview == 0 &&
                        !en.Value.IsStealthed())
                    {
                        if (_targetType == 1 && _targetIndex == en.Value.MyIndex)
                        {
                            en.Value.DrawTarget((int) TargetTypes.Selected);
                        }
                    }
                }
            }

            var x = (int) Math.Floor(Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left);
            var y = (int) Math.Floor(Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top);

            foreach (var map in MapInstance.GetObjects().Values)
            {
                if (x >= map.GetX() && x <= map.GetX() + (Options.MapWidth * Options.TileWidth))
                {
                    if (y >= map.GetY() && y <= map.GetY() + (Options.MapHeight * Options.TileHeight))
                    {
                        //Remove the offsets to just be dealing with pixels within the map selected
                        x -= (int) map.GetX();
                        y -= (int) map.GetY();

                        //transform pixel format to tile format
                        x /= Options.TileWidth;
                        y /= Options.TileHeight;
                        int mapNum = map.Id;

                        if (GetRealLocation(ref x, ref y, ref mapNum))
                        {
                            foreach (var en in Globals.Entities)
                            {
                                if (en.Value == null) continue;
                                if (en.Value.CurrentMap == mapNum && en.Value.CurrentX == x && en.Value.CurrentY == y &&
                                    !en.Value.IsStealthed())
                                {
                                    if (en.Value.GetType() != typeof(Projectile) &&
                                        en.Value.GetType() != typeof(Resource))
                                    {
                                        if (_targetType != 0 || _targetIndex != en.Value.MyIndex)
                                        {
                                            en.Value.DrawTarget((int) TargetTypes.Hover);
                                        }
                                    }
                                }
                            }
                            foreach (var eventMap in MapInstance.GetObjects().Values)
                            {
                                foreach (var en in eventMap.LocalEntities)
                                {
                                    if (en.Value == null) continue;
                                    if (en.Value.CurrentMap == mapNum && en.Value.CurrentX == x &&
                                        en.Value.CurrentY == y && ((Event) en.Value).DisablePreview == 0 &&
                                        !en.Value.IsStealthed())
                                    {
                                        if (_targetType != 1 || _targetIndex != en.Value.MyIndex)
                                        {
                                            en.Value.DrawTarget((int) TargetTypes.Hover);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    public class HotbarInstance
    {
        public int Slot = -1;
        public int Type = -1;
    }
}