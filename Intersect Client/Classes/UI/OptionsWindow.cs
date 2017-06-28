using System;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Menu;
using Intersect.Client.Classes.Core;
using System.Collections.Generic;

namespace Intersect_Client.Classes.UI
{
    public class OptionsWindow
    {
        //Parent Windows
        private bool _gameWindow = false;
        private MainMenu _mainMenu;

        //Panels
        private ScrollControl _optionsContainer;
        private ScrollControl _controlsContainer;

        //Window
        private Label _menuHeader;
        private Button _applyBtn;
        private Button _backBtn;

        private ImagePanel _fpsBackground;
        private Label _fpsLabel;
        private ComboBox _fpsList;

        private LabeledCheckBox _fullscreen;

        //Controls
        private ImagePanel _menuPanel;
        private Label _musicLabel;
        private HorizontalSlider _musicSlider;
        private int _previousMusicVolume;

        private int _previousSoundVolume;

        private ImagePanel _resolutionBackground;
        private Label _resolutionLabel;
        private ComboBox _resolutionList;
        private Label _soundLabel;
        private HorizontalSlider _soundSlider;

        //Keybindings
        private Button _editKeybindingsBtn;
        private Button _exitKeybindingsButton;
        private int _edittingKey = -1;
        private Controls _edittingControl;
        private Button _edittingButton;
        private GameControls _edittingControls;
        private Dictionary<Controls, Button[]> _keyButtons = new Dictionary<Controls, Button[]>();
        private long _listeningTimer;

        //Init
        public OptionsWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _menuPanel = new ImagePanel(parent)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uibody.png")
            };
            _menuPanel.SetSize(512, 393);
            if (_mainMenu != null && parentPanel != null)
            {
                _menuPanel.SetPosition(parentPanel.X, parentPanel.Y);
            }
            else
            {
                _menuPanel.SetPosition(parent.Width / 2 - _menuPanel.Width / 2,
                    parent.Height / 2 - _menuPanel.Height / 2);
            }
            _menuPanel.IsHidden = true;
            Gui.InputBlockingElements.Add(_menuPanel);

            //Menu Header
            _menuHeader = new Label(_menuPanel)
            {
                AutoSizeToContents = false
            };
            _menuHeader.SetText(Strings.Get("options", "title"));
            _menuHeader.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 24);
            _menuHeader.SetSize(_menuPanel.Width, _menuPanel.Height);
            _menuHeader.Alignment = Pos.CenterH;
            _menuHeader.TextColorOverride = new Color(255, 200, 200, 200);

            //Options Get Stored in the Options Scroll Control
            _optionsContainer = new ScrollControl(_menuPanel);
            _optionsContainer.SetSize(_menuPanel.Width, _menuPanel.Height - 34);
            _optionsContainer.SetPosition(0, 34);
            _optionsContainer.EnableScroll(false, false);
            _optionsContainer.AutoHideBars = false;
            _optionsContainer.Show();

            //Resolution Background
            _resolutionBackground = new ImagePanel(_optionsContainer)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png")
            };
            _resolutionBackground.SetSize(_resolutionBackground.Texture.GetWidth(),
                _resolutionBackground.Texture.GetHeight());
            _resolutionBackground.SetPosition(_menuPanel.Width / 2 - _resolutionBackground.Width / 2, 10);

            //Options - Resolution Label
            _resolutionLabel = new Label(_resolutionBackground);
            _resolutionLabel.SetText(Strings.Get("options", "resolution"));
            _resolutionLabel.AutoSizeToContents = false;
            _resolutionLabel.SetSize(178, 60);
            _resolutionLabel.Alignment = Pos.Center;
            _resolutionLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _resolutionLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

            _resolutionList = new ComboBox(_resolutionBackground);
            _resolutionList.SetPosition(190, 8);
            _resolutionList.SetSize(260, 38);
            _resolutionList.Alignment = Pos.Center;
            _resolutionList.ShouldDrawBackground = false;
            _resolutionList.SetMenuBackgroundColor(new Color(220, 0, 0, 0));
            _resolutionList.SetMenuMaxSize(260, 200);
            _resolutionList.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _resolutionList.SetTextColor(new Color(255, 220, 220, 220), Label.ControlState.Hovered);
            _resolutionList.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count; i++)
            {
                var item = _resolutionList.AddItem(myModes[i]);
                item.Alignment = Pos.Center;
            }

            //FPS Background
            _fpsBackground = new ImagePanel(_optionsContainer)
            {
                Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "inputfield.png")
            };
            _fpsBackground.SetSize(_fpsBackground.Texture.GetWidth(), _fpsBackground.Texture.GetHeight());
            _fpsBackground.SetPosition(_menuPanel.Width / 2 - _fpsBackground.Width / 2,
                _resolutionBackground.Bottom + 16);

            //Options - FPS Label
            _fpsLabel = new Label(_fpsBackground);
            _fpsLabel.SetText(Strings.Get("options", "targetfps"));
            _fpsLabel.AutoSizeToContents = false;
            _fpsLabel.SetSize(176, 55);
            _fpsLabel.Alignment = Pos.Center;
            _fpsLabel.TextColorOverride = new Color(255, 30, 30, 30);
            _fpsLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

            //Options - FPS List
            _fpsList = new ComboBox(_fpsBackground);
            _fpsList.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _fpsList.SetTextColor(new Color(255, 220, 220, 220), Label.ControlState.Hovered);
            _fpsList.AddItem(Strings.Get("options", "vsync"));
            _fpsList.AddItem(Strings.Get("options", "30fps"));
            _fpsList.AddItem(Strings.Get("options", "60fps"));
            _fpsList.AddItem(Strings.Get("options", "90fps"));
            _fpsList.AddItem(Strings.Get("options", "120fps"));
            _fpsList.AddItem(Strings.Get("options", "unlimitedfps"));
            _fpsList.SetPosition(190, 8);
            _fpsList.SetSize(260, 38);
            _fpsList.Alignment = Pos.Center;
            _fpsList.ShouldDrawBackground = false;
            _fpsList.SetMenuBackgroundColor(new Color(220, 0, 0, 0));
            _fpsList.SetMenuMaxSize(260, 200);
            _fpsList.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

            //Options - Fullscreen Checkbox
            _fullscreen = new LabeledCheckBox(_optionsContainer) { Text = Strings.Get("options", "fullscreen") };
            _fullscreen.SetSize(300, 36);
            _fullscreen.SetPosition(_fpsBackground.X + 24, _fpsBackground.Bottom + 16);
            _fullscreen.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxempty.png"),
                CheckBox.ControlState.Normal);
            _fullscreen.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "checkboxfull.png"),
                CheckBox.ControlState.CheckedNormal);
            _fullscreen.SetCheckSize(32, 32);
            _fullscreen.SetLabelDistance(12);
            _fullscreen.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _fullscreen.SetTextColor(new Color(255, 140, 140, 140), Label.ControlState.Hovered);
            _fullscreen.SetFont(Globals.ContentManager.GetFont(Gui.ActiveFont, 20));

            _editKeybindingsBtn = new Button(_optionsContainer) { Text = Strings.Get("controls", "edit") };
            _editKeybindingsBtn.SetSize(200, 36);
            _editKeybindingsBtn.SetPosition(_resolutionBackground.Right - 200, _fpsBackground.Bottom + 16);
            _editKeybindingsBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _editKeybindingsBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _editKeybindingsBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _editKeybindingsBtn.TextPadding = new Padding(0, 3, 0, 0);
            _editKeybindingsBtn.Font = (Globals.ContentManager.GetFont(Gui.ActiveFont, 18));
            _editKeybindingsBtn.Clicked += _editKeybindingsBtn_Clicked;
            _editKeybindingsBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "editcontrolsnormal.png"),Button.ControlState.Normal);
            _editKeybindingsBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "editcontrolshover.png"),Button.ControlState.Hovered);
            _editKeybindingsBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "editcontrolsclicked.png"),Button.ControlState.Clicked);

            //Options - Sound Label
            _soundLabel = new Label(_optionsContainer);
            _soundLabel.SetText(Strings.Get("options", "soundvolume", 100));
            _soundLabel.SetSize(210, 32);
            _soundLabel.AutoSizeToContents = false;
            _soundLabel.SetPosition(_resolutionBackground.X, _fullscreen.Bottom + 16);
            _soundLabel.Alignment = Pos.Center;
            _soundLabel.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _soundLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 16);

            //Options - Sound Slider
            _soundSlider = new HorizontalSlider(_optionsContainer);
            _soundSlider.SetSize(210, 25);
            _soundSlider.SetPosition(_soundLabel.X, _soundLabel.Bottom + 8);
            _soundSlider.Min = 0;
            _soundSlider.Max = 100;
            _soundSlider.ValueChanged += _soundSlider_ValueChanged;
            _soundSlider.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sliderbar.png"));
            _soundSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggerv.png"),
                Dragger.ControlState.Normal);
            _soundSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervclick.png"),
                Dragger.ControlState.Clicked);
            _soundSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervhover.png"),
                Dragger.ControlState.Hovered);

            _soundSlider.SetDraggerSize(14, 26);

            //Options - Music Label
            _musicLabel = new Label(_optionsContainer);
            _musicLabel.SetText(Strings.Get("options", "musicvolume", 100));
            _musicLabel.SetSize(210, 32);
            _musicLabel.AutoSizeToContents = false;
            _musicLabel.SetPosition(_resolutionBackground.Right - _musicLabel.Width, _fullscreen.Bottom + 16);
            _musicLabel.Alignment = Pos.Center;
            _musicLabel.SetTextColor(new Color(255, 200, 200, 200), Label.ControlState.Normal);
            _musicLabel.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 16);

            //Options - Music Slider
            _musicSlider = new HorizontalSlider(_optionsContainer);
            _musicSlider.SetSize(210, 24);
            _musicSlider.SetPosition(_musicLabel.X, _musicLabel.Bottom + 8);
            _musicSlider.Min = 0;
            _musicSlider.Max = 100;
            _musicSlider.ValueChanged += _musicSlider_ValueChanged;
            _musicSlider.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "sliderbar.png"));
            _musicSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggerv.png"),
                Dragger.ControlState.Normal);
            _musicSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervclick.png"),
                Dragger.ControlState.Clicked);
            _musicSlider.SetDraggerImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "draggervhover.png"),
                Dragger.ControlState.Hovered);
            _musicSlider.SetDraggerSize(14, 26);

            //Options - Apply Button
            _applyBtn = new Button(_optionsContainer);
            _applyBtn.SetText(Strings.Get("options", "apply"));
            _applyBtn.SetPosition(_resolutionBackground.X, _soundSlider.Bottom + 16);
            _applyBtn.SetSize(56, 32);
            _applyBtn.Clicked += ApplyBtn_Clicked;
            _applyBtn.SetSize(211, 61);
            _applyBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _applyBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _applyBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _applyBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _applyBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _applyBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _applyBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

            //Options - Back Button
            _backBtn = new Button(_optionsContainer);
            _backBtn.SetText(Strings.Get("options", "cancel"));
            _backBtn.SetSize(211, 61);
            _backBtn.SetPosition(_resolutionBackground.Right - _backBtn.Width, _soundSlider.Bottom + 16);
            _backBtn.Clicked += BackBtn_Clicked;
            _backBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonnormal.png"),
                Button.ControlState.Normal);
            _backBtn.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonhover.png"),
                Button.ControlState.Hovered);
            _backBtn.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "buttonclicked.png"),
                Button.ControlState.Clicked);
            _backBtn.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _backBtn.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _backBtn.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _backBtn.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 20);

            //Controls Get Stored in the Controls Scroll Control
            _controlsContainer = new ScrollControl(_menuPanel);
            _controlsContainer.SetSize(_menuPanel.Width, _menuPanel.Height - 34);
            _controlsContainer.SetPosition(0, 34);
            _controlsContainer.EnableScroll(false, true);
            _controlsContainer.AutoHideBars = false;
            _controlsContainer.Hide();

            var scrollbar = _controlsContainer.GetVerticalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"),
                Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"),
                Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"),
                Dragger.ControlState.Clicked);

            var upButton = scrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"),
                Button.ControlState.Normal);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"),
                Button.ControlState.Clicked);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"),
                Button.ControlState.Hovered);
            var downButton = scrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"),
                Button.ControlState.Normal);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"),
                Button.ControlState.Clicked);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"),
                Button.ControlState.Hovered);

            _exitKeybindingsButton = new Button(_menuPanel);
            _exitKeybindingsButton.SetSize(15, 15);
            _exitKeybindingsButton.SetPosition(4, 8);
            _exitKeybindingsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrownormal.png"),
                Button.ControlState.Normal);
            _exitKeybindingsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowclicked.png"),
                Button.ControlState.Clicked);
            _exitKeybindingsButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "leftarrowhover.png"),
                Button.ControlState.Hovered);
            _exitKeybindingsButton.Hide();
            _exitKeybindingsButton.Clicked += _editKeybindingsBtn_Clicked;

            var controlsY = 8;
            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var label = new Label(_controlsContainer);
                label.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 16);
                label.Text = Strings.Get("controls", Enum.GetName(typeof(Controls), control).ToLower());
                label.SetPosition(8, controlsY);
                label.SetTextColor(Color.White, Label.ControlState.Normal);

                var key1 = new Button(_controlsContainer);
                key1.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 16);
                key1.SetTextColor(Color.Black, Label.ControlState.Normal);
                key1.Text = "";
                key1.SetSize(120, 28);
                key1.SetPosition(200, controlsY - 2);
                key1.UserData = control;
                key1.Clicked += Key1_Clicked;
                key1.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
                key1.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
                key1.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
                key1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlnormal.png"), Button.ControlState.Normal);
                key1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlhover.png"), Button.ControlState.Hovered);
                key1.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlcliked.png"), Button.ControlState.Clicked);


                var key2 = new Button(_controlsContainer);
                key2.Font = Globals.ContentManager.GetFont(Gui.ActiveFont, 16);
                key2.SetTextColor(Color.Black, Label.ControlState.Normal);
                key2.Text = "";
                key2.SetSize(120, 28);
                key2.UserData = control;
                key2.SetPosition(350, controlsY - 2);
                key2.Clicked += Key2_Clicked;
                key2.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
                key2.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
                key2.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
                key2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlnormal.png"), Button.ControlState.Normal);
                key2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlhover.png"), Button.ControlState.Hovered);
                key2.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "controlcliked.png"), Button.ControlState.Clicked);
                controlsY += 32;

                _keyButtons.Add(control, new Button[2] { key1, key2 });
            }

            GameInputHandler.KeyDown += OnKeyDown;
            GameInputHandler.MouseDown += OnKeyDown;
        }

        private void Key2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button)sender, 2);
        }

        private void Key1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button)sender, 1);
        }

        private void EditKeyPressed(Button sender, int keyNum)
        {
            if (_edittingButton == null)
            {
                sender.Text = Strings.Get("controls", "listening");
                _edittingKey = keyNum;
                _edittingControl = (Controls)sender.UserData;
                _edittingButton = sender;
                Gui.GwenInput.HandleInput = false;
                _listeningTimer = Globals.System.GetTimeMS() + 5000;
            }
        }

        private void _editKeybindingsBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Determine if controls are currently being shown or not
            if (_controlsContainer.IsVisible)
            {
                _controlsContainer.Hide();
                _optionsContainer.Show();
                _menuHeader.SetText(Strings.Get("options", "title"));
                _exitKeybindingsButton.Hide();
            }
            else
            {
                _controlsContainer.Show();
                _optionsContainer.Hide();
                _menuHeader.SetText(Strings.Get("controls", "title"));
                _exitKeybindingsButton.Show();
                foreach (Controls control in Enum.GetValues(typeof(Controls)))
                {
                    _keyButtons[control][0].Text = Strings.Get("keys", Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[control].key1));
                    _keyButtons[control][1].Text = Strings.Get("keys", Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[control].key2));
                }
            }
        }

        private void OnKeyDown(Keys key)
        {
            if (_edittingButton != null)
            {
                if (key == Keys.Escape) key = Keys.None;
                _edittingControls.UpdateControl(_edittingControl, _edittingKey, key);
                if (_edittingKey == 1)
                {
                    _edittingButton.Text = Strings.Get("keys", Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[_edittingControl].key1));
                }
                else
                {
                    _edittingButton.Text = Strings.Get("keys", Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[_edittingControl].key2));
                }
                _edittingButton = null;
                Gui.GwenInput.HandleInput = true;
            }
        }

        //Methods
        public void Update()
        {
            if (_menuPanel.IsVisible && _edittingButton != null && _listeningTimer < Globals.System.GetTimeMS())
            {
                OnKeyDown(Keys.None);
            }
        }

        public void Show()
        {
            if (_mainMenu == null)
            {
                _menuPanel.MakeModal(true);
            }
            _previousMusicVolume = Globals.Database.MusicVolume;
            _previousSoundVolume = Globals.Database.SoundVolume;
            _edittingControls = new GameControls(GameControls.ActiveControls);
            if (GameGraphics.Renderer.GetValidVideoModes().Count > 0)
            {
                _resolutionList.SelectByText(
                    GameGraphics.Renderer.GetValidVideoModes()[Globals.Database.TargetResolution]);
            }
            switch (Globals.Database.TargetFps)
            {
                case -1: //Unlimited
                    _fpsList.SelectByText(Strings.Get("options", "unlimitedfps"));
                    break;
                case 0: //VSYNC
                    _fpsList.SelectByText(Strings.Get("options", "vsync"));
                    break;
                case 1:
                    _fpsList.SelectByText(Strings.Get("options", "30fps"));
                    break;
                case 2:
                    _fpsList.SelectByText(Strings.Get("options", "60fps"));
                    break;
                case 3:
                    _fpsList.SelectByText(Strings.Get("options", "90fps"));
                    break;
                case 4:
                    _fpsList.SelectByText(Strings.Get("options", "120fps"));
                    break;
                default:
                    _fpsList.SelectByText(Strings.Get("options", "vsync"));
                    break;
            }
            _fullscreen.IsChecked = Globals.Database.FullScreen;
            _musicSlider.Value = Globals.Database.MusicVolume;
            _soundSlider.Value = Globals.Database.SoundVolume;
            _musicLabel.Text = Strings.Get("options", "musicvolume", (int)_musicSlider.Value);
            _soundLabel.Text = Strings.Get("options", "soundvolume", (int)_soundSlider.Value);
            _menuPanel.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_menuPanel.IsHidden;
        }

        public void Hide()
        {
            if (_mainMenu == null)
            {
                _menuPanel.RemoveModal();
            }
            _menuPanel.IsHidden = true;
        }

        //Input Handlers
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Database.MusicVolume = _previousMusicVolume;
            Globals.Database.SoundVolume = _previousSoundVolume;
            GameAudio.UpdateGlobalVolume();
            if (Globals.GameState == GameStates.Menu)
            {
                Hide();
                _mainMenu.Show();
            }
            else
            {
                Hide();
            }
        }

        void _musicSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            _musicLabel.Text = Strings.Get("options", "musicvolume", (int)_musicSlider.Value);
            Globals.Database.MusicVolume = (int)_musicSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void _soundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            _soundLabel.Text = Strings.Get("options", "soundvolume", (int)_soundSlider.Value);
            Globals.Database.SoundVolume = (int)_soundSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void ApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            bool shouldReset = false;
            var resolution = _resolutionList.SelectedItem;
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count; i++)
            {
                if (resolution.Text == myModes[i])
                {
                    if (Globals.Database.TargetResolution != i) shouldReset = true;
                    Globals.Database.TargetResolution = i;
                }
            }
            if (Globals.Database.FullScreen != _fullscreen.IsChecked)
            {
                Globals.Database.FullScreen = _fullscreen.IsChecked;
                shouldReset = true;
            }
            var newFps = 0;
            if (_fpsList.SelectedItem.Text == Strings.Get("options", "unlimitedfps"))
            {
                newFps = -1;
            }
            else if (_fpsList.SelectedItem.Text == Strings.Get("options", "30fps"))
            {
                newFps = 1;
            }
            else if (_fpsList.SelectedItem.Text == Strings.Get("options", "60fps"))
            {
                newFps = 2;
            }
            else if (_fpsList.SelectedItem.Text == Strings.Get("options", "90fps"))
            {
                newFps = 3;
            }
            else if (_fpsList.SelectedItem.Text == Strings.Get("options", "120fps"))
            {
                newFps = 4;
            }

            if (newFps != Globals.Database.TargetFps)
            {
                shouldReset = true;
                Globals.Database.TargetFps = newFps;
            }
            Globals.Database.MusicVolume = (int)_musicSlider.Value;
            Globals.Database.SoundVolume = (int)_soundSlider.Value;
            GameControls.ActiveControls = _edittingControls;
            GameControls.ActiveControls.Save();
            GameAudio.UpdateGlobalVolume();
            Globals.Database.SavePreferences();
            if (shouldReset) GameGraphics.Renderer.Init();
            if (Globals.GameState == GameStates.InGame)
            {
                Hide();
            }
            else
            {
                Hide();
                _mainMenu.Show();
            }
        }
    }
}