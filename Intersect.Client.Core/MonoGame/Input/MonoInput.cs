using System.Diagnostics;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Client.ThirdParty;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Intersect.Client.Framework.GenericClasses.Keys;
using Rectangle = Intersect.Client.Framework.GenericClasses.Rectangle;

namespace Intersect.Client.MonoGame.Input;

using MonoGameKeys = Microsoft.Xna.Framework.Input.Keys;

public partial class MonoInput : GameInput
{
    private readonly Dictionary<Keys, MonoGameKeys> _intersectToMonoGameKeyMap;

    private GamePadState _currentGamePadState;
    private GamePadState _previousGamePadState;

    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    private MouseState _currentMouseState;
    private MouseState _previousMouseState;

    private int mMouseX;

    private int mMouseY;

    private int mMouseVScroll;

    private int mMouseHScroll;

    private Game mMyGame;

    private readonly GamePadCapabilities[] _gamePadCapabilities =
        new GamePadCapabilities[GamePad.MaximumGamePadCount];

    private int _activeGamePad;

    private bool _keyboardOpened;
    private IControlSet _controlSet;

    public MonoInput(Game myGame)
    {
        myGame.Window.TextInput += Window_TextInput;
        mMyGame = myGame;

        _intersectToMonoGameKeyMap = [];
        foreach (var intersectKey in Enum.GetValues<Keys>())
        {
#pragma warning disable CA1864
            if (!_intersectToMonoGameKeyMap.ContainsKey(intersectKey))
#pragma warning restore CA1864
            {
                foreach (var monoGameKey in Enum.GetValues<MonoGameKeys>())
                {
                    if (intersectKey == Keys.Shift)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.LeftShift);
                        break;
                    }

                    if (intersectKey is Keys.Control or Keys.LControlKey)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.LeftControl);
                        break;
                    }

                    if (intersectKey == Keys.RControlKey)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.RightControl);
                        break;
                    }

                    if (intersectKey is Keys.Alt or Keys.LMenu)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.LeftAlt);
                        break;
                    }

                    if (intersectKey == Keys.RMenu)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.RightAlt);
                        break;
                    }

                    if (intersectKey == Keys.Return)
                    {
                        _intersectToMonoGameKeyMap.Add(intersectKey, MonoGameKeys.Enter);
                        break;
                    }

                    if (intersectKey.ToString() != monoGameKey.ToString())
                    {
                        continue;
                    }

                    _intersectToMonoGameKeyMap.Add(intersectKey, monoGameKey);

                    break;
                }
            }

            if (!_intersectToMonoGameKeyMap.ContainsKey(intersectKey))
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    "No matching MonoGame key for {IntersectKey}",
                    intersectKey
                );
            }
        }

        _controlSet = new Controls();

        InputHandler.FocusChanged += InputHandlerOnFocusChanged;
    }

    public override IControlSet ControlSet
    {
        get => _controlSet;
        set => _controlSet = value;
    }

    public override bool MouseHitInterface => Interface.Interface.DoesMouseHitInterface();

    public override bool IsMouseInBounds => Interface.Interface.IsInBounds(_currentMouseState.X, _currentMouseState.Y);

    private void InputHandlerOnFocusChanged(Base? control, FocusSource focusSource)
    {
        if (control == default)
        {
            return;
        }

        if (focusSource == FocusSource.Mouse)
        {
            return;
        }

        Vector2 center = new(
            (control.GlobalBounds.Left + control.GlobalBounds.Right) / 2f,
            (control.GlobalBounds.Bottom + control.GlobalBounds.Top) / 2f
        );

        Mouse.SetPosition((int)center.X, (int)center.Y);
        var mouseState = Mouse.GetState();
        Interface.Interface.GwenInput.ProcessMessage(
            new GwenInputMessage(IntersectInput.InputEvent.MouseMove, new Pointf(mouseState.X, mouseState.Y), MouseButton.None, Keys.Alt)
        );
    }

    private void Window_TextInput(object sender, TextInputEventArgs e)
    {
        Interface.Interface.GwenInput.ProcessMessage(
            new GwenInputMessage(
                IntersectInput.InputEvent.TextEntered, GetMousePosition(), MouseButton.None, Keys.Alt, false,
                false, false, e.Character.ToString()
            )
        );
    }

    private bool MouseButtonDown(MouseState mouseState, MouseButton mb)
    {
        switch (mb)
        {
            case MouseButton.Left:
                return mouseState.LeftButton == ButtonState.Pressed;
            case MouseButton.Right:
                return mouseState.RightButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return mouseState.MiddleButton == ButtonState.Pressed;
            case MouseButton.X1:
                return mouseState.XButton1 == ButtonState.Pressed;
            case MouseButton.X2:
                return mouseState.XButton2 == ButtonState.Pressed;
            default:
                throw new ArgumentOutOfRangeException(nameof(mb), mb, null);
        }
    }

    public override bool IsMouseButtonDown(MouseButton mb) => MouseButtonDown(_currentMouseState, mb);

    public override bool WasMouseButtonDown(MouseButton mb) => MouseButtonDown(_previousMouseState, mb);

    public override bool IsKeyDown(Keys key) =>
        _intersectToMonoGameKeyMap.TryGetValue(key, out var mappedKey) && _currentKeyboardState.IsKeyDown(mappedKey);

    public override bool WasKeyDown(Keys key) =>
        _intersectToMonoGameKeyMap.TryGetValue(key, out var mappedKey) && _previousKeyboardState.IsKeyDown(mappedKey);

    public override Pointf GetMousePosition()
    {
        return new Pointf(mMouseX, mMouseY);
    }

    private void CheckMouseButton(Keys modifier, ButtonState bs, MouseButton mb)
    {
        if (Globals.GameState == GameStates.Intro)
        {
            return; //No mouse input allowed while showing intro slides
        }

        if (bs == ButtonState.Pressed && !IsMouseButtonDown(mb))
        {
            if (Core.Input.TestInterceptMouse(modifier, mb, down: true))
            {
                return;
            }

            Interface.Interface.GwenInput.ProcessMessage(
                new GwenInputMessage(IntersectInput.InputEvent.MouseDown, GetMousePosition(), mb, Keys.Alt)
            );

            Core.Input.OnMouseDown(modifier, mb);
        }
        else if (bs == ButtonState.Released && IsMouseButtonDown(mb))
        {
            if (Core.Input.TestInterceptMouse(modifier, mb, down: false))
            {
                return;
            }

            Interface.Interface.GwenInput.ProcessMessage(
                new GwenInputMessage(IntersectInput.InputEvent.MouseUp, GetMousePosition(), mb, Keys.Alt)
            );

            Core.Input.OnMouseUp(modifier, mb);
        }
    }

    private const double ZoomDelayInMilliseconds = 300;
    private DateTime _lastZoomTime = DateTime.MinValue;

    private void CheckMouseScrollWheel(int scrlVValue, int scrlHValue)
    {
        Pointf p = new Pointf(0, 0);

        if (scrlVValue != mMouseVScroll || scrlHValue != mMouseHScroll)
        {
            p = new Pointf(scrlHValue - mMouseHScroll, scrlVValue - mMouseVScroll);

            Interface.Interface.GwenInput.ProcessMessage(
                new GwenInputMessage(IntersectInput.InputEvent.MouseScroll, p, MouseButton.Middle, Keys.Alt)
            );

            var deltaV = Math.Sign(mMouseVScroll - scrlVValue);

            if ((DateTime.Now - _lastZoomTime).TotalMilliseconds >= ZoomDelayInMilliseconds)

            {

                if ((InputHandler.HoveredControl == null || !InputHandler.HoveredControl.IsVisibleInTree)
                  && Globals.GameState == GameStates.InGame
                  && Globals.Database.EnableMouseScrollZoom)
            {
                if (deltaV < 0)
                {
                    Core.Input.HandleZoomIn(false);
                }
                else
                {
                    Core.Input.HandleZoomOut(false);
                }
                    _lastZoomTime = DateTime.Now;
                }
            }
            mMouseVScroll = scrlVValue;
            mMouseHScroll = scrlHValue;
        }
    }

    public override void Update(TimeSpan elapsed)
    {
        if (mMyGame.IsActive)
        {
            if (_keyboardOpened)
            {
                Steam.PumpEvents();
            }

            var gamePadCapabilities = Enumerable.Range(0, GamePad.MaximumGamePadCount)
                .Select(GamePad.GetCapabilities)
                .ToArray();

            Array.Copy(
                gamePadCapabilities,
                _gamePadCapabilities,
                Math.Min(gamePadCapabilities.Length, _gamePadCapabilities.Length)
            );

            var gamePadState = Enumerable
                .Range(0, GamePad.MaximumGamePadCount)
                .Select(GamePad.GetState)
                .Skip(_activeGamePad)
                .FirstOrDefault(gamePad => gamePad.IsConnected);

            if (gamePadState.IsConnected)
            {
                var deltaX = (int)(gamePadState.ThumbSticks.Right.X * elapsed.TotalSeconds * 1000);
                var deltaY = (int)(-gamePadState.ThumbSticks.Right.Y * elapsed.TotalSeconds * 1000);

                var temporaryMouseState = Mouse.GetState();
                Mouse.SetPosition(temporaryMouseState.X + deltaX, temporaryMouseState.Y + deltaY);
            }

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.X != mMouseX || mouseState.Y != mMouseY)
            {
                mMouseX = (int)(mouseState.X * ((MonoRenderer)Core.Graphics.Renderer).GetMouseOffset().X);
                mMouseY = (int)(mouseState.Y * ((MonoRenderer)Core.Graphics.Renderer).GetMouseOffset().Y);
                Interface.Interface.GwenInput.ProcessMessage(
                    new GwenInputMessage(
                        IntersectInput.InputEvent.MouseMove, GetMousePosition(), MouseButton.None, Keys.Alt
                    )
                );
            }

            if (gamePadState.IsConnected)
            {
                var leftMouseButton = gamePadState.Buttons.A == ButtonState.Released
                    ? mouseState.LeftButton
                    : ButtonState.Pressed;

                var rightMouseButton = gamePadState.Buttons.B == ButtonState.Released
                    ? mouseState.RightButton
                    : ButtonState.Pressed;

                mouseState = new MouseState(
                    mouseState.X,
                    mouseState.Y,
                    mouseState.ScrollWheelValue,
                    leftMouseButton,
                    mouseState.MiddleButton,
                    rightMouseButton,
                    mouseState.XButton1,
                    mouseState.XButton2,
                    mouseState.HorizontalScrollWheelValue
                );

                var gamePadKeys = Enum.GetValues<Buttons>()
                    .Where(gamePadState.IsButtonDown)
                    .Select(
                        button => button switch
                        {
                            Buttons.None => MonoGameKeys.None,
                            Buttons.DPadUp => MonoGameKeys.Up,
                            Buttons.DPadDown => MonoGameKeys.Down,
                            Buttons.DPadLeft => MonoGameKeys.Left,
                            Buttons.DPadRight => MonoGameKeys.Right,
                            Buttons.Start => MonoGameKeys.Escape,
                            Buttons.Back => MonoGameKeys.None,
                            Buttons.LeftStick => MonoGameKeys.None,
                            Buttons.RightStick => MonoGameKeys.None,
                            Buttons.LeftShoulder => MonoGameKeys.Back,
                            Buttons.RightShoulder => MonoGameKeys.Tab,
                            Buttons.BigButton => MonoGameKeys.None,
                            Buttons.A => MonoGameKeys.Enter,
                            Buttons.B => MonoGameKeys.Back,
                            Buttons.X => MonoGameKeys.E,
                            Buttons.Y => MonoGameKeys.None,
                            Buttons.LeftThumbstickLeft => MonoGameKeys.None,
                            Buttons.RightTrigger => MonoGameKeys.None,
                            Buttons.LeftTrigger => MonoGameKeys.None,
                            Buttons.RightThumbstickUp => MonoGameKeys.None,
                            Buttons.RightThumbstickDown => MonoGameKeys.None,
                            Buttons.RightThumbstickRight => MonoGameKeys.None,
                            Buttons.RightThumbstickLeft => MonoGameKeys.None,
                            Buttons.LeftThumbstickUp => MonoGameKeys.None,
                            Buttons.LeftThumbstickDown => MonoGameKeys.None,
                            Buttons.LeftThumbstickRight => MonoGameKeys.None,
                            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
                        }
                    );

                keyboardState = new KeyboardState(
                    keyboardState.GetPressedKeys().Concat(gamePadKeys).ToArray(),
                    keyboardState.CapsLock,
                    keyboardState.NumLock
                );
            }

            // Get what modifier key we're currently pressing.
            var modifier = GetPressedModifier(keyboardState);

            //Check for state changes in the left mouse button
            if (Interface.Interface.IsInBounds(mouseState.X, mouseState.Y))
            {
                CheckMouseButton(modifier, mouseState.LeftButton, MouseButton.Left);
                CheckMouseButton(modifier, mouseState.RightButton, MouseButton.Right);
                CheckMouseButton(modifier, mouseState.MiddleButton, MouseButton.Middle);
                CheckMouseButton(modifier, mouseState.XButton1, MouseButton.X1);
                CheckMouseButton(modifier, mouseState.XButton2, MouseButton.X2);

                CheckMouseScrollWheel(mouseState.ScrollWheelValue, mouseState.HorizontalScrollWheelValue);
            }

            foreach (var key in _intersectToMonoGameKeyMap)
            {
                if (keyboardState.IsKeyDown(key.Value) && !_currentKeyboardState.IsKeyDown(key.Value))
                {
                    ApplicationContext.Context.Value?.Logger.LogTrace("{0} -> {1}", key.Key, key.Value);
                    Interface.Interface.GwenInput.ProcessMessage(
                        new GwenInputMessage(
                            IntersectInput.InputEvent.KeyDown, GetMousePosition(), MouseButton.None, key.Key
                        )
                    );

                    Core.Input.OnKeyPressed(modifier, key.Key);
                }
                else if (!keyboardState.IsKeyDown(key.Value) && _currentKeyboardState.IsKeyDown(key.Value))
                {
                    Interface.Interface.GwenInput.ProcessMessage(
                        new GwenInputMessage(
                            IntersectInput.InputEvent.KeyUp, GetMousePosition(), MouseButton.None, key.Key
                        )
                    );

                    Core.Input.OnKeyReleased(modifier, key.Key);
                }
            }

            (_previousGamePadState, _currentGamePadState) = (_currentGamePadState, gamePadState);
            (_previousKeyboardState, _currentKeyboardState) = (_currentKeyboardState, keyboardState);
            (_previousMouseState, _currentMouseState) = (_currentMouseState, mouseState);
        }
        else
        {
            var modifier = GetPressedModifier(_currentKeyboardState);

            foreach (var key in _intersectToMonoGameKeyMap)
            {
                if (_currentKeyboardState.IsKeyDown(key.Value))
                {
                    Interface.Interface.GwenInput.ProcessMessage(
                        new GwenInputMessage(
                            IntersectInput.InputEvent.KeyUp, GetMousePosition(), MouseButton.None, key.Key
                        )
                    );

                    Core.Input.OnKeyReleased(modifier, key.Key);
                }
            }

            CheckMouseButton(modifier, ButtonState.Released, MouseButton.Left);
            CheckMouseButton(modifier, ButtonState.Released, MouseButton.Right);
            CheckMouseButton(modifier, ButtonState.Released, MouseButton.Middle);
            CheckMouseButton(modifier, ButtonState.Released, MouseButton.X1);
            CheckMouseButton(modifier, ButtonState.Released, MouseButton.X2);

            _currentGamePadState = new GamePadState();
            _previousGamePadState = new GamePadState();
            _currentKeyboardState = new KeyboardState();
            _previousKeyboardState = new KeyboardState();
            _currentMouseState = new MouseState();
            _previousMouseState = new MouseState();
        }
    }

    public Keys GetPressedModifier(KeyboardState state)
    {
        var modifier = Keys.None;
        if (state.IsKeyDown(MonoGameKeys.LeftControl) || state.IsKeyDown(MonoGameKeys.RightControl))
        {
            modifier = Keys.Control;
        }

        if (state.IsKeyDown(MonoGameKeys.LeftShift) || state.IsKeyDown(MonoGameKeys.RightShift))
        {
            modifier = Keys.Shift;
        }

        if (state.IsKeyDown(MonoGameKeys.LeftAlt) || state.IsKeyDown(MonoGameKeys.RightAlt))
        {
            modifier = Keys.Alt;
        }

        return modifier;
    }
    public override void OpenKeyboard(
        KeyboardType type,
        string text,
        bool autoCorrection,
        bool multiLine,
        bool secure
    )
    {
        return; //no on screen keyboard for pc clients
    }

    public override void OpenKeyboard(
        KeyboardType keyboardType,
        Action<string?> inputHandler,
        string description,
        string text,
        bool multiline = false,
        uint maxLength = 1024,
        Rectangle? inputBounds = default
    )
    {
        if (!Steam.SteamDeck)
        {
            return;
        }

        _keyboardOpened = Steam.ShowKeyboard(
            inputHandler,
            description,
            existingInput: text,
            keyboardType == KeyboardType.Password,
            maxLength,
            inputBounds
        );
    }
}
