using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Control;

public class ScrollPanel : Base
{
    private readonly ScrollControl _scroller;

    internal ScrollPanel(ScrollControl parent, string? name = nameof(ScrollPanel)) : base(parent: parent, name: name)
    {
        _scroller = parent;

        MouseInputEnabled = false;
        Padding = Padding.Four;
    }

    protected override void ApplyDockFill(Base child, Point position, Point size)
    {
        var childSize = child.Size;
        if (_scroller.OverflowX is OverflowBehavior.Auto or OverflowBehavior.Scroll)
        {
            size.X = Math.Max(size.X, childSize.X);
        }

        if (_scroller.OverflowY is OverflowBehavior.Auto or OverflowBehavior.Scroll)
        {
            size.Y = Math.Max(size.Y, childSize.Y);
        }

        base.ApplyDockFill(child, position, size);
    }

    public override Point GetChildrenSize()
    {
        var childrenSize = base.GetChildrenSize();
        return childrenSize;
    }

    protected override void OnPositionChanged(Point oldPosition, Point newPosition)
    {
        base.OnPositionChanged(oldPosition, newPosition);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }
}