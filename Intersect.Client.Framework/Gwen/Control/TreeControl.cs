using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Tree control.
/// </summary>
public partial class TreeControl : TreeNode
{
    private readonly ScrollControl _scrollControl;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeControl" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TreeControl(Base parent, string? name = default) : base(parent, name)
    {
        _treeControl = this;

        RemoveChild(_toggleButton, true);
        RemoveChild(_trigger, true);
        RemoveChild(_innerPanel, true);
        _toggleButton = null;
        _trigger = null;
        _innerPanel = null;

        AllowMultiSelect = false;

        _scrollControl = new ScrollControl(this, nameof(_innerPanel))
        {
            Dock = Pos.Fill, Margin = Margin.One,
        };

        _innerPanel = _scrollControl;

        _scrollControl.SetInnerSize(1000, 1000); // todo: why such arbitrary numbers?

        Dock = Pos.None;
    }

    /// <summary>
    ///     Determines if multiple nodes can be selected at the same time.
    /// </summary>
    public bool AllowMultiSelect { get; set; }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        if (ShouldDrawBackground)
        {
            skin.DrawTreeControl(this);
        }
    }

    /// <summary>
    ///     Handler invoked when control children's bounds change.
    /// </summary>
    /// <param name="child"></param>
    /// <param name="oldChildBounds"></param>
    /// <param name="newChildBounds"></param>
    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        if (_scrollControl != null)
        {
            _scrollControl.UpdateScrollBars();
        }
    }

    /// <summary>
    ///     Removes all child nodes.
    /// </summary>
    public virtual void RemoveAll()
    {
        _scrollControl.DeleteAll();
    }

    /// <summary>
    ///     Handler for node added event.
    /// </summary>
    /// <param name="node">Node added.</param>
    public virtual void OnNodeAdded(TreeNode node)
    {
        node.LabelPressed += OnNodeSelected;
    }

    /// <summary>
    ///     Handler for node selected event.
    /// </summary>
    /// <param name="control">Node selected.</param>
    protected virtual void OnNodeSelected(Base control, EventArgs args)
    {
        if (!AllowMultiSelect /*|| InputHandler.InputHandler.IsKeyDown(Key.Control)*/)
        {
            UnselectAll();
        }
    }
}