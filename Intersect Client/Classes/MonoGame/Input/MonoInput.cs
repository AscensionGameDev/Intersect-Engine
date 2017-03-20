using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.UI;
using Intersect_Client_MonoGame.Classes.SFML.Graphics;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Keys = IntersectClientExtras.GenericClasses.Keys;

namespace Intersect_Client_MonoGame.Classes.SFML.Input
{
    public class MonoInput : GameInput
    {
        private MouseState lastMouseState;
        private KeyboardState lastKeyboardState;
        private Dictionary<Keys,Microsoft.Xna.Framework.Input.Keys> _keyDictionary; 
        private int _mouseX = 0;
        private int _mouseY = 0;
        private Game _myGame;

        public MonoInput(Game myGame)
        {
            myGame.Window.TextInput += Window_TextInput;
            _myGame = myGame;
            _keyDictionary = new Dictionary<Keys, Microsoft.Xna.Framework.Input.Keys>();
            foreach (Keys key in Enum.GetValues(typeof (Keys)))
            {
                if (!_keyDictionary.ContainsKey(key))
                {
                    foreach (
                        Microsoft.Xna.Framework.Input.Keys monoKey in
                            Enum.GetValues(typeof (Microsoft.Xna.Framework.Input.Keys)))
                    {
                        if (key == Keys.Shift)
                        {
                            _keyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.LeftShift);
                            break;
                        }
                        if (key == Keys.Return)
                        {
                            _keyDictionary.Add(key, Microsoft.Xna.Framework.Input.Keys.Enter);
                            break;
                        }
                        else
                        {
                            if (key.ToString() == monoKey.ToString())
                            {
                                _keyDictionary.Add(key, monoKey);
                                break;
                            }
                        }
                    }
                }
                if (!_keyDictionary.ContainsKey(key))
                {
                    Console.WriteLine("Mono does not have a key to match: " + key.ToString());
                }
            }
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.TextEntered,
               GetMousePosition(), (int)MouseButtons.None, Keys.Alt, false, false, false, e.Character.ToString()));
        }

        public override bool MouseButtonDown(MouseButtons mb)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    return (lastMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.Right:
                    return (lastMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.Middle:
                    return (lastMouseState.MiddleButton == ButtonState.Pressed);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mb), mb, null);
            }
        }

        public override bool KeyDown(Keys key)
        {
            if (_keyDictionary.ContainsKey(key))
            {
                if (lastKeyboardState.IsKeyDown(_keyDictionary[key]))
                {
                    return true;
                }
            }
            return false;
        }

        public override Pointf GetMousePosition()
        {
            return new Pointf(_mouseX, _mouseY);
        }

        private void CheckMouseButton(ButtonState bs, MouseButtons mb)
        {
            if (bs == ButtonState.Pressed && !MouseButtonDown(mb))
            {
                Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseDown,
                   GetMousePosition(), (int)mb, Keys.Alt));
                GameInputHandler.OnMouseDown(mb);
            }
            else if (bs == ButtonState.Released && MouseButtonDown(mb))
            {
                Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseUp,
                   GetMousePosition(), (int)mb, Keys.Alt));
                GameInputHandler.OnMouseUp(mb);
            }
        }

        public override void Update()
        {
            if (_myGame.IsActive)
            {
                KeyboardState kbState = Keyboard.GetState();
                MouseState mState = Mouse.GetState();

                if (mState.X != _mouseX || mState.Y != _mouseY)
                {
                    _mouseX = (int)(mState.X * ((MonoRenderer)GameGraphics.Renderer).GetMouseOffset().X);
                    _mouseY = (int)(mState.Y * ((MonoRenderer)GameGraphics.Renderer).GetMouseOffset().Y);
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseMove,
                        GetMousePosition(), (int) MouseButtons.None, Keys.Alt));
                }

                //Check for state changes in the left mouse button
                CheckMouseButton(mState.LeftButton, MouseButtons.Left);
                CheckMouseButton(mState.RightButton, MouseButtons.Right);
                CheckMouseButton(mState.MiddleButton, MouseButtons.Middle);

                foreach (KeyValuePair<Keys, Microsoft.Xna.Framework.Input.Keys> key in _keyDictionary)
                {
                    if (kbState.IsKeyDown(key.Value) && !lastKeyboardState.IsKeyDown(key.Value))
                    {
                        Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.KeyDown,
                            GetMousePosition(), (int) MouseButtons.None, key.Key));
                        GameInputHandler.OnKeyPressed(key.Key);
                    }
                    else if (!kbState.IsKeyDown(key.Value) && lastKeyboardState.IsKeyDown(key.Value))
                    {
                        Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.KeyUp,
                            GetMousePosition(), (int) MouseButtons.None, key.Key));
                        GameInputHandler.OnKeyReleased(key.Key);
                    }
                }

                lastKeyboardState = kbState;
                lastMouseState = mState;
            }
            else
            {
                foreach (KeyValuePair<Keys, Microsoft.Xna.Framework.Input.Keys> key in _keyDictionary)
                {
                    if (lastKeyboardState.IsKeyDown(key.Value))
                    {
                        Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.KeyUp,
                            GetMousePosition(), (int)MouseButtons.None, key.Key));
                        GameInputHandler.OnKeyReleased(key.Key);
                    }
                }
                CheckMouseButton(ButtonState.Released, MouseButtons.Left);
                CheckMouseButton(ButtonState.Released, MouseButtons.Right);
                CheckMouseButton(ButtonState.Released, MouseButtons.Middle);
                lastKeyboardState = new KeyboardState();
                lastMouseState = new MouseState();
            }
        }

        public override void OpenKeyboard(KeyboardType type, string text, bool autoCorrection, bool multiLine, bool secure)
        {
            return; //no on screen keyboard for pc clients
        }
    }
}
