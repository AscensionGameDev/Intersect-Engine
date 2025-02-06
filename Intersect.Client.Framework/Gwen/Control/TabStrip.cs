using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.DragDrop;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Tab strip - groups TabButtons and allows reordering.
/// </summary>
public partial class TabStrip : Base
{

    private bool mAllowReorder;

    private Base mTabDragControl;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TabStrip" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public TabStrip(Base parent) : base(parent)
    {
        mAllowReorder = false;
    }

    /// <summary>
    ///     Determines whether it is possible to reorder tabs by mouse dragging.
    /// </summary>
    public bool AllowReorder
    {
        get => mAllowReorder;
        set => mAllowReorder = value;
    }

    /// <summary>
    ///     Determines whether the control should be clipped to its bounds while rendering.
    /// </summary>
    protected override bool ShouldClip => false;

    /// <summary>
    ///     Strip position (top/left/right/bottom).
    /// </summary>
    public Pos StripPosition
    {
        get => Dock;
        set
        {
            Dock = value;
            if (Dock == Pos.Top)
            {
                Padding = new Padding(5, 0, 0, 0);
            }

            if (Dock == Pos.Left)
            {
                Padding = new Padding(0, 5, 0, 0);
            }

            if (Dock == Pos.Bottom)
            {
                Padding = new Padding(5, 0, 0, 0);
            }

            if (Dock == Pos.Right)
            {
                Padding = new Padding(0, 5, 0, 0);
            }
        }
    }

    public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
    {
        var localPos = CanvasPosToLocal(new Point(x, y));

        var button = DragAndDrop.SourceControl as TabButton;
        var tabControl = Parent as TabControl;
        if (tabControl != null && button != null)
        {
            if (button.TabControl != tabControl)
            {
                // We've moved tab controls!
                tabControl.AddPage(button);
            }
        }

        var droppedOn = GetControlAt(localPos.X, localPos.Y);
        if (droppedOn != null)
        {
            var dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
            DragAndDrop.SourceControl.BringNextToControl(droppedOn, dropPos.X > droppedOn.Width / 2);
        }
        else
        {
            DragAndDrop.SourceControl.BringToFront();
        }

        return true;
    }

    public override bool DragAndDrop_CanAcceptPackage(Package p)
    {
        if (!mAllowReorder)
        {
            return false;
        }

        if (p.Name == "TabButtonMove")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        var largestTab = new Point(5, 5);

        var children = Children.ToArray();
        for (var childIndex = 0; childIndex < children.Length; childIndex++)
        {
            var child = children[childIndex];
            if (child is not TabButton tab)
            {
                continue;
            }

            var tabSize = tab.MeasureShrinkToContents();

            Margin margin = new();
            var notFirst = childIndex > 0 ? -1 : 0;

            switch (Dock)
            {
                case Pos.Top:
                    margin.Left = notFirst;
                    tab.Dock = Pos.Left;
                    break;
                case Pos.Left:
                    margin.Top = notFirst;
                    tab.Dock = Pos.Top;
                    break;
                case Pos.Right:
                    margin.Top = notFirst;
                    tab.Dock = Pos.Top;
                    break;
                case Pos.Bottom:
                    margin.Left = notFirst;
                    tab.Dock = Pos.Left;
                    break;
            }

            largestTab.X = Math.Max(largestTab.X, tabSize.X);
            largestTab.Y = Math.Max(largestTab.Y, tabSize.Y);

            tab.Margin = margin;
        }

        if (Dock is Pos.Top or Pos.Bottom)
        {
            SetSize(Width, largestTab.Y);
        }

        if (Dock is Pos.Left or Pos.Right)
        {
            SetSize(largestTab.X, Height);
        }

        base.Layout(skin);
    }

    public override void DragAndDrop_HoverEnter(Package p, int x, int y)
    {
        if (mTabDragControl != null)
        {
            throw new InvalidOperationException("ERROR! TabStrip::DragAndDrop_HoverEnter");
        }

        mTabDragControl = new Highlight(this);
        mTabDragControl.MouseInputEnabled = false;
        mTabDragControl.SetSize(3, Height);
    }

    public override void DragAndDrop_HoverLeave(Package p)
    {
        if (mTabDragControl != null)
        {
            RemoveChild(mTabDragControl, false); // [omeg] need to do that explicitely
            mTabDragControl.Dispose();
        }

        mTabDragControl = null;
    }

    public override void DragAndDrop_Hover(Package p, int x, int y)
    {
        var localPos = CanvasPosToLocal(new Point(x, y));

        var droppedOn = GetControlAt(localPos.X, localPos.Y);
        if (droppedOn != null && droppedOn != this)
        {
            var dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
            mTabDragControl.SetBounds(new Rectangle(0, 0, 3, Height));
            mTabDragControl.BringToFront();
            mTabDragControl.SetPosition(droppedOn.X - 1, 0);

            if (dropPos.X > droppedOn.Width / 2)
            {
                mTabDragControl.MoveBy(droppedOn.Width - 1, 0);
            }

            mTabDragControl.Dock = Pos.None;
        }
        else
        {
            mTabDragControl.Dock = Pos.Left;
            mTabDragControl.BringToFront();
        }
    }

}
