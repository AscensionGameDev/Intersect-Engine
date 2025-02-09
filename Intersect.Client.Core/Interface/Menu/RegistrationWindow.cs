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

public partial class RegistrationWindow : Window, IMainMenuWindow
{
    private readonly GameFont? _defaultFont;

    private readonly MainMenu _mainMenu;

    private readonly Panel _inputPanel;
    private readonly Panel _buttonPanel;

    private readonly Panel _usernamePanel;
    private readonly Label _usernameLabel;
    private readonly TextBox _usernameInput;

    private readonly Panel _emailPanel;
    private readonly Label _emailLabel;
    private readonly TextBox _emailInput;

    private readonly Panel _passwordPanel;
    private readonly Label _passwordLabel;
    private readonly TextBoxPassword _passwordInput;

    private readonly Panel _passwordConfirmationPanel;
    private readonly Label _passwordConfirmationLabel;
    private readonly TextBoxPassword _passwordConfirmationInput;

    private readonly Button _registerButton;
    private readonly Button _backButton;

    public RegistrationWindow(Canvas parent, MainMenu mainMenu) : base(
        parent,
        title: Strings.Registration.Title,
        modal: false,
        name: nameof(RegistrationWindow)
    )
    {
        _mainMenu = mainMenu;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack", 12);

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 560, y: 180);
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

        _registerButton = new Button(_buttonPanel, "RegisterButton")
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.Registration.Register,
        };
        _registerButton.Clicked += RegisterButtonOnClicked;

        _backButton = new Button(_buttonPanel, nameof(_backButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            Text = Strings.LoginWindow.Back,
        };
        _backButton.Clicked += BackButtonClicked;

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
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.Registration.Username,
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
        _usernameInput.SubmitPressed += OnInputSubmitPressed;
        _usernameInput.Clicked += UsernameInputClicked;
        _usernameInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _usernameInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _usernameInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        _emailPanel = new Panel(_inputPanel, nameof(_emailPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };

        _emailLabel = new Label(_emailPanel, nameof(_emailLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.Registration.Email,
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _emailInput = new TextBox(_emailPanel, nameof(_emailInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _emailInput.SubmitPressed += OnInputSubmitPressed;
        _emailInput.Clicked += EmailInputClicked;
        _emailInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _emailInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _emailInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

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
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.Registration.Password,
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
        _passwordInput.SubmitPressed += OnInputSubmitPressed;
        _passwordInput.Clicked += PasswordInputClicked;
        _passwordInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _passwordInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _passwordInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        _passwordConfirmationPanel = new Panel(_inputPanel, nameof(_passwordConfirmationPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };

        _passwordConfirmationLabel = new Label(_passwordConfirmationPanel, nameof(_passwordConfirmationLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.Registration.ConfirmPassword,
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _passwordConfirmationInput = new TextBoxPassword(_passwordConfirmationPanel, nameof(_passwordConfirmationInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _passwordConfirmationInput.SubmitPressed += OnInputSubmitPressed;
        _passwordConfirmationInput.Clicked += PasswordConfirmationInputClicked;
        _passwordConfirmationInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _passwordConfirmationInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _passwordConfirmationInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");
    }

    private void BackButtonClicked(Base sender, MouseButtonState arguments)
    {
        Hide();
        _mainMenu.Show();
        Networking.Network.DebounceClose("returning_to_main_menu");
    }

    private void RegisterButtonOnClicked(Base @base, MouseButtonState mouseButtonState)
    {
        TryRegister();
    }

    private void OnInputSubmitPressed(Base @base, EventArgs eventArgs)
    {
        TryRegister();
    }

    protected override void EnsureInitialized()
    {
        // LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    //Methods
    public void Update()
    {
        if (!Networking.Network.IsConnected)
        {
            Hide();
            _mainMenu.Show();
        }

        // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
        if (!Globals.WaitingOnServer && _registerButton.IsDisabled)
        {
            _registerButton.Enable();
        }
    }

    public override void Show()
    {
        base.Show();
        _usernameInput.Focus();
    }

    private static void OpenKeyboardForInput(TextBox textbox, KeyboardType keyboardType, string description)
    {
        Globals.InputManager.OpenKeyboard(
            keyboardType: keyboardType,
            inputHandler: text => textbox.Text = text ?? string.Empty,
            description: description,
            text: textbox.Text ?? string.Empty,
            inputBounds: textbox.BoundsGlobal
        );
    }

    private void UsernameInputClicked(Base sender, MouseButtonState arguments) => OpenKeyboardForInput(
        _usernameInput,
        KeyboardType.Normal,
        Strings.Registration.Username
    );

    private void EmailInputClicked(Base sender, MouseButtonState arguments) => OpenKeyboardForInput(
        _emailInput,
        KeyboardType.Email,
        Strings.Registration.Email
    );

    private void PasswordInputClicked(Base sender, MouseButtonState arguments) => OpenKeyboardForInput(
        _passwordInput,
        KeyboardType.Password,
        Strings.Registration.Password
    );

    private void PasswordConfirmationInputClicked(Base sender, MouseButtonState arguments) => OpenKeyboardForInput(
        _passwordConfirmationInput,
        KeyboardType.Password,
        Strings.Registration.ConfirmPassword
    );

    private void TryRegister()
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

        if (!FieldChecking.IsWellformedEmailAddress(_emailInput.Text, Strings.Regex.Email))
        {
            Interface.ShowError(Strings.Registration.EmailInvalid);
            return;
        }

        if (!FieldChecking.IsValidPassword(_passwordInput.Text, Strings.Regex.Password))
        {
            Interface.ShowError(Strings.Errors.PasswordInvalid);
            return;
        }

        if (_passwordInput.Text != _passwordConfirmationInput.Text)
        {
            Interface.ShowError(Strings.Registration.PasswordMismatch);
            return;
        }

        PacketSender.SendCreateAccount(
            _usernameInput.Text,
            PasswordUtils.ComputePasswordHash(_passwordInput.Text.Trim()),
            _emailInput.Text
        );

        Globals.WaitingOnServer = true;
        _registerButton.Disable();
        ChatboxMsg.ClearMessages();
        Hide();
    }
}