using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Control.Layout;

public class TableCell : Label
{
    public TableCell(TableRow row, string? name = nameof(TableCell)) : base(parent: row, name: name)
    {
    }

    protected override void OnDockChanged(Pos oldDock, Pos newDock)
    {
        base.OnDockChanged(oldDock, newDock);
    }

    protected override void Layout(Skin.Base skin)
    {
        base.Layout(skin);
    }

    public override bool SizeToChildren(bool resizeX = true, bool resizeY = true, bool recursive = false)
    {
        return base.SizeToChildren(resizeX, resizeY, recursive);
    }

    public override bool SizeToContents(out Point contentSize)
    {
        return base.SizeToContents(out contentSize);
    }

    public override Point GetChildrenSize()
    {
        var childrenSize = base.GetChildrenSize();
        return childrenSize;
    }

    protected override Point GetContentSize()
    {
        var contentSize = base.GetContentSize();
        return contentSize;
    }

    protected override void OnSizeChanged(Point oldSize, Point newSize)
    {
        base.OnSizeChanged(oldSize, newSize);
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);
    }
}