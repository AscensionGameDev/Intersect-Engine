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

public partial class LoginWindow : ImagePanel, IMainMenuWindow
{
    private readonly GameFont? _defaultFont;

    private readonly MainMenu _mainMenu;
    private readonly ImagePanel _usernamePanel;
    private readonly Label _usernameLabel;
    private readonly TextBox _usernameInput;
    private readonly ImagePanel _passwordPanel;
    private readonly TextBoxPassword _passwordInput;
    private readonly LabeledCheckBox _savePasswordCheckbox;
    private readonly Button _btnForgotPassword;
    private readonly Button _btnLogin;
    private bool _useSavedPass;
    private string _savedPass = string.Empty;
    private readonly Label _passwordLabel;

    //Init
    public LoginWindow(Canvas parent, MainMenu mainMenu) : base(parent, "LoginWindow")
    {
        //Assign References
        _mainMenu = mainMenu;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack", 10);

        //Menu Header
        _ = new Label(this, "LoginHeader")
        {
            Text = Strings.LoginWindow.Title,
        };

        //Login Username Label/Textbox
        _usernamePanel = new ImagePanel(this, nameof(_usernamePanel))
        {
            X = 14,
            Y = 61,
            Width = 308,
            Height = 22,
            Margin = new Margin(14, 0, 0, 0),
        };
        _usernameLabel = new Label(_usernamePanel, nameof(_usernameLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Left,
            Font = _defaultFont,
            Text = Strings.LoginWindow.Username,
            TextAlign = Pos.Right | Pos.CenterV,
            TextPadding = new Padding(0, 0, 10, 0),
        };
        _usernameInput = new TextBox(_usernamePanel, nameof(_usernameInput))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            TextAlign = Pos.Left | Pos.CenterV,
            TextPadding = new Padding(2, 0),
        };
        _usernameInput.SubmitPressed += (s, e) => TryLogin();
        _usernameInput.Clicked += UsernameInputClicked;
        _usernameInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _usernameInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _usernameInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        //Login Password Label/Textbox
        _passwordPanel = new ImagePanel(this, nameof(_passwordPanel))
        {
            X = 14,
            Y = 96,
            Width = 308,
            Height = 22,
            Margin = new Margin(14, 0, 0, 0),
        };
        _passwordLabel = new Label(_passwordPanel, nameof(_passwordLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Left,
            Font = _defaultFont,
            Text = Strings.LoginWindow.Password,
            TextAlign = Pos.Right | Pos.CenterV,
            TextPadding = new Padding(0, 0, 10, 0),
        };
        _passwordInput = new TextBoxPassword(_passwordPanel, nameof(_passwordInput))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            TextAlign = Pos.Left | Pos.CenterV,
            TextPadding = new Padding(2, 0),
        };
        _passwordInput.SubmitPressed += (s, e) => TryLogin();
        _passwordInput.TextChanged += PasswordInputTextChanged;
        _passwordInput.Clicked += PasswordInputClicked;
        _passwordInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _passwordInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _passwordInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        //Login Save Pass Checkbox
        _savePasswordCheckbox = new LabeledCheckBox(this, nameof(_savePasswordCheckbox))
        {
            X = 13,
            Y = 124,
            Width = 160,
            Height = 24,
            Font = _defaultFont,
            Text = Strings.LoginWindow.SavePassword,
        };

        //Forgot Password Button
        _btnForgotPassword = new Button(this, "ForgotPasswordButton")
        {
            IsHidden = true,
            Text = Strings.LoginWindow.ForgotPassword,
        };
        _btnForgotPassword.Clicked += _btnForgotPassword_Clicked;

        //Login - Send Login Button
        _btnLogin = new Button(this, "LoginButton")
        {
            Text = Strings.LoginWindow.Login,
        };
        _btnLogin.Clicked += (s, e) => TryLogin();

        //Login - Back Button
        var btnBack = new Button(this, "BackButton")
        {
            Text = Strings.LoginWindow.Back,
        };
        btnBack.Clicked += _btnBack_Clicked;

        LoadCredentials();
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