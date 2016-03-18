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
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.Spells;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect_Client.Classes.Entities
{
    public class Player : Entity
    {
        private long _attackTimer;
        public HotbarInstance[] Hotbar = new HotbarInstance[Constants.MaxHotbar];
        public int StatPoints = 0;
        public int Experience = 0;

        public bool NoClip = false;

        private int _targetType = -1; //None (-1), Entity, Item, Event
        private int _targetIndex = -1;
        private EntityBox _targetBox;
        private ItemDescWindow _itemTargetBox;

        public Player()
            : base()
        {
            for (int i = 0; i < Constants.MaxHotbar; i++)
            {
                Hotbar[i] = new HotbarInstance();
            }
        }

        public override bool Update()
        {
            bool returnval = base.Update();
            HandleInput();
            if (Globals.MyIndex == MyIndex && base.IsMoving == false) { ProcessDirectionalInput(); }
            if ((Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left) && Gui.MouseHitGUI() == false) || Globals.InputManager.KeyDown(Keys.E))
            {
                TryTarget();
                if (TryAttack()) { return returnval; }
                if (TryPickupItem()) { return returnval; }
            }
            if (_targetBox != null) { _targetBox.Update(); }
            return returnval;
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
            if (Inventory[index].ItemNum > -1)
            {
                if (Inventory[index].ItemVal > 1)
                {
                    InputBox iBox = new InputBox("Drop Item", "How many/much " + Globals.GameItems[Inventory[index].ItemNum].Name + " do you want to drop?", true, DropItemInputBoxOkay, null, index, true);
                }
                else
                {
                    PacketSender.SendDropItem(index, 1);
                }
            }
        }
        private void DropItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendDropItem(((InputBox)sender).Slot, value);
            }
        }
        public void TryUseItem(int index)
        {
            PacketSender.SendUseItem(index);
        }
        public bool IsEquipped(int slot)
        {
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++)
            {
                if (Equipment[i] == slot) { return true; }
            }
            return false;
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
            if (Spells[index].SpellNum > -1)
            {
                InputBox iBox = new InputBox("Forget Spell", "Are you sure you want to forget " + Globals.GameSpells[Spells[index].SpellNum].Name + "?", true, ForgetSpellInputBoxOkay, null, index, false);
            }
        }
        private void ForgetSpellInputBoxOkay(Object sender, EventArgs e)
        {
            PacketSender.SendForgetSpell(((InputBox)sender).Slot);
        }
        public void TryUseSpell(int index)
        {
            if (Spells[index].SpellNum >= 0 && Spells[index].SpellCD < Globals.System.GetTimeMS())
            {
                PacketSender.SendUseSpell(index);
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
            if (CurrentX < Globals.Database.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Globals.Database.MapHeight && CurrentY >= 0)
                {
                    if (Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].value == (int)Enums.MapAttributes.ZDimension)
                    {
                        if (Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].data1 > 0)
                        {
                            CurrentZ = Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].data1 - 1;
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
            if (Gui.HasInputFocus()) { return; }
            if (Globals.InputManager.KeyDown(Keys.W)) { movey = 1; }
            if (Globals.InputManager.KeyDown(Keys.S)) { movey = -1; }
            if (Globals.InputManager.KeyDown(Keys.A)) { movex = -1; }
            if (Globals.InputManager.KeyDown(Keys.D)) { movex = 1; }
            Globals.Entities[Globals.MyIndex].MoveDir = -1;
            if (movex != 0f || movey != 0f)
            {
                if (movey < 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 1;
                }
                if (movey > 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 0;
                }
                if (movex < 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 2;
                }
                if (movex > 0)
                {
                    Globals.Entities[Globals.MyIndex].MoveDir = 3;
                }

            }
        }
        private bool TryAttack()
        {
            if (_attackTimer > Globals.System.GetTimeMS()) { return false; }
            var x = Globals.Entities[Globals.MyIndex].CurrentX;
            var y = Globals.Entities[Globals.MyIndex].CurrentY;
            var map = Globals.Entities[Globals.MyIndex].CurrentMap;
            switch (Globals.Entities[Globals.MyIndex].Dir)
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
                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null) continue;
                    if (en.Value != Globals.Me)
                    {
                        if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                        {
                            //ATTACKKKKK!!!
                            PacketSender.SendAttack(en.Key);
                            _attackTimer = Globals.System.GetTimeMS() + 1000;
                            return true;
                        }
                    }
                }
                foreach (var eventMap in Globals.GameMaps)
                {
                    foreach (var en in eventMap.Value.LocalEntities)
                    {
                        if (en.Value == null) continue;
                        if (en.Value != Globals.Me)
                        {
                            if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                            {
                                if (en.Value.GetType() == typeof(Event))
                                {
                                    //Talk to Event
                                    PacketSender.SendActivateEvent(en.Value.CurrentMap, en.Key);
                                    _attackTimer = Globals.System.GetTimeMS() + 1000;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool GetRealLocation(ref int x, ref int y, ref int map)
        {
            var tmpX = x;
            var tmpY = y;
            var tmpMap = map;
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == map)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (tmpI == -1)
            {
                return true;
            }
            try
            {
                if (x < 0)
                {
                    tmpX = (Globals.Database.MapWidth - 1) - (x * -1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = (Globals.Database.MapHeight - 1) - (y * -1);
                        if (Globals.LocalMaps[tmpI - 4] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > (Globals.Database.MapHeight - 1))
                    {
                        tmpY = y - (Globals.Database.MapHeight - 1);
                        if (Globals.LocalMaps[tmpI + 2] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI - 1] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 1];
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (x > (Globals.Database.MapWidth - 1))
                {
                    tmpX = x - (Globals.Database.MapWidth - 1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = (Globals.Database.MapHeight - 1) - (y * -1);
                        if (Globals.LocalMaps[tmpI - 2] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > (Globals.Database.MapHeight - 1))
                    {
                        tmpY = y - (Globals.Database.MapHeight - 1);
                        if (Globals.LocalMaps[tmpI + 4] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI + 1] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI + 1];
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (y < 0)
                {
                    tmpX = x;
                    tmpY = (Globals.Database.MapHeight - 1) - (y * -1);
                    if (Globals.LocalMaps[tmpI - 3] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI - 3];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (y > (Globals.Database.MapHeight - 1))
                {
                    tmpX = x;
                    tmpY = y - (Globals.Database.MapHeight - 1);
                    if (Globals.LocalMaps[tmpI + 3] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI + 3];
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                    if (Globals.LocalMaps[tmpI] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI];
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;

            }
        }
        private bool TryTarget()
        {
            var x = (int)Math.Floor((Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left) / Globals.Database.TileWidth);
            var y = (int)Math.Floor((Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top) / Globals.Database.TileHeight);
            var map = Globals.Entities[Globals.MyIndex].CurrentMap;
            if (GetRealLocation(ref x, ref y, ref map))
            {
                foreach (var en in Globals.Entities)
                {
                    if (en.Value == null) continue;
                    if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                    {
                        if (_targetBox != null) { _targetBox.Dispose(); _targetBox = null; }
                        _targetBox = new EntityBox(Gui.GameUI.GameCanvas, en.Value, 0, 100);
                        return true;
                    }
                }
                foreach (var eventMap in Globals.GameMaps)
                {
                    foreach (var en in eventMap.Value.LocalEntities)
                    {
                        if (en.Value == null) continue;
                        if (en.Value.CurrentMap == map && en.Value.CurrentX == x && en.Value.CurrentY == y)
                        {
                            if (_targetBox != null) { _targetBox.Dispose(); _targetBox = null; }
                            _targetBox = new EntityBox(Gui.GameUI.GameCanvas, en.Value, 0, 100);
                            return true;
                        }
                    }
                }
            }
            if (_targetBox != null) { _targetBox.Dispose(); _targetBox = null; }
            if (_itemTargetBox != null) { _itemTargetBox.Dispose(); _itemTargetBox = null; }
            return false;
        }
        private bool TryPickupItem()
        {
            if (!Globals.GameMaps.ContainsKey(CurrentMap)) { return false; }
            for (int i = 0; i < Globals.GameMaps[CurrentMap].MapItems.Count; i++)
            {
                if (Globals.GameMaps[CurrentMap].MapItems[i].X == CurrentX && Globals.GameMaps[CurrentMap].MapItems[i].Y == CurrentY)
                {
                    PacketSender.SendPickupItem(i);
                    return true;
                }
            }
            return false;
        }

        //Forumlas
        public int GetNextLevelExperience()
        {
            if (Level == Constants.MaxLevel)
            {
                return 0;
            }
            else
            {
                return (int)Math.Pow(2, Level + 1) * 20;
            }
        }

        //Movement Processing
        private void ProcessDirectionalInput()
        {
            var didMove = false;
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] == CurrentMap)
                {
                    tmpI = i;
                    i = 9;
                }
            }
            if (MoveDir > -1 && Globals.EventDialogs.Count == 0)
            {
                //Try to move if able and not casting spells.
                if (MoveTimer < Globals.System.GetTimeMS() && CastTime < Globals.System.GetTimeMS())
                {
                    switch (MoveDir)
                    {
                        case 0:
                            if (IsTileBlocked(CurrentX, CurrentY - 1, CurrentZ, CurrentMap) == -1)
                            {
                                CurrentY--;
                                Dir = 0;
                                IsMoving = true;
                                OffsetY = Globals.Database.TileHeight;
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
                                OffsetY = -Globals.Database.TileHeight;
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
                                OffsetX = Globals.Database.TileWidth;
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
                                OffsetX = -Globals.Database.TileWidth;
                                TryToChangeDimension();
                            }
                            break;
                    }

                    if (IsMoving)
                    {
                        MoveTimer = Globals.System.GetTimeMS() + (Stat[(int)Enums.Stats.Speed] / 10f);
                        didMove = true;
                        if (CurrentX < 0 || CurrentY < 0 || CurrentX > (Globals.Database.MapWidth - 1) || CurrentY > (Globals.Database.MapHeight - 1))
                        {
                            if (tmpI != -1)
                            {
                                try
                                {
                                    //At each of these cases, we have switched chunks. We need to re-number the chunk renderers.
                                    if (CurrentX < 0)
                                    {

                                        if (CurrentY < 0)
                                        {
                                            if (Globals.LocalMaps[tmpI - 4] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI - 4];
                                                CurrentX = (Globals.Database.MapWidth - 1);
                                                CurrentY = (Globals.Database.MapHeight - 1);
                                                UpdateMapRenderers(0);
                                                UpdateMapRenderers(2);
                                            }
                                        }
                                        else if (CurrentY > (Globals.Database.MapHeight - 1))
                                        {
                                            if (Globals.LocalMaps[tmpI + 2] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI + 2];
                                                CurrentX = (Globals.Database.MapWidth - 1);
                                                CurrentY = 0;
                                                UpdateMapRenderers(1);
                                                UpdateMapRenderers(2);
                                            }

                                        }
                                        else
                                        {
                                            if (Globals.LocalMaps[tmpI - 1] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI - 1];
                                                CurrentX = (Globals.Database.MapWidth - 1);
                                                UpdateMapRenderers(2);
                                            }
                                        }

                                    }
                                    else if (CurrentX > (Globals.Database.MapWidth - 1))
                                    {
                                        if (CurrentY < 0)
                                        {
                                            if (Globals.LocalMaps[tmpI - 2] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI - 2];
                                                CurrentX = 0;
                                                CurrentY = (Globals.Database.MapHeight - 1);
                                                UpdateMapRenderers(0);
                                                UpdateMapRenderers(3);
                                            }
                                        }
                                        else if (CurrentY > (Globals.Database.MapHeight - 1))
                                        {
                                            if (Globals.LocalMaps[tmpI + 4] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI + 4];
                                                CurrentX = 0;
                                                CurrentY = 0;
                                                UpdateMapRenderers(1);
                                                UpdateMapRenderers(3);
                                            }

                                        }
                                        else
                                        {
                                            if (Globals.LocalMaps[tmpI + 1] > -1)
                                            {
                                                CurrentX = 0;
                                                CurrentMap = Globals.LocalMaps[tmpI + 1];
                                                UpdateMapRenderers(3);
                                            }
                                        }
                                    }
                                    else if (CurrentY < 0)
                                    {
                                        if (Globals.LocalMaps[tmpI - 3] > -1)
                                        {
                                            CurrentY = (Globals.Database.MapHeight - 1);
                                            CurrentMap = Globals.LocalMaps[tmpI - 3];
                                            UpdateMapRenderers(0);
                                        }
                                    }
                                    else if (CurrentY > (Globals.Database.MapHeight - 1))
                                    {
                                        if (Globals.LocalMaps[tmpI + 3] > -1)
                                        {
                                            CurrentY = 0;
                                            CurrentMap = Globals.LocalMaps[tmpI + 3];
                                            UpdateMapRenderers(1);
                                        }

                                    }
                                }
                                catch (Exception)
                                {
                                    //player out of bounds
                                    //Debug.Log("Detected player out of bounds.");
                                }


                            }
                            else
                            {
                                //player out of bounds
                                //.Log("Detected player out of bounds.");
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
        private void UpdateMapRenderers(int dir)
        {
            if (!IsLocal)
            {
                return;
            }
            if (dir == 2)
            {
                if (Globals.LocalMaps[3] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[3];
                    Globals.LocalMaps[2] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = Globals.LocalMaps[0];
                    Globals.LocalMaps[0] = -1;
                    Globals.LocalMaps[5] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = -1;
                    Globals.LocalMaps[8] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = Globals.LocalMaps[6];
                    Globals.LocalMaps[6] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    GameGraphics.DarkOffsetX = Globals.Database.TileWidth * Globals.Database.MapWidth;
                    PacketSender.SendEnterMap();
                }
            }
            else if (dir == 3)
            {
                if (Globals.LocalMaps[5] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[5];
                    Globals.LocalMaps[0] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = Globals.LocalMaps[2];
                    Globals.LocalMaps[2] = -1;
                    Globals.LocalMaps[3] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = -1;
                    Globals.LocalMaps[6] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = Globals.LocalMaps[8];
                    Globals.LocalMaps[8] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    GameGraphics.DarkOffsetX = -Globals.Database.TileWidth * Globals.Database.MapWidth;
                    PacketSender.SendEnterMap();
                }

            }
            else if (dir == 1)
            {
                if (Globals.LocalMaps[7] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[7];
                    Globals.LocalMaps[0] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = Globals.LocalMaps[6];
                    Globals.LocalMaps[6] = -1;
                    Globals.LocalMaps[1] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[7];
                    Globals.LocalMaps[7] = -1;
                    Globals.LocalMaps[2] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = Globals.LocalMaps[8];
                    Globals.LocalMaps[8] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    GameGraphics.DarkOffsetY = -Globals.Database.TileHeight * Globals.Database.MapHeight;
                    PacketSender.SendEnterMap();
                }
            }
            else
            {
                if (Globals.LocalMaps[1] > -1)
                {
                    Globals.CurrentMap = Globals.LocalMaps[1];
                    Globals.LocalMaps[6] = Globals.LocalMaps[3];
                    Globals.LocalMaps[3] = Globals.LocalMaps[0];
                    Globals.LocalMaps[0] = -1;
                    Globals.LocalMaps[7] = Globals.LocalMaps[4];
                    Globals.LocalMaps[4] = Globals.LocalMaps[1];
                    Globals.LocalMaps[1] = -1;
                    Globals.LocalMaps[8] = Globals.LocalMaps[5];
                    Globals.LocalMaps[5] = Globals.LocalMaps[2];
                    Globals.LocalMaps[2] = -1;
                    Globals.CurrentMap = Globals.LocalMaps[4];
                    CurrentMap = Globals.LocalMaps[4];
                    GameGraphics.DarkOffsetY = Globals.Database.TileHeight * Globals.Database.MapHeight;
                    PacketSender.SendEnterMap();
                }
            }
            GameGraphics.FogOffsetX = GameGraphics.DarkOffsetX;
            GameGraphics.FogOffsetY = GameGraphics.DarkOffsetY;
        }

        /// <summary>
        /// Returns -5 if the tile is completely out of bounds.
        /// Returns -4 if a tile is blocked because of a local event.
        /// Returns -3 if a tile is blocked because of a Z dimension tile
        /// Returns -2 if a tile does not exist or is blocked by a map attribute. 
        /// Returns -1 is a tile is passable.
        /// Returns any value zero or greater matching the entity index that is in the way.
        /// </summary>
        /// <returns></returns>
        public int IsTileBlocked(int x, int y, int z, int map)
        {
            var tmpI = -1;
            for (var i = 0; i < 9; i++)
            {
                if (Globals.LocalMaps[i] != map) continue;
                tmpI = i;
                i = 9;
            }
            if (tmpI == -1)
            {
                return -2;
            }
            try
            {
                int tmpX = x;
                int tmpY = y;
                int tmpMap;
                if (x < 0)
                {
                    tmpX = (Globals.Database.MapWidth - 1) - (x * -1);
                    if (y < 0)
                    {
                        tmpY = (Globals.Database.MapHeight - 1) - (y * -1);
                        tmpI = tmpI - 4;
                    }
                    else if (y > (Globals.Database.MapHeight - 1))
                    {
                        tmpY = y - (Globals.Database.MapHeight - 1);
                        tmpI = tmpI + 2;
                    }
                    else
                    {
                        tmpI = tmpI - 1;
                    }
                }
                else if (x > (Globals.Database.MapWidth - 1))
                {
                    tmpX = x - (Globals.Database.MapWidth - 1);
                    if (y < 0)
                    {
                        tmpY = (Globals.Database.MapHeight - 1) - (y * -1);
                        tmpI = tmpI - 2;
                    }
                    else if (y > (Globals.Database.MapHeight - 1))
                    {
                        tmpY = y - (Globals.Database.MapHeight - 1);
                        tmpI = tmpI + 4;
                    }
                    else
                    {
                        tmpI = tmpI + 1;
                    }
                }
                else if (y < 0)
                {
                    tmpY = (Globals.Database.MapHeight) - (y * -1);
                    tmpI = tmpI - 3;
                }
                else if (y > (Globals.Database.MapHeight - 1))
                {
                    tmpY = y - (Globals.Database.MapHeight - 1);
                    tmpI = tmpI + 3;
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                }

                if (Globals.LocalMaps[tmpI] > -1)
                {
                    if (Globals.GameMaps[Globals.LocalMaps[tmpI]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked && !NoClip)
                    {
                        return -2;
                    }
                    else if (Globals.GameMaps[Globals.LocalMaps[tmpI]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.ZDimension && !NoClip)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI]].Attributes[tmpX, tmpY].data2 - 1 == z)
                        {
                            return -3;
                        }
                    }
                    tmpMap = Globals.LocalMaps[tmpI];
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
                        if (en.Value.CurrentMap == tmpMap && en.Value.CurrentX == tmpX && en.Value.CurrentY == tmpY && !NoClip)
                        {
                            if (en.Value.GetType() != typeof(Projectile)) return en.Key;
                        }
                    }
                }
                if (Globals.GameMaps.ContainsKey(tmpMap))
                {
                    foreach (var en in Globals.GameMaps[tmpMap].LocalEntities)
                    {
                        if (en.Value == null) continue;
                        if (en.Value.CurrentMap == tmpMap && en.Value.CurrentX == tmpX && en.Value.CurrentY == tmpY && en.Value.Passable == 0 && !NoClip)
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
    }

    public class HotbarInstance
    {
        public int Type = -1;
        public int Slot = -1;
    }


}
