using System;
using System.Collections.Generic;
using System.Diagnostics;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Client.ThirdParty;
using Intersect.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using Keys = Intersect.Client.Framework.GenericClasses.Keys;
using Rectangle = Intersect.Client.Framework.GenericClasses.Rectangle;

namespace Intersect.Client.MonoGame.Input
{

    public partial class MonoInput : GameInput
    {

        private Dictionary<Keys, Microsoft.Xna.Framework.Input.Keys> mKeyDictionary;

        private KeyboardState mLastKeyboardState;

        private MouseState mLastMouseState;

        private int mMouseX;

        private int mMouseY;

        private int mMouseVScroll;

        private int mMouseHScroll;

        private Game mMyGame;

        private readonly GamePadCapabilities[] _gamePadCapabilities =
            new GamePadCapabilities[GamePad.MaximumGamePadCount];

        private GamePadState _lastGamePadState;

        private int _activeGamePad;

        private bool _keyboardOpened;

        public MonoInput(Game myGame)
        {
            myGame.Window.TextInput += Window_TextInput;
            mMyGame = myGame;
            mKeyDictionary = new Dictionary<Keys, Microsoft.Xna.Framework.Input.Keys>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!mKeyDictionary.ContainsKey(key))
                {
                    foreach (Microsoft.Xna.Framework.Input.Keys monoKey in Enum.GetValues(
                        typeof(Microsoft.Xna.Framework.Input.Keys)
                    ))
                    {
                        if (key == Keys.Shift)
                        {
                            mKeyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.LeftShift);

                            break;
                        }

                        if (key == Keys.Control || key == Keys.LControlKey)
                        {
                            mKeyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.LeftControl);

                            break;
                        }

                        if (key == Keys.RControlKey)
                        {
                            mKeyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.RightControl);

                            break;
                        }

                        if (key == Keys.Return)
                        {
                            mKeyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.Enter);

                            break;
                        }
                        else
                        {
                            if (key.ToString() == monoKey.ToString())
                            {
                                mKeyDictionary.Add(key, monoKey);

                                break;
                            }
                        }
                    }
                }

                if (!mKeyDictionary.ContainsKey(key))
                {
                    Debug.WriteLine("Mono does not have a key to match: " + key);
                }
            }

            InputHandler.FocusChanged += InputHandlerOnFocusChanged;
        }

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
                (control.BoundsGlobal.Left + control.BoundsGlobal.Right) / 2f,
                (control.BoundsGlobal.Bottom + control.BoundsGlobal.Top) / 2f
            );

            Mouse.SetPosition((int)center.X, (int)center.Y);
            var mouseState = Mouse.GetState();
            Interface.Interface.GwenInput.ProcessMessage(
                new GwenInputMessage(IntersectInput.InputEvent.MouseMove, new Pointf(mouseState.X, mouseState.Y), (int)MouseButtons.None, Keys.Alt)
            );
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            Interface.Interface.GwenInput.ProcessMessage(
                new GwenInputMessage(
                    IntersectInput.InputEvent.TextEntered, GetMousePosition(), (int) MouseButtons.None, Keys.Alt, false,
                    false, false, e.Character.ToString()
                )
            );
        }

        public override bool MouseButtonDown(MouseButtons mb)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    return mLastMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.Right:
                    return mLastMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.Middle:
                    return mLastMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.X1:
                    return mLastMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.X2:
                    return mLastMouseState.XButton2 == ButtonState.Pressed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mb), mb, null);
            }
        }

        public override bool KeyDown(Keys key)
        {
            if (mKeyDictionary.ContainsKey(key))
            {
                if (mLastKeyboardState.IsKeyDown(mKeyDictionary[key]))
                {
                    return true;
                }
            }

            return false;
        }

        public override Pointf GetMousePosition()
        {
            return new Pointf(mMouseX, mMouseY);
        }

        private void CheckMouseButton(Keys modifier, ButtonState bs, MouseButtons mb)
        {
            if (Globals.GameState == GameStates.Intro)
            {
                return; //No mouse input allowed while showing intro slides
            }

            if (bs == ButtonState.Pressed && !MouseButtonDown(mb))
            {
                Interface.Interface.GwenInput.ProcessMessage(
                    new GwenInputMessage(IntersectInput.InputEvent.MouseDown, GetMousePosition(), (int) mb, Keys.Alt)
                );

                Core.Input.OnMouseDown(modifier, mb);
            }
            else if (bs == ButtonState.Released && MouseButtonDown(mb))
            {
                Interface.Interface.GwenInput.ProcessMessage(
                    new GwenInputMessage(IntersectInput.InputEvent.MouseUp, GetMousePosition(), (int) mb, Keys.Alt)
                );

                Core.Input.OnMouseUp(modifier, mb);
            }
        }

        private void CheckMouseScrollWheel(int scrlVValue, int scrlHValue)
        {
            Pointf p = new Pointf(0, 0);

            if (scrlVValue != mMouseVScroll || scrlHValue != mMouseHScroll)
            {
                p = new Pointf(scrlHValue - mMouseHScroll, scrlVValue - mMouseVScroll);

                Interface.Interface.GwenInput.ProcessMessage(
                    new GwenInputMessage(IntersectInput.InputEvent.MouseScroll, p, (int)MouseButtons.Middle, Keys.Alt)
                );

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
                            IntersectInput.InputEvent.MouseMove, GetMousePosition(), (int)MouseButtons.None, Keys.Alt
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
                                Buttons.None => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.DPadUp => Microsoft.Xna.Framework.Input.Keys.Up,
                                Buttons.DPadDown => Microsoft.Xna.Framework.Input.Keys.Down,
                                Buttons.DPadLeft => Microsoft.Xna.Framework.Input.Keys.Left,
                                Buttons.DPadRight => Microsoft.Xna.Framework.Input.Keys.Right,
                                Buttons.Start => Microsoft.Xna.Framework.Input.Keys.Escape,
                                Buttons.Back => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftStick => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightStick => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftShoulder => Microsoft.Xna.Framework.Input.Keys.Back,
                                Buttons.RightShoulder => Microsoft.Xna.Framework.Input.Keys.Tab,
                                Buttons.BigButton => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.A => Microsoft.Xna.Framework.Input.Keys.Enter,
                                Buttons.B => Microsoft.Xna.Framework.Input.Keys.Back,
                                Buttons.X => Microsoft.Xna.Framework.Input.Keys.E,
                                Buttons.Y => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftThumbstickLeft => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightTrigger => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftTrigger => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightThumbstickUp => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightThumbstickDown => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightThumbstickRight => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.RightThumbstickLeft => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftThumbstickUp => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftThumbstickDown => Microsoft.Xna.Framework.Input.Keys.None,
                                Buttons.LeftThumbstickRight => Microsoft.Xna.Framework.Input.Keys.None,
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
                CheckMouseButton(modifier, mouseState.LeftButton, MouseButtons.Left);
                CheckMouseButton(modifier, mouseState.RightButton, MouseButtons.Right);
                CheckMouseButton(modifier, mouseState.MiddleButton, MouseButtons.Middle);
                CheckMouseButton(modifier, mouseState.XButton1, MouseButtons.X1);
                CheckMouseButton(modifier, mouseState.XButton2, MouseButtons.X2);

                CheckMouseScrollWheel(mouseState.ScrollWheelValue, mouseState.HorizontalScrollWheelValue);

                foreach (var key in mKeyDictionary)
                {
                    if (keyboardState.IsKeyDown(key.Value) && !mLastKeyboardState.IsKeyDown(key.Value))
                    {
                        Log.Diagnostic("{0} -> {1}", key.Key, key.Value);
                        Interface.Interface.GwenInput.ProcessMessage(
                            new GwenInputMessage(
                                IntersectInput.InputEvent.KeyDown, GetMousePosition(), (int) MouseButtons.None, key.Key
                            )
                        );

                        Core.Input.OnKeyPressed(modifier, key.Key);
                    }
                    else if (!keyboardState.IsKeyDown(key.Value) && mLastKeyboardState.IsKeyDown(key.Value))
                    {
                        Interface.Interface.GwenInput.ProcessMessage(
                            new GwenInputMessage(
                                IntersectInput.InputEvent.KeyUp, GetMousePosition(), (int) MouseButtons.None, key.Key
                            )
                        );

                        Core.Input.OnKeyReleased(modifier, key.Key);
                    }
                }

                mLastKeyboardState = keyboardState;
                mLastMouseState = mouseState;
                _lastGamePadState = gamePadState;
            }
            else
            {
                var modifier = GetPressedModifier(mLastKeyboardState);

                foreach (var key in mKeyDictionary)
                {
                    if (mLastKeyboardState.IsKeyDown(key.Value))
                    {
                        Interface.Interface.GwenInput.ProcessMessage(
                            new GwenInputMessage(
                                IntersectInput.InputEvent.KeyUp, GetMousePosition(), (int) MouseButtons.None, key.Key
                            )
                        );

                        Core.Input.OnKeyReleased(modifier, key.Key);
                    }
                }

                CheckMouseButton(modifier, ButtonState.Released, MouseButtons.Left);
                CheckMouseButton(modifier, ButtonState.Released, MouseButtons.Right);
                CheckMouseButton(modifier, ButtonState.Released, MouseButtons.Middle);
                CheckMouseButton(modifier, ButtonState.Released, MouseButtons.X1);
                CheckMouseButton(modifier, ButtonState.Released, MouseButtons.X2);
                mLastKeyboardState = new KeyboardState();
                mLastMouseState = new MouseState();
            }
        }

        public Keys GetPressedModifier(KeyboardState state)
        {
            var modifier = Keys.None;
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                modifier = Keys.Control;
            }

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
            {
                modifier = Keys.Shift;
            }

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt))
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

}
