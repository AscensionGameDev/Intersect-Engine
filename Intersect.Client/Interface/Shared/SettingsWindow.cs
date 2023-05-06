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

        // Game Settings - Interface.
        private readonly ScrollControl mInterfaceSettings;

        private readonly LabeledCheckBox mAutoCloseWindowsCheckbox;

        private readonly LabeledCheckBox mAutoToggleChatLogCheckbox;

        private readonly LabeledCheckBox mShowExperienceAsPercentageCheckbox;

        private readonly LabeledCheckBox mShowHealthAsPercentageCheckbox;

        private readonly LabeledCheckBox mShowManaAsPercentageCheckbox;

        // Game Settings - Information.
        private readonly ScrollControl mInformationSettings;

        private readonly LabeledCheckBox mFriendOverheadInfoCheckbox;

        private readonly LabeledCheckBox mGuildMemberOverheadInfoCheckbox;

        private readonly LabeledCheckBox mMyOverheadInfoCheckbox;

        private readonly LabeledCheckBox mNpcOverheadInfoCheckbox;

        private readonly LabeledCheckBox mPartyMemberOverheadInfoCheckbox;

        private readonly LabeledCheckBox mPlayerOverheadInfoCheckbox;

        private readonly LabeledCheckBox mFriendOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mGuildMemberOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mMyOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mNpcOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mPartyMemberOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mPlayerOverheadHpBarCheckbox;

        private readonly LabeledCheckBox mTypewriterCheckbox;

        // Game Settings - Targeting.
        private readonly ScrollControl mTargetingSettings;

        private readonly LabeledCheckBox mStickyTarget;

        private readonly LabeledCheckBox mAutoTurnToTarget;

        // Video Settings.
        private readonly ComboBox mResolutionList;

        private MenuItem mCustomResolutionMenuItem;

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

            // Game Settings are stored in the GameSettings Scroll Control.
            mGameSettingsContainer = new ScrollControl(mSettingsPanel, "GameSettingsContainer");
            mGameSettingsContainer.EnableScroll(false, false);

            // Game Settings subcategories are stored in the GameSettings List.
            var gameSettingsList = new ListBox(mGameSettingsContainer, "GameSettingsList");
            gameSettingsList.AddRow(Strings.Settings.InterfaceSettings);
            gameSettingsList.AddRow(Strings.Settings.InformationSettings);
            gameSettingsList.AddRow(Strings.Settings.TargetingSettings);
            gameSettingsList.EnableScroll(false, true);
            gameSettingsList.SelectedRowIndex = 0;
            gameSettingsList[0].Clicked += InterfaceSettings_Clicked;
            gameSettingsList[1].Clicked += InformationSettings_Clicked;
            gameSettingsList[2].Clicked += TargetingSettings_Clicked;

            // Game Settings - Interface.
            mInterfaceSettings = new ScrollControl(mGameSettingsContainer, "InterfaceSettings");
            mInterfaceSettings.EnableScroll(false, true);

            // Game Settings - Interface: Auto-close Windows.
            mAutoCloseWindowsCheckbox = new LabeledCheckBox(mInterfaceSettings, "AutoCloseWindowsCheckbox");
            mAutoCloseWindowsCheckbox.Text = Strings.Settings.AutoCloseWindows;

            // Game Settings - Interface: Auto-toggle chat log visibility.
            mAutoToggleChatLogCheckbox = new LabeledCheckBox(mInterfaceSettings, "AutoToggleChatLogCheckbox");
            mAutoToggleChatLogCheckbox.Text = Strings.Settings.AutoToggleChatLog;

            // Game Settings - Interface: Show EXP/HP/MP as Percentage.
            mShowExperienceAsPercentageCheckbox =
                new LabeledCheckBox(mInterfaceSettings, "ShowExperienceAsPercentageCheckbox");
            mShowExperienceAsPercentageCheckbox.Text = Strings.Settings.ShowExperienceAsPercentage;
            mShowHealthAsPercentageCheckbox = new LabeledCheckBox(mInterfaceSettings, "ShowHealthAsPercentageCheckbox");
            mShowHealthAsPercentageCheckbox.Text = Strings.Settings.ShowHealthAsPercentage;
            mShowManaAsPercentageCheckbox = new LabeledCheckBox(mInterfaceSettings, "ShowManaAsPercentageCheckbox");
            mShowManaAsPercentageCheckbox.Text = Strings.Settings.ShowManaAsPercentage;

            // Game Settings - Information.
            mInformationSettings = new ScrollControl(mGameSettingsContainer, "InformationSettings");
            mInformationSettings.EnableScroll(false, true);

            // Game Settings - Information: Friends.
            mFriendOverheadInfoCheckbox =
                new LabeledCheckBox(mInformationSettings, "FriendOverheadInfoCheckbox");
            mFriendOverheadInfoCheckbox.Text = Strings.Settings.ShowFriendOverheadInformation;

            // Game Settings - Information: Guild Members.
            mGuildMemberOverheadInfoCheckbox =
                new LabeledCheckBox(mInformationSettings, "GuildMemberOverheadInfoCheckbox");
            mGuildMemberOverheadInfoCheckbox.Text = Strings.Settings.ShowGuildOverheadInformation;

            // Game Settings - Information: Myself.
            mMyOverheadInfoCheckbox = new LabeledCheckBox(mInformationSettings, "MyOverheadInfoCheckbox");
            mMyOverheadInfoCheckbox.Text = Strings.Settings.ShowMyOverheadInformation;

            // Game Settings - Information: NPCs.
            mNpcOverheadInfoCheckbox = new LabeledCheckBox(mInformationSettings, "NpcOverheadInfoCheckbox");
            mNpcOverheadInfoCheckbox.Text = Strings.Settings.ShowNpcOverheadInformation;

            // Game Settings - Information: Party Members.
            mPartyMemberOverheadInfoCheckbox =
                new LabeledCheckBox(mInformationSettings, "PartyMemberOverheadInfoCheckbox");
            mPartyMemberOverheadInfoCheckbox.Text = Strings.Settings.ShowPartyOverheadInformation;

            // Game Settings - Information: Players.
            mPlayerOverheadInfoCheckbox =
                new LabeledCheckBox(mInformationSettings, "PlayerOverheadInfoCheckbox");
            mPlayerOverheadInfoCheckbox.Text = Strings.Settings.ShowPlayerOverheadInformation;

            // Game Settings - Information: friends overhead hp bar.
            mFriendOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "FriendOverheadHpBarCheckbox");
            mFriendOverheadHpBarCheckbox.Text = Strings.Settings.ShowFriendOverheadHpBar;

            // Game Settings - Information: guild members overhead hp bar.
            mGuildMemberOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "GuildMemberOverheadHpBarCheckbox");
            mGuildMemberOverheadHpBarCheckbox.Text = Strings.Settings.ShowGuildOverheadHpBar;

            // Game Settings - Information: my overhead hp bar.
            mMyOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "MyOverheadHpBarCheckbox");
            mMyOverheadHpBarCheckbox.Text = Strings.Settings.ShowMyOverheadHpBar;

            // Game Settings - Information: NPC overhead hp bar.
            mNpcOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "NpcOverheadHpBarCheckbox");
            mNpcOverheadHpBarCheckbox.Text = Strings.Settings.ShowNpcOverheadHpBar;

            // Game Settings - Information: party members overhead hp bar.
            mPartyMemberOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "PartyMemberOverheadHpBarCheckbox");
            mPartyMemberOverheadHpBarCheckbox.Text = Strings.Settings.ShowPartyOverheadHpBar;

            // Game Settings - Information: players overhead hp bar.
            mPlayerOverheadHpBarCheckbox =
                new LabeledCheckBox(mInformationSettings, "PlayerOverheadHpBarCheckbox");
            mPlayerOverheadHpBarCheckbox.Text = Strings.Settings.ShowPlayerOverheadHpBar;

            // Game Settings - Targeting.
            mTargetingSettings = new ScrollControl(mGameSettingsContainer, "TargetingSettings");
            mTargetingSettings.EnableScroll(false, false);

            // Game Settings - Targeting: Sticky Target.
            mStickyTarget = new LabeledCheckBox(mTargetingSettings, "StickyTargetCheckbox");
            mStickyTarget.Text = Strings.Settings.StickyTarget;

            // Game Settings - Targeting: Auto-turn to Target.
            mAutoTurnToTarget = new LabeledCheckBox(mTargetingSettings, "AutoTurnToTargetCheckbox");
            mAutoTurnToTarget.Text = Strings.Settings.AutoTurnToTarget;

            // Game Settings - Typewriter Text
            mTypewriterCheckbox = new LabeledCheckBox(mInterfaceSettings, "TypewriterCheckbox");
            mTypewriterCheckbox.Text = Strings.Settings.TypewriterText;

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
            var resolutionBackground = new ImagePanel(mVideoSettingsContainer, "ResolutionPanel");

            // Video Settings - Resolution Label.
            var resolutionLabel = new Label(resolutionBackground, "ResolutionLabel");
            resolutionLabel.SetText(Strings.Settings.Resolution);

            // Video Settings - Resolution List.
            mResolutionList = new ComboBox(resolutionBackground, "ResolutionCombobox");
            var myModes = Graphics.Renderer.GetValidVideoModes();
            myModes?.ForEach(
                t =>
                {
                    var item = mResolutionList.AddItem(t);
                    item.Alignment = Pos.Left;
                }
            );

            // Video Settings - FPS Background.
            var fpsBackground = new ImagePanel(mVideoSettingsContainer, "FPSPanel");

            // Video Settings - FPS Label.
            var fpsLabel = new Label(fpsBackground, "FPSLabel");
            fpsLabel.SetText(Strings.Settings.TargetFps);

            // Video Settings - FPS List.
            mFpsList = new ComboBox(fpsBackground, "FPSCombobox");
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
            Input.KeyUp += OnKeyUp;
            Input.MouseUp += OnKeyUp;

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

        void InterfaceSettings_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mInterfaceSettings.Show();
            mInformationSettings.Hide();
            mTargetingSettings.Hide();
        }

        void InformationSettings_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mInterfaceSettings.Hide();
            mInformationSettings.Show();
            mTargetingSettings.Hide();
        }

        void TargetingSettings_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mInterfaceSettings.Hide();
            mInformationSettings.Hide();
            mTargetingSettings.Show();
            mAutoTurnToTarget.IsDisabled = !(Options.Instance?.PlayerOpts?.EnableAutoTurnToTarget ?? false);
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

        private readonly HashSet<Keys> _keysDown = new HashSet<Keys>();

        private void OnKeyDown(Keys modifier, Keys key)
        {
            if (mKeybindingEditBtn != default)
            {
                _ = _keysDown.Add(key);
            }
        }

        private void OnKeyUp(Keys modifier, Keys key)
        {
            if (modifier == Keys.None && key == Keys.None)
            {
                return;
            }

            if (mKeybindingEditBtn != null)
            {
                if (!_keysDown.Remove(key))
                {
                    return;
                }

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
                _keysDown.Clear();
                Interface.GwenInput.HandleInput = true;
            }
        }

        // Methods.
        public void Update()
        {
            if (mSettingsPanel.IsVisible &&
                mKeybindingEditBtn != null &&
                mKeybindingListeningTimer < Timing.Global.MillisecondsUtc)
            {
                OnKeyUp(Keys.None, Keys.None);
            }
        }

        public void Show(bool returnToMenu = false)
        {
            // Take over all input when we're in-game.
            if (Globals.GameState == GameStates.InGame)
            {
                mSettingsPanel.MakeModal(true);
            }

            // Game Settings.
            mAutoCloseWindowsCheckbox.IsChecked = Globals.Database.HideOthersOnWindowOpen;
            mAutoToggleChatLogCheckbox.IsChecked = Globals.Database.AutoToggleChatLog;
            mShowHealthAsPercentageCheckbox.IsChecked = Globals.Database.ShowHealthAsPercentage;
            mShowManaAsPercentageCheckbox.IsChecked = Globals.Database.ShowManaAsPercentage;
            mShowExperienceAsPercentageCheckbox.IsChecked = Globals.Database.ShowExperienceAsPercentage;
            mFriendOverheadInfoCheckbox.IsChecked = Globals.Database.FriendOverheadInfo;
            mGuildMemberOverheadInfoCheckbox.IsChecked = Globals.Database.GuildMemberOverheadInfo;
            mMyOverheadInfoCheckbox.IsChecked = Globals.Database.MyOverheadInfo;
            mNpcOverheadInfoCheckbox.IsChecked = Globals.Database.NpcOverheadInfo;
            mPartyMemberOverheadInfoCheckbox.IsChecked = Globals.Database.PartyMemberOverheadInfo;
            mPlayerOverheadInfoCheckbox.IsChecked = Globals.Database.PlayerOverheadInfo;
            mFriendOverheadHpBarCheckbox.IsChecked = Globals.Database.FriendOverheadHpBar;
            mGuildMemberOverheadHpBarCheckbox.IsChecked = Globals.Database.GuildMemberOverheadHpBar;
            mMyOverheadHpBarCheckbox.IsChecked = Globals.Database.MyOverheadHpBar;
            mNpcOverheadHpBarCheckbox.IsChecked = Globals.Database.NpcOverheadHpBar;
            mPartyMemberOverheadHpBarCheckbox.IsChecked = Globals.Database.PartyMemberOverheadHpBar;
            mPlayerOverheadHpBarCheckbox.IsChecked = Globals.Database.PlayerOverheadHpBar;
            mStickyTarget.IsChecked = Globals.Database.StickyTarget;
            mAutoTurnToTarget.IsChecked = Globals.Database.AutoTurnToTarget;
            mTypewriterCheckbox.IsChecked = Globals.Database.TypewriterBehavior == Enums.TypewriterBehavior.Word;

            // Video Settings.
            mFullscreenCheckbox.IsChecked = Globals.Database.FullScreen;
            mLightingEnabledCheckbox.IsChecked = Globals.Database.EnableLighting;

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

            // Audio Settings.
            mPreviousMusicVolume = Globals.Database.MusicVolume;
            mPreviousSoundVolume = Globals.Database.SoundVolume;
            mMusicSlider.Value = Globals.Database.MusicVolume;
            mSoundSlider.Value = Globals.Database.SoundVolume;
            mMusicLabel.Text = Strings.Settings.MusicVolume.ToString((int)mMusicSlider.Value);
            mSoundLabel.Text = Strings.Settings.SoundVolume.ToString((int)mSoundSlider.Value);

            // Control Settings.
            mKeybindingEditControls = new Controls(Controls.ActiveControls);

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
                mKeybindingListeningTimer = Timing.Global.MillisecondsUtc + 3000;
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

            // Game Settings.
            Globals.Database.HideOthersOnWindowOpen = mAutoCloseWindowsCheckbox.IsChecked;
            Globals.Database.AutoToggleChatLog = mAutoToggleChatLogCheckbox.IsChecked;
            Globals.Database.ShowExperienceAsPercentage = mShowExperienceAsPercentageCheckbox.IsChecked;
            Globals.Database.ShowHealthAsPercentage = mShowHealthAsPercentageCheckbox.IsChecked;
            Globals.Database.ShowManaAsPercentage = mShowManaAsPercentageCheckbox.IsChecked;
            Globals.Database.FriendOverheadInfo = mFriendOverheadInfoCheckbox.IsChecked;
            Globals.Database.GuildMemberOverheadInfo = mGuildMemberOverheadInfoCheckbox.IsChecked;
            Globals.Database.MyOverheadInfo = mMyOverheadInfoCheckbox.IsChecked;
            Globals.Database.NpcOverheadInfo = mNpcOverheadInfoCheckbox.IsChecked;
            Globals.Database.PartyMemberOverheadInfo = mPartyMemberOverheadInfoCheckbox.IsChecked;
            Globals.Database.PlayerOverheadInfo = mPlayerOverheadInfoCheckbox.IsChecked;
            Globals.Database.FriendOverheadHpBar = mFriendOverheadHpBarCheckbox.IsChecked;
            Globals.Database.GuildMemberOverheadHpBar = mGuildMemberOverheadHpBarCheckbox.IsChecked;
            Globals.Database.MyOverheadHpBar= mMyOverheadHpBarCheckbox.IsChecked;
            Globals.Database.NpcOverheadHpBar= mNpcOverheadHpBarCheckbox.IsChecked;
            Globals.Database.PartyMemberOverheadHpBar = mPartyMemberOverheadHpBarCheckbox.IsChecked;
            Globals.Database.PlayerOverheadHpBar = mPlayerOverheadHpBarCheckbox.IsChecked;
            Globals.Database.StickyTarget = mStickyTarget.IsChecked;
            Globals.Database.AutoTurnToTarget = mAutoTurnToTarget.IsChecked;
            Globals.Database.TypewriterBehavior = mTypewriterCheckbox.IsChecked ? Enums.TypewriterBehavior.Word : Enums.TypewriterBehavior.Off;

            // Video Settings.
            var resolution = mResolutionList.SelectedItem;
            var validVideoModes = Graphics.Renderer.GetValidVideoModes();
            var targetResolution = validVideoModes?.FindIndex(videoMode =>
                string.Equals(videoMode, resolution.Text)) ?? -1;
            var newFps = 0;

            Globals.Database.EnableLighting = mLightingEnabledCheckbox.IsChecked;

            if (targetResolution > -1)
            {
                shouldReset = Globals.Database.TargetResolution != targetResolution ||
                              Graphics.Renderer.HasOverrideResolution;
                Globals.Database.TargetResolution = targetResolution;
            }

            if (Globals.Database.FullScreen != mFullscreenCheckbox.IsChecked)
            {
                Globals.Database.FullScreen = mFullscreenCheckbox.IsChecked;
                shouldReset = true;
            }

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

            // Audio Settings.
            Globals.Database.MusicVolume = (int)mMusicSlider.Value;
            Globals.Database.SoundVolume = (int)mSoundSlider.Value;
            Audio.UpdateGlobalVolume();

            // Control Settings.
            Controls.ActiveControls = mKeybindingEditControls;
            Controls.ActiveControls.Save();

            // Save Preferences.
            Globals.Database.SavePreferences();

            if (shouldReset)
            {
                mCustomResolutionMenuItem?.Hide();
                Graphics.Renderer.OverrideResolution = Resolution.Empty;
                Graphics.Renderer.Init();
            }

            // Hide the currently opened window.
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
