using System.Security.Cryptography;
using System.Text;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu
{

    public partial class LoginWindow : IMainMenuWindow
    {

        private Button mBackBtn;

        private Button mForgotPassswordButton;

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
        public LoginWindow(Canvas parent, MainMenu mainMenu)
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
            mPasswordTextbox.Clicked += MPasswordTextboxOnClicked;

            //Login Save Pass Checkbox
            mSavePassChk = new LabeledCheckBox(mLoginWindow, "SavePassCheckbox")
            {
                // IsTabable = true,
                Text = Strings.Login.savepass,
            };

            //Forgot Password Button
            mForgotPassswordButton = new Button(mLoginWindow, "ForgotPasswordButton")
            {
                IsHidden = true,
                // IsTabable = true,
                Text = Strings.Login.forgot,
            };
            mForgotPassswordButton.Clicked += mForgotPassswordButton_Clicked;

            //Login - Send Login Button
            mLoginBtn = new Button(mLoginWindow, "LoginButton")
            {
                // IsTabable = true,
                Text = Strings.Login.login,
            };
            mLoginBtn.Clicked += LoginBtn_Clicked;

            //Login - Back Button
            mBackBtn = new Button(mLoginWindow, "BackButton")
            {
                // IsTabable = true,
                Text = Strings.Login.back,
            };
            mBackBtn.Clicked += BackBtn_Clicked;

            LoadCredentials();

            mLoginWindow.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());

        }

        private void MPasswordTextboxOnClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(
                KeyboardType.Password,
                text => mPasswordTextbox.Text = text ?? string.Empty,
                "Password",
                mPasswordTextbox.Text
            );
        }

        public bool IsHidden => mLoginWindow.IsHidden;

        private void mForgotPassswordButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Interface.MenuUi.MainMenu.NotifyOpenForgotPassword();
        }

        private void _usernameTextbox_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(
                KeyboardType.Normal,
                text => mUsernameTextbox.Text = text ?? string.Empty,
                "Username",
                mUsernameTextbox.Text,
                inputBounds: mUsernameTextbox.BoundsGlobal
            );
        }

        //Methods
        public void Update()
        {
            if (!Networking.Network.IsConnected)
            {
                Hide();
                mMainMenu.Show();
                return;
            }

            // Re-Enable our buttons button if we're not waiting for the server anymore with it disabled.
            if (!Globals.WaitingOnServer && mLoginBtn.IsDisabled)
            {
                mLoginBtn.Enable();
            }
        }

        public void Hide()
        {
            mLoginWindow.IsHidden = true;
        }

        public void Show()
        {
            mLoginWindow.IsHidden = false;
            if (!mForgotPassswordButton.IsHidden)
            {
                mForgotPassswordButton.IsHidden = !Options.Instance.SmtpValid;
            }

            // Set focus to the appropriate elements.
            if (!string.IsNullOrWhiteSpace(mUsernameTextbox.Text))
            {
                mPasswordTextbox.Focus();
            }
            else
            {
                mUsernameTextbox.Focus();
            }
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

            Networking.Network.DebounceClose("returning_to_main_menu");
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

            if (!Networking.Network.IsConnected)
            {
                Interface.ShowError(Strings.Errors.notconnected);
                return;
            }

            if (!FieldChecking.IsValidUsername(mUsernameTextbox?.Text, Strings.Regex.username))
            {
                Interface.ShowError(Strings.Errors.usernameinvalid);
                return;
            }

            if (!FieldChecking.IsValidPassword(mPasswordTextbox?.Text, Strings.Regex.password))
            {
                if (!mUseSavedPass)
                {
                    Interface.ShowError(Strings.Errors.passwordinvalid);
                    return;
                }
            }

            var password = mSavedPass;
            if (!mUseSavedPass)
            {
                password = ComputePasswordHash(mPasswordTextbox?.Text?.Trim());
            }

            Globals.WaitingOnServer = true;
            mLoginBtn.Disable();
            PacketSender.SendLogin(mUsernameTextbox?.Text, password);
            SaveCredentials();
            ChatboxMsg.ClearMessages();
        }

        private void LoadCredentials()
        {
            var name = Globals.Database.LoadPreference("Username");
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            mUsernameTextbox.Text = name;
            var pass = Globals.Database.LoadPreference("Password");
            if (string.IsNullOrEmpty(pass))
            {
                return;
            }

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
