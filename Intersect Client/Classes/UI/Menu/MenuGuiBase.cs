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
            _mainMenu.Update();
            if (shouldReset)
            {
                _mainMenu.Reset();
                shouldReset = false;
            }
            _menuCanvas.RenderCanvas();
        }

        public void Reset()
        {
            shouldReset = true;
        }
    }
}