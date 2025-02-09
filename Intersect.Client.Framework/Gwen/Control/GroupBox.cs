namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Group box (container).
/// </summary>
/// <remarks>Don't use autosize with docking.</remarks>
public partial class GroupBox : Label
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="GroupBox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public GroupBox(Base parent) : base(parent)
    {
        AutoSizeToContents = false;

        // Set to true, because it's likely that our
        // children will want mouse input, and they
        // can't get it without us..
        MouseInputEnabled = true;
        KeyboardInputEnabled = true;

        Padding = new Padding(10, 0, 10, 0);
        TextAlign = Pos.Top | Pos.Left;
        Invalidate();

        _innerPanel = new Base(this, name: nameof(_innerPanel));
        _innerPanel.Dock = Pos.Fill;
        _innerPanel.Margin = new Margin(5, TextHeight + 5, 5, 5);

        //Margin = new Margin(5, 5, 5, 5);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
        if (AutoSizeToContents)
        {
            DoSizeToContents();
        }
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawGroupBox(this, TextX, TextHeight, TextWidth);
    }

    /// <summary>
    ///     Sizes to contents.
    /// </summary>
    public override bool SizeToContents() => DoSizeToContents();

    protected virtual bool DoSizeToContents()
    {
        var sizeChanged = _innerPanel?.SizeToChildren() ?? false;
        sizeChanged &= SizeToChildren();

        var textWidth = TextWidth + Padding.Right + Padding.Left;

        // ReSharper disable once InvertIf
        if (Width < textWidth)
        {
            Width = textWidth;
            sizeChanged = true;
        }

        return sizeChanged;
    }
}
