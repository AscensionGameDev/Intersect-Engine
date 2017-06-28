using System;
using System.Security.Cryptography;
using System.Text;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class LoginWindow
    {
        private Button _backBtn;
        private Button _loginBtn;

        //Parent
        private MainMenu _mainMenu;
        private Label _menuHeader;
        //Controls
        private ImagePanel _menuPanel;

        private ImagePanel _passwordBackground;
        private Label _passwordLabel;
        private TextBoxPassword _passwordTextbox;
        private string _savedPass = "";
        private LabeledCheckBox _savePassChk;

        //Controls
        private ImagePanel _usernameBackground;
        private Label _usernameLabel;
        private TextBox _usernameTextbox;

        private bool _useSavedPass;

        //Init
        public LoginWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _menuPanel = new ImagePanel(parent)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png")
            };
            _menuPanel.SetSize(512, 393);
            _menuPanel.SetPosition(parentPanel.X, parentPanel.Y);
            _menuPanel.IsHidden = true;

            //Menu Header
            _menuHeader = new Label(_menuPanel)
            {
                AutoSizeToContents = false
            };
            _menuHeader.SetText(Strings.Get("login", "title"));
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 24);
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

            _usernameBackground = new ImagePanel(_menuPanel)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png")
            };
            _usernameBackground.SetSize(_usernameBackground.Texture.GetWidth(), _usernameBackground.Texture.GetHeight());
            _usernameBackground.SetPosition(_menuPanel.Width / 2 - _usernameBackground.Width / 2, 44);

            //Login Username Label
            _usernameLabel = new Label(_usernameBackground);
            _usernameLabel.SetText(Strings.Get("login", "username"));
            _usernameLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _usernameLabel.AutoSizeToContents = false;
            _usernameLabel.SetSize(176, 55);
            _usernameLabel.Alignment = Pos.Center;
            _usernameLabel.TextColorOverride = new Color(255, 30, 30, 30);

            //Login Username Textbox
            _usernameTextbox = new TextBox(_usernameBackground);
            _usernameTextbox.SetPosition(190, 8);
            _usernameTextbox.SetSize(248, 38);
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;
            _usernameTextbox.ShouldDrawBackground = false;
            _usernameTextbox.TextColorOverride = new Color(255, 220, 220, 220);
            _usernameTextbox.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _usernameTextbox.Clicked += _usernameTextbox_Clicked;

            _passwordBackground = new ImagePanel(_menuPanel)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png")
            };
            _passwordBackground.SetSize(_passwordBackground.Texture.GetWidth(), _passwordBackground.Texture.GetHeight());
            _passwordBackground.SetPosition(_menuPanel.Width / 2 - _passwordBackground.Width / 2,
                _usernameBackground.Bottom + 16);

            //Login Password Label
            _passwordLabel = new Label(_passwordBackground);
            _passwordLabel.SetText(Strings.Get("login", "password"));
            _passwordLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _passwordLabel.AutoSizeToContents = false;
            _passwordLabel.SetSize(176, 55);
            _passwordLabel.Alignment = Pos.Center;
            _passwordLabel.TextColorOverride = new Color(255, 30, 30, 30);

            //Login Password Textbox
            _passwordTextbox = new TextBoxPassword(_passwordBackground)
            {
                Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20)
            };
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;
            _passwordTextbox.TextChanged += _passwordTextbox_TextChanged;
            _passwordTextbox.SetPosition(190, 8);
            _passwordTextbox.SetSize(248, 38);
            _passwordTextbox.ShouldDrawBackground = false;
            _passwordTextbox.TextColorOverride = new Color(255, 220, 220, 220);

            //Login Save Pass Checkbox
            _savePassChk = new LabeledCheckBox(_menuPanel) {Text = Strings.Get("login", "savepass")};
            _savePassChk.SetFont(Globals.ContentManager.GetFont(Gui.ActiveFont, 20));
            _savePassChk.SetSize(300, 36);
            _savePassChk.SetPosition(_passwordBackground.X + 24, _passwordBackground.Bottom + 16);
            _savePassChk.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxempty.png"),
                CheckBox.ControlState.Normal);
            _savePassChk.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxfull.png"),
                CheckBox.ControlState.CheckedNormal);
            _savePassChk.SetCheckSize(32, 32);
            _savePassChk.SetLabelDistance(12);
            _savePassChk.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _savePassChk.SetTextColor(new Color(255, 140, 140, 140), Label.ControlState.Hovered);

            //Login - Send Login Button
            _loginBtn = new Button(_menuPanel);
            _loginBtn.SetText(Strings.Get("login", "login"));
            _loginBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _loginBtn.Clicked += LoginBtn_Clicked;
            _loginBtn.SetPosition(_usernameBackground.X, _savePassChk.Bottom + 16);
            _loginBtn.SetSize(211, 61);
            _loginBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _loginBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _loginBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _loginBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _loginBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _loginBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            //Login - Back Button
            _backBtn = new Button(_menuPanel);
            _backBtn.SetText(Strings.Get("login", "back"));
            _backBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _backBtn.SetSize(211, 61);
            _backBtn.SetPosition(_usernameBackground.Right - _backBtn.Width, _savePassChk.Bottom + 16);
            _backBtn.Clicked += BackBtn_Clicked;
            _backBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _backBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _backBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _backBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _backBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            LoadCredentials();
        }

        private void _usernameTextbox_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.InputManager.OpenKeyboard(GameInput.KeyboardType.Normal, _usernameTextbox.Text, false, false, false);
        }

        //Methods
        public void Update()
        {
        }

        public void Hide()
        {
            _menuPanel.IsHidden = true;
        }

        public void Show()
        {
            _menuPanel.IsHidden = false;
        }

        //Input Handlers
        void _passwordTextbox_TextChanged(Base sender, EventArgs arguments)
        {
            _useSavedPass = false;
        }

        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _mainMenu.Show();
        }

        void UsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }

        void PasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }

        void LoginBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryLogin();
        }

        public void TryLogin()
        {
            var sha = new SHA256Managed();
            if (Globals.WaitingOnServer)
            {
                return;
            }
            if (GameNetwork.Connected)
            {
                if (FieldChecking.IsValidName(_usernameTextbox.Text))
                {
                    if (_useSavedPass)
                    {
                        GameFade.FadeOut();
                        PacketSender.SendLogin(_usernameTextbox.Text, _savedPass);
                        if (!_savePassChk.IsChecked) SaveCredentials();
                        Globals.WaitingOnServer = true;
                    }
                    else
                    {
                        if (FieldChecking.IsValidPass(_passwordTextbox.Text))
                        {
                            GameFade.FadeOut();
                            PacketSender.SendLogin(_usernameTextbox.Text,
                                BitConverter.ToString(
                                        sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim())))
                                    .Replace("-", ""));
                            SaveCredentials();
                            Globals.WaitingOnServer = true;
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add(Strings.Get("errors", "passwordinvalid"));
                        }
                    }
                }
                else
                {
                    Gui.MsgboxErrors.Add(Strings.Get("errors", "usernameinvalid"));
                }
            }
            else
            {
                Gui.MsgboxErrors.Add(Strings.Get("errors", "notconnected"));
            }
        }

        private void LoadCredentials()
        {
            string name = Globals.Database.LoadPreference("Username");
            if (!string.IsNullOrEmpty(name))
            {
                _usernameTextbox.Text = name;
                string pass = Globals.Database.LoadPreference("Password");
                if (!string.IsNullOrEmpty(pass))
                {
                    _passwordTextbox.Text = "*********";
                    _savedPass = pass;
                    _useSavedPass = true;
                    _savePassChk.IsChecked = true;
                }
            }
        }

        private void SaveCredentials()
        {
            var sha = new SHA256Managed();
            if (_savePassChk.IsChecked)
            {
                Globals.Database.SavePreference("Username", _usernameTextbox.Text.Trim());
                Globals.Database.SavePreference("Password",
                    BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim())))
                        .Replace("-", ""));
            }
            else
            {
                Globals.Database.SavePreference("Username", "");
                Globals.Database.SavePreference("Password", "");
            }
        }
    }
}