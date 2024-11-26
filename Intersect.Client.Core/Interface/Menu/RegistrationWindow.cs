using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Core.Localization;
using Intersect.Client.Networking;
using Intersect.Security;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class RegistrationWindow : ImagePanel, IMainMenuWindow
{
    private readonly MainMenu _mainMenu;

    private readonly Label _registrationHeader;
    private readonly TextBox _textBoxUsername;
    private readonly TextBox _textBoxEmail;
    private readonly TextBoxPassword _textBoxPassword;
    private readonly TextBoxPassword _textBoxConfirmPassword;
    private readonly Button _buttonRegister;

    //Init
    public RegistrationWindow(Canvas parent, MainMenu mainMenu) : base(parent, nameof(RegistrationWindow))
    {
        //Assign References
        _mainMenu = mainMenu;

        //Menu Header
        _registrationHeader = new Label(this, "RegistrationLabel");
        _registrationHeader.SetText(Strings.Registration.Title);

        //Register Username Label/Textbox
        var usernameContainer = new ImagePanel(this, "UsernamePanel");
        _ = new Label(usernameContainer, "UsernameLabel")
        {
            Text = Strings.Registration.Username,
        };
        _textBoxUsername = new TextBox(usernameContainer, "UsernameField")
        {
            IsTabable = true,
        };
        _textBoxUsername.SubmitPressed += (s, e) => TryRegister();

        //Register Email Label/Textbox
        var emailContainer = new ImagePanel(this, "EmailPanel");
        _ = new Label(emailContainer, "EmailLabel")
        {
            Text = Strings.Registration.Email,
        };
        _textBoxEmail = new TextBox(emailContainer, "EmailField")
        {
            IsTabable = true,
        };
        _textBoxEmail.SubmitPressed += (s, e) => TryRegister();

        //Register Password Label/Textbox
        var passwordContainer = new ImagePanel(this, "Password1Panel");
        _ = new Label(passwordContainer, "Password1Label")
        {
            Text = Strings.Registration.Password,
        };
        _textBoxPassword = new TextBoxPassword(passwordContainer, "Password1Field")
        {
            IsTabable = true,
        };
        _textBoxPassword.SubmitPressed += (s, e) => TryRegister();

        //Register Confirm Password Label/Textbox
        var confirmPasswordContainer = new ImagePanel(this, "Password2Panel");
        _ = new Label(confirmPasswordContainer, "Password2Label")
        {
            Text = Strings.Registration.ConfirmPassword,
        };
        _textBoxConfirmPassword = new TextBoxPassword(confirmPasswordContainer, "Password2Field")
        {
            IsTabable = true,
        };
        _textBoxConfirmPassword.SubmitPressed += (s, e) => TryRegister();

        //Register - Send Registration Button
        _buttonRegister = new Button(this, "RegisterButton")
        {
            Text = Strings.Registration.Register,
        };
        _buttonRegister.Clicked += (s, e) => TryRegister();

        //Register - Back Button
        var buttonBack = new Button(this, "BackButton")
        {
            Text = Strings.Registration.Back,
        };
        buttonBack.Clicked += (s, e) =>
        {
            Hide();
            _mainMenu.Show();
            Networking.Network.DebounceClose("returning_to_main_menu");
        };

        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
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
        if (!Globals.WaitingOnServer && _buttonRegister.IsDisabled)
        {
            _buttonRegister.Enable();
        }
    }

    public override void Show()
    {
        base.Show();
        _textBoxUsername.Focus();
    }

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

        if (!FieldChecking.IsValidUsername(_textBoxUsername.Text, Strings.Regex.Username))
        {
            Interface.ShowError(Strings.Errors.UsernameInvalid);
            return;
        }

        if (!FieldChecking.IsWellformedEmailAddress(_textBoxEmail.Text, Strings.Regex.Email))
        {
            Interface.ShowError(Strings.Registration.EmailInvalid);
            return;
        }

        if (!FieldChecking.IsValidPassword(_textBoxPassword.Text, Strings.Regex.Password))
        {
            Interface.ShowError(Strings.Errors.PasswordInvalid);
            return;
        }

        if (_textBoxPassword.Text != _textBoxConfirmPassword.Text)
        {
            Interface.ShowError(Strings.Registration.PasswordMismatch);
            return;
        }

        PacketSender.SendCreateAccount(
            _textBoxUsername.Text,
            PasswordUtils.ComputePasswordHash(_textBoxPassword.Text.Trim()),
            _textBoxEmail.Text
        );

        Globals.WaitingOnServer = true;
        _buttonRegister.Disable();
        ChatboxMsg.ClearMessages();
        Hide();
    }
}