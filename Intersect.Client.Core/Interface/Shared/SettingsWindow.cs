using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Localization;
using Intersect.Config;
using Intersect.Core;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using static Intersect.Client.Framework.File_Management.GameContentManager;
using MathHelper = Intersect.Client.Utilities.MathHelper;

namespace Intersect.Client.Interface.Shared;

using BottomBarItems = (Panel BottomBar, Button RestoreDefaultControlsButton, Button ApplyPendingChangesButton, Button CancelPendingChangesButton);

public partial class SettingsWindow : Window
{
    private readonly GameFont? _defaultFont;

    // Bottom Bar
    private readonly Button _restoreDefaultsButton;
    private readonly Button _applyPendingChangesButton;
    private readonly Button _cancelPendingChangesButton;

    // Game Settings
    private readonly TabButton _gameSettingsTab;
    private readonly TabControl _gameContainer;

    // Game Settings - Interface
    private readonly TabButton _gameSettingsTabInterface;
    private readonly ScrollControl _interfaceSettings;
    private readonly LabeledCheckBox _autoCloseWindowsCheckbox;
    private readonly LabeledCheckBox _autoToggleChatLogCheckbox;
    private readonly LabeledCheckBox _showExperienceAsPercentageCheckbox;
    private readonly LabeledCheckBox _showHealthAsPercentageCheckbox;
    private readonly LabeledCheckBox _showManaAsPercentageCheckbox;
    private readonly LabeledCheckBox _simplifiedEscapeMenu;

    // Game Settings - Information
    private readonly TabButton _gameSettingsTabInformation;
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
    private readonly LabeledCheckBox _npcOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _partyMemberOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _playerOverheadHpBarCheckbox;
    private readonly LabeledCheckBox _typewriterCheckbox;

    // Game Settings - Targeting
    private readonly TabButton _gameSettingsTabTargeting;
    private readonly ScrollControl _targetingSettings;
    private readonly LabeledCheckBox _stickyTarget;
    private readonly LabeledCheckBox _autoTurnToTarget;
    private readonly LabeledCheckBox _autoSoftRetargetOnSelfCast;

    // Video Settings
    private readonly TabButton _videoSettingsTab;
    private readonly ScrollControl _videoContainer;
    private readonly LabeledComboBox _resolutionList;
    private readonly LabeledComboBox _fpsList;
    private readonly LabeledSlider _worldScale;
    private readonly LabeledCheckBox _fullscreenCheckbox;
    private readonly LabeledCheckBox _lightingEnabledCheckbox;
    private MenuItem? _customResolutionMenuItem;

    // Audio Settings
    private readonly TabButton _audioSettingsTab;
    private readonly ScrollControl _audioContainer;
    private readonly LabeledSlider _musicSlider;
    private readonly LabeledSlider _soundEffectsSlider;

    private int _previousMusicVolume;
    private int _previousSoundVolume;

    // Controls
    private readonly TabButton _controlsTab;
    private readonly ScrollControl _controlsContainer;
    private readonly Table _controlsTable;
    private readonly Dictionary<Control, Button[]> _controlBindingButtons = [];
    private readonly Panel _bottomBar;
    private readonly TabControl _tabs;

    private readonly HashSet<Keys> _keysDown = [];
    private Control _keybindingEditControl;
    private Controls? _keybindingEditControls;
    private Button? _keybindingEditBtn;
    private long _keybindingListeningTimer;
    private int _keyEdit = -1;

    private Base? _returnTo;

    // Initialize.
    public SettingsWindow(Base parent) : base(parent: parent, title: Strings.Settings.Title, modal: false, name: nameof(SettingsWindow))
    {
        Interface.InputBlockingComponents.Add(item: this);

        IconName = "SettingsWindow.icon.png";

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 640, y: 400);
        IsResizable = false;
        IsClosable = false;

        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _defaultFont = Current.GetFont(name: TitleLabel.FontName, 12);

#region Game

        // Game Settings are stored in the GameSettings Scroll Control.
        _gameContainer = new TabControl(parent: this, name: nameof(_gameContainer))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            Font = _defaultFont,
            TabStripPosition = Pos.Left,
        };
        _gameContainer.TabChanged += GameContainerOnTabChanged;

        _interfaceSettings = new ScrollControl(parent: _gameContainer, name: nameof(_interfaceSettings))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            InnerPanelPadding = new Padding(4),
        };
        _gameSettingsTabInterface = _gameContainer.AddPage(
            label: Strings.Settings.InterfaceSettings,
            tabName: nameof(_gameSettingsTabInterface),
            page: _interfaceSettings
        );

        _informationSettings = new ScrollControl(parent: _gameContainer, name: nameof(_informationSettings))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            InnerPanelPadding = new Padding(4),
        };
        _gameSettingsTabInformation = _gameContainer.AddPage(
            label: Strings.Settings.InformationSettings,
            tabName: nameof(_gameSettingsTabInformation),
            page: _informationSettings
        );

        _targetingSettings = new ScrollControl(parent: _gameContainer, name: nameof(_targetingSettings))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            InnerPanelPadding = new Padding(4),
        };
        _gameSettingsTabTargeting = _gameContainer.AddPage(
            label: Strings.Settings.TargetingSettings,
            tabName: nameof(_gameSettingsTabTargeting),
            page: _targetingSettings
        );

        // Game Settings - Interface.

        // Game Settings - Interface: Auto-close Windows.
        _autoCloseWindowsCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_autoCloseWindowsCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.AutoCloseWindows,
        };

        // Game Settings - Interface: Auto-toggle chat log visibility.
        _autoToggleChatLogCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_autoToggleChatLogCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.AutoToggleChatLog,
        };

        // Game Settings - Interface: Show EXP/HP/MP as Percentage.
        _showExperienceAsPercentageCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_showExperienceAsPercentageCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowExperienceAsPercentage,
        };

        _showHealthAsPercentageCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_showHealthAsPercentageCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowHealthAsPercentage,
        };

        _showManaAsPercentageCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_showManaAsPercentageCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowManaAsPercentage,
        };

        // Game Settings - Interface: simplified escape menu.
        _simplifiedEscapeMenu = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_simplifiedEscapeMenu))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.SimplifiedEscapeMenu,
        };

        // Game Settings - Typewriter Text
        _typewriterCheckbox = new LabeledCheckBox(parent: _interfaceSettings, name: nameof(_typewriterCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.TypewriterText,
        };

        // Game Settings - Information: Friends.
        _friendOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_friendOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowFriendOverheadInformation,
        };

        // Game Settings - Information: Guild Members.
        _guildMemberOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_guildMemberOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowGuildOverheadInformation,
        };

        // Game Settings - Information: Myself.
        _myOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_myOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowMyOverheadInformation,
        };

        // Game Settings - Information: NPCs.
        _npcOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_npcOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowNpcOverheadInformation,
        };

        // Game Settings - Information: Party Members.
        _partyMemberOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_partyMemberOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowPartyOverheadInformation,
        };

        // Game Settings - Information: Players.
        _playerOverheadInfoCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_playerOverheadInfoCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowPlayerOverheadInformation,
        };

        // Game Settings - Information: friends overhead hp bar.
        _friendOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_friendOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowFriendOverheadHpBar,
        };

        // Game Settings - Information: guild members overhead hp bar.
        _guildMemberOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_guildMemberOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowGuildOverheadHpBar,
        };

        // Game Settings - Information: my overhead hp bar.
        _myOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_myOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowMyOverheadHpBar,
        };

        // Game Settings - Information: NPC overhead hp bar.
        _npcOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_npcOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowNpcOverheadHpBar,
        };

        // Game Settings - Information: party members overhead hp bar.
        _partyMemberOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_partyMemberOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowPartyOverheadHpBar,
        };

        // Game Settings - Information: players overhead hp bar.
        _playerOverheadHpBarCheckbox = new LabeledCheckBox(parent: _informationSettings, name: nameof(_playerOverheadHpBarCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.ShowPlayerOverheadHpBar,
        };

        // Game Settings - Targeting: Sticky Target.
        _stickyTarget = new LabeledCheckBox(parent: _targetingSettings, name: nameof(_stickyTarget))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.StickyTarget,
        };

        // Game Settings - Targeting: Auto-turn to Target.
        _autoTurnToTarget = new LabeledCheckBox(parent: _targetingSettings, name: nameof(_autoTurnToTarget))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.AutoTurnToTarget,
        };

        // Game Settings - Targeting: Auto-turn to Target.
        _autoSoftRetargetOnSelfCast = new LabeledCheckBox(parent: _targetingSettings, name: nameof(_autoSoftRetargetOnSelfCast))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.AutoSoftRetargetOnSelfCast,
            TooltipText = Strings.Settings.AutoSoftRetargetOnSelfCastTooltip,
            TooltipBackgroundName = "tooltip.png",
            TooltipFont = _defaultFont,
            TooltipTextColor = Color.White,
        };

#endregion Game

#region Video

        // Video Settings Get Stored in the VideoSettings Scroll Control.
        _videoContainer = new ScrollControl(parent: this, name: nameof(_videoContainer))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            InnerPanelPadding = new Padding(4),
        };

        // Video Settings - Resolution List.
        _resolutionList = new LabeledComboBox(parent: _videoContainer, name: nameof(_resolutionList))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Label = Strings.Settings.Resolution,
            TextPadding = new Padding(8, 4, 0, 4),
        };

        var availableVideoModes = Graphics.Renderer?.GetValidVideoModes().ToArray() ?? [];
        for (var videoModeIndex = 0; videoModeIndex < availableVideoModes.Length; videoModeIndex++)
        {
            var availableVideoMode = availableVideoModes[videoModeIndex];
            var resolutionParts = availableVideoMode.Split(',');
            var resolutionLabel = Strings.Settings.FormatResolution.ToString(resolutionParts.Cast<object>().ToArray());
            var addedItem = _resolutionList.AddItem(label: resolutionLabel, userData: videoModeIndex);
            addedItem.TextAlign = Pos.Left;
        }

        // Video Settings - FPS List.
        _fpsList = new LabeledComboBox(parent: _videoContainer, name: nameof(_fpsList))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Label = Strings.Settings.FPS,
            TextPadding = new Padding(8, 4, 0, 4),
        };
        _ = _fpsList.AddItem(label: Strings.Settings.Vsync);
        _ = _fpsList.AddItem(label: Strings.Settings.Fps30);
        _ = _fpsList.AddItem(label: Strings.Settings.Fps60);
        _ = _fpsList.AddItem(label: Strings.Settings.Fps90);
        _ = _fpsList.AddItem(label: Strings.Settings.Fps120);
        _ = _fpsList.AddItem(label: Strings.Settings.UnlimitedFps);

        // Video Settings - Fullscreen Checkbox.
        _fullscreenCheckbox = new LabeledCheckBox(parent: _videoContainer, name: nameof(_fullscreenCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.Fullscreen,
        };

        // Video Settings - Enable Lighting Checkbox
        _lightingEnabledCheckbox = new LabeledCheckBox(parent: _videoContainer, name: nameof(_lightingEnabledCheckbox))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Text = Strings.Settings.EnableLighting,
        };

        _worldScale = new LabeledSlider(parent: _videoContainer, name: nameof(_worldScale))
        {
            Dock = Pos.Top,
            IsDisabled = !Options.IsLoaded,
            Font = _defaultFont,
            Label = Strings.Settings.WorldScale,
            Orientation = Orientation.LeftToRight,
            SnapToNotches = true,
            ValueFormatString = Strings.Settings.FormatZoom,
        };

#endregion Video

#region Audio

        // Audio Settings Get Stored in the AudioSettings Scroll Control.
        _audioContainer = new ScrollControl(parent: this, name: nameof(_audioContainer))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            InnerPanelPadding = new Padding(4),
        };

        // Audio Settings - Music Slider
        _musicSlider = new LabeledSlider(parent: _audioContainer, name: nameof(_musicSlider))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Orientation = Orientation.LeftToRight,
            Height = 35,
            DraggerSize = new Point(9, 9),
            SliderSize =  new Point(200, 35),
            Label = Strings.Settings.VolumeMusic,
            LabelMinimumSize = new Point(100, 0),
            Rounding = 0,
            Minimum = 0,
            Maximum = 100,
            NotchCount = 5,
            SnapToNotches = false,
        };

        _musicSlider.ValueChanged += MusicSliderOnValueChanged;
        _musicSlider.SetSound("octave-tap-resonant.wav", ButtonSoundState.Hover);
        _musicSlider.SetSound("octave-tap-professional.wav", ButtonSoundState.MouseDown);
        _musicSlider.SetSound("octave-tap-professional.wav", ButtonSoundState.MouseUp);

        // Audio Settings - Sound Slider
        _soundEffectsSlider = new LabeledSlider(parent: _audioContainer, name: nameof(_soundEffectsSlider))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            Orientation = Orientation.LeftToRight,
            Height = 35,
            DraggerSize = new Point(9, 9),
            SliderSize =  new Point(200, 35),
            Label = Strings.Settings.VolumeSoundEffects,
            LabelMinimumSize = new Point(100, 0),
            Rounding = 0,
            Minimum = 0,
            Maximum = 100,
            NotchCount = 5,
            SnapToNotches = false,
        };

        _soundEffectsSlider.ValueChanged += SoundEffectsSliderOnValueChanged;
        _soundEffectsSlider.SetSound("octave-tap-resonant.wav", ButtonSoundState.Hover);
        _soundEffectsSlider.SetSound("octave-tap-professional.wav", ButtonSoundState.MouseDown);
        _soundEffectsSlider.SetSound("octave-tap-professional.wav", ButtonSoundState.MouseUp);

#endregion Audio

#region Controls

        // KeybindingSettings Get Stored in the KeybindingSettings Scroll Control
        _controlsContainer = new ScrollControl(parent: this, name: nameof(_controlsContainer))
        {
            Dock = Pos.Fill,
            InnerPanelPadding = new Padding(4),
        };

        _controlsTable = new Table(parent: _controlsContainer, name: nameof(_controlsTable))
        {
            Dock = Pos.Top,
            DockChildSpacing = new Padding(0, 4, 0, 0),
            ColumnCount = 3,
            Font = _defaultFont,
            SizeToContents = true,
        };

        // Keybinding Settings - Controls
        var row = 0;
        foreach (var control in (_keybindingEditControls ?? Controls.ActiveControls).Mappings.Keys)
        {
            AddControlKeybindRow(control: control, row: ref row, keyButtons: out _);
        }

        Input.KeyDown += OnKeyDown;
        Input.MouseDown += OnKeyDown;
        Input.KeyUp += OnKeyUp;
        Input.MouseUp += OnKeyUp;

#endregion Controls

        _tabs = new TabControl(parent: this, name: nameof(_tabs))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            Margin = new Margin(left: 4, top: 4, right: 4, bottom: 0),
        };

        _gameSettingsTab = _tabs.AddPage(
            label: Strings.Settings.GameSettingsTab,
            page: _gameContainer,
            tabName: nameof(_gameSettingsTab)
        );
        _videoSettingsTab = _tabs.AddPage(
            label: Strings.Settings.VideoSettingsTab,
            page: _videoContainer,
            tabName: nameof(_videoSettingsTab)
        );
        _audioSettingsTab = _tabs.AddPage(
            label: Strings.Settings.AudioSettingsTab,
            page: _audioContainer,
            tabName: nameof(_audioSettingsTab)
        );
        _controlsTab = _tabs.AddPage(
            label: Strings.Settings.ControlsTab,
            page: _controlsContainer,
            tabName: nameof(_controlsTab)
        );
        _tabs.TabChanged += TabsOnTabChanged;

        (_bottomBar, _restoreDefaultsButton, _applyPendingChangesButton, _cancelPendingChangesButton) =
            CreateBottomBar(this);
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(stage: UI.Shared, resolution: Graphics.Renderer?.GetResolutionString());
    }

    public override bool IsBlockingInput => _keybindingEditBtn is not null;

    private BottomBarItems CreateBottomBar(Base parent)
    {
        var bottomBar = new Panel(parent: parent, name: nameof(_bottomBar))
        {
            Dock = Pos.Bottom,
            MinimumSize = new Point(x: 0, y: 40),
            Margin = Margin.Four,
            Padding = new Padding(horizontal: 8, vertical: 4),
        };

        // Keybinding Settings - Restore Default Keys Button.
        var restoreDefaultKeybindingsButton = new Button(parent: bottomBar, name: nameof(_restoreDefaultsButton))
        {
            Alignment = [Alignments.Left, Alignments.CenterV],
            AutoSizeToContents = true,
            Font = _defaultFont,
            IsVisible = false,
            MinimumSize = new Point(x: 96, y: 24),
            Padding = new Padding(horizontal: 16, vertical: 2),
            Text = Strings.Settings.Restore,
        };
        restoreDefaultKeybindingsButton.Clicked += RestoreDefaultKeybindingsButton_Clicked;

        // Apply Button.
        var applyPendingChangesButton = new Button(parent: bottomBar, name: nameof(_applyPendingChangesButton))
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(x: 96, y: 24),
            Padding = new Padding(horizontal: 16, vertical: 2),
            Text = Strings.Settings.Apply,
        };
        applyPendingChangesButton.Clicked += SettingsApplyBtn_Clicked;

        // Cancel Button.
        var cancelPendingChangesButton = new Button(parent: bottomBar, name: nameof(_cancelPendingChangesButton))
        {
            Alignment = [Alignments.Right, Alignments.CenterV],
            AutoSizeToContents = true,
            Font = _defaultFont,
            MinimumSize = new Point(x: 96, y: 24),
            Padding = new Padding(horizontal: 16, vertical: 2),
            Text = Strings.Settings.Cancel,
        };
        cancelPendingChangesButton.Clicked += CancelPendingChangesButton_Clicked;

        return (
            BottomBar: bottomBar,
            RestoreDefaultControlsButton: restoreDefaultKeybindingsButton,
            ApplyPendingChangesButton: applyPendingChangesButton,
            CancelPendingChangesButton: cancelPendingChangesButton
        );
    }

    private void GameContainerOnTabChanged(Base sender, TabChangeEventArgs arguments)
    {
        // ReSharper disable once InvertIf
        if (arguments.ActiveTab == _gameSettingsTabTargeting)
        {
            _autoTurnToTarget.IsDisabled = !(Options.Instance?.Player?.EnableAutoTurnToTarget ?? false);
            _autoSoftRetargetOnSelfCast.IsDisabled =
                !(Options.Instance?.Combat?.EnableAutoSelfCastFriendlySpellsWhenTargetingHostile ?? false);
        }
    }

    private void TabsOnTabChanged(Base @base, TabChangeEventArgs tabChangeEventArgs)
    {
        if (_controlsTab.IsTabActive)
        {
            _restoreDefaultsButton.IsVisible = true;

            bool controlsAdded = false;

            _controlsTable.FitContents();

            var row = _controlBindingButtons.Count;
            foreach (var (control, mapping) in (_keybindingEditControls ?? Controls.ActiveControls).Mappings)
            {
                if (!_controlBindingButtons.TryGetValue(control, out var controlButtons))
                {
                    controlsAdded |= AddControlKeybindRow(control, ref row, out controlButtons);
                }

                var bindings = mapping.Bindings;
                for (var bindingIndex = 0; bindingIndex < bindings.Count; bindingIndex++)
                {
                    var binding = bindings[bindingIndex];
                    controlButtons[bindingIndex].Text = Strings.Keys.FormatKeyName(binding.Modifier, binding.Key);
                }
            }

            if (controlsAdded)
            {
                // Current.SaveUIJson(UI.Shared, Name, GetJsonUI(true), Graphics.Renderer?.GetResolutionString());
            }
        }
        else
        {
            _restoreDefaultsButton.IsVisible = false;
        }
    }

    private bool AddControlKeybindRow(Control control, ref int row, out Button[] keyButtons)
    {
        if (_controlBindingButtons.TryGetValue(control, out var existingButtons))
        {
            keyButtons = existingButtons;
            return false;
        }

        GameFont? defaultFont = Current.GetFont("sourcesansproblack", 10);

            var offset = row++ * 32;
        var controlName = control.GetControlId();
            var name = controlName?.ToLower() ?? string.Empty;

        if (!Strings.Controls.KeyDictionary.TryGetValue(name, out var localizedControlName))
        {
            var hotbarSlotCount = Options.Instance?.Player.HotbarSlotCount ?? PlayerOptions.DefaultHotbarSlotCount;
            var hotkeySlotHumanNumber = control - Control.HotkeyOffset;
            if (0 < hotkeySlotHumanNumber && hotkeySlotHumanNumber <= hotbarSlotCount)
            {
                localizedControlName = Strings.Controls.HotkeyXLabel.ToString(hotkeySlotHumanNumber);
            }
            else
            {
                localizedControlName = $"ControlName:{controlName}";
            }
        }

        var controlRow = _controlsTable.AddRow();

        var prefix = $"Control{controlName}";
        var controlLabel = new Label(controlRow, $"{prefix}Label")
        {
            Alignment = [Alignments.CenterV, Alignments.Right],
            Margin = new Margin(0, 0, 8, 0),
            Text = localizedControlName,
            AutoSizeToContents = true,
            MouseInputEnabled = false,
            Font = defaultFont,
        };

        controlRow.SetCellContents(0, controlLabel, enableMouseInput: false);

        var key1 = new Button(controlRow, $"{prefix}Button1")
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = false,
            Font = defaultFont,
            MinimumSize = new Point(150, 20),
            Text = string.Empty,
            UserData = new KeyValuePair<Control, int>(control, 0),
        };
        controlRow.SetCellContents(1, key1, enableMouseInput: true);
        key1.Clicked += Key_Clicked;

        var key2 = new Button(controlRow, $"{prefix}Button2")
        {
            Alignment = [Alignments.Center],
            AutoSizeToContents = false,
            Font = defaultFont,
            MinimumSize = new Point(150, 20),
            Text = string.Empty,
            UserData = new KeyValuePair<Control, int>(control, 1),
        };
        controlRow.SetCellContents(2, key2, enableMouseInput: true);
        key2.Clicked += Key_Clicked;

        keyButtons = [key1, key2];
        _controlBindingButtons.Add(control, keyButtons);
        return true;
    }

    protected override void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnVisibilityChanged(sender, eventArgs);

        if (eventArgs.IsVisible)
        {
            UpdateWorldScaleControls();
        }
    }

    private void UpdateWorldScaleControls()
    {
        var worldScaleNotches = new double[] { 1, 2, 4 }.Select(n => n * Graphics.MinimumWorldScale).ToList();
        while (worldScaleNotches.Last() < Graphics.MaximumWorldScale)
        {
            worldScaleNotches.Add(worldScaleNotches.Last() * 2);
        }

        if (Options.IsLoaded)
        {
            _worldScale.IsDisabled = false;
            _worldScale.SetToolTipText(null);

            Globals.Database.WorldZoom = (float)MathHelper.Clamp(
                Globals.Database.WorldZoom,
                worldScaleNotches.Min(),
                worldScaleNotches.Max()
            );
        }
        else
        {
            _worldScale.SetToolTipText(Strings.Settings.WorldScaleTooltip);
            _worldScale.IsDisabled = true;
        }

        _worldScale.SetRange(worldScaleNotches.Min(), worldScaleNotches.Max());
        _worldScale.Notches = worldScaleNotches.ToArray();
        _worldScale.Value = Globals.Database.WorldZoom;
    }

    void InterfaceSettings_Clicked(Base sender, MouseButtonState arguments)
    {
        _interfaceSettings.Show();
        _informationSettings.Hide();
        _targetingSettings.Hide();
    }

    void InformationSettings_Clicked(Base sender, MouseButtonState arguments)
    {
        _interfaceSettings.Hide();
        _informationSettings.Show();
        _targetingSettings.Hide();
    }

    void TargetingSettings_Clicked(Base sender, MouseButtonState arguments)
    {
        _interfaceSettings.Hide();
        _informationSettings.Hide();
        _targetingSettings.Show();
        _autoTurnToTarget.IsDisabled = !(Options.Instance?.Player?.EnableAutoTurnToTarget ?? false);
        _autoSoftRetargetOnSelfCast.IsDisabled =
            !(Options.Instance?.Combat?.EnableAutoSelfCastFriendlySpellsWhenTargetingHostile ?? false);
    }

    private void Reset()
    {
        Title = Strings.Settings.Title;

        _gameSettingsTab.Select();

        UpdateWorldScaleControls();
    }

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
            foreach (var control in _keybindingEditControls.Mappings)
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
                        _controlBindingButtons[control.Key][bindingIndex].Text = Strings.Keys.KeyDictionary[Enum.GetName(typeof(Keys), Keys.None)?.ToLower() ?? string.Empty];
                    }
                }
            }

            _keybindingEditBtn.PlaySound(ButtonSoundState.Hover);
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

    public override void Show() => Show(returnTo: null);

    public void Show(Base? returnTo)
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
        _npcOverheadHpBarCheckbox.IsChecked = Globals.Database.NpcOverheadHpBar;
        _partyMemberOverheadHpBarCheckbox.IsChecked = Globals.Database.PartyMemberOverheadHpBar;
        _playerOverheadHpBarCheckbox.IsChecked = Globals.Database.PlayerOverheadHpBar;
        _stickyTarget.IsChecked = Globals.Database.StickyTarget;
        _autoTurnToTarget.IsChecked = Globals.Database.AutoTurnToTarget;
        _autoSoftRetargetOnSelfCast.IsChecked = Globals.Database.AutoSoftRetargetOnSelfCast;
        _typewriterCheckbox.IsChecked = Globals.Database.TypewriterBehavior == Enums.TypewriterBehavior.Word;

        // Video Settings.
        _fullscreenCheckbox.IsChecked = Globals.Database.FullScreen;
        _lightingEnabledCheckbox.IsChecked = Globals.Database.EnableLighting;

        // _uiScale.Value = Globals.Database.UIScale;

        if (Graphics.Renderer?.GetValidVideoModes().Count > 0)
        {
            if (Graphics.Renderer.HasOverrideResolution)
            {
                _customResolutionMenuItem ??= _resolutionList.AddItem(
                    label: Strings.Settings.ResolutionCustom,
                    userData: -1
                );
                _customResolutionMenuItem.Show();
                _resolutionList.SelectedItem = _customResolutionMenuItem;
            }
            else
            {
                _resolutionList.SelectByUserData(Globals.Database.TargetResolution);
            }
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
        _soundEffectsSlider.Value = Globals.Database.SoundVolume;

        // Control Settings.
        _keybindingEditControls = new Controls(Controls.ActiveControls);

        // Settings Window is not hidden anymore.
        base.Show();

        // Load every GUI element to their default state when showing up the settings window (pressed tabs, containers, etc.)
        Reset();

        // Set up whether we're supposed to return to the previous menu.
        _returnTo = returnTo;
    }

    public override void Hide()
    {
        // Hide the current window.
        base.Hide();
        RemoveModal();

        // Return to our previous menus (or not) depending on gamestate and the method we'd been opened.
        _returnTo?.Show();
        _returnTo = null;
    }

    // Input Handlers
    private void MusicSliderOnValueChanged(Base sender, ValueChangedEventArgs<double> arguments)
    {
        Globals.Database.MusicVolume = (int)arguments.Value;
        ApplicationContext.CurrentContext.Logger.LogInformation("Music volume set to {MusicVolume}", arguments.Value);
        Audio.UpdateGlobalVolume();
    }

    private void SoundEffectsSliderOnValueChanged(Base sender, ValueChangedEventArgs<double> arguments)
    {
        Globals.Database.SoundVolume = (int)arguments.Value;
        ApplicationContext.CurrentContext.Logger.LogInformation("Sound volume set to {SoundVolume}", arguments.Value);
        Audio.UpdateGlobalVolume();
    }

    private void Key_Clicked(Base sender, MouseButtonState arguments)
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

    private void RestoreDefaultKeybindingsButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_keybindingEditControls is not {} controls)
        {
            return;
        }

        controls.ResetDefaults();
        foreach (Control control in GameInput.Current.AllControls)
        {
            if (!controls.TryGetMappingFor(control, out var mapping))
            {
                continue;
            }

            for (var bindingIndex = 0; bindingIndex < mapping.Bindings.Count; bindingIndex++)
            {
                var binding = mapping.Bindings[bindingIndex];
                _controlBindingButtons[control][bindingIndex].Text = Strings.Keys.FormatKeyName(binding.Modifier, binding.Key);
            }
        }
    }

    private void SettingsApplyBtn_Clicked(Base sender, MouseButtonState arguments)
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
        Globals.Database.NpcOverheadHpBar = _npcOverheadHpBarCheckbox.IsChecked;
        Globals.Database.PartyMemberOverheadHpBar = _partyMemberOverheadHpBarCheckbox.IsChecked;
        Globals.Database.PlayerOverheadHpBar = _playerOverheadHpBarCheckbox.IsChecked;
        Globals.Database.StickyTarget = _stickyTarget.IsChecked;
        Globals.Database.AutoTurnToTarget = _autoTurnToTarget.IsChecked;
        Globals.Database.AutoSoftRetargetOnSelfCast = _autoSoftRetargetOnSelfCast.IsChecked;
        Globals.Database.TypewriterBehavior = _typewriterCheckbox.IsChecked ? Enums.TypewriterBehavior.Word : Enums.TypewriterBehavior.Off;

        // Video Settings.
        Globals.Database.WorldZoom = (float)_worldScale.Value;

        var resolutionItem = _resolutionList.SelectedItem;
        var targetResolution = resolutionItem?.UserData as int? ?? -1;
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
        Globals.Database.SoundVolume = (int)_soundEffectsSlider.Value;
        Audio.UpdateGlobalVolume();

        // Control Settings.
        var activeControls = _keybindingEditControls ?? Controls.ActiveControls;
        Controls.ActiveControls = activeControls;
        activeControls.TrySave();

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

    private void CancelPendingChangesButton_Clicked(Base sender, MouseButtonState arguments)
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
