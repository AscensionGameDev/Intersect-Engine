using System;
using System.Collections.Generic;
using Intersect.Client.Classes.Core;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
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
        private Button mApplyBtn;
        private Button mBackBtn;
        private ScrollControl mControlsContainer;

        //Keybindings
        private Button mEditKeybindingsBtn;

        private Button mEdittingButton;
        private Controls mEdittingControl;
        private GameControls mEdittingControls;
        private int mEdittingKey = -1;
        private Button mExitKeybindingsButton;

        private ImagePanel mFpsBackground;
        private Label mFpsLabel;
        private ComboBox mFpsList;

        private LabeledCheckBox mFullscreen;

        //Parent Windows
        private bool mGameWindow = false;

        private Dictionary<Controls, Button[]> mKeyButtons = new Dictionary<Controls, Button[]>();
        private long mListeningTimer;
        private MainMenu mMainMenu;
        private Label mMusicLabel;
        private HorizontalSlider mMusicSlider;

        //Panels
        private ScrollControl mOptionsContainer;

        //Window
        private Label mOptionsHeader;

        //Controls
        private ImagePanel mOptionsPanel;

        private int mPreviousMusicVolume;

        private int mPreviousSoundVolume;

        private ImagePanel mResolutionBackground;
        private Label mResolutionLabel;
        private ComboBox mResolutionList;
        private Label mSoundLabel;
        private HorizontalSlider mSoundSlider;

        //Init
        public OptionsWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mOptionsPanel = new ImagePanel(parent, "OptionsWindow");
            mOptionsPanel.IsHidden = true;
            Gui.InputBlockingElements.Add(mOptionsPanel);

            //Menu Header
            mOptionsHeader = new Label(mOptionsPanel, "OptionsHeader");
            mOptionsHeader.SetText(Strings.Options.title);

            //Options Get Stored in the Options Scroll Control
            mOptionsContainer = new ScrollControl(mOptionsPanel, "OptionsContainer");
            mOptionsContainer.EnableScroll(false, false);
            mOptionsContainer.Show();

            //Resolution Background
            mResolutionBackground = new ImagePanel(mOptionsContainer, "ResolutionPanel");

            //Options - Resolution Label
            mResolutionLabel = new Label(mResolutionBackground, "ResolutionLabel");
            mResolutionLabel.SetText(Strings.Options.resolution);

            mResolutionList = new ComboBox(mResolutionBackground, "ResolutionCombobox");
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count; i++)
            {
                var item = mResolutionList.AddItem(myModes[i]);
                item.Alignment = Pos.Center;
            }

            //FPS Background
            mFpsBackground = new ImagePanel(mOptionsContainer, "FPSPanel");

            //Options - FPS Label
            mFpsLabel = new Label(mFpsBackground, "FPSLabel");
            mFpsLabel.SetText(Strings.Options.targetfps);

            //Options - FPS List
            mFpsList = new ComboBox(mFpsBackground, "FPSCombobox");
            mFpsList.AddItem(Strings.Options.vsync);
            mFpsList.AddItem(Strings.Options.fps30);
            mFpsList.AddItem(Strings.Options.fps60);
            mFpsList.AddItem(Strings.Options.fps90);
            mFpsList.AddItem(Strings.Options.fps120);
            mFpsList.AddItem(Strings.Options.unlimitedfps);

            //Options - Fullscreen Checkbox
            mFullscreen =
                new LabeledCheckBox(mOptionsContainer, "FullscreenCheckbox")
                {
                    Text = Strings.Options.fullscreen
                };

            mEditKeybindingsBtn =
                new Button(mOptionsContainer, "KeybindingsButton") { Text = Strings.Controls.edit };
            mEditKeybindingsBtn.Clicked += _editKeybindingsBtn_Clicked;

            //Options - Sound Label
            mSoundLabel = new Label(mOptionsContainer, "SoundLabel");
            mSoundLabel.SetText(Strings.Options.soundvolume.ToString( 100));

            //Options - Sound Slider
            mSoundSlider = new HorizontalSlider(mOptionsContainer, "SoundSlider");
            mSoundSlider.Min = 0;
            mSoundSlider.Max = 100;
            mSoundSlider.ValueChanged += _soundSlider_ValueChanged;

            //Options - Music Label
            mMusicLabel = new Label(mOptionsContainer, "MusicLabel");
            mMusicLabel.SetText(Strings.Options.musicvolume.ToString( 100));

            //Options - Music Slider
            mMusicSlider = new HorizontalSlider(mOptionsContainer, "MusicSlider");
            mMusicSlider.Min = 0;
            mMusicSlider.Max = 100;
            mMusicSlider.ValueChanged += _musicSlider_ValueChanged;

            //Controls Get Stored in the Controls Scroll Control
            mControlsContainer = new ScrollControl(mOptionsPanel, "ControlsContainer");
            mControlsContainer.EnableScroll(false, true);
            mControlsContainer.Hide();

            mExitKeybindingsButton = new Button(mOptionsPanel, "ExitControlsButton");
            mExitKeybindingsButton.Hide();
            mExitKeybindingsButton.Clicked += _editKeybindingsBtn_Clicked;

            foreach (Controls control in Enum.GetValues(typeof(Controls)))
            {
                var label = new Label(mControlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Label");
                label.Text = Strings.Controls.controldict[Enum.GetName(typeof(Controls), control).ToLower()];

                var key1 = new Button(mControlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Button1");
                key1.Text = "";
                key1.UserData = control;
                key1.Clicked += Key1_Clicked;

                var key2 = new Button(mControlsContainer,
                    "Control" + Enum.GetName(typeof(Controls), control) + "Button2");
                key2.Text = "";
                key2.UserData = control;
                key2.Clicked += Key2_Clicked;

                mKeyButtons.Add(control, new Button[2] {key1, key2});
            }

            //Options - Apply Button
            mApplyBtn = new Button(mOptionsContainer, "ApplyButton");
            mApplyBtn.SetText(Strings.Options.apply);
            mApplyBtn.Clicked += ApplyBtn_Clicked;

            //Options - Back Button
            mBackBtn = new Button(mOptionsContainer, "CancelButton");
            mBackBtn.SetText(Strings.Options.cancel);
            mBackBtn.Clicked += BackBtn_Clicked;

            GameInputHandler.KeyDown += OnKeyDown;
            GameInputHandler.MouseDown += OnKeyDown;

            if (mainMenu == null)
            {
                _optionsPanel.LoadJsonUi(GameContentManager.UI.InGame);
            }
            else
            {
                _optionsPanel.LoadJsonUi(GameContentManager.UI.Menu);
            }
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
            if (mEdittingButton == null)
            {
                sender.Text = Strings.Controls.listening;
                mEdittingKey = keyNum;
                mEdittingControl = (Controls) sender.UserData;
                mEdittingButton = sender;
                Gui.GwenInput.HandleInput = false;
                mListeningTimer = Globals.System.GetTimeMs() + 5000;
            }
        }

        private void _editKeybindingsBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Determine if controls are currently being shown or not
            if (mControlsContainer.IsVisible)
            {
                mControlsContainer.Hide();
                mOptionsContainer.Show();
                mOptionsHeader.SetText(Strings.Options.title);
                mExitKeybindingsButton.Hide();
            }
            else
            {
                mControlsContainer.Show();
                mOptionsContainer.Hide();
                mOptionsHeader.SetText(Strings.Controls.title);
                mExitKeybindingsButton.Show();
                foreach (Controls control in Enum.GetValues(typeof(Controls)))
                {
                    mKeyButtons[control][0].Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key1).ToLower()];
                    mKeyButtons[control][1].Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key2).ToLower()];
                }
            }
        }

        private void OnKeyDown(Keys key)
        {
            if (mEdittingButton != null)
            {
                if (key == Keys.Escape) key = Keys.None;
                mEdittingControls.UpdateControl(mEdittingControl, mEdittingKey, key);
                if (mEdittingKey == 1)
                {
                    mEdittingButton.Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[mEdittingControl].Key1).ToLower()];
                }
                else
                {
                    mEdittingButton.Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[mEdittingControl].Key2).ToLower()];
                }
                mEdittingButton = null;
                Gui.GwenInput.HandleInput = true;
            }
        }

        //Methods
        public void Update()
        {
            if (mOptionsPanel.IsVisible && mEdittingButton != null && mListeningTimer < Globals.System.GetTimeMs())
            {
                OnKeyDown(Keys.None);
            }
        }

        public void Show()
        {
            if (mMainMenu == null)
            {
                mOptionsPanel.MakeModal(true);
            }
            mPreviousMusicVolume = Globals.Database.MusicVolume;
            mPreviousSoundVolume = Globals.Database.SoundVolume;
            mEdittingControls = new GameControls(GameControls.ActiveControls);
            if (GameGraphics.Renderer.GetValidVideoModes().Count > 0)
            {
                mResolutionList.SelectByText(
                    GameGraphics.Renderer.GetValidVideoModes()[Globals.Database.TargetResolution]);
            }
            switch (Globals.Database.TargetFps)
            {
                case -1: //Unlimited
                    mFpsList.SelectByText(Strings.Options.unlimitedfps);
                    break;
                case 0: //VSYNC
                    mFpsList.SelectByText(Strings.Options.vsync);
                    break;
                case 1:
                    mFpsList.SelectByText(Strings.Options.fps30);
                    break;
                case 2:
                    mFpsList.SelectByText(Strings.Options.fps60);
                    break;
                case 3:
                    mFpsList.SelectByText(Strings.Options.fps90);
                    break;
                case 4:
                    mFpsList.SelectByText(Strings.Options.fps120);
                    break;
                default:
                    mFpsList.SelectByText(Strings.Options.vsync);
                    break;
            }
            mFullscreen.IsChecked = Globals.Database.FullScreen;
            mMusicSlider.Value = Globals.Database.MusicVolume;
            mSoundSlider.Value = Globals.Database.SoundVolume;
            mMusicLabel.Text = Strings.Options.musicvolume.ToString( (int)mMusicSlider.Value);
            mSoundLabel.Text = Strings.Options.soundvolume.ToString( (int)mSoundSlider.Value);
            mOptionsPanel.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mOptionsPanel.IsHidden;
        }

        public void Hide()
        {
            if (mMainMenu == null)
            {
                mOptionsPanel.RemoveModal();
            }
            mOptionsPanel.IsHidden = true;
        }

        //Input Handlers
        void BackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Database.MusicVolume = mPreviousMusicVolume;
            Globals.Database.SoundVolume = mPreviousSoundVolume;
            GameAudio.UpdateGlobalVolume();
            if (Globals.GameState == GameStates.Menu)
            {
                Hide();
                mMainMenu.Show();
            }
            else
            {
                Hide();
            }
        }

        void _musicSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            mMusicLabel.Text = Strings.Options.musicvolume.ToString((int)mMusicSlider.Value);
            Globals.Database.MusicVolume = (int) mMusicSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void _soundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            mSoundLabel.Text = Strings.Options.soundvolume.ToString((int)mSoundSlider.Value);
            Globals.Database.SoundVolume = (int) mSoundSlider.Value;
            GameAudio.UpdateGlobalVolume();
        }

        void ApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            bool shouldReset = false;
            var resolution = mResolutionList.SelectedItem;
            var myModes = GameGraphics.Renderer.GetValidVideoModes();
            for (var i = 0; i < myModes.Count; i++)
            {
                if (resolution.Text == myModes[i])
                {
                    if (Globals.Database.TargetResolution != i) shouldReset = true;
                    Globals.Database.TargetResolution = i;
                }
            }
            if (Globals.Database.FullScreen != mFullscreen.IsChecked)
            {
                Globals.Database.FullScreen = mFullscreen.IsChecked;
                shouldReset = true;
            }
            var newFps = 0;
            if (mFpsList.SelectedItem.Text == Strings.Options.unlimitedfps)
            {
                newFps = -1;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Options.fps30)
            {
                newFps = 1;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Options.fps60)
            {
                newFps = 2;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Options.fps90)
            {
                newFps = 3;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Options.fps120)
            {
                newFps = 4;
            }

            if (newFps != Globals.Database.TargetFps)
            {
                shouldReset = true;
                Globals.Database.TargetFps = newFps;
            }
            Globals.Database.MusicVolume = (int) mMusicSlider.Value;
            Globals.Database.SoundVolume = (int) mSoundSlider.Value;
            GameControls.ActiveControls = mEdittingControls;
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
                mMainMenu.Show();
            }
        }
    }
}