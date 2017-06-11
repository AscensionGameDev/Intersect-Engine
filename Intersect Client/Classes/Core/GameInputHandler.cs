using System;
using Intersect;
using Intersect.Enums;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;
using Intersect.Client.Classes.Core;

namespace Intersect_Client.Classes.Core
{
    public static class GameInputHandler
    {
        public delegate void HandleKeyEvent(Keys key);
        public static HandleKeyEvent KeyDown;
        public static HandleKeyEvent KeyUp;
        public static HandleKeyEvent MouseDown;
        public static HandleKeyEvent MouseUp;
        public static void OnKeyPressed(Keys key)
        {
            if (KeyDown != null) KeyDown(key);
            if (!Gui.HasInputFocus())
            {
                if (GameControls.ControlHasKey(Controls.AttackInteract, key))
                {
                    if (Globals.Me != null)
                    {
                        Globals.Me.TryAttack();
                        if (GameControls.ControlHasKey(Controls.PickUp, key) && Globals.Me.TryPickupItem())
                        {
                            return;
                        }
                        if (Globals.Me.AttackTimer < Globals.System.GetTimeMS())
                            Globals.Me.AttackTimer = Globals.System.GetTimeMS() + Globals.Me.CalculateAttackTime();
                    }
                }
                else if (GameControls.ControlHasKey(Controls.PickUp, key))
                {
                    if (Globals.Me != null)
                    {
                        if (Globals.Me.TryPickupItem())
                        {
                            return;
                        }
                    }
                }
                else if (GameControls.ControlHasKey(Controls.Block, key))
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
                else if (GameControls.ControlHasKey(Controls.Enter, key))
                {
                    if (Globals.GameState != GameStates.InGame) return;
                    if (!Gui.HasInputFocus())
                    {
                        Gui.GameUI.FocusChat = true;
                    }
                }
                if (Globals.GameState != GameStates.InGame) return;
                if (!Gui.HasInputFocus())
                {
                    if (GameControls.ControlHasKey(Controls.Hotkey0, key))
                    {
                        Gui.GameUI.Hotbar.Items[9].Activate();
                    }
                    for (var i = Controls.Hotkey1; i <= Controls.Hotkey9; i++)
                    {
                        if (GameControls.ControlHasKey(i, key))
                        {
                            var index = (int)(i - Controls.Hotkey1);
                            Gui.GameUI.Hotbar.Items[index].Activate();
                        }
                    }
                }
            }
        }

        public static void OnKeyReleased(Keys key)
        {
            if (KeyUp != null) KeyUp(key);
            if (!Gui.HasInputFocus())
            {
                if (Globals.Me != null)
                {
                    if (GameControls.ControlHasKey(Controls.Block, key))
                    {
                        Globals.Me.StopBlocking();
                    }
                }
            }
        }

        public static void OnMouseDown(GameInput.MouseButtons btn)
        {
            var key = Keys.LButton;
            if (btn == GameInput.MouseButtons.Right)
            {
                key = Keys.RButton;
            }
            if (btn == GameInput.MouseButtons.Middle)
            {
                key = Keys.MButton;
            }
            if (MouseDown != null) MouseDown(key);
            if (!Gui.HasInputFocus())
            {
                if (Globals.GameState == GameStates.InGame && Globals.Me != null)
                {
                    if (!Gui.MouseHitGUI())
                    {
                        if (Globals.Me != null)
                        {
                            if (Globals.Me.TryTarget())
                            {
                                return;
                            }
                            if (GameControls.ControlHasKey(Controls.AttackInteract, key))
                            {
                                if (Globals.Me.TryAttack())
                                {
                                    return;
                                }
                                if (Globals.Me.AttackTimer < Globals.System.GetTimeMS())
                                    Globals.Me.AttackTimer = Globals.System.GetTimeMS() +
                                                             Globals.Me.CalculateAttackTime();
                            }
                            if (GameControls.ControlHasKey(Controls.PickUp, key))
                            {
                                if (Globals.Me.TryPickupItem())
                                {
                                    return;
                                }
                                if (Globals.Me.AttackTimer < Globals.System.GetTimeMS())
                                    Globals.Me.AttackTimer = Globals.System.GetTimeMS() +
                                                             Globals.Me.CalculateAttackTime();
                            }
                            if (GameControls.ControlHasKey(Controls.Block, key))
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
            var key = Keys.LButton;
            if (btn == GameInput.MouseButtons.Right)
            {
                key = Keys.RButton;
            }
            if (btn == GameInput.MouseButtons.Middle)
            {
                key = Keys.MButton;
            }
            if (MouseUp != null) MouseUp(key);
            if (!Gui.HasInputFocus())
            {
                if (Globals.Me != null)
                {
                    if (GameControls.ControlHasKey(Controls.Block,key))
                    {
                        Globals.Me.StopBlocking();
                    }
                    if (btn == GameInput.MouseButtons.Right)
                    {
                        if (Globals.InputManager.KeyDown(Keys.Shift) == true)
                        {
                            var x =
                                (int)
                                Math.Floor(Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left);
                            var y =
                                (int)
                                Math.Floor(Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top);

                            foreach (MapInstance map in MapInstance.Lookup.Values)
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
                                        int mapNum = map.Index;

                                        if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                                        {
                                            PacketSender.SendAdminAction((int)AdminActions.WarpToLoc,
                                                Convert.ToString(mapNum), Convert.ToString(x), Convert.ToString(y));
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