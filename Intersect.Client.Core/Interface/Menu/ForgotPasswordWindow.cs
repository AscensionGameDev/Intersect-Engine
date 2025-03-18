using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;


public partial class ForgotPasswordWindow : Window
{
    private readonly IFont? _defaultFont;

    private readonly Panel _inputPanel;
    private readonly Panel _buttonPanel;

    private readonly Panel _usernameOrEmailPanel;
    private readonly TextBox _usernameOrEmailInput;

    private readonly Button _submitButton;
    private readonly Button _backButton;

    private readonly Label _disclaimerTemplate;
    private readonly RichLabel _disclaimer;
    private readonly ScrollControl _disclaimerScroller;

    public ForgotPasswordWindow(Canvas parent) : base(
        parent,
        title: Strings.ForgotPass.Title,
        modal: false,
        name: nameof(ForgotPasswordWindow)
    )
    {
        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack");

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 544, y: 168);
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
            MinimumSize = new Point(160, 0),
        };

        _submitButton = new Button(_buttonPanel, nameof(_submitButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(160, 24),
            Padding = new Padding(8, 4),
            Text = Strings.ForgotPass.Submit,
        };
        _submitButton.Clicked += SubmitButtonOnClicked;

        _backButton = new Button(_buttonPanel, nameof(_backButton))
        {
            AutoSizeToContents = true,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(160, 24),
            Padding = new Padding(8, 4),
            Text = Strings.ForgotPass.Back,
        };
        _backButton.Clicked += BackButtonOnClicked;

        _inputPanel = new Panel(this, nameof(_inputPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
        };

        _usernameOrEmailPanel = new Panel(_inputPanel, nameof(_usernameOrEmailPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Top,
            DockChildSpacing = new Padding(4),
            MinimumSize = new Point(0, 28),
        };

        _usernameOrEmailInput = new TextBox(_usernameOrEmailPanel, nameof(_usernameOrEmailInput))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            Padding = new Padding(4, 2),
            PlaceholderText = Strings.ForgotPass.UsernameOrEmailPlaceholder,
            TextAlign = Pos.Left | Pos.CenterV,
        };
        _usernameOrEmailInput.SubmitPressed += UsernameOrEmailInputOnSubmitPressed;
        _usernameOrEmailInput.Clicked += UsernameOrEmailInputOnClicked;
        _usernameOrEmailInput.SetSound(TextBox.Sounds.AddText, "octave-tap-resonant.wav");
        _usernameOrEmailInput.SetSound(TextBox.Sounds.RemoveText, "octave-tap-professional.wav");
        _usernameOrEmailInput.SetSound(TextBox.Sounds.Submit, "octave-tap-warm.wav");

        _disclaimerTemplate = new Label(_inputPanel, nameof(_disclaimerTemplate))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            IsVisibleInParent = false,
            WrappingBehavior = WrappingBehavior.Wrapped,
        };
        _disclaimerTemplate.SizeToContents();

        _disclaimerScroller = new ScrollControl(_inputPanel, nameof(_disclaimerScroller))
        {
            Dock = Pos.Fill,
            MinimumSize = _disclaimerTemplate.Size + new Point(16, 16),
            OverflowX = OverflowBehavior.Hidden,
            OverflowY = OverflowBehavior.Auto,
        };

        _disclaimer = new RichLabel(_disclaimerScroller, nameof(_disclaimer))
        {
            Dock = Pos.Fill,
            MinimumSize = _disclaimerTemplate.Size,
        };
    }

    private void UsernameOrEmailInputOnSubmitPressed(TextBox textBox, EventArgs eventArgs)
    {
        TrySendCode();
    }

    private void UsernameOrEmailInputOnClicked(Base sender, MouseButtonState arguments)
    {
        Globals.InputManager.OpenKeyboard(
            type: KeyboardType.Normal,
            text: _usernameOrEmailInput.Text ?? string.Empty,
            autoCorrection: false,
            multiLine: false,
            secure: false
        );
    }

    private void BackButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        Hide();
        Interface.MenuUi.MainMenu.NotifyOpenLogin();
    }

    private void SubmitButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        TrySendCode();
    }

    public override void Show()
    {
        _usernameOrEmailInput.Text = null;
        base.Show();
    }

    private void TrySendCode()
    {
        if (Globals.WaitingOnServer)
        {
            _submitButton.IsDisabled = true;
            return;
        }

        if (!Networking.Network.IsConnected)
        {
            Interface.ShowAlert(Strings.Errors.NotConnected, alertType: AlertType.Error);

            return;
        }

        var usernameOrEmail = _usernameOrEmailInput.Text;
        if (!FieldChecking.IsValidUsername(usernameOrEmail, Strings.Regex.Username) &&
            !FieldChecking.IsWellformedEmailAddress(usernameOrEmail, Strings.Regex.Email))
        {
            Interface.ShowAlert(Strings.Errors.UsernameInvalid, alertType: AlertType.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(usernameOrEmail))
        {
            throw new InvalidOperationException(
                "IsValidUsername() and IsWellformedEmailAddress() should have blocked this"
            );
        }

        Interface.MenuUi.MainMenu.OpenPasswordChangeWindow(usernameOrEmail, PasswordChangeMode.ResetToken, null);
        PacketSender.SendRequestPasswordReset(usernameOrEmail);
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
        _disclaimer.AddText(Strings.ForgotPass.Disclaimer, _disclaimerTemplate);
    }
}
