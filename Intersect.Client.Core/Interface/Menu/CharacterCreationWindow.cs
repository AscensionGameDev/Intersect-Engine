using Intersect.Framework.Core.GameObjects.Factions;
using Intersect.Network.Packets;
using Intersect.Client.Entities;
using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.Framework.Reflection;
using Intersect.GameObjects;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace Intersect.Client.Interface.Menu;

public partial class CharacterCreationWindow : Window
{
    private readonly MainMenu _mainMenu;
    private readonly SelectCharacterWindow _selectCharacterWindow;

    private readonly IFont? _defaultFont;

    private readonly Panel _previewPanel;
    private readonly Panel _previewContainer;
    private readonly Panel _propertiesPanel;
    private readonly Panel _buttonsPanel;

    private readonly TextBox _nameInput;

    // Fantasy class carousel: replaces the old plain class dropdown. One portrait per unlocked class,
    // laid out left-to-right around whichever index is "centered"; scale/opacity are faked (this GWEN
    // fork has no transform support) by lerping each portrait's pixel Bounds/RenderColor alpha every
    // frame towards a target computed from its circular distance to the centered class.
    private readonly Panel _carouselPanel;
    private readonly Panel _carouselPortraitLayer;
    private readonly Button _carouselLeftButton;
    private readonly Button _carouselRightButton;
    private readonly Label _carouselClassName;
    private readonly Label _carouselClassDescription;
    private readonly Panel _statBarsPanel;
    private readonly (Label Label, ProgressBar Bar)[] _statRows;
    private ClassDescriptor[] _carouselClasses = [];
    private readonly List<ImagePanel> _carouselPortraits = [];
    private readonly List<Label> _carouselFallbackLabels = [];
    private readonly List<(double X, double Y, double W, double H, double Alpha)> _carouselCurrent = [];
    private readonly List<(double X, double Y, double W, double H, double Alpha)> _carouselTarget = [];
    private int _centerClassIndex;
    private long _lastCarouselAnimMs;
    private int _maxStatIncrease = 1;
    private const int CarouselPortraitBaseSize = 120;
    private const int CarouselPitch = 170;
    private const int CarouselMaxVisibleDistance = 2;
    private const int CarouselWidth = 860;
    private const int CarouselTotalHeight = 310;

    /// <summary>
    /// Two-step flow: false = choosing a class (carousel visible, preview/properties hidden), true =
    /// customizing the chosen class (name/faction/gender/colors visible, carousel hidden). Keeps the
    /// window small instead of showing everything - carousel AND full customization - at once.
    /// </summary>
    private bool _isCustomizeStep;

    private readonly LabeledComboBox _factionCombobox;

    private readonly Panel _genderInputPanel;
    private readonly LabeledCheckBox _genderMaleCheckbox;
    private readonly LabeledCheckBox _genderFemaleCheckbox;

    // Dofus-style customisation: interchangeable parts, one colour per "_cN" layer, edited with an HSV picker
    private readonly Panel _colorsPanel;
    private readonly Label _colorsTitle;
    private readonly Button _randomColorsButton;
    private readonly ImagePanel _pickerPreview;
    private readonly Label _zoneNameLabel;
    private readonly Label _hueLabel;
    private readonly Slider _hueSlider;
    private readonly Label _saturationLabel;
    private readonly Slider _saturationSlider;
    private readonly Label _brightnessLabel;
    private readonly Slider _brightnessSlider;
    private readonly List<ImagePanel> _quickSwatches = [];
    private readonly List<PartRow> _partRows = [];
    private readonly List<ZoneBox> _zoneBoxes = [];
    private readonly List<(SpritePart? Part, int Layer)> _zones = [];
    private readonly List<Color> _selectedColors = [];
    private readonly List<SpritePart> _selectedParts = [];
    private string? _customizedSprite;
    private int _selectedZone;
    private bool _updatingPicker;
    private static readonly Random ColorRandom = new();
    private const int SwatchPitch = 18;
    private const int BlockWidth = 252;

    private sealed class PartRow
    {
        public required Panel Row;
        public required Label Counter;
        public required string Category;
    }

    private sealed class ZoneBox
    {
        public required ImagePanel Frame;
        public required ImagePanel Fill;
    }

    private static readonly Color[] ColorPalette =
    [
        new(255, 242, 211, 179), // pale skin
        new(255, 198, 134, 66),  // tan
        new(255, 107, 62, 38),   // brown
        new(255, 27, 27, 27),    // black
        new(255, 245, 245, 245), // white
        new(255, 230, 57, 70),   // red
        new(255, 247, 127, 0),   // orange
        new(255, 255, 214, 10),  // yellow
        new(255, 45, 198, 83),   // green
        new(255, 30, 136, 229),  // blue
        new(255, 142, 68, 173),  // purple
        new(255, 240, 98, 146),  // pink
    ];

    private readonly ImagePanel _preview;
    private ImagePanel[]? _renderLayers;
    private readonly Button _nextSpriteButton;
    private readonly Button _prevSpriteButton;

    // Buttons
    private readonly Button _createButton;

    private int _displaySpriteIndex = -1;
    private readonly List<KeyValuePair<int, ClassSprite>> _femaleSprites = [];
    private readonly List<KeyValuePair<int, ClassSprite>> _maleSprites = [];
    private Button _backButton;


    public CharacterCreationWindow(Canvas parent, MainMenu mainMenu, SelectCharacterWindow selectCharacterWindow) :
        base(parent, title: Strings.CharacterCreation.Title, modal: false, name: nameof(CharacterCreationWindow))
    {
        _mainMenu = mainMenu;
        _selectCharacterWindow = selectCharacterWindow;

        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack");

        Alignment = [Alignments.Center];
        MinimumSize = new Point(x: 560, y: 240);
        IsClosable = false;
        IsResizable = false;
        InnerPanelPadding = new Padding(8);
        Titlebar.MouseInputEnabled = false;
        TitleLabel.FontSize = 14;
        TitleLabel.TextColorOverride = Color.White;

        _carouselPanel = new Panel(this, name: nameof(_carouselPanel))
        {
            Dock = Pos.Top,
            ShouldDrawBackground = false,
        };
        _carouselPanel.SetSize(CarouselWidth, CarouselTotalHeight);
        _carouselPanel.MinimumSize = new Point(CarouselWidth, CarouselTotalHeight);
        _carouselPanel.MaximumSize = new Point(int.MaxValue, CarouselTotalHeight);

        _carouselPortraitLayer = new Panel(_carouselPanel, name: nameof(_carouselPortraitLayer))
        {
            Dock = Pos.Top,
            ShouldDrawBackground = false,
        };
        _carouselPortraitLayer.SetSize(CarouselWidth, 170);
        _carouselPortraitLayer.MinimumSize = new Point(CarouselWidth, 170);

        _carouselLeftButton = new Button(_carouselPortraitLayer, name: nameof(_carouselLeftButton), disableText: true)
        {
            Dock = Pos.Left | Pos.CenterV,
            MinimumSize = new Point(30, 35),
            MaximumSize = new Point(30, 35),
        };
        _carouselLeftButton.Clicked += CarouselLeftButton_Clicked;
        _carouselLeftButton.SetStateTexture(ComponentState.Normal, "button.arrow_left.normal.png");
        _carouselLeftButton.SetStateTexture(ComponentState.Disabled, "button.arrow_left.disabled.png");
        _carouselLeftButton.SetStateTexture(ComponentState.Hovered, "button.arrow_left.hovered.png");
        _carouselLeftButton.SetStateTexture(ComponentState.Active, "button.arrow_left.active.png");

        _carouselRightButton = new Button(_carouselPortraitLayer, name: nameof(_carouselRightButton), disableText: true)
        {
            Dock = Pos.Right | Pos.CenterV,
            MinimumSize = new Point(30, 35),
            MaximumSize = new Point(30, 35),
        };
        _carouselRightButton.Clicked += CarouselRightButton_Clicked;
        _carouselRightButton.SetStateTexture(ComponentState.Normal, "button.arrow_right.normal.png");
        _carouselRightButton.SetStateTexture(ComponentState.Disabled, "button.arrow_right.disabled.png");
        _carouselRightButton.SetStateTexture(ComponentState.Hovered, "button.arrow_right.hovered.png");
        _carouselRightButton.SetStateTexture(ComponentState.Active, "button.arrow_right.active.png");

        _carouselClassName = new Label(_carouselPanel, name: nameof(_carouselClassName))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 20,
            AutoSizeToContents = false,
            TextAlign = Pos.Center,
            TextColorOverride = Color.White,
        };
        _carouselClassName.SetSize(CarouselWidth, 28);

        _carouselClassDescription = new Label(_carouselPanel, name: nameof(_carouselClassDescription))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 11,
            AutoSizeToContents = false,
            TextAlign = Pos.Center,
            WrappingBehavior = WrappingBehavior.Wrapped,
        };
        _carouselClassDescription.SetSize(CarouselWidth, 36);

        _statBarsPanel = new Panel(_carouselPanel, name: nameof(_statBarsPanel))
        {
            Dock = Pos.Top,
            ShouldDrawBackground = false,
        };
        _statBarsPanel.SetSize(CarouselWidth, 76);
        _statBarsPanel.MinimumSize = new Point(CarouselWidth, 76);

        var statLabels = new[] { Strings.CharacterCreation.StatForce, Strings.CharacterCreation.StatDefense, Strings.CharacterCreation.StatMagic, Strings.CharacterCreation.StatAgility };
        _statRows = new (Label, ProgressBar)[statLabels.Length];
        const int statBlockX = (CarouselWidth - 380) / 2;
        for (var statRowIndex = 0; statRowIndex < statLabels.Length; statRowIndex++)
        {
            var rowLabel = new Label(_statBarsPanel, name: $"StatLabel{statRowIndex}")
            {
                Font = _defaultFont,
                FontSize = 11,
                AutoSizeToContents = false,
                TextAlign = Pos.Left | Pos.CenterV,
                Text = statLabels[statRowIndex].ToString(),
            };
            rowLabel.SetBounds(statBlockX, statRowIndex * 19, 90, 18);

            var rowBar = new ProgressBar(_statBarsPanel)
            {
                AutoLabel = false,
                IsHorizontal = true,
            };
            rowBar.SetBounds(statBlockX + 96, statRowIndex * 19 + 3, 284, 13);
            _statRows[statRowIndex] = (rowLabel, rowBar);
        }

        _buttonsPanel = new Panel(this, name: nameof(_buttonsPanel))
        {
            Dock = Pos.Bottom,
            Margin = new Margin(0, 8, 0, 0),
            ShouldDrawBackground = false,
        };

        _createButton = new Button(_buttonsPanel, name: nameof(_createButton))
        {
            Alignment = [Alignments.Left],
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Text = Strings.CharacterCreation.Create,
        };
        _createButton.Clicked += CreateButton_Clicked;

        _backButton = new Button(_buttonsPanel, name: nameof(_backButton))
        {
            Alignment = [Alignments.Right],
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(120, 24),
            Text = Strings.CharacterCreation.Back,
        };
        _backButton.Clicked += BackButton_Clicked;

        _propertiesPanel = new Panel(this, name: nameof(_propertiesPanel))
        {
            Dock = Pos.Right,
            DockChildSpacing = new Padding(8),
            Margin = new Margin(16, 0, 0, 0),
            ShouldDrawBackground = false,
        };

        _previewPanel = new Panel(this, name: nameof(_previewPanel))
        {
            Dock = Pos.Fill,
            ShouldDrawBackground = false,
        };

        _nameInput = new TextBox(_propertiesPanel, name: nameof(_nameInput))
        {
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            MinimumSize = new Point(240, 0),
            PlaceholderText = Strings.CharacterCreation.Name,
        };
        _nameInput.SubmitPressed += (sender, e) => TryCreateCharacter();

        _factionCombobox = new LabeledComboBox(_propertiesPanel, name: nameof(_factionCombobox))
        {
            AutoSizeToContents = false,
            Dock = Pos.Top,
            Font = _defaultFont,
            FontSize = 12,
            Label = Strings.CharacterCreation.Faction,
            IsHidden = true,
        };

        _genderInputPanel = new Panel(_propertiesPanel, name: nameof(_genderInputPanel))
        {
            Dock = Pos.Top,
            ShouldDrawBackground = false,
        };

        // Male Checkbox
        _genderMaleCheckbox = new LabeledCheckBox(_genderInputPanel, name: nameof(_genderMaleCheckbox))
        {
            AutoSizeToContents = true,
            Dock = Pos.Left | Pos.CenterV,
            Font = _defaultFont,
            FontSize = 12,
            IsChecked = true,
            Text = Strings.CharacterCreation.Male,
        };
        _genderMaleCheckbox.Checked += MaleCheckboxGenderChecked;
        _genderMaleCheckbox.Unchecked += FemaleCheckboxGenderChecked;

        _genderFemaleCheckbox = new LabeledCheckBox(_genderInputPanel, name: nameof(_genderFemaleCheckbox))
        {
            AutoSizeToContents = true,
            Dock = Pos.Right | Pos.CenterV,
            Font = _defaultFont,
            FontSize = 12,
            Text = Strings.CharacterCreation.Female,
        };
        _genderFemaleCheckbox.Checked += FemaleCheckboxGenderChecked;
        _genderFemaleCheckbox.Unchecked += MaleCheckboxGenderChecked;

        _colorsPanel = new Panel(_propertiesPanel, name: nameof(_colorsPanel))
        {
            Dock = Pos.Top,
            ShouldDrawBackground = false,
            IsHidden = true,
        };

        _colorsTitle = new Label(_colorsPanel, name: nameof(_colorsTitle))
        {
            Font = _defaultFont,
            FontSize = 12,
            AutoSizeToContents = false,
            Text = Strings.CharacterCreation.Colors,
        };

        _pickerPreview = new ImagePanel(_colorsPanel, name: nameof(_pickerPreview))
        {
            Texture = Graphics.Renderer?.WhitePixel,
            RenderColor = Color.White,
            MouseInputEnabled = false,
        };

        _zoneNameLabel = new Label(_colorsPanel, name: nameof(_zoneNameLabel))
        {
            Font = _defaultFont,
            FontSize = 10,
            AutoSizeToContents = false,
            TextAlign = Pos.Left | Pos.CenterV,
        };

        _hueLabel = CreatePickerLabel(nameof(_hueLabel), "H");
        _hueSlider = CreatePickerSlider(nameof(_hueSlider), 360);
        _saturationLabel = CreatePickerLabel(nameof(_saturationLabel), "S");
        _saturationSlider = CreatePickerSlider(nameof(_saturationSlider), 1);
        _brightnessLabel = CreatePickerLabel(nameof(_brightnessLabel), "L");
        _brightnessSlider = CreatePickerSlider(nameof(_brightnessSlider), 1);

        for (var paletteIndex = 0; paletteIndex < ColorPalette.Length; paletteIndex++)
        {
            var frame = new ImagePanel(_colorsPanel, name: $"QuickSwatch{paletteIndex}")
            {
                Texture = Graphics.Renderer?.WhitePixel,
                RenderColor = Color.Black,
                MouseInputEnabled = true,
                UserData = paletteIndex,
            };
            frame.Clicked += QuickSwatch_Clicked;

            var fill = new ImagePanel(frame, name: "Fill")
            {
                Texture = Graphics.Renderer?.WhitePixel,
                RenderColor = ColorPalette[paletteIndex],
                MouseInputEnabled = false,
            };
            fill.SetBounds(2, 2, 12, 12);
            _quickSwatches.Add(frame);
        }

        _randomColorsButton = new Button(_colorsPanel, name: nameof(_randomColorsButton))
        {
            Font = _defaultFont,
            FontSize = 10,
            Text = Strings.CharacterCreation.RandomColors,
        };
        _randomColorsButton.Clicked += (_, _) => RandomizeCustomization();

        _previewContainer = new Panel(_previewPanel, name: nameof(_previewContainer))
        {
            Dock = Pos.Fill,
            ShouldDrawBackground = false,
        };

        _prevSpriteButton = new Button(_previewPanel, name: nameof(_prevSpriteButton), disableText: true)
        {
            Dock = Pos.Left | Pos.CenterV,
            MinimumSize = new Point(30, 35),
            MaximumSize = new Point(30, 35),
        };
        _prevSpriteButton.Clicked += _prevSpriteButton_Clicked;
        _prevSpriteButton.SetStateTexture(ComponentState.Normal, "button.arrow_left.normal.png");
        _prevSpriteButton.SetStateTexture(ComponentState.Disabled, "button.arrow_left.disabled.png");
        _prevSpriteButton.SetStateTexture(ComponentState.Hovered, "button.arrow_left.hovered.png");
        _prevSpriteButton.SetStateTexture(ComponentState.Active, "button.arrow_left.active.png");

        _nextSpriteButton = new Button(_previewPanel, name: nameof(_nextSpriteButton), disableText: true)
        {
            Dock = Pos.Right | Pos.CenterV,
            MinimumSize = new Point(30, 35),
            MaximumSize = new Point(30, 35),
        };
        _nextSpriteButton.Clicked += _nextSpriteButton_Clicked;
        _nextSpriteButton.SetStateTexture(ComponentState.Normal, "button.arrow_right.normal.png");
        _nextSpriteButton.SetStateTexture(ComponentState.Disabled, "button.arrow_right.disabled.png");
        _nextSpriteButton.SetStateTexture(ComponentState.Hovered, "button.arrow_right.hovered.png");
        _nextSpriteButton.SetStateTexture(ComponentState.Active, "button.arrow_right.active.png");

        _preview = new ImagePanel(_previewContainer, name: nameof(_preview))
        {
            Alignment = [Alignments.Center],
            MaintainAspectRatio = true,
            TextureFilename = "character_preview_background.png",
        };

        _buttonsPanel.SizeToChildren(recursive: true);
        _propertiesPanel.SizeToChildren(recursive: true);
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.Menu, Graphics.Renderer?.GetResolutionString());

        BuildCarousel();

        ApplicationContext.Context.Value?.Logger.LogDebug(
            "Added {ClassCount} classes to the {WindowType}",
            _carouselClasses.Length,
            typeof(CharacterCreationWindow).GetName(qualified: true)
        );


        _factionCombobox.ClearItems();
        var factionDescriptors = FactionDescriptor.Lookup.Values.OfType<FactionDescriptor>().OrderBy(faction => faction.Name).ToArray();
        MenuItem? firstFactionItem = null;
        foreach (var factionDescriptor in factionDescriptors)
        {
            var factionItem = _factionCombobox.AddItem(factionDescriptor.Name, userData: factionDescriptor);
            firstFactionItem ??= factionItem;
        }

        _factionCombobox.IsHidden = factionDescriptors.Length == 0;
        if (firstFactionItem != null && _factionCombobox.SelectedItem == null)
        {
            _factionCombobox.SelectedItem = firstFactionItem;
        }

        // Size once now that every row's final visibility (in particular the faction row, which may or may not
        // exist) is known, so docked rows below it don't overlap.
        SizeToChildren(recursive: true);

        LoadClass();
        UpdateDisplay();
        SetStep(false);
    }

    public void Update()
    {
        if (!Networking.Network.IsConnected)
        {
            Hide();
            _mainMenu.Show();
            return;
        }

        // Re-Enable our buttons if we're not waiting for the server anymore with it disabled.
        if (!Globals.WaitingOnServer && _createButton.IsDisabled)
        {
            _createButton.Enable();
        }

        StepCarouselAnimation();
    }

    public override void Show() => Show(force: false);

    public void Show(bool force)
    {
        _backButton.IsVisibleInTree = true;
        _createButton.Alignment = force ? [Alignments.Center] : [Alignments.Left];
        SetStep(false);

        _renderLayers = new ImagePanel[Options.Instance.Equipment.Paperdoll.Down.Count];
        for (var i = 0; i < _renderLayers.Length; i++)
        {
            _renderLayers[i] = new ImagePanel(_preview)
            {
                Alignment = [Alignments.Center],
            };
        }

        base.Show();
    }

    //Methods
    private void UpdateDisplay()
    {
        var classDescriptor = GetClass();

        if (classDescriptor == default || _displaySpriteIndex == -1 || classDescriptor.Sprites.Count <= 0)
        {
            _colorsPanel.IsHidden = true;
            foreach (var renderLayer in _renderLayers)
            {
                renderLayer.IsVisibleInTree = false;
            }
            return;
        }

        var source = _genderMaleCheckbox.IsChecked ? _maleSprites[_displaySpriteIndex] : _femaleSprites[_displaySpriteIndex];

        var faceTexture = GameContentManager.Current.GetTexture(TextureType.Face, source.Value.Face);
        if (faceTexture != default)
        {
            var faceLayer = _renderLayers[0];
            var faceScale = Math.Min(
                _preview.InnerWidth / (double)faceTexture.Width,
                _preview.InnerHeight / (double)faceTexture.Height
            );
            var faceTextureWidth = (int)(faceTexture.Width * faceScale);
            var faceTextureHeight = (int)(faceTexture.Height * faceScale);
            var x = (_preview.Width - faceTextureWidth) / 2;
            var y = (_preview.Height - faceTextureHeight) / 2;
            faceLayer.ResetUVs();
            faceLayer.SetBounds(x, y, faceTextureWidth, faceTextureHeight);
            faceLayer.Texture = faceTexture;
            faceLayer.IsVisibleInTree = true;
            _colorsPanel.IsHidden = true;

            foreach (var renderLayer in _renderLayers.Skip(1))
            {
                renderLayer.IsVisibleInTree = false;
            }

            return;
        }

        string? playerSprite = null;

        // we are rendering the player facing down, then we need to know the render order of the equipments
        for (var paperdollLayerIndex = 0; paperdollLayerIndex < Options.Instance.Equipment.Paperdoll.Down.Count; paperdollLayerIndex++)
        {
            var paperdollLayerType = Options.Instance.Equipment.Paperdoll.Down[paperdollLayerIndex];
            var paperdollContainer = _renderLayers[paperdollLayerIndex];

            // handle player/equip rendering, we just need to find the correct texture
            if (string.Equals("Player", paperdollLayerType, StringComparison.Ordinal))
            {
                var spriteSource = source.Value.Sprite;
                var spriteTex = Globals.ContentManager.GetTexture(TextureType.Entity, spriteSource);
                paperdollContainer.Texture = spriteTex;
                playerSprite = spriteSource;
            }
            else
            {
                paperdollContainer.Texture = default;
                continue;
                // if (paperdollLayerIndex >= selectedPreviewMetadata.Equipment.Length)
                // {
                //     continue;
                // }
                //
                // var equipFragment = selectedPreviewMetadata.Equipment[paperdollLayerIndex];
                //
                // if (equipFragment == default)
                // {
                //     paperdollContainer.Texture = default;
                //     continue;
                // }
                //
                // paperdollContainer.Texture = Globals.ContentManager.GetTexture(TextureType.Paperdoll, equipFragment.Name);
                //
                // if (paperdollContainer.Texture != default)
                // {
                //     paperdollContainer.RenderColor = equipFragment.RenderColor;
                // }
            }

            var layerTexture = paperdollContainer.Texture;
            if (layerTexture == default)
            {
                paperdollContainer.Hide();
                continue;
            }

            var imgWidth = layerTexture.Width;
            var imgHeight = layerTexture.Height;
            var textureWidth = imgWidth / Options.Instance.Sprites.NormalFrames;
            var textureHeight = imgHeight / Options.Instance.Sprites.Directions;

            paperdollContainer.SetTextureRect(0, 0, textureWidth, textureHeight);
            _ = paperdollContainer.SetSize(textureWidth, textureHeight);

            var centerX = (_preview.Width / 2) - (paperdollContainer.Width / 2);
            var centerY = (_preview.Height / 2) - (paperdollContainer.Height / 2);
            paperdollContainer.SetPosition(centerX, centerY);

            if (string.Equals("Player", paperdollLayerType, StringComparison.Ordinal))
            {
                SyncCustomization(playerSprite);
                SpriteColorPreview.Apply(paperdollContainer, _selectedColors, _selectedParts, textureWidth, textureHeight);
            }

            paperdollContainer.Show();
        }
    }

    #region Customization (parts + colours)

    private Slider CreatePickerSlider(string name, double maximum)
    {
        var slider = new Slider(_colorsPanel, name: name)
        {
            Orientation = Orientation.LeftToRight,
        };
        slider.SetRange(0, maximum);
        slider.ValueChanged += (_, _) => PickerChanged();

        return slider;
    }

    private Label CreatePickerLabel(string name, string text) => new(_colorsPanel, name: name)
    {
        Font = _defaultFont,
        FontSize = 9,
        AutoSizeToContents = false,
        TextAlign = Pos.Center,
        Text = text,
    };

    /// <summary>
    /// Brings the part rows and colour zones in line with the sprite being previewed. Hides the whole block when the
    /// sprite has neither colour layers nor parts.
    /// </summary>
    private void SyncCustomization(string? spriteName)
    {
        var baseLayerCount = SpriteColorLayers.CountLayers(spriteName);
        var catalog = SpriteColorLayers.GetPartCatalog(spriteName);
        var spriteChanged = !string.Equals(_customizedSprite, spriteName, StringComparison.OrdinalIgnoreCase);
        _customizedSprite = spriteName;

        if (baseLayerCount == 0 && catalog.Count == 0)
        {
            if (!_colorsPanel.IsHidden)
            {
                _colorsPanel.IsHidden = true;
                LayoutCustomization();
            }

            return;
        }

        while (_selectedColors.Count < baseLayerCount)
        {
            _selectedColors.Add(RandomColor());
        }

        if (_selectedColors.Count > baseLayerCount)
        {
            _selectedColors.RemoveRange(baseLayerCount, _selectedColors.Count - baseLayerCount);
        }

        if (spriteChanged || _partRows.Count != catalog.Count)
        {
            // keep the player's picks for categories the new sprite also has
            var previous = _selectedParts.ToDictionary(part => part.Category, StringComparer.OrdinalIgnoreCase);
            _selectedParts.Clear();
            foreach (var category in catalog)
            {
                previous.TryGetValue(category.Category, out var old);
                var index = old != null ? Math.Min(old.Index, category.MaxIndex) : ColorRandom.Next(0, category.MaxIndex + 1);
                _selectedParts.Add(new SpritePart(category.Category, index, old?.Colors ?? []));
            }

            RebuildPartRows(catalog);
        }

        foreach (var part in _selectedParts)
        {
            EnsurePartColors(part);
        }

        RebuildZones();
        var wasHidden = _colorsPanel.IsHidden;
        _colorsPanel.IsHidden = false;
        if (wasHidden || spriteChanged)
        {
            SelectZone(Math.Clamp(_selectedZone, 0, Math.Max(0, _zones.Count - 1)));
        }

        LayoutCustomization();
    }

    /// <summary>
    /// A part's colour list must have exactly one entry per "_cN" layer of the selected variant.
    /// </summary>
    private void EnsurePartColors(SpritePart part)
    {
        var layerCount = part.Index <= 0 || _customizedSprite == null
            ? 0
            : SpriteColorLayers.CountLayers(SpriteColorLayers.PartTextureName(_customizedSprite, part.Category, part.Index));

        if (part.Colors.Length == layerCount)
        {
            return;
        }

        var colors = new Color[layerCount];
        for (var index = 0; index < layerCount; index++)
        {
            colors[index] = index < part.Colors.Length ? part.Colors[index] : RandomColor();
        }

        part.Colors = colors;
    }

    private void RebuildPartRows(IReadOnlyList<SpriteColorLayers.SpritePartCategory> catalog)
    {
        foreach (var row in _partRows)
        {
            _colorsPanel.RemoveChild(row.Row, dispose: true);
        }

        _partRows.Clear();

        foreach (var category in catalog)
        {
            var row = new Panel(_colorsPanel, name: $"PartRow_{category.Category}")
            {
                ShouldDrawBackground = false,
            };

            var name = new Label(row, name: "Name")
            {
                Font = _defaultFont,
                FontSize = 10,
                AutoSizeToContents = false,
                Text = CapitalizeCategory(category.Category),
                TextAlign = Pos.Left | Pos.CenterV,
            };
            name.SetBounds(0, 0, 64, 22);

            var previous = new Button(row, name: "Previous")
            {
                Font = _defaultFont,
                FontSize = 10,
                Text = "<",
                UserData = (category.Category, -1),
            };
            previous.SetBounds(64, 1, 22, 20);
            previous.Clicked += PartButton_Clicked;

            var counter = new Label(row, name: "Counter")
            {
                Font = _defaultFont,
                FontSize = 10,
                AutoSizeToContents = false,
                TextAlign = Pos.Center,
            };
            counter.SetBounds(88, 0, 60, 22);

            var next = new Button(row, name: "Next")
            {
                Font = _defaultFont,
                FontSize = 10,
                Text = ">",
                UserData = (category.Category, 1),
            };
            next.SetBounds(150, 1, 22, 20);
            next.Clicked += PartButton_Clicked;

            _partRows.Add(new PartRow { Row = row, Counter = counter, Category = category.Category });
        }

        RefreshPartCounters();
    }

    private void RefreshPartCounters()
    {
        if (_customizedSprite == null)
        {
            return;
        }

        var catalog = SpriteColorLayers.GetPartCatalog(_customizedSprite);
        foreach (var row in _partRows)
        {
            var part = _selectedParts.FirstOrDefault(part => string.Equals(part.Category, row.Category, StringComparison.OrdinalIgnoreCase));
            var maxIndex = catalog.FirstOrDefault(category => string.Equals(category.Category, row.Category, StringComparison.OrdinalIgnoreCase))?.MaxIndex ?? 0;
            row.Counter.Text = part == null || part.Index <= 0
                ? Strings.CharacterCreation.PartNone
                : Strings.CharacterCreation.PartCounter.ToString(part.Index, maxIndex);
        }
    }

    private void PartButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (sender.UserData is not (string category, int delta) || _customizedSprite == null)
        {
            return;
        }

        var part = _selectedParts.FirstOrDefault(part => string.Equals(part.Category, category, StringComparison.OrdinalIgnoreCase));
        var maxIndex = SpriteColorLayers.GetPartCatalog(_customizedSprite)
            .FirstOrDefault(entry => string.Equals(entry.Category, category, StringComparison.OrdinalIgnoreCase))?.MaxIndex ?? 0;
        if (part == null || maxIndex <= 0)
        {
            return;
        }

        // 0 is "none", then 1..max, wrapping around
        part.Index = (part.Index + delta + maxIndex + 1) % (maxIndex + 1);
        EnsurePartColors(part);
        RefreshPartCounters();
        RebuildZones();
        SelectZone(Math.Clamp(_selectedZone, 0, Math.Max(0, _zones.Count - 1)));
        LayoutCustomization();
        UpdateDisplay();
    }

    /// <summary>
    /// One clickable colour box per tintable layer: the sprite's own layers first, then each visible part's layers.
    /// </summary>
    private void RebuildZones()
    {
        _zones.Clear();
        for (var layer = 0; layer < _selectedColors.Count; layer++)
        {
            _zones.Add((null, layer));
        }

        foreach (var part in _selectedParts)
        {
            for (var layer = 0; layer < part.Colors.Length; layer++)
            {
                _zones.Add((part, layer));
            }
        }

        while (_zoneBoxes.Count < _zones.Count)
        {
            var zoneIndex = _zoneBoxes.Count;
            var frame = new ImagePanel(_colorsPanel, name: $"Zone{zoneIndex}")
            {
                Texture = Graphics.Renderer?.WhitePixel,
                RenderColor = Color.Black,
                MouseInputEnabled = true,
                UserData = zoneIndex,
            };
            frame.Clicked += (sender, _) =>
            {
                if (sender.UserData is int index)
                {
                    SelectZone(index);
                }
            };

            var fill = new ImagePanel(frame, name: "Fill")
            {
                Texture = Graphics.Renderer?.WhitePixel,
                MouseInputEnabled = false,
            };
            fill.SetBounds(2, 2, 20, 20);
            _zoneBoxes.Add(new ZoneBox { Frame = frame, Fill = fill });
        }

        for (var index = 0; index < _zoneBoxes.Count; index++)
        {
            var visible = index < _zones.Count;
            _zoneBoxes[index].Frame.IsHidden = !visible;
            if (visible)
            {
                _zoneBoxes[index].Fill.RenderColor = GetZoneColor(index);
                _zoneBoxes[index].Frame.SetToolTipText(ZoneName(index));
            }
        }
    }

    private string ZoneName(int zoneIndex)
    {
        var (part, layer) = _zones[zoneIndex];

        return part == null
            ? Strings.CharacterCreation.ZoneBase.ToString(layer + 1)
            : Strings.CharacterCreation.ZonePart.ToString(CapitalizeCategory(part.Category), layer + 1);
    }

    private Color GetZoneColor(int zoneIndex)
    {
        var (part, layer) = _zones[zoneIndex];

        return part == null ? _selectedColors[layer] : part.Colors[layer];
    }

    private void SetZoneColor(int zoneIndex, Color color)
    {
        if (zoneIndex < 0 || zoneIndex >= _zones.Count)
        {
            return;
        }

        var (part, layer) = _zones[zoneIndex];
        if (part == null)
        {
            _selectedColors[layer] = color;
        }
        else
        {
            part.Colors[layer] = color;
        }

        _zoneBoxes[zoneIndex].Fill.RenderColor = color;
        _pickerPreview.RenderColor = color;
    }

    private void SelectZone(int zoneIndex)
    {
        _selectedZone = zoneIndex;
        for (var index = 0; index < _zoneBoxes.Count; index++)
        {
            _zoneBoxes[index].Frame.RenderColor = index == zoneIndex ? new Color(255, 240, 215, 138) : Color.Black;
        }

        if (zoneIndex < 0 || zoneIndex >= _zones.Count)
        {
            _zoneNameLabel.Text = string.Empty;
            return;
        }

        var color = GetZoneColor(zoneIndex);
        _zoneNameLabel.Text = ZoneName(zoneIndex);
        _pickerPreview.RenderColor = color;

        var (hue, saturation, brightness) = ToHsv(color);
        _updatingPicker = true;
        _hueSlider.Value = hue;
        _saturationSlider.Value = saturation;
        _brightnessSlider.Value = brightness;
        _updatingPicker = false;
    }

    private void PickerChanged()
    {
        if (_updatingPicker || _selectedZone < 0 || _selectedZone >= _zones.Count)
        {
            return;
        }

        SetZoneColor(_selectedZone, FromHsv(_hueSlider.Value, _saturationSlider.Value, _brightnessSlider.Value));
        UpdateDisplay();
    }

    private void QuickSwatch_Clicked(Base sender, MouseButtonState arguments)
    {
        if (sender.UserData is not int paletteIndex || _selectedZone < 0 || _selectedZone >= _zones.Count)
        {
            return;
        }

        SetZoneColor(_selectedZone, ColorPalette[paletteIndex]);
        SelectZone(_selectedZone);
        UpdateDisplay();
    }

    private void RandomizeCustomization()
    {
        if (_customizedSprite == null)
        {
            return;
        }

        var catalog = SpriteColorLayers.GetPartCatalog(_customizedSprite);
        foreach (var part in _selectedParts)
        {
            var maxIndex = catalog.FirstOrDefault(category => string.Equals(category.Category, part.Category, StringComparison.OrdinalIgnoreCase))?.MaxIndex ?? 0;
            part.Index = ColorRandom.Next(0, maxIndex + 1);
            part.Colors = [];
            EnsurePartColors(part);
        }

        for (var index = 0; index < _selectedColors.Count; index++)
        {
            _selectedColors[index] = RandomColor();
        }

        RefreshPartCounters();
        RebuildZones();
        SelectZone(Math.Clamp(_selectedZone, 0, Math.Max(0, _zones.Count - 1)));
        LayoutCustomization();
        UpdateDisplay();
    }

    /// <summary>
    /// Explicit layout of the block (title, part rows, zone boxes, HSV picker, quick swatches, random button); the
    /// window grows so everything stays visible.
    /// </summary>
    private void LayoutCustomization()
    {
        var y = 0;
        if (!_colorsPanel.IsHidden)
        {
            _colorsTitle.SetBounds(0, y, BlockWidth, 20);
            y += 20;

            foreach (var row in _partRows)
            {
                row.Row.SetBounds(0, y, BlockWidth, 22);
                y += 22;
            }

            if (_zones.Count > 0)
            {
                y += 2;
                for (var index = 0; index < _zones.Count && index < _zoneBoxes.Count; index++)
                {
                    _zoneBoxes[index].Frame.SetBounds(index * 28, y, 24, 24);
                }

                y += 28;

                _pickerPreview.SetBounds(0, y, 40, 44);
                _zoneNameLabel.SetBounds(46, y, BlockWidth - 46, 14);
                _hueLabel.SetBounds(46, y + 16, 16, 16);
                _hueSlider.SetBounds(64, y + 17, BlockWidth - 64, 14);
                _saturationLabel.SetBounds(46, y + 33, 16, 16);
                _saturationSlider.SetBounds(64, y + 34, BlockWidth - 64, 14);
                _brightnessLabel.SetBounds(46, y + 50, 16, 16);
                _brightnessSlider.SetBounds(64, y + 51, BlockWidth - 64, 14);
                y += 68;

                for (var index = 0; index < _quickSwatches.Count; index++)
                {
                    _quickSwatches[index].SetBounds(index * SwatchPitch, y, 16, 16);
                }

                y += 20;
            }

            var pickerVisible = _zones.Count > 0;
            _pickerPreview.IsHidden = !pickerVisible;
            _zoneNameLabel.IsHidden = !pickerVisible;
            _hueLabel.IsHidden = !pickerVisible;
            _hueSlider.IsHidden = !pickerVisible;
            _saturationLabel.IsHidden = !pickerVisible;
            _saturationSlider.IsHidden = !pickerVisible;
            _brightnessLabel.IsHidden = !pickerVisible;
            _brightnessSlider.IsHidden = !pickerVisible;
            foreach (var swatch in _quickSwatches)
            {
                swatch.IsHidden = !pickerVisible;
            }

            _randomColorsButton.SetBounds(0, y, 100, 22);
            y += 22;
        }

        _colorsPanel.MinimumSize = new Point(BlockWidth, y);
        _ = _colorsPanel.SetSize(BlockWidth, y);

        // Two-step window: only whichever step is currently visible needs to fit, so the window stays as
        // small as possible instead of always being tall enough for both the carousel AND the
        // name/faction/gender/colors block at once.
        var neededHeight = Titlebar.Height + 16 + _buttonsPanel.Height + 8;
        if (_isCustomizeStep)
        {
            neededHeight += _nameInput.Height + 8
                            + (_factionCombobox.IsHidden ? 0 : _factionCombobox.Height + 8)
                            + _genderInputPanel.Height + 8
                            + y + 8;
        }
        else
        {
            neededHeight += CarouselTotalHeight + 8;
        }

        var height = Math.Max(240, neededHeight);
        MinimumSize = new Point(Math.Max(MinimumSize.X, CarouselWidth), height);
        _ = SetSize(Math.Max(Width, CarouselWidth), height);
    }

    private Color[] GetColorsToSend() => _colorsPanel.IsHidden ? [] : _selectedColors.ToArray();

    private SpritePart[] GetPartsToSend() => _colorsPanel.IsHidden
        ? []
        : _selectedParts.Select(part => new SpritePart(part.Category, part.Index, part.Index > 0 ? part.Colors : [])).ToArray();

    private static string CapitalizeCategory(string category) =>
        category.Length == 0 ? category : char.ToUpperInvariant(category[0]) + category[1..];

    private static Color RandomColor() => FromHsv(ColorRandom.NextDouble() * 360, 0.35 + ColorRandom.NextDouble() * 0.65, 0.35 + ColorRandom.NextDouble() * 0.65);

    private static (double Hue, double Saturation, double Value) ToHsv(Color color)
    {
        double r = color.R / 255.0, g = color.G / 255.0, b = color.B / 255.0;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        double hue = 0;
        if (delta > 0)
        {
            if (max == r)
            {
                hue = 60 * (((g - b) / delta) % 6);
            }
            else if (max == g)
            {
                hue = 60 * ((b - r) / delta + 2);
            }
            else
            {
                hue = 60 * ((r - g) / delta + 4);
            }

            if (hue < 0)
            {
                hue += 360;
            }
        }

        return (hue, max <= 0 ? 0 : delta / max, max);
    }

    private static Color FromHsv(double hue, double saturation, double value)
    {
        hue = ((hue % 360) + 360) % 360;
        saturation = Math.Clamp(saturation, 0, 1);
        value = Math.Clamp(value, 0, 1);

        var c = value * saturation;
        var x = c * (1 - Math.Abs(hue / 60 % 2 - 1));
        var m = value - c;
        var (r, g, b) = (int)(hue / 60) switch
        {
            0 => (c, x, 0d),
            1 => (x, c, 0d),
            2 => (0d, c, x),
            3 => (0d, x, c),
            4 => (x, 0d, c),
            _ => (c, 0d, x),
        };

        return new Color(255, (int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
    }

    #endregion

    private ClassDescriptor? GetClass()
    {
        if (_carouselClasses.Length == 0)
        {
            return null;
        }

        return _carouselClasses[_centerClassIndex];
    }

    #region Class carousel

    private void BuildCarousel()
    {
        foreach (var portrait in _carouselPortraits)
        {
            _carouselPortraitLayer.RemoveChild(portrait, dispose: true);
        }

        _carouselPortraits.Clear();
        _carouselFallbackLabels.Clear();
        _carouselCurrent.Clear();
        _carouselTarget.Clear();

        _carouselClasses = ClassDescriptor.Lookup.Values.OfType<ClassDescriptor>()
            .Where(classDescriptor => !classDescriptor.Locked)
            .OrderBy(classDescriptor => classDescriptor.Name, StringComparer.Ordinal)
            .ToArray();

        _maxStatIncrease = Math.Max(
            1,
            _carouselClasses.Length == 0
                ? 1
                : _carouselClasses.Max(cls => cls.StatIncrease.DefaultIfEmpty(0).Max())
        );

        for (var index = 0; index < _carouselClasses.Length; index++)
        {
            var classDescriptor = _carouselClasses[index];

            var portrait = new ImagePanel(_carouselPortraitLayer, name: $"CarouselPortrait{index}")
            {
                MouseInputEnabled = true,
                UserData = index,
            };
            portrait.Clicked += CarouselPortrait_Clicked;
            ApplyPortraitTexture(portrait, classDescriptor);
            _carouselPortraits.Add(portrait);

            var fallbackLabel = new Label(portrait, name: "Fallback")
            {
                Font = _defaultFont,
                FontSize = 22,
                AutoSizeToContents = false,
                TextAlign = Pos.Center,
                Dock = Pos.Fill,
                IsHidden = portrait.Texture != null,
                Text = classDescriptor.Name.Length > 0 ? classDescriptor.Name[..1].ToUpperInvariant() : "?",
            };
            _carouselFallbackLabels.Add(fallbackLabel);

            _carouselCurrent.Add((0, 0, CarouselPortraitBaseSize, CarouselPortraitBaseSize, 0));
            _carouselTarget.Add((0, 0, CarouselPortraitBaseSize, CarouselPortraitBaseSize, 0));
        }

        _centerClassIndex = 0;
        _carouselLeftButton.IsHidden = _carouselClasses.Length <= 1;
        _carouselRightButton.IsHidden = _carouselClasses.Length <= 1;

        LayoutCarousel(animate: false);
        RefreshClassInfo();
    }

    /// <summary>
    /// Tries a Face-type portrait texture first (a dedicated bust/face art asset), then falls back to
    /// cropping the class's own walking spritesheet down to its first south-facing frame. If neither
    /// exists the portrait stays textureless and the caller shows a big letter instead.
    /// </summary>
    private void ApplyPortraitTexture(ImagePanel portrait, ClassDescriptor classDescriptor)
    {
        portrait.Texture = null;

        var sprite = classDescriptor.Sprites.FirstOrDefault(s => s.Gender == 0) ?? classDescriptor.Sprites.FirstOrDefault();
        if (sprite == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(sprite.Face))
        {
            var faceTexture = GameContentManager.Current.GetTexture(TextureType.Face, sprite.Face);
            if (faceTexture != null)
            {
                portrait.ResetUVs();
                portrait.Texture = faceTexture;
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(sprite.Sprite))
        {
            return;
        }

        var entityTexture = GameContentManager.Current.GetTexture(TextureType.Entity, sprite.Sprite);
        if (entityTexture == null)
        {
            return;
        }

        var frameWidth = entityTexture.Width / Options.Instance.Sprites.NormalFrames;
        var frameHeight = entityTexture.Height / Options.Instance.Sprites.Directions;
        if (frameWidth <= 0 || frameHeight <= 0)
        {
            return;
        }

        portrait.Texture = entityTexture;
        portrait.SetTextureRect(0, 0, frameWidth, frameHeight);
    }

    /// <summary>
    /// Signed circular distance from <see cref="_centerClassIndex"/> to <paramref name="index"/>, in the
    /// range roughly [-count/2, count/2] - lets the carousel treat "just past the last class" as being
    /// right next to "the first class" instead of far away.
    /// </summary>
    private int CircularDistance(int index)
    {
        var count = _carouselClasses.Length;
        if (count == 0)
        {
            return 0;
        }

        var raw = index - _centerClassIndex;
        var wrapped = ((raw % count) + count) % count;
        if (wrapped > count / 2)
        {
            wrapped -= count;
        }

        return wrapped;
    }

    private void LayoutCarousel(bool animate)
    {
        var centerX = _carouselPortraitLayer.Width / 2;
        var centerY = _carouselPortraitLayer.Height / 2;

        for (var index = 0; index < _carouselClasses.Length; index++)
        {
            var distance = CircularDistance(index);
            var absDistance = Math.Abs(distance);

            double scale;
            double alpha;
            switch (absDistance)
            {
                case 0:
                    scale = 1.20;
                    alpha = 1.0;
                    break;
                case 1:
                    scale = 0.85;
                    alpha = 0.70;
                    break;
                case 2:
                    scale = 0.70;
                    alpha = 0.40;
                    break;
                default:
                    scale = 0.55;
                    alpha = 0.0;
                    break;
            }

            var size = CarouselPortraitBaseSize * scale;
            var x = centerX + distance * CarouselPitch - size / 2;
            var y = centerY - size / 2;

            _carouselTarget[index] = (x, y, size, size, alpha);
            if (!animate)
            {
                _carouselCurrent[index] = _carouselTarget[index];
                ApplyPortraitTransform(index);
            }
        }

        // Draw order: farthest portraits first, so nearer ones (and finally the centered one) overlap on top.
        foreach (var index in Enumerable.Range(0, _carouselClasses.Length).OrderByDescending(i => Math.Abs(CircularDistance(i))))
        {
            _carouselPortraits[index].BringToFront();
        }

        _lastCarouselAnimMs = Timing.Global.Milliseconds;
    }

    private void ApplyPortraitTransform(int index)
    {
        var (x, y, w, h, alpha) = _carouselCurrent[index];
        var portrait = _carouselPortraits[index];
        portrait.SetBounds((int)Math.Round(x), (int)Math.Round(y), Math.Max(1, (int)Math.Round(w)), Math.Max(1, (int)Math.Round(h)));
        var byteAlpha = (byte)Math.Clamp((int)Math.Round(alpha * 255), 0, 255);
        portrait.RenderColor = new Color(byteAlpha, 255, 255, 255);
        portrait.MouseInputEnabled = alpha > 0.05;
    }

    private void StepCarouselAnimation()
    {
        if (_carouselClasses.Length == 0)
        {
            return;
        }

        var nowMs = Timing.Global.Milliseconds;
        var deltaMs = _lastCarouselAnimMs == 0 ? 16 : Math.Clamp(nowMs - _lastCarouselAnimMs, 0, 100);
        _lastCarouselAnimMs = nowMs;

        // Exponential smoothing towards the target - converges in ~150ms regardless of frame rate.
        var lerpFactor = 1 - Math.Pow(0.001, deltaMs / 150.0);

        for (var index = 0; index < _carouselClasses.Length; index++)
        {
            var (cx, cy, cw, ch, ca) = _carouselCurrent[index];
            var (tx, ty, tw, th, ta) = _carouselTarget[index];

            cx += (tx - cx) * lerpFactor;
            cy += (ty - cy) * lerpFactor;
            cw += (tw - cw) * lerpFactor;
            ch += (th - ch) * lerpFactor;
            ca += (ta - ca) * lerpFactor;

            _carouselCurrent[index] = (cx, cy, cw, ch, ca);
            ApplyPortraitTransform(index);
        }
    }

    private void RefreshClassInfo()
    {
        var classDescriptor = GetClass();
        if (classDescriptor == null)
        {
            _carouselClassName.Text = string.Empty;
            _carouselClassDescription.Text = string.Empty;
            foreach (var (_, bar) in _statRows)
            {
                bar.Value = 0;
            }

            return;
        }

        _carouselClassName.Text = classDescriptor.Name;
        _carouselClassDescription.Text = classDescriptor.Description;

        var stats = classDescriptor.StatIncreaseLookup;
        _statRows[0].Bar.Value = stats[Stat.Attack] / (float)_maxStatIncrease;
        _statRows[1].Bar.Value = stats[Stat.Defense] / (float)_maxStatIncrease;
        _statRows[2].Bar.Value = stats[Stat.AbilityPower] / (float)_maxStatIncrease;
        _statRows[3].Bar.Value = stats[Stat.Speed] / (float)_maxStatIncrease;
    }

    /// <summary>
    /// Switches between the class-selection step (carousel) and the customization step (name/faction/
    /// gender/colors/sprite preview), resizing the window to fit only whatever is currently shown.
    /// </summary>
    private void SetStep(bool customize)
    {
        _isCustomizeStep = customize;

        _carouselPanel.IsHidden = customize;
        _previewPanel.IsHidden = !customize;
        _propertiesPanel.IsHidden = !customize;

        _createButton.Text = customize ? Strings.CharacterCreation.Create : Strings.CharacterCreation.Next;

        LayoutCustomization();
    }

    private void SelectCarouselIndex(int index)
    {
        if (_carouselClasses.Length == 0)
        {
            return;
        }

        var count = _carouselClasses.Length;
        _centerClassIndex = ((index % count) + count) % count;
        LayoutCarousel(animate: true);
        RefreshClassInfo();
        LoadClass();
        UpdateDisplay();
        LayoutCustomization();
    }

    private void CarouselPortrait_Clicked(Base sender, MouseButtonState arguments)
    {
        if (sender.UserData is int index)
        {
            SelectCarouselIndex(index);
        }
    }

    private void CarouselLeftButton_Clicked(Base sender, MouseButtonState arguments) => SelectCarouselIndex(_centerClassIndex - 1);

    private void CarouselRightButton_Clicked(Base sender, MouseButtonState arguments) => SelectCarouselIndex(_centerClassIndex + 1);

    #endregion

    private void LoadClass()
    {
        var cls = GetClass();
        _maleSprites.Clear();
        _femaleSprites.Clear();
        _displaySpriteIndex = -1;
        if (cls != null)
        {
            for (var i = 0; i < cls.Sprites.Count; i++)
            {
                if (cls.Sprites[i].Gender == 0)
                {
                    _maleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                }
                else
                {
                    _femaleSprites.Add(new KeyValuePair<int, ClassSprite>(i, cls.Sprites[i]));
                }
            }
        }

        ResetSprite();
    }

    private void ResetSprite()
    {
        _nextSpriteButton.IsHidden = true;
        _prevSpriteButton.IsHidden = true;

        if (_genderMaleCheckbox.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                _displaySpriteIndex = 0;
                if (_maleSprites.Count > 1)
                {
                    _nextSpriteButton.IsHidden = false;
                    _prevSpriteButton.IsHidden = false;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                _displaySpriteIndex = 0;
                if (_femaleSprites.Count > 1)
                {
                    _nextSpriteButton.IsHidden = false;
                    _prevSpriteButton.IsHidden = false;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
    }

    private void _prevSpriteButton_Clicked(Base sender, MouseButtonState arguments)
    {
        _displaySpriteIndex--;
        if (_genderMaleCheckbox.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                if (_displaySpriteIndex == -1)
                {
                    _displaySpriteIndex = _maleSprites.Count - 1;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                if (_displaySpriteIndex == -1)
                {
                    _displaySpriteIndex = _femaleSprites.Count - 1;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }

        UpdateDisplay();
    }

    private void _nextSpriteButton_Clicked(Base sender, MouseButtonState arguments)
    {
        _displaySpriteIndex++;
        if (_genderMaleCheckbox.IsChecked)
        {
            if (_maleSprites.Count > 0)
            {
                if (_displaySpriteIndex >= _maleSprites.Count)
                {
                    _displaySpriteIndex = 0;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }
        else
        {
            if (_femaleSprites.Count > 0)
            {
                if (_displaySpriteIndex >= _femaleSprites.Count)
                {
                    _displaySpriteIndex = 0;
                }
            }
            else
            {
                _displaySpriteIndex = -1;
            }
        }

        UpdateDisplay();
    }

    void TryCreateCharacter()
    {
        var cls = GetClass();
        if (Globals.WaitingOnServer || _displaySpriteIndex == -1 || cls == default)
        {
            return;
        }

        if (!FieldChecking.IsValidUsername(_nameInput.Text, Strings.Regex.Username))
        {
            Interface.ShowAlert(Strings.CharacterCreation.InvalidName, alertType: AlertType.Error);
            return;
        }

        var charName = _nameInput.Text;
        var spriteKey = _genderMaleCheckbox.IsChecked ? _maleSprites[_displaySpriteIndex].Key : _femaleSprites[_displaySpriteIndex].Key;

        PacketSender.SendCreateCharacter(charName, cls.Id, spriteKey, GetColorsToSend(), GetPartsToSend(), (_factionCombobox.SelectedItem?.UserData as FactionDescriptor)?.Id ?? Guid.Empty);
        Globals.WaitingOnServer = true;
        _createButton.Disable();
        ChatboxMsg.ClearMessages();
    }

    //Input Handlers
    void MaleCheckboxGenderChecked(ICheckbox sender, EventArgs arguments)
    {
        _genderMaleCheckbox.IsChecked = true;
        _genderFemaleCheckbox.IsChecked = false;
        ResetSprite();
        UpdateDisplay();
        LayoutCustomization();
    }

    void FemaleCheckboxGenderChecked(ICheckbox sender, EventArgs arguments)
    {
        _genderFemaleCheckbox.IsChecked = true;
        _genderMaleCheckbox.IsChecked = false;
        ResetSprite();
        UpdateDisplay();
        LayoutCustomization();
    }

    void CreateButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.WaitingOnServer)
        {
            return;
        }

        if (!_isCustomizeStep)
        {
            SetStep(true);

            return;
        }

        TryCreateCharacter();
    }

    private void BackButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (_isCustomizeStep)
        {
            SetStep(false);

            return;
        }

        Hide();
        if (Options.Instance.Player.MaxCharacters <= 1)
        {
            //Logout
            _mainMenu.Show();
        }
        else
        {
            //Character Selection Screen
            _selectCharacterWindow.Show();
        }
    }
}
