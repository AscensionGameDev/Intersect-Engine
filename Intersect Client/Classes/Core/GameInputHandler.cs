using System;
using Intersect;
using Intersect.Client.Classes.Core;
using Intersect.Enums;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI;

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
            KeyDown?.Invoke(key);
            if (Gui.HasInputFocus())
            {
                return;
            }

            if (key == Keys.F11)
            {
                Gui.HideUi = !Gui.HideUi;
                return;
            }

            if (GameControls.ControlHasKey(Controls.Screenshot, key))
            {
                GameGraphics.Renderer?.RequestScreenshot();
                return;
            }

			if (GameControls.ControlHasKey(Controls.PickUp, key))
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
			else if (GameControls.ControlHasKey(Controls.AutoTarget, key))
			{
				if (Globals.Me != null)
				{
					Globals.Me.AutoTarget();
					return;
				}
			}
			else switch (key)
				{
					case Keys.Escape:
						if (Globals.GameState == GameStates.Intro)
						{
							GameFade.FadeIn();
							Globals.GameState = GameStates.Menu;
						}
						break;
					case Keys.Insert:
						//Try to open admin panel!
						if (Globals.GameState == GameStates.InGame)
						{
							PacketSender.SendOpenAdminWindow();
						}
						break;
					case Keys.F2:
						if (Globals.GameState != GameStates.InGame) return;
						Gui.GameUi.ShowHideDebug();
						break;
					default:
						if (GameControls.ControlHasKey(Controls.Enter, key))
						{
							if (Globals.GameState != GameStates.InGame) return;
							if (!Gui.HasInputFocus())
							{
								Gui.GameUi.FocusChat = true;
							}
						}
						break;
				}

            if (Globals.GameState != GameStates.InGame) return;
            if (Gui.HasInputFocus()) return;
            if (GameControls.ControlHasKey(Controls.Hotkey0, key))
            {
                Gui.GameUi.Hotbar.Items[9].Activate();
            }

            for (var i = Controls.Hotkey1; i <= Controls.Hotkey9; i++)
            {
                if (!GameControls.ControlHasKey(i, key)) continue;
                var index = (int) (i - Controls.Hotkey1);
                Gui.GameUi?.Hotbar?.Items?[index]?.Activate();
            }

            if (GameControls.ControlHasKey(Controls.OpenCharacterInfo, key))
            {
                Gui.GameUi.GameMenu.ToggleCharacterWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenFriends, key))
            {
                Gui.GameUi.GameMenu.ToggleFriendsWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenInventory, key))
            {
                Gui.GameUi.GameMenu.ToggleInventoryWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenParties, key))
            {
                Gui.GameUi.GameMenu.TogglePartyWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenQuests, key))
            {
                Gui.GameUi.GameMenu.ToggleQuestsWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenSpells, key))
            {
                Gui.GameUi.GameMenu.ToggleSpellsWindow();
            }
            else if (GameControls.ControlHasKey(Controls.OpenMenu, key))
            {

            }
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
                    if (!Gui.MouseHitGui())
                    {
                        if (Globals.Me != null)
                        {
                            if (Globals.Me.TryTarget())
                            {
                                return;
                            }
                            if (GameControls.ControlHasKey(Controls.PickUp, key))
                            {
                                if (Globals.Me.TryPickupItem())
                                {
                                    return;
                                }
                                if (Globals.Me.AttackTimer < Globals.System.GetTimeMs())
                                    Globals.Me.AttackTimer = Globals.System.GetTimeMs() +
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
                    if (GameControls.ControlHasKey(Controls.Block, key))
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
                                        x -= (int) map.GetX();
                                        y -= (int) map.GetY();

                                        //transform pixel format to tile format
                                        x /= Options.TileWidth;
                                        y /= Options.TileHeight;
                                        int mapNum = map.Index;

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
                    }
                }
            }
        }
    }
}