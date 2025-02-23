using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Displays text. Always sized to contents.
/// </summary>
public partial class Text : Base
{
    public static readonly Color DefaultBoundsOutlineColor = Color.FromHex("bf4060", Color.Pink);
    public static readonly Color DefaultMarginOutlineColor = Color.FromHex("1fad1f", Color.ForestGreen);

    private IFont? _font;

    private float _scale = 1f;

    private string? _displayedText;
    private string[] _lines = [];
    private WrappingBehavior _wrappingBehavior;

    private int _lastParentInnerWidth;

    private bool _recalculateLines;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Text" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name">The name of the element.</param>
    public Text(Base parent, string? name = default) : base(parent, name)
    {
        if (SafeSkin is { } skin)
        {
            InitializeFromSkin(this);
            Color = skin.Colors.Label.Normal;
        }
        else
        {
            PreLayout.Enqueue(InitializeFromSkin, this);
        }

        MouseInputEnabled = false;
        ColorOverride = Color.FromArgb(0, 255, 255, 255); // A==0, override disabled

        BoundsOutlineColor = DefaultBoundsOutlineColor;
        MarginOutlineColor = DefaultMarginOutlineColor;

        _lastParentInnerWidth = SelectWidthFrom(parent);
        parent.SizeChanged += ParentOnSizeChanged;
    }

    private static void InitializeFromSkin(Text @this)
    {
        @this.Color = @this.Skin.Colors.Label.Normal;
    }

    private void ParentOnSizeChanged(Base @base, ValueChangedEventArgs<Point> eventArgs)
    {
        var newSelectedWidth = SelectWidthFrom(@base);
        if (_lastParentInnerWidth == newSelectedWidth)
        {
            return;
        }

        RecalculateLines(newSelectedWidth);
    }

    /// <summary>
    ///     Font used to display the text.
    /// </summary>
    /// <remarks>
    ///     The font is not being disposed by this class.
    /// </remarks>
    public IFont? Font
    {
        get => _font;
        set => SetAndDoIfChanged(ref _font, value, Invalidate);
    }

    private int _fontSize = 10;

    public int FontSize
    {
        get => _fontSize;
        set => SetAndDoIfChanged(ref _fontSize, value, Invalidate);
    }

    /// <summary>
    ///     Text to display.
    /// </summary>
    public string? DisplayedText
    {
        get => _displayedText;
        set => SetAndDoIfChanged(ref _displayedText, value, Invalidate, this);
    }

    public WrappingBehavior WrappingBehavior
    {
        get => _wrappingBehavior;
        set => SetAndDoIfChanged(ref _wrappingBehavior, value, Invalidate);
    }

    public override void Invalidate()
    {
        base.Invalidate();

        _recalculateLines = true;
    }

    private static int SelectWidthFrom(Base? parent) =>
        parent == null ? 0 : Math.Max(0, Math.Max(parent.MaximumInnerWidth, parent.InnerWidth));

    private void RecalculateLines(int parentInnerWidth = 0)
    {
        if (_lines.Length == 0 && string.IsNullOrEmpty(_displayedText))
        {
            return;
        }

        var parent = Parent;
        if (parentInnerWidth < 1)
        {
            parentInnerWidth = SelectWidthFrom(parent);
        }

        // If the last calculation yielded one line and the new width is not smaller (or if it's less than one), skip
        if (_lines.Length == 1 && (parentInnerWidth >= _lastParentInnerWidth || parentInnerWidth < 1))
        {
            // But only skip if we're not oversize
            if (parentInnerWidth > Width)
            {
                return;
            }
        }

        _lastParentInnerWidth = parentInnerWidth;

        var wrappingBehavior = (parent as Label)?.WrappingBehavior ?? WrappingBehavior.NoWrap;
        _lines = wrappingBehavior switch
        {
            WrappingBehavior.Wrapped => WrapText(
                _displayedText,
                parentInnerWidth,
                Font,
                FontSize,
                Skin.Renderer
            ),
            WrappingBehavior.NoWrap => _displayedText == null ? [] : [_displayedText],
            _ => throw new NotImplementedException($"{nameof(WrappingBehavior)} '{wrappingBehavior}' not implemented"),
        };

        SizeToContents(skipRecalculateLines: true);
    }

    /// <summary>
    ///     Text color.
    /// </summary>
    public Color? Color { get; set; }

    /// <summary>
    ///     Determines whether the control should be automatically resized to fit the text.
    /// </summary>
    /// <summary>
    ///     Text length in characters.
    /// </summary>
    public int Length => _displayedText?.Length ?? 0;

    /// <summary>
    ///     Text color override - used by tooltips.
    /// </summary>
    public Color? ColorOverride { get; set; }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        if (string.IsNullOrEmpty(_displayedText))
        {
            return;
        }

        var font = _font ?? SafeSkin?.DefaultFont;
        if (font == default)
        {
            return;
        }

        var color = Color ?? Color.White;
        var colorOverride = ColorOverride;

        if (colorOverride == default || colorOverride.A == 0)
        {
            skin.Renderer.DrawColor = color;
        }
        else
        {
            skin.Renderer.DrawColor = colorOverride;
        }

        Point cursor = default;
        foreach (var line in _lines)
        {
            skin.Renderer.RenderText(font, _fontSize, cursor, line, _scale);

            if (_lines.Length <= 1)
            {
                break;
            }

            cursor.Y += skin.Renderer.MeasureText(font: font, fontSize: _fontSize, text: line, scale: _scale).Y;
        }

#if DEBUG_TEXT_MEASURE
        {
            Point lastPos = Point.Empty;

            for (int i = 0; i < m_String.Length + 1; i++)
            {
                String sub = (TextOverride ?? String).Substring(0, i);
                Point p = Skin.Renderer.MeasureText(Font, sub);

                Rectangle rect = new Rectangle();
                rect.Location = lastPos;
                rect.Size = new Size(p.X - lastPos.X, p.Y);
                skin.Renderer.DrawColor = Color.FromArgb(64, 0, 0, 0);
                skin.Renderer.DrawLinedRect(rect);

                lastPos = new Point(rect.Right, 0);
            }
        }
#endif
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        if (_recalculateLines || _lines.Length < 1)
        {
            _lines = [];
            RecalculateLines();
            _recalculateLines = false;
        }

        SizeToContents();
        base.Layout(skin);
    }

    public override bool SizeToChildren(SizeToChildrenArgs args) => SizeToContents();

    /// <summary>
    ///     Handler invoked when control's scale changes.
    /// </summary>
    protected override void OnScaleChanged()
    {
        Invalidate();
    }

    public void SetScale(float scale)
    {
        _scale = scale;
        OnScaleChanged();
    }

    public float GetScale()
    {
        return _scale;
    }

    /// <summary>
    ///     Sizes the control to its contents.
    /// </summary>
    private bool SizeToContents(bool skipRecalculateLines = false)
    {
        if (!HasSkin)
        {
            return false;
        }

        if (IsHidden)
        {
            return false;
        }

        var font = Font;
        if (font == default)
        {
            return false;
        }

        if (!skipRecalculateLines)
        {
            RecalculateLines();
        }

        Point newSize;

        if (string.IsNullOrEmpty(_displayedText))
        {
            const string verticalBar = "|";
            newSize = Skin.Renderer.MeasureText(font, fontSize: _fontSize, verticalBar, _scale) with { X = 0 };
        }
        else
        {
            newSize = default;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var line in _lines)
            {
                var lineSize = Skin.Renderer.MeasureText(font, fontSize: _fontSize,line, _scale);
                newSize = new Point(
                    Math.Max(newSize.X, lineSize.X),
                    newSize.Y + lineSize.Y
                );
            }
        }

        newSize.X = Math.Clamp(newSize.X, MinimumSize.X, MaximumSize.X > 0 ? MaximumSize.X : int.MaxValue);
        newSize.Y = Math.Clamp(newSize.Y, MinimumSize.Y, MaximumSize.Y > 0 ? MaximumSize.Y : int.MaxValue);

        if (Size == newSize)
        {
            return false;
        }

        Size = newSize;
        Invalidate();

        return true;
    }

    /// <summary>
    ///     Gets the coordinates of specified character in the text.
    /// </summary>
    /// <param name="index">Character index.</param>
    /// <returns>Character position in local coordinates.</returns>
    public Point GetCharacterPosition(int index)
    {
        if (index < 1)
        {
            return default;
        }

        var font = _font ?? SafeSkin?.DefaultFont;
        if (font == default)
        {
            return default;
        }

        var displayedText = _displayedText;
        if (string.IsNullOrEmpty(displayedText))
        {
            return default;
        }

        if (index > displayedText.Length)
        {
            return default;
        }

        var substring = displayedText[..index];
        var substringSize = Skin.Renderer.MeasureText(font, fontSize: _fontSize, substring) with
        {
            Y = 0,
        };

        return substringSize;
    }

    /// <summary>
    ///     Searches for a character closest to given point.
    /// </summary>
    /// <param name="point">Point.</param>
    /// <returns>Character index.</returns>
    public int GetClosestCharacter(Point point)
    {
        var displayedText = _displayedText;
        if (string.IsNullOrEmpty(displayedText))
        {
            return -1;
        }

        var closestDistance = int.MaxValue;
        var closestIndex = 0;

        for (var characterIndex = 0; characterIndex < displayedText.Length + 1; characterIndex++)
        {
            var characterPosition = GetCharacterPosition(characterIndex);

            // Since GetCharacterPosition always returns a Y of 0 we don't need to factor it into the distance
            var distance = Math.Abs(characterPosition.X - point.X);

            if (distance > closestDistance)
            {
                continue;
            }

            closestDistance = distance;
            closestIndex = characterIndex;
        }

        return closestIndex;
    }

    public static string[] WrapText(string? input, int width, IFont? font, int size, ITextHelper textHelper)
    {
        var sanitizedInput = input?.ReplaceLineEndings("\n");
        var inputLines = (sanitizedInput ?? string.Empty).Split('\n', StringSplitOptions.TrimEntries);

        if (string.IsNullOrWhiteSpace(sanitizedInput) || width < 1)
        {
            return inputLines;
        }

        if (font == null)
        {
            return inputLines;
        }

        List<string> lines = [];

        foreach (var inputLine in inputLines)
        {
            if (inputLine.Length < 1)
            {
                lines.Add(inputLine);
                continue;
            }

            float measured = textHelper.MeasureText(inputLine, font, size, 1).X;
            if (measured <= width)
            {
                lines.Add(inputLine);
                continue;
            }

            inputLine.Split(". ");

            var lastSpace = 0;
            var curPos = 0;
            var curLen = 1;
            var lastOk = 0;

            string line;
            while (curPos + curLen < inputLine.Length)
            {
                line = inputLine.Substring(curPos, curLen);
                measured = textHelper.MeasureText(line, font, size, 1).X;
                if (measured < width)
                {
                    lastOk = lastSpace;
                    lastSpace = inputLine[curPos + curLen] switch
                    {
                        ' ' or '-' => curLen,
                        _ => lastSpace,
                    };
                }
                else
                {
                    if (lastOk == 0)
                    {
                        lastOk = curLen - 1;
                    }

                    line = inputLine.Substring(curPos, lastOk).Trim();
                    lines.Add(line);

                    curPos += lastOk;
                    lastOk = 0;
                    lastSpace = 0;
                    curLen = 1;
                }

                curLen++;
            }

            var newLine = inputLine.Substring(curPos, inputLine.Length - curPos).Trim();
            lines.Add(newLine);
        }

        return lines.ToArray();
    }
}
