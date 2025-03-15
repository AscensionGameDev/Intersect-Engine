using Intersect.Admin.Actions;
using Intersect.Client.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Maps;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Framework.Core;

namespace Intersect.Client.Core;

public delegate bool MouseButtonEventInterceptor(Keys modifier, MouseButton mouseButton);

public static partial class Input
{
    public delegate void HandleKeyEvent(Keys modifier, Keys key);

    private static HandleKeyEvent? _keyDown;

    private static HandleKeyEvent? _keyUp;

    private static HandleKeyEvent? _mouseDown;

    private static HandleKeyEvent? _mouseUp;

    public static HandleKeyEvent? KeyDown { get => _keyDown; set => _keyDown = value; }

    public static HandleKeyEvent? KeyUp { get => _keyUp; set => _keyUp = value; }

    public static HandleKeyEvent? MouseDown { get => _mouseDown; set => _mouseDown = value; }

    public static HandleKeyEvent? MouseUp { get => _mouseUp; set => _mouseUp = value; }

    public static void HandleZoomOut(bool wrap = true)
    {
        var nextZoom = Globals.Database.WorldZoom / 2;

        if (nextZoom < Graphics.MinimumWorldScale)
        {
            nextZoom = wrap ? Graphics.MaximumWorldScale : Graphics.MinimumWorldScale;
        }

        Globals.Database.WorldZoom = nextZoom;
    }

    public static void HandleZoomIn(bool wrap = true)
    {
        var nextZoom = Globals.Database.WorldZoom * 2;

        if (nextZoom > Graphics.MaximumWorldScale)
        {
            nextZoom = wrap ? Graphics.MinimumWorldScale : Graphics.MaximumWorldScale;
        }

        Globals.Database.WorldZoom = nextZoom;
    }

    public static void OnKeyPressed(Keys modifier, Keys key)
    {
        if (key == Keys.None)
        {
            return;
        }

        var currentGameState = Globals.GameState;
        var consumeKey = false;
        bool canFocusChat = true;

        KeyDown?.Invoke(modifier, key);

        switch (key)
        {
            case Keys.Escape:
                if (currentGameState != GameStates.Intro)
                {
                    break;
                }

                Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
                Globals.GameState = GameStates.Menu;

                return;

            case Keys.Enter:
                var components = Interface.Interface.InputBlockingComponents.ToArray();
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    var inputBlockingComponent = components[i];
                    try
                    {
                        if (inputBlockingComponent is InputBox { IsHidden: false } inputBox)
                        {
                            inputBox.SubmitInput();
                            consumeKey = true;
                            canFocusChat = false;
                            break;
                        }
                    }
                    catch { }

                    try
                    {
                        if (inputBlockingComponent is EventWindow { IsHidden: false } eventWindow && Globals.EventDialogs.Count > 0)
                        {
                            eventWindow.CloseEventResponse(EventResponseType.OneOption);
                            consumeKey = true;
                            canFocusChat = false;

                            break;
                        }
                    }
                    catch { }
                }

                if (!consumeKey)
                {

                }
                break;
        }

        if (Controls.Controls.ControlHasKey(Control.OpenMenu, modifier, key))
        {
            if (currentGameState != GameStates.InGame)
            {
                return;
            }

            if (!Interface.Interface.HasInGameUI)
            {
                return;
            }

            if (DragAndDrop.IsDragging)
            {
                return;
            }

            var gameUi = Interface.Interface.GameUi;

            // First try and unfocus chat then close all UI elements, then untarget our target.. and THEN open the escape menu.
            // Most games do this, why not this?
            if (gameUi.ChatFocussed)
            {
                gameUi.UnfocusChat = true;
            }
            else if (gameUi.CloseAllWindows())
            {
                // We've closed our windows, don't do anything else. :)
            }
            else if (Globals.Me is {} me && me.TargetId != default && me.Status.All(s => s.Type != SpellEffect.Taunt))
            {
                _ = me.ClearTarget();
            }
            else
            {
                var simplifiedEscapeMenuSetting = Globals.Database.SimplifiedEscapeMenu;

                if (simplifiedEscapeMenuSetting)
                {
                    if (gameUi.EscapeMenu.IsVisibleInTree)
                    {
                        gameUi.EscapeMenu.ToggleHidden();
                    }
                    else
                    {
                        gameUi.GameMenu.ToggleSimplifiedEscapeMenu();
                    }
                }
                else
                {
                    gameUi.EscapeMenu.ToggleHidden();
                }
            }
        }

        if (Interface.Interface.HasInputFocus())
        {
            return;
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var control in Controls.Controls.GetControlsFor(modifier, key))
        {
            if (consumeKey)
            {
                continue;
            }

            switch (control)
            {
                case Control.Screenshot:
                    Graphics.Renderer?.RequestScreenshot();
                    break;

                case Control.ToggleGui:
                    if (currentGameState == GameStates.InGame && !DragAndDrop.IsDragging)
                    {
                        Interface.Interface.HideUi = !Interface.Interface.HideUi;
                    }
                    break;

                case Control.HoldToZoomIn:
                case Control.ToggleZoomIn:
                {
                    HandleZoomIn();
                    break;
                }

                case Control.HoldToZoomOut:
                case Control.ToggleZoomOut:
                {
                    HandleZoomOut();
                    break;
                }

                case Control.ToggleFullscreen:
                {
                    if (Graphics.Renderer == default)
                    {
                        break;
                    }

                    Globals.Database.FullScreen = !Globals.Database.FullScreen;
                    Globals.Database.SavePreferences();
                    Graphics.Renderer.OverrideResolution = Resolution.Empty;
                    Graphics.Renderer.Init();
                    break;
                }

                case Control.OpenDebugger:
                    Interface.Interface.CurrentInterface.ToggleDebug();
                    break;
            }

            switch (currentGameState)
            {
                case GameStates.Intro:
                    break;

                case GameStates.Menu:
                    break;

                case GameStates.InGame:
                    if (!Interface.Interface.HasInGameUI)
                    {
                        break;
                    }

                    if (DragAndDrop.IsDragging)
                    {
                        break;
                    }

                    switch (control)
                    {
                        case Control.Block:
                            _ = (Globals.Me?.TryBlock());
                            break;

                        case Control.AutoTarget:
                            Globals.Me?.AutoTarget();
                            break;

                        case Control.HoldToSoftRetargetOnSelfCast:
                            Globals.HoldToSoftRetargetOnSelfCast = true;
                            break;

                        case Control.ToggleAutoSoftRetargetOnSelfCast:
                            Globals.Database.AutoSoftRetargetOnSelfCast = !Globals.Database.AutoSoftRetargetOnSelfCast;
                            break;

                        case Control.PickUp:
                            if (Globals.Me != default && Globals.Me.MapInstance != default)
                            {
                                _ = Player.TryPickupItem(
                                    Globals.Me.MapInstance.Id,
                                    Globals.Me.Y * Options.Instance.Map.MapWidth + Globals.Me.X
                                );
                            }
                            break;

                        case Control.Enter:
                            if (canFocusChat)
                            {
                                Interface.Interface.GameUi.FocusChat = true;
                                consumeKey = true;
                            }
                            continue;

                        case Control.OpenInventory:
                            Interface.Interface.GameUi.GameMenu?.ToggleInventoryWindow();
                            break;

                        case Control.OpenQuests:
                            Interface.Interface.GameUi.GameMenu?.ToggleQuestsWindow();
                            break;

                        case Control.OpenCharacterInfo:
                            Interface.Interface.GameUi.GameMenu?.ToggleCharacterWindow();
                            break;

                        case Control.OpenParties:
                            Interface.Interface.GameUi.GameMenu?.TogglePartyWindow();
                            break;

                        case Control.OpenSpells:
                            Interface.Interface.GameUi.GameMenu?.ToggleSpellsWindow();
                            break;

                        case Control.OpenFriends:
                            _ = (Interface.Interface.GameUi.GameMenu?.ToggleFriendsWindow());
                            break;

                        case Control.OpenSettings:
                            Interface.Interface.GameUi.EscapeMenu?.OpenSettingsWindow();
                            break;

                        case Control.OpenAdminPanel:
                            PacketSender.SendOpenAdminWindow();
                            break;

                        case Control.OpenGuild:
                            _ = Interface.Interface.GameUi.GameMenu?.ToggleGuildWindow();
                            break;
                    }
                    break;

                case GameStates.Loading:
                    break;

                case GameStates.Error:
                    break;

                default:
                    throw new NotImplementedException(
                        $"{nameof(GameStates)} '{currentGameState}' not yet implemented"
                    );
            }
        }
    }

    public static void OnKeyReleased(Keys modifier, Keys key)
    {
        KeyUp?.Invoke(modifier, key);
        if (Interface.Interface.HasInputFocus())
        {
            return;
        }

        if (Controls.Controls.ControlHasKey(Control.HoldToZoomIn, modifier, key))
        {
            HandleZoomOut();
        }

        if (Controls.Controls.ControlHasKey(Control.HoldToZoomOut, modifier, key))
        {
            HandleZoomIn();
        }

        if (Controls.Controls.ControlHasKey(Control.HoldToSoftRetargetOnSelfCast, modifier, key))
        {
            Globals.HoldToSoftRetargetOnSelfCast = false;
        }

        if (Globals.Me == null)
        {
            return;
        }
    }

    public static event MouseButtonEventInterceptor? MouseDownIntercept;

    public static event MouseButtonEventInterceptor? MouseUpIntercept;

    private static bool InvokeMouseButtonInterceptors(
        MulticastDelegate? multicastDelegate,
        Keys modifier,
        MouseButton mouseButton
    )
    {
        var rawInvocationList = multicastDelegate?.GetInvocationList() ?? [];
        var invocationList = rawInvocationList.OfType<MouseButtonEventInterceptor>().ToArray();
        return invocationList.Any(interceptor => interceptor(modifier, mouseButton));
    }

    public static bool TestInterceptMouse(Keys modifier, MouseButton mouseButton, bool down)
    {
        return modifier != Keys.Alt &&
               InvokeMouseButtonInterceptors(down ? MouseDownIntercept : MouseUpIntercept, modifier, mouseButton);
    }

    public static void OnMouseDown(Keys modifier, MouseButton mouseButton)
    {
        var key = Keys.None;
        switch (mouseButton)
        {
            case MouseButton.Left:
                key = Keys.LButton;

                break;

            case MouseButton.Right:
                key = Keys.RButton;

                break;

            case MouseButton.Middle:
                key = Keys.MButton;

                break;
            case MouseButton.X1:
                key = Keys.XButton1;

                break;
            case MouseButton.X2:
                key = Keys.XButton2;

                break;
        }

        MouseDown?.Invoke(modifier, key);
        if (Interface.Interface.HasInputFocus())
        {
            return;
        }

        if (Globals.GameState != GameStates.InGame || Globals.Me == null)
        {
            return;
        }

        if (Interface.Interface.DoesMouseHitInterface())
        {
            return;
        }

        if (Globals.Me == null)
        {
            return;
        }

        if (modifier == Keys.None && mouseButton == MouseButton.Left && Globals.Me.TryTarget())
        {
            return;
        }

        if (Controls.Controls.ControlHasKey(Control.PickUp, modifier, key))
        {
            if (Globals.Me.MapInstance != default &&
                Player.TryPickupItem(Globals.Me.MapInstance.Id, Globals.Me.Y * Options.Instance.Map.MapWidth + Globals.Me.X, Guid.Empty, true)
            )
            {
                return;
            }

            if (!Globals.Me.IsAttacking)
            {
                Globals.Me.AttackTimer = Timing.Global.Milliseconds + Globals.Me.CalculateAttackTime();
            }
        }

        if (Controls.Controls.ControlHasKey(Control.Block, modifier, key))
        {
            if (Globals.Me.TryBlock())
            {
                return;
            }
        }

        if (key != Keys.None)
        {
            OnKeyPressed(modifier, key);
        }
    }

    public static void OnMouseUp(Keys modifier, MouseButton mouseButton)
    {
        var key = Keys.LButton;
        switch (mouseButton)
        {
            case MouseButton.Right:
                key = Keys.RButton;

                break;

            case MouseButton.Middle:
                key = Keys.MButton;

                break;
            case MouseButton.X1:
                key = Keys.XButton1;

                break;
            case MouseButton.X2:
                key = Keys.XButton2;

                break;
        }

        MouseUp?.Invoke(modifier, key);
        if (Interface.Interface.HasInputFocus())
        {
            return;
        }

        if (Controls.Controls.ControlHasKey(Control.HoldToZoomIn, modifier, key))
        {
            HandleZoomOut();
        }

        if (Controls.Controls.ControlHasKey(Control.HoldToZoomOut, modifier, key))
        {
            HandleZoomIn();
        }

        if (Globals.Me == null)
        {
            return;
        }

        if (mouseButton != MouseButton.Right)
        {
            return;
        }

        if (Globals.InputManager.IsKeyDown(Keys.Shift) != true)
        {
            return;
        }

        var mouseInWorld = Graphics.ConvertToWorldPoint(Globals.InputManager.GetMousePosition());
        var x = (int)mouseInWorld.X;
        var y = (int)mouseInWorld.Y;

        foreach (MapInstance map in MapInstance.Lookup.Values.Cast<MapInstance>())
        {
            if (!(x >= map.X) || !(x <= map.X + Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth))
            {
                continue;
            }

            if (!(y >= map.Y) || !(y <= map.Y + Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight))
            {
                continue;
            }

            //Remove the offsets to just be dealing with pixels within the map selected
            x -= (int) map.X;
            y -= (int) map.Y;

            //transform pixel format to tile format
            x /= Options.Instance.Map.TileWidth;
            y /= Options.Instance.Map.TileHeight;
            var mapNum = map.Id;

            if (Globals.Me.TryGetRealLocation(ref x, ref y, ref mapNum))
            {
                PacketSender.SendAdminAction(new WarpToLocationAction(map.Id, (byte) x, (byte) y));
            }

            return;
        }
    }

    public static bool IsModifier(Keys key)
    {
        return key switch
        {
            Keys.Control or
            Keys.ControlKey or
            Keys.LControlKey or
            Keys.RControlKey or
            Keys.LShiftKey or
            Keys.RShiftKey or
            Keys.Shift or
            Keys.ShiftKey or
            Keys.Alt => true,
            _ => false,
        };
    }
}
