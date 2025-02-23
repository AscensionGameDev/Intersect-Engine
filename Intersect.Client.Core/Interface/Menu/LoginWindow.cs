using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Security;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class LoginWindow : Window, IMainMenuWindow
{
    private readonly IFont? _defaultFont;

    private readonly MainMenu _mainMenu;

    private readonly Panel _inputPanel;
    private readonly Panel _inputOptionsPanel;
    private readonly Panel _buttonPanel;
    private readonly Panel _usernamePanel;
    private readonly Label _usernameLabel;
    private readonly TextBox _usernameInput;
    private readonly Panel _passwordPanel;
    private readonly TextBoxPassword _passwordInput;
    private readonly LabeledCheckBox _savePasswordCheckbox;
    private readonly Button _forgotPasswordButton;
    private readonly Button _loginButton;
    private bool _useSavedPass;
    private string _savedPass = string.Empty;
    private readonly Label _passwordLabel;
    private readonly Button _backButton;

    public LoginWindow(Canvas parent, MainMenu mainMenu) : base(
        parent,
        title: Strings.LoginWindow.Title,
        modal: false,
        name: nameof(LoginWindow)
    )
    {
        _mainMenu = mainMenu;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack");

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 504, y: 144);
        IsClosable = false;
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _buttonPanel = new Panel(this, nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Right,
            DockChildSpacing = new Padding(8),
            Margin = new Margin(8, 0, 0, 0),
            MinimumSize = new Point(120, 0),
        };

        _loginButton = new Button(_buttonPanel, "LoginButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.Login,
        };
        _loginButton.Clicked += LoginButtonOnClicked;

        _forgotPasswordButton = new Button(_buttonPanel, "ForgotPasswordButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Right | Pos.CenterV,
            Font = _defaultFont,
            FontSize = 12,
            IsHidden = true,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.ForgotPassword,
        };
        _forgotPasswordButton.Clicked += ForgotPasswordButtonOnClicked;

        _backButton = new Button(_buttonPanel, nameof(_backButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.Back,
        };
        _backButton.Clicked += BackButtonOnClicked;

        _inputPanel = new Panel(this, nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
        };

        _usernamePanel = new Panel(_inputPanel, nameof(_usernamePanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };
        _usernameLabel = new Label(_usernamePanel, nameof(_usernameLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.LoginWindow.Username,
            TextAlign = Pos.Right | Pos.CenterV,
        };
        _usernameInput = new TextBox(_usernamePanel, nameof(_usernameInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _usernameInput.SubmitPressed += (_, _) => TryLogin();
        _usernameInput.Clicked += UsernameInputClicked;
        _usernameInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _usernameInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _usernameInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        //Login Password Label/Textbox
        _passwordPanel = new Panel(_inputPanel, nameof(_passwordPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };
        _passwordLabel = new Label(_passwordPanel, nameof(_passwordLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.LoginWindow.Password,
            TextAlign = Pos.Right | Pos.CenterV,
        };
        _passwordInput = new TextBoxPassword(_passwordPanel, nameof(_passwordInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _passwordInput.SubmitPressed += (_, _) => TryLogin();
        _passwordInput.TextChanged += PasswordInputTextChanged;
        _passwordInput.Clicked += PasswordInputClicked;
        _passwordInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _passwordInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _passwordInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        _inputOptionsPanel = new Panel(_inputPanel, nameof(_inputOptionsPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(8),
            MinimumSize = new Point(0, 28),
        };

        _savePasswordCheckbox = new LabeledCheckBox(_inputOptionsPanel, nameof(_savePasswordCheckbox))
        {
            Dock = Pos.Right | Pos.CenterV,
            Font = _defaultFont,
            FontSize = 12,
            Text = Strings.LoginWindow.SavePassword,
        };
    }

    private void LoginButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        TryLogin();
    }

    protected override void EnsureInitialized()
    {
        LoadCredentials();

        _inputOptionsPanel.SizeToChildren(recursive: true);

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    #region Input Handling

    private void UsernameInputClicked(Base sender, MouseButtonState arguments)
    {
        Globals.InputManager.OpenKeyboard(
            KeyboardType.Normal,
            text => _usernameInput.Text = text ?? string.Empty,
            Strings.LoginWindow.Username,
            _usernameInput.Text,
            inputBounds: _usernameInput.GlobalBounds
        );
    }

    private void PasswordInputTextChanged(Base sender, EventArgs arguments)
    {
        _useSavedPass = false;
    }

    private void PasswordInputClicked(Base sender, MouseButtonState arguments)
    {
        Globals.InputManager.OpenKeyboard(
            KeyboardType.Password,
            text => _passwordInput.Text = text ?? string.Empty,
            Strings.LoginWindow.Password,
            _passwordInput.Text
        );
    }

    private static void ForgotPasswordButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        Interface.MenuUi.MainMenu.NotifyOpenForgotPassword();
    }

    private void BackButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        Hide();
        _mainMenu.Show();
        Networking.Network.DebounceClose("returning_to_main_menu");
    }

    #endregion

    public void Update()
    {
        if (!Networking.Network.IsConnected)
        {
            Hide();
            _mainMenu.Show();
            return;
        }

        // Re-Enable our buttons button if we're not waiting for the server anymore with it disabled.
        if (!Globals.WaitingOnServer && _loginButton.IsDisabled)
        {
            _loginButton.Enable();
        }
    }

    public override void Show()
    {
        base.Show();
        if (!_forgotPasswordButton.IsHidden)
        {
            _forgotPasswordButton.IsHidden = !Options.Instance.SmtpValid;
        }

        // Set focus to the appropriate elements.
        if (!string.IsNullOrWhiteSpace(_usernameInput.Text))
        {
            _passwordInput.Focus();
        }
        else
        {
            _usernameInput.Focus();
        }
    }

    private void TryLogin()
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        if (!Networking.Network.IsConnected)
        {
            Interface.ShowAlert(Strings.Errors.NotConnected, alertType: AlertType.Error);
            return;
        }

        if (!FieldChecking.IsValidUsername(_usernameInput.Text, Strings.Regex.Username))
        {
            Interface.ShowAlert(Strings.Errors.UsernameInvalid, alertType: AlertType.Error);
            return;
        }

        if (!FieldChecking.IsValidPassword(_passwordInput.Text, Strings.Regex.Password))
        {
            if (!_useSavedPass)
            {
                Interface.ShowAlert(Strings.Errors.PasswordInvalid, alertType: AlertType.Error);
                return;
            }
        }

        var password = _savedPass;
        if (!_useSavedPass)
        {
            password = PasswordUtils.ComputePasswordHash(_passwordInput.Text.Trim());
        }

        Globals.WaitingOnServer = true;
        _loginButton.Disable();

        PacketSender.SendLogin(_usernameInput.Text, password);
        SaveCredentials();
        ChatboxMsg.ClearMessages();
    }

    private const string DefaultPasswordInputMask = "****************";

    private void LoadCredentials()
    {
        var name = Globals.Database.LoadPreference("Username");
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        _usernameInput.Text = name;
        var pass = Globals.Database.LoadPreference("Password");
        if (string.IsNullOrEmpty(pass))
        {
            return;
        }

        _passwordInput.Text = DefaultPasswordInputMask;
        _passwordInput.CursorPos = DefaultPasswordInputMask.Length;
        _savedPass = pass;
        _useSavedPass = true;
        _savePasswordCheckbox.IsChecked = true;
    }

    private void SaveCredentials()
    {
        string? username = default, password = default;

        if (_savePasswordCheckbox.IsChecked)
        {
            username = _usernameInput.Text?.Trim();
            password = _useSavedPass ? _savedPass : PasswordUtils.ComputePasswordHash(_passwordInput.Text?.Trim());
        }

        Globals.Database.SavePreference("Username", username);
        Globals.Database.SavePreference("Password", password);
    }
}