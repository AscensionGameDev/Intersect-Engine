using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Shared;
using Intersect.Framework.Core;
using Intersect.Network;

namespace Intersect.Client.Interface.Menu;

public partial class MainMenu : MutableInterface
{
    private readonly Canvas _menuCanvas;
    private readonly MainMenuWindow _mainMenuWindow;

    private bool _shouldOpenCharacterCreation;
    private bool _shouldOpenCharacterSelection;
    private bool _forceCharacterCreation;

    // Network status
    public static NetworkStatus ActiveNetworkStatus { get; set; }

    public delegate void NetworkStatusHandler();

    public static event NetworkStatusHandler? NetworkStatusChanged;
    internal static event EventHandler? ReceivedConfiguration;

    public static long LastNetworkStatusChangeTime { get; private set; }

    private LoginWindow? _loginWindow;

    private LoginWindow LoginWindow => _loginWindow ??= new LoginWindow(_menuCanvas, this)
    {
        Alignment = [Alignments.CenterH],
        Y = 480,
        IsVisibleInTree = false,
    };

    private RegistrationWindow? _registrationWindow;

    private RegistrationWindow RegistrationWindow => _registrationWindow ??= new RegistrationWindow(_menuCanvas, this)
    {
        Alignment = [Alignments.CenterH],
        Y = 480,
        IsVisibleInTree = false,
    };

    private ForgotPasswordWindow? _forgotPasswordWindow;

    private ForgotPasswordWindow ForgotPasswordWindow => _forgotPasswordWindow ??= new ForgotPasswordWindow(_menuCanvas, this)
    {
        // Alignment = [Alignments.CenterH],
        // Y = 480,
        // IsVisible = false,
    };

    private ResetPasswordWindow? _resetPasswordWindow;

    private ResetPasswordWindow ResetPasswordWindow => _resetPasswordWindow ??= new ResetPasswordWindow(_menuCanvas, this)
    {
        // Alignment = [Alignments.CenterH],
        // Y = 480,
        // IsVisible = false,
    };

    private CharacterCreationWindow? _characterCreationWindow;

    private CharacterCreationWindow CharacterCreationWindow => _characterCreationWindow ??= new CharacterCreationWindow(_menuCanvas, this, SelectCharacterWindow)
    {
        Alignment = [Alignments.CenterH],
        Y = 480,
        IsVisibleInTree = false,
    };

    private CreditsWindow? _creditsWindow;

    private CreditsWindow CreditsWindow => _creditsWindow ??= new CreditsWindow(_menuCanvas, this)
    {
        Alignment = [Alignments.CenterH],
        Y = 480,
        IsVisibleInTree = false,
    };

    private SelectCharacterWindow? _selectCharacterWindow;

    public SelectCharacterWindow SelectCharacterWindow => _selectCharacterWindow ??=
        new SelectCharacterWindow(_menuCanvas, this)
        {
            Alignment = [Alignments.CenterH],
            Y = 480,
            IsVisibleInTree = false,
        };

    private SettingsWindow? _settingsWindow;

    private SettingsWindow SettingsWindow => _settingsWindow ??= new SettingsWindow(_menuCanvas)
    {
        Alignment = [Alignments.CenterH],
        Y = 480,
        IsVisibleInTree = false,
    };

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
        logo.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());

        NetworkStatusChanged += HandleNetworkStatusChanged;
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

        if (_shouldOpenCharacterSelection)
        {
            CreateCharacterSelection();
        }

        if (_shouldOpenCharacterCreation)
        {
            CreateCharacterCreation();
        }

        if (_loginWindow is { IsVisibleInTree: true } loginWindow)
        {
            loginWindow.Update();
        }

        if (_characterCreationWindow is { IsVisibleInTree: true } characterCreationWindow)
        {
            characterCreationWindow.Update();
        }

        if (_registrationWindow is { IsVisibleInTree: true } registrationWindow)
        {
            registrationWindow.Update();
        }

        if (_selectCharacterWindow is { IsVisibleInTree: true } selectCharacterWindow)
        {
            selectCharacterWindow.Update();
        }

        if (_settingsWindow is { IsVisibleInTree: true } settingsWindow)
        {
            settingsWindow.Update();
        }
    }

    public void Reset()
    {
        _settingsWindow = null;

        LoginWindow.Hide();
        RegistrationWindow.Hide();
        CreditsWindow.Hide();
        ForgotPasswordWindow.Hide();
        ResetPasswordWindow.Hide();
        CharacterCreationWindow.Hide();
        SelectCharacterWindow.Hide();
        _mainMenuWindow.Show();
        _mainMenuWindow.Reset();
    }

    public void Show() => _mainMenuWindow.Show();

    private void Hide() => _mainMenuWindow.Hide();

    public void NotifyOpenCharacterSelection(List<CharacterSelectionPreviewMetadata> characterSelectionPreviews)
    {
        _shouldOpenCharacterSelection = true;
        SelectCharacterWindow.CharacterSelectionPreviews = [..characterSelectionPreviews];
    }

    public void NotifyOpenForgotPassword()
    {
        Reset();
        Hide();
        ForgotPasswordWindow.Show();
    }

    public void NotifyOpenLogin()
    {
        Reset();
        Hide();
        LoginWindow.Show();
    }

    public void OpenResetPassword(string nameEmail)
    {
        Reset();
        Hide();
        ResetPasswordWindow.Target = nameEmail;
        ResetPasswordWindow.Show();
    }

    private void CreateCharacterSelection()
    {
        Hide();
        LoginWindow.Hide();
        RegistrationWindow.Hide();
        SettingsWindow.Hide();
        CharacterCreationWindow.Hide();
        SelectCharacterWindow.Show();
        _shouldOpenCharacterSelection = false;
    }

    public void NotifyOpenCharacterCreation(bool force = false)
    {
        _forceCharacterCreation = force;
        _shouldOpenCharacterCreation = true;
    }

    private void CreateCharacterCreation()
    {
        Hide();
        LoginWindow.Hide();
        RegistrationWindow.Hide();
        SettingsWindow.Hide();
        SelectCharacterWindow.Hide();
        CharacterCreationWindow.Show(force: _forceCharacterCreation);
        _shouldOpenCharacterCreation = false;
    }

    internal void SwitchToWindow<TMainMenuWindow>() where TMainMenuWindow : IMainMenuWindow
    {
        _mainMenuWindow.Hide();
        if (typeof(TMainMenuWindow) == typeof(LoginWindow))
        {
            LoginWindow.Show();
        }
        else if (typeof(TMainMenuWindow) == typeof(RegistrationWindow))
        {
            RegistrationWindow.Show();
        }
        else if (typeof(TMainMenuWindow) == typeof(CreditsWindow))
        {
            CreditsWindow.Show();
        }
    }

    internal void SettingsButton_Clicked()
    {
        Hide();
        SettingsWindow.Show(_mainMenuWindow);
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
