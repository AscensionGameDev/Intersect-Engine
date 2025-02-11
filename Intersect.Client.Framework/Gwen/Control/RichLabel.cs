using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Multiline label with text chunks having different color/font.
/// </summary>
public partial class RichLabel : Base
{
    private readonly string[] _newlines = new[]{ Environment.NewLine, "\n" }.Distinct().ToArray();
    private readonly List<TextBlock> _textBlocks = [];
    private readonly List<Label> _formattedLabels = [];

    private GameFont? _font;

    private string? _fontInfo;

    private bool mNeedsRebuild;

    public List<Label> FormattedLabels => _formattedLabels;

    public GameFont? Font
    {
        get => _font;
        set
        {
            _font = value;
            _fontInfo = value is { } font ? $"{font.GetName()},{font.GetSize()}" : null;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RichLabel" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public RichLabel(Base parent, string? name = default) : base(parent, name)
    {
    }

    /// <summary>
    ///     Adds a line break to the control.
    /// </summary>
    public void AddLineBreak()
    {
        var block = new TextBlock { Type = BlockType.NewLine };
        _textBlocks.Add(block);
    }

    /// <inheritdoc />
    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);

        if (obj["Font"] != null && obj["Font"].Type != JTokenType.Null)
        {
            var fontArr = ((string)obj["Font"]).Split(',');
            _fontInfo = (string)obj["Font"];
            _font = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
        }
    }

    /// <param name="isRoot"></param>
    /// <param name="onlySerializeIfNotEmpty"></param>
    /// <inheritdoc />
    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add("Font", _fontInfo);

        return base.FixJson(serializedProperties);
    }

    /// <summary>
    ///     Adds text to the control via the properties of a <see cref="Label"/>
    /// </summary>
    /// <param name="text">Text to add</param>
    /// <param name="template">Label to use as a template</param>
    public void AddText(string text, Label? template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        AddText(
            text,
            template.TextColor,
            template.CurAlignments.Count > 0 ? template.CurAlignments[0] : Alignments.Left,
            template.Font
        );
    }

    /// <summary>
    ///     Adds text to the control.
    /// </summary>
    /// <param name="text">Text to add.</param>
    /// <param name="color">Text color.</param>
    /// <param name="font">Font to use.</param>
    public void AddText(string text, Color? color, Alignments alignment = Alignments.Left, GameFont? font = default)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        font ??= _font;

        var lines = text.Split(_newlines, StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                AddLineBreak();
            }

            var block = new TextBlock
            {
                Type = BlockType.Text,
                Text = lines[i],
                Color = color,
                Font = font,
                Alignment = alignment
            };

            _textBlocks.Add(block);
            mNeedsRebuild = true;
            Invalidate();
        }
    }

    /// <summary>
    ///     Resizes the control to fit its children.
    /// </summary>
    /// <param name="resizeX">Determines whether to change control's width.</param>
    /// <param name="resizeY">Determines whether to change control's height.</param>
    /// <param name="recursive"></param>
    /// <returns>
    ///     True if bounds changed.
    /// </returns>
    public override bool SizeToChildren(bool resizeX = true, bool resizeY = true, bool recursive = false)
    {
        Rebuild();

        return base.SizeToChildren(resizeX: resizeX, resizeY: resizeY, recursive: recursive);
    }

    protected override Point ApplyDockFillOnSizeToChildren(Point size, Point internalSize)
    {
        if (size.X >= internalSize.X && size.Y >= internalSize.Y)
        {
            return size;
        }

        return base.ApplyDockFillOnSizeToChildren(size, internalSize);
    }

    public override Point GetChildrenSize()
    {
        var childrenSize = base.GetChildrenSize();
        return childrenSize;
    }

    protected void SplitLabel(
        string text,
        GameFont font,
        TextBlock block,
        ref int x,
        ref int y,
        ref int lineHeight,
        int initialX,
        int availableWidth
    )
    {
        var spaced = Util.SplitAndKeep(text, " ");
        if (spaced.Length == 0)
        {
            return;
        }

        var spaceLeft = availableWidth - x;
        string leftOver;

        // Does the whole word fit in?
        var stringSize = Skin.Renderer.MeasureText(font, text);
        if (spaceLeft > stringSize.X)
        {
            CreateLabel(
                text,
                block,
                ref x,
                ref y,
                ref lineHeight,
                initialX,
                availableWidth,
                true
            );

            return;
        }

        // If the first word is bigger than the line, just give up.
        var wordSize = Skin.Renderer.MeasureText(font, spaced[0]);
        if (wordSize.X >= spaceLeft)
        {
            CreateLabel(
                spaced[0],
                block,
                ref x,
                ref y,
                ref lineHeight,
                initialX,
                availableWidth,
                true
            );
            if (spaced[0].Length >= text.Length)
            {
                return;
            }

            leftOver = text.Substring(spaced[0].Length + 1);
            SplitLabel(
                leftOver,
                font,
                block,
                ref x,
                ref y,
                ref lineHeight,
                initialX,
                availableWidth
            );

            return;
        }

        var newString = String.Empty;
        for (var i = 0; i < spaced.Length; i++)
        {
            wordSize = Skin.Renderer.MeasureText(font, newString + spaced[i]);
            if (wordSize.X > spaceLeft)
            {
                CreateLabel(
                    newString,
                    block,
                    ref x,
                    ref y,
                    ref lineHeight,
                    initialX,
                    availableWidth,
                    true
                );
                x = 0;
                y += lineHeight;

                break;
            }

            newString += spaced[i];
        }

        var newstrLen = newString.Length;
        if (newstrLen < text.Length)
        {
            leftOver = text.Substring(newstrLen + 1);
            SplitLabel(
                leftOver,
                font,
                block,
                ref x,
                ref y,
                ref lineHeight,
                initialX,
                availableWidth
            );
        }
    }

    protected void CreateLabel(
        string text,
        TextBlock block,
        ref int x,
        ref int y,
        ref int lineHeight,
        int initialX,
        int availableWidth,
        bool noSplit
    )
    {
        // Use default font or is one set?
        var font = Skin.DefaultFont;
        if (block.Font != null)
        {
            font = block.Font;
        }

        // This string is too long for us, split it up.
        var p = Skin.Renderer.MeasureText(font, text);

        if (lineHeight == -1)
        {
            lineHeight = p.Y;
        }

        if (!noSplit)
        {
            if (x + p.X > Width)
            {
                SplitLabel(
                    text,
                    font,
                    block,
                    ref x,
                    ref y,
                    ref lineHeight,
                    initialX,
                    availableWidth
                );

                return;
            }
        }

        // Wrap
        if (x + p.X > Width)
        {
            CreateNewline(ref x, ref y, lineHeight, initialX);
        }

        var label = new Label(this)
        {
            X = x,
            Y = y,
            Alignment = [block.Alignment],
            AutoSizeToContents = true,
            Font = font,
            RestrictToParent = false,
            Text = x == 0 ? text.TrimStart(' ') : text,
            TextAlign = Pos.None,
            TextColorOverride = block.Color,
        };

        label.SizeToContents();

        _formattedLabels.Add(label);

        x += label.Width;

        if (x > availableWidth)
        {
            CreateNewline(ref x, ref y, lineHeight, initialX);
        }
    }

    protected void CreateNewline(ref int x, ref int y, int lineHeight, int initialX)
    {
        x = initialX;
        y += lineHeight;
    }

    public void ClearText()
    {
        _textBlocks.Clear();
        mNeedsRebuild = true;
    }

    protected void Rebuild()
    {
        DeleteAllChildren();
        _formattedLabels.Clear();

        var bounds = Bounds;
        var padding = Padding;
        var initialX = padding.Left;
        var availableWidth = bounds.Width - (initialX + padding.Right);
        var x = initialX;
        var y = padding.Top;
        var lineHeight = -1;

        foreach (var block in _textBlocks)
        {
            if (block.Type == BlockType.NewLine)
            {
                CreateNewline(ref x, ref y, lineHeight, initialX);

                continue;
            }

            if (block.Type == BlockType.Text)
            {
                CreateLabel(block.Text, block, ref x, ref y, ref lineHeight, initialX, availableWidth, false);

                continue;
            }
        }

        mNeedsRebuild = false;
    }

    /// <summary>
    ///     Handler invoked when control's bounds change.
    /// </summary>
    /// <param name="oldBounds">Old bounds.</param>
    /// <param name="newBounds"></param>
    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);

        SizeToChildren(resizeX: true, resizeY: true);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
        if (mNeedsRebuild)
        {
            Rebuild();
        }

        // align bottoms. this is still not ideal, need to take font metrics into account.
        Base prev = null;
        foreach (var child in Children)
        {
            if (prev != null && child.Y == prev.Y)
            {
                Align.PlaceRightBottom(child, prev);
            }

            prev = child;
        }
    }

    protected partial struct TextBlock
    {

        public BlockType Type;

        public string Text;

        public Color Color;

        public GameFont Font;

        public Alignments Alignment;

    }

    protected enum BlockType
    {

        Text,

        NewLine

    }

}
