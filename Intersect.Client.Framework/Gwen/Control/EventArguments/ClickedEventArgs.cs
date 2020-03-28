using System;

namespace Intersect.Client.Framework.Gwen.Control.EventArguments
{

    public class ClickedEventArgs : EventArgs
    {

        internal ClickedEventArgs(int x, int y, bool down)
        {
            this.X = x;
            this.Y = y;
            this.MouseDown = down;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public bool MouseDown { get; private set; }

    }

}
