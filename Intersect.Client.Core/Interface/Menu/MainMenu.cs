using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Framework;
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

    private string? _username;

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

    private ForgotPasswordWindow ForgotPasswordWindow => _forgotPasswordWindow ??= new ForgotPasswordWindow(_menuCanvas)
    {
        // Alignment = [Alignments.CenterH],
        // Y = 480,
        // IsVisible = false,
    };

    private PasswordChangeWindow? _passwordChangeWindow;

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

        CreateGameTitle(menuCanvas);

        NetworkStatusChanged += HandleNetworkStatusChanged;
    }

    private static void CreateGameTitle(Canvas menuCanvas)
    {
        // Si un logo image existe (resources/gui/menu-logo.png), on l'affiche a la place
        // du titre texte genere par code - ca permet de remplacer le logo sans toucher au code,
        // juste en deposant le fichier au bon endroit.
        var logoTexture = GameContentManager.Current.GetTexture(
            Framework.Content.TextureType.Gui, "menu-logo.png"
        );

        if (logoTexture != default)
        {
            var aspectRatio = logoTexture.Width / (float)logoTexture.Height;
            const int maxWidth = 820;
            const int maxHeight = 340;
            var width = maxWidth;
            var height = (int)(width / aspectRatio);
            if (height > maxHeight)
            {
                height = maxHeight;
                width = (int)(height * aspectRatio);
            }

            _ = new ImagePanel(menuCanvas, nameof(CreateGameTitle))
            {
                Alignment = [Alignments.CenterH],
                Y = 20,
                Size = new Point(width, height),
                Texture = logoTexture,
                MouseInputEnabled = false,
            };

            return;
        }

        var titlePanel = new Panel(menuCanvas, nameof(CreateGameTitle))
        {
            Alignment = [Alignments.CenterH],
            Y = 60,
            Size = new Point(560, 96),
            MouseInputEnabled = false,
        };

        var titleFont = GameContentManager.Current.GetFont("sourcesansproblack");
        var taglineFont = GameContentManager.Current.GetFont("sourcesanspro");
        var spacedTitle = string.Join(" ", Strings.MainMenu.GameTitle.ToString().ToUpperInvariant());

        _ = new Label(titlePanel, "GameTitleShadow")
        {
            X = 2,
            Y = 10,
            Width = titlePanel.Width,
            Height = 40,
            Font = titleFont,
            FontSize = 26,
            TextAlign = Pos.Center,
            TextColor = new Color(150, 10, 8, 6),
            Text = spacedTitle,
            MouseInputEnabled = false,
        };

        _ = new Label(titlePanel, "GameTitleLabel")
        {
            X = 0,
            Y = 8,
            Width = titlePanel.Width,
            Height = 40,
            Font = titleFont,
            FontSize = 26,
            TextAlign = Pos.Center,
            TextColor = new Color(255, 235, 196, 110),
            Text = spacedTitle,
            MouseInputEnabled = false,
        };

        _ = new Panel(titlePanel, "GameTitleRule")
        {
            X = 90,
            Y = 56,
            Width = titlePanel.Width - 180,
            Height = 2,
            BackgroundColor = new Color(150, 235, 196, 110),
            MouseInputEnabled = false,
        };

        _ = new Label(titlePanel, "GameTagline")
        {
            X = 0,
            Y = 66,
            Width = titlePanel.Width,
            Height = 20,
            Font = taglineFont,
            FontSize = 10,
            TextAlign = Pos.Center,
            TextColor = new Color(220, 214, 206, 196),
            Text = Strings.MainMenu.GameTagline,
            MouseInputEnabled = false,
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

        _passwordChangeWindow?.DelayedDelete();
        _passwordChangeWindow = null;

        CharacterCreationWindow.Hide();
        SelectCharacterWindow.Hide();
        _mainMenuWindow.Show();
        _mainMenuWindow.Reset();
    }

    public void Show() => _mainMenuWindow.Show();

    private void Hide() => _mainMenuWindow.Hide();

    public void NotifyOpenCharacterSelection(
        List<CharacterSelectionPreviewMetadata> characterSelectionPreviews,
        string username
    )
    {
        _username = username;
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

    public void OpenPasswordChangeWindow(string? identifier, PasswordChangeMode changeMode, Window? previousWindow)
    {
        Reset();
        Hide();

        _passwordChangeWindow?.Dispose();

        identifier ??= _username;

        previousWindow ??= changeMode switch
        {
            PasswordChangeMode.ResetToken => LoginWindow,
            PasswordChangeMode.ExistingPassword => SelectCharacterWindow,
            _ => throw Exceptions.UnreachableInvalidEnum(changeMode),
        };

        _passwordChangeWindow = new PasswordChangeWindow(
            _menuCanvas,
            this,
            previousWindow,
            changeMode
        )
        {
            // Alignment = [Alignments.CenterH],
            // Y = 480,
            // IsVisible = false,
            Target = identifier,
        };
        _passwordChangeWindow.Disposed += PasswordChangeWindowOnDisposed;
    }

    private void PasswordChangeWindowOnDisposed(Base sender, EventArgs _)
    {
        if (sender == _passwordChangeWindow)
        {
            _passwordChangeWindow = null;
        }
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
