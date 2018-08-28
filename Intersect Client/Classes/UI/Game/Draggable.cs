using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect.Client.Classes.Core;

namespace Intersect_Client.Classes.UI.Game
{
    class Draggable
    {
        ImagePanel mPnl;

        public Draggable(int x, int y, GameTexture tex)
        {
            mPnl = new ImagePanel(Gui.GameUi.GameCanvas, "Draggable");
            mPnl.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
            mPnl.SetPosition(InputHandler.MousePosition.X - mPnl.Width / 2,
                InputHandler.MousePosition.Y - mPnl.Height / 2);
            mPnl.Texture = tex;
        }

        public int X
        {
            get { return mPnl.X; }
            set { mPnl.X = value; }
        }

        public int Y
        {
            get { return mPnl.Y; }
            set { mPnl.Y = value; }
        }

        public bool Update()
        {
            mPnl.SetPosition(InputHandler.MousePosition.X - mPnl.Width / 2,
                InputHandler.MousePosition.Y - mPnl.Height / 2);
            if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Gui.GameUi.GameCanvas.RemoveChild(mPnl, false);
        }
    }
}