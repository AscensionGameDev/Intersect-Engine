using System;
using System.Collections.Generic;
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
        private Button mBackBtn;

        private ImagePanel mEmailBackground;
        private Label mEmailLabel;
        private TextBox mEmailTextbox;

        //Parent
        private MainMenu mMainMenu;

        private ImagePanel mPasswordBackground;

        private ImagePanel mPasswordBackground2;
        private Label mPasswordLabel;
        private Label mPasswordLabel2;
        private TextBoxPassword mPasswordTextbox;
        private TextBoxPassword mPasswordTextbox2;
        private Button mRegisterBtn;

        private Label mRegistrationHeader;

        //Controls
        private ImagePanel mRegistrationPanel;

        private ImagePanel mUsernameBackground;
        private Label mUsernameLabel;
        private TextBox mUsernameTextbox;

        //Init
        public RegisterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mRegistrationPanel = new ImagePanel(parent, "RegistrationWindow");

            //Menu Header
            mRegistrationHeader = new Label(mRegistrationPanel, "RegistrationLabel");
            mRegistrationHeader.SetText(Strings.Get("registration", "title"));

            //Register Username Background
            mUsernameBackground = new ImagePanel(mRegistrationPanel, "UsernamePanel");

            //Register Username Label
            mUsernameLabel = new Label(mUsernameBackground, "UsernameLabel");
            mUsernameLabel.SetText(Strings.Get("registration", "username"));

            //Register Username Textbox
            mUsernameTextbox = new TextBox(mUsernameBackground, "UsernameField");
            mUsernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;

            //Register Email Background
            mEmailBackground = new ImagePanel(mRegistrationPanel, "EmailPanel");

            //Register Email Label
            mEmailLabel = new Label(mEmailBackground, "EmailLabel");
            mEmailLabel.SetText(Strings.Get("registration", "email"));

            //Register Email Textbox
            mEmailTextbox = new TextBox(mEmailBackground, "EmailField");
            mEmailTextbox.SubmitPressed += EmailTextbox_SubmitPressed;

            //Register Password Background
            mPasswordBackground = new ImagePanel(mRegistrationPanel, "Password1Panel");

            //Register Password Label
            mPasswordLabel = new Label(mPasswordBackground, "Password1Label");
            mPasswordLabel.SetText(Strings.Get("registration", "password"));

            //Register Password Textbox
            mPasswordTextbox = new TextBoxPassword(mPasswordBackground, "Password1Field");
            mPasswordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;

            //Register Password Background
            mPasswordBackground2 = new ImagePanel(mRegistrationPanel, "Password2Panel");

            //Register Password Label2
            mPasswordLabel2 = new Label(mPasswordBackground2, "Password2Label");
            mPasswordLabel2.SetText(Strings.Get("registration", "confirmpass"));

            //Register Password Textbox2
            mPasswordTextbox2 = new TextBoxPassword(mPasswordBackground2, "Password2Field");
            mPasswordTextbox2.SubmitPressed += PasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            mRegisterBtn = new Button(mRegistrationPanel, "RegisterButton");
            mRegisterBtn.SetText(Strings.Get("registration", "register"));
            mRegisterBtn.Clicked += RegisterBtn_Clicked;

            //Register - Back Button
            mBackBtn = new Button(mRegistrationPanel, "BackButton");
            mBackBtn.SetText(Strings.Get("registration", "back"));
            mBackBtn.Clicked += BackBtn_Clicked;
        }

        //Methods
        public void Update()
        {
        }

        public void Show()
        {
            mRegistrationPanel.Show();
        }

        public void Hide()
        {
            mRegistrationPanel.Hide();
        }

        void TryRegister()
        {
            if (Globals.WaitingOnServer)
            {
                return;
            }
            if (GameNetwork.Connected)
            {
                if (FieldChecking.IsValidUsername(mUsernameTextbox.Text))
                {
                    if (mPasswordTextbox.Text == mPasswordTextbox2.Text)
                    {
                        if (FieldChecking.IsValidPassword(mPasswordTextbox.Text))
                        {
                            if (FieldChecking.IsWellformedEmailAddress(mEmailTextbox.Text))
                            {
                                GameFade.FadeOut();
                                Hide();
                                PacketSender.SendCreateAccount(mUsernameTextbox.Text, mPasswordTextbox.Text,
                                    mEmailTextbox.Text);
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
            mMainMenu.Show();
        }
    }
}