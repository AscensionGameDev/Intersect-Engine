using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Core.Localization;
using Intersect.Utilities;
using static Intersect.Client.Framework.File_Management.GameContentManager;
using MathHelper = Intersect.Client.Utilities.MathHelper;

namespace Intersect.Client.Interface.Shared;

public partial class SettingsWindow : ImagePanel
{
    // Parent Window.
    private readonly MainMenu? _mainMenu;
    private readonly EscapeMenu? _escapeMenu;

    // Settings Window.
    private readonly Label _settingsHeader;
    private readonly Button _settingsApplyBtn;
    private readonly Button _settingsCancelBtn;

    // Settings Containers.
    private readonly ScrollControl _gameSettingsContainer;
    private readonly ScrollControl _videoSettingsContainer;
    private readonly ScrollControl _audioSettingsContainer;
    private readonly ScrollControl _keybindingSettingsContainer;

    // Tabs.
    private readonly Button _gameSettingsTab;
    private readonly Button _videoSettingsTab;
    private readonly Button _audioSettingsTab;
    private readonly Button _keybindingSettingsTab;

    // Game Settings - Interface.
    private readonly ScrollControl _interfaceSettings;
    private readonly LabeledCheckBox _autoCloseWindowsCheckbox;
    private readonly LabeledCheckBox _autoToggleChatLogCheckbox;
    private readonly LabeledCheckBox _showExperienceAsPercentageCheckbox;
    private readonly LabeledCheckBox _showHealthAsPercentageCheckbox;
    private readonly LabeledCheckBox _showManaAsPercentageCheckbox;
    private readonly LabeledCheckBox _simplifiedEscapeMenu;

    // Game Settings - Information.
    private readonly ScrollControl _informationSettings;
    private readonly LabeledCheckBox _friendOverheadInfoCheckbox;
    private readonly LabeledCheckBox _guildMemberOverheadInfoCheckbox;
    private readonly LabeledCheckBox _myOverheadInfoCheckbox;
    private readonly LabeledCheckBox _npcOverheadInfoCheckbox;
    private readonly LabeledCheckBox _partyMemberOverheadInfoCheckbox;
    private readonly LabeledCheckBox _playerOverheadInfoCheckbox;
    private readonly LabeledCheckBox _friendOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _guildMemberOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _myOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _mpcOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _partyMemberOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _playerOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _typewriterCheckbox;

    // Game Settings - Targeting.
    private readonly ScrollControl _targetingSettings;
    private readonly LabeledCheckBox _stickyTarget;
    private readonly LabeledCheckBox _autoTurnToTarget;

    // Video Settings.
    private readonly ComboBox _resolutionList;
    private MenuItem? _customResolutionMenuItem;
    private readonly ComboBox _fpsList;
    private readonly LabeledHorizontalSlider _worldScale;
    private readonly LabeledCheckBox _fullscreenCheckbox;
    private readonly LabeledCheckBox _lightingEnabledCheckbox;

    // Audio Settings.
    private readonly HorizontalSlider _musicSlider;
    private int _previousMusicVolume;
    private readonly Label _musicLabel;
    private readonly HorizontalSlider _soundSlider;
    private int _previousSoundVolume;
    private readonly Label _soundLabel;

    // Keybinding Settings.
    private Control _keybindingEditControl;
    private Controls _keybindingEditControls = default!;
    private Button? _keybindingEditBtn;
    private readonly Button _keybindingRestoreBtn;
    private long _keybindingListeningTimer;
    private int _keyEdit = -1;
    private readonly Dictionary<Control, Button[]> mKeybindingBtns = [];

    // Open Settings.
    private bool _returnToMenu;

    // Initialize.
    public SettingsWindow(Base parent, MainMenu? mainMenu, EscapeMenu? escapeMenu) : base(parent, nameof(SettingsWindow))
    {
        // Assign References.
        _mainMenu = mainMenu;
        _escapeMenu = escapeMenu;

        // Main Menu Window.
        Interface.InputBlockingElements.Add(this);

        // Menu Header.
        _settingsHeader = new Label(this, "SettingsHeader")
        {
            Text = Strings.Settings.Title
        };

        // Apply Button.
        _settingsApplyBtn = new Button(this, "SettingsApplyBtn")
        {
            Text = Strings.Settings.Apply
        };
        _settingsApplyBtn.Clicked += SettingsApplyBtn_Clicked;

        // Cancel Button.
        _settingsCancelBtn = new Button(this, "SettingsCancelBtn")
        {
            Text = Strings.Settings.Cancel
        };
        _settingsCancelBtn.Clicked += SettingsCancelBtn_Clicked;

        #region InitGameSettings

        // Init GameSettings Tab.
        _gameSettingsTab = new Button(this, "GameSettingsTab")
        {
            Text = Strings.Settings.GameSettingsTab
        };
        _gameSettingsTab.Clicked += GameSettingsTab_Clicked;

        // Game Settings are stored in the GameSettings Scroll Control.
        _gameSettingsContainer = new ScrollControl(this, "GameSettingsContainer");
        _gameSettingsContainer.EnableScroll(false, false);

        // Game Settings subcategories are stored in the GameSettings List.
        var gameSettingsList = new ListBox(_gameSettingsContainer, "GameSettingsList");
        _ = gameSettingsList.AddRow(Strings.Settings.InterfaceSettings);
        _ = gameSettingsList.AddRow(Strings.Settings.InformationSettings);
        _ = gameSettingsList.AddRow(Strings.Settings.TargetingSettings);
        gameSettingsList.EnableScroll(false, true);
        gameSettingsList.SelectedRowIndex = 0;
        gameSettingsList[0].Clicked += InterfaceSettings_Clicked;
        gameSettingsList[1].Clicked += InformationSettings_Clicked;
        gameSettingsList[2].Clicked += TargetingSettings_Clicked;

        // Game Settings - Interface.
        _interfaceSettings = new ScrollControl(_gameSettingsContainer, "InterfaceSettings");
        _interfaceSettings.EnableScroll(false, true);

        // Game Settings - Interface: Auto-close Windows.
        _autoCloseWindowsCheckbox = new LabeledCheckBox(_interfaceSettings, "AutoCloseWindowsCheckbox")
        {
            Text = Strings.Settings.AutoCloseWindows
        };

        // Game Settings - Interface: Auto-toggle chat log visibility.
        _autoToggleChatLogCheckbox = new LabeledCheckBox(_interfaceSettings, "AutoToggleChatLogCheckbox")
        {
            Text = Strings.Settings.AutoToggleChatLog
        };

        // Game Settings - Interface: Show EXP/HP/MP as Percentage.
        _showExperienceAsPercentageCheckbox = new LabeledCheckBox(_interfaceSettings, "ShowExperienceAsPercentageCheckbox")
        {
            Text = Strings.Settings.ShowExperienceAsPercentage
        };

        _showHealthAsPercentageCheckbox = new LabeledCheckBox(_interfaceSettings, "ShowHealthAsPercentageCheckbox")
        {
            Text = Strings.Settings.ShowHealthAsPercentage
        };

        _showManaAsPercentageCheckbox = new LabeledCheckBox(_interfaceSettings, "ShowManaAsPercentageCheckbox")
        {
            Text = Strings.Settings.ShowManaAsPercentage
        };
        
        // Game Settings - Interface: simplified escape menu.
        _simplifiedEscapeMenu = new LabeledCheckBox(_interfaceSettings, "SimplifiedEscapeMenu")
        {
            Text = Strings.Settings.SimplifiedEscapeMenu
        };

        // Game Settings - Information.
        _informationSettings = new ScrollControl(_gameSettingsContainer, "InformationSettings");
        _informationSettings.EnableScroll(false, true);

        // Game Settings - Information: Friends.
        _friendOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "FriendOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowFriendOverheadInformation
        };

        // Game Settings - Information: Guild Members.
        _guildMemberOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "GuildMemberOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowGuildOverheadInformation
        };

        // Game Settings - Information: Myself.
        _myOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "MyOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowMyOverheadInformation
        };

        // Game Settings - Information: NPCs.
        _npcOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "NpcOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowNpcOverheadInformation
        };

        // Game Settings - Information: Party Members.
        _partyMemberOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "PartyMemberOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowPartyOverheadInformation
        };

        // Game Settings - Information: Players.
        _playerOverheadInfoCheckbox = new LabeledCheckBox(_informationSettings, "PlayerOverheadInfoCheckbox")
        {
            Text = Strings.Settings.ShowPlayerOverheadInformation
        };

        // Game Settings - Information: friends overhead hp bar.
        _friendOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "FriendOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowFriendOverheadHpBar
        };

        // Game Settings - Information: guild members overhead hp bar.
        _guildMemberOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "GuildMemberOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowGuildOverheadHpBar
        };

        // Game Settings - Information: my overhead hp bar.
        _myOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "MyOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowMyOverheadHpBar
        };

        // Game Settings - Information: NPC overhead hp bar.
        _mpcOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "NpcOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowNpcOverheadHpBar
        };

        // Game Settings - Information: party members overhead hp bar.
        _partyMemberOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "PartyMemberOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowPartyOverheadHpBar
        };

        // Game Settings - Information: players overhead hp bar.
        _playerOverheadHpBarCheckbox = new LabeledCheckBox(_informationSettings, "PlayerOverheadHpBarCheckbox")
        {
            Text = Strings.Settings.ShowPlayerOverheadHpBar
        };

        // Game Settings - Targeting.
        _targetingSettings = new ScrollControl(_gameSettingsContainer, "TargetingSettings");
        _targetingSettings.EnableScroll(false, false);

        // Game Settings - Targeting: Sticky Target.
        _stickyTarget = new LabeledCheckBox(_targetingSettings, "StickyTargetCheckbox")
        {
            Text = Strings.Settings.StickyTarget
        };

        // Game Settings - Targeting: Auto-turn to Target.
        _autoTurnToTarget = new LabeledCheckBox(_targetingSettings, "AutoTurnToTargetCheckbox")
        {
            Text = Strings.Settings.AutoTurnToTarget
        };

        // Game Settings - Typewriter Text
        _typewriterCheckbox = new LabeledCheckBox(_interfaceSettings, "TypewriterCheckbox")
        {
            Text = Strings.Settings.TypewriterText
        };

        #endregion

        #region InitVideoSettings

        // Init VideoSettings Tab.
        _videoSettingsTab = new Button(this, "VideoSettingsTab")
        {
            Text = Strings.Settings.VideoSettingsTab
        };
        _videoSettingsTab.Clicked += VideoSettingsTab_Clicked;

        // Video Settings Get Stored in the VideoSettings Scroll Control.
        _videoSettingsContainer = new ScrollControl(this, "VideoSettingsContainer");
        _videoSettingsContainer.EnableScroll(false, false);

        // Video Settings - Resolution Background.
        var resolutionBackground = new ImagePanel(_videoSettingsContainer, "ResolutionPanel");

        // Video Settings - Resolution Label.
        var resolutionLabel = new Label(resolutionBackground, "ResolutionLabel")
        {
            Text = Strings.Settings.Resolution
        };

        // Video Settings - Resolution List.
        _resolutionList = new ComboBox(resolutionBackground, "ResolutionCombobox");
        var myModes = Graphics.Renderer?.GetValidVideoModes();
        myModes?.ForEach(
            t =>
            {
                var item = _resolutionList.AddItem(t);
                item.Alignment = Pos.Left;
            }
        );

        Globals.Database.WorldZoom = MathHelper.Clamp(Globals.Database.WorldZoom, 1, 4);

        var worldScaleNotches = new double[] { 1, 2, 4 }.Select(n => n * Graphics.BaseWorldScale).ToArray();
        _worldScale = new LabeledHorizontalSlider(_videoSettingsContainer, "WorldScale")
        {
            Label = Strings.Settings.WorldScale,
            Min = worldScaleNotches.Min(),
            Max = worldScaleNotches.Max(),
            Notches = worldScaleNotches,
            SnapToNotches = false,
            Value = Globals.Database.WorldZoom,
        };

        // Video Settings - FPS Background.
        var fpsBackground = new ImagePanel(_videoSettingsContainer, "FPSPanel");

        // Video Settings - FPS Label.
        var fpsLabel = new Label(fpsBackground, "FPSLabel")
        {
            Text = Strings.Settings.TargetFps
        };

        // Video Settings - FPS List.
        _fpsList = new ComboBox(fpsBackground, "FPSCombobox");
        _ = _fpsList.AddItem(Strings.Settings.Vsync);
        _ = _fpsList.AddItem(Strings.Settings.Fps30);
        _ = _fpsList.AddItem(Strings.Settings.Fps60);
        _ = _fpsList.AddItem(Strings.Settings.Fps90);
        _ = _fpsList.AddItem(Strings.Settings.Fps120);
        _ = _fpsList.AddItem(Strings.Settings.UnlimitedFps);

        // Video Settings - Fullscreen Checkbox.
        _fullscreenCheckbox = new LabeledCheckBox(_videoSettingsContainer, "FullscreenCheckbox")
        {
            Text = Strings.Settings.Fullscreen
        };

        // Video Settings - Enable Lighting Checkbox
        _lightingEnabledCheckbox = new LabeledCheckBox(_videoSettingsContainer, "EnableLightingCheckbox")
        {
            Text = Strings.Settings.EnableLighting
        };

        #endregion

        #region InitAudioSettings

        // Init AudioSettingsTab.
        _audioSettingsTab = new Button(this, "AudioSettingsTab")
        {
            Text = Strings.Settings.AudioSettingsTab
        };
        _audioSettingsTab.Clicked += AudioSettingsTab_Clicked;

        // Audio Settings Get Stored in the AudioSettings Scroll Control.
        _audioSettingsContainer = new ScrollControl(this, "AudioSettingsContainer");
        _audioSettingsContainer.EnableScroll(false, false);

        // Audio Settings - Sound Label
        _soundLabel = new Label(_audioSettingsContainer, "SoundLabel")
        {
            Text = Strings.Settings.SoundVolume.ToString(100)
        };

        // Audio Settings - Sound Slider
        _soundSlider = new HorizontalSlider(_audioSettingsContainer, "SoundSlider")
        {
            Min = 0,
            Max = 100
        };
        _soundSlider.ValueChanged += SoundSlider_ValueChanged;

        // Audio Settings - Music Label
        _musicLabel = new Label(_audioSettingsContainer, "MusicLabel")
        {
            Text = Strings.Settings.MusicVolume.ToString(100)
        };

        // Audio Settings - Music Slider
        _musicSlider = new HorizontalSlider(_audioSettingsContainer, "MusicSlider")
        {
            Min = 0,
            Max = 100
        };
        _musicSlider.ValueChanged += MusicSlider_ValueChanged;

        #endregion

        #region InitKeybindingSettings

        // Init KeybindingsSettings Tab.
        _keybindingSettingsTab = new Button(this, "KeybindingSettingsTab")
        {
            Text = Strings.Settings.KeyBindingSettingsTab
        };
        _keybindingSettingsTab.Clicked += KeybindingSettingsTab_Clicked;

        // KeybindingSettings Get Stored in the KeybindingSettings Scroll Control
        _keybindingSettingsContainer = new ScrollControl(this, "KeybindingSettingsContainer");
        _keybindingSettingsContainer.EnableScroll(false, true);

        // Keybinding Settings - Restore Default Keys Button.
        _keybindingRestoreBtn = new Button(this, "KeybindingsRestoreBtn")
        {
            Text = Strings.Settings.Restore
        };
        _keybindingRestoreBtn.Clicked += KeybindingsRestoreBtn_Clicked;

        // Keybinding Settings - Controls 
        var row = 0;
        var defaultFont = Current.GetFont("sourcesansproblack", 10);
        foreach (Control control in Enum.GetValues(typeof(Control)))
        {
            var offset = row++ * 32;
            var name = Enum.GetName(typeof(Control), control)?.ToLower() ?? string.Empty;

            var prefix = $"Control{Enum.GetName(typeof(Control), control)}";
            var label = new Label(_keybindingSettingsContainer, $"{prefix}Label")
            {
                Text = Strings.Controls.KeyDictionary[name],
                AutoSizeToContents = true,
                Font = defaultFont,
            };
            _ = label.SetBounds(14, 11 + offset, 21, 16);
            label.SetTextColor(new Color(255, 255, 255, 255), Label.ControlState.Normal);

            var key1 = new Button(_keybindingSettingsContainer, $"{prefix}Button1")
            {
                AutoSizeToContents = false,
                Font = defaultFont,
                Text = string.Empty,
                UserData = new KeyValuePair<Control, int>(control, 0),
            };
            key1.SetTextColor(Color.White, Label.ControlState.Normal);
            key1.SetImage("control_button.png", Button.ControlState.Normal);
            key1.SetImage("control_button_hovered.png", Button.ControlState.Hovered);
            key1.SetImage("control_button_clicked.png", Button.ControlState.Clicked);
            key1.SetHoverSound("octave-tap-resonant.wav");
            key1.SetMouseDownSound("octave-tap-warm.wav");
            _ = key1.SetBounds(181, 6 + offset, 120, 28);
            key1.Clicked += Key_Clicked;

            var key2 = new Button(_keybindingSettingsContainer, $"{prefix}Button2")
            {
                AutoSizeToContents = false,
                Font = defaultFont,
                Text = string.Empty,
                UserData = new KeyValuePair<Control, int>(control, 1),
            };
            key2.SetTextColor(Color.White, Label.ControlState.Normal);
            key2.SetImage("control_button.png", Button.ControlState.Normal);
            key2.SetImage("control_button_hovered.png", Button.ControlState.Hovered);
            key2.SetImage("control_button_clicked.png", Button.ControlState.Clicked);
            key2.SetHoverSound("octave-tap-resonant.wav");
            key2.SetMouseDownSound("octave-tap-warm.wav");
            _ = key2.SetBounds(309, 6 + offset, 120, 28);
            key2.Clicked += Key_Clicked;

            mKeybindingBtns.Add(control, [key1, key2]);
        }

        Input.KeyDown += OnKeyDown;
        Input.MouseDown += OnKeyDown;
        Input.KeyUp += OnKeyUp;
        Input.MouseUp += OnKeyUp;

        #endregion

        LoadJsonUi(UI.Shared, Graphics.Renderer?.GetResolutionString());
        IsHidden = true;
    }

    private void GameSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
    {
        // Determine if GameSettingsContainer is currently being shown or not.
        if (!_gameSettingsContainer.IsVisible)
        {
            // Disable the GameSettingsTab to fake it being selected visually.
            _gameSettingsTab.Disable();
            _videoSettingsTab.Enable();
            _audioSettingsTab.Enable();
            _keybindingSettingsTab.Enable();

            // Containers.
            _gameSettingsContainer.Show();
            _videoSettingsContainer.Hide();
            _audioSettingsContainer.Hide();
            _keybindingSettingsContainer.Hide();

            // Restore Default KeybindingSettings Button.
            _keybindingRestoreBtn.Hide();
        }
    }

    void InterfaceSettings_Clicked(Base sender, ClickedEventArgs arguments)
    {
        _interfaceSettings.Show();
        _informationSettings.Hide();
        _targetingSettings.Hide();
    }

    void InformationSettings_Clicked(Base sender, ClickedEventArgs arguments)
    {
        _interfaceSettings.Hide();
        _informationSettings.Show();
        _targetingSettings.Hide();
    }

    void TargetingSettings_Clicked(Base sender, ClickedEventArgs arguments)
    {
        _interfaceSettings.Hide();
        _informationSettings.Hide();
        _targetingSettings.Show();
        _autoTurnToTarget.IsDisabled = !(Options.Instance?.PlayerOpts?.EnableAutoTurnToTarget ?? false);
    }

    private void VideoSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
    {
        // Determine if VideoSettingsContainer is currently being shown or not.
        if (!_videoSettingsContainer.IsVisible)
        {
            // Disable the VideoSettingsTab to fake it being selected visually.
            _gameSettingsTab.Enable();
            _videoSettingsTab.Disable();
            _audioSettingsTab.Enable();
            _keybindingSettingsTab.Enable();

            // Containers.
            _gameSettingsContainer.Hide();
            _videoSettingsContainer.Show();
            _audioSettingsContainer.Hide();
            _keybindingSettingsContainer.Hide();

            // Restore Default KeybindingSettings Button.
            _keybindingRestoreBtn.Hide();
        }
    }

    private void AudioSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
    {
        // Determine if AudioSettingsContainer is currently being shown or not.
        if (!_audioSettingsContainer.IsVisible)
        {
            // Disable the AudioSettingsTab to fake it being selected visually.
            _gameSettingsTab.Enable();
            _videoSettingsTab.Enable();
            _audioSettingsTab.Disable();
            _keybindingSettingsTab.Enable();

            // Containers.
            _gameSettingsContainer.Hide();
            _videoSettingsContainer.Hide();
            _audioSettingsContainer.Show();
            _keybindingSettingsContainer.Hide();

            // Restore Default KeybindingSettings Button.
            _keybindingRestoreBtn.Hide();
        }
    }

    private void KeybindingSettingsTab_Clicked(Base sender, ClickedEventArgs arguments)
    {
        // Determine if controls are currently being shown or not.
        if (!_keybindingSettingsContainer.IsVisible)
        {
            // Disable the KeybindingSettingsTab to fake it being selected visually.
            _gameSettingsTab.Enable();
            _videoSettingsTab.Enable();
            _audioSettingsTab.Enable();
            _keybindingSettingsTab.Disable();

            // Containers.
            _gameSettingsContainer.Hide();
            _videoSettingsContainer.Hide();
            _audioSettingsContainer.Hide();
            _keybindingSettingsContainer.Show();

            // Restore Default KeybindingSettings Button.
            _keybindingRestoreBtn.Show();

            // KeybindingBtns.
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var controlMapping = _keybindingEditControls.ControlMapping[control];
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
        _settingsHeader.SetText(Strings.Settings.Title);

        // Containers.
        _gameSettingsContainer.Show();
        _videoSettingsContainer.Hide();
        _audioSettingsContainer.Hide();
        _keybindingSettingsContainer.Hide();

        // Tabs.
        _gameSettingsTab.Show();
        _videoSettingsTab.Show();
        _audioSettingsTab.Show();
        _keybindingSettingsTab.Show();

        // Disable the GameSettingsTab to fake it being selected visually by default.
        _gameSettingsTab.Disable();
        _videoSettingsTab.Enable();
        _audioSettingsTab.Enable();
        _keybindingSettingsTab.Enable();

        // Buttons.
        _settingsApplyBtn.Show();
        _settingsCancelBtn.Show();
        _keybindingRestoreBtn.Hide();

        var worldScaleNotches = new double[] { 1, 2, 4 }.Select(n => n * Graphics.BaseWorldScale).ToArray();

        Globals.Database.WorldZoom = (float)MathHelper.Clamp(
            Globals.Database.WorldZoom,
            worldScaleNotches.Min(),
            worldScaleNotches.Max()
        );
        _worldScale.Min = worldScaleNotches.Min();
        _worldScale.Max = worldScaleNotches.Max();
        _worldScale.Value = Globals.Database.WorldZoom;
    }

    private readonly HashSet<Keys> _keysDown = [];

    private void OnKeyDown(Keys modifier, Keys key)
    {
        if (_keybindingEditBtn != default)
        {
            _ = _keysDown.Add(key);
        }
    }

    private void OnKeyUp(Keys modifier, Keys key)
    {
        if (_keybindingEditBtn == null)
        {
            return;
        }

        if (key != Keys.None && !_keysDown.Remove(key))
        {
            return;
        }

        _keybindingEditControls.UpdateControl(_keybindingEditControl, _keyEdit, modifier, key);
        _keybindingEditBtn.Text = Strings.Keys.FormatKeyName(modifier, key);

        if (key != Keys.None)
        {
            foreach (var control in _keybindingEditControls.ControlMapping)
            {
                if (control.Key == _keybindingEditControl)
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
                        _keybindingEditControls.UpdateControl(control.Key, bindingIndex, Keys.None, Keys.None);

                        // Update UI.
                        mKeybindingBtns[control.Key][bindingIndex].Text = Strings.Keys.KeyDictionary[Enum.GetName(typeof(Keys), Keys.None)?.ToLower() ?? string.Empty];
                    }
                }
            }

            _keybindingEditBtn.PlayHoverSound();
        }

        _keybindingEditBtn = null;
        _keysDown.Clear();
        Interface.GwenInput.HandleInput = true;
    }

    // Methods.
    public void Update()
    {
        if (!IsHidden &&
            _keybindingEditBtn != null &&
            _keybindingListeningTimer < Timing.Global.MillisecondsUtc)
        {
            OnKeyUp(Keys.None, Keys.None);
        }
    }

    public void Show(bool returnToMenu = false)
    {
        // Take over all input when we're in-game.
        if (Globals.GameState == GameStates.InGame)
        {
            MakeModal(true);
        }

        // Game Settings.
        _autoCloseWindowsCheckbox.IsChecked = Globals.Database.HideOthersOnWindowOpen;
        _autoToggleChatLogCheckbox.IsChecked = Globals.Database.AutoToggleChatLog;
        _showHealthAsPercentageCheckbox.IsChecked = Globals.Database.ShowHealthAsPercentage;
        _showManaAsPercentageCheckbox.IsChecked = Globals.Database.ShowManaAsPercentage;
        _showExperienceAsPercentageCheckbox.IsChecked = Globals.Database.ShowExperienceAsPercentage;
        _simplifiedEscapeMenu.IsChecked = Globals.Database.SimplifiedEscapeMenu;
        _friendOverheadInfoCheckbox.IsChecked = Globals.Database.FriendOverheadInfo;
        _guildMemberOverheadInfoCheckbox.IsChecked = Globals.Database.GuildMemberOverheadInfo;
        _myOverheadInfoCheckbox.IsChecked = Globals.Database.MyOverheadInfo;
        _npcOverheadInfoCheckbox.IsChecked = Globals.Database.NpcOverheadInfo;
        _partyMemberOverheadInfoCheckbox.IsChecked = Globals.Database.PartyMemberOverheadInfo;
        _playerOverheadInfoCheckbox.IsChecked = Globals.Database.PlayerOverheadInfo;
        _friendOverheadHpBarCheckbox.IsChecked = Globals.Database.FriendOverheadHpBar;
        _guildMemberOverheadHpBarCheckbox.IsChecked = Globals.Database.GuildMemberOverheadHpBar;
        _myOverheadHpBarCheckbox.IsChecked = Globals.Database.MyOverheadHpBar;
        _mpcOverheadHpBarCheckbox.IsChecked = Globals.Database.NpcOverheadHpBar;
        _partyMemberOverheadHpBarCheckbox.IsChecked = Globals.Database.PartyMemberOverheadHpBar;
        _playerOverheadHpBarCheckbox.IsChecked = Globals.Database.PlayerOverheadHpBar;
        _stickyTarget.IsChecked = Globals.Database.StickyTarget;
        _autoTurnToTarget.IsChecked = Globals.Database.AutoTurnToTarget;
        _typewriterCheckbox.IsChecked = Globals.Database.TypewriterBehavior == Enums.TypewriterBehavior.Word;

        // Video Settings.
        _fullscreenCheckbox.IsChecked = Globals.Database.FullScreen;
        _lightingEnabledCheckbox.IsChecked = Globals.Database.EnableLighting;

        // _uiScale.Value = Globals.Database.UIScale;
        _worldScale.Value = Globals.Database.WorldZoom;

        if (Graphics.Renderer?.GetValidVideoModes().Count > 0)
        {
            string resolutionLabel;
            if (Graphics.Renderer.HasOverrideResolution)
            {
                resolutionLabel = Strings.Settings.ResolutionCustom;

                _customResolutionMenuItem ??= _resolutionList.AddItem(Strings.Settings.ResolutionCustom);
                _customResolutionMenuItem.Show();
            }
            else
            {
                var validVideoModes = Graphics.Renderer.GetValidVideoModes();
                var targetResolution = Globals.Database.TargetResolution;
                resolutionLabel = targetResolution < 0 || validVideoModes.Count <= targetResolution ? Strings.Settings.ResolutionCustom : validVideoModes[Globals.Database.TargetResolution];
            }

            _resolutionList.SelectByText(resolutionLabel);
        }

        switch (Globals.Database.TargetFps)
        {
            case -1: // Unlimited.
                _fpsList.SelectByText(Strings.Settings.UnlimitedFps);

                break;
            case 0: // Vertical Sync.
                _fpsList.SelectByText(Strings.Settings.Vsync);

                break;
            case 1: // 30 Frames per second.
                _fpsList.SelectByText(Strings.Settings.Fps30);

                break;
            case 2: // 60 Frames per second.
                _fpsList.SelectByText(Strings.Settings.Fps60);

                break;
            case 3: // 90 Frames per second.
                _fpsList.SelectByText(Strings.Settings.Fps90);

                break;
            case 4: // 120 Frames per second.
                _fpsList.SelectByText(Strings.Settings.Fps120);

                break;
            default:
                _fpsList.SelectByText(Strings.Settings.Vsync);

                break;
        }

        // Audio Settings.
        _previousMusicVolume = Globals.Database.MusicVolume;
        _previousSoundVolume = Globals.Database.SoundVolume;
        _musicSlider.Value = Globals.Database.MusicVolume;
        _soundSlider.Value = Globals.Database.SoundVolume;
        _musicLabel.Text = Strings.Settings.MusicVolume.ToString((int)_musicSlider.Value);
        _soundLabel.Text = Strings.Settings.SoundVolume.ToString((int)_soundSlider.Value);

        // Control Settings.
        _keybindingEditControls = new Controls(Controls.ActiveControls);

        // Settings Window is not hidden anymore.
        base.Show();

        // Load every GUI element to their default state when showing up the settings window (pressed tabs, containers, etc.)
        LoadSettingsWindow();

        // Set up whether we're supposed to return to the previous menu.
        _returnToMenu = returnToMenu;
    }

    public override void Hide()
    {
        // Hide the current window.
        base.Hide();
        RemoveModal();

        // Return to our previous menus (or not) depending on gamestate and the method we'd been opened.
        if (_returnToMenu)
        {
            switch (Globals.GameState)
            {
                case GameStates.Menu:
                    _mainMenu?.Show();
                    break;

                case GameStates.InGame:
                    _escapeMenu?.Show();
                    break;

                default:
                    throw new NotImplementedException();
            }

            _returnToMenu = false;
        }
    }

    // Input Handlers
    private void MusicSlider_ValueChanged(Base sender, EventArgs arguments)
    {
        _musicLabel.Text = Strings.Settings.MusicVolume.ToString((int)_musicSlider.Value);
        Globals.Database.MusicVolume = (int)_musicSlider.Value;
        Audio.UpdateGlobalVolume();
    }

    private void SoundSlider_ValueChanged(Base sender, EventArgs arguments)
    {
        _soundLabel.Text = Strings.Settings.SoundVolume.ToString((int)_soundSlider.Value);
        Globals.Database.SoundVolume = (int)_soundSlider.Value;
        Audio.UpdateGlobalVolume();
    }

    private void Key_Clicked(Base sender, ClickedEventArgs arguments)
    {
        EditKeyPressed((Button)sender);
    }

    private void EditKeyPressed(Button sender)
    {
        if (_keybindingEditBtn == null)
        {
            sender.Text = Strings.Controls.Listening;
            _keyEdit = ((KeyValuePair<Control, int>)sender.UserData).Value;
            _keybindingEditControl = ((KeyValuePair<Control, int>)sender.UserData).Key;
            _keybindingEditBtn = sender;
            Interface.GwenInput.HandleInput = false;
            _keybindingListeningTimer = Timing.Global.MillisecondsUtc + 3000;
        }
    }

    private void KeybindingsRestoreBtn_Clicked(Base sender, ClickedEventArgs arguments)
    {
        _keybindingEditControls.ResetDefaults();
        foreach (Control control in Enum.GetValues(typeof(Control)))
        {
            var controlMapping = _keybindingEditControls.ControlMapping[control];
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
        Globals.Database.HideOthersOnWindowOpen = _autoCloseWindowsCheckbox.IsChecked;
        Globals.Database.AutoToggleChatLog = _autoToggleChatLogCheckbox.IsChecked;
        Globals.Database.ShowExperienceAsPercentage = _showExperienceAsPercentageCheckbox.IsChecked;
        Globals.Database.ShowHealthAsPercentage = _showHealthAsPercentageCheckbox.IsChecked;
        Globals.Database.ShowManaAsPercentage = _showManaAsPercentageCheckbox.IsChecked;
        Globals.Database.SimplifiedEscapeMenu = _simplifiedEscapeMenu.IsChecked;
        Globals.Database.FriendOverheadInfo = _friendOverheadInfoCheckbox.IsChecked;
        Globals.Database.GuildMemberOverheadInfo = _guildMemberOverheadInfoCheckbox.IsChecked;
        Globals.Database.MyOverheadInfo = _myOverheadInfoCheckbox.IsChecked;
        Globals.Database.NpcOverheadInfo = _npcOverheadInfoCheckbox.IsChecked;
        Globals.Database.PartyMemberOverheadInfo = _partyMemberOverheadInfoCheckbox.IsChecked;
        Globals.Database.PlayerOverheadInfo = _playerOverheadInfoCheckbox.IsChecked;
        Globals.Database.FriendOverheadHpBar = _friendOverheadHpBarCheckbox.IsChecked;
        Globals.Database.GuildMemberOverheadHpBar = _guildMemberOverheadHpBarCheckbox.IsChecked;
        Globals.Database.MyOverheadHpBar = _myOverheadHpBarCheckbox.IsChecked;
        Globals.Database.NpcOverheadHpBar = _mpcOverheadHpBarCheckbox.IsChecked;
        Globals.Database.PartyMemberOverheadHpBar = _partyMemberOverheadHpBarCheckbox.IsChecked;
        Globals.Database.PlayerOverheadHpBar = _playerOverheadHpBarCheckbox.IsChecked;
        Globals.Database.StickyTarget = _stickyTarget.IsChecked;
        Globals.Database.AutoTurnToTarget = _autoTurnToTarget.IsChecked;
        Globals.Database.TypewriterBehavior = _typewriterCheckbox.IsChecked ? Enums.TypewriterBehavior.Word : Enums.TypewriterBehavior.Off;

        // Video Settings.
        Globals.Database.WorldZoom = (float)_worldScale.Value;

        var resolution = _resolutionList.SelectedItem;
        var validVideoModes = Graphics.Renderer?.GetValidVideoModes();
        var targetResolution = validVideoModes?.FindIndex(videoMode => string.Equals(videoMode, resolution.Text)) ?? -1;
        var newFps = 0;

        Globals.Database.EnableLighting = _lightingEnabledCheckbox.IsChecked;

        if (targetResolution > -1)
        {
            shouldReset = Globals.Database.TargetResolution != targetResolution || Graphics.Renderer?.HasOverrideResolution == true;
            Globals.Database.TargetResolution = targetResolution;
        }

        if (Globals.Database.FullScreen != _fullscreenCheckbox.IsChecked)
        {
            Globals.Database.FullScreen = _fullscreenCheckbox.IsChecked;
            shouldReset = true;
        }

        if (_fpsList.SelectedItem.Text == Strings.Settings.UnlimitedFps)
        {
            newFps = -1;
        }
        else if (_fpsList.SelectedItem.Text == Strings.Settings.Fps30)
        {
            newFps = 1;
        }
        else if (_fpsList.SelectedItem.Text == Strings.Settings.Fps60)
        {
            newFps = 2;
        }
        else if (_fpsList.SelectedItem.Text == Strings.Settings.Fps90)
        {
            newFps = 3;
        }
        else if (_fpsList.SelectedItem.Text == Strings.Settings.Fps120)
        {
            newFps = 4;
        }

        if (newFps != Globals.Database.TargetFps)
        {
            shouldReset = true;
            Globals.Database.TargetFps = newFps;
        }

        // Audio Settings.
        Globals.Database.MusicVolume = (int)_musicSlider.Value;
        Globals.Database.SoundVolume = (int)_soundSlider.Value;
        Audio.UpdateGlobalVolume();

        // Control Settings.
        Controls.ActiveControls = _keybindingEditControls;
        Controls.ActiveControls.Save();

        // Save Preferences.
        Globals.Database.SavePreferences();

        if (shouldReset && Graphics.Renderer != default)
        {
            _customResolutionMenuItem?.Hide();
            Graphics.Renderer.OverrideResolution = Resolution.Empty;
            Graphics.Renderer.Init();
        }

        // Hide the currently opened window.
        Hide();
    }

    private void SettingsCancelBtn_Clicked(Base sender, ClickedEventArgs arguments)
    {
        // Update previously saved values in order to discard changes.
        Globals.Database.MusicVolume = _previousMusicVolume;
        Globals.Database.SoundVolume = _previousSoundVolume;
        Audio.UpdateGlobalVolume();
        _keybindingEditControls = new Controls(Controls.ActiveControls);

        // Hide our current window.
        Hide();
    }
}
