using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    class GameMenu : IGUIElement
    {
        //Control Variables
        private WindowControl _gameMenu;
        private Button _inventoryButton;
        private Button _skillsButton;
        private Button _characterButton;
        private Button _questsButton;
        private Button _optionsButton;
        private Button _closeButton;

        //Window References
        private OptionsWindow _optionsWindow;

        //Init
        public GameMenu(Canvas _gameCanvas, OptionsWindow optionsWindow) {
            _gameMenu = new WindowControl(_gameCanvas, "Game Menu") { IsClosable = false };
            _gameMenu.DisableResizing();
            _gameMenu.SetSize(166, 84);
            _gameMenu.SetPosition(Graphics.ScreenWidth - 116, Graphics.ScreenHeight - 84);
            _gameMenu.Margin = Margin.Zero;
            _gameMenu.Padding = Padding.Zero;

            _inventoryButton = new Button(_gameMenu);
            _inventoryButton.SetSize(50, 24);
            _inventoryButton.SetText("Inventory");
            _inventoryButton.SetPosition(4, 4);

            _skillsButton = new Button(_gameMenu);
            _skillsButton.SetSize(50, 24);
            _skillsButton.SetText("Skills");
            _skillsButton.SetPosition(58, 4);

            _characterButton = new Button(_gameMenu);
            _characterButton.SetSize(50, 24);
            _characterButton.SetText("Character");
            _characterButton.SetPosition(112, 4);

            _questsButton = new Button(_gameMenu);
            _questsButton.SetSize(50, 24);
            _questsButton.SetText("Tasks");
            _questsButton.SetPosition(4, 32);

            _optionsButton = new Button(_gameMenu);
            _optionsButton.SetSize(50, 24);
            _optionsButton.SetText("Options");
            _optionsButton.SetPosition(58, 32);
            _optionsButton.Clicked += OptionBtn_Clicked;

            _closeButton = new Button(_gameMenu);
            _closeButton.SetSize(50, 24);
            _closeButton.SetText("Exit");
            _closeButton.SetPosition(112, 32);
            _closeButton.Clicked += CloseBtn_Clicked;

            //Assign Window References
            _optionsWindow = optionsWindow;
        }

        //Methods
        public void Update()
        {

        }


        //Input Handlers
        void CloseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameMain.IsRunning = false;
        }
        void OptionBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _optionsWindow.Show();
        }
        
    }
}
