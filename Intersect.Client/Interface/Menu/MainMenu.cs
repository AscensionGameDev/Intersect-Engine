using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Network;

namespace Intersect.Client.Interface.Menu
{

    public partial class MainMenu : MutableInterface
    {

        public delegate void NetworkStatusHandler();

        public static NetworkStatus ActiveNetworkStatus;

        public static NetworkStatusHandler NetworkStatusChanged;

        private readonly CreateCharacterWindow mCreateCharacterWindow;

        private readonly Button mCreditsButton;

        private readonly CreditsWindow mCreditsWindow;

        private readonly Button mExitButton;

        private readonly ForgotPasswordWindow mForgotPasswordWindow;

        private readonly Button mLoginButton;

        private readonly LoginWindow mLoginWindow;

        //Controls
        private readonly Canvas mMenuCanvas;

        private readonly Label mMenuHeader;

        private readonly ImagePanel mMenuWindow;

        private readonly Button mSettingsButton;

        private readonly SettingsWindow mSettingsWindow;

        private readonly Button mRegisterButton;

        private readonly RegisterWindow mRegisterWindow;

        private readonly ResetPasswordWindow mResetPasswordWindow;

        private readonly SelectCharacterWindow mSelectCharacterWindow;

        private readonly Label mServerStatusLabel;

        //Character creation feild check
        private bool mHasMadeCharacterCreation;

        private bool mShouldOpenCharacterCreation;

        private bool mShouldOpenCharacterSelection;

        //Init
        public MainMenu(Canvas menuCanvas) : base(menuCanvas)
        {
            mMenuCanvas = menuCanvas;

            var logo = new ImagePanel(menuCanvas, "Logo");
            logo.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());

            //Main Menu Window
            mMenuWindow = new ImagePanel(menuCanvas, "MenuWindow");

            mServerStatusLabel = new Label(mMenuWindow, "ServerStatusLabel")
            {
                AutoSizeToContents = true,
                ShouldDrawBackground = true,
                Text = Strings.Server.StatusLabel.ToString(ActiveNetworkStatus.ToLocalizedString()),
            };

            mServerStatusLabel.SetTextColor(Color.White, Label.ControlState.Normal);
            mServerStatusLabel.AddAlignment(Alignments.Bottom);
            mServerStatusLabel.AddAlignment(Alignments.Left);
            mServerStatusLabel.ProcessAlignments();

            NetworkStatusChanged += HandleNetworkStatusChanged;

            //Menu Header
            mMenuHeader = new Label(mMenuWindow, "Title");
            mMenuHeader.SetText(Strings.MainMenu.Title);

            //Login Button
            mLoginButton = new Button(mMenuWindow, "LoginButton");
            mLoginButton.SetText(Strings.MainMenu.Login);
            mLoginButton.Clicked += LoginButton_Clicked;

            //Register Button
            mRegisterButton = new Button(mMenuWindow, "RegisterButton");
            mRegisterButton.SetText(Strings.MainMenu.Register);
            mRegisterButton.Clicked += RegisterButton_Clicked;

            //Credits Button
            mCreditsButton = new Button(mMenuWindow, "CreditsButton");
            mCreditsButton.SetText(Strings.MainMenu.Credits);
            mCreditsButton.Clicked += CreditsButton_Clicked;

            //Exit Button
            mExitButton = new Button(mMenuWindow, "ExitButton");
            mExitButton.SetText(Strings.MainMenu.Exit);
            mExitButton.Clicked += ExitButton_Clicked;

            //Settings Button
            mSettingsButton = new Button(mMenuWindow, "SettingsButton");
            mSettingsButton.Clicked += SettingsButton_Clicked;
            mSettingsButton.SetText(Strings.MainMenu.Settings);
            if (!string.IsNullOrEmpty(Strings.MainMenu.SettingsTooltip))
            {
                mSettingsButton.SetToolTipText(Strings.MainMenu.SettingsTooltip);
            }

            mMenuWindow.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());

            //Settings Controls
            mSettingsWindow = new SettingsWindow(menuCanvas, this, null);

            //Login Controls
            mLoginWindow = new LoginWindow(menuCanvas, this, mMenuWindow);

            //Register Controls
            mRegisterWindow = new RegisterWindow(menuCanvas, this, mMenuWindow);

            //Forgot Password Controls
            mForgotPasswordWindow = new ForgotPasswordWindow(menuCanvas, this, mMenuWindow);

            //Reset Password Controls
            mResetPasswordWindow = new ResetPasswordWindow(menuCanvas, this, mMenuWindow);

            //Character Selection Controls
            mSelectCharacterWindow = new SelectCharacterWindow(mMenuCanvas, this, mMenuWindow);

            //Character Creation Controls
            mCreateCharacterWindow = new CreateCharacterWindow(mMenuCanvas, this, mMenuWindow, mSelectCharacterWindow);

            //Credits Controls
            mCreditsWindow = new CreditsWindow(mMenuCanvas, this);

            UpdateDisabled();
        }

        ~MainMenu()
        {
            // ReSharper disable once DelegateSubtraction
            NetworkStatusChanged -= HandleNetworkStatusChanged;
        }

        //Methods
        public void Update()
        {
            if (mShouldOpenCharacterSelection)
            {
                CreateCharacterSelection();
            }

            if (mShouldOpenCharacterCreation)
            {
                CreateCharacterCreation();
            }

            if (!mLoginWindow.IsHidden)
            {
                mLoginWindow.Update();
            }

            if (!mCreateCharacterWindow.IsHidden)
            {
                mCreateCharacterWindow.Update();
            }

            if (!mRegisterWindow.IsHidden)
            {
                mRegisterWindow.Update();
            }

            if (!mSelectCharacterWindow.IsHidden)
            {
                mSelectCharacterWindow.Update();
            }

            mSettingsWindow.Update();
        }

        public void Reset()
        {
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mSettingsWindow.Hide();
            mCreditsWindow.Hide();
            mForgotPasswordWindow.Hide();
            mResetPasswordWindow.Hide();
            if (mCreateCharacterWindow != null)
            {
                mCreateCharacterWindow.Hide();
            }

            if (mSelectCharacterWindow != null)
            {
                mSelectCharacterWindow.Hide();
            }

            mMenuWindow.Show();
            mSettingsButton.Show();
        }

        public void Show()
        {
            mMenuWindow.IsHidden = false;
            mSettingsButton.IsHidden = false;
        }

        public void Hide()
        {
            mMenuWindow.IsHidden = true;
            mSettingsButton.IsHidden = true;
        }

        public void NotifyOpenCharacterSelection(List<Character> characters)
        {
            mShouldOpenCharacterSelection = true;
            mSelectCharacterWindow.Characters = characters;
        }

        public void NotifyOpenForgotPassword()
        {
            Reset();
            Hide();
            mForgotPasswordWindow.Show();
        }

        public void NotifyOpenLogin()
        {
            Reset();
            Hide();
            mLoginWindow.Show();
        }

        public void OpenResetPassword(string nameEmail)
        {
            Reset();
            Hide();
            mResetPasswordWindow.Target = nameEmail;
            mResetPasswordWindow.Show();
        }

        public void CreateCharacterSelection()
        {
            Hide();
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mSettingsWindow.Hide();
            mCreateCharacterWindow.Hide();
            mSelectCharacterWindow.Show();
            mShouldOpenCharacterSelection = false;
        }

        public void NotifyOpenCharacterCreation()
        {
            mShouldOpenCharacterCreation = true;
        }

        public void CreateCharacterCreation()
        {
            Hide();
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mSettingsWindow.Hide();
            mSelectCharacterWindow.Hide();
            mCreateCharacterWindow.Show();
            mCreateCharacterWindow.Init();
            mHasMadeCharacterCreation = true;
            mShouldOpenCharacterCreation = false;
        }

        //Input Handlers
        void LoginButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mLoginWindow.Show();
        }

        void RegisterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mRegisterWindow.Show();
        }

        void CreditsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mCreditsWindow.Show();
        }

        void SettingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mSettingsWindow.Show(true);
        }

        void ExitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }

        private void HandleNetworkStatusChanged()
        {
            mServerStatusLabel.Text = Strings.Server.StatusLabel.ToString(ActiveNetworkStatus.ToLocalizedString());
            UpdateDisabled();
        }

        private void UpdateDisabled()
        {
            mLoginButton.IsDisabled = ActiveNetworkStatus != NetworkStatus.Online;
            mRegisterButton.IsDisabled = ActiveNetworkStatus != NetworkStatus.Online ||
                                         Options.Loaded && Options.BlockClientRegistrations;
        }

        public static void OnNetworkConnecting()
        {
            ActiveNetworkStatus = NetworkStatus.Connecting;
        }

        public static void SetNetworkStatus(NetworkStatus networkStatus)
        {
            ActiveNetworkStatus = networkStatus;
            NetworkStatusChanged?.Invoke();
        }

    }

}
