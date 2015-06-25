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
using Gwen.Control;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    class Draggable
    {
        ImagePanel pnl;
        public int x;
        public int y;

        public Draggable(int x, int y, Gwen.Texture tex)
        {
            pnl = new ImagePanel(Gui._GameGui.GameCanvas);
            pnl.SetPosition(x, y);
            pnl.SetSize(32, 32);
            pnl.Texture = tex;

        }

        public bool Update()
        {
            pnl.SetPosition(Gwen.Input.InputHandler.MousePosition.X - 16, Gwen.Input.InputHandler.MousePosition.Y - 16);
            x = Gwen.Input.InputHandler.MousePosition.X - 16;
            y = Gwen.Input.InputHandler.MousePosition.Y - 16;
            if (!Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Gui._GameGui.GameCanvas.RemoveChild(pnl,false);
        }
    }
}
