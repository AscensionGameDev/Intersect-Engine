using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Shared;
using Intersect.Framework.Core;
using Intersect.Network;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class MainMenu : MutableInterface
{
    private readonly Canvas _menuCanvas;
    private readonly MainMenuWindow _mainMenuWindow;
    private readonly LoginWindow _loginWindow;
    private readonly RegistrationWindow _registerWindow;
    private readonly ForgotPasswordWindow _forgotPasswordWindow;
    private readonly ResetPasswordWindow _resetPasswordWindow;
    private readonly CharacterCreationWindow _characterCreationWindow;
    private readonly SettingsWindow _settingsWindow;
    private readonly CreditsWindow _creditsWindow;

    public readonly SelectCharacterWindow SelectCharacterWindow;

    //Character creation feild check
    private bool mShouldOpenCharacterCreation;
    private bool mShouldOpenCharacterSelection;
    private bool _forceCharacterCreation;

    // Network status
    public static NetworkStatus ActiveNetworkStatus;
    public delegate void NetworkStatusHandler();
    public static NetworkStatusHandler? NetworkStatusChanged;
    internal static event EventHandler? ReceivedConfiguration;

    public static long LastNetworkStatusChangeTime { get; private set; }

    public MainMenu(Canvas menuCanvas) : base(menuCanvas)
    {
        _menuCanvas = menuCanvas;
        _mainMenuWindow = new MainMenuWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = true,
        };

        var logo = new ImagePanel(menuCanvas, "Logo");
        logo.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());

        NetworkStatusChanged += HandleNetworkStatusChanged;

        _loginWindow = new LoginWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

        _registerWindow = new RegistrationWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

        _settingsWindow = new SettingsWindow(_menuCanvas)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

        _creditsWindow = new CreditsWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

        _forgotPasswordWindow = new ForgotPasswordWindow(_menuCanvas, this)
        {
            // Alignment = [Alignments.CenterH],
            // Y = 480,
            // IsVisible = false,
        };

        _resetPasswordWindow = new ResetPasswordWindow(_menuCanvas, this)
        {
            // Alignment = [Alignments.CenterH],
            // Y = 480,
            // IsVisible = false,
        };

        SelectCharacterWindow = new SelectCharacterWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

        _characterCreationWindow = new CharacterCreationWindow(_menuCanvas, this, SelectCharacterWindow)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };
    }

    ~MainMenu()
    {
        // ReSharper disable once DelegateSubtraction
        NetworkStatusChanged -= HandleNetworkStatusChanged;
    }

    public static void HandleReceivedConfiguration()
    {
        ReceivedConfiguration?.Invoke(default, EventArgs.Empty);
    }

    //Methods
    public void Update(TimeSpan elapsed, TimeSpan total)
    {
        if (_mainMenuWindow.IsVisibleInTree)
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

        if (!_loginWindow.IsHidden)
        {
            _loginWindow.Update();
        }

        if (!_characterCreationWindow.IsHidden)
        {
            _characterCreationWindow.Update();
        }

        if (!_registerWindow.IsHidden)
        {
            _registerWindow.Update();
        }

        if (!SelectCharacterWindow.IsHidden)
        {
            SelectCharacterWindow.Update();
        }

        _settingsWindow.Update();
    }

    public void Reset()
    {
        _loginWindow.Hide();
        _registerWindow.Hide();
        _settingsWindow.Hide();
        _creditsWindow.Hide();
        _forgotPasswordWindow.Hide();
        _resetPasswordWindow.Hide();
        _characterCreationWindow.Hide();
        SelectCharacterWindow.Hide();
        _mainMenuWindow.Show();
        _mainMenuWindow.Reset();
    }

    public void Show() => _mainMenuWindow.Show();

    public void Hide() => _mainMenuWindow.Hide();

    public void NotifyOpenCharacterSelection(List<CharacterSelectionPreviewMetadata> characterSelectionPreviews)
    {
        mShouldOpenCharacterSelection = true;
        SelectCharacterWindow.CharacterSelectionPreviews = [..characterSelectionPreviews];
    }

    public void NotifyOpenForgotPassword()
    {
        Reset();
        Hide();
        _forgotPasswordWindow.Show();
    }

    public void NotifyOpenLogin()
    {
        Reset();
        Hide();
        _loginWindow.Show();
    }

    public void OpenResetPassword(string nameEmail)
    {
        Reset();
        Hide();
        _resetPasswordWindow.Target = nameEmail;
        _resetPasswordWindow.Show();
    }

    public void CreateCharacterSelection()
    {
        Hide();
        _loginWindow.Hide();
        _registerWindow.Hide();
        _settingsWindow.Hide();
        _characterCreationWindow.Hide();
        SelectCharacterWindow.Show();
        mShouldOpenCharacterSelection = false;
    }

    public void NotifyOpenCharacterCreation(bool force = false)
    {
        _forceCharacterCreation = force;
        mShouldOpenCharacterCreation = true;
    }

    public void CreateCharacterCreation()
    {
        Hide();
        _loginWindow.Hide();
        _registerWindow.Hide();
        _settingsWindow.Hide();
        SelectCharacterWindow.Hide();
        _characterCreationWindow.Show(force: _forceCharacterCreation);
        mShouldOpenCharacterCreation = false;
    }

    internal void SwitchToWindow<TMainMenuWindow>() where TMainMenuWindow : IMainMenuWindow
    {
        _mainMenuWindow.Hide();
        if (typeof(TMainMenuWindow) == typeof(LoginWindow))
        {
            _loginWindow.Show();
        }
        else if (typeof(TMainMenuWindow) == typeof(RegistrationWindow))
        {
            _registerWindow.Show();
        }
        else if (typeof(TMainMenuWindow) == typeof(CreditsWindow))
        {
            _creditsWindow.Show();
        }
    }

    internal void SettingsButton_Clicked()
    {
        Hide();
        _settingsWindow.Show(_mainMenuWindow);
    }

    private void HandleNetworkStatusChanged() => _mainMenuWindow.UpdateDisabled();

    public static void SetNetworkStatus(NetworkStatus networkStatus, bool resetStatusCheck = false)
    {
        if (ActiveNetworkStatus != networkStatus)
        {
            ActiveNetworkStatus = networkStatus;
            NetworkStatusChanged?.Invoke();
        }
        LastNetworkStatusChangeTime = resetStatusCheck ? -1 : Timing.Global.MillisecondsUtc;
    }
}
