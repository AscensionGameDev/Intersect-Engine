using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gwen;
using Gwen.Control;

namespace Intersect_Client.Classes.UI.Menu
{
    public class MenuGuiBase
    {
        private readonly Canvas _menuCanvas;
        public MenuGuiBase(Canvas myCanvas)
        {
            _menuCanvas = myCanvas;
            InitMenuGui();
        }

        private MainMenu _mainMenu;

        private void InitMenuGui()
        {
            _mainMenu = new MainMenu(_menuCanvas);
        }

        public void Draw()
        {
            _menuCanvas.RenderCanvas();
        }

        public void Reset()
        {
            _mainMenu.Reset();
        }

        
    }
}
