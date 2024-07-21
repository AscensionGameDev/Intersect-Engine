using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Interface.Game.DescriptionWindows.Components;

namespace Intersect.Client.Interface.Game.DescriptionWindows;

public partial class DescriptionWindowBase : ComponentBase
{
    // Track current Y height for placing components.
    private int mComponentY = 0;

    // Our internal list of components.
    private List<ComponentBase> mComponents;

    public DescriptionWindowBase(Base parent, string name) : base(parent, name)
    {
        // Set up our internal component list we use for re-ordering.
        mComponents = new List<ComponentBase>();

        GenerateComponents();
    }

    protected override void GenerateComponents()
    {
        base.GenerateComponents();

        // Load layout prior to adding components so we can retrieve padding information.
        LoadLayout();
    }

    /// <summary>
    /// Adds a <see cref="HeaderComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <param name="loadLayout">Should we load the layour json file automatically?</param>
    /// <returns>Returns a new instance of the <see cref="HeaderComponent"/> class</returns>
    public HeaderComponent AddHeader(string name = "DescriptionWindowHeader", bool loadLayout = true)
    {
        var component = new HeaderComponent(mContainer, name);
        if (loadLayout)
        {
            component.LoadLayout();
        }
        mComponents.Add(component);
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
        var component = new DividerComponent(mContainer, name);
        if (loadLayout)
        {
            component.LoadLayout();
        }
        mComponents.Add(component);
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
        var component = new DescriptionComponent(mContainer, name);
        if (loadLayout)
        {
            component.LoadLayout();
        }
        mComponents.Add(component);
        return component;
    }

    /// <summary>
    /// Adds a <see cref="RowContainerComponent"/> to the current window.
    /// </summary>
    /// <param name="name">The name of the component.</param>
    /// <returns>Returns a new instance of the <see cref="DescriptionComponent"/> class</returns>
    public RowContainerComponent AddRowContainer(string name = "DescriptionWindowRowContainer")
    {
        var component = new RowContainerComponent(mContainer, name);
        mComponents.Add(component);
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

        component.SetPosition(component.X, mComponentY);
        mComponentY += component.Height;
    }

    /// <summary>
    /// Position and resize all components properly for display.
    /// </summary>
    protected void FinalizeWindow()
    {
        // Reset our componentY so we start from scratch!
        mComponentY = 0;

        // Correctly set our container width to the largest child start with, this way our other child components will have a width to work with.
        mContainer.SizeToChildren(true, false);

        // Resize and relocate our components to properly display on our window.
        foreach (var component in mComponents)
        {
            component.CorrectWidth();
            PositionComponent(component);
        }

        // Correctly set our container height so we display everything.
        mContainer.SizeToChildren(false, true);
    }

    /// <inheritdoc/>
    public override void SetPosition(int x, int y, ImagePanel? itemDecriptionContainer = null)
    {
        if (mContainer == null || mContainer.Canvas == null)
        {
            return;
        }

        int newX, newY;
        int HoveredControlX, HoveredControlY;

        // Bind description window to the HoveredControl position.
        HoveredControlX = InputHandler.HoveredControl.LocalPosToCanvas(new Point(0, 0)).X;
        HoveredControlY = InputHandler.HoveredControl.LocalPosToCanvas(new Point(0, 0)).Y;
        newX = HoveredControlX + InputHandler.HoveredControl.Width;
        newY = itemDecriptionContainer != null ? itemDecriptionContainer.Bottom : HoveredControlY + InputHandler.HoveredControl.Height;

        // Do not allow it to render outside of the screen canvas.
        if (newX > mContainer.Canvas.Width - mContainer.Width)
        {
            newX = HoveredControlX - mContainer.Width;
        }

        if (newY > mContainer.Canvas.Height - mContainer.Height)
        {
            newY = itemDecriptionContainer != null ? itemDecriptionContainer.Y - mContainer.Height : HoveredControlY - mContainer.Height;
        }

        mContainer.MoveTo(newX, newY);
    }

    public override void SetPosition(Base _icon, SpellDescriptionWindow _descriptionWindow)
    {
        var X = _icon.LocalPosToCanvas(new Point(0, 0)).X;
        var Y = _icon.LocalPosToCanvas(new Point(0, 0)).Y;

        X = X + _descriptionWindow.Width + _icon.Height;
        Y = Y + _icon.Height;

        // Do not allow it to render outside of the screen canvas while based on the _icon position.
        // Prevents flickering when _icon is hovered near the edge of the screen.
        if (X > Interface.GameUi.GameCanvas.Width - _descriptionWindow.Width)
        {
            X = X - _descriptionWindow.Width - _icon.Height;
        }

        if (Y > Interface.GameUi.GameCanvas.Height - _descriptionWindow.Height)
        {
            Y = Y - _descriptionWindow.Height - _icon.Height;
        }

        _descriptionWindow.SetPosition(X, Y);
    }
}
