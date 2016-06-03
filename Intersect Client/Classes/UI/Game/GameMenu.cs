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

using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using IntersectClientExtras.Gwen.Control.EventArguments;

namespace Intersect_Client.Classes.UI.Game
{
    class GameMenu
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
        private InventoryWindow _inventoryWindow;
        private SpellWindow _spellsWindow;
        private CharacterWindow _characterWindow;

        //Init
        public GameMenu(Canvas _gameCanvas) {
            _gameMenu = new WindowControl(_gameCanvas, "Game Menu") { IsClosable = false };
            _gameMenu.DisableResizing();
            _gameMenu.SetSize(166, 84);
            _gameMenu.SetPosition(GameGraphics.Renderer.GetScreenWidth() - 116, GameGraphics.Renderer.GetScreenHeight() - 84);
            _gameMenu.Margin = Margin.Zero;
            _gameMenu.Padding = Padding.Zero;

            _inventoryButton = new Button(_gameMenu);
            _inventoryButton.SetSize(50, 24);
            _inventoryButton.SetText("Inventory");
            _inventoryButton.SetPosition(4, 4);
            _inventoryButton.Clicked += InventoryButton_Clicked;

            _skillsButton = new Button(_gameMenu);
            _skillsButton.SetSize(50, 24);
            _skillsButton.SetText("Skills");
            _skillsButton.SetPosition(58, 4);
            _skillsButton.Clicked += SkillsButton_Clicked;

            _characterButton = new Button(_gameMenu);
            _characterButton.SetSize(50, 24);
            _characterButton.SetText("Character");
            _characterButton.SetPosition(112, 4);
            _characterButton.Clicked += CharacterButton_Clicked;

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
            _optionsWindow = new OptionsWindow(_gameCanvas,null,null);
            _inventoryWindow = new InventoryWindow(_gameCanvas);
            _spellsWindow = new SpellWindow(_gameCanvas);
            _characterWindow = new CharacterWindow(_gameCanvas);
        }

        //Methods
        public void Update()
        {
            _inventoryWindow.Update();
            _spellsWindow.Update();
            _characterWindow.Update();
        }


        //Input Handlers
        void CloseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }
        void OptionBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_optionsWindow.IsVisible())
            {
                _optionsWindow.Hide();
            }
            else
            {
                _optionsWindow.Show();
            }
        }
        void InventoryButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_inventoryWindow.IsVisible())
            {
                _inventoryWindow.Hide();
            }
            else
            {
                _inventoryWindow.Show();
            }
        }
        void SkillsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_spellsWindow.IsVisible())
            {
                _spellsWindow.Hide();
            }
            else
            {
                _spellsWindow.Show();
            }
        }
        void CharacterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (_characterWindow.IsVisible())
            {
                _characterWindow.Hide();
            }
            else
            {
                _characterWindow.Show();
            }
        }
        
    }
}
