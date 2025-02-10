using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Static text label.
/// </summary>
public partial class Label : Base, ILabel
{
    protected readonly Text _textElement;

    private string? _fontInfo;

    private Pos _textAlign;

    private bool _autoSizeToContents;

    private string? mBackgroundTemplateFilename;

    private GameTexture? mBackgroundTemplateTex;

    protected Color? mClickedTextColor;

    protected Color? mDisabledTextColor;

    protected Color? mHoverTextColor;

    protected Color? mNormalTextColor;

    private Padding _textPadding;

    private string? _displayedText;
    private string? _text;
    private string? _formatString;
    private Range _replacementRange;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Label" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    /// <param name="disableText"></param>
    public Label(Base parent, string? name = default, bool disableText = false) : base(parent, name)
    {
        _textElement = new Text(this)
        {
            IsHidden = disableText,
        };

        MouseInputEnabled = false;
        SetSize(100, 10);
        TextAlign = Pos.Left | Pos.Top;

        _autoSizeToContents = true;
    }

    public string? FormatString
    {
        get => _formatString;
        set
        {
            if (string.Equals(value, _formatString, StringComparison.Ordinal))
            {
                return;
            }

            _replacementRange = GetRangeForFormatArgument(value, 0);
            _formatString = value;
        }
    }

    private static Range GetRangeForFormatArgument(string? format, int argumentIndex)
    {
        const char charStartArgument = '{';
        const char charEndArgument = '}';

        if (argumentIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(argumentIndex), argumentIndex, "Must be non-negative");
        }

        if (string.IsNullOrEmpty(format))
        {
            return default;
        }

        var minimumDigits = argumentIndex < 10 ? 1 : (int)Math.Floor(Math.Log10(argumentIndex));
        if (format.Length < 2 + minimumDigits)
        {
            return default;
        }

        char searchChar = charStartArgument;
        int? formatSplit = null;
        Range argumentIndexCharacterRange = default;
        for (var characterIndex = 0; characterIndex < format.Length; ++characterIndex)
        {
            var currentChar = format[characterIndex];
            switch (currentChar)
            {
                case charStartArgument:
                    searchChar = charEndArgument;
                    formatSplit = null;
                    argumentIndexCharacterRange = new Range(characterIndex + 1, characterIndex + 1);
                    continue;

                case charEndArgument:
                {
                    if (searchChar == charStartArgument)
                    {
                        continue;
                    }

                    searchChar = charStartArgument;
                    formatSplit = null;
                    argumentIndexCharacterRange = new Range(
                        argumentIndexCharacterRange.Start,
                        (formatSplit ?? characterIndex)
                    );

                    if (IsArgument(format, argumentIndexCharacterRange, argumentIndex))
                    {
                        return new Range(
                            argumentIndexCharacterRange.Start.Value - 1,
                            ^(format.Length - (argumentIndexCharacterRange.End.Value + 1))
                        );
                    }

                    argumentIndexCharacterRange = default;

                    continue;
                }

                case ':':
                    formatSplit = characterIndex;

                    Range offsetRange = new(argumentIndexCharacterRange.Start, characterIndex);
                    if (IsArgument(format, offsetRange, argumentIndex))
                    {
                        continue;
                    }

                    searchChar = charStartArgument;
                    formatSplit = null;
                    continue;

                default:
                    if (formatSplit == null && !char.IsAsciiDigit(currentChar))
                    {
                        searchChar = charStartArgument;
                    }

                    break;
            }
        }

        return argumentIndexCharacterRange;
    }

    private static bool IsArgument(string format, Range range, int argumentIndex) =>
        int.TryParse(format[range], out var index) && index == argumentIndex;

    public WrappingBehavior WrappingBehavior
    {
        get => _wrappingBehavior;
        set
        {
            if (value == _wrappingBehavior)
            {
                return;
            }

            _wrappingBehavior = value;
            _textElement.WrappingBehavior = value;
        }
    }

    public GameTexture? ToolTipBackground
    {
        get => _tooltipBackground;
        set
        {
            if (value == _tooltipBackground)
            {
                return;
            }

            _tooltipBackground = value;
        }
    }

    public bool IsTextDisabled
    {
        get => _textElement.IsHidden;
        set => _textElement.IsHidden = value;
    }

    /// <summary>
    ///     Text alignment.
    /// </summary>
    public Pos TextAlign
    {
        get => _textAlign;
        set => SetAndDoIfChanged(ref _textAlign, value, Invalidate);
    }

    public override void Invalidate()
    {
        base.Invalidate();
    }

    /// <summary>
    ///     Text.
    /// </summary>
    public virtual string? Text
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => SetText(value, doEvents: true);
    }

    /// <summary>
    ///     Font.
    /// </summary>
    public virtual GameFont? Font
    {
        get => _textElement.Font;
        set
        {
            if (value == _textElement.Font)
            {
                return;
            }

            var oldValue = _textElement.Font;
            if (value is not null)
            {
                _textElement.Font = value;
                _fontInfo = $"{value.GetName()},{value.GetSize()}";
            }
            else
            {
                _fontInfo = null;
            }

            OnFontChanged(this, oldValue, value);

            if (_autoSizeToContents)
            {
                SizeToContents();
            }

            Invalidate();

            FontChanged?.Invoke(this, new ValueChangedEventArgs<GameFont?>
            {
                OldValue = oldValue,
                Value = value,
            });
        }
    }

    public event GwenEventHandler<ValueChangedEventArgs<GameFont?>>? FontChanged;

    protected virtual void OnFontChanged(Base sender, GameFont? oldFont, GameFont? newFont)
    {
    }

    /// <summary>
    /// Font Name
    /// </summary>
    public string? FontName
    {
        get => _textElement.Font?.GetName();
        set => Font = GameContentManager.Current?.GetFont(value, FontSize);
    }

    /// <summary>
    /// Font Size
    /// </summary>
    public int FontSize
    {
        get => _textElement?.Font?.GetSize() ?? 12;
        set => Font = GameContentManager.Current?.GetFont(FontName, value);
    }

    /// <summary>
    ///     Text color.
    /// </summary>
    public Color? TextColor
    {
        get => _textElement.Color ?? Color.White;
        set
        {
            if (value == _textElement.Color)
            {
                return;
            }

            _textElement.Color = value;
        }
    }

    /// <summary>
    ///     Override text color (used by tooltips).
    /// </summary>
    public Color? TextColorOverride
    {
        get => _textElement.ColorOverride;
        set => _textElement.ColorOverride = value;
    }

    public Padding TextPadding { get; set; }

    private string? _textOverride;
    private WrappingBehavior _wrappingBehavior;
    private GameTexture? _tooltipBackground;

    /// <summary>
    ///     Text override - used to display different string.
    /// </summary>
    public string? TextOverride
    {
        get => _textOverride;
        set
        {
            if (string.Equals(value, _textOverride, StringComparison.Ordinal))
            {
                return;
            }

            _textOverride = value;
            _textElement.DisplayedText = _textOverride ?? _text;
        }
    }

    /// <summary>
    ///     Width of the text (in pixels).
    /// </summary>
    public int TextWidth => _textElement.Width;

    /// <summary>
    ///     Height of the text (in pixels).
    /// </summary>
    public int TextHeight => _textElement.Height;

    public int TextX => _textElement.X;

    public int TextY => _textElement.Y;

    /// <summary>
    ///     Text length (in characters).
    /// </summary>
    public int TextLength => _text?.Length ?? 0;

    public int TextRight => _textElement.Right;

    /// <summary>
    ///     Determines if the control should autosize to its text.
    /// </summary>
    public bool AutoSizeToContents
    {
        get => _autoSizeToContents;
        set
        {
            _autoSizeToContents = value;
            Invalidate();
            InvalidateParent();
        }
    }

    public Color? TextPaddingDebugColor { get; set; }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        if (typeof(Label) == GetType())
        {
            serializedProperties.Add("BackgroundTemplate", mBackgroundTemplateFilename);
        }

        serializedProperties.Add(nameof(TextColor), mNormalTextColor?.ToString());
        serializedProperties.Add("HoveredTextColor", mHoverTextColor?.ToString());
        serializedProperties.Add("ClickedTextColor", mClickedTextColor?.ToString());
        serializedProperties.Add("DisabledTextColor", mDisabledTextColor?.ToString());
        serializedProperties.Add(nameof(TextAlign), TextAlign.ToString());
        serializedProperties.Add(nameof(AutoSizeToContents), _autoSizeToContents);
        serializedProperties.Add(nameof(Font), _fontInfo);
        serializedProperties.Add("TextScale", _textElement.GetScale());

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (typeof(Label) == GetType() && obj["BackgroundTemplate"] != null)
        {
            SetBackgroundTemplate(
                GameContentManager.Current.GetTexture(
                    TextureType.Gui, (string)obj["BackgroundTemplate"]
                ), (string)obj["BackgroundTemplate"]
            );
        }

        if (obj["TextColor"] != null)
        {
            TextColor = Color.FromString((string)obj["TextColor"]);
        }

        mNormalTextColor = TextColor;
        if (obj["HoveredTextColor"] != null)
        {
            mHoverTextColor = Color.FromString((string)obj["HoveredTextColor"]);
        }

        if (obj["ClickedTextColor"] != null)
        {
            mClickedTextColor = Color.FromString((string)obj["ClickedTextColor"]);
        }

        if (obj["DisabledTextColor"] != null)
        {
            mDisabledTextColor = Color.FromString((string)obj["DisabledTextColor"]);
        }

        if (obj["TextAlign"] != null)
        {
            TextAlign = (Pos)Enum.Parse(typeof(Pos), (string)obj["TextAlign"]);
        }

        if (obj["AutoSizeToContents"] != null)
        {
            _autoSizeToContents = (bool)obj["AutoSizeToContents"];
        }

        var tokenFont = obj["Font"];
        if (tokenFont != null && tokenFont.Type != JTokenType.Null)
        {
            var stringFont = (string)tokenFont;
            if (!string.IsNullOrWhiteSpace(stringFont))
            {
                var fontArr = stringFont.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                );
                if (fontArr.Length > 1)
                {
                    _fontInfo = stringFont;
                    try
                    {
                        Font = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        if (obj["TextScale"] != null)
        {
            _textElement.SetScale((float)obj["TextScale"]);
        }
    }

    public GameTexture GetTemplate()
    {
        return mBackgroundTemplateTex;
    }

    public string? BackgroundTemplateName
    {
        get => mBackgroundTemplateFilename;
        set
        {
            var newFileName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            if (newFileName == mBackgroundTemplateFilename?.Trim())
            {
                return;
            }

            mBackgroundTemplateFilename = newFileName;
            mBackgroundTemplateTex = newFileName == null
                ? null
                : GameContentManager.Current?.GetTexture(TextureType.Gui, newFileName);
        }
    }

    public void SetBackgroundTemplate(GameTexture texture, string fileName)
    {
        if (texture == null && !string.IsNullOrWhiteSpace(fileName))
        {
            texture = GameContentManager.Current?.GetTexture(TextureType.Gui, fileName);
        }

        mBackgroundTemplateFilename = fileName;
        mBackgroundTemplateTex = texture;
    }

    public virtual void MakeColorNormal()
    {
        TextColor = Skin.Colors.Label.Normal;
    }

    public virtual void MakeColorBright()
    {
        TextColor = Skin.Colors.Label.Hover;
    }

    public virtual void MakeColorDark()
    {
        TextColor = Skin.Colors.Label.Disabled;
    }

    public virtual void MakeColorHighlight()
    {
        TextColor = Skin.Colors.Label.Active;
    }

    public override event GwenEventHandler<MouseButtonState> Clicked
    {
        add
        {
            base.Clicked += value;
            MouseInputEnabled = ClickEventAssigned;
        }
        remove
        {
            base.Clicked -= value;
            MouseInputEnabled = ClickEventAssigned;
        }
    }

    public override event GwenEventHandler<MouseButtonState> DoubleClicked
    {
        add
        {
            base.DoubleClicked += value;
            MouseInputEnabled = ClickEventAssigned;
        }
        remove
        {
            base.DoubleClicked -= value;
            MouseInputEnabled = ClickEventAssigned;
        }
    }

    /// <summary>
    ///     Returns index of the character closest to specified point (in canvas coordinates).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected virtual Point GetClosestCharacter(int x, int y) => GetClosestCharacter(new Point(x, y));

    protected Point GetClosestCharacter(Point point)
    {
        point = _textElement.CanvasPosToLocal(point);
        var closestCharacterIndex = _textElement.GetClosestCharacter(point);
        Point closestCharacterPoint = new(closestCharacterIndex, 0);

        var replacementRange = _replacementRange;
        if (_textOverride != null || _displayedText is not { } displayedText || replacementRange.Equals(default))
        {
            return closestCharacterPoint;
        }

        var startIndex = replacementRange.Start.Value;
        var endIndex = replacementRange.End.Value;
        if (replacementRange.End.IsFromEnd)
        {
            endIndex = displayedText.Length - endIndex;
        }

        closestCharacterPoint.X = Math.Clamp(
            closestCharacterPoint.X,
            startIndex,
            endIndex
        );

        return closestCharacterPoint;
    }

    /// <summary>
    ///     Sets the position of the internal text control.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    protected void SetTextPosition(int x, int y)
    {
        _textElement.SetPosition(x, y);
    }

    /// <summary>
    ///     Handler for text changed event.
    /// </summary>
    protected virtual void OnTextChanged()
    {
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);

        if (_autoSizeToContents)
        {
            SizeToContents();
        }

        AlignTextElement(_textElement);
    }

    protected void AlignTextElement(Text textElement)
    {
        var align = TextAlign;
        var textOuterWidth = textElement.OuterWidth;
        var textOuterHeight = textElement.OuterHeight;
        var textPadding = Padding;

        var availableWidth = Width - (textPadding.Left + textPadding.Right);
        var availableHeight = Height - (textPadding.Top + textPadding.Bottom);
        Rectangle contentBounds = new Rectangle(textPadding.Left, textPadding.Top, availableWidth, availableHeight);

        var x = textPadding.Left;
        var y = textPadding.Top;

        if (align.HasFlag(Pos.CenterH))
        {
            x = textPadding.Left + (int)((contentBounds.Width - textOuterWidth) / 2f);
        }
        else if (align.HasFlag(Pos.Right))
        {
            x = contentBounds.Right - textOuterWidth;
        }

        if (align.HasFlag(Pos.CenterV))
        {
            y = textPadding.Top + (int)((contentBounds.Height - textOuterHeight) / 2f);
        }
        else if (align.HasFlag(Pos.Bottom))
        {
            y = contentBounds.Bottom - textOuterHeight;
        }

        textElement.SetPosition(x, y);
    }

    /// <summary>
    ///     Sets the label text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    /// <param name="doEvents">Determines whether to invoke "text changed" event.</param>
    public virtual void SetText(string? text, bool doEvents = true)
    {
        if (string.Equals(_text, text, StringComparison.Ordinal))
        {
            return;
        }

        _text = text;
        _displayedText = string.IsNullOrWhiteSpace(_formatString) ? _text : string.Format(_formatString, _text);
        _textElement.DisplayedText = _displayedText;

        var sizeChanged = _autoSizeToContents && SizeToContents();
        if (sizeChanged)
        {
            Invalidate();
            InvalidateParent();
        }

        if (doEvents)
        {
            OnTextChanged();
        }
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);

        if (oldChildBounds.Size != newChildBounds.Size)
        {
            if (_autoSizeToContents)
            {
                Invalidate();
            }
            else
            {
                AlignTextElement(_textElement);
            }
        }
    }

    public virtual void SetTextScale(float scale)
    {
        _textElement.SetScale(scale);
        if (_autoSizeToContents)
        {
            SizeToContents();
        }

        Invalidate();
        InvalidateParent();
    }

    protected virtual Point GetContentSize() => _textElement.Size;

    protected virtual Padding GetContentPadding() => Padding + _textPadding;

    public virtual bool SizeToContents()
    {
        var newSize = MeasureShrinkToContents();

        var oldSize = Size;
        if (!SetSize(newSize))
        {
            return false;
        }

        ProcessAlignments();
        InvalidateParent();

        SizedToContents?.Invoke(
            this,
            new ValueChangedEventArgs<Point>
            {
                Value = newSize, OldValue = oldSize,
            }
        );

        return true;
    }

    public GwenEventHandler<ValueChangedEventArgs<Point>>? SizedToContents;

    public virtual Point MeasureShrinkToContents()
    {
        var contentPadding = GetContentPadding();
        _textElement.SetPosition(contentPadding.Left, contentPadding.Top);

        var contentSize = GetContentSize();

        var newWidth = contentSize.X + contentPadding.Left + contentPadding.Right;
        newWidth = Math.Max(newWidth, MinimumSize.X);

        var newHeight = contentSize.Y + contentPadding.Top + contentPadding.Bottom;
        newHeight = Math.Max(newHeight, MinimumSize.Y);

        return new Point(newWidth, newHeight);
    }

    /// <summary>
    ///     Gets the coordinates of specified character.
    /// </summary>
    /// <param name="index">Character index.</param>
    /// <returns>Character coordinates (local).</returns>
    public virtual Point GetCharacterPosition(int index)
    {
        var p = _textElement.GetCharacterPosition(index);

        return new Point(p.X + _textElement.X, p.Y + _textElement.Y);
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawLabel(this);
    }

    /// <summary>
    ///     Updates control colors.
    /// </summary>
    public override void UpdateColors()
    {
        var textColor = GetTextColor(ComponentState.Normal) ?? Skin.Colors.Label.Normal;
        if (IsDisabledByTree)
        {
            textColor = GetTextColor(ComponentState.Disabled) ?? Skin.Colors.Label.Disabled;
        }
        else if (IsActive)
        {
            textColor = GetTextColor(ComponentState.Active) ?? Skin.Colors.Label.Active;
        }
        else if (IsHovered)
        {
            textColor = GetTextColor(ComponentState.Hovered) ?? Skin.Colors.Label.Hover;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (textColor == null)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Text color for the current control state of '{ComponentName}' is somehow null IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered}",
                CanonicalName,
                IsDisabled,
                IsActive,
                IsHovered
            );

            textColor = new Color(r: 255, g: 0, b: 255);
        }

        TextColor = textColor;
    }

    public virtual void SetTextColor(Color clr, ComponentState state)
    {
        switch (state)
        {
            case ComponentState.Normal:
                mNormalTextColor = clr;

                break;
            case ComponentState.Hovered:
                mHoverTextColor = clr;

                break;
            case ComponentState.Active:
                mClickedTextColor = clr;

                break;
            case ComponentState.Disabled:
                mDisabledTextColor = clr;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        UpdateColors();
    }

    public virtual Color? GetTextColor(ComponentState state)
    {
        switch (state)
        {
            case ComponentState.Normal:
                return mNormalTextColor;
            case ComponentState.Hovered:
                return mHoverTextColor;
            case ComponentState.Active:
                return mClickedTextColor;
            case ComponentState.Disabled:
                return mDisabledTextColor;
            default:
                return null;
        }
    }

    public override string ToString()
    {
        return $"Label (Text='{Text}')";
    }
}