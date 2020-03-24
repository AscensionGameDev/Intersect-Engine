using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game
{
    class Draggable
    {
        ImagePanel mPnl;

        public static Draggable Active = null;

        public Draggable(int x, int y, GameTexture tex)
        {
            mPnl = new ImagePanel(Interface.GameUi.GameCanvas, "Draggable");
            mPnl.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            mPnl.SetPosition(InputHandler.MousePosition.X - mPnl.Width / 2,
                InputHandler.MousePosition.Y - mPnl.Height / 2);
            mPnl.Texture = tex;
            Active = this;
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
            if (Active == this) Active = null; ;
            Interface.GameUi.GameCanvas.RemoveChild(mPnl, false);
        }
    }
}