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
