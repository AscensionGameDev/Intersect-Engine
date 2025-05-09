using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows;

public partial class DescriptionWindowBase : ComponentBase
{
    // Track current Y height for placing components.
    private int _componentY = 0;

    // Our internal list of components.
    private readonly List<ComponentBase> _components = [];

    public DescriptionWindowBase(Base parent, string name) : base(parent, name)
    {
        LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    protected override void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {
        base.OnVisibilityChanged(sender, eventArgs);

        if (!eventArgs.IsVisibleInTree)
        {
            ClearChildren();
        }
    }

    protected override void OnPreDraw(Framework.Gwen.Skin.Base skin)
    {
        base.OnPreDraw(skin);

        if (!IsOnTop)
        {
            BringToFront();
        }
    }

    /// <summary>
    /// Adds a <see cref="HeaderComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="loadLayout">Should we load the layour json file automatically?</param>
    /// <returns>Returns a new instance of the <see cref="HeaderComponent"/> class</returns>
    public HeaderComponent AddHeader(string name = "DescriptionWindowHeader", bool loadLayout = true)
    {
        var component = new HeaderComponent(this, name);
        if (loadLayout)
        {
            component.LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a <see cref="DividerComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="loadLayout">Should we load the layour json file automatically?</param>
    /// <returns>Returns a new instance of the <see cref="DividerComponent"/> class</returns>
    public DividerComponent AddDivider(string name = "DescriptionWindowDivider", bool loadLayout = true)
    {
        var component = new DividerComponent(this, name);
        if (loadLayout)
        {
            component.LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a <see cref="DescriptionComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="loadLayout">Should we load the layour json file automatically?</param>
    /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
    public DescriptionComponent AddDescription(string name = "DescriptionWindowDescription", bool loadLayout = true)
    {
        var component = new DescriptionComponent(this, name);
        if (loadLayout)
        {
            component.LoadJsonUi(Framework.File_Management.GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a <see cref="RowContainerComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
    public RowContainerComponent AddRowContainer(string name = "DescriptionWindowRowContainer")
    {
        var component = new RowContainerComponent(this, name);
        _components.Add(component);
        return component;
    }

    /// <summary>
    /// Positions a component correctly on the current window.
    /// </summary>
    /// <param name="component">The <see cref="ComponentBase"/> to place.</param>
    private void PositionComponent(ComponentBase component)
    {
        if (component == null)
        {
            return;
        }

        component.SetPosition(component.X, _componentY);
        _componentY += component.Height;
    }

    /// <summary>
    /// Position and resize all components properly for display.
    /// </summary>
    protected void FinalizeWindow()
    {
        // Reset our componentY so we start from scratch!
        _componentY = 0;

        // Correctly set our container width to the largest child start with, this way our other child components will have a width to work with.
        SizeToChildren(true, false);

        // Resize and relocate our components to properly display on our window.
        foreach (var component in _components)
        {
            component.CorrectWidth();
            PositionComponent(component);
        }

        // Correctly set our container height so we display everything.
        SizeToChildren(false, true);
    }

    /// <inheritdoc/>
    public void PositionToHoveredControl()
    {
        if (Canvas == default)
        {
            return;
        }

        if (InputHandler.HoveredControl == default)
        {
            return;
        }

        int newX, newY;
        var hoveredPos = InputHandler.HoveredControl.ToCanvas(new Point(0, 0));
        int HoveredControlX = hoveredPos.X;
        int HoveredControlY = hoveredPos.Y;

        // Bind description window to the right, bottom of HoveredControl.
        newX = HoveredControlX + InputHandler.HoveredControl.Width;
        newY = HoveredControlY + InputHandler.HoveredControl.Height;

        // Do not allow it to render outside of the screen canvas.
        if (newX > Canvas.Width - Width)
        {
            newX = HoveredControlX - Width;
        }

        if (newY > Canvas.Height - Height)
        {
            newY = HoveredControlY - Height;
        }

        MoveTo(newX, newY);
    }
}
