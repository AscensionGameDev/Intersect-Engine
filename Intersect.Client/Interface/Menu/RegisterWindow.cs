using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class RegisterWindow : ImagePanel, IMainMenuWindow
{
    private readonly MainMenu _mainMenu;

    private readonly Label _registrationHeader;
    private readonly TextBox _txtUsername;
    private readonly TextBox _txtEmail;
    private readonly TextBoxPassword _txtPassword;
    private readonly TextBoxPassword _txtConfirmPassword;
    private readonly Button _btnRegister;

    //Init
    public RegisterWindow(Canvas parent, MainMenu mainMenu) : base(parent, "RegistrationWindow")
    {
        //Assign References
        _mainMenu = mainMenu;

        //Menu Header
        _registrationHeader = new Label(this, "RegistrationLabel");
        _registrationHeader.SetText(Strings.Registration.Title);

        //Register Username Label/Textbox
        var _usernameBackground = new ImagePanel(this, "UsernamePanel");
        _ = new Label(_usernameBackground, "UsernameLabel") { Text = Strings.Registration.Username };
        _txtUsername = new TextBox(_usernameBackground, "UsernameField")
        {
            IsTabable = true,
        };
        _txtUsername.SubmitPressed += (s, e) => TryRegister();

        //Register Email Label/Textbox
        var _emailBackground = new ImagePanel(this, "EmailPanel");
        _ = new Label(_emailBackground, "EmailLabel") { Text = Strings.Registration.Email };
        _txtEmail = new TextBox(_emailBackground, "EmailField")
        {
            IsTabable = true,
        };
        _txtEmail.SubmitPressed += (s, e) => TryRegister();

        //Register Password Label/Textbox
        var _passwordBackground = new ImagePanel(this, "Password1Panel");
        _ = new Label(_passwordBackground, "Password1Label") { Text = Strings.Registration.Password };
        _txtPassword = new TextBoxPassword(_passwordBackground, "Password1Field")
        {
            IsTabable = true,
        };
        _txtPassword.SubmitPressed += (s, e) => TryRegister();

        //Register Confirm Password Label/Textbox
        var _passwordBackground2 = new ImagePanel(this, "Password2Panel");
        _ = new Label(_passwordBackground2, "Password2Label") { Text = Strings.Registration.ConfirmPassword };
        _txtConfirmPassword = new TextBoxPassword(_passwordBackground2, "Password2Field")
        {
            IsTabable = true,
        };
        _txtConfirmPassword.SubmitPressed += (s, e) => TryRegister();

        //Register - Send Registration Button
        _btnRegister = new Button(this, "RegisterButton")
        {
            Text = Strings.Registration.Register,
        };
        _btnRegister.Clicked += (s, e) => TryRegister();

        //Register - Back Button
        var _btnBack = new Button(this, "BackButton")
        {
            // IsTabable = true,
            Text = Strings.Registration.Back,
        };
        _btnBack.Clicked += (s, e) =>
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
        if (!Globals.WaitingOnServer && _btnRegister.IsDisabled)
        {
            _btnRegister.Enable();
        }
    }

    public override void Show()
    {
        base.Show();
        _txtUsername.Focus();
    }

    void TryRegister()
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

        if (!FieldChecking.IsValidUsername(_txtUsername.Text, Strings.Regex.Username))
        {
            Interface.ShowError(Strings.Errors.UsernameInvalid);
            return;
        }

        if (!FieldChecking.IsWellformedEmailAddress(_txtEmail.Text, Strings.Regex.Email))
        {
            Interface.ShowError(Strings.Registration.EmailInvalid);
            return;
        }

        if (!FieldChecking.IsValidPassword(_txtPassword.Text, Strings.Regex.Password))
        {
            Interface.ShowError(Strings.Errors.PasswordInvalid);
            return;
        }

        if (_txtPassword.Text != _txtConfirmPassword.Text)
        {
            Interface.ShowError(Strings.Registration.PasswordMismatch);
            return;
        }

        PacketSender.SendCreateAccount(
            _txtUsername.Text,
            LoginWindow.ComputePasswordHash(_txtPassword.Text.Trim()),
            _txtEmail.Text
        );

        Globals.WaitingOnServer = true;
        _btnRegister.Disable();
        ChatboxMsg.ClearMessages();
        Hide();
    }
}
