using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intersect_Client.Classes;

namespace Intersect_Client.Classes.UI.Menu
{
    public class RegisterControls : IGUIElement
    {
        //Controls
        private Label _usernameLabel;
        private TextBox _usernameTextbox;
        private Label _emailLabel;
        private TextBox _emailTextbox;
        private Label _passwordLabel;
        private TextBoxPassword _passwordTextbox;
        private Label _passwordLabel2;
        private TextBoxPassword _passwordTextbox2;
        private Button _registerBtn;
        private Button _backBtn;

        //Parent
        private MainMenu _mainMenu;

        //Init
        public RegisterControls(WindowControl _parent, MainMenu mainMenu)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Register Username Label
            _usernameLabel = new Label(_parent);
            _usernameLabel.SetText("Username:");
            _usernameLabel.SetPosition(_parent.Width / 2 - 120 / 2, 12);
            _usernameLabel.IsHidden = true;

            //Register Username Textbox
            _usernameTextbox = new TextBox(_parent);
            _usernameTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 24);
            _usernameTextbox.SetSize(120, 14);
            _usernameTextbox.IsHidden = true;
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;

            //Register Email Label
            _emailLabel = new Label(_parent);
            _emailLabel.SetText("Email:");
            _emailLabel.SetPosition(_parent.Width / 2 - 120 / 2, 42);
            _emailLabel.IsHidden = true;

            //Register Email Textbox
            _emailTextbox = new TextBox(_parent);
            _emailTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 54);
            _emailTextbox.SetSize(120, 14);
            _emailTextbox.IsHidden = true;
            _emailTextbox.SubmitPressed += EmailTextbox_SubmitPressed;

            //Register Password Label
            _passwordLabel = new Label(_parent);
            _passwordLabel.SetText("Password:");
            _passwordLabel.SetPosition(_parent.Width / 2 - 120 / 2, 72);
            _passwordLabel.IsHidden = true;

            //Register Password Textbox
            _passwordTextbox = new TextBoxPassword(_parent);
            _passwordTextbox.SetPosition(_parent.Width / 2 - 120 / 2, 84);
            _passwordTextbox.SetSize(120, 14);
            _passwordTextbox.IsHidden = true;
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;

            //Register Password Label2
            _passwordLabel2 = new Label(_parent);
            _passwordLabel2.SetText("Re-Enter Password:");
            _passwordLabel2.SetPosition(_parent.Width / 2 - 120 / 2, 102);
            _passwordLabel2.IsHidden = true;

            //Register Password Textbox2
            _passwordTextbox2 = new TextBoxPassword(_parent);
            _passwordTextbox2.SetPosition(_parent.Width / 2 - 120 / 2, 114);
            _passwordTextbox2.SetSize(120, 14);
            _passwordTextbox2.IsHidden = true;
            _passwordTextbox2.SubmitPressed += PasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            _registerBtn = new Button(_parent);
            _registerBtn.SetText("Register");
            _registerBtn.SetPosition(_parent.Width / 2 - 120 / 2, 132);
            _registerBtn.SetSize(56, 32);
            _registerBtn.IsHidden = true;
            _registerBtn.Clicked += RegisterBtn_Clicked;

            //Register - Back Button
            _backBtn = new Button(_parent);
            _backBtn.SetText("Back");
            _backBtn.SetPosition(_parent.Width / 2 + 4, 132);
            _backBtn.SetSize(56, 32);
            _backBtn.IsHidden = true;
            _backBtn.Clicked += BackBtn_Clicked; 

        }

        //Methods
        public void Update()
        {

        }
        public void Show()
        {
            _usernameLabel.Show();
            _usernameTextbox.Show();
            _emailLabel.Show();
            _emailTextbox.Show();
            _passwordLabel.Show();
            _passwordTextbox.Show();
            _passwordLabel2.Show();
            _passwordTextbox2.Show();
            _registerBtn.Show();
            _backBtn.Show();
        }
        public void Hide()
        {
            _usernameLabel.Hide();
            _usernameTextbox.Hide();
            _emailLabel.Hide();
            _emailTextbox.Hide();
            _passwordLabel.Hide();
            _passwordTextbox.Hide();
            _passwordLabel2.Hide();
            _passwordTextbox2.Hide();
            _registerBtn.Hide();
            _backBtn.Hide();
        }

        void TryRegister()
        {
            if (Globals.WaitingOnServer) { return; }
            if (FieldChecking.IsValidName(_usernameTextbox.Text))
            {
                if (_passwordTextbox.Text == _passwordTextbox2.Text)
                {
                    if (FieldChecking.IsValidPass(_passwordTextbox.Text))
                    {
                        if (FieldChecking.IsEmail(_emailTextbox.Text))
                        {
                            Graphics.FadeStage = 2;
                            PacketSender.SendCreateAccount(_usernameTextbox.Text, _passwordTextbox.Text, _emailTextbox.Text);
                            Globals.WaitingOnServer = true;
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add("Email is invalid!");
                        }
                    }
                    else
                    {
                        Gui.MsgboxErrors.Add("Password is invalid. Please user alphanumeric characters with a length between 4 and 20");
                    }
                }
                else
                {
                    Gui.MsgboxErrors.Add("Passwords didn't match!");
                }
            }
            else
            {
                Gui.MsgboxErrors.Add("Username is invalid. Please user alphanumeric characters with a length between 2 and 20");
            }
        }

        //Input Handlers
        void UsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void EmailTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void PasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void PasswordTextbox2_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void RegisterBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryRegister();
        }
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _mainMenu.Reset();
        }

    }
}
