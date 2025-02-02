﻿using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Tree control node.
/// </summary>
public partial class TreeNode : Base
{

    public const int TREE_INDENTATION = 14;

    private bool mRoot;

    private bool mSelectable;

    private bool mSelected;

    protected Button? _label;
    private GameFont? _font;

    protected Button mToggleButton;

    protected TreeControl mTreeControl;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeNode" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public TreeNode(Base parent, string? name = default) : base(parent: parent, name: name)
    {
        mToggleButton = new TreeToggleButton(this);
        mToggleButton.SetBounds(0, 0, 15, 15);
        mToggleButton.Toggled += OnToggleButtonPress;

        _label = new TreeNodeLabel(this);
        _label.Dock = Pos.Top;
        _label.Margin = new Margin(16, 0, 0, 0);
        _label.DoubleClicked += OnDoubleClickName;
        _label.Clicked += OnClickName;

        _innerPanel = new Base(this);
        _innerPanel.Dock = Pos.Top;
        _innerPanel.Height = 100;
        _innerPanel.Margin = new Margin(TREE_INDENTATION, 1, 0, 0);
        _innerPanel.Hide();

        mRoot = parent is TreeControl;
        mSelected = false;
        mSelectable = true;

        Dock = Pos.Top;
    }

    public virtual GameFont? Font
    {
        get => _label?.Font ?? _font;
        set
        {
            _font = value;

            if (_label is {} label)
            {
                label.Font = value;
            }

            foreach (var node in Children.OfType<TreeNode>())
            {
                node.Font = value;
            }
        }
    }

    /// <summary>
    ///     Indicates whether this is a root node.
    /// </summary>
    public bool IsRoot
    {
        get => mRoot;
        set => mRoot = value;
    }

    /// <summary>
    ///     Parent tree control.
    /// </summary>
    public TreeControl TreeControl
    {
        get => mTreeControl;
        set => mTreeControl = value;
    }

    /// <summary>
    ///     Determines whether the node is selectable.
    /// </summary>
    public bool IsSelectable
    {
        get => mSelectable;
        set => mSelectable = value;
    }

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

            if (_label != null)
            {
                _label.ToggleState = value;
            }

            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, EventArgs.Empty);
            }

            // propagate to root parent (tree)
            if (mTreeControl != null && mTreeControl.SelectionChanged != null)
            {
                mTreeControl.SelectionChanged.Invoke(this, EventArgs.Empty);
            }

            if (value)
            {
                if (Selected != null)
                {
                    Selected.Invoke(this, EventArgs.Empty);
                }

                if (mTreeControl != null && mTreeControl.Selected != null)
                {
                    mTreeControl.Selected.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (Unselected != null)
                {
                    Unselected.Invoke(this, EventArgs.Empty);
                }

                if (mTreeControl != null && mTreeControl.Unselected != null)
                {
                    mTreeControl.Unselected.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///     Node's label.
    /// </summary>
    public string? Text
    {
        get => _label?.Text;
        set
        {
            if (_label is { } label)
            {
                label.Text = value;
            }
        }
    }

    private Color? _textColor;

    public Color? TextColor
    {
        get => _label?.TextColor ?? _textColor;
        set
        {
            _textColor = value;
            if (_label is { } label)
            {
                label.TextColor = value;
            }
        }
    }

    private Color? _textColorOverride;

    public Color? TextColorOverride
    {
        get => _label?.TextColorOverride ?? _textColorOverride;
        set
        {
            _textColorOverride = value;
            if (_label is { } label)
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

            if (this.IsSelected)
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
    /// Renders the control using the specified skin.
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
            _label?.Height ?? treeNodeHeight,
            _label?.TextRight ?? 0,
            (int)(mToggleButton.Y + mToggleButton.Height * 0.5f),
            treeNodeHeight,
            mTreeControl == Parent
        );

        // Invalidate the tree node.
        this.Invalidate();
    }

    /// <summary>
    /// Calculates the height of tree node.
    /// </summary>
    private int CalculateTreeNodeHeight(bool isOpen)
    {
        if (_label is not { } label)
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
        if (mToggleButton != null)
        {
            if (_label != null)
            {
                mToggleButton.SetPosition(0, (_label.Height - mToggleButton.Height) * 0.5f);
            }

            if (_innerPanel.Children.Count == 0)
            {
                mToggleButton.Hide();
                mToggleButton.ToggleState = false;
                _innerPanel.Hide();
            }
            else
            {
                mToggleButton.Show();
                _innerPanel.SizeToChildren(false, true);
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
        if (SizeToChildren(false, true))
        {
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Adds a new child node.
    /// </summary>
    /// <param name="label">Node's label.</param>
    /// <returns>Newly created control.</returns>
    public TreeNode AddNode(string label)
    {
        TreeNode node = new(this)
        {
            Font = Font,
            Text = label,
            TextColor = _textColor,
            TextColorOverride = _textColorOverride,
        };

        return node;
    }

    /// <summary>
    ///     Opens the node.
    /// </summary>
    public void Open()
    {
        _innerPanel.Show();
        if (mToggleButton != null)
        {
            mToggleButton.ToggleState = true;
        }

        if (Expanded != null)
        {
            Expanded.Invoke(this, EventArgs.Empty);
        }

        if (mTreeControl != null && mTreeControl.Expanded != null)
        {
            mTreeControl.Expanded.Invoke(this, EventArgs.Empty);
        }

        Invalidate();
    }

    /// <summary>
    ///     Closes the node.
    /// </summary>
    public void Close()
    {
        _innerPanel.Hide();
        if (mToggleButton != null)
        {
            mToggleButton.ToggleState = false;
        }

        if (Collapsed != null)
        {
            Collapsed.Invoke(this, EventArgs.Empty);
        }

        if (mTreeControl != null && mTreeControl.Collapsed != null)
        {
            mTreeControl.Collapsed.Invoke(this, EventArgs.Empty);
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
        if (_label != null)
        {
            _label.ToggleState = false;
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
        if (mToggleButton.ToggleState)
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
        if (!mToggleButton.IsVisible)
        {
            return;
        }

        mToggleButton.Toggle();
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
        _label.SetImage(texture, fileName, Button.ControlState.Normal);
    }

    protected override void OnChildAdded(Base child)
    {
        var node = child as TreeNode;
        if (node != null)
        {
            node.TreeControl = mTreeControl;

            if (mTreeControl != null)
            {
                mTreeControl.OnNodeAdded(node);
            }
        }

        base.OnChildAdded(child);
    }

    public override event GwenEventHandler<ClickedEventArgs> Clicked
    {
        add { _label.Clicked += delegate(Base sender, ClickedEventArgs args) { value(this, args); }; }
        remove { _label.Clicked -= delegate(Base sender, ClickedEventArgs args) { value(this, args); }; }
    }

    public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
    {
        add
        {
            if (value != null)
            {
                _label.DoubleClicked += delegate(Base sender, ClickedEventArgs args) { value(this, args); };
            }
        }
        remove { _label.DoubleClicked -= delegate(Base sender, ClickedEventArgs args) { value(this, args); }; }
    }

    public override event GwenEventHandler<ClickedEventArgs> RightClicked
    {
        add { _label.RightClicked += delegate(Base sender, ClickedEventArgs args) { value(this, args); }; }
        remove { _label.RightClicked -= delegate(Base sender, ClickedEventArgs args) { value(this, args); }; }
    }

    public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
    {
        add
        {
            if (value != null)
            {
                _label.DoubleRightClicked += delegate(Base sender, ClickedEventArgs args) { value(this, args); };
            }
        }
        remove
        {
            _label.DoubleRightClicked -= delegate(Base sender, ClickedEventArgs args) { value(this, args); };
        }
    }

}
