using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gwen;
using Gwen.Control;

namespace Intersect_Client.Classes
{
    public class MenuGui
    {
        private readonly Canvas _menuCanvas;
        public MenuGui(Canvas myCanvas)
        {
            _menuCanvas = myCanvas;
            InitMenuGui();
        }

        //GUI Elements
        public WindowControl MenuWindow;

        //Main Menu
        public Button LoginButton;
        public Button RegisterButton;
        public Button OptionsButton;
        public Button ExitButton;

        //Login Menu
        public Label LUsernameLabel;
        public TextBox LUsernameTextbox;
        public Label LPasswordLabel;
        public TextBoxPassword LPasswordTextbox;
        public Button LLoginBtn;
        public Button LBackBtn;
        public LabeledCheckBox LSavePassChk;

        //Register Menu
        public Label RUsernameLabel;
        public TextBox RUsernameTextbox;
        public Label REmailLabel;
        public TextBox REmailTextbox;
        public Label RPasswordLabel;
        public TextBoxPassword RPasswordTextbox;
        public Label RPasswordLabel2;
        public TextBoxPassword RPasswordTextbox2;
        public Button RRegisterBtn;
        public Button RBackBtn;

        //Options Menu
        public Label OResolutionLabel;
        public ComboBox OResolutionList;
        public LabeledCheckBox OFullscreen;
        public Label OSoundLabel;
        public LabeledCheckBox OEnableSound;
        public LabeledCheckBox ODisableSound;
        public Label OMusicLabel;
        public LabeledCheckBox OEnableMusic;
        public LabeledCheckBox ODisableMusic;
        public Button OApplyBtn;
        public Button OBackBtn;

        #region "Menu GUI"
        private void InitMenuGui()
        {

            //Main Menu Window
            MenuWindow = new WindowControl(_menuCanvas, "Main Menu");
            MenuWindow.SetSize(200, 200);
            MenuWindow.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
            MenuWindow.IsClosable = false;
            MenuWindow.DisableResizing();
            MenuWindow.Margin = Margin.Zero;
            MenuWindow.Padding = Padding.Zero;

            //Login Button
            LoginButton = new Button(MenuWindow);
            LoginButton.SetText("Login");
            LoginButton.SetSize(120, 32);
            LoginButton.SetPosition(MenuWindow.Width / 2 - 120 / 2, 14);
            LoginButton.Clicked += m_loginButton_Clicked;

            //Register Button
            RegisterButton = new Button(MenuWindow);
            RegisterButton.SetText("Register");
            RegisterButton.SetSize(120, 32);
            RegisterButton.SetPosition(MenuWindow.Width / 2 - 120 / 2, 54);
            RegisterButton.Clicked += m_registerButton_Clicked;

            //Options Button
            OptionsButton = new Button(MenuWindow);
            OptionsButton.SetText("Options");
            OptionsButton.SetSize(120, 32);
            OptionsButton.SetPosition(MenuWindow.Width / 2 - 120 / 2, 94);
            OptionsButton.Clicked += m_optionsButton_Clicked;

            //Exit Button
            ExitButton = new Button(MenuWindow);
            ExitButton.SetText("Exit");
            ExitButton.SetSize(120, 32);
            ExitButton.SetPosition(MenuWindow.Width / 2 - 120 / 2, 134);
            ExitButton.Clicked += m_exitButton_Clicked;

            //Login Username Label
            LUsernameLabel = new Label(MenuWindow);
            LUsernameLabel.SetText("Username:");
            LUsernameLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 12);
            LUsernameLabel.IsHidden = true;

            //Login Username Textbox
            LUsernameTextbox = new TextBox(MenuWindow);
            LUsernameTextbox.SetPosition(MenuWindow.Width / 2 - 120 / 2, 24);
            LUsernameTextbox.SetSize(120, 14);
            LUsernameTextbox.IsHidden = true;
            LUsernameTextbox.SubmitPressed += m_lUsernameTextbox_SubmitPressed;

            //Login Password Label
            LPasswordLabel = new Label(MenuWindow);
            LPasswordLabel.SetText("Password:");
            LPasswordLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 42);
            LPasswordLabel.IsHidden = true;

            //Login Password Textbox
            LPasswordTextbox = new TextBoxPassword(MenuWindow);
            LPasswordTextbox.SetPosition(MenuWindow.Width / 2 - 120 / 2, 54);
            LPasswordTextbox.SetSize(120, 14);
            LPasswordTextbox.IsHidden = true;
            LPasswordTextbox.SubmitPressed += m_lPasswordTextbox_SubmitPressed;

            //Login Save Pass Checkbox
            LSavePassChk = new LabeledCheckBox(MenuWindow) {Text = "Save Password"};
            LSavePassChk.SetSize(120, 14);
            LSavePassChk.SetPosition(MenuWindow.Width / 2 - 120 / 2, 72);
            LSavePassChk.IsHidden = true;

            //Login - Send Login Button
            LLoginBtn = new Button(MenuWindow);
            LLoginBtn.SetText("Login");
            LLoginBtn.SetPosition(MenuWindow.Width / 2 - 120 / 2, 94);
            LLoginBtn.SetSize(56, 32);
            LLoginBtn.IsHidden = true;
            LLoginBtn.Clicked += m_lLoginBtn_Clicked;

            //Login - Back Button
            LBackBtn = new Button(MenuWindow);
            LBackBtn.SetText("Back");
            LBackBtn.SetPosition(MenuWindow.Width / 2 + 4, 94);
            LBackBtn.SetSize(56, 32);
            LBackBtn.IsHidden = true;
            LBackBtn.Clicked += m_lBackBtn_Clicked;


            //Register Username Label
            RUsernameLabel = new Label(MenuWindow);
            RUsernameLabel.SetText("Username:");
            RUsernameLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 12);
            RUsernameLabel.IsHidden = true;

            //Register Username Textbox
            RUsernameTextbox = new TextBox(MenuWindow);
            RUsernameTextbox.SetPosition(MenuWindow.Width / 2 - 120 / 2, 24);
            RUsernameTextbox.SetSize(120, 14);
            RUsernameTextbox.IsHidden = true;
            RUsernameTextbox.SubmitPressed += m_rUsernameTextbox_SubmitPressed;

            //Register Email Label
            REmailLabel = new Label(MenuWindow);
            REmailLabel.SetText("Email:");
            REmailLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 42);
            REmailLabel.IsHidden = true;

            //Register Email Textbox
            REmailTextbox = new TextBox(MenuWindow);
            REmailTextbox.SetPosition(MenuWindow.Width / 2 - 120 / 2, 54);
            REmailTextbox.SetSize(120, 14);
            REmailTextbox.IsHidden = true;
            REmailTextbox.SubmitPressed += m_rEmailTextbox_SubmitPressed;

            //Register Password Label
            RPasswordLabel = new Label(MenuWindow);
            RPasswordLabel.SetText("Password:");
            RPasswordLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 72);
            RPasswordLabel.IsHidden = true;

            //Register Password Textbox
            RPasswordTextbox = new TextBoxPassword(MenuWindow);
            RPasswordTextbox.SetPosition(MenuWindow.Width / 2 - 120 / 2, 84);
            RPasswordTextbox.SetSize(120, 14);
            RPasswordTextbox.IsHidden = true;
            RPasswordTextbox.SubmitPressed += m_rPasswordTextbox_SubmitPressed;

            //Register Password Label2
            RPasswordLabel2 = new Label(MenuWindow);
            RPasswordLabel2.SetText("Re-Enter Password:");
            RPasswordLabel2.SetPosition(MenuWindow.Width / 2 - 120 / 2, 102);
            RPasswordLabel2.IsHidden = true;

            //Register Password Textbox2
            RPasswordTextbox2 = new TextBoxPassword(MenuWindow);
            RPasswordTextbox2.SetPosition(MenuWindow.Width / 2 - 120 / 2, 114);
            RPasswordTextbox2.SetSize(120, 14);
            RPasswordTextbox2.IsHidden = true;
            RPasswordTextbox2.SubmitPressed += m_rPasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            RRegisterBtn = new Button(MenuWindow);
            RRegisterBtn.SetText("Register");
            RRegisterBtn.SetPosition(MenuWindow.Width / 2 - 120 / 2, 132);
            RRegisterBtn.SetSize(56, 32);
            RRegisterBtn.IsHidden = true;
            RRegisterBtn.Clicked += m_rRegisterBtn_Clicked;

            //Register - Back Button
            RBackBtn = new Button(MenuWindow);
            RBackBtn.SetText("Back");
            RBackBtn.SetPosition(MenuWindow.Width / 2 + 4, 132);
            RBackBtn.SetSize(56, 32);
            RBackBtn.IsHidden = true;
            RBackBtn.Clicked += m_lBackBtn_Clicked; //Sharing the Login Back button function, they both open the main menu.

            //Options - Resolution Label
            OResolutionLabel = new Label(MenuWindow);
            OResolutionLabel.SetText("Resolution:");
            OResolutionLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 12);
            OResolutionLabel.IsHidden = true;

            //Options - Resolution Drop Down List
            OResolutionList = new ComboBox(MenuWindow);
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                OResolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
            }
            OResolutionList.SetSize(120, 14);
            OResolutionList.SetPosition(MenuWindow.Width / 2 - 120 / 2, 28);
            OResolutionList.IsHidden = true;

            //Options - Fullscreen Checkbox
            OFullscreen = new LabeledCheckBox(MenuWindow) {Text = "Fullscreen"};
            OFullscreen.SetSize(120, 14);
            OFullscreen.SetPosition(MenuWindow.Width / 2 - 120 / 2, 46);
            OFullscreen.IsHidden = true;

            //Options - Sound Label
            OSoundLabel = new Label(MenuWindow);
            OSoundLabel.SetText("Sound:");
            OSoundLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 64);
            OSoundLabel.IsHidden = true;

            //Options - Sound On Checkbox
            OEnableSound = new LabeledCheckBox(MenuWindow) {Text = "On"};
            OEnableSound.SetSize(56, 14);
            OEnableSound.SetPosition(MenuWindow.Width / 2 - 120 / 2, 82);
            OEnableSound.IsHidden = true;
            OEnableSound.Checked += m_oEnableSound_Checked;

            //Options - Sound Off Checkbox
            ODisableSound = new LabeledCheckBox(MenuWindow) {Text = "Off"};
            ODisableSound.SetSize(56, 14);
            ODisableSound.SetPosition(MenuWindow.Width / 2 + 4, 82);
            ODisableSound.IsHidden = true;
            ODisableSound.Checked += m_oDisableSound_Checked;

            //Options - Music Label
            OMusicLabel = new Label(MenuWindow);
            OMusicLabel.SetText("Music:");
            OMusicLabel.SetPosition(MenuWindow.Width / 2 - 120 / 2, 100);
            OMusicLabel.IsHidden = true;

            //Options - Music On Checkbox
            OEnableMusic = new LabeledCheckBox(MenuWindow) {Text = "On"};
            OEnableMusic.SetSize(56, 14);
            OEnableMusic.SetPosition(MenuWindow.Width / 2 - 120 / 2, 118);
            OEnableMusic.IsHidden = true;
            OEnableMusic.Checked += m_oEnableMusic_Checked;

            //Options - Music Off Checkbox
            ODisableMusic = new LabeledCheckBox(MenuWindow) {Text = "Off"};
            ODisableMusic.SetSize(56, 14);
            ODisableMusic.SetPosition(MenuWindow.Width / 2 + 4, 118);
            ODisableMusic.IsHidden = true;
            ODisableMusic.Checked += m_oDisableMusic_Checked;

            //Options - Apply Button
            OApplyBtn = new Button(MenuWindow);
            OApplyBtn.SetText("Apply");
            OApplyBtn.SetPosition(MenuWindow.Width / 2 - 120 / 2, 136);
            OApplyBtn.SetSize(56, 32);
            OApplyBtn.IsHidden = true;
            OApplyBtn.Clicked += m_oApplyBtn_Clicked;

            //Options - Back Button
            OBackBtn = new Button(MenuWindow);
            OBackBtn.SetText("Back");
            OBackBtn.SetPosition(MenuWindow.Width / 2 + 4, 136);
            OBackBtn.SetSize(56, 32);
            OBackBtn.IsHidden = true;
            OBackBtn.Clicked += m_lBackBtn_Clicked; //Sharing the Login Back button function, they both open the main menu.
        }

        //Register Menu
        void m_rUsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void m_rEmailTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void m_rPasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void m_rPasswordTextbox2_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryRegister();
        }
        void m_rRegisterBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryRegister();
        }
        void TryRegister()
        {
            if (Globals.WaitingOnServer) { return; }
            if (IsValidName(RUsernameTextbox.Text))
            {
                if (RPasswordTextbox.Text == RPasswordTextbox2.Text)
                {
                    if (IsValidPass(RPasswordTextbox.Text))
                    {
                        if (IsEmail(REmailTextbox.Text))
                        {
                            Graphics.FadeStage = 2;
                            PacketSender.SendCreateAccount(RUsernameTextbox.Text, RPasswordTextbox.Text, REmailTextbox.Text);
                            Globals.WaitingOnServer = true;
                        }
                        else
                        {
                            AddError("Email is invalid!");
                        }
                    }
                    else
                    {
                        AddError("Password is invalid. Please user alphanumeric characters with a length between 4 and 20");
                    }
                }
                else
                {
                    AddError("Passwords didn't match!");
                }
            }
            else
            {
                AddError("Username is invalid. Please user alphanumeric characters with a length between 2 and 20");
            }
        }
        public bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            return false;
        }

        public bool IsValidName(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), UsernamePattern);
            return false;
        }

        public bool IsValidPass(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), PasswordPattern);
            return false;
        }

        //Options Menu
        void m_oDisableMusic_Checked(Base sender, EventArgs arguments)
        {
            OEnableMusic.IsChecked = false;
        }
        void m_oEnableMusic_Checked(Base sender, EventArgs arguments)
        {
            ODisableMusic.IsChecked = false;
        }
        void m_oDisableSound_Checked(Base sender, EventArgs arguments)
        {
            OEnableSound.IsChecked = false;
        }
        void m_oEnableSound_Checked(Base sender, EventArgs arguments)
        {
            ODisableSound.IsChecked = false;
        }
        void m_oApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var mi = OResolutionList.SelectedItem;
            var myModes = Graphics.GetValidVideoModes();
            for (var i = 0; i < myModes.Count(); i++)
            {
                if (mi.Text == myModes[i].Width + "x" + myModes[i].Height)
                {
                    Graphics.DisplayMode = i;
                    Graphics.MustReInit = true;
                }
            }
            if (Graphics.FullScreen != OFullscreen.IsChecked)
            {
                Graphics.FullScreen = OFullscreen.IsChecked;
                Graphics.MustReInit = true;
            }
            Globals.MusicEnabled = OEnableMusic.IsChecked;
            Globals.SoundEnabled = OEnableSound.IsChecked;
            Database.SaveOptions();
        }

        //Main Menu
        void m_loginButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            MenuWindow.Title = "Login";
            RegisterButton.Hide();
            ExitButton.Hide();
            LoginButton.Hide();
            OptionsButton.Hide();
            LUsernameLabel.Show();
            LUsernameTextbox.Show();
            LPasswordLabel.Show();
            LPasswordTextbox.Show();
            LSavePassChk.Show();
            LLoginBtn.Show();
            LBackBtn.Show();
        }
        void m_registerButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            MenuWindow.Title = "Register";
            RegisterButton.Hide();
            ExitButton.Hide();
            LoginButton.Hide();
            OptionsButton.Hide();
            RUsernameLabel.Show();
            RUsernameTextbox.Show();
            REmailLabel.Show();
            REmailTextbox.Show();
            RPasswordLabel.Show();
            RPasswordTextbox.Show();
            RPasswordLabel2.Show();
            RPasswordTextbox2.Show();
            RRegisterBtn.Show();
            RBackBtn.Show();
        }
        void m_optionsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            MenuWindow.Title = "Options";
            RegisterButton.Hide();
            ExitButton.Hide();
            LoginButton.Hide();
            OptionsButton.Hide();
            OResolutionLabel.Show();
            OResolutionList.Show();
            //Select current resolution
            OResolutionList.SelectByText(Graphics.GetValidVideoModes()[Graphics.DisplayMode].Width + "x" + Graphics.GetValidVideoModes()[Graphics.DisplayMode].Height);
            OFullscreen.Show();
            OFullscreen.IsChecked = Graphics.FullScreen;
            OSoundLabel.Show();
            OEnableSound.Show();
            OEnableSound.IsChecked = Globals.SoundEnabled;
            ODisableSound.Show();
            ODisableSound.IsChecked = !Globals.SoundEnabled;
            OMusicLabel.Show();
            OEnableMusic.Show();
            OEnableMusic.IsChecked = Globals.MusicEnabled;
            ODisableMusic.Show();
            ODisableMusic.IsChecked = !Globals.MusicEnabled;
            OApplyBtn.Show();
            OBackBtn.Show();
        }
        void m_exitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameMain.IsRunning = false;
        }

        //Login Menu
        void m_lBackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Show Menu Elements
            MenuWindow.Title = "Main Menu";
            RegisterButton.Show();
            ExitButton.Show();
            LoginButton.Show();
            OptionsButton.Show();

            //Hide Login Elements
            LUsernameLabel.Hide();
            LUsernameTextbox.Hide();
            LPasswordLabel.Hide();
            LPasswordTextbox.Hide();
            LSavePassChk.Hide();
            LLoginBtn.Hide();
            LBackBtn.Hide();

            //Hide Register Elements
            RUsernameLabel.Hide();
            RUsernameTextbox.Hide();
            REmailLabel.Hide();
            REmailTextbox.Hide();
            RPasswordLabel.Hide();
            RPasswordTextbox.Hide();
            RPasswordLabel2.Hide();
            RPasswordTextbox2.Hide();
            RRegisterBtn.Hide();
            RBackBtn.Hide();

            //Hide option elements
            OResolutionLabel.Hide();
            OResolutionList.Hide();
            OFullscreen.Hide();
            OSoundLabel.Hide();
            OEnableSound.Hide();
            ODisableSound.Hide();
            OMusicLabel.Hide();
            OEnableMusic.Hide();
            ODisableMusic.Hide();
            OApplyBtn.Hide();
            OBackBtn.Hide();
        }
        void m_lUsernameTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }
        void m_lPasswordTextbox_SubmitPressed(Base sender, EventArgs arguments)
        {
            TryLogin();
        }
        void m_lLoginBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            TryLogin();
        }
        public void TryLogin()
        {
            if (Globals.WaitingOnServer) { return; }
            if (IsValidName(LUsernameTextbox.Text))
            {
                if (IsValidPass(LPasswordTextbox.Text))
                {
                    Graphics.FadeStage = 2;
                    PacketSender.SendLogin(LUsernameTextbox.Text, LPasswordTextbox.Text);
                    Globals.WaitingOnServer = true;
                }
                else
                {
                    AddError("Password is invalid. Please user alphanumeric characters with a length between 4 and 20");
                }
            }
            else
            {
                AddError("Username is invalid. Please user alphanumeric characters with a length between 2 and 20");
            }
        }

        //Error Handling
        protected virtual void Msgbox_Resized(Base sender, EventArgs arguments)
        {
            sender.SetPosition(Graphics.ScreenWidth / 2 - sender.Width / 2, Graphics.ScreenHeight / 2 - sender.Height / 2);
        }
        public void AddError(string error)
        {
            Gui.MsgboxErrors.Add(error);
        }
        #endregion

        public void Draw()
        {
            while (Gui.MsgboxErrors.Count > 0)
            {
                var msgbox = new MessageBox(_menuCanvas, Gui.MsgboxErrors[0], "Error!");
                msgbox.Resized += Msgbox_Resized;
                Gui.MsgboxErrors.RemoveAt(0);
                m_lBackBtn_Clicked(null, null);
            }

            _menuCanvas.RenderCanvas();
        }

        //Field Checking
        public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public const string UsernamePattern = @"^[a-zA-Z0-9]{2,20}$";
        public const string PasswordPattern = @"^[a-zA-Z0-9]{4,20}$";
    }
}
