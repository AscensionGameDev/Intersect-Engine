using System.Collections.Generic;
using Intersect;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
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
        private ImagePanel _logoPanel;
        private Label _menuHeader;
        private ImagePanel _menuPanel;
        private Button _optionsButton;

        private OptionsWindow _optionsWindow;
        private Button _registerButton;
        private RegisterWindow _registerWindow;

		private bool _shouldOpenCharacterSelection;
		private bool _shouldOpenCharacterCreation;

        //Character creation feild check
        private bool HasMadeCharacterCreation;
        //Controls
        private Canvas MenuCanvas;

        //Init
        public MainMenu(Canvas _menuCanvas)
        {
            MenuCanvas = _menuCanvas;

            _logoPanel = new ImagePanel(_menuCanvas)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                    Globals.Database.Logo)
            };

            //Main Menu Window
            _menuPanel = new ImagePanel(_menuCanvas)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png")
            };
            _menuPanel.SetSize(512, 393);
            _menuPanel.SetPosition(_menuCanvas.Width / 2 - _menuPanel.Width / 2,
                _menuCanvas.Height / 2 - _menuPanel.Height / 2);

            //Menu Header
            _menuHeader = new Label(_menuPanel)
            {
                AutoSizeToContents = false
            };
            _menuHeader.SetText(Strings.Get("mainmenu", "title"));
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 24);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

            if (_logoPanel.Texture != null)
            {
                _logoPanel.SetSize(_logoPanel.Texture.GetWidth(), _logoPanel.Texture.GetHeight());
                _logoPanel.SetPosition(_menuCanvas.Width / 2 - _logoPanel.Width / 2, 0);
            }

            if (_logoPanel.Bottom > _menuPanel.Y + 20)
            {
                _menuPanel.Y = _logoPanel.Bottom + 20;
            }

            if (_menuPanel.Bottom > _menuCanvas.Height)
            {
                var diff = _menuPanel.Bottom - _menuCanvas.Height;
                _logoPanel.SetPosition(_logoPanel.X, _logoPanel.Y - diff);
                _menuPanel.SetPosition(_menuPanel.X, _menuPanel.Y - diff);
            }

            if (_logoPanel.Y < 0)
            {
                _logoPanel.SetPosition(_logoPanel.X, 0);
            }

            //Login Button
            _loginButton = new Button(_menuPanel);
            _loginButton.SetText(Strings.Get("mainmenu", "login"));
            _loginButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _loginButton.SetSize(211, 61);
            _loginButton.SetPosition(_menuPanel.Width / 2 - _loginButton.Width / 2, 60);
            _loginButton.Clicked += LoginButton_Clicked;
            _loginButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _loginButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _loginButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _loginButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _loginButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _loginButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            //Register Button
            _registerButton = new Button(_menuPanel);
            _registerButton.SetText(Strings.Get("mainmenu", "register"));
            _registerButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _registerButton.SetSize(211, 61);
            _registerButton.SetPosition(_menuPanel.Width / 2 - _registerButton.Width / 2, 130);
            _registerButton.Clicked += RegisterButton_Clicked;
            _registerButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _registerButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _registerButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _registerButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _registerButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _registerButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            //Exit Button
            _exitButton = new Button(_menuPanel);
            _exitButton.SetText(Strings.Get("mainmenu", "exit"));
            _exitButton.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            _exitButton.SetSize(211, 61);
            _exitButton.SetPosition(_menuPanel.Width / 2 - _exitButton.Width / 2, 200);
            _exitButton.Clicked += ExitButton_Clicked;
            _exitButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _exitButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _exitButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _exitButton.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _exitButton.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _exitButton.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);

            //Options Button
            _optionsButton = new Button(_menuCanvas);
            _optionsButton.Clicked += OptionsButton_Clicked;
            _optionsButton.SetText("");
            if (!string.IsNullOrEmpty(Strings.Get("mainmenu", "options")))
                _optionsButton.SetToolTipText(Strings.Get("mainmenu", "options"));
            _optionsButton.SetSize(48, 49);
            _optionsButton.SetPosition(_menuCanvas.Width - 50, 2);
            _optionsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "settingsnormal.png"),
                Button.ControlState.Normal);
            _optionsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "settingshover.png"),
                Button.ControlState.Hovered);
            _optionsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "settingclicked.png"),
                Button.ControlState.Clicked);

            //Options Controls
            _optionsWindow = new OptionsWindow(_menuCanvas, this, _menuPanel);

            //Login Controls
            _loginWindow = new LoginWindow(_menuCanvas, this, _menuPanel);

            //Register Controls
            _registerWindow = new RegisterWindow(_menuCanvas, this, _menuPanel);
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
            if (_createCharacterWindow != null) _createCharacterWindow.Hide();
			if (_selectCharacterWindow != null) _selectCharacterWindow.Hide();
			_menuPanel.Show();
            _optionsButton.Show();
        }

        public void Show()
        {
            _menuPanel.IsHidden = false;
            _optionsButton.IsHidden = false;
        }

        public void Hide()
        {
            _menuPanel.IsHidden = true;
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
			_selectCharacterWindow = new SelectCharacterWindow(MenuCanvas, this, _menuPanel);
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
            _createCharacterWindow = new CreateCharacterWindow(MenuCanvas, this, _menuPanel);
            _createCharacterWindow.Show();
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