using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Security;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class LoginWindow : Window, IMainMenuWindow
{
    private readonly GameFont? _defaultFont;

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
    private readonly Button _btnForgotPassword;
    private readonly Button _btnLogin;
    private bool _useSavedPass;
    private string _savedPass = string.Empty;
    private readonly Label _passwordLabel;

    public LoginWindow(Canvas parent, MainMenu mainMenu) : base(
        parent,
        title: Strings.LoginWindow.Title,
        modal: false,
        name: nameof(LoginWindow)
    )
    {
        //Assign References
        _mainMenu = mainMenu;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack", 12);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 504, y: 144);
        IsClosable = false;
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;

        _inputPanel = new Panel(this, nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
            MinimumSize = new Point(360, 0),
        };

        _buttonPanel = new Panel(this, nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Right,
            DockChildSpacing = new Padding(8),
            Margin = new Margin(8, 0, 0, 0),
            MinimumSize = new Point(120, 0),
        };

        //Login Username Label/Textbox
        _usernamePanel = new Panel(_inputPanel, nameof(_usernamePanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MaximumSize = new Point(360, 0),
            MinimumSize = new Point(360, 28),
        };
        _usernameLabel = new Label(_usernamePanel, nameof(_usernameLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.LoginWindow.Username,
            TextAlign = Pos.Right | Pos.CenterV,
        };
        _usernameInput = new TextBox(_usernamePanel, nameof(_usernameInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _usernameInput.SubmitPressed += (s, e) => TryLogin();
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
            MaximumSize = new Point(360, 0),
            MinimumSize = new Point(360, 28),
        };
        _passwordLabel = new Label(_passwordPanel, nameof(_passwordLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.LoginWindow.Password,
            TextAlign = Pos.Right | Pos.CenterV,
        };
        _passwordInput = new TextBoxPassword(_passwordPanel, nameof(_passwordInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _passwordInput.SubmitPressed += (s, e) => TryLogin();
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
            MinimumSize = new Point(360, 28),
        };

        //Login Save Pass Checkbox
        _savePasswordCheckbox = new LabeledCheckBox(_inputOptionsPanel, nameof(_savePasswordCheckbox))
        {
            Dock = Pos.Right | Pos.CenterV, Font = _defaultFont, Text = Strings.LoginWindow.SavePassword,
        };

        //Login - Send Login Button
        _btnLogin = new Button(_buttonPanel, "LoginButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.Login,
        };
        _btnLogin.Clicked += (s, e) => TryLogin();

        //Forgot Password Button
        _btnForgotPassword = new Button(_buttonPanel, "ForgotPasswordButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Right | Pos.CenterV,
            Font = _defaultFont,
            IsHidden = true,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.ForgotPassword,
        };
        _btnForgotPassword.Clicked += _btnForgotPassword_Clicked;

        //Login - Back Button
        var btnBack = new Button(_buttonPanel, "BackButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.Back,
        };
        btnBack.Clicked += _btnBack_Clicked;
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
            inputBounds: _usernameInput.BoundsGlobal
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

    private static void _btnForgotPassword_Clicked(Base sender, MouseButtonState arguments)
    {
        Interface.MenuUi.MainMenu.NotifyOpenForgotPassword();
    }

    private void _btnBack_Clicked(Base sender, MouseButtonState arguments)
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
        if (!Globals.WaitingOnServer && _btnLogin.IsDisabled)
        {
            _btnLogin.Enable();
        }
    }

    public override void Show()
    {
        base.Show();
        if (!_btnForgotPassword.IsHidden)
        {
            _btnForgotPassword.IsHidden = !Options.Instance.SmtpValid;
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
            Interface.ShowError(Strings.Errors.NotConnected);
            return;
        }

        if (!FieldChecking.IsValidUsername(_usernameInput.Text, Strings.Regex.Username))
        {
            Interface.ShowError(Strings.Errors.UsernameInvalid);
            return;
        }

        if (!FieldChecking.IsValidPassword(_passwordInput.Text, Strings.Regex.Password))
        {
            if (!_useSavedPass)
            {
                Interface.ShowError(Strings.Errors.PasswordInvalid);
                return;
            }
        }

        var password = _savedPass;
        if (!_useSavedPass)
        {
            password = PasswordUtils.ComputePasswordHash(_passwordInput.Text.Trim());
        }

        Globals.WaitingOnServer = true;
        _btnLogin.Disable();

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