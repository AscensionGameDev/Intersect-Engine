namespace Intersect.Client.Framework.Gwen;


/// <summary>
///     Represents relative position.
/// </summary>
[Flags]
public enum Pos
{

    None = 0,

    Left = 1 << 1,

    Right = 1 << 2,

    Top = 1 << 3,

    Bottom = 1 << 4,

    CenterV = 1 << 5,

    CenterH = 1 << 6,

    Fill = 1 << 7,

    Center = CenterV | CenterH,

}

public static class PosExtensions
{
    private const Pos MaskTopPriority = ~(Pos.Bottom | Pos.CenterV);
    private const Pos MaskBottomPriority = ~(Pos.Top | Pos.CenterV);
    private const Pos MaskLeftPriority = ~(Pos.Right | Pos.CenterH);
    private const Pos MaskRightPriority = ~(Pos.Left | Pos.CenterH);

    public static Point GetDockSpacing(this Pos dock, Padding dockSpacing)
    {
        if (dock.HasFlag(Pos.Fill) || dock.HasFlag(Pos.Center))
        {
            return default;
        }

        Point spacing = default;

        if (dock == (dock & MaskTopPriority))
        {
            spacing.Y += dockSpacing.Top;
        }
        else if (dock == (dock & MaskBottomPriority))
        {
            spacing.Y += dockSpacing.Bottom;
        }

        if (dock == (dock & MaskLeftPriority))
        {
            spacing.Y += dockSpacing.Left;
        }
        else if (dock == (dock & MaskRightPriority))
        {
            spacing.Y += dockSpacing.Right;
        }

        return spacing;
    }
}
