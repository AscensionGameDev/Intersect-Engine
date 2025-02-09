using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Tree control.
/// </summary>
public partial class TreeControl : TreeNode
{

    private readonly ScrollControl _scrollControl;

    private bool mMultiSelect;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeControl" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TreeControl(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        mTreeControl = this;

        RemoveChild(mToggleButton, true);
        mToggleButton = null;
        RemoveChild(_label, true);
        _label = null;
        RemoveChild(_innerPanel, true);
        _innerPanel = null;

        mMultiSelect = false;

        _scrollControl = new ScrollControl(this, name: nameof(_innerPanel))
        {
            Dock = Pos.Fill,
            Margin = Margin.One,
        };

        _innerPanel = _scrollControl;

        _scrollControl.SetInnerSize(1000, 1000); // todo: why such arbitrary numbers?

        Dock = Pos.None;
    }

    /// <summary>
    ///     Determines if multiple nodes can be selected at the same time.
    /// </summary>
    public bool AllowMultiSelect
    {
        get => mMultiSelect;
        set => mMultiSelect = value;
    }

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
        if (!mMultiSelect /*|| InputHandler.InputHandler.IsKeyDown(Key.Control)*/)
        {
            UnselectAll();
        }
    }

}
