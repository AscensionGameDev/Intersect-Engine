using System;

using Intersect.Admin.Actions;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Logging;
using Intersect.Utilities;

namespace Intersect.Client.Core
{

    public static class Input
    {

        public delegate void HandleKeyEvent(Keys key);

        public static HandleKeyEvent KeyDown;

        public static HandleKeyEvent KeyUp;

        public static HandleKeyEvent MouseDown;

        public static HandleKeyEvent MouseUp;

        public static void OnKeyPressed(Keys key)
        {
            if (key == Keys.None)
            {
                return;
            }

            var consumeKey = false;
            bool canFocusChat = true;

            KeyDown?.Invoke(key);
            switch (key)
            {
                case Keys.Escape:
                    if (Globals.GameState != GameStates.Intro)
                    {
                        break;
                    }

                    Fade.FadeIn();
                    Globals.GameState = GameStates.Menu;

                    return;

                case Keys.Enter:

                    for (int i = Interface.Interface.InputBlockingElements.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            var iBox = (InputBox)Interface.Interface.InputBlockingElements[i];
                            if (iBox != null && !iBox.IsHidden)
                            {
                                iBox.okayBtn_Clicked(null, null);
                                canFocusChat = false;

                                break;
                            }
                        }
                        catch { }

                        try
                        {
                            var eventWindow = (EventWindow)Interface.Interface.InputBlockingElements[i];
                            if (eventWindow != null && !eventWindow.IsHidden && Globals.EventDialogs.Count > 0)
                            {
                                eventWindow.EventResponse1_Clicked(null, null);
                                canFocusChat = false;

                                break;
                            }
                        }
                        catch { }
                    }

                    break;
            }

            if (Controls.Controls.ControlHasKey(Control.OpenMenu, key))
            {
                if (Globals.GameState != GameStates.InGame)
                {
                    return;
                }

                // First try and unfocus chat then close all UI elements, then untarget our target.. and THEN open the escape menu.
                // Most games do this, why not this?
                if (Interface.Interface.GameUi != null && Interface.Interface.GameUi.ChatFocussed)
                {
                    Interface.Interface.GameUi.UnfocusChat = true;
                }
                else if (Interface.Interface.GameUi != null && Interface.Interface.GameUi.CloseAllWindows())
                {
                    // We've closed our windows, don't do anything else. :)
                }
                else if (Globals.Me != null && Globals.Me.TargetIndex != Guid.Empty)
                {
                    Globals.Me.ClearTarget();
                }
                else
                {
                    Interface.Interface.GameUi?.EscapeMenu?.ToggleHidden();
                }
            }

            if (Interface.Interface.HasInputFocus())
            {
                return;
            }

            Controls.Controls.GetControlsFor(key)
                ?.ForEach(
                    control =>
                    {
                        if (consumeKey)
                        {
                            return;
                        }

                        switch (control)
                        {
                            case Control.Screenshot:
                                Graphics.Renderer?.RequestScreenshot();

                                break;

                            case Control.ToggleGui:
                                if (Globals.GameState == GameStates.InGame)
                                {
                                    Interface.Interface.HideUi = !Interface.Interface.HideUi;
                                }

                                break;
                        }

                        switch (Globals.GameState)
                        {
                            case GameStates.Intro:
                                break;

                            case GameStates.Menu:
                                break;

                            case GameStates.InGame:
                                switch (control)
                                {
                                    case Control.MoveUp:
                                        break;

                                    case Control.MoveLeft:
                                        break;

                                    case Control.MoveDown:
                                        break;

                                    case Control.MoveRight:
                                        break;

                                    case Control.AttackInteract:
                                        break;

                                    case Control.Block:
                                        Globals.Me?.TryBlock();

                                        break;

                                    case Control.AutoTarget:
                                        Globals.Me?.AutoTarget();

                                        break;

                                    case Control.PickUp:
                                        Globals.Me?.TryPickupItem(Globals.Me.MapInstance.Id, Globals.Me.Y * Options.MapWidth + Globals.Me.X);

                                        break;

                                    case Control.Enter:
                                        if (canFocusChat)
                                        {
                                            Interface.Interface.GameUi.FocusChat = true;
                                            consumeKey = true;
                                        }

                                        return;

                                    case Control.Hotkey1:
                                    case Control.Hotkey2:
                                    case Control.Hotkey3:
                                    case Control.Hotkey4:
                                    case Control.Hotkey5:
                                    case Control.Hotkey6:
                                    case Control.Hotkey7:
                                    case Control.Hotkey8:
                                    case Control.Hotkey9:
                                    case Control.Hotkey0:
                                        break;

                                    case Control.OpenInventory:
                                        Interface.Interface.GameUi?.GameMenu?.ToggleInventoryWindow();

                                        break;

                                    case Control.OpenQuests:
                                        Interface.Interface.GameUi?.GameMenu?.ToggleQuestsWindow();

                                        break;

                                    case Control.OpenCharacterInfo:
                                        Interface.Interface.GameUi?.GameMenu?.ToggleCharacterWindow();

                                        break;

                                    case Control.OpenParties:
                                        Interface.Interface.GameUi?.GameMenu?.TogglePartyWindow();

                                        break;

                                    case Control.OpenSpells:
                                        Interface.Interface.GameUi?.GameMenu?.ToggleSpellsWindow();

                                        break;

                                    case Control.OpenFriends:
                                        Interface.Interface.GameUi?.GameMenu?.ToggleFriendsWindow();

                                        break;

                                    case Control.OpenSettings:
                                        Interface.Interface.GameUi?.EscapeMenu?.OpenSettings();

                                        break;

                                    case Control.OpenDebugger:
                                        Interface.Interface.GameUi?.ShowHideDebug();

                                        break;

                                    case Control.OpenAdminPanel:
                                        PacketSender.SendOpenAdminWindow();

                                        break;
                                }

                                break;

                            case GameStates.Loading:
                                break;

                            case GameStates.Error:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(
                                    nameof(Globals.GameState), Globals.GameState, null
                                );
                        }
                    }
                );
        }

        public static void OnKeyReleased(Keys key)
        {
            KeyUp?.Invoke(key);
            if (Interface.Interface.HasInputFocus())
            {
                return;
            }

            if (Globals.Me == null)
            {
                return;
            }

            if (Controls.Controls.ControlHasKey(Control.Block, key))
            {
                Globals.Me.StopBlocking();
            }
        }

        public static void OnMouseDown(GameInput.MouseButtons btn)
        {
            var key = Keys.None;
            switch (btn)
            {
                case GameInput.MouseButtons.Left:
                    key = Keys.LButton;

                    break;

                case GameInput.MouseButtons.Right:
                    key = Keys.RButton;

                    break;

                case GameInput.MouseButtons.Middle:
                    key = Keys.MButton;

                    break;
            }

            MouseDown?.Invoke(key);
            if (Interface.Interface.HasInputFocus())
            {
                return;
            }

            if (Globals.GameState != GameStates.InGame || Globals.Me == null)
            {
                return;
            }

            if (Interface.Interface.MouseHitGui())
            {
                return;
            }

            if (Globals.Me == null)
            {
                return;
            }

            if (Globals.Me.TryTarget())
            {
                return;
            }

            if (Controls.Controls.ControlHasKey(Control.PickUp, key))
            {
                if (Globals.Me.TryPickupItem(Globals.Me.MapInstance.Id, Globals.Me.Y * Options.MapWidth + Globals.Me.X, Guid.Empty, true))
                {
                    return;
                }

                if (Globals.Me.AttackTimer < Timing.Global.Ticks / TimeSpan.TicksPerMillisecond)
                {
                    Globals.Me.AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + Globals.Me.CalculateAttackTime();
                }
            }

            if (Controls.Controls.ControlHasKey(Control.Block, key))
            {
                if (Globals.Me.TryBlock())
                {
                    return;
                }
            }

            if (key != Keys.None)
            {
                OnKeyPressed(key);
            }
        }

        public static void OnMouseUp(GameInput.MouseButtons btn)
        {
            var key = Keys.LButton;
            switch (btn)
            {
                case GameInput.MouseButtons.Right:
                    key = Keys.RButton;

                    break;

                case GameInput.MouseButtons.Middle:
                    key = Keys.MButton;

                    break;
            }

            MouseUp?.Invoke(key);
            if (Interface.Interface.HasInputFocus())
            {
                return;
            }

            if (Globals.Me == null)
            {
                return;
            }

            if (Controls.Controls.ControlHasKey(Control.Block, key))
            {
                Globals.Me.StopBlocking();
            }

            if (btn != GameInput.MouseButtons.Right)
            {
                return;
            }

            if (Globals.InputManager.KeyDown(Keys.Shift) != true)
            {
                return;
            }

            var x = (int) Math.Floor(Globals.InputManager.GetMousePosition().X + Graphics.CurrentView.Left);
            var y = (int) Math.Floor(Globals.InputManager.GetMousePosition().Y + Graphics.CurrentView.Top);

            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (!(x >= map.GetX()) || !(x <= map.GetX() + Options.MapWidth * Options.TileWidth))
                {
                    continue;
                }

                if (!(y >= map.GetY()) || !(y <= map.GetY() + Options.MapHeight * Options.TileHeight))
                {
                    continue;
                }

                //Remove the offsets to just be dealing with pixels within the map selected
                x -= (int) map.GetX();
                y -= (int) map.GetY();

                //transform pixel format to tile format
                x /= Options.TileWidth;
                y /= Options.TileHeight;
                var mapNum = map.Id;

                if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                {
                    PacketSender.SendAdminAction(new WarpToLocationAction(map.Id, (byte) x, (byte) y));
                }

                return;
            }
        }

    }

}
