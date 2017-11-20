using System;
using System.Collections.Generic;
using Intersect;
using Intersect.Localization;
using Intersect.Utilities;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class RegisterWindow
    {
        private Button _backBtn;

        private ImagePanel _emailBackground;
        private Label _emailLabel;
        private TextBox _emailTextbox;

        //Parent
        private MainMenu _mainMenu;

        private ImagePanel _passwordBackground;

        private ImagePanel _passwordBackground2;
        private Label _passwordLabel;
        private Label _passwordLabel2;
        private TextBoxPassword _passwordTextbox;
        private TextBoxPassword _passwordTextbox2;
        private Button _registerBtn;

        private Label _registrationHeader;

        //Controls
        private ImagePanel _registrationPanel;

        private ImagePanel _usernameBackground;
        private Label _usernameLabel;
        private TextBox _usernameTextbox;

        //Init
        public RegisterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _registrationPanel = new ImagePanel(parent, "RegistrationWindow");

            //Menu Header
            _registrationHeader = new Label(_registrationPanel, "RegistrationLabel");
            _registrationHeader.SetText(Strings.Get("registration", "title"));

            //Register Username Background
            _usernameBackground = new ImagePanel(_registrationPanel, "UsernamePanel");

            //Register Username Label
            _usernameLabel = new Label(_usernameBackground, "UsernameLabel");
            _usernameLabel.SetText(Strings.Get("registration", "username"));

            //Register Username Textbox
            _usernameTextbox = new TextBox(_usernameBackground, "UsernameField");
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;

            //Register Email Background
            _emailBackground = new ImagePanel(_registrationPanel, "EmailPanel");

            //Register Email Label
            _emailLabel = new Label(_emailBackground, "EmailLabel");
            _emailLabel.SetText(Strings.Get("registration", "email"));

            //Register Email Textbox
            _emailTextbox = new TextBox(_emailBackground, "EmailField");
            _emailTextbox.SubmitPressed += EmailTextbox_SubmitPressed;

            //Register Password Background
            _passwordBackground = new ImagePanel(_registrationPanel, "Password1Panel");

            //Register Password Label
            _passwordLabel = new Label(_passwordBackground, "Password1Label");
            _passwordLabel.SetText(Strings.Get("registration", "password"));

            //Register Password Textbox
            _passwordTextbox = new TextBoxPassword(_passwordBackground, "Password1Field");
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;

            //Register Password Background
            _passwordBackground2 = new ImagePanel(_registrationPanel, "Password2Panel");

            //Register Password Label2
            _passwordLabel2 = new Label(_passwordBackground2, "Password2Label");
            _passwordLabel2.SetText(Strings.Get("registration", "confirmpass"));

            //Register Password Textbox2
            _passwordTextbox2 = new TextBoxPassword(_passwordBackground2, "Password2Field");
            _passwordTextbox2.SubmitPressed += PasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            _registerBtn = new Button(_registrationPanel, "RegisterButton");
            _registerBtn.SetText(Strings.Get("registration", "register"));
            _registerBtn.Clicked += RegisterBtn_Clicked;

            //Register - Back Button
            _backBtn = new Button(_registrationPanel, "BackButton");
            _backBtn.SetText(Strings.Get("registration", "back"));
            _backBtn.Clicked += BackBtn_Clicked;
        }

        //Methods
        public void Update()
        {
        }

        public void Show()
        {
            _registrationPanel.Show();
        }

        public void Hide()
        {
            _registrationPanel.Hide();
        }

        void TryRegister()
        {
            if (Globals.WaitingOnServer)
            {
                return;
            }
            if (GameNetwork.Connected)
            {
                if (FieldChecking.IsValidUsername(_usernameTextbox.Text))
                {
                    if (_passwordTextbox.Text == _passwordTextbox2.Text)
                    {
                        if (FieldChecking.IsValidPassword(_passwordTextbox.Text))
                        {
                            if (FieldChecking.IsWellformedEmailAddress(_emailTextbox.Text))
                            {
                                GameFade.FadeOut();
                                Hide();
                                PacketSender.SendCreateAccount(_usernameTextbox.Text, _passwordTextbox.Text,
                                    _emailTextbox.Text);
                                Globals.WaitingOnServer = true;
                            }
                            else
                            {
                                Gui.MsgboxErrors.Add(
                                    new KeyValuePair<string, string>("", Strings.Get("registration", "emailinvalid")));
                            }
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add(
                                new KeyValuePair<string, string>("", Strings.Get("errors", "passwordinvalid")));
                        }
                    }
                    else
                    {
                        Gui.MsgboxErrors.Add(
                            new KeyValuePair<string, string>("", Strings.Get("registration", "passwordmatch")));
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

        //Input Handlers
        void UsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }

        void EmailTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }

        void PasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }

        void PasswordTextbox2_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }

        void RegisterBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryRegister();
        }

        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _mainMenu.Show();
        }
    }
}