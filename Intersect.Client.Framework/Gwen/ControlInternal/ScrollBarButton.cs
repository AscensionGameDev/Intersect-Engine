using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen.ControlInternal;

/// <summary>
///     Scrollbar button.
/// </summary>
public partial class ScrollBarButton : Button
{
    private Pos _direction = Pos.Top;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScrollBarButton" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public ScrollBarButton(Base parent) : base(parent, disableText: true)
    {
        MinimumSize = new Point(15, 15);
        Size = new Point(15, 15);
        MaximumSize = new Point(15, 15);
    }

    public virtual Pos Direction => _direction;

    public void SetDirectionUp()
    {
        _direction = Pos.Top;
    }

    public void SetDirectionDown()
    {
        _direction = Pos.Bottom;
    }

    public void SetDirectionLeft()
    {
        _direction = Pos.Left;
    }

    public void SetDirectionRight()
    {
        _direction = Pos.Right;
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawScrollButton(this, _direction, IsActive, IsHovered, IsDisabled);
    }
}