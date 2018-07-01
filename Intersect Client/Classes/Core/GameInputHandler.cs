using System;
using Intersect.Enums;
using Intersect.Logging;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

namespace Intersect.Client.Classes.Core
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
            var consumeKey = false;

            KeyDown?.Invoke(key);
            switch (key)
            {
                case Keys.Escape:
                    if (Globals.GameState != GameStates.Intro) break;
                    GameFade.FadeIn();
                    Globals.GameState = GameStates.Menu;
                    return;
            }

            if (GameControls.ControlHasKey(Controls.OpenMenu, key))
            {
                if (Globals.GameState != GameStates.InGame) return;
                Gui.GameUi?.IngameMenu?.ToggleHidden();
            }
            
            if (Gui.HasInputFocus())
            {
                return;
            }

            GameControls.GetControlsFor(key)?.ForEach(control =>
            {
                if (consumeKey)
                {
                    return;
                }

                switch (control)
                {
                    case Controls.Screenshot:
                        GameGraphics.Renderer?.RequestScreenshot();
                        break;

                    case Controls.ToggleGui:
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
                            case Controls.MoveUp:
                                break;

                            case Controls.MoveLeft:
                                break;

                            case Controls.MoveDown:
                                break;

                            case Controls.MoveRight:
                                break;

                            case Controls.AttackInteract:
                                break;

                            case Controls.Block:
                                Globals.Me?.TryBlock();
                                break;

                            case Controls.AutoTarget:
                                Globals.Me?.AutoTarget();
                                break;

                            case Controls.PickUp:
                                Globals.Me?.TryPickupItem();
                                break;

                            case Controls.Enter:
                                Gui.GameUi.FocusChat = true;
                                consumeKey = true;
                                return;

                            case Controls.Hotkey1:
                            case Controls.Hotkey2:
                            case Controls.Hotkey3:
                            case Controls.Hotkey4:
                            case Controls.Hotkey5:
                            case Controls.Hotkey6:
                            case Controls.Hotkey7:
                            case Controls.Hotkey8:
                            case Controls.Hotkey9:
                            case Controls.Hotkey0:
                                var index = control - Controls.Hotkey1;
                                if (0 < index && index < Gui.GameUi?.Hotbar?.Items?.Count)
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

                            case Controls.OpenInventory:
                                Gui.GameUi?.GameMenu?.ToggleInventoryWindow();
                                break;

                            case Controls.OpenQuests:
                                Gui.GameUi?.GameMenu?.ToggleQuestsWindow();
                                break;

                            case Controls.OpenCharacterInfo:
                                Gui.GameUi?.GameMenu?.ToggleCharacterWindow();
                                break;

                            case Controls.OpenParties:
                                Gui.GameUi?.GameMenu?.TogglePartyWindow();
                                break;

                            case Controls.OpenSpells:
                                Gui.GameUi?.GameMenu?.ToggleSpellsWindow();
                                break;

                            case Controls.OpenFriends:
                                Gui.GameUi?.GameMenu?.ToggleFriendsWindow();
                                break;

                            case Controls.OpenSettings:
                                break;

                            case Controls.OpenDebugger:
                                Gui.GameUi?.ShowHideDebug();
                                break;

                            case Controls.OpenAdminPanel:
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
            if (GameControls.ControlHasKey(Controls.Block, key))
            {
                Globals.Me.StopBlocking();
            }
        }

        public static void OnMouseDown(GameInput.MouseButtons btn)
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

            MouseDown?.Invoke(key);
            if (Gui.HasInputFocus()) return;
            if (Globals.GameState != GameStates.InGame || Globals.Me == null) return;
            if (Gui.MouseHitGui()) return;
            if (Globals.Me == null) return;
            if (Globals.Me.TryTarget()) return;
            if (GameControls.ControlHasKey(Controls.PickUp, key))
            {
                if (Globals.Me.TryPickupItem()) return;
                if (Globals.Me.AttackTimer < Globals.System.GetTimeMs())
                {
                    Globals.Me.AttackTimer = Globals.System.GetTimeMs() + Globals.Me.CalculateAttackTime();
                }
            }

            if (!GameControls.ControlHasKey(Controls.Block, key)) return;
            if (Globals.Me.TryBlock()) return;
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
            if (GameControls.ControlHasKey(Controls.Block, key))
            {
                Globals.Me.StopBlocking();
            }
            if (btn != GameInput.MouseButtons.Right) return;
            if (Globals.InputManager.KeyDown(Keys.Shift) != true) return;
            var x =
                (int)
                Math.Floor(Globals.InputManager.GetMousePosition().X + GameGraphics.CurrentView.Left);
            var y =
                (int)
                Math.Floor(Globals.InputManager.GetMousePosition().Y + GameGraphics.CurrentView.Top);

            foreach (MapInstance map in MapInstance.Lookup.Values)
            {
                if (!(x >= map.GetX()) || !(x <= map.GetX() + (Options.MapWidth * Options.TileWidth))) continue;
                if (!(y >= map.GetY()) || !(y <= map.GetY() + (Options.MapHeight * Options.TileHeight))) continue;
                //Remove the offsets to just be dealing with pixels within the map selected
                x -= (int) map.GetX();
                y -= (int) map.GetY();

                //transform pixel format to tile format
                x /= Options.TileWidth;
                y /= Options.TileHeight;
                var mapNum = map.Id;

                if (Globals.Me.GetRealLocation(ref x, ref y, ref mapNum))
                {
                    PacketSender.SendAdminAction((int) AdminActions.WarpToLoc,
                        Convert.ToString(mapNum), Convert.ToString(x), Convert.ToString(y));
                }
                return;
            }
        }
    }
}