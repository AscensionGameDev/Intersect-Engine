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
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Localization;
using Intersect.Utilities;

using static Intersect.Client.Framework.File_Management.GameContentManager;

namespace Intersect.Client.Interface.Shared
{
    public partial class SettingsWindow
    {
        // Parent Window.
        private readonly MainMenu mMainMenu;

        private readonly EscapeMenu mEscapeMenu;

        // Settings Window.
        private readonly Label mSettingsHeader;

        private readonly ImagePanel mSettingsPanel;

        private readonly Button mSettingsApplyBtn;

        private readonly Button mSettingsCancelBtn;

        // Settings Containers.
        private readonly ScrollControl mGameSettingsContainer;

        private readonly ScrollControl mVideoSettingsContainer;

        private readonly ScrollControl mAudioSettingsContainer;

        private readonly ScrollControl mKeybindingSettingsContainer;

        // Tabs.
        private readonly Button mGameSettingsTab;

        private readonly Button mVideoSettingsTab;

        private readonly Button mAudioSettingsTab;

        private readonly Button mKeybindingSettingsTab;

        // Game Settings - Overhead Information.
        private readonly Button mOverheadInformationSettings;

        private readonly Button mOverheadInfoSettingsHelper;

        private readonly LabeledCheckBox mFriendOverheadInfoCheckbox;

        private readonly LabeledCheckBox mGuildMemberOverheadInfoCheckbox;

        private readonly LabeledCheckBox mMyOverheadInfoCheckbox;

        private readonly LabeledCheckBox mNpcOverheadInfoCheckbox;

        private readonly LabeledCheckBox mPartyMemberOverheadInfoCheckbox;

        private readonly LabeledCheckBox mPlayerOverheadInfoCheckbox;

        // Game Settings - Interface.
        private readonly Button mInterfaceSettings;

        private readonly Button mInterfaceSettingsHelper;

        private readonly LabeledCheckBox mAutoCloseWindowsCheckbox;

        // Video Settings.
        private readonly ImagePanel mResolutionBackground;

        private readonly Label mResolutionLabel;

        private readonly ComboBox mResolutionList;

        private MenuItem mCustomResolutionMenuItem;

        private readonly ImagePanel mFpsBackground;

        private readonly Label mFpsLabel;

        private readonly ComboBox mFpsList;

        private readonly LabeledCheckBox mFullscreenCheckbox;

        private readonly LabeledCheckBox mLightingEnabledCheckbox;

        // Audio Settings.
        private readonly HorizontalSlider mMusicSlider;

        private int mPreviousMusicVolume;

        private readonly Label mMusicLabel;

        private readonly HorizontalSlider mSoundSlider;

        private int mPreviousSoundVolume;

        private readonly Label mSoundLabel;

        // Keybinding Settings.
        private Control mKeybindingEditControl;

        private Controls mKeybindingEditControls;

        private Button mKeybindingEditBtn;

        private readonly Button mKeybindingRestoreBtn;

        private long mKeybindingListeningTimer;

        private int mKeyEdit = -1;

        private readonly Dictionary<Control, Button[]> mKeybindingBtns = new Dictionary<Control, Button[]>();

        // Open Settings.
        private bool mReturnToMenu;

        // Initialize.
        public SettingsWindow(Base parent, MainMenu mainMenu, EscapeMenu escapeMenu)
        {
            // Assign References.
            mMainMenu = mainMenu;
            mEscapeMenu = escapeMenu;

            // Main Menu Window.
            mSettingsPanel = new ImagePanel(parent, "SettingsWindow") { IsHidden = true };
            Interface.InputBlockingElements.Add(mSettingsPanel);

            // Menu Header.
            mSettingsHeader = new Label(mSettingsPanel, "SettingsHeader");
            mSettingsHeader.SetText(Strings.Settings.Title);

            // Apply Button.
            mSettingsApplyBtn = new Button(mSettingsPanel, "SettingsApplyBtn");
            mSettingsApplyBtn.SetText(Strings.Settings.Apply);
            mSettingsApplyBtn.Clicked += SettingsApplyBtn_Clicked;

            // Cancel Button.
            mSettingsCancelBtn = new Button(mSettingsPanel, "SettingsCancelBtn");
            mSettingsCancelBtn.SetText(Strings.Settings.Cancel);
            mSettingsCancelBtn.Clicked += SettingsCancelBtn_Clicked;

            #region InitGameSettings

            // Init GameSettings Tab.
            mGameSettingsTab = new Button(mSettingsPanel, "GameSettingsTab");
            mGameSettingsTab.Text = Strings.Settings.GameSettingsTab;
            mGameSettingsTab.Clicked += GameSettingsTab_Clicked;

            // Game Settings Get Stored in the GameSettings Scroll Control.
            mGameSettingsContainer = new ScrollControl(mSettingsPanel, "GameSettingsContainer");
            mGameSettingsContainer.EnableScroll(false, true);

            // Game Settings - Overhead Information.
            mOverheadInformationSettings = new Button(mGameSettingsContainer, "OverheadInformationSettings");
            mOverheadInformationSettings.Text = Strings.Settings.OverheadInformationSettings;
            mOverheadInfoSettingsHelper = new Button(mGameSettingsContainer, "OverheadInfoSettingsHelper");
            mOverheadInfoSettingsHelper.SetToolTipText(Strings.Settings.OverheadInfoSettingsHelper);

            // Game Settings - Toggle for: Friends Overhead Information.
            mFriendOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "FriendOverheadInfoCheckbox");
            mFriendOverheadInfoCheckbox.Text = Strings.Settings.FriendOverheadInfo;

            // Game Settings - Toggle for: Guild Members Overhead Information.
            mGuildMemberOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "GuildMemberOverheadInfoCheckbox");
            mGuildMemberOverheadInfoCheckbox.Text = Strings.Settings.GuildMemberOverheadInfo;

            // Game Settings - Toggle for: My Overhead Information (Local Player).
            mMyOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "MyOverheadInfoCheckbox");
            mMyOverheadInfoCheckbox.Text = Strings.Settings.MyOverheadInfo;

            // Game Settings - Toggle for: NPCs Overhead Information.
            mNpcOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "NpcOverheadInfoCheckbox");
            mNpcOverheadInfoCheckbox.Text = Strings.Settings.NpcOverheadInfo;

            // Game Settings - Toggle for: Party Members Overhead Information.
            mPartyMemberOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "PartyMemberOverheadInfoCheckbox");
            mPartyMemberOverheadInfoCheckbox.Text = Strings.Settings.PartyMemberOverheadInfo;

            // Game Settings - Toggle for: Players Overhead Information.
            mPlayerOverheadInfoCheckbox = new LabeledCheckBox(mGameSettingsContainer, "PlayerOverheadInfoCheckbox");
            mPlayerOverheadInfoCheckbox.Text = Strings.Settings.PlayerOverheadInfo;

            // Game Settings - Interface.
            mInterfaceSettings = new Button(mGameSettingsContainer, "InterfaceSettings");
            mInterfaceSettings.Text = Strings.Settings.InterfaceSettings;
            mInterfaceSettingsHelper =
                new Button(mGameSettingsContainer, "InterfaceSettingsHelper");
            mInterfaceSettingsHelper.SetToolTipText(Strings.Settings.InterfaceSettingsHelper);

            // Game Settings - Toggle for: Interface Auto-close Windows.
            mAutoCloseWindowsCheckbox = new LabeledCheckBox(mGameSettingsContainer, "AutoCloseWindowsCheckbox");
            mAutoCloseWindowsCheckbox.Text = Strings.Settings.AutoCloseWindows;

            #endregion

            #region InitVideoSettings

            // Init VideoSettings Tab.
            mVideoSettingsTab = new Button(mSettingsPanel, "VideoSettingsTab");
            mVideoSettingsTab.Text = Strings.Settings.VideoSettingsTab;
            mVideoSettingsTab.Clicked += VideoSettingsTab_Clicked;

            // Video Settings Get Stored in the VideoSettings Scroll Control.
            mVideoSettingsContainer = new ScrollControl(mSettingsPanel, "VideoSettingsContainer");
            mVideoSettingsContainer.EnableScroll(false, false);

            // Video Settings - Resolution Background.
            mResolutionBackground = new ImagePanel(mVideoSettingsContainer, "ResolutionPanel");

            // Video Settings - Resolution Label.
            mResolutionLabel = new Label(mResolutionBackground, "ResolutionLabel");
            mResolutionLabel.SetText(Strings.Settings.Resolution);

            // Video Settings - Resolution List.
            mResolutionList = new ComboBox(mResolutionBackground, "ResolutionCombobox");
            var myModes = Graphics.Renderer.GetValidVideoModes();
            myModes?.ForEach(
                t =>
                {
                    var item = mResolutionList.AddItem(t);
                    item.Alignment = Pos.Left;
                }
            );

            // Video Settings - FPS Background.
            mFpsBackground = new ImagePanel(mVideoSettingsContainer, "FPSPanel");

            // Video Settings - FPS Label.
            mFpsLabel = new Label(mFpsBackground, "FPSLabel");
            mFpsLabel.SetText(Strings.Settings.TargetFps);

            // Video Settings - FPS List.
            mFpsList = new ComboBox(mFpsBackground, "FPSCombobox");
            mFpsList.AddItem(Strings.Settings.Vsync);
            mFpsList.AddItem(Strings.Settings.Fps30);
            mFpsList.AddItem(Strings.Settings.Fps60);
            mFpsList.AddItem(Strings.Settings.Fps90);
            mFpsList.AddItem(Strings.Settings.Fps120);
            mFpsList.AddItem(Strings.Settings.UnlimitedFps);

            // Video Settings - Fullscreen Checkbox.
            mFullscreenCheckbox = new LabeledCheckBox(mVideoSettingsContainer, "FullscreenCheckbox")
            {
                Text = Strings.Settings.Fullscreen
            };

            // Video Settings - Enable Lighting Checkbox
            mLightingEnabledCheckbox = new LabeledCheckBox(mVideoSettingsContainer, "EnableLightingCheckbox")
            {
                Text = Strings.Settings.EnableLighting
            };

            #endregion

            #region InitAudioSettings

            // Init AudioSettingsTab.
            mAudioSettingsTab = new Button(mSettingsPanel, "AudioSettingsTab");
            mAudioSettingsTab.Text = Strings.Settings.AudioSettingsTab;
            mAudioSettingsTab.Clicked += AudioSettingsTab_Clicked;

            // Audio Settings Get Stored in the AudioSettings Scroll Control.
            mAudioSettingsContainer = new ScrollControl(mSettingsPanel, "AudioSettingsContainer");
            mAudioSettingsContainer.EnableScroll(false, false);

            // Audio Settings - Sound Label
            mSoundLabel = new Label(mAudioSettingsContainer, "SoundLabel");
            mSoundLabel.SetText(Strings.Settings.SoundVolume.ToString(100));

            // Audio Settings - Sound Slider
            mSoundSlider = new HorizontalSlider(mAudioSettingsContainer, "SoundSlider");
            mSoundSlider.Min = 0;
            mSoundSlider.Max = 100;
            mSoundSlider.ValueChanged += SoundSlider_ValueChanged;

            // Audio Settings - Music Label
            mMusicLabel = new Label(mAudioSettingsContainer, "MusicLabel");
            mMusicLabel.SetText(Strings.Settings.MusicVolume.ToString(100));

            // Audio Settings - Music Slider
            mMusicSlider = new HorizontalSlider(mAudioSettingsContainer, "MusicSlider");
            mMusicSlider.Min = 0;
            mMusicSlider.Max = 100;
            mMusicSlider.ValueChanged += MusicSlider_ValueChanged;

            #endregion

            #region InitKeybindingSettings

            // Init KeybindingsSettings Tab.
            mKeybindingSettingsTab = new Button(mSettingsPanel, "KeybindingSettingsTab");
            mKeybindingSettingsTab.Text = Strings.Settings.KeyBindingSettingsTab;
            mKeybindingSettingsTab.Clicked += KeybindingSettingsTab_Clicked;

            // KeybindingSettings Get Stored in the KeybindingSettings Scroll Control
            mKeybindingSettingsContainer = new ScrollControl(mSettingsPanel, "KeybindingSettingsContainer");
            mKeybindingSettingsContainer.EnableScroll(false, true);

            // Keybinding Settings - Restore Default Keys Button.
            mKeybindingRestoreBtn = new Button(mSettingsPanel, "KeybindingsRestoreBtn");
            mKeybindingRestoreBtn.Text = Strings.Settings.Restore;
            mKeybindingRestoreBtn.Clicked += KeybindingsRestoreBtn_Clicked;

            // Keybinding Settings - Controls 
            var row = 0;
            var defaultFont = GameContentManager.Current?.GetFont("sourcesansproblack", 16);
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var offset = row * 32;
                var name = Enum.GetName(typeof(Control), control)?.ToLower();

                var label = new Label(mKeybindingSettingsContainer, $"Control{Enum.GetName(typeof(Control), control)}Label");
                label.Text = Strings.Controls.controldict[name];
                label.AutoSizeToContents = true;
                label.Font = defaultFont;
                label.SetBounds(8, 8 + offset, 0, 24);
                label.SetTextColor(new Color(255, 255, 255, 255), Label.ControlState.Normal);

                var key1 = new Button(mKeybindingSettingsContainer, $"Control{Enum.GetName(typeof(Control), control)}Button1");
                key1.Text = "";
                key1.AutoSizeToContents = false;
                key1.UserData = new KeyValuePair<Control, int>(control, 0);
                key1.Font = defaultFont;

                key1.Clicked += Key_Clicked;

                var key2 = new Button(mKeybindingSettingsContainer, $"Control{Enum.GetName(typeof(Control), control)}Button2")
                {
                    Text = "",
                    AutoSizeToContents = false,
                    UserData = new KeyValuePair<Control, int>(control, 1),
                    Font = defaultFont
                };

                key2.Clicked += Key_Clicked;

                mKeybindingBtns.Add(control, new[] { key1, key2 });

                row++;
            }

            Input.KeyDown += OnKeyDown;
            Input.MouseDown += OnKeyDown;

            #endregion

            mSettingsPanel.LoadJsonUi(UI.Shared, Graphics.Renderer.GetResolutionString());
        }

        private void GameSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Determine if GameSettingsContainer is currently being shown or not.
            if (!mGameSettingsContainer.IsVisible)
            {
                // Disable the GameSettingsTab to fake it being selected visually.
                mGameSettingsTab.Disable();
                mVideoSettingsTab.Enable();
                mAudioSettingsTab.Enable();
                mKeybindingSettingsTab.Enable();

                // Containers.
                mGameSettingsContainer.Show();
                mVideoSettingsContainer.Hide();
                mAudioSettingsContainer.Hide();
                mKeybindingSettingsContainer.Hide();

                // Restore Default KeybindingSettings Button.
                mKeybindingRestoreBtn.Hide();
            }
        }

        private void VideoSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Determine if VideoSettingsContainer is currently being shown or not.
            if (!mVideoSettingsContainer.IsVisible)
            {
                // Disable the VideoSettingsTab to fake it being selected visually.
                mGameSettingsTab.Enable();
                mVideoSettingsTab.Disable();
                mAudioSettingsTab.Enable();
                mKeybindingSettingsTab.Enable();

                // Containers.
                mGameSettingsContainer.Hide();
                mVideoSettingsContainer.Show();
                mAudioSettingsContainer.Hide();
                mKeybindingSettingsContainer.Hide();

                // Restore Default KeybindingSettings Button.
                mKeybindingRestoreBtn.Hide();
            }
        }

        private void AudioSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Determine if AudioSettingsContainer is currently being shown or not.
            if (!mAudioSettingsContainer.IsVisible)
            {
                // Disable the AudioSettingsTab to fake it being selected visually.
                mGameSettingsTab.Enable();
                mVideoSettingsTab.Enable();
                mAudioSettingsTab.Disable();
                mKeybindingSettingsTab.Enable();

                // Containers.
                mGameSettingsContainer.Hide();
                mVideoSettingsContainer.Hide();
                mAudioSettingsContainer.Show();
                mKeybindingSettingsContainer.Hide();

                // Restore Default KeybindingSettings Button.
                mKeybindingRestoreBtn.Hide();
            }
        }

        private void KeybindingSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Determine if controls are currently being shown or not.
            if (!mKeybindingSettingsContainer.IsVisible)
            {
                // Disable the KeybindingSettingsTab to fake it being selected visually.
                mGameSettingsTab.Enable();
                mVideoSettingsTab.Enable();
                mAudioSettingsTab.Enable();
                mKeybindingSettingsTab.Disable();

                // Containers.
                mGameSettingsContainer.Hide();
                mVideoSettingsContainer.Hide();
                mAudioSettingsContainer.Hide();
                mKeybindingSettingsContainer.Show();

                // Restore Default KeybindingSettings Button.
                mKeybindingRestoreBtn.Show();

                // KeybindingBtns.
                foreach (Control control in Enum.GetValues(typeof(Control)))
                {
                    var controlMapping = mKeybindingEditControls.ControlMapping[control];
                    for (var bindingIndex = 0; bindingIndex < controlMapping.Bindings.Count; bindingIndex++)
                    {
                        var binding = controlMapping.Bindings[bindingIndex];
                        mKeybindingBtns[control][bindingIndex].Text = Strings.Keys.FormatKeyName(binding.Modifier, binding.Key);
                    }
                }
            }
        }

        private void LoadSettingsWindow()
        {
            // Settings Window Title.
            mSettingsHeader.SetText(Strings.Settings.Title);

            // Containers.
            mGameSettingsContainer.Show();
            mVideoSettingsContainer.Hide();
            mAudioSettingsContainer.Hide();
            mKeybindingSettingsContainer.Hide();

            // Tabs.
            mGameSettingsTab.Show();
            mVideoSettingsTab.Show();
            mAudioSettingsTab.Show();
            mKeybindingSettingsTab.Show();

            // Disable the GameSettingsTab to fake it being selected visually by default.
            mGameSettingsTab.Disable();
            mVideoSettingsTab.Enable();
            mAudioSettingsTab.Enable();
            mKeybindingSettingsTab.Enable();

            // Buttons.
            mSettingsApplyBtn.Show();
            mSettingsCancelBtn.Show();
            mKeybindingRestoreBtn.Hide();
        }

        private void OnKeyDown(Keys modifier, Keys key)
        {
            if (mKeybindingEditBtn != null)
            {
                mKeybindingEditControls.UpdateControl(mKeybindingEditControl, mKeyEdit, modifier, key);
                mKeybindingEditBtn.Text = Strings.Keys.FormatKeyName(modifier, key);

                if (key != Keys.None)
                {
                    foreach (var control in mKeybindingEditControls.ControlMapping)
                    {
                        if (control.Key == mKeybindingEditControl)
                        {
                            continue;
                        }

                        var bindings = control.Value.Bindings;
                        for (var bindingIndex = 0; bindingIndex < bindings.Count; bindingIndex++)
                        {
                            var binding = bindings[bindingIndex];

                            if (binding.Modifier == modifier && binding.Key == key)
                            {
                                // Remove this mapping.
                                mKeybindingEditControls.UpdateControl(control.Key, bindingIndex, Keys.None, Keys.None);

                                // Update UI.
                                mKeybindingBtns[control.Key][bindingIndex].Text = Strings.Keys.keydict[Enum.GetName(typeof(Keys), Keys.None).ToLower()];
                            }
                        }
                    }
                }

                mKeybindingEditBtn.PlayHoverSound();
                mKeybindingEditBtn = null;
                Interface.GwenInput.HandleInput = true;
            }
        }

        // Methods.
        public void Update()
        {
            if (mSettingsPanel.IsVisible &&
                mKeybindingEditBtn != null &&
                mKeybindingListeningTimer < Timing.Global.Milliseconds)
            {
                OnKeyDown(Keys.None, Keys.None);
            }
        }

        public void Show(bool returnToMenu = false)
        {
            // Take over all input when we're in-game.
            if (Globals.GameState == GameStates.InGame)
            {
                mSettingsPanel.MakeModal(true);
            }

            mKeybindingEditControls = new Controls(Controls.ActiveControls);
            if (Graphics.Renderer.GetValidVideoModes().Count > 0)
            {
                string resolutionLabel;
                if (Graphics.Renderer.HasOverrideResolution)
                {
                    resolutionLabel = Strings.Settings.ResolutionCustom;

                    if (mCustomResolutionMenuItem == null)
                    {
                        mCustomResolutionMenuItem = mResolutionList.AddItem(Strings.Settings.ResolutionCustom);
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
                case -1: // Unlimited.
                    mFpsList.SelectByText(Strings.Settings.UnlimitedFps);

                    break;
                case 0: // Vertical Sync.
                    mFpsList.SelectByText(Strings.Settings.Vsync);

                    break;
                case 1: // 30 Frames per second.
                    mFpsList.SelectByText(Strings.Settings.Fps30);

                    break;
                case 2: // 60 Frames per second.
                    mFpsList.SelectByText(Strings.Settings.Fps60);

                    break;
                case 3: // 90 Frames per second.
                    mFpsList.SelectByText(Strings.Settings.Fps90);

                    break;
                case 4: // 120 Frames per second.
                    mFpsList.SelectByText(Strings.Settings.Fps120);

                    break;
                default:
                    mFpsList.SelectByText(Strings.Settings.Vsync);

                    break;
            }

            // Game Settings.
            mFriendOverheadInfoCheckbox.IsChecked = Globals.Database.FriendOverheadInfo;
            mGuildMemberOverheadInfoCheckbox.IsChecked = Globals.Database.GuildMemberOverheadInfo;
            mMyOverheadInfoCheckbox.IsChecked = Globals.Database.MyOverheadInfo;
            mNpcOverheadInfoCheckbox.IsChecked = Globals.Database.NpcOverheadInfo;
            mPartyMemberOverheadInfoCheckbox.IsChecked = Globals.Database.PartyMemberOverheadInfo;
            mPlayerOverheadInfoCheckbox.IsChecked = Globals.Database.PlayerOverheadInfo;

            // Video Settings.
            mAutoCloseWindowsCheckbox.IsChecked = Globals.Database.HideOthersOnWindowOpen;
            mFullscreenCheckbox.IsChecked = Globals.Database.FullScreen;
            mLightingEnabledCheckbox.IsChecked = Globals.Database.EnableLighting;

            // Audio Settings.
            mPreviousMusicVolume = Globals.Database.MusicVolume;
            mPreviousSoundVolume = Globals.Database.SoundVolume;
            mMusicSlider.Value = Globals.Database.MusicVolume;
            mSoundSlider.Value = Globals.Database.SoundVolume;
            mMusicLabel.Text = Strings.Settings.MusicVolume.ToString((int)mMusicSlider.Value);
            mSoundLabel.Text = Strings.Settings.SoundVolume.ToString((int)mSoundSlider.Value);

            // Settings Window is not hidden anymore.
            mSettingsPanel.Show();

            // Load every GUI element to their default state when showing up the settings window (pressed tabs, containers, etc.)
            LoadSettingsWindow();

            // Set up whether we're supposed to return to the previous menu.
            mReturnToMenu = returnToMenu;
        }

        public bool IsVisible() => !mSettingsPanel.IsHidden;

        public void Hide()
        {
            // Hide the current window.
            mSettingsPanel.Hide();
            mSettingsPanel.RemoveModal();

            // Return to our previous menus (or not) depending on gamestate and the method we'd been opened.
            if (mReturnToMenu)
            {
                switch (Globals.GameState)
                {
                    case GameStates.Menu:
                        mMainMenu?.Show();
                        break;

                    case GameStates.InGame:
                        mEscapeMenu?.Show();
                        break;

                    default:
                        throw new NotImplementedException();
                }

                mReturnToMenu = false;
            }
        }

        // Input Handlers
        private void MusicSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            mMusicLabel.Text = Strings.Settings.MusicVolume.ToString((int)mMusicSlider.Value);
            Globals.Database.MusicVolume = (int)mMusicSlider.Value;
            Audio.UpdateGlobalVolume();
        }

        private void SoundSlider_ValueChanged(Base sender, EventArgs arguments)
        {
            mSoundLabel.Text = Strings.Settings.SoundVolume.ToString((int)mSoundSlider.Value);
            Globals.Database.SoundVolume = (int)mSoundSlider.Value;
            Audio.UpdateGlobalVolume();
        }

        private void Key_Clicked(Base sender, ClickedEventArgs arguments)
        {
            EditKeyPressed((Button)sender);
        }

        private void EditKeyPressed(Button sender)
        {
            if (mKeybindingEditBtn == null)
            {
                sender.Text = Strings.Controls.listening;
                mKeyEdit = ((KeyValuePair<Control, int>)sender.UserData).Value;
                mKeybindingEditControl = ((KeyValuePair<Control, int>)sender.UserData).Key;
                mKeybindingEditBtn = sender;
                Interface.GwenInput.HandleInput = false;
                mKeybindingListeningTimer = Timing.Global.Milliseconds + 3000;
            }
        }

        private void KeybindingsRestoreBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mKeybindingEditControls.ResetDefaults();
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var controlMapping = mKeybindingEditControls.ControlMapping[control];
                for (var bindingIndex = 0; bindingIndex < controlMapping.Bindings.Count; bindingIndex++)
                {
                    var binding = controlMapping.Bindings[bindingIndex];
                    mKeybindingBtns[control][bindingIndex].Text = Strings.Keys.FormatKeyName(binding.Modifier, binding.Key);
                }
            }
        }

        private void SettingsApplyBtn_Clicked(Base sender, ClickedEventArgs arguments)
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

            Globals.Database.HideOthersOnWindowOpen = mAutoCloseWindowsCheckbox.IsChecked;
            if (Globals.Database.FullScreen != mFullscreenCheckbox.IsChecked)
            {
                Globals.Database.FullScreen = mFullscreenCheckbox.IsChecked;
                shouldReset = true;
            }

            var newFps = 0;
            if (mFpsList.SelectedItem.Text == Strings.Settings.UnlimitedFps)
            {
                newFps = -1;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Settings.Fps30)
            {
                newFps = 1;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Settings.Fps60)
            {
                newFps = 2;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Settings.Fps90)
            {
                newFps = 3;
            }
            else if (mFpsList.SelectedItem.Text == Strings.Settings.Fps120)
            {
                newFps = 4;
            }

            if (newFps != Globals.Database.TargetFps)
            {
                shouldReset = true;
                Globals.Database.TargetFps = newFps;
            }

            Globals.Database.EnableLighting = mLightingEnabledCheckbox.IsChecked;

            if (Globals.Database.FriendOverheadInfo != mFriendOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.FriendOverheadInfo = mFriendOverheadInfoCheckbox.IsChecked;
            }

            if (Globals.Database.GuildMemberOverheadInfo != mGuildMemberOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.GuildMemberOverheadInfo = mGuildMemberOverheadInfoCheckbox.IsChecked;
            }

            if (Globals.Database.MyOverheadInfo != mMyOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.MyOverheadInfo = mMyOverheadInfoCheckbox.IsChecked;
            }

            if (Globals.Database.NpcOverheadInfo != mNpcOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.NpcOverheadInfo = mNpcOverheadInfoCheckbox.IsChecked;
            }

            if (Globals.Database.PartyMemberOverheadInfo != mPartyMemberOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.PartyMemberOverheadInfo = mPartyMemberOverheadInfoCheckbox.IsChecked;
            }

            if (Globals.Database.PlayerOverheadInfo != mPlayerOverheadInfoCheckbox.IsChecked)
            {
                Globals.Database.PlayerOverheadInfo = mPlayerOverheadInfoCheckbox.IsChecked;
            }

            // Save Settings.
            Globals.Database.MusicVolume = (int)mMusicSlider.Value;
            Globals.Database.SoundVolume = (int)mSoundSlider.Value;
            Audio.UpdateGlobalVolume();
            Controls.ActiveControls = mKeybindingEditControls;
            Controls.ActiveControls.Save();
            Globals.Database.SavePreferences();

            if (shouldReset)
            {
                mCustomResolutionMenuItem?.Hide();
                Graphics.Renderer.OverrideResolution = Resolution.Empty;
                Graphics.Renderer.Init();
            }

            // Hide our current window.
            Hide();
        }

        private void SettingsCancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Update previously saved values in order to discard changes.
            Globals.Database.MusicVolume = mPreviousMusicVolume;
            Globals.Database.SoundVolume = mPreviousSoundVolume;
            Audio.UpdateGlobalVolume();
            mKeybindingEditControls = new Controls(Controls.ActiveControls);

            // Hide our current window.
            Hide();
        }
    }
}
