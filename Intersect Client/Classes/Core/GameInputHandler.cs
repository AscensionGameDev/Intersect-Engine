using System;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.Maps;
using Intersect;

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
                        Globals.Me.TryAttack();
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
                    if (Globals.GameState != GameStates.InGame) return;
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
                                        int mapNum = map.Id;

                                        if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                                        {
                                            PacketSender.SendAdminAction((int)AdminActions.WarpToLoc, Convert.ToString(mapNum), Convert.ToString(x), Convert.ToString(y));
                                        }
                                        return;
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
