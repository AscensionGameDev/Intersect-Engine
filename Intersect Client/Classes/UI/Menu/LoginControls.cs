using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Menu
{
    public class LoginControls : IGUIElement
    {
        //Controls
        private Label _usernameLabel;
        private TextBox _usernameTextbox;
        private Label _passwordLabel;
        private TextBoxPassword _passwordTextbox;
        private Button _loginBtn;
        private Button _backBtn;
        private LabeledCheckBox _savePassChk;

        //Parent
        private MainMenu _mainMenu;

        //Init
        public LoginControls(WindowControl _parent, MainMenu mainMenu)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Login Username Label
            _usernameLabel = new Label(_parent);
            _usernameLabel.SetText("Username:");
            _usernameLabel.SetPosition(_parent.Width / 2 - 120 / 2, 12);
            _usernameLabel.IsHidden = true;

            //Login Username Textbox
            _usernameTextbox = new TextBox(_parent);
            _usernameTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 24);
            _usernameTextbox.SetSize(120, 14);
            _usernameTextbox.IsHidden = true;
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;

            //Login Password Label
            _passwordLabel = new Label(_parent);
            _passwordLabel.SetText("Password:");
            _passwordLabel.SetPosition(_parent.Width / 2 - 120 / 2, 42);
            _passwordLabel.IsHidden = true;

            //Login Password Textbox
            _passwordTextbox = new TextBoxPassword(_parent);
            _passwordTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 54);
            _passwordTextbox.SetSize(120, 14);
            _passwordTextbox.IsHidden = true;
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;

            //Login Save Pass Checkbox
            _savePassChk = new LabeledCheckBox(_parent) { Text = "Save Password" };
            _savePassChk.SetSize(120, 14);
            _savePassChk.SetPosition(_parent.Width / 2 - 120 / 2, 72);
            _savePassChk.IsHidden = true;

            //Login - Send Login Button
            _loginBtn = new Button(_parent);
            _loginBtn.SetText("Login");
            _loginBtn.SetPosition(_parent.Width / 2 - 120 / 2, 94);
            _loginBtn.SetSize(56, 32);
            _loginBtn.IsHidden = true;
            _loginBtn.Clicked += LoginBtn_Clicked;

            //Login - Back Button
            _backBtn = new Button(_parent);
            _backBtn.SetText("Back");
            _backBtn.SetPosition(_parent.Width / 2 + 4, 94);
            _backBtn.SetSize(56, 32);
            _backBtn.IsHidden = true;
            _backBtn.Clicked += BackBtn_Clicked;
        }

        //Methods
        public void Update()
        {

        }
        public void Hide()
        {
            _usernameLabel.Hide();
            _usernameTextbox.Hide();
            _passwordLabel.Hide();
            _passwordTextbox.Hide();
            _savePassChk.Hide();
            _loginBtn.Hide();
            _backBtn.Hide();
        }
        public void Show()
        {
            _usernameLabel.Show();
            _usernameTextbox.Show();
            _passwordLabel.Show();
            _passwordTextbox.Show();
            _savePassChk.Show();
            _loginBtn.Show();
            _backBtn.Show();
        }

        //Input Handlers
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _mainMenu.Reset();
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
            if (Globals.WaitingOnServer) { return; }
            if (FieldChecking.IsValidName(_usernameTextbox.Text))
            {
                if (FieldChecking.IsValidPass(_passwordTextbox.Text))
                {
                    Graphics.FadeStage = 2;
                    PacketSender.SendLogin(_usernameTextbox.Text, _passwordTextbox.Text);
                    Globals.WaitingOnServer = true;
                }
                else
                {
                    Gui.MsgboxErrors.Add("Password is invalid. Please user alphanumeric characters with a length between 4 and 20");
                }
            }
            else
            {
                Gui.MsgboxErrors.Add("Username is invalid. Please user alphanumeric characters with a length between 2 and 20");
            }
        }
    }
}
