using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        //Init
        public MainMenu(Canvas _menuCanvas)
        {
            //Main Menu Window
            _menuWindow = new WindowControl(_menuCanvas, "Main Menu");
            _menuWindow.SetSize(200, 200);
            _menuWindow.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
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
            GameMain.IsRunning = false;
        }
    }
}
