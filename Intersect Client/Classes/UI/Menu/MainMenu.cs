using System.Collections.Generic;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Menu
{
    public class MainMenu
    {
        private CreateCharacterWindow mCreateCharacterWindow;
        private Button mCreditsButton;
        private CreditsWindow mCreditsWindow;
        private Button mExitButton;

        private Button mLoginButton;
        private LoginWindow mLoginWindow;
        private Label mMenuHeader;
        private ImagePanel mMenuWindow;
        private Button mOptionsButton;

        private OptionsWindow mOptionsWindow;
        private Button mRegisterButton;
        private RegisterWindow mRegisterWindow;
        private SelectCharacterWindow mSelectCharacterWindow;
        private bool mShouldOpenCharacterCreation;
        private bool mShouldOpenCharacterSelection;

        //Character creation feild check
        private bool mHasMadeCharacterCreation;

        //Controls
        private Canvas mMenuCanvas;

        //Init
        public MainMenu(Canvas menuCanvas)
        {
            mMenuCanvas = menuCanvas;

            new ImagePanel(menuCanvas, "Logo");

            //Main Menu Window
            mMenuWindow = new ImagePanel(menuCanvas, "MenuWindow");

            //Menu Header
            mMenuHeader = new Label(mMenuWindow, "Title");
            mMenuHeader.SetText(Strings.MainMenu.title);

            //Login Button
            mLoginButton = new Button(mMenuWindow, "LoginButton");
            mLoginButton.SetText(Strings.MainMenu.login);
            mLoginButton.Clicked += LoginButton_Clicked;

            //Register Button
            mRegisterButton = new Button(mMenuWindow, "RegisterButton");
            mRegisterButton.SetText(Strings.MainMenu.register);
            mRegisterButton.Clicked += RegisterButton_Clicked;

            //Credits Button
            mCreditsButton = new Button(mMenuWindow, "CreditsButton");
            mCreditsButton.SetText(Strings.MainMenu.credits);
            mCreditsButton.Clicked += CreditsButton_Clicked;

            //Exit Button
            mExitButton = new Button(mMenuWindow, "ExitButton");
            mExitButton.SetText(Strings.MainMenu.exit);
            mExitButton.Clicked += ExitButton_Clicked;

            //Options Button
            mOptionsButton = new Button(mMenuWindow, "OptionsButton");
            mOptionsButton.Clicked += OptionsButton_Clicked;
            mOptionsButton.SetText(Strings.MainMenu.options);
            if (!string.IsNullOrEmpty(Strings.MainMenu.options))
                mOptionsButton.SetToolTipText(Strings.MainMenu.options);

            //Options Controls
            mOptionsWindow = new OptionsWindow(menuCanvas, this, mMenuWindow);
            //Login Controls
            mLoginWindow = new LoginWindow(menuCanvas, this, mMenuWindow);
            //Register Controls
            mRegisterWindow = new RegisterWindow(menuCanvas, this, mMenuWindow);
            //Character Creation Controls
            mCreateCharacterWindow = new CreateCharacterWindow(mMenuCanvas, this, mMenuWindow);
            //Character Selection Controls
            mSelectCharacterWindow = new SelectCharacterWindow(mMenuCanvas, this, mMenuWindow);
            //Credits Controls
            mCreditsWindow = new CreditsWindow(mMenuCanvas, this);
        }

        //Methods
        public void Update()
        {
            if (mShouldOpenCharacterSelection)
            {
                CreateCharacterSelection();
            }
            if (mShouldOpenCharacterCreation)
            {
                CreateCharacterCreation();
            }
            mOptionsWindow.Update();
        }

        public void Reset()
        {
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mOptionsWindow.Hide();
            mCreditsWindow.Hide();
            if (mCreateCharacterWindow != null) mCreateCharacterWindow.Hide();
            if (mSelectCharacterWindow != null) mSelectCharacterWindow.Hide();
            mMenuWindow.Show();
            mOptionsButton.Show();
        }

        public void Show()
        {
            mMenuWindow.IsHidden = false;
            mOptionsButton.IsHidden = false;
        }

        public void Hide()
        {
            mMenuWindow.IsHidden = true;
            mOptionsButton.IsHidden = true;
        }

        public void NotifyOpenCharacterSelection(List<Character> characters)
        {
            mShouldOpenCharacterSelection = true;
            mSelectCharacterWindow.Characters = characters;
        }

        public void CreateCharacterSelection()
        {
            Hide();
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mOptionsWindow.Hide();
            mCreateCharacterWindow.Hide();
            mSelectCharacterWindow.Show();
            mShouldOpenCharacterSelection = false;
        }

        public void NotifyOpenCharacterCreation()
        {
            mShouldOpenCharacterCreation = true;
        }

        public void CreateCharacterCreation()
        {
            Hide();
            mLoginWindow.Hide();
            mRegisterWindow.Hide();
            mOptionsWindow.Hide();
            mSelectCharacterWindow.Hide();
            mCreateCharacterWindow.Show();
            mCreateCharacterWindow.Init();
            mHasMadeCharacterCreation = true;
            mShouldOpenCharacterCreation = false;
        }

        //Input Handlers
        void LoginButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mLoginWindow.Show();
        }

        void RegisterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mRegisterWindow.Show();
        }

        void CreditsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mCreditsWindow.Show();
        }

        void OptionsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            mOptionsWindow.Show();
        }

        void ExitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }
    }
}