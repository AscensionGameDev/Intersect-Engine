using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Menu
{

    public class RegisterWindow
    {

        private Button mBackBtn;

        private ImagePanel mEmailBackground;

        private Label mEmailLabel;

        private TextBox mEmailTextbox;

        //Parent
        private MainMenu mMainMenu;

        private ImagePanel mPasswordBackground;

        private ImagePanel mPasswordBackground2;

        private Label mPasswordLabel;

        private Label mPasswordLabel2;

        private TextBoxPassword mPasswordTextbox;

        private TextBoxPassword mPasswordTextbox2;

        private Button mRegisterBtn;

        private Label mRegistrationHeader;

        //Controls
        private ImagePanel mRegistrationPanel;

        private ImagePanel mUsernameBackground;

        private Label mUsernameLabel;

        private TextBox mUsernameTextbox;

        //Init
        public RegisterWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mRegistrationPanel = new ImagePanel(parent, "RegistrationWindow");

            //Menu Header
            mRegistrationHeader = new Label(mRegistrationPanel, "RegistrationLabel");
            mRegistrationHeader.SetText(Strings.Registration.title);

            //Register Username Background
            mUsernameBackground = new ImagePanel(mRegistrationPanel, "UsernamePanel");

            //Register Username Label
            mUsernameLabel = new Label(mUsernameBackground, "UsernameLabel");
            mUsernameLabel.SetText(Strings.Registration.username);

            //Register Username Textbox
            mUsernameTextbox = new TextBox(mUsernameBackground, "UsernameField");
            mUsernameTextbox.SubmitPressed += UsernameTextbox_SubmitPressed;

            //Register Email Background
            mEmailBackground = new ImagePanel(mRegistrationPanel, "EmailPanel");

            //Register Email Label
            mEmailLabel = new Label(mEmailBackground, "EmailLabel");
            mEmailLabel.SetText(Strings.Registration.email);

            //Register Email Textbox
            mEmailTextbox = new TextBox(mEmailBackground, "EmailField");
            mEmailTextbox.SubmitPressed += EmailTextbox_SubmitPressed;

            //Register Password Background
            mPasswordBackground = new ImagePanel(mRegistrationPanel, "Password1Panel");

            //Register Password Label
            mPasswordLabel = new Label(mPasswordBackground, "Password1Label");
            mPasswordLabel.SetText(Strings.Registration.password);

            //Register Password Textbox
            mPasswordTextbox = new TextBoxPassword(mPasswordBackground, "Password1Field");
            mPasswordTextbox.SubmitPressed += PasswordTextbox_SubmitPressed;

            //Register Password Background
            mPasswordBackground2 = new ImagePanel(mRegistrationPanel, "Password2Panel");

            //Register Password Label2
            mPasswordLabel2 = new Label(mPasswordBackground2, "Password2Label");
            mPasswordLabel2.SetText(Strings.Registration.confirmpass);

            //Register Password Textbox2
            mPasswordTextbox2 = new TextBoxPassword(mPasswordBackground2, "Password2Field");
            mPasswordTextbox2.SubmitPressed += PasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            mRegisterBtn = new Button(mRegistrationPanel, "RegisterButton");
            mRegisterBtn.SetText(Strings.Registration.register);
            mRegisterBtn.Clicked += RegisterBtn_Clicked;

            //Register - Back Button
            mBackBtn = new Button(mRegistrationPanel, "BackButton");
            mBackBtn.SetText(Strings.Registration.back);
            mBackBtn.Clicked += BackBtn_Clicked;

            mRegistrationPanel.LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer.GetResolutionString());
        }

        public bool IsHidden => mRegistrationPanel.IsHidden;

        //Methods
        public void Update()
        {
            if (!Networking.Network.Connected)
            {
                Hide();
                mMainMenu.Show();
                Interface.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.lostconnection));
            }

            // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
            if (!Globals.WaitingOnServer && mRegisterBtn.IsDisabled)
            {
                mRegisterBtn.Enable();
            }
        }

        public void Show()
        {
            mRegistrationPanel.Show();
            mUsernameTextbox.Focus();
        }

        public void Hide()
        {
            mRegistrationPanel.Hide();
        }

        private static string ComputePasswordHash(string password)
        {
            using (var sha = new SHA256Managed())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""))).Replace("-", "");
            }
        }

        void TryRegister()
        {
            if (Globals.WaitingOnServer)
            {
                return;
            }

            if (Networking.Network.Connected)
            {
                if (FieldChecking.IsValidUsername(mUsernameTextbox.Text, Strings.Regex.username))
                {
                    if (mPasswordTextbox.Text == mPasswordTextbox2.Text)
                    {
                        if (FieldChecking.IsValidPassword(mPasswordTextbox.Text, Strings.Regex.password))
                        {
                            if (FieldChecking.IsWellformedEmailAddress(mEmailTextbox.Text, Strings.Regex.email))
                            {
                                Hide();

                                //Hash Password
                                using (var sha = new SHA256Managed())
                                {
                                    var hashedPass = BitConverter.ToString(
                                            sha.ComputeHash(Encoding.UTF8.GetBytes(mPasswordTextbox.Text.Trim()))
                                        )
                                        .Replace("-", "");

                                    PacketSender.SendCreateAccount(
                                        mUsernameTextbox.Text, hashedPass, mEmailTextbox.Text
                                    );
                                }

                                Globals.WaitingOnServer = true;
                                mRegisterBtn.Disable();
                                ChatboxMsg.ClearMessages();
                            }
                            else
                            {
                                Interface.MsgboxErrors.Add(
                                    new KeyValuePair<string, string>("", Strings.Registration.emailinvalid)
                                );
                            }
                        }
                        else
                        {
                            Interface.MsgboxErrors.Add(
                                new KeyValuePair<string, string>("", Strings.Errors.passwordinvalid)
                            );
                        }
                    }
                    else
                    {
                        Interface.MsgboxErrors.Add(
                            new KeyValuePair<string, string>("", Strings.Registration.passwordmatch)
                        );
                    }
                }
                else
                {
                    Interface.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.usernameinvalid));
                }
            }
            else
            {
                Interface.MsgboxErrors.Add(new KeyValuePair<string, string>("", Strings.Errors.notconnected));
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
            mMainMenu.Show();
        }

    }

}
