using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Intersect.Localization;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class LoginWindow
    {
        private Button _backBtn;
        private Button _loginBtn;

        private Label _loginHeader;

        //Controls
        private ImagePanel _loginWindow;

        //Parent
        private MainMenu _mainMenu;

        private ImagePanel _passwordBackground;
        private Label _passwordLabel;
        private TextBoxPassword _passwordTextbox;
        private string _savedPass = "";
        private LabeledCheckBox _savePassChk;

        //Controls
        private ImagePanel _usernameBackground;

        private Label _usernameLabel;
        private TextBox _usernameTextbox;

        private bool _useSavedPass;

        //Init
        public LoginWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _loginWindow = new ImagePanel(parent, "LoginWindow");

            //Menu Header
            _loginHeader = new Label(_loginWindow, "LoginHeader");
            _loginHeader.SetText(Strings.Get("login", "title"));

            _usernameBackground = new ImagePanel(_loginWindow, "UsernamePanel");

            //Login Username Label
            _usernameLabel = new Label(_usernameBackground, "UsernameLabel");
            _usernameLabel.SetText(Strings.Get("login", "username"));

            //Login Username Textbox
            _usernameTextbox = new TextBox(_usernameBackground, "UsernameField");
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;
            _usernameTextbox.Clicked += _usernameTextbox_Clicked;

            _passwordBackground = new ImagePanel(_loginWindow, "PasswordPanel");

            //Login Password Label
            _passwordLabel = new Label(_passwordBackground, "PasswordLabel");
            _passwordLabel.SetText(Strings.Get("login", "password"));

            //Login Password Textbox
            _passwordTextbox = new TextBoxPassword(_passwordBackground, "PasswordField");
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;
            _passwordTextbox.TextChanged += _passwordTextbox_TextChanged;

            //Login Save Pass Checkbox
            _savePassChk =
                new LabeledCheckBox(_loginWindow, "SavePassCheckbox") {Text = Strings.Get("login", "savepass")};

            //Login - Send Login Button
            _loginBtn = new Button(_loginWindow, "LoginButton");
            _loginBtn.SetText(Strings.Get("login", "login"));
            _loginBtn.Clicked += LoginBtn_Clicked;

            //Login - Back Button
            _backBtn = new Button(_loginWindow, "BackButton");
            _backBtn.SetText(Strings.Get("login", "back"));
            _backBtn.Clicked += BackBtn_Clicked;

            LoadCredentials();
        }

        private void _usernameTextbox_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(GameInput.KeyboardType.Normal, _usernameTextbox.Text, false, false,
                false);
        }

        //Methods
        public void Update()
        {
        }

        public void Hide()
        {
            _loginWindow.IsHidden = true;
        }

        public void Show()
        {
            _loginWindow.IsHidden = false;
        }

        //Input Handlers
        void _passwordTextbox_TextChanged(Base sender, EventArgs arguments)
        {
            _useSavedPass = false;
        }

        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _mainMenu.Show();
        }

        void UsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }

        void PasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }

        void LoginBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryLogin();
        }

        public void TryLogin()
        {
            var sha = new SHA256Managed();
            if (Globals.WaitingOnServer)
            {
                return;
            }
            if (GameNetwork.Connected)
            {
                if (FieldChecking.IsValidName(_usernameTextbox.Text))
                {
                    if (_useSavedPass)
                    {
                        GameFade.FadeOut();
                        PacketSender.SendLogin(_usernameTextbox.Text, _savedPass);
                        if (!_savePassChk.IsChecked) SaveCredentials();
                        Globals.WaitingOnServer = true;
                    }
                    else
                    {
                        if (FieldChecking.IsValidPass(_passwordTextbox.Text))
                        {
                            GameFade.FadeOut();
                            PacketSender.SendLogin(_usernameTextbox.Text,
                                BitConverter.ToString(
                                        sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim())))
                                    .Replace("-", ""));
                            SaveCredentials();
                            Globals.WaitingOnServer = true;
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add(
                                new KeyValuePair<string, string>("", Strings.Get("errors", "passwordinvalid")));
                        }
                    }
                }
                else
                {
                    Gui.MsgboxErrors.Add(
                        new KeyValuePair<string, string>("", Strings.Get("errors", "usernameinvalid")));
                }
            }
            else
            {
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Get("errors", "notconnected")));
            }
        }

        private void LoadCredentials()
        {
            string name = Globals.Database.LoadPreference("Username");
            if (!string.IsNullOrEmpty(name))
            {
                _usernameTextbox.Text = name;
                string pass = Globals.Database.LoadPreference("Password");
                if (!string.IsNullOrEmpty(pass))
                {
                    _passwordTextbox.Text = "*********";
                    _savedPass = pass;
                    _useSavedPass = true;
                    _savePassChk.IsChecked = true;
                }
            }
        }

        private void SaveCredentials()
        {
            var sha = new SHA256Managed();
            if (_savePassChk.IsChecked)
            {
                Globals.Database.SavePreference("Username", _usernameTextbox.Text.Trim());
                Globals.Database.SavePreference("Password",
                    BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim())))
                        .Replace("-", ""));
            }
            else
            {
                Globals.Database.SavePreference("Username", "");
                Globals.Database.SavePreference("Password", "");
            }
        }
    }
}