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

using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

namespace Intersect_Client.Classes.Core
{
    public static class GameInputHandler
    {
        public static void OnKeyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                if (Globals.GameState == GameStates.Intro)
                {
                    GameFade.FadeIn();
                    Globals.GameState = GameStates.Menu;
                }
            }
            else if (key == Keys.Insert)
            {
                //Try to open admin panel!
                if (Globals.GameState == GameStates.InGame)
                {
                    PacketSender.SendOpenAdminWindow();
                }
            }
            else if (key == Keys.F2)
            {
                Gui.GameUI.ShowHideDebug();
            }
            else if (key == Keys.Enter || key == Keys.Return)
            {
                if (Globals.GameState != GameStates.InGame) return;
                if (!Gui.HasInputFocus())
                {
                    Gui.GameUI.FocusChat = true;
                }
            }
            else if (key >= Keys.D1 && key <= Keys.D9)
            {
                if (Globals.GameState != GameStates.InGame) return;
                if (!Gui.HasInputFocus())
                {
                    Gui.GameUI.Hotbar.Items[((int) key - (int) Keys.D1)].Activate();
                }
            }
            else if (key == Keys.D0)
            {
                if (Globals.GameState != GameStates.InGame) return;
                if (!Gui.HasInputFocus())
                {
                    Gui.GameUI.Hotbar.Items[9].Activate();
                }
            }
        }

        public static void OnKeyReleased(Keys key)
        {
            
        }

        public static void OnMouseDown(GameInput.MouseButtons btn)
        {
            if (btn == GameInput.MouseButtons.Left)
            {
                if (Globals.GameState == GameStates.InGame && Globals.Me != null)
                {
                    if (!Gui.MouseHitGUI())
                    {
                        if (Globals.Me.TryTarget())
                        {
                            return;
                        }
                        if (Globals.Me.TryAttack())
                        {
                            return;
                        }
                        if (Globals.Me.TryPickupItem())
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
