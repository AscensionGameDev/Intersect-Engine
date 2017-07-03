using System.Collections.Generic;
using Intersect;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.UI.Menu
{
    public class MainMenu
    {
		private SelectCharacterWindow _selectCharacterWindow;
        private CreateCharacterWindow _createCharacterWindow;
        private Button _exitButton;

        private Button _loginButton;
        private LoginWindow _loginWindow;
        private Label _menuHeader;
        private ImagePanel _menuWindow;
        private Button _optionsButton;

        private OptionsWindow _optionsWindow;
        private Button _registerButton;
        private RegisterWindow _registerWindow;
		    private bool _shouldOpenCharacterSelection;
		    private bool _shouldOpenCharacterCreation;
        private Button _creditsButton;
        private CreditsWindow _creditsWindow;

        //Character creation feild check
        private bool HasMadeCharacterCreation;
        //Controls
        private Canvas MenuCanvas;

        //Init
        public MainMenu(Canvas _menuCanvas)
        {
            MenuCanvas = _menuCanvas;

            new ImagePanel(_menuCanvas, "Logo");

            //Main Menu Window
            _menuWindow = new ImagePanel(_menuCanvas, "MenuWindow");
            
            //Menu Header
            _menuHeader = new Label(_menuWindow, "Title");
            _menuHeader.SetText(Strings.Get("mainmenu", "title"));

            //Login Button
            _loginButton = new Button(_menuWindow, "LoginButton");
            _loginButton.SetText(Strings.Get("mainmenu", "login"));
            _loginButton.Clicked += LoginButton_Clicked;

            //Register Button
            _registerButton = new Button(_menuWindow, "RegisterButton");
            _registerButton.SetText(Strings.Get("mainmenu", "register"));
            _registerButton.Clicked += RegisterButton_Clicked;

            //Credits Button
            _creditsButton = new Button(_menuWindow, "CreditsButton");
            _creditsButton.SetText(Strings.Get("mainmenu", "credits"));
            _creditsButton.Clicked += CreditsButton_Clicked;

            //Exit Button
            _exitButton = new Button(_menuWindow, "ExitButton");
            _exitButton.SetText(Strings.Get("mainmenu", "exit"));
            _exitButton.Clicked += ExitButton_Clicked;

            //Options Button
            _optionsButton = new Button(_menuWindow, "OptionsButton");
            _optionsButton.Clicked += OptionsButton_Clicked;
            _optionsButton.SetText(Strings.Get("mainmenu", "options"));
            if (!string.IsNullOrEmpty(Strings.Get("mainmenu", "options")))
                _optionsButton.SetToolTipText(Strings.Get("mainmenu", "options"));

            //Options Controls
            _optionsWindow = new OptionsWindow(_menuCanvas, this, _menuWindow);
            //Login Controls
            _loginWindow = new LoginWindow(_menuCanvas, this, _menuWindow);
            //Register Controls
            _registerWindow = new RegisterWindow(_menuCanvas, this, _menuWindow);
            //Character Creation Controls
            _createCharacterWindow = new CreateCharacterWindow(MenuCanvas, this, _menuWindow);
            //Credits Controls
            _creditsWindow = new CreditsWindow(MenuCanvas, this);
        }

        //Methods
        public void Update()
        {
			if (_shouldOpenCharacterSelection)
			{
				CreateCharacterSelection();
			}
			if (_shouldOpenCharacterCreation)
            {
                CreateCharacterCreation();
            }
            _optionsWindow.Update();
        }

        public void Reset()
        {
            _loginWindow.Hide();
            _registerWindow.Hide();
            _optionsWindow.Hide();
            _creditsWindow.Hide();
            if (_createCharacterWindow != null) _createCharacterWindow.Hide();
			      if (_selectCharacterWindow != null) _selectCharacterWindow.Hide();
            _menuWindow.Show();
            _optionsButton.Show();
        }

        public void Show()
        {
            _menuWindow.IsHidden = false;
            _optionsButton.IsHidden = false;
        }

        public void Hide()
        {
            _menuWindow.IsHidden = true;
            _optionsButton.IsHidden = true;
        }

		public void NotifyOpenCharacterSelection(List<Character> Characters)
		{
			_shouldOpenCharacterSelection = true;
			_selectCharacterWindow.Characters = Characters;
		}

		public void CreateCharacterSelection()
		{
			Hide();
			_loginWindow.Hide();
			_registerWindow.Hide();
			_optionsWindow.Hide();
			_createCharacterWindow.Hide();
			_selectCharacterWindow = new SelectCharacterWindow(MenuCanvas, this, _menuWindow);
			_selectCharacterWindow.Show();
			_shouldOpenCharacterSelection = false;
		}

		public void NotifyOpenCharacterCreation()
        {
            _shouldOpenCharacterCreation = true;
        }

        public void CreateCharacterCreation()
        {
            Hide();
            _loginWindow.Hide();
            _registerWindow.Hide();
            _optionsWindow.Hide();
            _createCharacterWindow.Show();
            _createCharacterWindow.Init();
            HasMadeCharacterCreation = true;
            _shouldOpenCharacterCreation = false;
        }

        //Input Handlers
        void LoginButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _loginWindow.Show();
        }

        void RegisterButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _registerWindow.Show();
        }

        void CreditsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _creditsWindow.Show();
        }

        void OptionsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Hide();
            _optionsWindow.Show();
        }

        void ExitButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.IsRunning = false;
        }
    }
}