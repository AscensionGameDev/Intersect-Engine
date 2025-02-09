using Intersect.Client.Framework.Gwen.Control;

namespace Intersect.Client.Framework.Gwen;


public enum Alignments
{

    Top,

    Bottom,

    Left,

    Right,

    Center,

    CenterH,

    CenterV

}

/// <summary>
///     Utility class for manipulating control's position according to its parent. Rarely needed, use control.Dock.
/// </summary>
public static partial class Align
{
    /// <summary>
    ///     Moves the control to the left of its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void AlignLeft(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        control.SetPosition(parent.Padding.Left + control.Margin.Left + control.AlignmentPadding.Left, control.Y);
    }

    /// <summary>
    ///     Moves the control to the right of its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void AlignRight(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        var offsetRight = control.Width + control.Margin.Right + control.AlignmentPadding.Right + parent.Padding.Right;
        control.SetPosition(parent.Width - offsetRight, control.Y);
    }

    /// <summary>
    ///     Moves the control to the top of its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void AlignTop(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        control.SetPosition(control.X, control.Margin.Top + parent.Padding.Top + control.AlignmentPadding.Top);
    }

    /// <summary>
    ///     Moves the control to the bottom of its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void AlignBottom(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        var offsetBottom = control.Height +
                           control.Margin.Bottom +
                           control.AlignmentPadding.Bottom +
                           parent.Padding.Bottom;
        control.SetPosition(control.X, parent.Height - offsetBottom);
    }

    /// <summary>
    ///     Centers the control inside its parent.
    /// </summary>
    /// <param name="control">Control to center.</param>
    public static void Center(Base control)
    {
        var parent = control.Parent;
        if (parent == null)
        {
            return;
        }

        var parentPadding = parent.Padding;
        var parentPaddingH = parentPadding.Left + parentPadding.Right;
        var parentPaddingV = parentPadding.Top + parentPadding.Bottom;

        Point offset = new(control.AlignmentPadding.Left, control.AlignmentPadding.Top);
        offset.X += parentPadding.Left;
        offset.Y += parentPadding.Top;

        var availableHeight = parent.Height - parentPaddingV;
        var availableWidth = parent.Width - parentPaddingH;

        Point alignedPosition = new(availableWidth - control.Width, availableHeight - control.Height);
        alignedPosition /= 2;
        control.SetPosition(offset + alignedPosition);
    }

    /// <summary>
    ///     Centers the control horizontally inside its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void CenterHorizontally(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        var parentPadding = parent.Padding;
        var parentPaddingH = parentPadding.Left + parentPadding.Right;

        Point offset = new(control.AlignmentPadding.Left, control.Y);
        offset.X += parentPadding.Left;

        var availableWidth = parent.Width - parentPaddingH;

        Point alignedPosition = new(availableWidth - control.Width, 0);
        alignedPosition /= 2;
        control.SetPosition(offset + alignedPosition);
    }

    /// <summary>
    ///     Centers the control vertically inside its parent.
    /// </summary>
    /// <param name="control"></param>
    public static void CenterVertically(Base control)
    {
        var parent = control.Parent;
        if (null == parent)
        {
            return;
        }

        var parentPadding = parent.Padding;
        var parentPaddingV = parentPadding.Top + parentPadding.Bottom;

        Point offset = new(control.X, control.AlignmentPadding.Top);
        offset.Y += parentPadding.Top;

        var availableHeight = parent.Height - parentPaddingV;

        Point alignedPosition = new(0, availableHeight - control.Height);
        alignedPosition /= 2;
        control.SetPosition(offset + alignedPosition);
    }

    /// <summary>
    ///     Places the control below other control (left aligned), taking margins into account.
    /// </summary>
    /// <param name="control">Control to place.</param>
    /// <param name="anchor">Anchor control.</param>
    /// <param name="spacing">Optional spacing.</param>
    public static void PlaceDownLeft(Base control, Base anchor, int spacing = 0)
    {
        control.SetPosition(anchor.X, anchor.Bottom + spacing);
    }

    /// <summary>
    ///     Places the control to the right of other control (bottom aligned), taking margins into account.
    /// </summary>
    /// <param name="control">Control to place.</param>
    /// <param name="anchor">Anchor control.</param>
    /// <param name="spacing">Optional spacing.</param>
    public static void PlaceRightBottom(Base control, Base anchor, int spacing = 0)
    {
        control.SetPosition(anchor.Right + spacing, anchor.Y - control.Height + anchor.Height);
    }

}
