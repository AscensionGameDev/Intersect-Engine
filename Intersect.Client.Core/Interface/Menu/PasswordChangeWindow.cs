using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Framework;
using Intersect.Security;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class PasswordChangeWindow : Window
{
    private readonly MainMenu _mainMenu;
    private readonly Window _previousWindow;
    private readonly IFont? _defaultFont;

    private readonly Panel _inputPanel;
    private readonly Panel _buttonPanel;

    private readonly Button _submitButton;
    private readonly Button _backButton;

    private readonly Panel _tokenPanel;
    private readonly Label _tokenLabel;
    private readonly TextBox _tokenInput;

    private readonly Panel _passwordPanel;
    private readonly Label _passwordLabel;
    private readonly TextBoxPassword _passwordInput;

    private readonly Panel _passwordConfirmationPanel;
    private readonly Label _passwordConfirmationLabel;
    private readonly TextBoxPassword _passwordConfirmationInput;

    private readonly PasswordChangeMode _changeMode;

    public PasswordChangeWindow(Canvas parent, MainMenu mainMenu, Window previousWindow, PasswordChangeMode passwordChangeMode) : base(
        parent,
        title: Strings.PasswordChange.Title,
        modal: false,
        name: $"{nameof(PasswordChangeWindow)}{passwordChangeMode}"
    )
    {
        _mainMenu = mainMenu;
        _previousWindow = previousWindow;
        _changeMode = passwordChangeMode;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack");

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 600, y: 148);
        IsClosable = false;
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        SkipRender();

        _buttonPanel = new Panel(this, nameof(_buttonPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Right,
            DockChildSpacing = new Padding(8),
            Margin = new Margin(8, 0, 0, 0),
            MinimumSize = new Point(120, 0),
        };

        _submitButton = new Button(_buttonPanel, nameof(_submitButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            TabOrder = 4,
            Text = Strings.ForgotPass.Submit,
        };
        _submitButton.Clicked += SubmitButtonOnClicked;

        _backButton = new Button(_buttonPanel, nameof(_backButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Padding = new Padding(8, 4),
            TabOrder = 5,
            Text = Strings.ForgotPass.Back,
        };
        _backButton.Clicked += BackButtonOnClicked;

        _inputPanel = new Panel(this, nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
        };

        _tokenPanel = new Panel(_inputPanel, nameof(_tokenPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };

        var tokenLabel = _changeMode switch
        {
            PasswordChangeMode.ResetToken => Strings.PasswordChange.InputLabelResetCode,
            PasswordChangeMode.ExistingPassword => Strings.PasswordChange.InputLabelPassword,
            _ => throw Exceptions.UnreachableInvalidEnum(_changeMode),
        };
        _tokenLabel = new Label(_tokenPanel, nameof(_passwordLabel))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            Padding = new Padding(0, 0, 10, 0),
            Text = tokenLabel,
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _tokenInput = CreateTokenInput(_defaultFont, _tokenPanel, _changeMode);
        _tokenInput.SubmitPressed += InputTextboxOnSubmitPressed;
        _tokenInput.Clicked += InputTextboxOnClicked;
        _tokenInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _tokenInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _tokenInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

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
            Text = Strings.PasswordChange.NewPassword,
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _passwordInput = new TextBoxPassword(_passwordPanel, nameof(_passwordInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TabOrder = 2,
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _passwordInput.SubmitPressed += InputTextboxOnSubmitPressed;
        _passwordInput.Clicked += InputTextboxOnClicked;
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
            FontSize = 12,
            Padding = new Padding(0, 0, 10, 0),
            Text = Strings.PasswordChange.ConfirmNewPassword,
            TextAlign = Pos.Right | Pos.CenterV,
        };

        _passwordConfirmationInput = new TextBoxPassword(_passwordConfirmationPanel, nameof(_passwordConfirmationInput))
        {
            Dock = Pos.Right,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            TabOrder = 3,
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _passwordConfirmationInput.SubmitPressed += InputTextboxOnSubmitPressed;
        _passwordConfirmationInput.Clicked += InputTextboxOnClicked;
        _passwordConfirmationInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _passwordConfirmationInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _passwordConfirmationInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");
    }

    private static TextBox CreateTokenInput(IFont? font, Panel tokenPanel, PasswordChangeMode changeMode)
    {
        return changeMode switch
        {
            PasswordChangeMode.ResetToken => new TextBox(tokenPanel, nameof(_passwordInput))
            {
                Dock = Pos.Right,
                Font = font,
                FontSize = 12,
                IsTabable = true,
                MinimumSize = new Point(240, 0),
                Padding = new Padding(4, 2),
                TabOrder = 1,
                TextAlign = Pos.Left | Pos.CenterV,
            },
            PasswordChangeMode.ExistingPassword => new TextBoxPassword(tokenPanel, nameof(_passwordInput))
            {
                Dock = Pos.Right,
                Font = font,
                FontSize = 12,
                MinimumSize = new Point(240, 0),
                Padding = new Padding(4, 2),
                TabOrder = 1,
                TextAlign = Pos.Left | Pos.CenterV,
            },
            _ => throw Exceptions.UnreachableInvalidEnum(changeMode),
        };
    }

    private void BackButtonOnClicked(Base sender, MouseButtonState arguments) => ReturnToPreviousWindow();

    private void SubmitButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        TrySendRequest();
    }

    private void InputTextboxOnSubmitPressed(Base sender, EventArgs arguments)
    {
        TrySendRequest();
    }

    //The username or email of the acc we are resetting the pass for
    public string? Target { set; get; }

    private void InputTextboxOnClicked(Base sender, MouseButtonState arguments)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var isPassword = textBox is TextBoxPassword;
        Globals.InputManager.OpenKeyboard(
            type: isPassword ? KeyboardType.Password : KeyboardType.Normal,
            text: textBox.Text ?? string.Empty,
            autoCorrection: false,
            multiLine: false,
            secure: isPassword
        );
    }

    private void ReturnToPreviousWindow()
    {
        _previousWindow.Show();
        DelayedDelete();
    }

    private void ReturnToMainMenu()
    {
        _mainMenu.Show();
        DelayedDelete();
    }

    public override void Show()
    {
        _tokenInput.Text = null;
        _passwordInput.Text = null;
        _passwordConfirmationInput.Text = null;
        base.Show();
    }

    private void TrySendRequest()
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

        var identifier = Target?.Trim();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            Interface.ShowAlert(Strings.Errors.InvalidStateReturnToMainMenu, alertType: AlertType.Error);
            ReturnToMainMenu();
            return;
        }

        var token = _tokenInput.Text?.Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            var message = _changeMode switch
            {
                PasswordChangeMode.ResetToken => Strings.PasswordChange.ErrorNoResetCode,
                PasswordChangeMode.ExistingPassword => Strings.PasswordChange.ErrorNoOldPassword,
                _ => throw Exceptions.UnreachableInvalidEnum(_changeMode),
            };
            Interface.ShowAlert(message, alertType: AlertType.Error);
            return;
        }

        var password = _passwordInput.Text?.Trim();
        if (string.IsNullOrWhiteSpace(password))
        {
            Interface.ShowAlert(Strings.PasswordChange.ErrorNoNewPassword, alertType: AlertType.Error);
            return;
        }

        var passwordConfirmation = _passwordConfirmationInput.Text?.Trim();
        if (string.IsNullOrWhiteSpace(passwordConfirmation))
        {
            Interface.ShowAlert(Strings.PasswordChange.ErrorNoNewPasswordConfirmation, alertType: AlertType.Error);
            return;
        }

        if (!FieldChecking.IsValidPassword(password, Strings.Regex.Password))
        {
            Interface.ShowAlert(Strings.Errors.PasswordInvalid, alertType: AlertType.Error);
            return;
        }

        if (!string.Equals(password, passwordConfirmation, StringComparison.Ordinal))
        {
            Interface.ShowAlert(Strings.Registration.PasswordMismatch, alertType: AlertType.Error);
            return;
        }

        if (_changeMode == PasswordChangeMode.ExistingPassword)
        {
            token = PasswordUtils.ComputePasswordHash(token);
        }

        var passwordHash = PasswordUtils.ComputePasswordHash(password);

        PacketSender.SendPasswordChangeRequest(identifier, token, passwordHash);

        Globals.WaitingOnServer = true;
        ChatboxMsg.ClearMessages();
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
    }
}
