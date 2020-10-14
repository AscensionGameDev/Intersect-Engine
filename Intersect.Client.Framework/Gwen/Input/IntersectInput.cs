using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.Input
{

    public class IntersectInput : InputBase
    {

        public enum InputEvent
        {

            MouseMove = 0,

            MouseDown,

            MouseUp,

            KeyDown,

            KeyUp,

            TextEntered,

            MouseScroll,
        }

        private Canvas mCanvas;

        private bool mInitialized = false;

        private int mMouseX;

        private int mMouseY;

        public override void Initialize(Canvas canvas)
        {
            mCanvas = canvas;
            mInitialized = true;
        }

        public override Key TranslateKeyCode(object sfKey)
        {
            switch ((Keys) sfKey)
            {
                case Keys.Back:
                    return Key.Backspace;
                case Keys.Return:
                    return Key.Return;
                case Keys.Escape:
                    return Key.Escape;
                case Keys.Tab:
                    return Key.Tab;
                case Keys.Space:
                    return Key.Space;
                case Keys.Up:
                    return Key.Up;
                case Keys.Down:
                    return Key.Down;
                case Keys.Left:
                    return Key.Left;
                case Keys.Right:
                    return Key.Right;
                case Keys.Home:
                    return Key.Home;
                case Keys.End:
                    return Key.End;
                case Keys.Delete:
                    return Key.Delete;
                case Keys.Control:
                    return Key.Control;
                case Keys.Alt:
                    return Key.Alt;
                case Keys.Shift:
                    return Key.Shift;
            }

            return Key.Invalid;
        }

        public override char TranslateChar(object sfKey)
        {
            var keyCode = (Keys) sfKey;
            if (keyCode >= Keys.A && keyCode <= Keys.Z)
            {
                return (char) ('A' + ((int) keyCode - (int) Keys.A));
            }

            return ' ';
        }

        public override bool ProcessMessage(object message)
        {
            if (!mInitialized || !HandleInput)
            {
                return false;
            }

            var msg = (GwenInputMessage) message;
            Key key;
            switch (msg.Type)
            {
                case InputEvent.MouseMove:
                    var dx = (int) (int) msg.MousePosition.X - mMouseX;
                    var dy = (int) (int) msg.MousePosition.Y - mMouseY;

                    mMouseX = (int) (int) msg.MousePosition.X;
                    mMouseY = (int) (int) msg.MousePosition.Y;

                    return mCanvas.Input_MouseMoved(mMouseX, mMouseY, dx, dy);
                case InputEvent.MouseDown:
                    return mCanvas.Input_MouseButton((int) msg.MouseBtn, true);
                case InputEvent.MouseUp:
                    return mCanvas.Input_MouseButton((int) msg.MouseBtn, false);
                case InputEvent.TextEntered:
                    return mCanvas.Input_Character((char) msg.Unicode[0]);
                case InputEvent.KeyDown:
                    var ch = TranslateChar(msg.Key);
                    if ((int) msg.MouseBtn > -1 && InputHandler.DoSpecialKeys(mCanvas, ch))
                    {
                        return false;
                    }

                    key = TranslateKeyCode(msg.Key);
                    if (key == Key.Invalid || key == Key.Space)
                    {
                        return InputHandler.HandleAccelerator(mCanvas, ch);
                    }

                    return mCanvas.Input_Key(key, true); //TODO FIX THIS LAST PARAMETER
                case InputEvent.KeyUp:
                    key = TranslateKeyCode(msg.Key);

                    return mCanvas.Input_Key(key, false); //TODO FIX THIS LAST PARAMETER
                case InputEvent.MouseScroll:
                    return mCanvas.Input_MouseScroll((int)msg.MousePosition.X, (int)msg.MousePosition.Y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class GwenInputMessage
    {

        public bool Alt;

        public bool Control;

        public Keys Key;

        public int MouseBtn;

        public Pointf MousePosition;

        public bool Shift;

        public IntersectInput.InputEvent Type;

        public string Unicode = "";

        public GwenInputMessage(
            IntersectInput.InputEvent type,
            Pointf mousePos,
            int mousebtn,
            Keys keyAction,
            bool alt = false,
            bool control = false,
            bool shift = false,
            string unicode = ""
        )
        {
            this.Type = type;
            MousePosition = mousePos;
            Key = keyAction;
            MouseBtn = mousebtn;
            Alt = alt;
            Control = control;
            Shift = shift;
            Unicode = unicode;
        }

    }

}
