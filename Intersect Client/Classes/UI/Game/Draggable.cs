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

using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    class Draggable
    {
        ImagePanel pnl;
        public int x;
        public int y;

        public Draggable(int x, int y, GameTexture tex)
        {
            pnl = new ImagePanel(Gui.GameUI.GameCanvas);
            pnl.SetPosition(x, y);
            pnl.SetSize(32, 32);
            pnl.Texture = tex;

        }

        public bool Update()
        {
            pnl.SetPosition(InputHandler.MousePosition.X - 16, InputHandler.MousePosition.Y - 16);
            x = InputHandler.MousePosition.X - 16;
            y = InputHandler.MousePosition.Y - 16;
            if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Gui.GameUI.GameCanvas.RemoveChild(pnl,false);
        }
    }
}
