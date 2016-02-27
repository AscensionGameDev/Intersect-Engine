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
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Menu
{
    public class MainMenu : IGUIElement
    {
        //Controls
        private WindowControl _menuWindow;

        private Button _loginButton;
        private Button _registerButton;
        private Button _optionsButton;
        private Button _exitButton;

        private OptionsWindow _optionControls;
        private LoginControls _loginControls;
        private RegisterControls _registerControls;
        private CreateCharControls _CreateCharControls;

        private bool _shouldOpenCharacterCreation;

        //Character creation feild check
        private bool HasMadeCharacterCreation = false;

        //Init
        public MainMenu(Canvas _menuCanvas)
        {
            //Main Menu Window
            _menuWindow = new WindowControl(_menuCanvas, "Main Menu");
            _menuWindow.SetSize(200, 200);
            _menuWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 100, GameGraphics.Renderer.GetScreenHeight() / 2 - 80);
            _menuWindow.IsClosable = false;
            _menuWindow.DisableResizing();
            _menuWindow.Margin = Margin.Zero;
            _menuWindow.Padding = Padding.Zero;

            //Login Button
            _loginButton = new Button(_menuWindow);
            _loginButton.SetText("Login");
            _loginButton.SetSize(120, 32);
            _loginButton.SetPosition(_menuWindow.Width / 2 - 120 / 2, 14);
            _loginButton.Clicked += LoginButton_Clicked;

            //Register Button
            _registerButton = new Button(_menuWindow);
            _registerButton.SetText("Register");
            _registerButton.SetSize(120, 32);
            _registerButton.SetPosition(_menuWindow.Width / 2 - 120 / 2, 54);
            _registerButton.Clicked += RegisterButton_Clicked;

            //Options Button
            _optionsButton = new Button(_menuWindow);
            _optionsButton.SetText("Options");
            _optionsButton.SetSize(120, 32);
            _optionsButton.SetPosition(_menuWindow.Width / 2 - 120 / 2, 94);
            _optionsButton.Clicked += OptionsButton_Clicked;

            //Exit Button
            _exitButton = new Button(_menuWindow);
            _exitButton.SetText("Exit");
            _exitButton.SetSize(120, 32);
            _exitButton.SetPosition(_menuWindow.Width / 2 - 120 / 2, 134);
            _exitButton.Clicked += ExitButton_Clicked;

            //Options Controls
            _optionControls = new OptionsWindow(_menuWindow, false, this);

            //Login Controls
            _loginControls = new LoginControls(_menuWindow, this);

            //Register Controls
            _registerControls = new RegisterControls(_menuWindow, this);

            Reset();
        }

        //Methods
        public void Update()
        {
            if (_shouldOpenCharacterCreation)
            {
                CreateCharacterCreation();
            }
        }
        public void Reset()
        {
            //Show Menu Elements
            _menuWindow.Title = "Main Menu";
            _registerButton.Show();
            _exitButton.Show();
            _loginButton.Show();
            _optionsButton.Show();

            //Hide Login Elements
            _loginControls.Hide();

            //Hide Register Elements
            _registerControls.Hide();

            //Hide option elements
            _optionControls.Hide();

            //Hide Create Char Elements
            if (HasMadeCharacterCreation == true)
            {
                _CreateCharControls.Hide();
            }
        }

        public void NotifyOpenCharacterCreation()
        {
            _shouldOpenCharacterCreation = true;
        }

        public void CreateCharacterCreation()
        {
            _menuWindow.Title = "Create Character";
            _registerControls.Hide();
            _loginControls.Hide();
            _CreateCharControls = new CreateCharControls(_menuWindow);
            _CreateCharControls.Show();
            HasMadeCharacterCreation = true;
            _shouldOpenCharacterCreation = false;
        }

        //Input Handlers
        void LoginButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _menuWindow.Title = "Login";
            _registerButton.Hide();
            _exitButton.Hide();
            _loginButton.Hide();
            _optionsButton.Hide();
            _loginControls.Show();
        }
        void RegisterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _menuWindow.Title = "Register";
            _registerButton.Hide();
            _exitButton.Hide();
            _loginButton.Hide();
            _optionsButton.Hide();
            _registerControls.Show();
        }
        void OptionsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _menuWindow.Title = "Options";
            _registerButton.Hide();
            _exitButton.Hide();
            _loginButton.Hide();
            _optionsButton.Hide();
            _optionControls.Show();
        }
        void ExitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }
    }
}
