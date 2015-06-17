using Intersect_Client.Classes.UI.Game;
using SFML.Window;
using System;
namespace Intersect_Client.Classes
{
    public class Player : Entity
    {
        private long _attackTimer;
        public ItemInstance[] Inventory = new ItemInstance[Constants.MaxInvItems];
        public SpellInstance[] Spells = new SpellInstance[Constants.MaxPlayerSkills];

        public Player()
            : base()
        {
            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                Inventory[i] = new ItemInstance();
            }
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
            {
                Spells[i] = new SpellInstance();
            }
        }

        public void Update(bool local = false)
        {
            base.Update();
            HandleInput();
            if (Globals.MyIndex == MyIndex && base.IsMoving == false) { ProcessDirectionalInput(); }
            if (Graphics.MouseState.IndexOf(Mouse.Button.Left) >= 0 || Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                if (TryAttack()) { return; }
                if (TryPickupItem()) { return; }
            }

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
                    InputBox iBox = new InputBox("Drop Item", "How many/much " + Globals.GameItems[Inventory[index].ItemNum].Name + " do you want to drop?", true, DropItemInputBoxOkay, null, index,true);
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
                InputBox iBox = new InputBox("Forget Spell", "Are you sure you want to forget " + Globals.GameSpells[Spells[index].SpellNum].Name + "?", true, ForgetSpellInputBoxOkay, null, index,false);
            }
        }
        private void ForgetSpellInputBoxOkay(Object sender, EventArgs e)
        {
            PacketSender.SendForgetSpell(((InputBox)sender).Slot);
        }
        public void TryUseSpell(int index)
        {
            PacketSender.SendUseSpell(index);
        }


        //Input Handling
        private void HandleInput()
        {
            var movex = 0f;
            var movey = 0f;
            if (Gui.HasInputFocus()) { return; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.W) >= 0) { movey = 1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.S) >= 0) { movey = -1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.A) >= 0) { movex = -1; }
            if (Graphics.KeyStates.IndexOf(Keyboard.Key.D) >= 0) { movex = 1; }
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
            if (_attackTimer > Environment.TickCount) { return false; }
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
                    tmpX = 29 - (x * -1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 4] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 4];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
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
                else if (x > 29)
                {
                    tmpX = x - 29;
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 2] > -1)
                        {
                            tmpMap = Globals.LocalMaps[tmpI - 2];
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
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
                    tmpY = 29 - (y * -1);
                    if (Globals.LocalMaps[tmpI - 3] > -1)
                    {
                        tmpMap = Globals.LocalMaps[tmpI - 3];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (y > 29)
                {
                    tmpX = x;
                    tmpY = y - 29;
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
                for (var i = 0; i < Globals.Entities.Count; i++)
                {
                    if (i != Globals.MyIndex)
                    {
                        if (Globals.Entities[i] != null)
                        {
                            if (Globals.Entities[i].CurrentMap == tmpMap && Globals.Entities[i].CurrentX == tmpX && Globals.Entities[i].CurrentY == tmpY)
                            {
                                //ATTACKKKKK!!!
                                PacketSender.SendAttack(i);
                                _attackTimer = Environment.TickCount + 1000;
                                return true;
                            }
                        }
                    }
                }
                for (var i = 0; i < Globals.Events.Count; i++)
                {
                    if (Globals.Events[i] != null)
                    {
                        if (Globals.Events[i].CurrentMap == tmpMap && Globals.Events[i].CurrentX == tmpX && Globals.Events[i].CurrentY == tmpY)
                        {
                            //Talk to Event
                            PacketSender.SendActivateEvent(i);
                            _attackTimer = Environment.TickCount + 1000;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;

            }
        }
        private bool TryPickupItem()
        {
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
                //Try to move
                if (MoveTimer < Environment.TickCount)
                {
                    switch (MoveDir)
                    {
                        case 0:
                            if (!IsTileBlocked(CurrentX, CurrentY - 1, CurrentMap))
                            {
                                CurrentY--;
                                Dir = 0;
                                IsMoving = true;
                                OffsetY = 32;
                                OffsetX = 0;
                            }
                            break;
                        case 1:
                            if (!IsTileBlocked(CurrentX, CurrentY + 1, CurrentMap))
                            {
                                CurrentY++;
                                Dir = 1;
                                IsMoving = true;
                                OffsetY = -32;
                                OffsetX = 0;
                            }
                            break;
                        case 2:
                            if (!IsTileBlocked(CurrentX - 1, CurrentY, CurrentMap))
                            {
                                CurrentX--;
                                Dir = 2;
                                IsMoving = true;
                                OffsetY = 0;
                                OffsetX = 32;
                            }
                            break;
                        case 3:
                            if (!IsTileBlocked(CurrentX + 1, CurrentY, CurrentMap))
                            {
                                CurrentX++;
                                Dir = 3;
                                IsMoving = true;
                                OffsetY = 0;
                                OffsetX = -32;
                            }
                            break;
                    }

                    if (IsMoving)
                    {
                        MoveTimer = Environment.TickCount + (Stat[(int)Enums.Stats.Speed] / 10f);
                        didMove = true;
                        if (CurrentX < 0 || CurrentY < 0 || CurrentX > 29 || CurrentY > 29)
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
                                                CurrentX = 29;
                                                CurrentY = 29;
                                                UpdateMapRenderers(0);
                                                UpdateMapRenderers(2);
                                            }
                                        }
                                        else if (CurrentY > 29)
                                        {
                                            if (Globals.LocalMaps[tmpI + 2] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI + 2];
                                                CurrentX = 29;
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
                                                CurrentX = 29;
                                                UpdateMapRenderers(2);
                                            }
                                        }

                                    }
                                    else if (CurrentX > 29)
                                    {
                                        if (CurrentY < 0)
                                        {
                                            if (Globals.LocalMaps[tmpI - 2] > -1)
                                            {
                                                CurrentMap = Globals.LocalMaps[tmpI - 2];
                                                CurrentX = 0;
                                                CurrentY = 29;
                                                UpdateMapRenderers(0);
                                                UpdateMapRenderers(3);
                                            }
                                        }
                                        else if (CurrentY > 29)
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
                                            CurrentY = 29;
                                            CurrentMap = Globals.LocalMaps[tmpI - 3];
                                            UpdateMapRenderers(0);
                                        }
                                    }
                                    else if (CurrentY > 29)
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
                    Graphics.NightOffsetX = 32 * Constants.MapWidth;
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
                    Graphics.NightOffsetX = -32 * Constants.MapWidth;
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
                    Graphics.NightOffsetY = -32 * Constants.MapHeight;
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
                    Graphics.NightOffsetY = 32 * Constants.MapHeight;
                    PacketSender.SendEnterMap();
                }
            }
        }
        bool IsTileBlocked(int x, int y, int map)
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
                return true;
            }
            try
            {
                int tmpX;
                int tmpY;
                int tmpMap;
                if (x < 0)
                {
                    tmpX = 29 - (x * -1);
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 4] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 4]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 2] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 2]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI - 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (x > 29)
                {
                    tmpX = x - 29;
                    tmpY = y;
                    if (y < 0)
                    {
                        tmpY = 29 - (y * -1);
                        if (Globals.LocalMaps[tmpI - 2] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI - 2]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI - 2];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (y > 29)
                    {
                        tmpY = y - 29;
                        if (Globals.LocalMaps[tmpI + 4] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 4]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 4];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Globals.LocalMaps[tmpI + 1] > -1)
                        {
                            if (Globals.GameMaps[Globals.LocalMaps[tmpI + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return true;
                            }
                            tmpMap = Globals.LocalMaps[tmpI + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (y < 0)
                {
                    tmpX = x;
                    tmpY = 29 - (y * -1);
                    if (Globals.LocalMaps[tmpI - 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI - 3]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;

                        }
                        tmpMap = Globals.LocalMaps[tmpI - 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (y > 29)
                {
                    tmpX = x;
                    tmpY = y - 29;
                    if (Globals.LocalMaps[tmpI + 3] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI + 3]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;
                        }
                        tmpMap = Globals.LocalMaps[tmpI + 3];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    tmpX = x;
                    tmpY = y;
                    if (Globals.LocalMaps[tmpI] > -1)
                    {
                        if (Globals.GameMaps[Globals.LocalMaps[tmpI]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return true;
                        }
                        tmpMap = Globals.LocalMaps[tmpI];
                    }
                    else
                    {
                        return true;
                    }
                }
                for (var i = 0; i < Globals.Entities.Count; i++)
                {
                    if (i == MyIndex) continue;
                    if (Globals.Entities[i] == null) continue;
                    if (Globals.Entities[i].CurrentMap == tmpMap && Globals.Entities[i].CurrentX == tmpX && Globals.Entities[i].CurrentY == tmpY && Globals.Entities[i].Passable == 0)
                    {
                        return true;
                    }
                }
                for (var i = 0; i < Globals.Events.Count; i++)
                {
                    if (Globals.Events[i] != null)
                    {
                        if (Globals.Events[i].CurrentMap == tmpMap && Globals.Events[i].CurrentX == tmpX && Globals.Events[i].CurrentY == tmpY && Globals.Events[i].Passable == 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return true;

            }

        }
    }


}
