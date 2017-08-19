using IntersectClientExtras.Gwen.Control;

namespace Intersect_Client.Classes.UI.Menu
{
    public class MenuGuiBase
    {
        private readonly Canvas _menuCanvas;

        public MainMenu _mainMenu;
        private bool shouldReset;

        public MenuGuiBase(Canvas myCanvas)
        {
            _menuCanvas = myCanvas;
            InitMenuGui();
        }

        private void InitMenuGui()
        {
            _mainMenu = new MainMenu(_menuCanvas);
        }

        public void Draw()
        {
            if (shouldReset)
            {
                _mainMenu.Reset();
                shouldReset = false;
            }
            _mainMenu.Update();
            _menuCanvas.RenderCanvas();
        }

        public void Reset()
        {
            shouldReset = true;
        }
    }
}