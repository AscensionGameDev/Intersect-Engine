using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Intersect.Client.Classes.Localization;
using Intersect.Utilities;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class LoginWindow
    {
        private Button mBackBtn;
        private Button mLoginBtn;

        private Label mLoginHeader;

        //Controls
        private ImagePanel mLoginWindow;

        //Parent
        private MainMenu mMainMenu;

        private ImagePanel mPasswordBackground;
        private Label mPasswordLabel;
        private TextBoxPassword mPasswordTextbox;
        private string mSavedPass = "";
        private LabeledCheckBox mSavePassChk;

        //Controls
        private ImagePanel mUsernameBackground;

        private Label mUsernameLabel;
        private TextBox mUsernameTextbox;

        private bool mUseSavedPass;

        //Init
        public LoginWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mLoginWindow = new ImagePanel(parent, "LoginWindow");

            //Menu Header
            mLoginHeader = new Label(mLoginWindow, "LoginHeader");
            mLoginHeader.SetText(Strings.Login.title);

            mUsernameBackground = new ImagePanel(mLoginWindow, "UsernamePanel");

            //Login Username Label
            mUsernameLabel = new Label(mUsernameBackground, "UsernameLabel");
            mUsernameLabel.SetText(Strings.Login.username);

            //Login Username Textbox
            mUsernameTextbox = new TextBox(mUsernameBackground, "UsernameField");
            mUsernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;
            mUsernameTextbox.Clicked += _usernameTextbox_Clicked;

            mPasswordBackground = new ImagePanel(mLoginWindow, "PasswordPanel");

            //Login Password Label
            mPasswordLabel = new Label(mPasswordBackground, "PasswordLabel");
            mPasswordLabel.SetText(Strings.Login.password);

            //Login Password Textbox
            mPasswordTextbox = new TextBoxPassword(mPasswordBackground, "PasswordField");
            mPasswordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;
            mPasswordTextbox.TextChanged += _passwordTextbox_TextChanged;

            //Login Save Pass Checkbox
            mSavePassChk =
                new LabeledCheckBox(mLoginWindow, "SavePassCheckbox") {Text = Strings.Login.savepass};

            //Login - Send Login Button
            mLoginBtn = new Button(mLoginWindow, "LoginButton");
            mLoginBtn.SetText(Strings.Login.login);
            mLoginBtn.Clicked += LoginBtn_Clicked;

            //Login - Back Button
            mBackBtn = new Button(mLoginWindow, "BackButton");
            mBackBtn.SetText(Strings.Login.back);
            mBackBtn.Clicked += BackBtn_Clicked;

            LoadCredentials();

            mLoginWindow.LoadJsonUi(GameContentManager.UI.Menu);
        }

        private void _usernameTextbox_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(GameInput.KeyboardType.Normal, mUsernameTextbox.Text, false, false,
                false);
        }

        //Methods
        public void Update()
        {
        }

        public void Hide()
        {
            mLoginWindow.IsHidden = true;
        }

        public void Show()
        {
            mLoginWindow.IsHidden = false;
        }

        //Input Handlers
        void _passwordTextbox_TextChanged(Base sender, EventArgs arguments)
        {
            mUseSavedPass = false;
        }

        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mMainMenu.Show();
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
                if (FieldChecking.IsValidUsername(mUsernameTextbox.Text, Strings.Regex.username))
                {
                    if (mUseSavedPass)
                    {
                        GameFade.FadeOut();
                        PacketSender.SendLogin(mUsernameTextbox.Text, mSavedPass);
                        if (!mSavePassChk.IsChecked) SaveCredentials();
                        Globals.WaitingOnServer = true;
                    }
                    else
                    {
                        if (FieldChecking.IsValidPassword(mPasswordTextbox.Text, Strings.Regex.password))
                        {
                            GameFade.FadeOut();
                            PacketSender.SendLogin(mUsernameTextbox.Text,
                                BitConverter.ToString(
                                        sha.ComputeHash(Encoding.UTF8.GetBytes(mPasswordTextbox.Text.Trim())))
                                    .Replace("-", ""));
                            SaveCredentials();
                            Globals.WaitingOnServer = true;
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add(
                                new KeyValuePair<string, string>("", Strings.Errors.passwordinvalid));
                        }
                    }
                }
                else
                {
                    Gui.MsgboxErrors.Add(
                        new KeyValuePair<string, string>("", Strings.Errors.usernameinvalid));
                }
            }
            else
            {
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.notconnected));
            }
        }

        private void LoadCredentials()
        {
            string name = Globals.Database.LoadPreference("Username");
            if (!string.IsNullOrEmpty(name))
            {
                mUsernameTextbox.Text = name;
                string pass = Globals.Database.LoadPreference("Password");
                if (!string.IsNullOrEmpty(pass))
                {
                    mPasswordTextbox.Text = "*********";
                    mSavedPass = pass;
                    mUseSavedPass = true;
                    mSavePassChk.IsChecked = true;
                }
            }
        }

        private void SaveCredentials()
        {
            var sha = new SHA256Managed();
            if (mSavePassChk.IsChecked)
            {
                Globals.Database.SavePreference("Username", mUsernameTextbox.Text.Trim());
                Globals.Database.SavePreference("Password",
                    BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(mPasswordTextbox.Text.Trim())))
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