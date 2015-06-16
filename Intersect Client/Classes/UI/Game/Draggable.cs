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

        public Draggable(int x, int y, string image)
        {
            pnl = new ImagePanel(Gui._GameGui.GameCanvas);
            pnl.SetPosition(x, y);
            pnl.SetSize(32, 32);
            pnl.ImageName = image;

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
