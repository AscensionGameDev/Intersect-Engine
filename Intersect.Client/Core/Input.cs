using System;

using Intersect.Admin.Actions;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Logging;

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
            if (key == Keys.None) return;
            var consumeKey = false;

            KeyDown?.Invoke(key);
            switch (key)
            {
                case Keys.Escape:
                    if (Globals.GameState != GameStates.Intro) break;
                    Fade.FadeIn();
                    Globals.GameState = GameStates.Menu;
                    return;
            }

            if (Controls.Controls.ControlHasKey(Control.OpenMenu, key))
            {
                if (Globals.GameState != GameStates.InGame) return;
                Gui.GameUi?.EscapeMenu?.ToggleHidden();
            }
            
            if (Gui.HasInputFocus())
            {
                return;
            }

            Controls.Controls.GetControlsFor(key)?.ForEach(control =>
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
                            Gui.HideUi = !Gui.HideUi;
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
                                Globals.Me?.TryPickupItem();
                                break;

                            case Control.Enter:
                                Gui.GameUi.FocusChat = true;
                                consumeKey = true;
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
                                var index = control - Control.Hotkey1;
                                if (0 <= index && index < Gui.GameUi?.Hotbar?.Items?.Count)
                                {
                                    Gui.GameUi?.Hotbar?.Items?[index]?.Activate();
                                }
                                else
                                {
                                    Log.Warn(Gui.GameUi?.Hotbar?.Items == null
                                        ? $"Tried to press Hotkey{(index + 1) % 10} but the hotbar items are null."
                                        : $"Tried to press Hotkey{(index + 1) % 10} which was out of bounds ({control}).");
                                }
                                break;

                            case Control.OpenInventory:
                                Gui.GameUi?.GameMenu?.ToggleInventoryWindow();
                                break;

                            case Control.OpenQuests:
                                Gui.GameUi?.GameMenu?.ToggleQuestsWindow();
                                break;

                            case Control.OpenCharacterInfo:
                                Gui.GameUi?.GameMenu?.ToggleCharacterWindow();
                                break;

                            case Control.OpenParties:
                                Gui.GameUi?.GameMenu?.TogglePartyWindow();
                                break;

                            case Control.OpenSpells:
                                Gui.GameUi?.GameMenu?.ToggleSpellsWindow();
                                break;

                            case Control.OpenFriends:
                                Gui.GameUi?.GameMenu?.ToggleFriendsWindow();
                                break;

                            case Control.OpenSettings:
                                Gui.GameUi?.EscapeMenu?.OpenSettings();
                                break;

                            case Control.OpenDebugger:
                                Gui.GameUi?.ShowHideDebug();
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
                        throw new ArgumentOutOfRangeException(nameof(Globals.GameState), Globals.GameState, null);
                }
            });
        }

        public static void OnKeyReleased(Keys key)
        {
            KeyUp?.Invoke(key);
            if (Gui.HasInputFocus()) return;
            if (Globals.Me == null) return;
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
            if (Gui.HasInputFocus()) return;
            if (Globals.GameState != GameStates.InGame || Globals.Me == null) return;
            if (Gui.MouseHitGui()) return;
            if (Globals.Me == null) return;
            if (Globals.Me.TryTarget()) return;
            if (Controls.Controls.ControlHasKey(Control.PickUp, key))
            {
                if (Globals.Me.TryPickupItem()) return;
                if (Globals.Me.AttackTimer < Globals.System.GetTimeMs())
                {
                    Globals.Me.AttackTimer = Globals.System.GetTimeMs() + Globals.Me.CalculateAttackTime();
                }
            }

            if (!Controls.Controls.ControlHasKey(Control.Block, key)) return;
            if (Globals.Me.TryBlock()) return;

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
            if (Gui.HasInputFocus()) return;
            if (Globals.Me == null) return;
            if (Controls.Controls.ControlHasKey(Control.Block, key))
            {
                Globals.Me.StopBlocking();
            }
            if (btn != GameInput.MouseButtons.Right) return;
            if (Globals.InputManager.KeyDown(Keys.Shift) != true) return;
            var x = (int)Math.Floor(Globals.InputManager.GetMousePosition().X + Graphics.CurrentView.Left);
            var y = (int)Math.Floor(Globals.InputManager.GetMousePosition().Y + Graphics.CurrentView.Top);

            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (!(x >= map.GetX()) || !(x <= map.GetX() + (Options.MapWidth * Options.TileWidth))) continue;
                if (!(y >= map.GetY()) || !(y <= map.GetY() + (Options.MapHeight * Options.TileHeight))) continue;
                //Remove the offsets to just be dealing with pixels within the map selected
                x -= (int)map.GetX();
                y -= (int)map.GetY();

                //transform pixel format to tile format
                x /= Options.TileWidth;
                y /= Options.TileHeight;
                var mapNum = map.Id;

                if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                {
                    PacketSender.SendAdminAction(new WarpToLocationAction(map.Id,(byte)x,(byte)y));
                }
                return;
            }
        }
    }
}