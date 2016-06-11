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
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Misc;
using Intersect_Client.Classes.Networking;

namespace Intersect_Client.Classes.UI.Menu
{
    public class RegisterWindow
    {
        //Controls
        private ImagePanel _menuPanel;
        private Label _menuHeader;

        private ImagePanel _usernameBackground;
        private Label _usernameLabel;
        private TextBox _usernameTextbox;

        private ImagePanel _emailBackground;
        private Label _emailLabel;
        private TextBox _emailTextbox;

        private ImagePanel _passwordBackground;
        private Label _passwordLabel;
        private TextBoxPassword _passwordTextbox;

        private ImagePanel _passwordBackground2;
        private Label _passwordLabel2;
        private TextBoxPassword _passwordTextbox2;
        private Button _registerBtn;
        private Button _backBtn;

        //Parent
        private MainMenu _mainMenu;

        //Init
        public RegisterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _menuPanel = new ImagePanel(parent);
            _menuPanel.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png");
            _menuPanel.SetSize(512, 393);
            _menuPanel.SetPosition(parentPanel.X, parentPanel.Y);
            _menuPanel.IsHidden = true;

            //Menu Header
            _menuHeader = new Label(_menuPanel);
            _menuHeader.AutoSizeToContents = false;
            _menuHeader.SetText("Register");
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 24);
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

            //Register Username Background
            _usernameBackground = new ImagePanel(_menuPanel);
            _usernameBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _usernameBackground.SetSize(_usernameBackground.Texture.GetWidth(), _usernameBackground.Texture.GetHeight());
            _usernameBackground.SetPosition(_menuPanel.Width / 2 - _usernameBackground.Width / 2, 44);

            //Register Username Label
            _usernameLabel = new Label(_usernameBackground);
            _usernameLabel.SetText("Username:");
            _usernameLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);
            _usernameLabel.AutoSizeToContents = false;
            _usernameLabel.SetSize(176, 55);
            _usernameLabel.Alignment = Pos.Center;
            _usernameLabel.TextColorOverride = new Color(255, 30, 30, 30);

            //Register Username Textbox
            _usernameTextbox = new TextBox(_usernameBackground);
            _usernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;
            _usernameTextbox.SetPosition(190, 8);
            _usernameTextbox.SetSize(248, 38);
            _usernameTextbox.ShouldDrawBackground = false;
            _usernameTextbox.TextColorOverride = new Color(255, 220, 220, 220);
            _usernameTextbox.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register Email Background
            _emailBackground = new ImagePanel(_menuPanel);
            _emailBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _emailBackground.SetSize(_emailBackground.Texture.GetWidth(), _emailBackground.Texture.GetHeight());
            _emailBackground.SetPosition(_menuPanel.Width / 2 - _emailBackground.Width / 2, _usernameBackground.Bottom + 16);

            //Register Email Label
            _emailLabel = new Label(_emailBackground);
            _emailLabel.SetText("Email:");
            _emailLabel.AutoSizeToContents = false;
            _emailLabel.SetSize(176, 55);
            _emailLabel.Alignment = Pos.Center;
            _emailLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _emailLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register Email Textbox
            _emailTextbox = new TextBox(_emailBackground);
            _emailTextbox.SubmitPressed += EmailTextbox_SubmitPressed;
            _emailTextbox.SetPosition(190, 8);
            _emailTextbox.SetSize(248, 38);
            _emailTextbox.ShouldDrawBackground = false;
            _emailTextbox.TextColorOverride = new Color(255, 220, 220, 220);
            _emailTextbox.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register Password Background
            _passwordBackground = new ImagePanel(_menuPanel);
            _passwordBackground.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _passwordBackground.SetSize(_passwordBackground.Texture.GetWidth(), _passwordBackground.Texture.GetHeight());
            _passwordBackground.SetPosition(_menuPanel.Width / 2 - _passwordBackground.Width / 2, _emailBackground.Bottom + 16);

            //Register Password Label
            _passwordLabel = new Label(_passwordBackground);
            _passwordLabel.SetText("Password:");
            _passwordLabel.AutoSizeToContents = false;
            _passwordLabel.SetSize(176, 55);
            _passwordLabel.Alignment = Pos.Center;
            _passwordLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _passwordLabel.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register Password Textbox
            _passwordTextbox = new TextBoxPassword(_passwordBackground);
            _passwordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;
            _passwordTextbox.SetPosition(190, 8);
            _passwordTextbox.SetSize(248, 38);
            _passwordTextbox.ShouldDrawBackground = false;
            _passwordTextbox.TextColorOverride = new Color(255, 220, 220, 220);
            _passwordTextbox.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register Password Background
            _passwordBackground2 = new ImagePanel(_menuPanel);
            _passwordBackground2.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png");
            _passwordBackground2.SetSize(_passwordBackground2.Texture.GetWidth(), _passwordBackground2.Texture.GetHeight());
            _passwordBackground2.SetPosition(_menuPanel.Width / 2 - _passwordBackground2.Width / 2, _passwordBackground.Bottom + 16);

            //Register Password Label2
            _passwordLabel2 = new Label(_passwordBackground2);
            _passwordLabel2.SetText("Confirm Password:");
            _passwordLabel2.SetTextScale(.75f);
            _passwordLabel2.AutoSizeToContents = false;
            _passwordLabel2.SetSize(176, 55);
            _passwordLabel2.Alignment = Pos.Center;
            _passwordLabel2.TextColorOverride = new Color(255, 30, 30, 30);
            _passwordLabel2.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 18);

            //Register Password Textbox2
            _passwordTextbox2 = new TextBoxPassword(_passwordBackground2);
            _passwordTextbox2.SubmitPressed += PasswordTextbox2_SubmitPressed;
            _passwordTextbox2.SetPosition(190, 8);
            _passwordTextbox2.SetSize(248, 38);
            _passwordTextbox2.ShouldDrawBackground = false;
            _passwordTextbox2.TextColorOverride = new Color(255, 220, 220, 220);
            _passwordTextbox2.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register - Send Registration Button
            _registerBtn = new Button(_menuPanel);
            _registerBtn.SetText("Register");
            _registerBtn.Clicked += RegisterBtn_Clicked;
            _registerBtn.SetPosition(_usernameBackground.X, _passwordBackground2.Bottom + 16);
            _registerBtn.SetSize(211, 61);
            _registerBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"), Button.ControlState.Normal);
            _registerBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"), Button.ControlState.Hovered);
            _registerBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"), Button.ControlState.Clicked);
            _registerBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _registerBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _registerBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _registerBtn.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);

            //Register - Back Button
            _backBtn = new Button(_menuPanel);
            _backBtn.SetText("Back");
            _backBtn.Clicked += BackBtn_Clicked;
            _backBtn.SetSize(211, 61);
            _backBtn.SetPosition(_usernameBackground.Right - _backBtn.Width, _passwordBackground2.Bottom + 16);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"), Button.ControlState.Normal);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"), Button.ControlState.Hovered);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"), Button.ControlState.Clicked);
            _backBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _backBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _backBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _backBtn.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 20);


        }

        //Methods
        public void Update()
        {

        }
        public void Show()
        {
            _menuPanel.Show();
        }
        public void Hide()
        {
            _menuPanel.Hide();
        }

        void TryRegister()
        {
            if (Globals.WaitingOnServer) { return; }
            if (GameNetwork.Connected)
            {
                if (FieldChecking.IsValidName(_usernameTextbox.Text))
                {
                    if (_passwordTextbox.Text == _passwordTextbox2.Text)
                    {
                        if (FieldChecking.IsValidPass(_passwordTextbox.Text))
                        {
                            if (FieldChecking.IsEmail(_emailTextbox.Text))
                            {
                                GameFade.FadeOut();
                                Hide();
                                PacketSender.SendCreateAccount(_usernameTextbox.Text, _passwordTextbox.Text,
                                    _emailTextbox.Text);
                                Globals.WaitingOnServer = true;
                            }
                            else
                            {
                                Gui.MsgboxErrors.Add("Email is invalid!");
                            }
                        }
                        else
                        {
                            Gui.MsgboxErrors.Add(
                                "Password is invalid. Please use alphanumeric characters with a length between 4 and 20");
                        }
                    }
                    else
                    {
                        Gui.MsgboxErrors.Add("Passwords didn't match!");
                    }
                }
                else
                {
                    Gui.MsgboxErrors.Add(
                        "Username is invalid. Please use alphanumeric characters with a length between 2 and 20");
                }
            }
            else
            {
                Gui.MsgboxErrors.Add(
                        "Not connected to the game server. Is it online?");
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
            Hide();
            _mainMenu.Show();
        }

    }
}
