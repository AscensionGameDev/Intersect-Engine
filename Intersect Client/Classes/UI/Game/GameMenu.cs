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

using System.Drawing.Printing;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
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
        private Button _inventoryButton;
        private ImagePanel _inventoryBackground;
        private Button _spellsButton;
        private ImagePanel _spellsBackground;
        private Button _characterButton;
        private ImagePanel _characterBackground;
        private Button _questsButton;
        private ImagePanel _questsBackground;
        private Button _optionsButton;
        private ImagePanel _optionsBackground;
        private Button _closeButton;
        private ImagePanel _closeBackground;

        private int buttonWidth = 34;
        private int buttonHeight = 34;
        private int backgroundWidth = 42;
        private int backgroundHeight = 42;
        private int buttonMargin = 8;

        //Window References
        private OptionsWindow _optionsWindow;
        private InventoryWindow _inventoryWindow;
        private SpellWindow _spellsWindow;
        private CharacterWindow _characterWindow;

        //Init
        public GameMenu(Canvas _gameCanvas)
        {
            int buttonCount = 0;

            //Go in reverse order from the right
            _closeBackground = new ImagePanel(_gameCanvas);
            _closeBackground.MouseInputEnabled = false;
            _closeBackground.SetSize(backgroundWidth, backgroundHeight);
            _closeBackground.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin), _gameCanvas.Height - buttonMargin - backgroundHeight);
            _closeBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png");
            _closeButton = new Button(_gameCanvas);
            _closeButton.SetSize(buttonWidth, buttonHeight);
            _closeButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin) + 4, _gameCanvas.Height - buttonMargin - backgroundHeight + 4);
            _closeButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "exiticon.png"), Button.ControlState.Normal);
            _closeButton.SetToolTipText("Exit Game");
            _closeButton.Clicked += CloseBtn_Clicked;
            _closeButton.HoverEnter += Button_HoverEnter;
            _closeButton.HoverLeave += Button_HoverLeave;
            buttonCount ++;

            _optionsBackground = new ImagePanel(_gameCanvas);
            _optionsBackground.MouseInputEnabled = false;
            _optionsBackground.SetSize(backgroundWidth, backgroundHeight);
            _optionsBackground.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin), _gameCanvas.Height - buttonMargin - backgroundHeight);
            _optionsBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png");
            _optionsButton = new Button(_gameCanvas);
            _optionsButton.SetSize(buttonWidth, buttonHeight);
            _optionsButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin) + 4, _gameCanvas.Height - buttonMargin - backgroundHeight + 4);
            _optionsButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "optionsicon.png"), Button.ControlState.Normal);
            _optionsButton.SetToolTipText("Options");
            _optionsButton.Clicked += OptionBtn_Clicked;
            _optionsButton.HoverEnter += Button_HoverEnter;
            _optionsButton.HoverLeave += Button_HoverLeave;
            buttonCount++;

            //_questsButton = new Button(_gameCanvas);
            //_questsButton.SetSize(buttonWidth, buttonHeight);
            //_questsButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (buttonWidth + buttonMargin), _gameCanvas.Height - buttonMargin - buttonHeight);
            //_questsButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png"), Button.ControlState.Normal);
            //_questsButton.SetToolTipText("Quest Log");
            //buttonCount++;

            _characterBackground  = new ImagePanel(_gameCanvas);
            _characterBackground.MouseInputEnabled = false;
            _characterBackground.SetSize(backgroundWidth, backgroundHeight);
            _characterBackground.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin), _gameCanvas.Height - buttonMargin - backgroundHeight);
            _characterBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png");
            _characterButton = new Button(_gameCanvas);
            _characterButton.SetSize(buttonWidth, buttonHeight);
            _characterButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin) + 4, _gameCanvas.Height - buttonMargin - backgroundHeight + 4);
            _characterButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "charactericon.png"), Button.ControlState.Normal);
            _characterButton.SetToolTipText("Character Info");
            _characterButton.Clicked += CharacterButton_Clicked;
            _characterButton.HoverEnter += Button_HoverEnter;
            _characterButton.HoverLeave += Button_HoverLeave;
            buttonCount++;

            _spellsBackground = new ImagePanel(_gameCanvas);
            _spellsBackground.MouseInputEnabled = false;
            _spellsBackground.SetSize(backgroundWidth, backgroundHeight);
            _spellsBackground.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin), _gameCanvas.Height - buttonMargin - backgroundHeight);
            _spellsBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png");
            _spellsButton = new Button(_gameCanvas);
            _spellsButton.SetSize(buttonWidth, buttonHeight);
            _spellsButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin) + 4, _gameCanvas.Height - buttonMargin - backgroundHeight + 4);
            _spellsButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "spellicon.png"), Button.ControlState.Normal);
            _spellsButton.SetToolTipText("Spell Book");
            _spellsButton.Clicked += SpellsButton_Clicked;
            _spellsButton.HoverEnter += Button_HoverEnter;
            _spellsButton.HoverLeave += Button_HoverLeave;
            buttonCount++;

            _inventoryBackground = new ImagePanel(_gameCanvas);
            _inventoryBackground.MouseInputEnabled = false;
            _inventoryBackground.SetSize(backgroundWidth, backgroundHeight);
            _inventoryBackground.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin), _gameCanvas.Height - buttonMargin - backgroundHeight);
            _inventoryBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "menuitem.png");
            _inventoryButton = new Button(_gameCanvas);
            _inventoryButton.SetSize(buttonWidth, buttonHeight);
            _inventoryButton.SetPosition(_gameCanvas.Width - (buttonCount + 1) * (backgroundWidth + buttonMargin) + 4, _gameCanvas.Height - buttonMargin - backgroundHeight + 4);
            _inventoryButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inventoryicon.png"), Button.ControlState.Normal);
            _inventoryButton.SetToolTipText("Inventory");
            _inventoryButton.Clicked += InventoryButton_Clicked;
            _inventoryButton.HoverEnter += Button_HoverEnter;
            _inventoryButton.HoverLeave += Button_HoverLeave;


            //Assign Window References
            _optionsWindow = new OptionsWindow(_gameCanvas,null,null);
            _inventoryWindow = new InventoryWindow(_gameCanvas);
            _spellsWindow = new SpellWindow(_gameCanvas);
            _characterWindow = new CharacterWindow(_gameCanvas);
        }

        private void Button_HoverEnter(Base sender, System.EventArgs arguments)
        {
            ((Button) sender).RenderColor = new Color(180, 255, 255, 255);
        }

        private void Button_HoverLeave(Base sender, System.EventArgs arguments)
        {
            ((Button)sender).RenderColor = new Color(255, 255, 255, 255);
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
        void SpellsButton_Clicked(Base sender, ClickedEventArgs arguments)
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
