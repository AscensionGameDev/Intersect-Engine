using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Network;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu
{

    public partial class MainMenu : MutableInterface
    {
        public static void HandleReceivedConfiguration()
        {
            ReceivedConfiguration?.Invoke(default, EventArgs.Empty);
        }

        internal static event EventHandler? ReceivedConfiguration;

        public delegate void NetworkStatusHandler();

        public static NetworkStatus ActiveNetworkStatus;

        public static NetworkStatusHandler NetworkStatusChanged;

        private readonly CreateCharacterWindow mCreateCharacterWindow;

        private readonly CreditsWindow mCreditsWindow;

        private readonly ForgotPasswordWindow mForgotPasswordWindow;

        private readonly LoginWindow mLoginWindow;

        //Controls
        private readonly Canvas mMenuCanvas;

        private readonly Label mMenuHeader;

        private readonly SettingsWindow mSettingsWindow;

        private readonly RegisterWindow mRegisterWindow;

        private readonly ResetPasswordWindow mResetPasswordWindow;

        private readonly SelectCharacterWindow mSelectCharacterWindow;

        //Character creation feild check
        private bool mHasMadeCharacterCreation;

        private bool mShouldOpenCharacterCreation;

        private bool mShouldOpenCharacterSelection;

        private readonly MainMenuWindow _mainMenuWindow;

        //Init
        public MainMenu(Canvas menuCanvas) : base(menuCanvas)
        {
            mMenuCanvas = menuCanvas;

            _mainMenuWindow = new MainMenuWindow(mMenuCanvas, this);

            var logo = new ImagePanel(menuCanvas, "Logo");
            logo.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());

            NetworkStatusChanged += HandleNetworkStatusChanged;

            //Settings Controls
            mSettingsWindow = new SettingsWindow(menuCanvas, this, null);

            //Login Controls
            mLoginWindow = new LoginWindow(menuCanvas, this);

            //Register Controls
            mRegisterWindow = new RegisterWindow(menuCanvas, this);

            //Forgot Password Controls
            mForgotPasswordWindow = new ForgotPasswordWindow(menuCanvas, this);

            //Reset Password Controls
            mResetPasswordWindow = new ResetPasswordWindow(menuCanvas, this);

            //Character Selection Controls
            mSelectCharacterWindow = new SelectCharacterWindow(mMenuCanvas, this);

            //Character Creation Controls
            mCreateCharacterWindow = new CreateCharacterWindow(mMenuCanvas, this, mSelectCharacterWindow);

            //Credits Controls
            mCreditsWindow = new CreditsWindow(mMenuCanvas, this);
        }

        ~MainMenu()
        {
            // ReSharper disable once DelegateSubtraction
            NetworkStatusChanged -= HandleNetworkStatusChanged;
        }

        //Methods
        public void Update()
        {
            if (_mainMenuWindow.IsVisible)
            {
                _mainMenuWindow.Update();
            }

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

            _mainMenuWindow.Show();
            _mainMenuWindow.Reset();
        }

        public void Show() => _mainMenuWindow.Show();

        public void Hide() => _mainMenuWindow.Hide();

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

        internal void SwitchToWindow<TMainMenuWindow>() where TMainMenuWindow : IMainMenuWindow
        {
            _mainMenuWindow.Hide();
            if (typeof(TMainMenuWindow) == typeof(LoginWindow))
            {
                mLoginWindow.Show();
            }
            else if (typeof(TMainMenuWindow) == typeof(RegisterWindow))
            {
                mRegisterWindow.Show();
            }
            else if (typeof(TMainMenuWindow) == typeof(CreditsWindow))
            {
                mCreditsWindow.Show();
            }
        }

        internal void SettingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mSettingsWindow.Show(true);
        }

        private void HandleNetworkStatusChanged()
        {
            _mainMenuWindow.UpdateDisabled();
        }

        public static void SetNetworkStatus(NetworkStatus networkStatus, bool resetStatusCheck = false)
        {
            ActiveNetworkStatus = networkStatus;
            NetworkStatusChanged?.Invoke();
            LastNetworkStatusChangeTime = resetStatusCheck ? -1 : Timing.Global.MillisecondsUtc;
        }

        public static long LastNetworkStatusChangeTime { get; private set; }
    }
}
