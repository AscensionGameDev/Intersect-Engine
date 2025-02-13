using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.Admin;

public class TexturePicker : Panel, ITextContainer
{
    private readonly GameFont? _defaultFont;

    private readonly Panel _previewPanel;
    private readonly ImagePanel _preview;
    private readonly Panel _inputPanel;
    private readonly LabeledComboBox _textureSelector;
    private readonly Button _submitButton;

    private TextureType _textureType;
    private bool _selectorDirty;
    private bool _allowNone;
    private MenuItem? _noneItem;

    public event GwenEventHandler<TexturePicker, ItemSelectedEventArgs>? Selected;
    public event GwenEventHandler<TexturePicker, ValueChangedEventArgs<string?>>? Submitted;

    public TexturePicker(Base parent, string? name = nameof(TexturePicker)) : base(parent: parent, name: name)
    {
        _allowNone = true;
        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack", size: 12);

        DockChildSpacing = new Padding(8);
        ShouldDrawBackground = false;

        _previewPanel = new Panel(this, name: nameof(_previewPanel))
        {
            Dock = Pos.Left,
            MinimumSize = new Point(68, 68),
            Padding = new Padding(4),
        };

        _preview = new ImagePanel(_previewPanel, name: nameof(_preview))
        {
            Alignment = [Alignments.Center],
            MaintainAspectRatio = true,
        };

        _inputPanel = new Panel(this, name: nameof(_inputPanel))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
            ShouldDrawBackground = false,
        };

        _textureSelector = new LabeledComboBox(_inputPanel, name: nameof(_textureSelector))
        {
            AutoSizeToContents = false,
            Dock = Pos.Fill,
            Font = _defaultFont,
            TextPadding = new Padding(8, 4, 0, 4),
        };
        _textureSelector.ItemSelected += TextureSelectorOnItemSelected;

        _submitButton = new Button(_inputPanel, name: nameof(_submitButton))
        {
            Dock = Pos.Bottom,
            Font = _defaultFont,
            Padding = new Padding(8, 4),
        };
        _submitButton.Clicked += SubmitButtonOnClicked;

        SizeToChildren(recursive: true);
    }

    private void TextureSelectorOnItemSelected(Base sender, ItemSelectedEventArgs arguments)
    {
        if (arguments.SelectedUserData is not string textureName)
        {
            _preview.Texture = null;
            return;
        }

        var texture = GameContentManager.Current.GetTexture(_textureType, textureName);
        _preview.Texture = texture;

        if (texture == null)
        {
            return;
        }

        var frameWidth = texture.Width;
        var frameHeight = texture.Height;

        if (_textureType == TextureType.Entity)
        {
            frameWidth = texture.Width / Options.Instance.Sprites.NormalFrames;
            frameHeight = texture.Height / Options.Instance.Sprites.Directions;
        }

        _preview.SetTextureRect(
            (_preview.Width - frameWidth) / 2,
            (_preview.Height - frameHeight) / 2,
            frameWidth,
            frameHeight
        );
        _ = _preview.SetSize(Math.Min(frameWidth, 48), Math.Min(frameHeight, 48));

        Selected?.Invoke(this, arguments);
    }

    private void SubmitButtonOnClicked(Base sender, MouseButtonState arguments)
    {
        Submitted?.Invoke(
            this,
            new ValueChangedEventArgs<string?>
            {
                Value = _textureSelector.SelectedItem?.UserData as string,
            }
        );
    }

    public bool AllowNone
    {
        get => _allowNone;
        set => SetAndDoIfChanged(ref _allowNone, value, UpdateNone);
    }

    public GameFont? ButtonFont
    {
        get => _submitButton.Font;
        set => _submitButton.Font = value;
    }

    public GameFont? LabelFont
    {
        get => _textureSelector.LabelFont;
        set => _textureSelector.LabelFont = value;
    }

    public GameFont? SelectorFont
    {
        get => _textureSelector.ItemFont;
        set => _textureSelector.ItemFont = value;
    }

    public GameFont? Font
    {
        get => _textureSelector.Font;
        set
        {
            _submitButton.Font = value;
            _textureSelector.Font = value;
        }
    }

    public string? ButtonText
    {
        get => _submitButton.Text;
        set => _submitButton.Text = value;
    }

    public string? LabelText
    {
        get => _textureSelector.Label;
        set => _textureSelector.Label = value;
    }

    public TextureType TextureType
    {
        get => _textureType;
        set => SetAndDoIfChanged(ref _textureType, value, InvalidateSelector);
    }

    string? ITextContainer.Text
    {
        get => LabelText;
        set => LabelText = value;
    }

    public Color? TextPaddingDebugColor { get; set; }

    private void UpdateNone()
    {
        if (_allowNone)
        {
            _noneItem ??= AddNone();
        }
        else if (_noneItem is not null)
        {
            RemoveChild(_noneItem, dispose: true);
            _noneItem = null;
        }
    }

    public void InvalidateSelector()
    {
        if (!_selectorDirty)
        {
            _selectorDirty = true;
        }

        Invalidate();
    }

    private MenuItem AddNone()
    {
        var noneItem = _textureSelector.AddItem(label: Strings.General.None);
        noneItem.SendToBack();
        return noneItem;
    }

    protected override void Layout(Framework.Gwen.Skin.Base skin)
    {
        if (_selectorDirty)
        {
            _selectorDirty = false;

            var textureNames = GameContentManager.Current.GetTextureNames(_textureType);
            Array.Sort(textureNames, new AlphanumComparatorFast());

            _noneItem = null;
            _textureSelector.ClearItems();

            UpdateNone();

            foreach (var textureName in textureNames)
            {
                _ = _textureSelector.AddItem(label: textureName, userData: textureName);
            }
        }

        base.Layout(skin);
    }
}