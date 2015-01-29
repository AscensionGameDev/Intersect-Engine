using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.Window;
using Gwen;
using Gwen.Renderer;
using Gwen.Control;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Text.RegularExpressions;

namespace Intersect_Client
{
    public static class GUI
    {

        //GWEN GUI
        private static Gwen.Input.SFML gwenInput;
        private static Canvas menuCanvas;
        private static Canvas gameCanvas;
        private static Gwen.Renderer.SFML gwenRenderer;
        private static Gwen.Skin.TexturedBase gwenSkin;
        private static Gwen.Font gwenFont;
        private static List<string> msgboxErrors = new List<string>();
        public static bool setupHandlers = false;
        public static bool focusChat = false;
        private static List<Gwen.Control.Base> focusElements = new List<Gwen.Control.Base>();

        //GUI Elements
        public static Gwen.Control.WindowControl menuWindow;

        //Main Menu
        public static Gwen.Control.Button m_loginButton;
        public static Gwen.Control.Button m_registerButton;
        public static Gwen.Control.Button m_optionsButton;
        public static Gwen.Control.Button m_exitButton;

        //Login Menu
        public static Gwen.Control.Label m_lUsernameLabel;
        public static Gwen.Control.TextBox m_lUsernameTextbox;
        public static Gwen.Control.Label m_lPasswordLabel;
        public static Gwen.Control.TextBoxPassword m_lPasswordTextbox;
        public static Gwen.Control.Button m_lLoginBtn;
        public static Gwen.Control.Button m_lBackBtn;
        public static Gwen.Control.LabeledCheckBox m_lSavePassChk;

        //Register Menu
        public static Gwen.Control.Label m_rUsernameLabel;
        public static Gwen.Control.TextBox m_rUsernameTextbox;
        public static Gwen.Control.Label m_rEmailLabel;
        public static Gwen.Control.TextBox m_rEmailTextbox;
        public static Gwen.Control.Label m_rPasswordLabel;
        public static Gwen.Control.TextBoxPassword m_rPasswordTextbox;
        public static Gwen.Control.Label m_rPasswordLabel2;
        public static Gwen.Control.TextBoxPassword m_rPasswordTextbox2;
        public static Gwen.Control.Button m_rRegisterBtn;
        public static Gwen.Control.Button m_rBackBtn;

        //Options Menu
        public static Gwen.Control.Label m_oResolutionLabel;
        public static Gwen.Control.ComboBox m_oResolutionList;
        public static Gwen.Control.LabeledCheckBox m_oFullscreen;
        public static Gwen.Control.Label m_oSoundLabel;
        public static Gwen.Control.LabeledCheckBox m_oEnableSound;
        public static Gwen.Control.LabeledCheckBox m_oDisableSound;
        public static Gwen.Control.Label m_oMusicLabel;
        public static Gwen.Control.LabeledCheckBox m_oEnableMusic;
        public static Gwen.Control.LabeledCheckBox m_oDisableMusic;
        public static Gwen.Control.Button m_oApplyBtn;
        public static Gwen.Control.Button m_oBackBtn;

        //END MENU GUI

        //BEGIN GAME GUI
        public static Gwen.Control.WindowControl g_eventDialogWindow;
        public static Gwen.Control.ListBox g_eventDialog;
        public static Gwen.Control.Button g_eventResponse1;
        public static Gwen.Control.Button g_eventResponse2;
        public static Gwen.Control.Button g_eventResponse3;
        public static Gwen.Control.Button g_eventResponse4;

        //ChatBox
        public static Gwen.Control.WindowControl g_ChatboxWindow;
        public static Gwen.Control.ListBox g_ChatBoxMessages;
        public static Gwen.Control.TextBox g_ChatBoxInput;
        public static Gwen.Control.Button g_ChatBoxSendBtn;

        //Menu
        public static Gwen.Control.WindowControl g_GameMenu;
        public static Gwen.Control.Button g_InventoryBtn;
        public static Gwen.Control.Button g_SkillsBtn;
        public static Gwen.Control.Button g_CharacterBtn;
        public static Gwen.Control.Button g_QuestsBtn;
        public static Gwen.Control.Button g_OptionBtn;
        public static Gwen.Control.Button g_CloseBtn;

        //Options Menu
        public static Gwen.Control.WindowControl g_oMenu;
        public static Gwen.Control.Label g_oResolutionLabel;
        public static Gwen.Control.ComboBox g_oResolutionList;
        public static Gwen.Control.LabeledCheckBox g_oFullscreen;
        public static Gwen.Control.Label g_oSoundLabel;
        public static Gwen.Control.LabeledCheckBox g_oEnableSound;
        public static Gwen.Control.LabeledCheckBox g_oDisableSound;
        public static Gwen.Control.Label g_oMusicLabel;
        public static Gwen.Control.LabeledCheckBox g_oEnableMusic;
        public static Gwen.Control.LabeledCheckBox g_oDisableMusic;
        public static Gwen.Control.Button g_oApplyBtn;
        public static Gwen.Control.Button g_oBackBtn;


        //Field Checking
        public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public const string UsernamePattern = @"^[a-zA-Z0-9]{2,20}$";
        public const string PasswordPattern = @"^[a-zA-Z0-9]{4,20}$";


        #region "Gwen Setup and Input"
        //Gwen Low Level Functions
        public static void InitGwen()
        {

            gwenRenderer = new Gwen.Renderer.SFML(Graphics.renderWindow);
            //TODO: Make it easier to modify skin.
            gwenSkin = new Gwen.Skin.TexturedBase(gwenRenderer, "DefaultSkin.png");



            //TODO Move font system over from Orion
            // try to load, fallback if failed
            gwenFont = new Gwen.Font(gwenRenderer) { Size = 10, FaceName = "Arvo-Regular.ttf" };
            if (gwenRenderer.LoadFont(gwenFont))
            {
                gwenRenderer.FreeFont(gwenFont);
            }
            else // try another
            {
                gwenFont.FaceName = "Arial";
                if (gwenRenderer.LoadFont(gwenFont))
                {
                    gwenRenderer.FreeFont(gwenFont);
                }
                else // try default
                {
                    gwenFont.FaceName = "OpenSans.ttf";
                }
            }

            gwenSkin.SetDefaultFont(gwenFont.FaceName);
            gwenFont.Dispose(); // skin has its own


            // Create a Canvas (it's root, on which all other GWEN controls are created)
            menuCanvas = new Canvas(gwenSkin);
            menuCanvas.SetSize(Graphics.ScreenWidth, Graphics.ScreenHeight);
            menuCanvas.ShouldDrawBackground = false;
            menuCanvas.BackgroundColor = System.Drawing.Color.FromArgb(255, 150, 170, 170);
            menuCanvas.KeyboardInputEnabled = true;

            // Create the game Canvas (it's root, on which all other GWEN controls are created)
            gameCanvas = new Canvas(gwenSkin);
            gameCanvas.SetSize(Graphics.ScreenWidth, Graphics.ScreenHeight);
            gameCanvas.ShouldDrawBackground = false;
            gameCanvas.BackgroundColor = System.Drawing.Color.FromArgb(255, 150, 170, 170);
            gameCanvas.KeyboardInputEnabled = true;

            // Create GWEN input processor
            gwenInput = new Gwen.Input.SFML();
            if (Globals.GameState == 0)
            {
                gwenInput.Initialize(menuCanvas, Graphics.renderWindow);
            }
            else
            {
                gwenInput.Initialize(gameCanvas, Graphics.renderWindow);
            }

            // Setup event handlers
            if (setupHandlers == false)
            {
                Graphics.renderWindow.Closed += OnClosed;
                Graphics.renderWindow.KeyPressed += OnKeyPressed;
                Graphics.renderWindow.KeyReleased += window_KeyReleased;
                Graphics.renderWindow.MouseButtonPressed += window_MouseButtonPressed;
                Graphics.renderWindow.MouseButtonReleased += window_MouseButtonReleased;
                Graphics.renderWindow.MouseWheelMoved += window_MouseWheelMoved;
                Graphics.renderWindow.MouseMoved += window_MouseMoved;
                Graphics.renderWindow.TextEntered += window_TextEntered;
                setupHandlers = true;
            }



            if (Globals.GameState == 0)
            {
                InitMenuGUI();
            }
            else
            {
                InitGameGUI();
            }


        }
        public static void DestroyGwen()
        {
            //The canvases dispose of all of their children.
            menuCanvas.Dispose();
            gameCanvas.Dispose();
            gwenRenderer.Dispose();
            gwenSkin.Dispose();
            gwenFont.Dispose();
        }
        static void window_TextEntered(object sender, TextEventArgs e) { 
            gwenInput.ProcessMessage(e); }
        static void window_MouseMoved(object sender, MouseMoveEventArgs e) { gwenInput.ProcessMessage(e); }
        static void window_MouseWheelMoved(object sender, MouseWheelEventArgs e) { gwenInput.ProcessMessage(e); }
        static void window_MouseButtonPressed(object sender, MouseButtonEventArgs e) { gwenInput.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, true)); }
        static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e) { gwenInput.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, false)); }
        static void window_KeyReleased(object sender, KeyEventArgs e) { gwenInput.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, false)); }
        static void OnClosed(object sender, EventArgs e) { GameMain.isRunning = false; }
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                GameMain.isRunning = false;
            }
            else
            {
                gwenInput.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, true));
                
            }
        }
        #endregion

        #region "Menu GUI"
        private static void InitMenuGUI()
        {
            
            //Main Menu Window
            menuWindow = new Gwen.Control.WindowControl(menuCanvas, "Main Menu", false);
            menuWindow.SetSize(200, 200);
            menuWindow.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
            menuWindow.IsClosable = false;
            menuWindow.DisableResizing();
            menuWindow.Margin = Gwen.Margin.Zero;
            menuWindow.Padding = Padding.Zero;

            //Login Button
            m_loginButton = new Gwen.Control.Button(menuWindow);
            m_loginButton.SetText("Login");
            m_loginButton.SetSize(120, 32);
            m_loginButton.SetPosition(menuWindow.Width / 2 - 120 / 2, 14);
            m_loginButton.Clicked += m_loginButton_Clicked;

            //Register Button
            m_registerButton = new Gwen.Control.Button(menuWindow);
            m_registerButton.SetText("Register");
            m_registerButton.SetSize(120, 32);
            m_registerButton.SetPosition(menuWindow.Width / 2 - 120 / 2, 54);
            m_registerButton.Clicked += m_registerButton_Clicked;

            //Options Button
            m_optionsButton = new Gwen.Control.Button(menuWindow);
            m_optionsButton.SetText("Options");
            m_optionsButton.SetSize(120, 32);
            m_optionsButton.SetPosition(menuWindow.Width / 2 - 120 / 2, 94);
            m_optionsButton.Clicked += m_optionsButton_Clicked;

            //Exit Button
            m_exitButton = new Gwen.Control.Button(menuWindow);
            m_exitButton.SetText("Exit");
            m_exitButton.SetSize(120, 32);
            m_exitButton.SetPosition(menuWindow.Width / 2 - 120 / 2, 134);
            m_exitButton.Clicked += m_exitButton_Clicked;

            //Login Username Label
            m_lUsernameLabel = new Gwen.Control.Label(menuWindow);
            m_lUsernameLabel.SetText("Username:");
            m_lUsernameLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 12);
            m_lUsernameLabel.IsHidden = true;

            //Login Username Textbox
            m_lUsernameTextbox = new Gwen.Control.TextBox(menuWindow);
            m_lUsernameTextbox.SetPosition(menuWindow.Width / 2 - 120 / 2, 24);
            m_lUsernameTextbox.SetSize(120, 14);
            m_lUsernameTextbox.IsHidden = true;
            m_lUsernameTextbox.SubmitPressed += m_lUsernameTextbox_SubmitPressed;

            //Login Password Label
            m_lPasswordLabel = new Gwen.Control.Label(menuWindow);
            m_lPasswordLabel.SetText("Password:");
            m_lPasswordLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 42);
            m_lPasswordLabel.IsHidden = true;

            //Login Password Textbox
            m_lPasswordTextbox = new Gwen.Control.TextBoxPassword(menuWindow);
            m_lPasswordTextbox.SetPosition(menuWindow.Width / 2 - 120 / 2, 54);
            m_lPasswordTextbox.SetSize(120, 14);
            m_lPasswordTextbox.IsHidden = true;
            m_lPasswordTextbox.SubmitPressed += m_lPasswordTextbox_SubmitPressed;

            //Login Save Pass Checkbox
            m_lSavePassChk = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_lSavePassChk.Text = "Save Password";
            m_lSavePassChk.SetSize(120, 14);
            m_lSavePassChk.SetPosition(menuWindow.Width / 2 - 120 / 2, 72);
            m_lSavePassChk.IsHidden = true;

            //Login - Send Login Button
            m_lLoginBtn = new Gwen.Control.Button(menuWindow);
            m_lLoginBtn.SetText("Login");
            m_lLoginBtn.SetPosition(menuWindow.Width / 2 - 120 / 2, 94);
            m_lLoginBtn.SetSize(56, 32);
            m_lLoginBtn.IsHidden = true;
            m_lLoginBtn.Clicked += m_lLoginBtn_Clicked;

            //Login - Back Button
            m_lBackBtn = new Gwen.Control.Button(menuWindow);
            m_lBackBtn.SetText("Back");
            m_lBackBtn.SetPosition(menuWindow.Width / 2 + 4, 94);
            m_lBackBtn.SetSize(56, 32);
            m_lBackBtn.IsHidden = true;
            m_lBackBtn.Clicked += m_lBackBtn_Clicked;


            //Register Username Label
            m_rUsernameLabel = new Gwen.Control.Label(menuWindow);
            m_rUsernameLabel.SetText("Username:");
            m_rUsernameLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 12);
            m_rUsernameLabel.IsHidden = true;

            //Register Username Textbox
            m_rUsernameTextbox = new Gwen.Control.TextBox(menuWindow);
            m_rUsernameTextbox.SetPosition(menuWindow.Width / 2 - 120 / 2, 24);
            m_rUsernameTextbox.SetSize(120, 14);
            m_rUsernameTextbox.IsHidden = true;
            m_rUsernameTextbox.SubmitPressed += m_rUsernameTextbox_SubmitPressed;

            //Register Email Label
            m_rEmailLabel = new Gwen.Control.Label(menuWindow);
            m_rEmailLabel.SetText("Email:");
            m_rEmailLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 42);
            m_rEmailLabel.IsHidden = true;

            //Register Email Textbox
            m_rEmailTextbox = new Gwen.Control.TextBox(menuWindow);
            m_rEmailTextbox.SetPosition(menuWindow.Width / 2 - 120 / 2, 54);
            m_rEmailTextbox.SetSize(120, 14);
            m_rEmailTextbox.IsHidden = true;
            m_rEmailTextbox.SubmitPressed += m_rEmailTextbox_SubmitPressed;

            //Register Password Label
            m_rPasswordLabel = new Gwen.Control.Label(menuWindow);
            m_rPasswordLabel.SetText("Password:");
            m_rPasswordLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 72);
            m_rPasswordLabel.IsHidden = true;

            //Register Password Textbox
            m_rPasswordTextbox = new Gwen.Control.TextBoxPassword(menuWindow);
            m_rPasswordTextbox.SetPosition(menuWindow.Width / 2 - 120 / 2, 84);
            m_rPasswordTextbox.SetSize(120, 14);
            m_rPasswordTextbox.IsHidden = true;
            m_rPasswordTextbox.SubmitPressed += m_rPasswordTextbox_SubmitPressed;

            //Register Password Label2
            m_rPasswordLabel2 = new Gwen.Control.Label(menuWindow);
            m_rPasswordLabel2.SetText("Re-Enter Password:");
            m_rPasswordLabel2.SetPosition(menuWindow.Width / 2 - 120 / 2, 102);
            m_rPasswordLabel2.IsHidden = true;

            //Register Password Textbox2
            m_rPasswordTextbox2 = new Gwen.Control.TextBoxPassword(menuWindow);
            m_rPasswordTextbox2.SetPosition(menuWindow.Width / 2 - 120 / 2, 114);
            m_rPasswordTextbox2.SetSize(120, 14);
            m_rPasswordTextbox2.IsHidden = true;
            m_rPasswordTextbox2.SubmitPressed += m_rPasswordTextbox2_SubmitPressed;

            //Register - Send Registration Button
            m_rRegisterBtn = new Gwen.Control.Button(menuWindow);
            m_rRegisterBtn.SetText("Register");
            m_rRegisterBtn.SetPosition(menuWindow.Width / 2 - 120 / 2, 132);
            m_rRegisterBtn.SetSize(56, 32);
            m_rRegisterBtn.IsHidden = true;
            m_rRegisterBtn.Clicked += m_rRegisterBtn_Clicked;

            //Register - Back Button
            m_rBackBtn = new Gwen.Control.Button(menuWindow);
            m_rBackBtn.SetText("Back");
            m_rBackBtn.SetPosition(menuWindow.Width / 2 + 4, 132);
            m_rBackBtn.SetSize(56, 32);
            m_rBackBtn.IsHidden = true;
            m_rBackBtn.Clicked += m_lBackBtn_Clicked; //Sharing the Login Back button function, they both open the main menu.

            //Options - Resolution Label
            m_oResolutionLabel = new Gwen.Control.Label(menuWindow);
            m_oResolutionLabel.SetText("Resolution:");
            m_oResolutionLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 12);
            m_oResolutionLabel.IsHidden = true;

            //Options - Resolution Drop Down List
            m_oResolutionList = new Gwen.Control.ComboBox(menuWindow);
            List<VideoMode> myModes = Graphics.GetValidVideoModes();
            for (int i = 0; i < myModes.Count(); i++)
            {
                m_oResolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
            }
            m_oResolutionList.SetSize(120, 14);
            m_oResolutionList.SetPosition(menuWindow.Width / 2 - 120 / 2, 28);
            m_oResolutionList.IsHidden = true;

            //Options - Fullscreen Checkbox
            m_oFullscreen = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_oFullscreen.Text = "Fullscreen";
            m_oFullscreen.SetSize(120, 14);
            m_oFullscreen.SetPosition(menuWindow.Width / 2 - 120 / 2, 46);
            m_oFullscreen.IsHidden = true;

            //Options - Sound Label
            m_oSoundLabel = new Gwen.Control.Label(menuWindow);
            m_oSoundLabel.SetText("Sound:");
            m_oSoundLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 64);
            m_oSoundLabel.IsHidden = true;

            //Options - Sound On Checkbox
            m_oEnableSound = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_oEnableSound.Text = "On";
            m_oEnableSound.SetSize(56, 14);
            m_oEnableSound.SetPosition(menuWindow.Width / 2 - 120 / 2, 82);
            m_oEnableSound.IsHidden = true;
            m_oEnableSound.Checked += m_oEnableSound_Checked;

            //Options - Sound Off Checkbox
            m_oDisableSound = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_oDisableSound.Text = "Off";
            m_oDisableSound.SetSize(56, 14);
            m_oDisableSound.SetPosition(menuWindow.Width / 2 + 4, 82);
            m_oDisableSound.IsHidden = true;
            m_oDisableSound.Checked += m_oDisableSound_Checked;

            //Options - Music Label
            m_oMusicLabel = new Gwen.Control.Label(menuWindow);
            m_oMusicLabel.SetText("Music:");
            m_oMusicLabel.SetPosition(menuWindow.Width / 2 - 120 / 2, 100);
            m_oMusicLabel.IsHidden = true;

            //Options - Music On Checkbox
            m_oEnableMusic = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_oEnableMusic.Text = "On";
            m_oEnableMusic.SetSize(56, 14);
            m_oEnableMusic.SetPosition(menuWindow.Width / 2 - 120 / 2, 118);
            m_oEnableMusic.IsHidden = true;
            m_oEnableMusic.Checked += m_oEnableMusic_Checked;

            //Options - Music Off Checkbox
            m_oDisableMusic = new Gwen.Control.LabeledCheckBox(menuWindow);
            m_oDisableMusic.Text = "Off";
            m_oDisableMusic.SetSize(56, 14);
            m_oDisableMusic.SetPosition(menuWindow.Width / 2 + 4, 118);
            m_oDisableMusic.IsHidden = true;
            m_oDisableMusic.Checked += m_oDisableMusic_Checked;

            //Options - Apply Button
            m_oApplyBtn = new Gwen.Control.Button(menuWindow);
            m_oApplyBtn.SetText("Apply");
            m_oApplyBtn.SetPosition(menuWindow.Width / 2 - 120 / 2, 136);
            m_oApplyBtn.SetSize(56, 32);
            m_oApplyBtn.IsHidden = true;
            m_oApplyBtn.Clicked += m_oApplyBtn_Clicked;

            //Options - Back Button
            m_oBackBtn = new Gwen.Control.Button(menuWindow);
            m_oBackBtn.SetText("Back");
            m_oBackBtn.SetPosition(menuWindow.Width / 2 + 4, 136);
            m_oBackBtn.SetSize(56, 32);
            m_oBackBtn.IsHidden = true;
            m_oBackBtn.Clicked += m_lBackBtn_Clicked; //Sharing the Login Back button function, they both open the main menu.

        }

        //Register Menu
        static void m_rUsernameTextbox_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryRegister();
        }
        static void m_rEmailTextbox_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryRegister();
        }
        static void m_rPasswordTextbox_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryRegister();
        }
        static void m_rPasswordTextbox2_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryRegister();
        } 
        static void m_rRegisterBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            tryRegister();
        }
        static void tryRegister()
        {
            if (Globals.WaitingOnServer) { return; }
            if (isValidName(m_rUsernameTextbox.Text))
            {
                if (m_rPasswordTextbox.Text == m_rPasswordTextbox2.Text)
                {
                    if (isValidPass(m_rPasswordTextbox.Text))
                    {
                        if (IsEmail(m_rEmailTextbox.Text))
                        {
                            Graphics.fadeStage = 2;
                            PacketSender.SendCreateAccount(m_rUsernameTextbox.Text, m_rPasswordTextbox.Text, m_rEmailTextbox.Text);
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
        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }
        public static bool isValidName(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), UsernamePattern);
            else return false;
        }
        public static bool isValidPass(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), PasswordPattern);
            else return false;
        }

        //Options Menu
        static void m_oDisableMusic_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            m_oEnableMusic.IsChecked = false;
        }
        static void m_oEnableMusic_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            m_oDisableMusic.IsChecked = false;
        }
        static void m_oDisableSound_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            m_oEnableSound.IsChecked = false;
        }
        static void m_oEnableSound_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            m_oDisableSound.IsChecked = false;
        }
        static void m_oApplyBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            MenuItem mi = m_oResolutionList.SelectedItem;
            List<VideoMode> myModes = Graphics.GetValidVideoModes();
            for (int i = 0; i < myModes.Count(); i++)
            {
                if (mi.Text == myModes[i].Width + "x" + myModes[i].Height)
                {
                    Graphics.DisplayMode = i;
                    Graphics.MustReInit = true;
                }
            }
            if (Graphics.FullScreen != m_oFullscreen.IsChecked)
            {
                Graphics.FullScreen = m_oFullscreen.IsChecked;
                Graphics.MustReInit = true;
            }
            Globals.MusicEnabled = m_oEnableMusic.IsChecked;
            Globals.SoundEnabled = m_oEnableSound.IsChecked;
            Database.SaveOptions();
        }

        //Main Menu
        static void m_loginButton_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            menuWindow.Title = "Login";
            m_registerButton.Hide();
            m_exitButton.Hide();
            m_loginButton.Hide();
            m_optionsButton.Hide();
            m_lUsernameLabel.Show();
            m_lUsernameTextbox.Show();
            m_lPasswordLabel.Show();
            m_lPasswordTextbox.Show();
            m_lSavePassChk.Show();
            m_lLoginBtn.Show();
            m_lBackBtn.Show();
        }
        static void m_registerButton_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            menuWindow.Title = "Register";
            m_registerButton.Hide();
            m_exitButton.Hide();
            m_loginButton.Hide();
            m_optionsButton.Hide();
            m_rUsernameLabel.Show();
            m_rUsernameTextbox.Show();
            m_rEmailLabel.Show();
            m_rEmailTextbox.Show();
            m_rPasswordLabel.Show();
            m_rPasswordTextbox.Show();
            m_rPasswordLabel2.Show();
            m_rPasswordTextbox2.Show();
            m_rRegisterBtn.Show();
            m_rBackBtn.Show();
        }
        static void m_optionsButton_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            menuWindow.Title = "Options";
            m_registerButton.Hide();
            m_exitButton.Hide();
            m_loginButton.Hide();
            m_optionsButton.Hide();
            m_oResolutionLabel.Show();
            m_oResolutionList.Show();
            //Select current resolution
            m_oResolutionList.SelectByText(Graphics.GetValidVideoModes()[Graphics.DisplayMode].Width + "x" + Graphics.GetValidVideoModes()[Graphics.DisplayMode].Height);
            m_oFullscreen.Show();
            m_oFullscreen.IsChecked = Graphics.FullScreen;
            m_oSoundLabel.Show();
            m_oEnableSound.Show();
            m_oEnableSound.IsChecked = Globals.SoundEnabled;
            m_oDisableSound.Show();
            m_oDisableSound.IsChecked = !Globals.SoundEnabled;
            m_oMusicLabel.Show();
            m_oEnableMusic.Show();
            m_oEnableMusic.IsChecked = Globals.MusicEnabled;
            m_oDisableMusic.Show();
            m_oDisableMusic.IsChecked = !Globals.MusicEnabled;
            m_oApplyBtn.Show();
            m_oBackBtn.Show();
        }
        static void m_exitButton_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            GameMain.isRunning = false;
        }

        //Login Menu
        static void m_lBackBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            //Show Menu Elements
            menuWindow.Title = "Main Menu";
            m_registerButton.Show();
            m_exitButton.Show();
            m_loginButton.Show();
            m_optionsButton.Show();

            //Hide Login Elements
            m_lUsernameLabel.Hide();
            m_lUsernameTextbox.Hide();
            m_lPasswordLabel.Hide();
            m_lPasswordTextbox.Hide();
            m_lSavePassChk.Hide();
            m_lLoginBtn.Hide();
            m_lBackBtn.Hide();

            //Hide Register Elements
            m_rUsernameLabel.Hide();
            m_rUsernameTextbox.Hide();
            m_rEmailLabel.Hide();
            m_rEmailTextbox.Hide();
            m_rPasswordLabel.Hide();
            m_rPasswordTextbox.Hide();
            m_rPasswordLabel2.Hide();
            m_rPasswordTextbox2.Hide();
            m_rRegisterBtn.Hide();
            m_rBackBtn.Hide();

            //Hide option elements
            m_oResolutionLabel.Hide();
            m_oResolutionList.Hide();
            m_oFullscreen.Hide();
            m_oSoundLabel.Hide();
            m_oEnableSound.Hide();
            m_oDisableSound.Hide();
            m_oMusicLabel.Hide();
            m_oEnableMusic.Hide();
            m_oDisableMusic.Hide();
            m_oApplyBtn.Hide();
            m_oBackBtn.Hide();
        }
        static void m_lUsernameTextbox_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryLogin();
        }
        static void m_lPasswordTextbox_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            tryLogin();
        }
        static void m_lLoginBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            tryLogin();
        }
        static void tryLogin()
        {
            if (Globals.WaitingOnServer) { return; }
            if (isValidName(m_lUsernameTextbox.Text))
            {
                if (isValidPass(m_lPasswordTextbox.Text))
                {
                    Graphics.fadeStage = 2;
                    PacketSender.SendLogin(m_lUsernameTextbox.Text, m_lPasswordTextbox.Text);
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
        static void msgbox_Resized(Gwen.Control.Base sender, EventArgs arguments)
        {
            sender.SetPosition(Graphics.ScreenWidth / 2 - sender.Width / 2, Graphics.ScreenHeight / 2 - sender.Height / 2);
        }
        public static void AddError(string error)
        {
            msgboxErrors.Add(error);
        }
        #endregion

        #region "Game GUI"
        public static void InitGameGUI()
        {
            //Event Dialog Window
            g_eventDialogWindow = new Gwen.Control.WindowControl(gameCanvas, "Event Dialog", false);
            g_eventDialogWindow.SetSize(200, 260);
            g_eventDialogWindow.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 260/2);
            g_eventDialogWindow.IsClosable = false;
            g_eventDialogWindow.DisableResizing();
            g_eventDialogWindow.Margin = Gwen.Margin.Zero;
            g_eventDialogWindow.Padding = Padding.Zero;
            g_eventDialogWindow.IsHidden = true;

            g_eventDialog = new Gwen.Control.ListBox(g_eventDialogWindow);
            string[] myText = WrapText("This is a really long string of text that I really, really want to get working with my text box, please wish me luck and hopefully in 10 minutes I will have something decent.", 180);
            for (int i = 0; i < myText.Length; i++)
            {
                ListBoxRow rw = g_eventDialog.AddRow(myText[i]);
                rw.MouseInputEnabled = false;
            }
            g_eventDialog.IsDisabled = true;
            g_eventDialog.SetPosition(6, 6);
            g_eventDialog.SetSize(188, 80);
            g_eventDialog.ShouldDrawBackground = false;
            

            g_eventResponse1 = new Gwen.Control.Button(g_eventDialogWindow);
            g_eventResponse1.SetSize(120, 32);
            g_eventResponse1.SetPosition(g_eventDialogWindow.Width / 2 - 120 / 2, 94);
            g_eventResponse1.SetText("Response 1");
            g_eventResponse1.Clicked += g_eventResponse1_Clicked;

            g_eventResponse2 = new Gwen.Control.Button(g_eventDialogWindow);
            g_eventResponse2.SetSize(120, 32);
            g_eventResponse2.SetPosition(g_eventDialogWindow.Width / 2 - 120 / 2, 130);
            g_eventResponse2.SetText("Response 2");
            g_eventResponse2.Clicked += g_eventResponse2_Clicked;

            g_eventResponse3 = new Gwen.Control.Button(g_eventDialogWindow);
            g_eventResponse3.SetSize(120, 32);
            g_eventResponse3.SetPosition(g_eventDialogWindow.Width / 2 - 120 / 2, 164);
            g_eventResponse3.SetText("Response 3");
            g_eventResponse3.Clicked += g_eventResponse3_Clicked;

            g_eventResponse4 = new Gwen.Control.Button(g_eventDialogWindow);
            g_eventResponse4.SetSize(120, 32);
            g_eventResponse4.SetPosition(g_eventDialogWindow.Width / 2 - 120 / 2, 198);
            g_eventResponse4.SetText("Response 4");
            g_eventResponse4.Clicked += g_eventResponse4_Clicked;

            //Chatbox Window
            g_ChatboxWindow = new Gwen.Control.WindowControl(gameCanvas, "Chatbox", false);
            g_ChatboxWindow.IsClosable = false;
            g_ChatboxWindow.SetSize(380, 140);
            g_ChatboxWindow.DisableResizing();
            g_ChatboxWindow.SetPosition(0, Graphics.ScreenHeight - 140);
            g_ChatboxWindow.Margin = Gwen.Margin.Zero;
            g_ChatboxWindow.Padding = Padding.Zero;

            g_ChatBoxMessages = new Gwen.Control.ListBox(g_ChatboxWindow);
            g_ChatBoxMessages.IsDisabled = true;
            g_ChatBoxMessages.SetPosition(2, 1);
            g_ChatBoxMessages.SetSize(376, 90);
            g_ChatBoxMessages.ShouldDrawBackground = false;

            g_ChatBoxInput = new Gwen.Control.TextBox(g_ChatboxWindow);
            g_ChatBoxInput.SetPosition(2, 140 - 18 - 24);
            g_ChatBoxInput.SetSize(338, 16);
            g_ChatBoxInput.SubmitPressed += g_ChatBoxInput_SubmitPressed;
            g_ChatBoxInput.Text = "Press enter to chat.";
            g_ChatBoxInput.Clicked += g_ChatBoxInput_Clicked;
            focusElements.Add(g_ChatBoxInput);

            g_ChatBoxSendBtn = new Gwen.Control.Button(g_ChatboxWindow);
            g_ChatBoxSendBtn.SetSize(36, 16);
            g_ChatBoxSendBtn.SetPosition(342, 140 - 18 - 24);
            g_ChatBoxSendBtn.Text = "Send";
            g_ChatBoxSendBtn.Clicked += g_ChatBoxSendBtn_Clicked;

            //Game Menu Window
            g_GameMenu = new Gwen.Control.WindowControl(gameCanvas, "Game Menu");
            g_GameMenu.IsClosable = false;
            g_GameMenu.DisableResizing();
            g_GameMenu.SetSize(166, 84);
            g_GameMenu.SetPosition(Graphics.ScreenWidth - 116, Graphics.ScreenHeight - 84);
            g_GameMenu.Margin = Gwen.Margin.Zero;
            g_GameMenu.Padding = Padding.Zero;

            g_InventoryBtn = new Gwen.Control.Button(g_GameMenu);
            g_InventoryBtn.SetSize(50, 24);
            g_InventoryBtn.SetText("Inventory");
            g_InventoryBtn.SetPosition(4, 4);

            g_SkillsBtn = new Gwen.Control.Button(g_GameMenu);
            g_SkillsBtn.SetSize(50, 24);
            g_SkillsBtn.SetText("Skills");
            g_SkillsBtn.SetPosition(58, 4);

            g_CharacterBtn = new Gwen.Control.Button(g_GameMenu);
            g_CharacterBtn.SetSize(50, 24);
            g_CharacterBtn.SetText("Character");
            g_CharacterBtn.SetPosition(112, 4);

            g_QuestsBtn = new Gwen.Control.Button(g_GameMenu);
            g_QuestsBtn.SetSize(50, 24);
            g_QuestsBtn.SetText("Missions");
            g_QuestsBtn.SetPosition(4, 32);

            g_OptionBtn = new Gwen.Control.Button(g_GameMenu);
            g_OptionBtn.SetSize(50, 24);
            g_OptionBtn.SetText("Options");
            g_OptionBtn.SetPosition(58, 32);
            g_OptionBtn.Clicked += g_OptionBtn_Clicked;

            g_CloseBtn = new Gwen.Control.Button(g_GameMenu);
            g_CloseBtn.SetSize(50, 24);
            g_CloseBtn.SetText("Exit");
            g_CloseBtn.SetPosition(112, 32);
            g_CloseBtn.Clicked += g_CloseBtn_Clicked;

            //Options Window
            g_oMenu = new Gwen.Control.WindowControl(gameCanvas, "Options");
            g_oMenu.SetSize(200, 200);
            g_oMenu.SetPosition(Graphics.ScreenWidth / 2 - 100, Graphics.ScreenHeight / 2 - 80);
            g_oMenu.DisableResizing();
            g_oMenu.Margin = Gwen.Margin.Zero;
            g_oMenu.Padding = Padding.Zero;
            g_oMenu.IsHidden = true;

            g_oResolutionList = new Gwen.Control.ComboBox(g_oMenu);
            List<VideoMode> myModes = Graphics.GetValidVideoModes();
            for (int i = 0; i < myModes.Count(); i++)
            {
                g_oResolutionList.AddItem(myModes[i].Width + "x" + myModes[i].Height);
            }
            g_oResolutionList.SetSize(120, 14);
            g_oResolutionList.SetPosition(g_oMenu.Width / 2 - 120 / 2, 28);

            //Options - Fullscreen Checkbox
            g_oFullscreen = new Gwen.Control.LabeledCheckBox(g_oMenu);
            g_oFullscreen.Text = "Fullscreen";
            g_oFullscreen.SetSize(120, 14);
            g_oFullscreen.SetPosition(g_oMenu.Width / 2 - 120 / 2, 46);

            //Options - Sound Label
            g_oSoundLabel = new Gwen.Control.Label(g_oMenu);
            g_oSoundLabel.SetText("Sound:");
            g_oSoundLabel.SetPosition(g_oMenu.Width / 2 - 120 / 2, 64);

            //Options - Sound On Checkbox
            g_oEnableSound = new Gwen.Control.LabeledCheckBox(g_oMenu);
            g_oEnableSound.Text = "On";
            g_oEnableSound.SetSize(56, 14);
            g_oEnableSound.SetPosition(g_oMenu.Width / 2 - 120 / 2, 82);
            g_oEnableSound.Checked += g_oEnableSound_Checked;

            //Options - Sound Off Checkbox
            g_oDisableSound = new Gwen.Control.LabeledCheckBox(g_oMenu);
            g_oDisableSound.Text = "Off";
            g_oDisableSound.SetSize(56, 14);
            g_oDisableSound.SetPosition(g_oMenu.Width / 2 + 4, 82);
            g_oDisableSound.Checked += g_oDisableSound_Checked;

            //Options - Music Label
            g_oMusicLabel = new Gwen.Control.Label(g_oMenu);
            g_oMusicLabel.SetText("Music:");
            g_oMusicLabel.SetPosition(g_oMenu.Width / 2 - 120 / 2, 100);

            //Options - Music On Checkbox
            g_oEnableMusic = new Gwen.Control.LabeledCheckBox(g_oMenu);
            g_oEnableMusic.Text = "On";
            g_oEnableMusic.SetSize(56, 14);
            g_oEnableMusic.SetPosition(g_oMenu.Width / 2 - 120 / 2, 118);
            g_oEnableMusic.Checked += g_oEnableMusic_Checked;

            //Options - Music Off Checkbox
            g_oDisableMusic = new Gwen.Control.LabeledCheckBox(g_oMenu);
            g_oDisableMusic.Text = "Off";
            g_oDisableMusic.SetSize(56, 14);
            g_oDisableMusic.SetPosition(g_oMenu.Width / 2 + 4, 118);
            g_oDisableMusic.Checked += g_oDisableMusic_Checked;

            //Options - Apply Button
            g_oApplyBtn = new Gwen.Control.Button(g_oMenu);
            g_oApplyBtn.SetText("Apply");
            g_oApplyBtn.SetPosition(g_oMenu.Width / 2 - 120 / 2, 136);
            g_oApplyBtn.SetSize(120, 32);
            g_oApplyBtn.Clicked += g_oApplyBtn_Clicked;



        }

        //Options Menu
        static void g_oDisableMusic_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            g_oEnableMusic.IsChecked = false;
        }
        static void g_oEnableMusic_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            g_oDisableMusic.IsChecked = false;
        }
        static void g_oDisableSound_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            g_oEnableSound.IsChecked = false;
        }
        static void g_oEnableSound_Checked(Gwen.Control.Base sender, EventArgs arguments)
        {
            g_oDisableSound.IsChecked = false;
        }
        static void g_oApplyBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            MenuItem mi = g_oResolutionList.SelectedItem;
            List<VideoMode> myModes = Graphics.GetValidVideoModes();
            for (int i = 0; i < myModes.Count(); i++)
            {
                if (mi.Text == myModes[i].Width + "x" + myModes[i].Height)
                {
                    Graphics.DisplayMode = i;
                    Graphics.MustReInit = true;
                }
            }
            if (Graphics.FullScreen != g_oFullscreen.IsChecked)
            {
                Graphics.FullScreen = g_oFullscreen.IsChecked;
                Graphics.MustReInit = true;
            }
            Globals.MusicEnabled = g_oEnableMusic.IsChecked;
            Globals.SoundEnabled = g_oEnableSound.IsChecked;
            Database.SaveOptions();
        }

        //Game Menu
        static void g_CloseBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            GameMain.isRunning = false;
        }
        static void g_OptionBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            //Select current resolution
            g_oResolutionList.SelectByText(Graphics.GetValidVideoModes()[Graphics.DisplayMode].Width + "x" + Graphics.GetValidVideoModes()[Graphics.DisplayMode].Height);
            g_oFullscreen.IsChecked = Graphics.FullScreen;
            g_oEnableSound.IsChecked = Globals.SoundEnabled;
            g_oDisableSound.IsChecked = !Globals.SoundEnabled;
            g_oEnableMusic.IsChecked = Globals.MusicEnabled;
            g_oDisableMusic.IsChecked = !Globals.MusicEnabled;
            g_oMenu.IsHidden = false;
        }

        //Chatbox Window
        static void g_ChatBoxInput_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {

            if (g_ChatBoxInput.Text == "Press enter to chat.") { g_ChatBoxInput.Text = ""; }
        }
        static void g_ChatBoxInput_SubmitPressed(Gwen.Control.Base sender, EventArgs arguments)
        {
            if (g_ChatBoxInput.Text.Trim().Length > 0 && g_ChatBoxInput.Text != "Press enter to chat.")
            {
                PacketSender.SendChatMsg(g_ChatBoxInput.Text.Trim());
                g_ChatBoxInput.Text = "Press enter to chat.";
            }
        }
        static void g_ChatBoxSendBtn_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            if (g_ChatBoxInput.Text.Trim().Length > 0 && g_ChatBoxInput.Text != "Press enter to chat.")
            {
                PacketSender.SendChatMsg(g_ChatBoxInput.Text.Trim());
                g_ChatBoxInput.Text = "Press enter to chat.";
            }
        }

        //Event Dialog Window
        static void g_eventResponse4_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            EventDialog ed = Globals.EventDialogs[0];
            if (ed.responseSent == 0)
            {
                PacketSender.SendEventResponse(4, ed);
                ed.responseSent = 1;
            }
        }
        static void g_eventResponse3_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            EventDialog ed = Globals.EventDialogs[0];
            if (ed.responseSent == 0)
            {
                PacketSender.SendEventResponse(3, ed);
                ed.responseSent = 1;
            }
        }
        static void g_eventResponse2_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            EventDialog ed = Globals.EventDialogs[0];
            if (ed.responseSent == 0)
            {
                PacketSender.SendEventResponse(2, ed);
                ed.responseSent = 1;
            }
        }
        static void g_eventResponse1_Clicked(Gwen.Control.Base sender, ClickedEventArgs arguments)
        {
            EventDialog ed = Globals.EventDialogs[0];
            if (ed.responseSent == 0)
            {
                PacketSender.SendEventResponse(1, ed);
                ed.responseSent = 1;
            }
        }

        
        #endregion




        
        #region "GUI Functions"
        //Actual Drawing Function
        public static void DrawGUI()
        {
            while (msgboxErrors.Count > 0)
            {
                Gwen.Control.MessageBox msgbox = new Gwen.Control.MessageBox(menuCanvas, msgboxErrors[0], "Error!");
                msgbox.Resized += msgbox_Resized;
                msgboxErrors.RemoveAt(0);
                m_lBackBtn_Clicked(null, null);
            }
            if (Globals.GameState == 0)
            {
                menuCanvas.RenderCanvas();
            }
            else
            {
                if (Globals.EventDialogs.Count > 0)
                {
                    if (g_eventDialogWindow.IsHidden == true)
                    {
                        g_eventDialogWindow.Show();
                        g_eventDialog.Clear();
                        string[] myText = WrapText(Globals.EventDialogs[0].prompt, 180);
                        for (int i = 0; i < myText.Length; i++)
                        {
                            ListBoxRow rw = g_eventDialog.AddRow(myText[i]);
                            rw.MouseInputEnabled = false;
                        }
                        if (Globals.EventDialogs[0].opt1.Length > 0 || Globals.EventDialogs[0].opt2.Length > 0 || Globals.EventDialogs[0].opt3.Length > 0 || Globals.EventDialogs[0].opt4.Length > 0)
                        {
                            if (Globals.EventDialogs[0].opt1 != "")
                            {
                                g_eventResponse1.Show();
                                g_eventResponse1.SetText(Globals.EventDialogs[0].opt1);
                            }
                            else
                            {
                                g_eventResponse1.Hide();
                            }
                            if (Globals.EventDialogs[0].opt2 != "")
                            {
                                g_eventResponse2.Show();
                                g_eventResponse2.SetText(Globals.EventDialogs[0].opt2);
                            }
                            else
                            {
                                g_eventResponse2.Hide();
                            }
                            if (Globals.EventDialogs[0].opt3 != "")
                            {
                                g_eventResponse3.Show();
                                g_eventResponse3.SetText(Globals.EventDialogs[0].opt3);
                            }
                            else
                            {
                                g_eventResponse3.Hide();
                            }
                            if (Globals.EventDialogs[0].opt4 != "")
                            {
                                g_eventResponse4.Show();
                                g_eventResponse4.SetText(Globals.EventDialogs[0].opt4);
                            }
                            else
                            {
                                g_eventResponse4.Hide();
                            }
                        }
                        else
                        {
                            g_eventResponse1.Show();
                            g_eventResponse1.SetText("Continue");
                            g_eventResponse2.Hide();
                            g_eventResponse3.Hide();
                            g_eventResponse4.Hide();
                        }


                    }
                }
                if (Globals.ChatboxContent.Count > 0)
                {
                    for (int i = 0; i < Globals.ChatboxContent.Count; i++)
                    {
                        string[] myText = WrapText((string)Globals.ChatboxContent[i], 360);
                        for (int x = 0; x < myText.Length; x++)
                        {
                            ListBoxRow rw = g_ChatBoxMessages.AddRow(myText[x]);
                            rw.MouseInputEnabled = false;
                        }
                    }

                    Globals.ChatboxContent.Clear();
                    g_ChatBoxMessages.Redraw();
                    g_ChatBoxMessages.Think();
                    g_ChatBoxMessages.ScrollToBottom();

                }
                if (focusChat)
                {
                    g_ChatBoxInput.Focus();
                    g_ChatBoxInput.Text = "";
                    focusChat = false;
                }
                gameCanvas.RenderCanvas();
            }
        }
        private static string[] WrapText(string input, int width)
        {
            List<string> myOutput = new List<string>();
            int lastSpace = 0;
            int curPos = 0;
            int curLen = 1;
            Text myText = new Text(input.Substring(curPos, curLen), Graphics.GameFont);
            myText.CharacterSize = 10;
            while (curPos + curLen < input.Length)
            {
                if (myText.GetLocalBounds().Width < width)
                {
                    if (input[curPos + curLen] == ' ' || input[curPos + curLen] == '-')
                    {
                        lastSpace = curLen;
                    }
                    else if (input[curPos + curLen] == '\n')
                    {
                        myOutput.Add(input.Substring(curPos, curLen));
                        curPos = curPos + curLen;
                        curLen = 1;
                    }
                }
                else
                {
                    myOutput.Add(input.Substring(curPos, lastSpace));
                    curPos = curPos + lastSpace;
                    curLen = 1;
                }
                curLen++;
                myText = new Text(input.Substring(curPos, curLen), Graphics.GameFont);
                myText.CharacterSize = 10;
            }
            myOutput.Add(input.Substring(curPos, input.Length - curPos));
            return myOutput.ToArray();
        }
        public static bool HasInputFocus()
        {
            for (int i = 0; i < focusElements.Count(); i++)
            {
                if (focusElements[i].HasFocus)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
