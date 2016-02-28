/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.UI;
using SFML.Window;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Input
{
    public class SfmlInput : GameInput
    {
        private Dictionary<GameInput.MouseButtons, bool> _mouseDictionary = new Dictionary<MouseButtons, bool>();
        private Dictionary<Keys, bool> _keyDictionary = new Dictionary<Keys, bool>();
        private float _mouseX = 0f;
        private float _mouseY = 0f;

        public SfmlInput()
        {
            foreach (MouseButtons mb in Enum.GetValues(typeof(MouseButtons)))
            {
                _mouseDictionary.Add(mb,false);
            }
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!_keyDictionary.ContainsKey(key)) _keyDictionary.Add(key, false);
            }
        }
        public override bool MouseButtonDown(MouseButtons mb)
        {
            return _mouseDictionary[mb];
        }

        public override bool KeyDown(Keys key)
        {
            return _keyDictionary[key];
        }

        public override Pointf GetMousePosition()
        {
            return new Pointf(_mouseX, _mouseY);
        }

        public override void Update()
        {
            
        }


        public void RenderWindow_MouseButtonReleased(Object sender, global::SFML.Window.MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    _mouseDictionary[MouseButtons.Left] = false;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseUp,
                GetMousePosition(), (int)MouseButtons.Left, Keys.Alt));
                    break;
                case Mouse.Button.Right:
                    _mouseDictionary[MouseButtons.Right] = false;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseUp,
                GetMousePosition(), (int)MouseButtons.Right, Keys.Alt));
                    break;
                case Mouse.Button.Middle:
                    _mouseDictionary[MouseButtons.Middle] = false;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseUp,
                GetMousePosition(), (int)MouseButtons.Middle, Keys.Alt));
                    break;
            }
        }

        public void RenderWindow_TextEntered(object sender, TextEventArgs e)
        {
            Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.TextEntered,
               GetMousePosition(), (int)MouseButtons.None, Keys.Alt, false,false,false,e.Unicode));
        }

        public void RenderWindow_MouseButtonPressed(Object sender, global::SFML.Window.MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case Mouse.Button.Left:
                    _mouseDictionary[MouseButtons.Left] = true;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseDown,
                GetMousePosition(), (int)MouseButtons.Left, Keys.Alt));
                    break;
                case Mouse.Button.Right:
                    _mouseDictionary[MouseButtons.Right] = true;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseDown,
                GetMousePosition(), (int)MouseButtons.Right, Keys.Alt));
                    break;
                case Mouse.Button.Middle:
                    _mouseDictionary[MouseButtons.Middle] = true;
                    Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseDown,
                GetMousePosition(), (int)MouseButtons.Middle, Keys.Alt));
                    break;
            }
            
        }

        internal void RenderWindow_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.MouseMove,
                GetMousePosition(), (int)MouseButtons.None,  Keys.Alt));
        }

        public void RenderWindow_KeyReleased(Object sender, global::SFML.Window.KeyEventArgs e)
        {
            try
            {
                
                string keyName = Enum.GetName(typeof(Keyboard.Key), e.Code);
                if (keyName.ToLower() == "backspace") keyName = "Back";
                if (keyName.ToLower().IndexOf("num") == 0) keyName = "D" + keyName.Substring(3);
                Keys key = (Keys)Enum.Parse(typeof(Keys), keyName, true);
                Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.KeyUp,
                GetMousePosition(), (int)MouseButtons.None, key));
                GameInputHandler.OnKeyReleased(key);
                _keyDictionary[key] = false;
            }
            catch (Exception)
            {

            }
            
        }

        public void RenderWindow_KeyPressed(Object sender, global::SFML.Window.KeyEventArgs e)
        {
            try
            {
                string keyName = Enum.GetName(typeof(Keyboard.Key), e.Code);
                if (keyName.ToLower() == "backspace") keyName = "Back";
                if (keyName.ToLower().IndexOf("num") == 0) keyName = "D" + keyName.Substring(3);
                Keys key = (Keys)Enum.Parse(typeof(Keys), keyName, true);
                Gui.GwenInput.ProcessMessage(new GwenInputMessage(IntersectInput.InputEvent.KeyDown,
                GetMousePosition(), (int)MouseButtons.None, key));
                GameInputHandler.OnKeyPressed(key);
                _keyDictionary[key] = true;
            }
            catch (Exception)
            {

            }
        }
    }
}
