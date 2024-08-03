using System.Security.Cryptography;
using System.Text;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu;

public partial class LoginWindow : ImagePanel, IMainMenuWindow
{
    private readonly MainMenu _mainMenu;
    private readonly TextBox _txtUsername;
    private readonly TextBoxPassword _txtPassword;
    private readonly LabeledCheckBox _chkSavePass;
    private readonly Button _btnForgotPasssword;
    private readonly Button _btnLogin;
    private bool _useSavedPass;
    private string _savedPass = string.Empty;

    //Init
    public LoginWindow(Canvas parent, MainMenu mainMenu) : base(parent, "LoginWindow")
    {
        //Assign References
        _mainMenu = mainMenu;

        //Menu Header
        var _loginHeader = new Label(this, "LoginHeader") { Text = Strings.LoginWindow.Title };

        //Login Username Label/Textbox
        var _usernameBackground = new ImagePanel(this, "UsernamePanel");
        var _usernameLabel = new Label(_usernameBackground, "UsernameLabel") { Text = Strings.LoginWindow.Username };
        _txtUsername = new TextBox(_usernameBackground, "UsernameField");
        _txtUsername.SubmitPressed += (s, e) => TryLogin();
        _txtUsername.Clicked += _txtUsername_Clicked;

        //Login Password Label/Textbox
        var _passwordBackground = new ImagePanel(this, "PasswordPanel");
        var _passwordLabel = new Label(_passwordBackground, "PasswordLabel") { Text = Strings.LoginWindow.Password };
        _txtPassword = new TextBoxPassword(_passwordBackground, "PasswordField");
        _txtPassword.SubmitPressed += (s, e) => TryLogin();
        _txtPassword.TextChanged += _txtPassword_TextChanged;
        _txtPassword.Clicked += _txtPassword_Clicked;

        //Login Save Pass Checkbox
        _chkSavePass = new LabeledCheckBox(this, "SavePassCheckbox") { Text = Strings.LoginWindow.SavePassword };

        //Forgot Password Button
        _btnForgotPasssword = new Button(this, "ForgotPasswordButton")
        {
            IsHidden = true,
            Text = Strings.LoginWindow.ForgotPassword,
        };
        _btnForgotPasssword.Clicked += _btnForgotPasssword_Clicked;

        //Login - Send Login Button
        _btnLogin = new Button(this, "LoginButton") { Text = Strings.LoginWindow.Login };
        _btnLogin.Clicked += (s, e) => TryLogin();

        //Login - Back Button
        var _btnBack = new Button(this, "BackButton") { Text = Strings.LoginWindow.Back };
        _btnBack.Clicked += _btnBack_Clicked;

        LoadCredentials();
        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());
    }

    #region Input Handling
    private void _txtUsername_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Globals.InputManager.OpenKeyboard(
            KeyboardType.Normal,
            text => _txtUsername.Text = text ?? string.Empty,
            Strings.LoginWindow.Username,
            _txtUsername.Text,
            inputBounds: _txtUsername.BoundsGlobal
        );
    }

    private void _txtPassword_TextChanged(Base sender, EventArgs arguments)
    {
        _useSavedPass = false;
    }

    private void _txtPassword_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Globals.InputManager.OpenKeyboard(
            KeyboardType.Password,
            text => _txtPassword.Text = text ?? string.Empty,
            Strings.LoginWindow.Password,
            _txtPassword.Text
        );
    }

    private void _btnForgotPasssword_Clicked(Base sender, ClickedEventArgs arguments)
    {
        Interface.MenuUi.MainMenu.NotifyOpenForgotPassword();
    }

    void _btnBack_Clicked(Base sender, ClickedEventArgs arguments)
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
        if (!_btnForgotPasssword.IsHidden)
        {
            _btnForgotPasssword.IsHidden = !Options.Instance.SmtpValid;
        }

        // Set focus to the appropriate elements.
        if (!string.IsNullOrWhiteSpace(_txtUsername.Text))
        {
            _txtPassword.Focus();
        }
        else
        {
            _txtUsername.Focus();
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

        if (!FieldChecking.IsValidUsername(_txtUsername.Text, Strings.Regex.Username))
        {
            Interface.ShowError(Strings.Errors.UsernameInvalid);
            return;
        }

        if (!FieldChecking.IsValidPassword(_txtPassword.Text, Strings.Regex.Password))
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
            password = ComputePasswordHash(_txtPassword.Text.Trim());
        }

        Globals.WaitingOnServer = true;
        _btnLogin.Disable();

        PacketSender.SendLogin(_txtUsername.Text, password);
        SaveCredentials();
        ChatboxMsg.ClearMessages();
    }

    private void LoadCredentials()
    {
        var name = Globals.Database.LoadPreference("Username");
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        _txtUsername.Text = name;
        var pass = Globals.Database.LoadPreference("Password");
        if (string.IsNullOrEmpty(pass))
        {
            return;
        }

        _txtPassword.Text = "****************";
        _savedPass = pass;
        _useSavedPass = true;
        _chkSavePass.IsChecked = true;
    }

    private static string ComputePasswordHash(string password) =>
        BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(password ?? string.Empty))).Replace("-", string.Empty);

    private void SaveCredentials()
    {
        string username = string.Empty, password = string.Empty;

        if (_chkSavePass.IsChecked)
        {
            username = _txtUsername.Text.Trim();
            password = _useSavedPass ? _savedPass : ComputePasswordHash(_txtPassword.Text.Trim());
        }

        Globals.Database.SavePreference("Username", username);
        Globals.Database.SavePreference("Password", password);
    }
}
