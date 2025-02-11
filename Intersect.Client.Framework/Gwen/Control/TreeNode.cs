using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Tree control node.
/// </summary>
public partial class TreeNode : Base
{
    public const int TREE_INDENTATION = 14;

    private readonly Dictionary<GwenEventHandler<MouseButtonState>, GwenEventHandler<MouseButtonState>>
        _wrappedMouseButtonStateDelegates = [];

    private GameFont? _font;

    private Color? _textColor;

    private Color? _textColorOverride;

    private bool mSelected;

    protected Button? _toggleButton { get; set; }

    protected TreeControl _treeControl;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeNode" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TreeNode(Base parent, string? name = default) : base(parent, name)
    {
        _toggleButton = new TreeToggleButton(this)
        {
            Size = new Point(15, 15),
        };
        _toggleButton.Toggled += OnToggleButtonPress;

        _trigger = new TreeNodeLabel(this)
        {
            Dock = Pos.Top,
            Margin = new Margin(16, 0, 0, 0),
        };
        _trigger.DoubleClicked += OnDoubleClickName;
        _trigger.Clicked += OnClickName;

        _innerPanel = new Base(this, nameof(_innerPanel))
        {
            Dock = Pos.Top,
            Height = 100,
            IsVisible = false,
            Margin = new Margin(TREE_INDENTATION, 1, 0, 0),
        };

        IsRoot = parent is TreeControl;
        mSelected = false;
        IsSelectable = true;

        Dock = Pos.Top;
    }

    protected Button? _trigger { get; set; }

    public virtual GameFont? Font
    {
        get => _trigger?.Font ?? _font;
        set
        {
            _font = value;

            if (_trigger is { } label)
            {
                label.Font = value;
            }

            var treeNodes = Children.OfType<TreeNode>().ToArray();
            foreach (var node in treeNodes)
            {
                node.Font = value;
            }
        }
    }

    /// <summary>
    ///     Indicates whether this is a root node.
    /// </summary>
    public bool IsRoot { get; set; }

    /// <summary>
    ///     Parent tree control.
    /// </summary>
    public TreeControl TreeControl
    {
        get => _treeControl;
        set => _treeControl = value;
    }

    /// <summary>
    ///     Determines whether the node is selectable.
    /// </summary>
    public bool IsSelectable { get; set; }

    /// <summary>
    ///     Indicates whether the node is selected.
    /// </summary>
    public bool IsSelected
    {
        get => mSelected;
        set
        {
            if (!IsSelectable)
            {
                return;
            }

            if (IsSelected == value)
            {
                return;
            }

            mSelected = value;

            if (_trigger != null)
            {
                _trigger.ToggleState = value;
            }

            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, EventArgs.Empty);
            }

            // propagate to root parent (tree)
            if (_treeControl != null && _treeControl.SelectionChanged != null)
            {
                _treeControl.SelectionChanged.Invoke(this, EventArgs.Empty);
            }

            if (value)
            {
                if (Selected != null)
                {
                    Selected.Invoke(this, EventArgs.Empty);
                }

                if (_treeControl != null && _treeControl.Selected != null)
                {
                    _treeControl.Selected.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (Unselected != null)
                {
                    Unselected.Invoke(this, EventArgs.Empty);
                }

                if (_treeControl != null && _treeControl.Unselected != null)
                {
                    _treeControl.Unselected.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///     Node's label.
    /// </summary>
    public string? Text
    {
        get => _trigger?.Text;
        set
        {
            if (_trigger is { } label)
            {
                label.Text = value;
            }
        }
    }

    public Color? TextColor
    {
        get => _trigger?.TextColor ?? _textColor;
        set
        {
            _textColor = value;
            if (_trigger is { } label)
            {
                label.TextColor = value;
            }
        }
    }

    public Color? TextColorOverride
    {
        get => _trigger?.TextColorOverride ?? _textColorOverride;
        set
        {
            _textColorOverride = value;
            if (_trigger is { } label)
            {
                label.TextColorOverride = value;
            }
        }
    }

    public IEnumerable<TreeNode> SelectedChildren
    {
        get
        {
            List<TreeNode> selectedChildren = [];

            foreach (var child in Children)
            {
                if (child is not TreeNode node)
                {
                    continue;
                }

                selectedChildren.AddRange(node.SelectedChildren);
            }

            if (IsSelected)
            {
                selectedChildren.Add(this);
            }

            return selectedChildren;
        }
    }

    /// <summary>
    ///     Invoked when the node label has been pressed.
    /// </summary>
    public event GwenEventHandler<EventArgs> LabelPressed;

    /// <summary>
    ///     Invoked when the node's selected state has changed.
    /// </summary>
    public event GwenEventHandler<EventArgs> SelectionChanged;

    /// <summary>
    ///     Invoked when the node has been selected.
    /// </summary>
    public event GwenEventHandler<EventArgs> Selected;

    /// <summary>
    ///     Invoked when the node has been unselected.
    /// </summary>
    public event GwenEventHandler<EventArgs> Unselected;

    /// <summary>
    ///     Invoked when the node has been expanded.
    /// </summary>
    public event GwenEventHandler<EventArgs> Expanded;

    /// <summary>
    ///     Invoked when the node has been collapsed.
    /// </summary>
    public event GwenEventHandler<EventArgs> Collapsed;

    /// <summary>
    ///     Renders the control using the specified skin.
    /// </summary>
    /// <param name="skin">The skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        // Calculate the height of the tree node
        var isOpen = _innerPanel?.IsVisible ?? false;
        var treeNodeHeight = CalculateTreeNodeHeight(isOpen);

        // Draw the tree node using the specified skin.
        skin.DrawTreeNode(
            this,
            isOpen,
            IsSelected,
            treeNodeHeight,
            _trigger?.Height ?? treeNodeHeight,
            _trigger?.TextRight ?? 0,
            (int)(_toggleButton.Y + (_toggleButton.Height * 0.5f)),
            treeNodeHeight,
            _treeControl == Parent
        );

        // Invalidate the tree node.
        Invalidate();
    }

    /// <summary>
    ///     Calculates the height of tree node.
    /// </summary>
    private int CalculateTreeNodeHeight(bool isOpen)
    {
        if (_trigger is not { } label)
        {
            return 0;
        }

        var height = label.Height;

        if (_innerPanel is not { } innerPanel)
        {
            return height;
        }

        // ReSharper disable once InvertIf
        if (isOpen)
        {
            if (innerPanel.Children.OfType<TreeNode>().LastOrDefault(child => child.IsVisible) is { } lastVisibleChild)
            {
                return height + lastVisibleChild.Y;
            }
        }

        return height == 0 ? innerPanel.Height : height;
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        if (_toggleButton != null)
        {
            if (_trigger != null)
            {
                _toggleButton.SetPosition(0, (_trigger.Height - _toggleButton.Height) * 0.5f);
            }

            if (_innerPanel.Children.Count == 0)
            {
                _toggleButton.Hide();
                _toggleButton.ToggleState = false;
                _innerPanel.Hide();
            }
            else
            {
                _toggleButton.Show();
                _innerPanel.SizeToChildren(false);
            }
        }

        base.Layout(skin);
    }

    /// <summary>
    ///     Function invoked after layout.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void PostLayout(Skin.Base skin)
    {
        if (SizeToChildren(false))
        {
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Adds a new child node.
    /// </summary>
    /// <param name="label">Node's label.</param>
    /// <param name="userData"></param>
    /// <returns>Newly created control.</returns>
    public TreeNode AddNode(string label, object? userData = null)
    {
        TreeNode node = new(this)
        {
            Font = Font,
            Text = label,
            TextColor = _textColor,
            TextColorOverride = _textColorOverride,
            UserData = userData,
        };

        return node;
    }

    /// <summary>
    ///     Opens the node.
    /// </summary>
    public void Open()
    {
        _innerPanel.Show();
        if (_toggleButton != null)
        {
            _toggleButton.ToggleState = true;
        }

        if (Expanded != null)
        {
            Expanded.Invoke(this, EventArgs.Empty);
        }

        if (_treeControl != null && _treeControl.Expanded != null)
        {
            _treeControl.Expanded.Invoke(this, EventArgs.Empty);
        }

        Invalidate();
    }

    /// <summary>
    ///     Closes the node.
    /// </summary>
    public void Close()
    {
        _innerPanel.Hide();
        if (_toggleButton != null)
        {
            _toggleButton.ToggleState = false;
        }

        if (Collapsed != null)
        {
            Collapsed.Invoke(this, EventArgs.Empty);
        }

        if (_treeControl != null && _treeControl.Collapsed != null)
        {
            _treeControl.Collapsed.Invoke(this, EventArgs.Empty);
        }

        Invalidate();
    }

    /// <summary>
    ///     Opens the node and all child nodes.
    /// </summary>
    public void ExpandAll()
    {
        Open();
        foreach (var child in Children)
        {
            var node = child as TreeNode;
            if (node == null)
            {
                continue;
            }

            node.ExpandAll();
        }
    }

    /// <summary>
    ///     Clears the selection on the node and all child nodes.
    /// </summary>
    public void UnselectAll()
    {
        IsSelected = false;
        if (_trigger != null)
        {
            _trigger.ToggleState = false;
        }

        foreach (var child in Children)
        {
            var node = child as TreeNode;
            if (node == null)
            {
                continue;
            }

            node.UnselectAll();
        }
    }

    /// <summary>
    ///     Handler for the toggle button.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void OnToggleButtonPress(Base control, EventArgs args)
    {
        if (_toggleButton.ToggleState)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    ///     Handler for label double click.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void OnDoubleClickName(Base control, EventArgs args)
    {
        if (!_toggleButton.IsVisible)
        {
            return;
        }

        _toggleButton.Toggle();
    }

    /// <summary>
    ///     Handler for label click.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void OnClickName(Base control, EventArgs args)
    {
        if (LabelPressed != null)
        {
            LabelPressed.Invoke(this, EventArgs.Empty);
        }

        IsSelected = !IsSelected;
    }

    public void SetImage(GameTexture texture, string fileName = "")
    {
        _trigger.SetStateTexture(texture, fileName, ComponentState.Normal);
    }

    protected override void OnChildAdded(Base child)
    {
        var node = child as TreeNode;
        if (node != null)
        {
            node.TreeControl = _treeControl;

            if (_treeControl != null)
            {
                _treeControl.OnNodeAdded(node);
            }
        }

        base.OnChildAdded(child);
    }

    public override event GwenEventHandler<MouseButtonState>? Clicked
    {
        add
        {
            if (_trigger is not { } label)
            {
                base.Clicked += value;
                return;
            }

            if (value == null)
            {
                label.Clicked += value;
                return;
            }

            GwenEventHandler<MouseButtonState> wrappedDelegate = delegate(Base _, MouseButtonState args)
            {
                value.Invoke(this, args);
            };
            _wrappedMouseButtonStateDelegates[value] = wrappedDelegate;
            label.Clicked += wrappedDelegate;
        }
        remove
        {
            if (_trigger is not { } label)
            {
                base.Clicked -= value;
                return;
            }

            if (value == null)
            {
                label.Clicked -= value;
                return;
            }

            if (_wrappedMouseButtonStateDelegates.Remove(value, out var wrappedDelegate))
            {
                label.Clicked -= wrappedDelegate;
            }
        }
    }

    public override event GwenEventHandler<MouseButtonState>? DoubleClicked
    {
        add
        {
            if (_trigger is not { } label)
            {
                base.DoubleClicked += value;
                return;
            }

            if (value == null)
            {
                label.DoubleClicked += value;
                return;
            }

            GwenEventHandler<MouseButtonState> wrappedDelegate = delegate(Base _, MouseButtonState args)
            {
                value.Invoke(this, args);
            };
            _wrappedMouseButtonStateDelegates[value] = wrappedDelegate;
            label.DoubleClicked += wrappedDelegate;
        }
        remove
        {
            if (_trigger is not { } label)
            {
                base.DoubleClicked -= value;
                return;
            }

            if (value == null)
            {
                label.DoubleClicked -= value;
                return;
            }

            if (_wrappedMouseButtonStateDelegates.Remove(value, out var wrappedDelegate))
            {
                label.DoubleClicked -= wrappedDelegate;
            }
        }
    }
}