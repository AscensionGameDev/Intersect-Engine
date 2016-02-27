/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Security.Cryptography;
using System.Text;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

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

        private bool _useSavedPass = false;
        private string _savedPass = "";

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
            _passwordTextbox.TextChanged += _passwordTextbox_TextChanged;

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

            LoadCredentials();
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
        void _passwordTextbox_TextChanged(Base sender, EventArgs arguments)
        {
            _useSavedPass = false;
        }
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
            var sha = new SHA256Managed();
            if (Globals.WaitingOnServer) { return; }
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
                        PacketSender.SendLogin(_usernameTextbox.Text, BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim()))).Replace("-", ""));
                        SaveCredentials();
                        Globals.WaitingOnServer = true;
                    }
                    else
                    {
                        Gui.MsgboxErrors.Add("Password is invalid. Please user alphanumeric characters with a length between 4 and 20");
                    }
                }
            }
            else
            {
                Gui.MsgboxErrors.Add("Username is invalid. Please user alphanumeric characters with a length between 2 and 20");
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
                Globals.Database.SavePreference("Password", BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(_passwordTextbox.Text.Trim()))).Replace("-", ""));
            }
            else
            {
                Globals.Database.SavePreference("Username", "");
                Globals.Database.SavePreference("Password", "");
            }
        }
    }
}
