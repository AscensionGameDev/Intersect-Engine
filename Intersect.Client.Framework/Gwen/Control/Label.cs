using System.Runtime.CompilerServices;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Static text label.
/// </summary>
public partial class Label : Base, ILabel
{

    public enum ControlState
    {

        Normal = 0,

        Hovered,

        Clicked,

        Disabled,

    }

    protected readonly Text _textElement;

    private string fontInfo;

    private Pos mAlign;

    private bool mAutoSizeToContents;

    private string? mBackgroundTemplateFilename;

    private GameTexture? mBackgroundTemplateTex;

    protected Color? mClickedTextColor;

    protected Color? mDisabledTextColor;

    protected Color? mHoverTextColor;

    protected Color? mNormalTextColor;

    private Padding mTextPadding;

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

        mAutoSizeToContents = true;
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
        get => mAlign;
        set => SetAndDoIfChanged(ref mAlign, value, Invalidate);
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

            _textElement.Font = value;
            fontInfo = $"{value?.GetName()},{value?.GetSize()}";

            if (mAutoSizeToContents)
            {
                SizeToContents();
            }

            Invalidate();
        }
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
        set => _textElement.Color = value;
    }

    /// <summary>
    ///     Override text color (used by tooltips).
    /// </summary>
    public Color? TextColorOverride
    {
        get => _textElement.ColorOverride;
        set => _textElement.ColorOverride = value;
    }

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
        get => mAutoSizeToContents;
        set
        {
            mAutoSizeToContents = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Text padding.
    /// </summary>
    public Padding TextPadding
    {
        get => mTextPadding;
        set
        {
            mTextPadding = value;
            Invalidate();
            InvalidateParent();
        }
    }

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        if (typeof(Label) == GetType())
        {
            obj.Add("BackgroundTemplate", mBackgroundTemplateFilename);
        }

        obj.Add("TextColor", Color.ToString(TextColor));
        obj.Add("HoveredTextColor", Color.ToString(mHoverTextColor));
        obj.Add("ClickedTextColor", Color.ToString(mClickedTextColor));
        obj.Add("DisabledTextColor", Color.ToString(mDisabledTextColor));
        obj.Add("TextAlign", mAlign.ToString());
        obj.Add("TextPadding", Padding.ToString(mTextPadding));
        obj.Add("AutoSizeToContents", mAutoSizeToContents);
        obj.Add("Font", fontInfo);
        obj.Add("TextScale", _textElement.GetScale());

        return base.FixJson(obj);
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

        mDisabledTextColor = Color.FromString((string)obj["DisabledTextColor"], new Color(255, 90, 90, 90));
        if (obj["TextAlign"] != null)
        {
            mAlign = (Pos)Enum.Parse(typeof(Pos), (string)obj["TextAlign"]);
        }

        if (obj["TextPadding"] != null)
        {
            TextPadding = Padding.FromString((string)obj["TextPadding"]);
        }

        if (obj["AutoSizeToContents"] != null)
        {
            mAutoSizeToContents = (bool)obj["AutoSizeToContents"];
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
                    fontInfo = stringFont;
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
        TextColor = Skin.Colors.Label.Default;
    }

    public virtual void MakeColorBright()
    {
        TextColor = Skin.Colors.Label.Bright;
    }

    public virtual void MakeColorDark()
    {
        TextColor = Skin.Colors.Label.Dark;
    }

    public virtual void MakeColorHighlight()
    {
        TextColor = Skin.Colors.Label.Highlight;
    }

    public override event GwenEventHandler<ClickedEventArgs> Clicked
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

    public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
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

    public override event GwenEventHandler<ClickedEventArgs> RightClicked
    {
        add
        {
            base.RightClicked += value;
            MouseInputEnabled = ClickEventAssigned;
        }
        remove
        {
            base.RightClicked -= value;
            MouseInputEnabled = ClickEventAssigned;
        }
    }

    public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
    {
        add
        {
            base.DoubleRightClicked += value;
            MouseInputEnabled = ClickEventAssigned;
        }
        remove
        {
            base.DoubleRightClicked -= value;
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

        if (mAutoSizeToContents)
        {
            SizeToContents();
        }

        AlignTextElement(_textElement);
    }

    protected void AlignTextElement(Text textElement)
    {
        var align = mAlign;

        var x = mTextPadding.Left + Padding.Left;
        var y = mTextPadding.Top + Padding.Top;

        if (0 != (align & Pos.Right))
        {
            x = Width - textElement.Width - mTextPadding.Right - Padding.Right;
        }

        if (0 != (align & Pos.CenterH))
        {
            x = (int)(mTextPadding.Left +
                      Padding.Left +
                      (Width -
                       textElement.Width -
                       mTextPadding.Left -
                       Padding.Left -
                       mTextPadding.Right -
                       Padding.Right) *
                      0.5f);
        }

        if (0 != (align & Pos.CenterV))
        {
            y = (int)(mTextPadding.Top +
                      Padding.Top +
                      (Height - textElement.Height) * 0.5f -
                      mTextPadding.Bottom -
                      Padding.Bottom);
        }

        if (0 != (align & Pos.Bottom))
        {
            y = Height - textElement.Height - mTextPadding.Bottom - Padding.Bottom;
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

        if (mAutoSizeToContents)
        {
            SizeToContents();
        }

        Invalidate();
        InvalidateParent();

        if (doEvents)
        {
            OnTextChanged();
        }
    }

    public virtual void SetTextScale(float scale)
    {
        _textElement.SetScale(scale);
        if (mAutoSizeToContents)
        {
            SizeToContents();
        }

        Invalidate();
        InvalidateParent();
    }

    public virtual void SizeToContents()
    {
        _textElement.SetPosition(mTextPadding.Left + Padding.Left, mTextPadding.Top + Padding.Top);

        var newWidth = _textElement.Width + Padding.Left + Padding.Right + mTextPadding.Left + mTextPadding.Right;
        newWidth = Math.Max(newWidth, MinimumSize.X);

        var newHeight = _textElement.Height + Padding.Top + Padding.Bottom + mTextPadding.Top + mTextPadding.Bottom;
        newHeight = Math.Max(newHeight, MinimumSize.Y);

        if (!SetSize(newWidth, newHeight))
        {
            return;
        }

        ProcessAlignments();
        InvalidateParent();
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
        var textColor = GetTextColor(ControlState.Normal);
        if (IsDisabled && GetTextColor(ControlState.Disabled) != null)
        {
            textColor = GetTextColor(ControlState.Disabled);
        }
        else if (IsHovered && GetTextColor(ControlState.Hovered) != null)
        {
            textColor = GetTextColor(ControlState.Hovered);
        }

        if (textColor != null)
        {
            TextColor = textColor;

            return;
        }

        if (IsDisabled)
        {
            TextColor = Skin.Colors.Button.Disabled;

            return;
        }

        if (IsHovered && ClickEventAssigned)
        {
            TextColor = Skin.Colors.Button.Hover;

            return;
        }

        TextColor = Skin.Colors.Button.Normal;
    }

    public virtual void SetTextColor(Color clr, ControlState state)
    {
        switch (state)
        {
            case ControlState.Normal:
                mNormalTextColor = clr;

                break;
            case ControlState.Hovered:
                mHoverTextColor = clr;

                break;
            case ControlState.Clicked:
                mClickedTextColor = clr;

                break;
            case ControlState.Disabled:
                mDisabledTextColor = clr;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        UpdateColors();
    }

    public virtual Color? GetTextColor(ControlState state)
    {
        switch (state)
        {
            case ControlState.Normal:
                return mNormalTextColor;
            case ControlState.Hovered:
                return mHoverTextColor;
            case ControlState.Clicked:
                return mClickedTextColor;
            case ControlState.Disabled:
                return mDisabledTextColor;
            default:
                return null;
        }
    }

}
