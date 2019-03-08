using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Client.UI.Game.Chat;
using Intersect.Utilities;

namespace Intersect.Client.UI.Menu
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
        private Button mForgotPassswordButton;

        //Controls
        private ImagePanel mUsernameBackground;

        private Label mUsernameLabel;
        private TextBox mUsernameTextbox;

        private bool mUseSavedPass;

        public bool IsHidden => mLoginWindow.IsHidden;

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

            //Forgot Password Button
            mForgotPassswordButton = new Button(mLoginWindow,"ForgotPasswordButton");
            mForgotPassswordButton.IsHidden = true;
            mForgotPassswordButton.SetText(Strings.Login.forgot);
            mForgotPassswordButton.Clicked += mForgotPassswordButton_Clicked;

            //Login - Send Login Button
            mLoginBtn = new Button(mLoginWindow, "LoginButton");
            mLoginBtn.SetText(Strings.Login.login);
            mLoginBtn.Clicked += LoginBtn_Clicked;

            //Login - Back Button
            mBackBtn = new Button(mLoginWindow, "BackButton");
            mBackBtn.SetText(Strings.Login.back);
            mBackBtn.Clicked += BackBtn_Clicked;

            LoadCredentials();

            mLoginWindow.LoadJsonUi(GameContentManager.UI.Menu, GameGraphics.Renderer.GetResolutionString());

            //Hide Forgot Password Button if not supported by server

        }

        private void mForgotPassswordButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Gui.MenuUi.MainMenu.NotifyOpenForgotPassword();
        }

        private void _usernameTextbox_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(GameInput.KeyboardType.Normal, mUsernameTextbox.Text, false, false,
                false);
        }

        //Methods
        public void Update()
        {
            if (GameNetwork.Connected)
            {
                return;
            }

            Hide();
            mMainMenu.Show();
            Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.lostconnection));
        }

        public void Hide()
        {
            mLoginWindow.IsHidden = true;
        }

        public void Show()
        {
            mLoginWindow.IsHidden = false;
            if (!mForgotPassswordButton.IsHidden) mForgotPassswordButton.IsHidden = !Options.SmtpValid;
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
            if (Globals.WaitingOnServer)
            {
                return;
            }

            if (!GameNetwork.Connected)
            {
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.notconnected));
                return;
            }

            if (!FieldChecking.IsValidUsername(mUsernameTextbox?.Text, Strings.Regex.username))
            {
                Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.usernameinvalid));
                return;
            }

            if (!FieldChecking.IsValidPassword(mPasswordTextbox?.Text, Strings.Regex.password))
            {
                if (!mUseSavedPass)
                {
                    Gui.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.passwordinvalid));
                    return;
                }
            }

            var password = mSavedPass;
            if (!mUseSavedPass)
            {
                using (var sha = new SHA256Managed())
                {
                    password = ComputePasswordHash(mPasswordTextbox?.Text?.Trim());
                }
            }

            GameFade.FadeOut();
            PacketSender.SendLogin(mUsernameTextbox?.Text, password);
            SaveCredentials();
            Globals.WaitingOnServer = true;
            ChatboxMsg.ClearMessages();
        }

        private void LoadCredentials()
        {
            var name = Globals.Database.LoadPreference("Username");
            if (string.IsNullOrEmpty(name)) return;
            mUsernameTextbox.Text = name;
            var pass = Globals.Database.LoadPreference("Password");
            if (string.IsNullOrEmpty(pass)) return;
            mPasswordTextbox.Text = "****************";
            mSavedPass = pass;
            mUseSavedPass = true;
            mSavePassChk.IsChecked = true;
        }

        private static string ComputePasswordHash(string password)
        {
            using (var sha = new SHA256Managed())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""))).Replace("-", "");
            }
        }

        private void SaveCredentials()
        {
            var username = "";
            var password = "";

            if (mSavePassChk.IsChecked)
            {
                username = mUsernameTextbox?.Text?.Trim();
                password = mUseSavedPass ? mSavedPass : ComputePasswordHash(mPasswordTextbox?.Text?.Trim());
            }

            Globals.Database.SavePreference("Username", username);
            Globals.Database.SavePreference("Password", password);
        }
    }
}