using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Shared
{

    public class OptionsWindow
    {

        private Button mApplyBtn;

        private Button mApplyKeybindingsButton;

        private LabeledCheckBox mAutocloseWindowsCheckbox;

        private Button mBackBtn;

        private Button mCancelKeybindingsButton;

        private ScrollControl mControlsContainer;

        //Keybindings
        private Button mEditKeybindingsBtn;

        private Button mEdittingButton;

        private Control mEdittingControl;

        private Controls mEdittingControls;

        private int mEdittingKey = -1;

        private ImagePanel mFpsBackground;

        private Label mFpsLabel;

        private ComboBox mFpsList;

        private LabeledCheckBox mFullscreen;

        //Parent Windows
        private bool mGameWindow = false;

        private Dictionary<Control, Button[]> mKeyButtons = new Dictionary<Control, Button[]>();

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

        private Button mRestoreKeybindingsButton;

        private Label mSoundLabel;

        private HorizontalSlider mSoundSlider;

        private MenuItem mCustomResolutionMenuItem;

        //Init
        public OptionsWindow(Canvas parent, MainMenu mainMenu, ImagePanel parentPanel)
        {
            //Assign References
            mMainMenu = mainMenu;

            //Main Menu Window
            mOptionsPanel = new ImagePanel(parent, "OptionsWindow") {IsHidden = true};
            Interface.InputBlockingElements.Add(mOptionsPanel);

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
            var myModes = Graphics.Renderer.GetValidVideoModes();
            myModes?.ForEach(
                t =>
                {
                    var item = mResolutionList.AddItem(t);
                    item.Alignment = Pos.Left;
                }
            );

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
            mFullscreen = new LabeledCheckBox(mOptionsContainer, "FullscreenCheckbox")
            {
                Text = Strings.Options.fullscreen
            };

            mAutocloseWindowsCheckbox = new LabeledCheckBox(mOptionsContainer, "AutocloseWindowsCheckbox")
            {
                Text = Strings.Options.AutocloseWindows
            };

            mEditKeybindingsBtn = new Button(mOptionsContainer, "KeybindingsButton") {Text = Strings.Controls.edit};
            mEditKeybindingsBtn.Clicked += EditKeybindingsButton_Clicked;

            //Options - Sound Label
            mSoundLabel = new Label(mOptionsContainer, "SoundLabel");
            mSoundLabel.SetText(Strings.Options.soundvolume.ToString(100));

            //Options - Sound Slider
            mSoundSlider = new HorizontalSlider(mOptionsContainer, "SoundSlider");
            mSoundSlider.Min = 0;
            mSoundSlider.Max = 100;
            mSoundSlider.ValueChanged += _soundSlider_ValueChanged;

            //Options - Music Label
            mMusicLabel = new Label(mOptionsContainer, "MusicLabel");
            mMusicLabel.SetText(Strings.Options.musicvolume.ToString(100));

            //Options - Music Slider
            mMusicSlider = new HorizontalSlider(mOptionsContainer, "MusicSlider");
            mMusicSlider.Min = 0;
            mMusicSlider.Max = 100;
            mMusicSlider.ValueChanged += _musicSlider_ValueChanged;

            //Controls Get Stored in the Controls Scroll Control
            mControlsContainer = new ScrollControl(mOptionsPanel, "ControlsContainer");
            mControlsContainer.EnableScroll(false, true);
            mControlsContainer.Hide();

            mApplyKeybindingsButton = new Button(mOptionsPanel, "ExitControlsButton");
            mApplyKeybindingsButton.Text = Strings.Options.apply;
            mApplyKeybindingsButton.Hide();
            mApplyKeybindingsButton.Clicked += ApplyKeybindingsButton_Clicked;

            mCancelKeybindingsButton = new Button(mOptionsPanel, "CancelControlsButton");
            mCancelKeybindingsButton.Text = Strings.Options.back;
            mCancelKeybindingsButton.Hide();
            mCancelKeybindingsButton.Clicked += CancelKeybindingsButton_Clicked;

            mRestoreKeybindingsButton = new Button(mOptionsPanel, "RestoreControlsButton");
            mRestoreKeybindingsButton.Text = Strings.Options.restore;
            mRestoreKeybindingsButton.Hide();
            mRestoreKeybindingsButton.Clicked += RestoreKeybindingsButton_Clicked;

            var row = 0;
            var defaultFont = GameContentManager.Current?.GetFont("sourcesansproblack", 16);
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var offset = row * 32;
                var name = Enum.GetName(typeof(Control), control)?.ToLower();

                var label = new Label(mControlsContainer, $"Control{Enum.GetName(typeof(Control), control)}Label")
                {
                    Text = Strings.Controls.controldict[name],
                    AutoSizeToContents = true,
                    Font = defaultFont
                };

                label.SetBounds(8, 8 + offset, 0, 24);
                label.SetTextColor(new Color(255, 255, 255, 255), Label.ControlState.Normal);

                var key1 = new Button(mControlsContainer, $"Control{Enum.GetName(typeof(Control), control)}Button1")
                {
                    Text = "",
                    AutoSizeToContents = false,
                    UserData = new KeyValuePair<Control, int>(control, 1),
                    Font = defaultFont
                };

                key1.Clicked += Key_Clicked;

                var key2 = new Button(mControlsContainer, $"Control{Enum.GetName(typeof(Control), control)}Button2")
                {
                    Text = "",
                    AutoSizeToContents = false,
                    UserData = new KeyValuePair<Control, int>(control, 2),
                    Font = defaultFont
                };

                key2.Clicked += Key_Clicked;

                mKeyButtons.Add(control, new[] {key1, key2});

                row++;
            }

            //Options - Apply Button
            mApplyBtn = new Button(mOptionsContainer, "ApplyButton");
            mApplyBtn.SetText(Strings.Options.apply);
            mApplyBtn.Clicked += ApplyBtn_Clicked;

            //Options - Back Button
            mBackBtn = new Button(mOptionsContainer, "CancelButton");
            mBackBtn.SetText(Strings.Options.cancel);
            mBackBtn.Clicked += BackBtn_Clicked;

            Input.KeyDown += OnKeyDown;
            Input.MouseDown += OnKeyDown;

            mOptionsPanel.LoadJsonUi(
                mainMenu == null ? GameContentManager.UI.InGame : GameContentManager.UI.Menu,
                Graphics.Renderer.GetResolutionString()
            );

            CloseKeybindings();
        }

        private void Key_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button) sender);
        }

        private void EditKeyPressed(Button sender)
        {
            if (mEdittingButton == null)
            {
                sender.Text = Strings.Controls.listening;
                mEdittingKey = ((KeyValuePair<Control, int>)sender.UserData).Value;
                mEdittingControl = ((KeyValuePair<Control, int>) sender.UserData).Key;
                mEdittingButton = sender;
                Interface.GwenInput.HandleInput = false;
                mListeningTimer = Globals.System.GetTimeMs() + 3000;
            }
        }

        private void EditKeybindingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Determine if controls are currently being shown or not
            if (!mControlsContainer.IsVisible)
            {
                mControlsContainer.Show();
                mOptionsContainer.Hide();
                mOptionsHeader.SetText(Strings.Controls.title);
                mApplyKeybindingsButton.Show();
                mCancelKeybindingsButton.Show();
                mRestoreKeybindingsButton.Show();
                foreach (Control control in Enum.GetValues(typeof(Control)))
                {
                    mKeyButtons[control][0].Text =
                        Strings.Keys.keydict[
                            Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key1).ToLower()];

                    mKeyButtons[control][1].Text =
                        Strings.Keys.keydict[
                            Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key2).ToLower()];
                }
            }
        }

        private void ApplyKeybindingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Controls.ActiveControls = mEdittingControls;
            Controls.ActiveControls.Save();
            CloseKeybindings();
        }

        private void CancelKeybindingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mEdittingControls = new Controls(Controls.ActiveControls);
            CloseKeybindings();
        }

        private void RestoreKeybindingsButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mEdittingControls.ResetDefaults();
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                mKeyButtons[control][0].Text =
                    Strings.Keys.keydict[
                        Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key1).ToLower()];

                mKeyButtons[control][1].Text =
                    Strings.Keys.keydict[
                        Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[control].Key2).ToLower()];
            }
        }

        private void CloseKeybindings()
        {
            mControlsContainer.Hide();
            mOptionsContainer.Show();
            mOptionsHeader.SetText(Strings.Options.title);
            mApplyKeybindingsButton.Hide();
            mCancelKeybindingsButton.Hide();
            mRestoreKeybindingsButton.Hide();
        }

        private void OnKeyDown(Keys key)
        {
            if (mEdittingButton != null)
            {
                mEdittingControls.UpdateControl(mEdittingControl, mEdittingKey, key);
                if (mEdittingKey == 1)
                {
                    mEdittingButton.Text =
                        Strings.Keys.keydict[
                            Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[mEdittingControl].Key1)
                                .ToLower()];
                }
                else
                {
                    mEdittingButton.Text =
                        Strings.Keys.keydict[
                            Enum.GetName(typeof(Keys), mEdittingControls.ControlMapping[mEdittingControl].Key2)
                                .ToLower()];
                }

                if (key != Keys.None) {
                    foreach (var control in mEdittingControls.ControlMapping)
                    {
                        if (control.Key != mEdittingControl)
                        {
                            if (control.Value.Key1 == key)
                            {
                                //Remove this mapping
                                mEdittingControls.UpdateControl(control.Key, 1, Keys.None);

                                //Update UI
                                mKeyButtons[control.Key][0].Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), Keys.None).ToLower()];
                            }
                            if (control.Value.Key2 == key)
                            {
                                //Remove this mapping
                                mEdittingControls.UpdateControl(control.Key, 2, Keys.None);

                                //Update UI
                                mKeyButtons[control.Key][1].Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), Keys.None).ToLower()];
                            }
                        }
                    }
                }

                mEdittingButton.PlayHoverSound();
                mEdittingButton = null;
                Interface.GwenInput.HandleInput = true;
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
            mEdittingControls = new Controls(Controls.ActiveControls);
            if (Graphics.Renderer.GetValidVideoModes().Count > 0)
            {
                string resolutionLabel;
                if (Graphics.Renderer.HasOverrideResolution)
                {
                    resolutionLabel = Strings.Options.ResolutionCustom;

                    if (mCustomResolutionMenuItem == null)
                    {
                        mCustomResolutionMenuItem = mResolutionList.AddItem(Strings.Options.ResolutionCustom);
                    }

                    mCustomResolutionMenuItem.Show();
                }
                else
                {
                    resolutionLabel = Graphics.Renderer.GetValidVideoModes()[Globals.Database.TargetResolution];
                }

                mResolutionList.SelectByText(resolutionLabel);
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

            mAutocloseWindowsCheckbox.IsChecked = Globals.Database.HideOthersOnWindowOpen;
            mFullscreen.IsChecked = Globals.Database.FullScreen;
            mMusicSlider.Value = Globals.Database.MusicVolume;
            mSoundSlider.Value = Globals.Database.SoundVolume;
            mMusicLabel.Text = Strings.Options.musicvolume.ToString((int) mMusicSlider.Value);
            mSoundLabel.Text = Strings.Options.soundvolume.ToString((int) mSoundSlider.Value);
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
            Audio.UpdateGlobalVolume();
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
            mMusicLabel.Text = Strings.Options.musicvolume.ToString((int) mMusicSlider.Value);
            Globals.Database.MusicVolume = (int) mMusicSlider.Value;
            Audio.UpdateGlobalVolume();
        }

        void _soundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            mSoundLabel.Text = Strings.Options.soundvolume.ToString((int) mSoundSlider.Value);
            Globals.Database.SoundVolume = (int) mSoundSlider.Value;
            Audio.UpdateGlobalVolume();
        }

        void ApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var shouldReset = false;
            var resolution = mResolutionList.SelectedItem;
            var validVideoModes = Graphics.Renderer.GetValidVideoModes();
            var targetResolution = validVideoModes?.FindIndex(videoMode => string.Equals(videoMode, resolution.Text)) ?? -1;
            if (targetResolution > -1)
            {
                shouldReset = Globals.Database.TargetResolution != targetResolution || Graphics.Renderer.HasOverrideResolution;
                Globals.Database.TargetResolution = targetResolution;
            }

            Globals.Database.HideOthersOnWindowOpen = mAutocloseWindowsCheckbox.IsChecked;
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
            Audio.UpdateGlobalVolume();
            Globals.Database.SavePreferences();
            if (shouldReset)
            {
                mCustomResolutionMenuItem?.Hide();
                Graphics.Renderer.OverrideResolution = Resolution.Empty;
                Graphics.Renderer.Init();
            }

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
