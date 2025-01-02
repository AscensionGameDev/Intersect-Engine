using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;


/// <summary>
///     Displays text. Always sized to contents.
/// </summary>
public partial class Text : Base
{

    private GameFont? _font;

    private float _scale = 1f;

    private string? _displayedText;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Text" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name">The name of the element.</param>
    public Text(Base parent, string? name = default) : base(parent, name)
    {
        _font = Skin.DefaultFont;
        _displayedText = String.Empty;
        Color = Skin.Colors.Label.Default;
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
            SizeToContents();
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
            SizeToContents();
        }
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

        skin.Renderer.RenderText(font, Point.Empty, _displayedText, _scale);

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

        if (_font == default)
        {
            return;
        }

        Point newSize;

        if (string.IsNullOrEmpty(_displayedText))
        {
            const string verticalBar = "|";
            newSize = Skin.Renderer.MeasureText(_font, verticalBar, _scale) with { X = 0 };
        }
        else
        {
            newSize = Skin.Renderer.MeasureText(_font, _displayedText, _scale);
        }

        if (newSize.X == Width && newSize.Y == Height)
        {
            return;
        }

        if (Size == newSize)
        {
            return;
        }

        Size = newSize;
        InvalidateParent();
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

        var closestDistance = MAX_COORD;
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

}
