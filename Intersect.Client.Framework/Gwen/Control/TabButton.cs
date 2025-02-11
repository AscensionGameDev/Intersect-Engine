using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

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
    /// <param name="name"></param>
    public TabButton(Base parent, string? name = null) : base(parent: parent, name: name)
    {
        DragAndDrop_SetPackage(true, "TabButtonMove");
        Font = GameContentManager.Current?.GetFont("sourcesansproblack", 10);
        KeyboardInputEnabled = true;

        Padding = new Padding(4, 2);
        TextAlign = Pos.Top | Pos.Left;
    }

    /// <summary>
    ///     Indicates whether the tab is active.
    /// </summary>
    public bool IsTabActive => _page is { IsVisible: true };

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
        IsActive = false;
    }

    public override bool DragAndDrop_ShouldStartDrag() => _tabControl?.AllowReorder ?? false;

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawTabButton(this, IsTabActive, _tabControl.TabStrip.Dock);
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
        var colorSource = IsTabActive ? Skin.Colors.Tab.Active : Skin.Colors.Tab.Inactive;

        var textColor = GetTextColor(ComponentState.Normal) ?? colorSource.Normal;
        if (IsDisabledByTree)
        {
            textColor = GetTextColor(ComponentState.Disabled) ?? colorSource.Disabled;
        }
        else if (IsActive)
        {
            textColor = GetTextColor(ComponentState.Active) ?? colorSource.Active;
        }
        else if (IsHovered)
        {
            textColor = GetTextColor(ComponentState.Hovered) ?? colorSource.Hover;
        }

        // ApplicationContext.CurrentContext.Logger.LogInformation(
        //     "'{ComponentName}' IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered} TextColor={TextColor}",
        //     CanonicalName,
        //     IsDisabled,
        //     IsActive,
        //     IsHovered,
        //     textColor
        // );

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (textColor == null)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Text color for the current control state of {ComponentType} '{ComponentName}' is somehow null IsDisabled={IsDisabled} IsActive={IsActive} IsHovered={IsHovered}",
                GetType().GetName(qualified: true),
                CanonicalName,
                IsDisabled,
                IsActive,
                IsHovered
            );

            textColor = new Color(r: 255, g: 0, b: 255);
        }

        TextColor = textColor;
    }

    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        base.OnChildBoundsChanged(child, oldChildBounds, newChildBounds);
    }

    protected override void OnTextExceedsSize(Point ownSize, Point textSize)
    {
        base.OnTextExceedsSize(ownSize, textSize);

        Invalidate(alsoInvalidateParent: true);
    }
}
