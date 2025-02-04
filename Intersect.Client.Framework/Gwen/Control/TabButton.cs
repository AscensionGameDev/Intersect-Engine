using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Tab header.
/// </summary>
public partial class TabButton : Button
{

    private TabControl? _tabControl;

    private Base _page;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TabButton" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public TabButton(Base parent) : base(parent)
    {
        DragAndDrop_SetPackage(true, "TabButtonMove");
        Font = GameContentManager.Current?.GetFont("sourcesansproblack", 10);
        KeyboardInputEnabled = true;
        Padding = Padding.Two;
        TextAlign = Pos.Top | Pos.Left;
        TextPadding = new Padding(4, 2);
    }

    /// <summary>
    ///     Indicates whether the tab is active.
    /// </summary>
    public bool IsActive => _page is { IsVisible: true };

    // todo: remove public access
    public TabControl? TabControl
    {
        get => _tabControl;
        set
        {
            if (value == _tabControl)
            {
                return;
            }

            _tabControl?.OnLoseTab(this);
            _tabControl = value;
        }
    }

    /// <summary>
    ///     Interior of the tab.
    /// </summary>
    public Base Page
    {
        get => _page;
        set => _page = value;
    }

    /// <summary>
    ///     Determines whether the control should be clipped to its bounds while rendering.
    /// </summary>
    protected override bool ShouldClip => false;

    public override void DragAndDrop_StartDragging(DragDrop.Package package, int x, int y)
    {
        IsHidden = true;
    }

    public override void DragAndDrop_EndDragging(bool success, int x, int y)
    {
        IsHidden = false;
        IsDepressed = false;
    }

    public override bool DragAndDrop_ShouldStartDrag() => _tabControl?.AllowReorder ?? false;

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawTabButton(this, IsActive, _tabControl.TabStrip.Dock);
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDown(bool down)
    {
        OnKeyRight(down);

        return true;
    }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyUp(bool down)
    {
        OnKeyLeft(down);

        return true;
    }

    public virtual void Select() => TabControl?.OnTabPressed(this, EventArgs.Empty);

    /// <summary>
    ///     Handler for Right Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyRight(bool down)
    {
        if (down)
        {
            var count = Parent.Children.Count;
            var me = Parent.Children.IndexOf(this);
            if (me + 1 < count)
            {
                var nextTab = Parent.Children[me + 1];
                TabControl.OnTabPressed(nextTab, EventArgs.Empty);
                InputHandler.KeyboardFocus = nextTab;
            }
        }

        return true;
    }

    /// <summary>
    ///     Handler for Left Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyLeft(bool down)
    {
        if (down)
        {
            var count = Parent.Children.Count;
            var me = Parent.Children.IndexOf(this);
            if (me - 1 >= 0)
            {
                var prevTab = Parent.Children[me - 1];
                TabControl.OnTabPressed(prevTab, EventArgs.Empty);
                InputHandler.KeyboardFocus = prevTab;
            }
        }

        return true;
    }

    /// <summary>
    ///     Updates control colors.
    /// </summary>
    public override void UpdateColors()
    {
        if (IsActive)
        {
            if (IsDisabled)
            {
                TextColor = Skin.Colors.Tab.Active.Disabled;

                return;
            }

            if (IsDepressed)
            {
                TextColor = Skin.Colors.Tab.Active.Down;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.Colors.Tab.Active.Hover;

                return;
            }

            TextColor = Skin.Colors.Tab.Active.Normal;
        }

        if (IsDisabled)
        {
            TextColor = Skin.Colors.Tab.Inactive.Disabled;

            return;
        }

        if (IsDepressed)
        {
            TextColor = Skin.Colors.Tab.Inactive.Down;

            return;
        }

        if (IsHovered)
        {
            TextColor = Skin.Colors.Tab.Inactive.Hover;

            return;
        }

        TextColor = Skin.Colors.Tab.Inactive.Normal;
    }

}
