using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Displays text. Always sized to contents.
/// </summary>
public partial class Text : Base
{

    private GameFont? _font;

    private float _scale = 1f;

    private string? _displayedText;
    private string[] _lines = [];
    private WrappingBehavior _wrappingBehavior;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Text" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name">The name of the element.</param>
    public Text(Base parent, string? name = default) : base(parent, name)
    {
        _font = Skin.DefaultFont;
        Color = Skin.Colors.Label.Normal;
        MouseInputEnabled = false;
        ColorOverride = Color.FromArgb(0, 255, 255, 255); // A==0, override disabled
    }

    /// <summary>
    ///     Font used to display the text.
    /// </summary>
    /// <remarks>
    ///     The font is not being disposed by this class.
    /// </remarks>
    public GameFont? Font
    {
        get => _font;
        set
        {
            if (value == _font)
            {
                return;
            }

            _font = value;
            RecalculateLines();
        }
    }

    /// <summary>
    ///     Text to display.
    /// </summary>
    public string? DisplayedText
    {
        get => _displayedText;
        set
        {
            if (string.Equals(value, _displayedText, StringComparison.Ordinal))
            {
                return;
            }

            _displayedText = value;
            RecalculateLines();
        }
    }

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
            RecalculateLines();
        }
    }

    private void RecalculateLines()
    {
        var wrappingBehavior = ((Parent as Label)?.WrappingBehavior ?? WrappingBehavior.NoWrap);
        _lines = wrappingBehavior switch
        {
            WrappingBehavior.Wrapped => WrapText(
                _displayedText,
                Parent?.MaximumSize.X ?? 0,
                Font,
                Skin.Renderer
            ),
            WrappingBehavior.NoWrap => _displayedText == null ? [] : [_displayedText],
            _ => throw new NotImplementedException($"{nameof(WrappingBehavior)} '{wrappingBehavior}' not implemented"),
        };
        SizeToContents();
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

    protected override void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnVisibilityChanged(sender, eventArgs);

        if (_displayedText == "This feature is experimental and may cause issues when enabled.")
        {
            this.ToString();
        }
    }

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

        var font = _font;
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
            skin.Renderer.RenderText(font, cursor, line, _scale);

            if (_lines.Length <= 1)
            {
                break;
            }

            cursor.Y += skin.Renderer.MeasureText(text: line, font: font, scale: _scale).Y;
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
        SizeToContents();
        base.Layout(skin);
    }

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
    private void SizeToContents()
    {
        if (!HasSkin)
        {
            return;
        }

        var font = Font;
        if (font == default)
        {
            return;
        }

        Point newSize;

        if (string.IsNullOrEmpty(_displayedText))
        {
            const string verticalBar = "|";
            newSize = Skin.Renderer.MeasureText(font, verticalBar, _scale) with { X = 0 };
        }
        else
        {
            newSize = default;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var line in _lines)
            {
                var lineSize = Skin.Renderer.MeasureText(font, _displayedText, _scale);
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
            return;
        }

        if (Parent is TextBox)
        {
            Parent?.ToString();
        }

        Size = newSize;
        Invalidate();
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

        var font = _font;
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
        var substringSize = Skin.Renderer.MeasureText(font, substring) with
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

    public static string[] WrapText(string? input, int width, GameFont font, ITextHelper textHelper)
    {
        var sanitizedInput = input?.ReplaceLineEndings("\n");
        var inputLines = (sanitizedInput ?? string.Empty).Split('\n', StringSplitOptions.TrimEntries);

        if (string.IsNullOrWhiteSpace(sanitizedInput) || width < 1)
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

            var lastSpace = 0;
            var curPos = 0;
            var curLen = 1;
            var lastOk = 0;
            var lastCut = 0;
            float measured;
            string line;
            while (curPos + curLen < inputLine.Length)
            {
                line = inputLine.Substring(curPos, curLen);
                measured = textHelper.MeasureText(line, font, 1).X;
                if (measured < width)
                {
                    lastOk = lastSpace;
                    lastSpace = inputLine[curPos + curLen] switch
                    {
                        ' ' or '-' => curLen,
                        _ => lastSpace
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
