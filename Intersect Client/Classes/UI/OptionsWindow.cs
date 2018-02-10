using System;
using System.Collections.Generic;
using Intersect.Client.Classes.Core;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI.Menu;

namespace Intersect_Client.Classes.UI
{
    public class OptionsWindow
    {
        private Button _applyBtn;
        private Button _backBtn;
        private ScrollControl _controlsContainer;

        //Keybindings
        private Button _editKeybindingsBtn;

        private Button _edittingButton;
        private Controls _edittingControl;
        private GameControls _edittingControls;
        private int _edittingKey = -1;
        private Button _exitKeybindingsButton;

        private ImagePanel _fpsBackground;
        private Label _fpsLabel;
        private ComboBox _fpsList;

        private LabeledCheckBox _fullscreen;

        //Parent Windows
        private bool _gameWindow = false;

        private Dictionary<Controls, Button[]> _keyButtons = new Dictionary<Controls, Button[]>();
        private long _listeningTimer;
        private MainMenu _mainMenu;
        private Label _musicLabel;
        private HorizontalSlider _musicSlider;

        //Panels
        private ScrollControl _optionsContainer;

        //Window
        private Label _optionsHeader;

        //Controls
        private ImagePanel _optionsPanel;

        private int _previousMusicVolume;

        private int _previousSoundVolume;

        private ImagePanel _resolutionBackground;
        private Label _resolutionLabel;
        private ComboBox _resolutionList;
        private Label _soundLabel;
        private HorizontalSlider _soundSlider;

        //Init
        public OptionsWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            _mainMenu = mainMenu;

            //Main Menu Window
            _optionsPanel = new ImagePanel(parent, "OptionsPanel");
            _optionsPanel.IsHidden = true;
            Gui.InputBlockingElements.Add(_optionsPanel);

            //Menu Header
            _optionsHeader = new Label(_optionsPanel, "OptionsHeader");
            _optionsHeader.SetText(Strings.Get("options", "title"));

            //Options Get Stored in the Options Scroll Control
            _optionsContainer = new ScrollControl(_optionsPanel, "OptionsContainer");
            _optionsContainer.EnableScroll(false, false);
            _optionsContainer.Show();

            //Resolution Background
            _resolutionBackground = new ImagePanel(_optionsContainer, "ResolutionPanel");

            //Options - Resolution Label
            _resolutionLabel = new Label(_resolutionBackground, "ResolutionLabel");
            _resolutionLabel.SetText(Strings.Get("options", "resolution"));

            _resolutionList = new ComboBox(_resolutionBackground, "ResolutionCombobox");
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count; i++)
            {
                var item = _resolutionList.AddItem(myModes[i]);
                item.Alignment = Pos.Center;
            }

            //FPS Background
            _fpsBackground = new ImagePanel(_optionsContainer, "FPSPanel");

            //Options - FPS Label
            _fpsLabel = new Label(_fpsBackground, "FPSLabel");
            _fpsLabel.SetText(Strings.Get("options", "targetfps"));

            //Options - FPS List
            _fpsList = new ComboBox(_fpsBackground, "FPSCombobox");
            _fpsList.AddItem(Strings.Get("options", "vsync"));
            _fpsList.AddItem(Strings.Get("options", "30fps"));
            _fpsList.AddItem(Strings.Get("options", "60fps"));
            _fpsList.AddItem(Strings.Get("options", "90fps"));
            _fpsList.AddItem(Strings.Get("options", "120fps"));
            _fpsList.AddItem(Strings.Get("options", "unlimitedfps"));

            //Options - Fullscreen Checkbox
            _fullscreen =
                new LabeledCheckBox(_optionsContainer, "FullscreenCheckbox")
                {
                    Text = Strings.Get("options", "fullscreen")
                };

            _editKeybindingsBtn =
                new Button(_optionsContainer, "KeybindingsButton") {Text = Strings.Get("controls", "edit")};
            _editKeybindingsBtn.Clicked += _editKeybindingsBtn_Clicked;

            //Options - Sound Label
            _soundLabel = new Label(_optionsContainer, "SoundLabel");
            _soundLabel.SetText(Strings.Get("options", "soundvolume", 100));

            //Options - Sound Slider
            _soundSlider = new HorizontalSlider(_optionsContainer, "SoundSlider");
            _soundSlider.Min = 0;
            _soundSlider.Max = 100;
            _soundSlider.ValueChanged += _soundSlider_ValueChanged;

            //Options - Music Label
            _musicLabel = new Label(_optionsContainer, "MusicLabel");
            _musicLabel.SetText(Strings.Get("options", "musicvolume", 100));

            //Options - Music Slider
            _musicSlider = new HorizontalSlider(_optionsContainer, "MusicSlider");
            _musicSlider.Min = 0;
            _musicSlider.Max = 100;
            _musicSlider.ValueChanged += _musicSlider_ValueChanged;

            //Controls Get Stored in the Controls Scroll Control
            _controlsContainer = new ScrollControl(_optionsPanel, "ControlsContainer");
            _controlsContainer.EnableScroll(false, true);
            _controlsContainer.Hide();

            _exitKeybindingsButton = new Button(_optionsPanel, "ExitControlsButton");
            _exitKeybindingsButton.Hide();
            _exitKeybindingsButton.Clicked += _editKeybindingsBtn_Clicked;

            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var label = new Label(_controlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Label");
                label.Text = Strings.Get("controls", Enum.GetName(typeof(Controls), control).ToLower());

                var key1 = new Button(_controlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Button1");
                key1.Text = "";
                key1.UserData = control;
                key1.Clicked += Key1_Clicked;

                var key2 = new Button(_controlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Button2");
                key2.Text = "";
                key2.UserData = control;
                key2.Clicked += Key2_Clicked;

                _keyButtons.Add(control, new Button[2] {key1, key2});
            }

            //Options - Apply Button
            _applyBtn = new Button(_optionsContainer, "ApplyButton");
            _applyBtn.SetText(Strings.Get("options", "apply"));
            _applyBtn.Clicked += ApplyBtn_Clicked;

            //Options - Back Button
            _backBtn = new Button(_optionsContainer, "CancelButton");
            _backBtn.SetText(Strings.Get("options", "cancel"));
            _backBtn.Clicked += BackBtn_Clicked;

            GameInputHandler.KeyDown += OnKeyDown;
            GameInputHandler.MouseDown += OnKeyDown;
        }

        private void Key2_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button) sender, 2);
        }

        private void Key1_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button) sender, 1);
        }

        private void EditKeyPressed(Button sender, int keyNum)
        {
            if (_edittingButton == null)
            {
                sender.Text = Strings.Get("controls", "listening");
                _edittingKey = keyNum;
                _edittingControl = (Controls) sender.UserData;
                _edittingButton = sender;
                Gui.GwenInput.HandleInput = false;
                _listeningTimer = Globals.System.GetTimeMs() + 5000;
            }
        }

        private void _editKeybindingsBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Determine if controls are currently being shown or not
            if (_controlsContainer.IsVisible)
            {
                _controlsContainer.Hide();
                _optionsContainer.Show();
                _optionsHeader.SetText(Strings.Get("options", "title"));
                _exitKeybindingsButton.Hide();
            }
            else
            {
                _controlsContainer.Show();
                _optionsContainer.Hide();
                _optionsHeader.SetText(Strings.Get("controls", "title"));
                _exitKeybindingsButton.Show();
                foreach (Controls control in Enum.GetValues(typeof(Controls)))
                {
                    _keyButtons[control][0].Text = Strings.Get("keys",
                        Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[control].key1));
                    _keyButtons[control][1].Text = Strings.Get("keys",
                        Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[control].key2));
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
                    _edittingButton.Text = Strings.Get("keys",
                        Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[_edittingControl].key1));
                }
                else
                {
                    _edittingButton.Text = Strings.Get("keys",
                        Enum.GetName(typeof(Keys), _edittingControls.ControlMapping[_edittingControl].key2));
                }
                _edittingButton = null;
                Gui.GwenInput.HandleInput = true;
            }
        }

        //Methods
        public void Update()
        {
            if (_optionsPanel.IsVisible && _edittingButton != null && _listeningTimer < Globals.System.GetTimeMs())
            {
                OnKeyDown(Keys.None);
            }
        }

        public void Show()
        {
            if (_mainMenu == null)
            {
                _optionsPanel.MakeModal(true);
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
            _musicLabel.Text = Strings.Get("options", "musicvolume", (int) _musicSlider.Value);
            _soundLabel.Text = Strings.Get("options", "soundvolume", (int) _soundSlider.Value);
            _optionsPanel.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !_optionsPanel.IsHidden;
        }

        public void Hide()
        {
            if (_mainMenu == null)
            {
                _optionsPanel.RemoveModal();
            }
            _optionsPanel.IsHidden = true;
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
            _musicLabel.Text = Strings.Get("options", "musicvolume", (int) _musicSlider.Value);
            Globals.Database.MusicVolume = (int) _musicSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void _soundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            _soundLabel.Text = Strings.Get("options", "soundvolume", (int) _soundSlider.Value);
            Globals.Database.SoundVolume = (int) _soundSlider.Value;
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
            Globals.Database.MusicVolume = (int) _musicSlider.Value;
            Globals.Database.SoundVolume = (int) _soundSlider.Value;
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