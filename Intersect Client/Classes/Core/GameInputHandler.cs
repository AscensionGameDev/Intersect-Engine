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
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.Maps;
using Intersect_Library;

namespace Intersect_Client.Classes.Core
{
    public static class GameInputHandler
    {

        public static void OnKeyPressed(Keys key)
        {
            if (!Gui.HasInputFocus())
            {
                if (key == Keys.E)
                {
                    if (Globals.Me != null)
                    {
                        if (Globals.Me.TryAttack())
                        {
                            return;
                        }
                        if (Globals.Me.TryPickupItem())
                        {
                            return;
                        }
                        if (Globals.Me.AttackTimer < Globals.System.GetTimeMS())
                            Globals.Me.AttackTimer = Globals.System.GetTimeMS() + Globals.Me.CalculateAttackTime();
                    }
                }
                else if (key == Keys.Q)
                {
                    if (Globals.Me != null)
                    {
                        if (Globals.Me.TryBlock())
                        {
                            return;
                        }
                    }
                }
                else if (key == Keys.Escape)
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
                        Gui.GameUI.Hotbar.Items[((int)key - (int)Keys.D1)].Activate();
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
        }

        public static void OnKeyReleased(Keys key)
        {
            if (!Gui.HasInputFocus())
            {
                if (Globals.Me != null)
                {
                    if (key == Keys.Q)
                    {
                        Globals.Me.StopBlocking();
                    }
                }
            }
        }

        public static void OnMouseDown(GameInput.MouseButtons btn)
        {
            if (!Gui.HasInputFocus())
            {
                if (Globals.GameState == GameStates.InGame && Globals.Me != null)
                {
                    if (btn == GameInput.MouseButtons.Left)
                    {
                        if (!Gui.MouseHitGUI())
                        {
                            if (Globals.Me != null)
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
                                if (Globals.Me.AttackTimer < Globals.System.GetTimeMS())
                                    Globals.Me.AttackTimer = Globals.System.GetTimeMS() + Globals.Me.CalculateAttackTime();
                            }
                        }
                    }
                    else if (btn == GameInput.MouseButtons.Right)
                    {
                        if (!Gui.MouseHitGUI())
                        {
                            if (Globals.Me != null)
                            {
                                if (Globals.Me.TryBlock())
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void OnMouseUp(GameInput.MouseButtons btn)
        {
            if (!Gui.HasInputFocus())
            {
                if (Globals.Me != null)
                {
                    if (btn == GameInput.MouseButtons.Right)
                    {
                        Globals.Me.StopBlocking();
                        if (Globals.InputManager.KeyDown(Keys.Shift) == true)
                        {
                            var x = (int)Math.Floor(Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left);
                            var y = (int)Math.Floor(Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top);

                            foreach (var map in MapInstance.GetObjects().Values)
                            {
                                if (x >= map.GetX() && x <= map.GetX() + (Options.MapWidth * Options.TileWidth))
                                {
                                    if (y >= map.GetY() && y <= map.GetY() + (Options.MapHeight * Options.TileHeight))
                                    {
                                        //Remove the offsets to just be dealing with pixels within the map selected
                                        x -= (int)map.GetX();
                                        y -= (int)map.GetY();

                                        //transform pixel format to tile format
                                        x /= Options.TileWidth;
                                        y /= Options.TileHeight;
                                        int mapNum = map.MyMapNum;

                                        if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                                        {
                                            PacketSender.SendAdminAction((int)AdminActions.WarpToLoc, Convert.ToString(mapNum), Convert.ToString(x), Convert.ToString(y));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
