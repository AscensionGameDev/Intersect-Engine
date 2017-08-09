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

        public Draggable(int x, int y, GameTexture tex)
        {
            pnl = new ImagePanel(Gui.GameUI.GameCanvas, "Draggable");
            Gui.LoadRootUIData(pnl, "InGame.xml");
            pnl.SetPosition(InputHandler.MousePosition.X - pnl.Width / 2,
                InputHandler.MousePosition.Y - pnl.Height / 2);
            pnl.Texture = tex;
        }

        public int X
        {
            get { return pnl.X; }
            set { pnl.X = value; }
        }

        public int Y
        {
            get { return pnl.Y; }
            set { pnl.Y = value; }
        }

        public bool Update()
        {
            pnl.SetPosition(InputHandler.MousePosition.X - pnl.Width / 2,
                InputHandler.MousePosition.Y - pnl.Height / 2);
            if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Gui.GameUI.GameCanvas.RemoveChild(pnl, false);
        }
    }
}