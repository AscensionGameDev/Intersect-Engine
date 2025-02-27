using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Anim;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Control.Layout;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
#if DEBUG || DIAGNOSTIC
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Intersect.Client.Framework.Gwen.Renderer;
using Intersect.Client.Framework.Input;
using Intersect.Core;
using Intersect.Framework;
using Intersect.Framework.Collections;
using Intersect.Framework.Reflection;
using Intersect.Framework.Serialization;
using Intersect.Framework.Threading;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;

public record struct NodePair(Base This, Base? Parent);

/// <summary>
///     Base control class.
/// </summary>
public partial class Base : IDisposable
{
    public delegate void GwenEventHandler<in TArgs>(Base sender, TArgs arguments) where TArgs : EventArgs;

    public delegate void GwenEventHandler<in TSender, in TArgs>(TSender sender, TArgs arguments)
        where TSender : Base where TArgs : EventArgs;

    private const string PropertyNameInnerPanel = "InnerPanel";

    private bool _inheritParentEnablementProperties;

    internal bool InheritParentEnablementProperties
    {
        get => _inheritParentEnablementProperties;
        set
        {
            _inheritParentEnablementProperties = value;
            IsDisabled = Parent?.IsDisabled ?? IsDisabled;
            IsHidden = Parent?.IsHidden ?? IsHidden;
        }
    }

    private bool _disposed;
    private bool _disposeCompleted;
    private StackTrace? _disposeStack;

    private Canvas? _canvas;

    protected Modal? _modal;
    private Base? _previousParent;
    private Base? _parent;
    private Base? _host { get; set; }
    protected Base? _innerPanel;
    private readonly List<Base> _children = [];

    private bool _disabled;
    private bool _skipRender;
    private bool _visible;

    private Package? _dragPayload;
    private object? _userData;

    private bool _drawDebugOutlines;

    private readonly Dictionary<string, GwenEventHandler<EventArgs>> _accelerators = [];
    private bool _keyboardInputEnabled;
    private bool _mouseInputEnabled;
    private bool _tabEnabled;

    private Rectangle _bounds;
    private Rectangle _boundsOnDisk;
    private Rectangle _innerBounds;
    private Rectangle _outerBounds;
    private Rectangle _renderBounds;

    private Point _maximumSize;
    private Point _minimumSize;
    private bool _restrictToParent;

    private Margin _margin;
    private Padding _padding;

    private List<Alignments> _alignments = [];
    private Padding _alignmentPadding;
    private Point _alignmentTranslation;

    private Pos _dock;
    private Padding _dockChildSpacing;

    private string? _name;
    private string? _cachedToString;

    private bool _cacheTextureDirty;
    private bool _cacheToTexture;

    private bool _needsAlignment;
    private bool _layoutDirty;
    private bool _dockDirty;

    private bool _drawBackground;
    private Color _color;
    private Cursor _cursor;
    private Skin.Base? _skin;

    private Base? _tooltip;
    private bool _tooltipEnabled;
    private IGameTexture? _tooltipBackground { get; set; }
    private string? _tooltipBackgroundName;
    private IFont? _tooltipFont;
    private string? _tooltipFontInfo;
    private Color? _tooltipTextColor;

    protected virtual string InternalToString()
    {
        StringBuilder builder = new();

        builder.Append(GetType().GetName(qualified: false));

        var name = _name?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            builder.Append($"({name})");
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Base" /> class.
    /// </summary>
    /// <param name="parent">parent control</param>
    /// <param name="name">name of this control</param>
    public Base(Base? parent = default, string? name = default)
    {
        _threadQueue = new ThreadQueue();
        _threadQueue.QueueNotEmpty += () => UpdatePendingThreadQueues(1);
        _name = name ?? string.Empty;
        _visible = true;
        _dockDirty = true;
        _layoutDirty = true;
        _disabled = false;
        _tooltip = null;
        _tooltipEnabled = true;
        _cacheTextureDirty = true;
        _cacheToTexture = false;
        _drawBackground = true;

        _keyboardInputEnabled = false;
        _mouseInputEnabled = true;
        _tabEnabled = false;

        PreLayout = new ManualActionQueue(_preLayoutActionsParent);
        PostLayout = new ManualActionQueue(_postLayoutActionsParent);

        if (this is Canvas canvas)
        {
            _canvas = canvas;
        }

        Parent = parent;

        _bounds = new Rectangle(0, 0, 10, 10);
        _padding = Padding.Zero;
        _margin = Margin.Zero;
        _color = Color.White;
        _alignmentPadding = Padding.Zero;

        RestrictToParent = false;

        _cursor = Cursors.Default;

        BoundsOutlineColor = Color.Red;
        MarginOutlineColor = Color.Green;
        PaddingOutlineColor = Color.Blue;
    }

    public virtual string? TooltipFontName
    {
        get => _tooltipFont?.Name;
        set
        {
            if (value == TooltipFontName)
            {
                return;
            }

            TooltipFont = GameContentManager.Current.GetFont(value);
        }
    }

    private int _tooltipFontSize = 10;

    public virtual int TooltipFontSize
    {
        get => _tooltipFontSize;
        set
        {
            if (value == _tooltipFontSize)
            {
                return;
            }

            _tooltipFontSize = value;
            if (_tooltip is Label label)
            {
                label.FontSize = value;
            }
        }
    }

    public virtual IGameTexture? TooltipBackground
    {
        get => _tooltipBackground;
        set
        {
            if (value != _tooltipBackground)
            {
                return;
            }

            if (value?.Name == _tooltipBackgroundName)
            {
                _tooltipBackground = value;
                return;
            }

            _tooltipBackground = value;
            _tooltipBackgroundName = value?.Name;

            if (Tooltip is Label label)
            {
                label.ToolTipBackground = _tooltipBackground;
            }
        }
    }

    public virtual string? TooltipBackgroundName
    {
        get => _tooltipBackgroundName;
        set
        {
            if (value == _tooltipBackgroundName)
            {
                return;
            }

            _tooltipBackgroundName = value;
            IGameTexture? texture = null;
            if (!string.IsNullOrWhiteSpace(_tooltipBackgroundName))
            {
                texture = GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui,
                    _tooltipBackgroundName
                );
            }

            _tooltipBackground = texture;

            if (Tooltip is Label label)
            {
                label.ToolTipBackground = _tooltipBackground;
            }
        }
    }

    public virtual Color? TooltipTextColor
    {
        get => _tooltipTextColor;
        set
        {
            if (value == _tooltipTextColor)
            {
                return;
            }

            _tooltipTextColor = value;
            if (Tooltip is Label label)
            {
                label.TextColorOverride = _tooltipTextColor;
            }
        }
    }

    /// <summary>
    ///     Font.
    /// </summary>
    public IFont? TooltipFont
    {
        get => _tooltipFont;
        set
        {
            _tooltipFont = value;
            _tooltipFontInfo = value == null ? null : $"{value.Name},{_tooltipFontSize}";

            if (_tooltip is Label label)
            {
                label.Font = value;
            }
        }
    }

    public List<Alignments> CurAlignments => _alignments;

    /// <summary>
    ///     Returns true if any on click events are set.
    /// </summary>
    internal bool ClickEventAssigned => Clicked != null || DoubleClicked != null;

    /// <summary>
    ///     Logical list of children. If InnerPanel is not null, returns InnerPanel's children.
    /// </summary>
    public IReadOnlyList<Base> Children => _innerPanel?.Children ?? _children;

    /// <summary>
    ///     The logical parent. It's usually what you expect, the control you've parented it to.
    /// </summary>
    public Base? Parent
    {
        get => _host;
        set
        {
            if (ReferenceEquals(_host, value))
            {
                return;
            }

            if (_host is { } oldParent)
            {
                // Detach from the previous parent on their thread
                oldParent.RunOnMainThread(ProcessDetachingFromParent, new NodePair(this, oldParent));
            }

            if (value is null)
            {
                // If we aren't being attached to a new parent, run this immediately
                ProcessAttachingToParent(this, null);
            }
            else
            {
                // Otherwise, we should wait until it's run on the new parent's thread
                value.RunOnMainThread(ProcessAttachingToParent, new NodePair(this, value));
            }
        }
    }

    private static void ProcessDetachingFromParent(NodePair pair)
    {
        pair.This.NotifyDetachingFromRoot(pair.Parent!.Root);
        pair.This.NotifyDetaching(pair.Parent);
        pair.Parent.RemoveChild(pair.This, false);
    }

    private static void ProcessAttachingToParent(NodePair pair) => ProcessAttachingToParent(pair.This, pair.Parent);

    private static void ProcessAttachingToParent(Base @this, Base? parent)
    {
        if (parent?._threadQueue is { } parentThreadQueue)
        {
            @this._threadQueue.SetMainThreadId(parentThreadQueue);
        }

        @this.PropagateCanvas(parent?._canvas);

        @this._host = parent;
        @this._parent = default;

        if (parent is not { } newParent)
        {
            return;
        }

        parent.AddChild(@this);

        @this.NotifyAttachingToRoot(parent.Root);
        @this.NotifyAttaching(parent);
    }

    protected virtual void OnAttached(Base parent)
    {
    }

    protected virtual void OnAttaching(Base newParent)
    {
    }

    private void NotifyAttaching(Base parent)
    {
        try
        {
            parent.UpdatePendingThreadQueues(_pendingThreadQueues);
            OnAttaching(parent);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking OnAttaching() for {NodeName}",
                ParentQualifiedName
            );
        }
    }

    protected virtual void OnDetached()
    {
    }

    protected virtual void OnDetaching(Base oldParent)
    {
    }

    private void NotifyDetaching(Base parent)
    {
        try
        {
            parent.UpdatePendingThreadQueues(-_pendingThreadQueues);
            OnDetaching(parent);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking OnDetaching() for {NodeName}",
                ParentQualifiedName
            );
        }
    }

    protected virtual void OnDetachingFromRoot(Base root) {}

    private void NotifyDetachingFromRoot(Base root)
    {
        try
        {
            var visibleProviders = _updatableDataProviderRefCounts.Where(e => e.Value.Visible > 0)
                .Select(e => e.Key)
                .ToArray();
            root.VisibleUpdatableDataProviders(visibleProviders, false);
            root.ListenUpdatableDataProviders(_updatableDataProviderRefCounts.Keys, false);

            try
            {
                OnDetachingFromRoot(root);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Error occurred while invoking OnDetachingFromRoot() for {NodeName}",
                    ParentQualifiedName
                );
            }

            foreach (var child in _children)
            {
                child.NotifyDetachingFromRoot(root);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking NotifyDetachingFromRoot() for {NodeName} from {RootName}",
                ParentQualifiedName,
                root.ParentQualifiedName
            );
        }
    }

    protected virtual void OnAttachingToRoot(Base root) { }

    private void NotifyAttachingToRoot(Base root)
    {
        try
        {
            var visibleProviders = _updatableDataProviderRefCounts.Where(e => e.Value.Visible > 0)
                .Select(e => e.Key)
                .ToArray();
            Root.ListenUpdatableDataProviders(_updatableDataProviderRefCounts.Keys, true);
            Root.VisibleUpdatableDataProviders(visibleProviders, true);

            try
            {
                OnAttachingToRoot(root);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Error occurred while invoking OnAttachingToRoot() for {NodeName}",
                    ParentQualifiedName
                );
            }

            foreach (var child in _children)
            {
                child.NotifyAttachingToRoot(root);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking NotifyAttachingToRoot() for {NodeName} to {RootName}",
                ParentQualifiedName,
                root.ParentQualifiedName
            );
        }
    }

    private void PropagateCanvas(Canvas? canvas)
    {
        try
        {
            _canvas = canvas;

            foreach (var child in _children)
            {
                child.PropagateCanvas(canvas);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown while invoking PropagateCanvas() for {NodeName} with {CanvasName}",
                ParentQualifiedName,
                canvas?.ParentQualifiedName ?? "null"
            );
        }
    }

    public Base Root
    {
        get
        {
            var root = this;
            while (root.Parent != default)
            {
                root = root.Parent;
            }
            return root;
        }
    }

    // todo: ParentChanged event?

    public Alignments[] Alignment
    {
        get => _alignments.ToArray();
        set
        {
            _alignments = value.ToList();
            ProcessAlignments();
        }
    }

    public Point AlignmentTranslation
    {
        get => _alignmentTranslation;
        set => SetAndDoIfChanged(ref _alignmentTranslation, value, InvalidateAlignment);
    }

    public void InvalidateAlignment()
    {
        if (!_needsAlignment)
        {
            _needsAlignment = true;
        }

        Invalidate();
    }

    protected static void InvalidateAlignment(Base @this) => @this.InvalidateAlignment();

    public virtual bool IsBlockingInput => true;

    /// <summary>
    ///     Dock position.
    /// </summary>
    public Pos Dock
    {
        get => _dock;
        set
        {
            var oldDock = _dock;
            if (oldDock == value)
            {
                return;
            }

            _dock = value;
            OnDockChanged(oldDock, value);

            InvalidateDock();
            InvalidateParentDock();
        }
    }

    protected virtual void OnDockChanged(Pos oldDock, Pos newDock)
    {
    }

    public Padding DockChildSpacing
    {
        get => _dockChildSpacing;
        set
        {
            _dockChildSpacing = value;
            if (_innerPanel is { } innerPanel)
            {
                innerPanel.DockChildSpacing = value;
            }
        }
    }

    protected bool HasSkin => _skin != null || (_host?.HasSkin ?? false);

    protected Skin.Base? SafeSkin => _skin ?? _parent?.SafeSkin;

    /// <summary>
    ///     Current skin.
    /// </summary>
    public Skin.Base Skin
    {
        get
        {
            if (SafeSkin is { } skin)
            {
                return skin;
            }

            throw new InvalidOperationException("GetSkin: null");
        }
    }

    private Skin.Base? DisposeSkin => _skin ?? _host?.DisposeSkin;

    /// <summary>
    ///     Current tooltip.
    /// </summary>
    public Base? Tooltip
    {
        get => _tooltip;
        set
        {
            if (value == _tooltip)
            {
                return;
            }

            _tooltip = value;

            if (_tooltip == null)
            {
                return;
            }

            _tooltip.Parent = this;
            _tooltip.IsHidden = true;
        }
    }

    public virtual string? TooltipText
    {
        get => (_tooltip as Label)?.Text;
        set
        {
            if (value == TooltipText)
            {
                return;
            }

            if (_tooltip is not Label label)
            {
                if (_tooltip is not null)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(
                        "Unable to set tooltip text of {ControlName} to '{TooltipText}' because it is set to an incompatible control type {ControlType}",
                        ParentQualifiedName,
                        value,
                        _tooltip.GetType().GetName(qualified: true)
                    );
                    return;
                }

                SetToolTipText(value);
            }
            else
            {
                label.Text = value ?? string.Empty;
            }
        }
    }

    /// <summary>
    ///     Indicates whether this control is a menu component.
    /// </summary>
    internal virtual bool IsMenuComponent
    {
        get
        {
            if (_host == null)
            {
                return false;
            }

            return _host.IsMenuComponent;
        }
    }

    /// <summary>
    ///     Determines whether the control should be clipped to its bounds while rendering.
    /// </summary>
    protected virtual bool ShouldClip => true;

    /// <summary>
    ///     Current padding - inner spacing.
    /// </summary>
    public Padding Padding
    {
        get => _padding;
        set
        {
            if (_padding == value)
            {
                return;
            }

            _padding = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Alignment Distance - Minimum distance from parent edges when processing alignments.
    /// </summary>
    public Padding AlignmentPadding
    {
        get => _alignmentPadding;
        set => SetAndDoIfChanged(ref _alignmentPadding, value, InvalidateAlignment);
    }

    /// <summary>
    ///     Current padding - inner spacing.
    /// </summary>
    public Color RenderColor
    {
        get => _color;
        set
        {
            if (_color == value)
            {
                return;
            }

            _color = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Current margin - outer spacing.
    /// </summary>
    public Margin Margin
    {
        get => _margin;
        set
        {
            if (_margin == value)
            {
                return;
            }

            _margin = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Indicates whether the control is on top of its parent's children.
    /// </summary>
    public bool IsOnTop => Parent?.IndexOf(this) == (Parent?.Children.Count ?? -1);

    /// <summary>
    ///     User data associated with the control.
    /// </summary>
    public object? UserData
    {
        get => _userData;
        set => _userData = value;
    }

    /// <summary>
    ///     Indicates whether the control is hovered by mouse pointer.
    /// </summary>
    public virtual bool IsHovered => InputHandler.HoveredControl == this;

    /// <summary>
    ///     Indicates whether the control has focus.
    /// </summary>
    public bool HasFocus => InputHandler.KeyboardFocus == this;

    /// <summary>
    ///     Indicates whether the control is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => (_inheritParentEnablementProperties && Parent != default) ? Parent.IsDisabled : _disabled;
        set => SetAndDoIfChanged(ref _disabled, value, OnDisabledChanged);
    }

    protected virtual void OnDisabledChanged(bool oldValue, bool newValue)
    {
        Invalidate();
    }

    /// <summary>
    ///     Indicates whether the control is hidden.
    /// </summary>
    public bool IsHidden
    {
        get => ((_inheritParentEnablementProperties && Parent is {} parent) ? parent.IsHidden : !_visible);
        set => RunOnMainThread(SetVisible, !value);
    }

    private static void SetVisible(Base @this, bool value)
    {
        var wasVisibleInParent = @this._visible;

        if (@this is { _inheritParentEnablementProperties: true, Parent: { } parent })
        {
            if (value != parent._visible)
            {
                ApplicationContext.CurrentContext.Logger.LogTrace(
                    "Tried to change visibility of restricted node '{NodeName}' to {Visible} when the parent ({ParentNodeName}) is set to {ParentVisible}",
                    @this.CanonicalName,
                    value,
                    parent.CanonicalName,
                    !value
                );
            }

            value = parent._visible;
        }

        if (value == wasVisibleInParent)
        {
            // ApplicationContext.CurrentContext.Logger.LogTrace(
            //     "{ComponentTypeName} (\"{ComponentName}\") set to same visibility ({Visibility})",
            //     GetType().GetName(qualified: true),
            //     CanonicalName,
            //     !value
            // );

            // Skip the rest of this method since nothing actually changed for this node
            return;
        }

        var visibilityInTreeChanging = value != @this.IsVisibleInTree;

        @this._visible = value;

        if (visibilityInTreeChanging)
        {
            @this.NotifyVisibilityInTreeChanged(value);
        }

        if (@this._dock != default)
        {
            @this.InvalidateParentDock();
        }

        VisibilityChangedEventArgs eventArgs = new(value, visibilityInTreeChanging ? value : !value);
        @this.OnVisibilityChanged(@this, eventArgs);
        @this.VisibilityChanged?.Invoke(@this, eventArgs);

        @this.Invalidate();
        @this.InvalidateParent();
    }

    private void NotifyVisibilityInTreeChanged(bool visible)
    {
        VisibleUpdatableDataProviders(_updatableDataProviderRefCounts.Keys, visible);

        foreach (var child in _children)
        {
            if (child._inheritParentEnablementProperties)
            {
                SetVisible(child, _visible);
                continue;
            }

            if (!child._visible)
            {
                continue;
            }

            child.NotifyVisibilityInTreeChanged(visible);
            child.VisibilityChanged?.Invoke(this, new VisibilityChangedEventArgs(child._visible, visible));
        }
    }

    protected virtual void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {

    }

    protected virtual Point InnerPanelSizeFrom(Point size) => size;

    public virtual bool IsHiddenByTree => !_visible || (Parent?.IsHiddenByTree ?? false);

    public virtual bool IsDisabledByTree => _disabled || (Parent?.IsDisabledByTree ?? false);

    /// <summary>
    ///     Determines whether the control's position should be restricted to parent's bounds.
    /// </summary>
    public bool RestrictToParent
    {
        get => _restrictToParent;
        set => _restrictToParent = value;
    }

    /// <summary>
    ///     Determines whether the control receives mouse input events.
    /// </summary>
    public bool MouseInputEnabled
    {
        get => _mouseInputEnabled;
        set
        {
            if (value == _mouseInputEnabled)
            {
                return;
            }

            _mouseInputEnabled = value;
        }
    }

    /// <summary>
    ///     Determines whether the control receives keyboard input events.
    /// </summary>
    public bool KeyboardInputEnabled
    {
        get => _keyboardInputEnabled;
        set => _keyboardInputEnabled = value;
    }

    /// <summary>
    ///     Gets or sets the mouse cursor when the cursor is hovering the control.
    /// </summary>
    public Cursor Cursor
    {
        get => _cursor;
        set => _cursor = value;
    }

    public int GlobalX => X + (Parent?.GlobalX ?? 0);

    public int GlobalY => Y + (Parent?.GlobalY ?? 0);

    public Point PositionGlobal => new Point(X, Y) + (_parent?.PositionGlobal ?? Point.Empty);

    public Rectangle GlobalBounds => new(PositionGlobal, Size);

    /// <summary>
    ///     Indicates whether the control is tabable (can be focused by pressing Tab).
    /// </summary>
    public bool IsTabable
    {
        get => _tabEnabled;
        set => _tabEnabled = value;
    }

    /// <summary>
    ///     Indicates whether control's background should be drawn during rendering.
    /// </summary>
    public bool ShouldDrawBackground
    {
        get => _drawBackground;
        set => _drawBackground = value;
    }

    /// <summary>
    ///     Indicates whether the renderer should cache drawing to a texture to improve performance (at the cost of memory).
    /// </summary>
    public bool ShouldCacheToTexture
    {
        get => _cacheToTexture;
        set => SetAndDoIfChanged(ref _cacheToTexture, value, Invalidate);
    }

    /// <summary>
    ///     Gets the control's internal canonical name.
    /// </summary>
    public string ParentQualifiedName =>
        _host is { } parent ? $"{parent.Name}.{Name}" : Name;

    public string CanonicalName =>
        (_parent ?? _host) is { } parent
            ? $"{parent.CanonicalName}.{Name}"
            : _name ?? $"(unnamed {GetType().GetName(qualified: true)})";

    public string QualifiedName => $"{GetType().GetName(qualified: true)} ({Name})";

    /// <summary>
    ///     Gets or sets the control's internal name.
    /// </summary>
    public string Name
    {
        get => _name ?? $"(unnamed {GetType().Name})";
        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;
            _cachedToString = null;
        }
    }

    /// <summary>
    ///     Control's size and position relative to the parent.
    /// </summary>
    public Rectangle Bounds => _bounds;

    public Rectangle OuterBounds => _outerBounds;

    public bool ClipContents { get; set; } = true;

    /// <summary>
    ///     Bounds for the renderer.
    /// </summary>
    public Rectangle RenderBounds => _renderBounds;

    /// <summary>
    ///     Bounds adjusted by padding.
    /// </summary>
    public Rectangle InnerBounds => _innerBounds;

    /// <summary>
    ///     Size restriction.
    /// </summary>
    public Point MinimumSize
    {
        get => _minimumSize;
        set
        {
            var oldValue = _minimumSize;
            if (oldValue == value)
            {
                return;
            }

            _minimumSize = value;
            if (_innerPanel != null)
            {
                _innerPanel.MinimumSize = InnerPanelSizeFrom(value);
            }
            OnMinimumSizeChanged(oldValue, value);
            Invalidate(alsoInvalidateParent: true);
        }
    }

    /// <summary>
    ///     Size restriction.
    /// </summary>
    public Point MaximumSize
    {
        get => _maximumSize;
        set
        {
            var oldValue = _maximumSize;
            if (oldValue == value)
            {
                return;
            }

            _maximumSize = value;
            if (_innerPanel != null)
            {
                _innerPanel.MaximumSize = InnerPanelSizeFrom(value);
            }
            OnMaximumSizeChanged(oldValue, value);
            Invalidate(alsoInvalidateParent: true);
        }
    }

    /// <summary>
    ///     Determines whether hover should be drawn during rendering.
    /// </summary>
    protected bool ShouldDrawHover => InputHandler.MouseFocus == this || InputHandler.MouseFocus == null;

    protected virtual bool AccelOnlyFocus => false;

    protected virtual bool NeedsInputChars => false;

    /// <summary>
    ///     Indicates whether the control and its parents are visible.
    /// </summary>
    public bool IsVisibleInTree
    {
        get
        {
            if (!_visible)
            {
                return false;
            }

            return Parent is not { } parent || parent.IsVisibleInTree || ToolTip.IsActiveTooltip(parent);
        }
        set => RunOnMainThread(SetVisible, value);
    }

    public bool IsVisibleInParent
    {
        get => !IsHidden || Parent is not { } parent || ToolTip.IsActiveTooltip(parent);
        set => RunOnMainThread(SetVisible, value);
    }

    /// <summary>
    ///     Leftmost coordinate of the control.
    /// </summary>
    public int X
    {
        get => _bounds.X;
        set => SetPosition(value, Y);
    }

    /// <summary>
    ///     Topmost coordinate of the control.
    /// </summary>
    public int Y
    {
        get => _bounds.Y;
        set => SetPosition(X, value);
    }

    // todo: Bottom/Right includes margin but X/Y not?

    public int Width
    {
        get => _bounds.Width;
        set => SetSize(value, Height);
    }

    public int Height
    {
        get => _bounds.Height;
        set => SetSize(Width, value);
    }

    public int OuterWidth => Width + Margin.Left + Margin.Right;

    public int OuterHeight => Height + Margin.Top + Margin.Bottom;

    public Point Size
    {
        get => _bounds.Size;
        set => SetSize(value.X, value.Y);
    }

    public int InnerWidth => _bounds.Width - (_padding.Left + _padding.Right);

    public int MaximumInnerWidth => _maximumSize.X - (_padding.Left + _padding.Right);

    public int InnerHeight => _bounds.Height - (_padding.Top + _padding.Bottom);

    public int MaximumInnerHeight => _maximumSize.Y - (_padding.Top + _padding.Bottom);

    public int Bottom => _bounds.Bottom + _margin.Bottom;

    public int Right => _bounds.Right + _margin.Right;

    /// <summary>
    ///     Determines whether margin, padding and bounds outlines for the control will be drawn. Applied recursively to all
    ///     children.
    /// </summary>
    public bool DrawDebugOutlines
    {
        get => _drawDebugOutlines;
        set
        {
            _drawDebugOutlines = value;
            if (_innerPanel is { } innerPanel)
            {
                innerPanel.DrawDebugOutlines = value;
            }

            RunOnMainThread(PropagateDrawDebugOutlinesToChildren, this, value);
        }
    }

    private static void PropagateDrawDebugOutlinesToChildren(Base @this, bool drawDebugOutlines)
    {
        foreach (var child in @this._children)
        {
            child.DrawDebugOutlines = drawDebugOutlines;
        }
    }

    public Color PaddingOutlineColor { get; set; }

    public Color MarginOutlineColor { get; set; }

    public Color BoundsOutlineColor { get; set; }

    /// <summary>
    ///     Gets the canvas (root parent) of the control.
    /// </summary>
    /// <returns></returns>
    public Canvas? Canvas => _canvas;

    public bool SkipSerialization { get; set; } = false;

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
        catch
        {
            throw;
        }

        _disposeStack = new StackTrace(fNeedFileInfo: true);

        _disposed = true;

        Dispose(disposing: true);

        ICacheToTexture? cache = default;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            cache = DisposeSkin?.Renderer.Ctt;
        }
        catch
        {
            // If this fails it's because mSkin and mParent.Skin are null, we couldn't care less
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (ShouldCacheToTexture && cache != null)
        {
            cache.DisposeCachedTexture(this);
        }

        if (InputHandler.HoveredControl == this)
        {
            InputHandler.HoveredControl = null;
        }

        if (InputHandler.KeyboardFocus == this)
        {
            InputHandler.KeyboardFocus = null;
        }

        if (InputHandler.MouseFocus == this)
        {
            InputHandler.MouseFocus = null;
        }

        DragAndDrop.ControlDeleted(this);
        ToolTip.ControlDeleted(this);
        Animation.Cancel(this);

        DisposeChildrenOf(this);

        GC.SuppressFinalize(this);

        _disposeCompleted = true;
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private void DisposeChildrenOf(Base target)
    {
        if (_host is not { _disposeCompleted: false } parent)
        {
            DisposeChildren(target);
        }
        else
        {
            parent.DisposeChildrenOf(target);
        }
    }

    private static void DisposeChildren(Base @this)
    {
        var children = @this._children.ToArray();
        try
        {
            foreach (var child in @this._children)
            {
                child.Dispose();
            }
        }
        catch
        {
            throw;
        }

        if (@this is Modal)
        {
            ApplicationContext.CurrentContext.Logger.LogTrace(
                "Clearing {ChildCount} children of Modal {Name}",
                @this._children.Count,
                @this.CanonicalName
            );
        }
        @this._children.Clear();
    }

    /// <summary>
    ///     Invoked before this control is drawn
    /// </summary>
    public event GwenEventHandler<EventArgs>? BeforeDraw;

    /// <summary>
    ///     Invoked after this control is drawn
    /// </summary>
    public event GwenEventHandler<EventArgs>? AfterDraw;

    public event GwenEventHandler<EventArgs>? BeforeLayout;

    public event GwenEventHandler<EventArgs>? AfterLayout;

    public void AddAlignment(Alignments alignment)
    {
        if (_alignments?.Contains(alignment) ?? true)
        {
            return;
        }

        _alignments.Add(alignment);
    }

    public void RemoveAlignments()
    {
        _alignments.Clear();
    }

    public virtual string GetJsonUI(bool isRoot = false)
    {
        return JsonConvert.SerializeObject(GetJson(isRoot), Formatting.Indented);
    }

    private static void AddChildrenToJson(Base @this, JObject json)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var node in @this._children)
        {
            if (node == @this._innerPanel)
            {
                continue;
            }

            if (node == @this.Tooltip)
            {
                continue;
            }

            if (node._name is not { } name || string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (json.ContainsKey(name))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Unable to add '{ChildName}' to the JSON of {ParentName} because a sibling shares the same name",
                    name,
                    @this.CanonicalName
                );
                continue;
            }

            var nodeJson = node.GetJson();
            json.Add(node.Name, nodeJson);
        }
    }

    public virtual JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        JObject json = new();

        RunOnMainThread(AddChildrenToJson, this, json);

        if (onlySerializeIfNotEmpty && !json.HasValues)
        {
            return null;
        }

        isRoot |= Parent == default;

        var boundsToWrite = isRoot
            ? new Rectangle(_boundsOnDisk.X, _boundsOnDisk.Y, _bounds.Width, _bounds.Height)
            : _bounds;

        var serializedProperties = new JObject(
            new JProperty(nameof(Bounds), Rectangle.ToString(boundsToWrite)),
            new JProperty(nameof(Dock), Dock.ToString()),
            new JProperty(nameof(Padding), Padding.ToString(_padding)),
            new JProperty("AlignmentEdgeDistances", Padding.ToString(_alignmentPadding)),
            new JProperty("AlignmentTransform", _alignmentTranslation.ToString()),
            new JProperty(nameof(Margin), _margin.ToString()),
            new JProperty(nameof(RenderColor), Color.ToString(_color)),
            new JProperty(nameof(Alignments), string.Join(",", _alignments.ToArray())),
            new JProperty("DrawBackground", _drawBackground),
            new JProperty(nameof(MinimumSize), _minimumSize.ToString()),
            new JProperty(nameof(MaximumSize), _maximumSize.ToString()),
            new JProperty("Disabled", _disabled),
            new JProperty("Hidden", !_visible),
            new JProperty(nameof(RestrictToParent), _restrictToParent),
            new JProperty(nameof(MouseInputEnabled), _mouseInputEnabled),
            new JProperty("HideToolTip", !_tooltipEnabled),
            new JProperty("ToolTipBackground", _tooltipBackgroundName),
            new JProperty("ToolTipFont", _tooltipFontInfo),
            new JProperty(
                nameof(TooltipTextColor),
                _tooltipTextColor == null ? JValue.CreateNull() : Color.ToString(_tooltipTextColor)
            )
        );

        if (_innerPanel is { } innerPanel)
        {
            if (innerPanel.GetJson(onlySerializeIfNotEmpty: true) is {} serializedInnerPanel)
            {
                serializedProperties.Add(PropertyNameInnerPanel, serializedInnerPanel);
            }
        }

        if (json.HasValues)
        {
            serializedProperties.Add(nameof(Children), json);
        }

        return FixJson(serializedProperties);
    }

    public virtual JObject FixJson(JObject json)
    {
        if (json.Remove(PropertyNameInnerPanel, out var tokenInnerPanel))
        {
            json.Add(PropertyNameInnerPanel, tokenInnerPanel);
        }

        if (json.Remove(nameof(Children), out var tokenChildren))
        {
            json.Add(nameof(Children), tokenChildren);
        }

        return json;
    }

    protected virtual Rectangle ValidateJsonBounds(Rectangle bounds) => bounds;

    public void LoadJsonUi(GameContentManager.UI stage, string? resolution = default, bool saveOutput = true)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                $"Attempted to load layout for nameless {GetType().FullName}"
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(resolution) || string.IsNullOrEmpty(resolution))
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                $"Attempted to load layout for {Name} with no resolution"
            );
            return;
        }

        _ = GameContentManager.Current?.GetLayout(stage, Name, resolution, false, (rawLayout, cacheHit) =>
        {
            if (!string.IsNullOrWhiteSpace(rawLayout))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(rawLayout);

                    if (obj != null)
                    {
                        LoadJson(obj, true);
                        ProcessAlignments();
                        OnJsonReloaded();
                    }
                }
                catch (Exception exception)
                {
                    //Log JSON UI Loading Error
                    throw new Exception("Error loading json ui for " + ParentQualifiedName, exception);
                }
            }

            if (!cacheHit && saveOutput)
            {
                GameContentManager.Current?.SaveUIJson(stage, Name, GetJsonUI(true), resolution);
            }
        });
    }

    protected virtual void OnJsonReloaded()
    {

    }

    public virtual void LoadJson(JToken token, bool isRoot = default)
    {
        if (token is not JObject obj)
        {
            return;
        }

        if (obj[nameof(Alignments)] != null)
        {
            RemoveAlignments();
            var alignments = ((string) obj[nameof(Alignments)]).Split(',');
            foreach (var alignment in alignments)
            {
                switch (alignment.ToLower())
                {
                    case "top":
                        AddAlignment(Alignments.Top);

                        break;

                    case "bottom":
                        AddAlignment(Alignments.Bottom);

                        break;

                    case "left":
                        AddAlignment(Alignments.Left);

                        break;

                    case "right":
                        AddAlignment(Alignments.Right);

                        break;

                    case "center":
                        AddAlignment(Alignments.Center);

                        break;

                    case "centerh":
                        AddAlignment(Alignments.CenterH);

                        break;

                    case "centerv":
                        AddAlignment(Alignments.CenterV);

                        break;
                }
            }
        }

        if (obj[nameof(MinimumSize)] != null)
        {
            MinimumSize = Point.FromString((string)obj[nameof(MinimumSize)]);
        }

        if (obj[nameof(MaximumSize)] != null)
        {
            MaximumSize = Point.FromString((string)obj[nameof(MaximumSize)]);
        }

        if (obj[nameof(Bounds)] != null)
        {
            _boundsOnDisk = Rectangle.FromString((string)obj[nameof(Bounds)]);
            isRoot = isRoot || Parent == default;
            if (isRoot)
            {
                SetSize(_boundsOnDisk.Width, _boundsOnDisk.Height);
            }
            else
            {
                SetBounds(ValidateJsonBounds(_boundsOnDisk));
            }
        }

        if (obj.TryGet<Pos>(nameof(Dock), out var dock))
        {
            Dock = dock;
        }

        if (obj[nameof(Padding)] != null)
        {
            Padding = Padding.FromString((string) obj[nameof(Padding)]);
        }

        if (obj["AlignmentEdgeDistances"] != null)
        {
            _alignmentPadding = Padding.FromString((string) obj["AlignmentEdgeDistances"]);
        }

        if (obj["AlignmentTransform"] != null)
        {
            _alignmentTranslation = Point.FromString((string) obj["AlignmentTransform"]);
        }

        if (obj[nameof(Margin)] != null)
        {
            Margin = Margin.FromString((string) obj[nameof(Margin)]);
        }

        if (obj[nameof(RenderColor)] != null)
        {
            RenderColor = Color.FromString((string) obj[nameof(RenderColor)]);
        }

        if (obj["DrawBackground"] != null)
        {
            ShouldDrawBackground = (bool) obj["DrawBackground"];
        }

        if (obj["Disabled"] != null)
        {
            IsDisabled = (bool) obj["Disabled"];
        }

        if (obj["Hidden"] != null)
        {
            IsHidden = (bool) obj["Hidden"];
        }

        if (obj[nameof(RestrictToParent)] != null)
        {
            RestrictToParent = (bool) obj[nameof(RestrictToParent)];
        }

        if (obj[nameof(MouseInputEnabled)] != null)
        {
            MouseInputEnabled = (bool) obj[nameof(MouseInputEnabled)];
        }

        if (obj["HideToolTip"] != null && (bool) obj["HideToolTip"])
        {
            _tooltipEnabled = false;
            SetToolTipText(null);
        }

        if (obj[nameof(TooltipBackground)] is JValue { Type: JTokenType.String } tokenTooltipBackgroundName)
        {
            var tooltipBackgroundName = tokenTooltipBackgroundName.Value<string>();
            TooltipBackgroundName = tooltipBackgroundName;
        }

        if (obj.TryGetValue(nameof(TooltipFont), out var tokenTooltipFont) && tokenTooltipFont is JValue { Type: JTokenType.String } valueTooltipFont)
        {
            if (valueTooltipFont.Value<string>() is { } fontInfo)
            {
                var fontParts = fontInfo.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var fontName = fontParts.FirstOrDefault();
                if (!int.TryParse(fontParts.Skip(1).FirstOrDefault(), out var fontSize))
                {
                    fontSize = 10;
                }

                TooltipFontName = fontName;
                TooltipFontSize = fontSize;
            }
            else
            {
                TooltipFont = null;
            }
        }

        if (obj[nameof(TooltipTextColor)] is JValue { Type: JTokenType.String } tooltipTextColorValue)
        {
            var tooltipTextColorString = tooltipTextColorValue.Value<string>();
            if (!string.IsNullOrWhiteSpace(tooltipTextColorString))
            {
                TooltipTextColor = Color.FromString(tooltipTextColorString);
            }
        }

        UpdateToolTipProperties();

        if (_innerPanel is { } innerPanel && obj.TryGetValue(PropertyNameInnerPanel, out var tokenInnerPanel))
        {
            innerPanel.LoadJson(tokenInnerPanel);
        }

        if (obj.TryGetValue(nameof(Children), out var tokenChildren) && tokenChildren is JObject objectChildren)
        {
            RunOnMainThread(LoadChildrenJson, this, objectChildren);
        }

        Invalidate(alsoInvalidateParent: true);
    }

    private static void LoadChildrenJson(Base @this, JObject objectChildren)
    {
        foreach (var child in @this._children)
        {
            if (child._name is not { } name || string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (objectChildren.TryGetValue(name, out var tokenChild))
            {
                child.LoadJson(tokenChild);
            }
        }
    }

    public virtual void ProcessAlignments()
    {
        foreach (var alignment in _alignments)
        {
            switch (alignment)
            {
                case Alignments.Top:
                    Align.AlignTop(this);
                    break;

                case Alignments.Bottom:
                    Align.AlignBottom(this);
                    break;

                case Alignments.Left:
                    Align.AlignLeft(this);
                    break;

                case Alignments.Right:
                    Align.AlignRight(this);
                    break;

                case Alignments.Center:
                    Align.Center(this);
                    break;

                case Alignments.CenterH:
                    Align.CenterHorizontally(this);
                    break;

                case Alignments.CenterV:
                    Align.CenterVertically(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
            }
        }

        MoveTo(X + _alignmentTranslation.X, Y + _alignmentTranslation.Y, true);
        foreach (var child in Children)
        {
            child?.ProcessAlignments();
        }
    }

    public event GwenEventHandler<VisibilityChangedEventArgs>? VisibilityChanged;

    /// <summary>
    ///     Invoked when mouse pointer enters the control.
    /// </summary>
    public event GwenEventHandler<EventArgs>? HoverEnter;

    /// <summary>
    ///     Invoked when mouse pointer leaves the control.
    /// </summary>
    public event GwenEventHandler<EventArgs>? HoverLeave;

    /// <summary>
    ///     Invoked when control's bounds have been changed.
    /// </summary>
    public event GwenEventHandler<ValueChangedEventArgs<Rectangle>>? BoundsChanged;
    public event GwenEventHandler<ValueChangedEventArgs<Point>>? PositionChanged;
    public event GwenEventHandler<ValueChangedEventArgs<Point>>? SizeChanged;

    public virtual event GwenEventHandler<MouseButtonState>? MouseDown;

    public virtual event GwenEventHandler<MouseButtonState>? MouseUp;

    /// <summary>
    ///     Invoked when the control has been left-clicked.
    /// </summary>
    public virtual event GwenEventHandler<MouseButtonState>? Clicked;

    /// <summary>
    ///     Invoked when the control has been double-left-clicked.
    /// </summary>
    public virtual event GwenEventHandler<MouseButtonState>? DoubleClicked;

#if DIAGNOSTIC
    ~Base()
    {
        ApplicationContext.Context.Value?.Logger.LogDebug($"IDisposable object finalized: {GetType()}");
    }
#endif

    /// <summary>
    ///     Detaches the control from canvas and adds to the deletion queue (processed in Canvas.DoThink).
    /// </summary>
    public void DelayedDelete()
    {
        Canvas?.AddDelayedDelete(this);
    }

    public override string ToString() => _cachedToString ??= InternalToString();

    /// <summary>
    ///     Enables the control.
    /// </summary>
    // TODO: Remove
    public void Enable()
    {
        IsDisabled = false;
    }

    /// <summary>
    ///     Disables the control.
    /// </summary>
    // TODO: Remove
    public virtual void Disable()
    {
        IsDisabled = true;
    }

    /// <summary>
    ///     Default accelerator handler.
    /// </summary>
    /// <param name="control">Event source.</param>
    private void DefaultAcceleratorHandler(Base control, EventArgs args)
    {
        OnAccelerator();
    }

    /// <summary>
    ///     Default accelerator handler.
    /// </summary>
    protected virtual void OnAccelerator()
    {
    }

    /// <summary>
    ///     Hides the control.
    /// </summary>
    public virtual void Hide()
    {
        IsHidden = true;
    }

    /// <summary>
    ///     Shows the control.
    /// </summary>
    public virtual void Show()
    {
        IsHidden = false;
    }

    public virtual void ToggleHidden()
    {
        IsHidden = !IsHidden;
    }

    /// <summary>
    ///     Creates a tooltip for the control.
    /// </summary>
    /// <param name="text">Tooltip text.</param>
    public virtual void SetToolTipText(string? text)
    {
        var tooltip = Tooltip;

        if (!_tooltipEnabled || string.IsNullOrWhiteSpace(text))
        {
            if (Tooltip is { Parent: not null })
            {
                Tooltip?.Parent.RemoveChild(Tooltip, true);
            }
            Tooltip = null;

            return;
        }

        if (tooltip is not Label labelTooltip)
        {
            if (tooltip is not null)
            {
                return;
            }

            labelTooltip = new Label(this, name: nameof(Tooltip))
            {
                AutoSizeToContents = true,
                Font = _tooltipFont ?? GameContentManager.Current.GetFont("sourcesansproblack"),
                FontSize = TooltipFontSize,
                MaximumSize = new Point(300, 0),
                Padding = new Padding(
                    5,
                    3
                ),
                TextColor = Skin.Colors.TooltipText,
                TextColorOverride = _tooltipTextColor ?? Skin.Colors.TooltipText,
                ToolTipBackground = _tooltipBackground,
                WrappingBehavior = WrappingBehavior.Wrapped,
            };

            Tooltip = labelTooltip;
        }

        labelTooltip.Text = text;
    }

    protected virtual void UpdateToolTipProperties()
    {
        if (Tooltip != null && Tooltip is Label tooltip)
        {
            tooltip.TextColorOverride = _tooltipTextColor ?? Skin.Colors.TooltipText;
            if (_tooltipFont != null)
            {
                tooltip.Font = _tooltipFont;
            }

            tooltip.ToolTipBackground = _tooltipBackground;
        }
    }

    /// <summary>
    ///     Invalidates the control's children (relayout/repaint).
    /// </summary>
    /// <param name="recursive">Determines whether the operation should be carried recursively.</param>
    protected virtual void InvalidateChildren(bool recursive = false)
    {
        RunOnMainThread(InvalidateChildren, this, recursive);

        _innerPanel?.Invalidate();
        _innerPanel?.InvalidateChildren(recursive: true);
    }

    private static void InvalidateChildren(Base @this, bool recursive)
    {
        foreach (var node in @this._children)
        {
            node.Invalidate();
            if (recursive)
            {
                node.InvalidateChildren(true);
            }
        }
    }

    /// <summary>
    ///     Invalidates the control.
    /// </summary>
    /// <remarks>
    ///     Causes layout, repaint, invalidates cached texture.
    /// </remarks>
    public virtual void Invalidate()
    {
        if (!_layoutDirty)
        {
            _layoutDirty = true;
        }

        if (!_cacheTextureDirty)
        {
            _cacheTextureDirty = true;
        }
    }

    protected static void Invalidate(Base @this) => @this.Invalidate();

    public void Invalidate(bool alsoInvalidateParent)
    {
        Invalidate();
        if (alsoInvalidateParent)
        {
            InvalidateParent();
        }
    }

    public void MoveBefore(Base other)
    {
        if (ReferenceEquals(this, other))
        {
            return;
        }

        if (_host is not { } parent)
        {
            return;
        }

        parent.RunOnMainThread(MoveChildBeforeOther, this, other);
    }

    private static void MoveChildBeforeOther(Base @this, Base childToMove, Base other)
    {
        if (other.Parent != @this)
        {
            return;
        }

        var children = @this.HostedChildren;
        var otherIndex = children.IndexOf(other);
        if (otherIndex < 0)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Possible race condition detected: '{Other}' is not a child of '{Parent}'",
                other.CanonicalName,
                @this.CanonicalName
            );
            return;
        }

        var ownIndex = children.IndexOf(childToMove);


        if (ownIndex < 0)
        {
            children.Insert(otherIndex, childToMove);
            return;
        }

        var insertionIndex = otherIndex;
        if (ownIndex < otherIndex)
        {
            --insertionIndex;
        }

        if (ownIndex == insertionIndex)
        {
            return;
        }

        children.Remove(childToMove);
        children.Insert(insertionIndex, childToMove);
    }

    public void MoveAfter(Base other)
    {
        if (ReferenceEquals(this, other))
        {
            return;
        }

        if (_host is not { } parent)
        {
            return;
        }

        parent.RunOnMainThread(MoveChildAfterOther, this, other);
    }

    private List<Base> HostedChildren => _innerPanel?.HostedChildren ?? _children;

    private static void MoveChildAfterOther(Base @this, Base childToMove, Base other)
    {
        if (other.Parent != @this)
        {
            return;
        }

        var children = @this.HostedChildren;
        var otherIndex = children.IndexOf(other);
        if (otherIndex < 0)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Possible race condition detected: '{Other}' is not a child of '{Parent}'",
                other.CanonicalName,
                @this.CanonicalName
            );
            return;
        }

        var ownIndex = children.IndexOf(childToMove);

        if (ownIndex < 0)
        {
            // If we have no parent yet, insert it
            children.Insert(otherIndex + 1, childToMove);
            return;
        }

        var insertionIndex = otherIndex;
        if (ownIndex > insertionIndex)
        {
            ++insertionIndex;
        }

        if (ownIndex == insertionIndex)
        {
            return;
        }

        children.Remove(childToMove);
        children.Insert(insertionIndex, childToMove);
    }

    public void SortChildrenBy<TSortKey>(Func<Base?, TSortKey> keySelector) where TSortKey : IComparable<TSortKey>
    {
        _children.Sort(
            (a, b) =>
            {
                TSortKey keyB = keySelector(b);

                if (keySelector(a) is { } keyA)
                {
                    return keyA.CompareTo(keyB);
                }

                // Not sure why no matter what I do that the static analysis refuses to acknowledge that it can be null,
                // so I'm just disabling it instead so it doesn't get hit by auto-formatting
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                return -keyB?.CompareTo(default) ?? 0;
            }
        );
    }

    public void ClearChildren(bool dispose = false) => RunOnMainThread(ClearChildren, this, dispose);

    private static void ClearChildren(Base @this, bool dispose)
    {
        if (@this is Modal)
        {
            ApplicationContext.CurrentContext.Logger.LogTrace(
                "Clearing {ChildCount} children of Modal {Name}",
                @this._children.Count,
                @this.CanonicalName
            );
        }

        if (dispose)
        {
            foreach (var child in @this.HostedChildren)
            {
                child.Dispose();
            }
        }

        @this.HostedChildren.Clear();
        @this.Invalidate();
    }

    public void Insert(int index, Base node) => RunOnMainThread(Insert, this, index, node);

    private static void Insert(Base @this, int index, Base node)
    {
        @this._children.Insert(index, node);
    }

    public void Remove(Base node) => RunOnMainThread(Remove, this, node);

    private static void Remove(Base @this, Base node)
    {
        var index = @this._children.BinarySearch(node);
        if (index < 0)
        {
            return;
        }

        RemoveAt(@this, index);
    }

    public void RemoveAt(int index) => RunOnMainThread(RemoveAt, this, index);

    private static void RemoveAt(Base @this, int index)
    {
        if (@this is Modal)
        {
            ApplicationContext.CurrentContext.Logger.LogTrace(
                "Removing child at {Index} of Modal {Name}",
                index,
                @this.CanonicalName
            );
        }
        @this._children.RemoveAt(index);
    }

    public int IndexOf(Base? node)
    {
        if (node?.Parent != this)
        {
            return -1;
        }

        return RunOnMainThread(IndexOf, node);
    }

    private static int IndexOf(Base @this, Base node) => @this._children.IndexOf(node);

    public int IndexOf(Func<Base?, bool> predicate) => RunOnMainThread(IndexOf, predicate);

    private static int IndexOf(Base @this, Func<Base?, bool> predicate)
    {
        for (var index = 0; index < @this._children.Count; ++index)
        {
            if (predicate(@this._children[index]))
            {
                return index;
            }
        }

        return -1;
    }

    public int LastIndexOf(Func<Base?, bool> predicate) => RunOnMainThread(LastIndexOf, predicate);

    private static int LastIndexOf(Base @this, Func<Base?, bool> predicate)
    {
        for (var index = @this._children.Count - 1; index >= 0; ++index)
        {
            if (predicate(@this._children[index]))
            {
                return index;
            }
        }

        return -1;
    }

    public void ResortChild<TKey>(Base child, Func<Base?, TKey?> keySelector) where TKey : IComparable<TKey>
    {
        if (child.Parent != this)
        {
            return;
        }

        RunOnMainThread(ResortChild, this, child, keySelector);
    }

    private static void ResortChild<TKey>(Base @this, Base child, Func<Base?, TKey?> keySelector)
        where TKey : IComparable<TKey> =>
        @this._children.Resort(child, keySelector);

    /// <summary>
    ///     Sends the control to the bottom of paren't visibility stack.
    /// </summary>
    public void SendToBack() => RunOnMainThread(SendToBack, this);

    private static void SendToBack(Base @this)
    {
        if (@this._parent == null)
        {
            return;
        }

        if (@this._parent._children.Count == 0)
        {
            return;
        }

        if (@this._parent._children.First() == @this)
        {
            return;
        }

        @this._parent._children.Remove(@this);
        @this._parent._children.Insert(0, @this);

        @this.InvalidateParent();
    }

    /// <summary>
    ///     Brings the control to the top of paren't visibility stack.
    /// </summary>
    public void BringToFront() => RunOnMainThread(BringToFront, this);

    private static void BringToFront(Base @this)
    {
        if (@this._host is Modal modal)
        {
            modal.BringToFront();
        }

        var actualParent = @this._parent;
        // Using null propagation somehow breaks the static analysis after the return
        // ReSharper disable once UseNullPropagation
        if (actualParent is null)
        {
            return;
        }

        var last = actualParent._children.LastOrDefault();
        if (last == default || last == @this)
        {
            return;
        }

        actualParent._children.Remove(@this);
        actualParent._children.Add(@this);
        @this.InvalidateParent();
        @this.Redraw();
    }

    /// <summary>
    /// Finds the first child that matches the predicate.
    /// </summary>
    /// <param name="predicate">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
    /// <param name="recurse">Whether or not the search will recurse through the element tree.</param>
    /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <see cref="Base" />.</returns>
    public virtual Base Find(Predicate<Base> predicate, bool recurse = false)
    {
        var child = _children.Find(predicate);
        if (child != null)
        {
            return child;
        }

        return recurse
            ? _children.Select(selectChild => selectChild?.Find(predicate, true)).FirstOrDefault()
            : default;
    }

    public (Base Highest, Base Closest)? FindMatchingNodes<TComponent>() where TComponent : class
    {
        Base? highest = null;
        Base? closest = null;
        Base? currentComponent = this;

        while (currentComponent is not null)
        {
            if (currentComponent is TComponent)
            {
                closest ??= currentComponent;
                highest = currentComponent;
            }

            currentComponent = currentComponent.Parent;
        }

        return (highest is null || closest is null) ? null : (highest, closest);
    }

    /// <summary>
    /// Finds all children that match the predicate.
    /// </summary>
    /// <param name="predicate">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
    /// <param name="recurse">Whether or not the search will recurse through the element tree.</param>
    /// <returns>All elements that matches the conditions defined by the specified predicate.</returns>
    public virtual IEnumerable<Base> FindAll(Predicate<Base> predicate, bool recurse = false)
    {
        var children = new List<Base>();

        children.AddRange(_children.FindAll(predicate));

        if (recurse)
        {
            children.AddRange(_children.SelectMany(selectChild => selectChild?.FindAll(predicate, true)));
        }

        return children;
    }

    /// <summary>
    ///     Finds a child by name.
    /// </summary>
    /// <param name="name">Child name.</param>
    /// <param name="recursive">Determines whether the search should be recursive.</param>
    /// <returns>Found control or null.</returns>
    public virtual Base? FindChildByName(string name, bool recursive = false) => FindChildByName<Base>(name, recursive);

    /// <summary>
    ///     Finds a child of a given type by name.
    /// </summary>
    /// <param name="name">Child name.</param>
    /// <param name="recursive">Determines whether the search should be recursive.</param>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    /// <returns>Found control or null.</returns>
    public virtual TComponent? FindChildByName<TComponent>(string name, bool recursive = false) where TComponent : Base
    {
        var children = Children.ToArray();
        foreach (var child in children)
        {
            if (child is TComponent typedChild)
            {
                if (string.Equals(typedChild.Name, name, StringComparison.Ordinal))
                {
                    return typedChild;
                }
            }

            if (recursive)
            {
                return child.FindChildByName<TComponent>(name, true);
            }
        }

        return null;
    }

    /// <summary>
    ///     Makes the window modal: covers the whole canvas and gets all input.
    /// </summary>
    /// <param name="dim">Determines whether all the background should be dimmed.</param>
    public void MakeModal(bool dim = false)
    {
        if (_modal != null)
        {
            return;
        }

        _modal = new Modal(Canvas)
        {
            ShouldDrawBackground = dim,
        };

        _previousParent = Parent;
        Parent = _modal;
    }

    public void RemoveModal()
    {
        if (_modal is not { } modal)
        {
            return;
        }

        Parent = _previousParent;
        Canvas?.RemoveChild(modal, false);
        _modal = null;
    }

    /// <summary>
    ///     Attaches specified control as a child of this one.
    /// </summary>
    /// <remarks>
    ///     If InnerPanel is not null, it will become the parent.
    /// </remarks>
    /// <param name="node">Control to be added as a child.</param>
    /// <param name="setParent"></param>
    public void AddChild(Base node) => RunOnMainThread(AddChild, this, node);

    private static void AddChild(Base @this, Base node)
    {
        if (@this._innerPanel == null)
        {
            @this._children.Add(node);
            node._parent = @this;
        }
        else
        {
            @this._innerPanel.AddChild(node);
        }

        if (node._dock != default)
        {
            @this.InvalidateDock();
        }

        node.DrawDebugOutlines = @this.DrawDebugOutlines;

        @this.OnChildAdded(node);
    }

    /// <summary>
    ///     Detaches specified control from this one.
    /// </summary>
    /// <param name="child">Child to be removed.</param>
    /// <param name="dispose">Determines whether the child should be disposed (added to delayed delete queue).</param>
    public virtual void RemoveChild(Base child, bool dispose) => RunOnMainThread(RemoveChild, this, child, dispose);

    private static void RemoveChild(Base @this, Base child, bool dispose)
    {
        // If we removed our inner panel
        // remove our pointer to it
        if (@this._innerPanel == child)
        {
            try
            {
                if (@this is Modal)
                {
                    ApplicationContext.CurrentContext.Logger.LogTrace(
                        "Removing {ChildName} (inner panel) from Modal {Name} (Thread {ThreadId})",
                        child.CanonicalName,
                        @this.CanonicalName,
                        Environment.CurrentManagedThreadId
                    );
                }
                @this._children.Remove(child);
                child.DelayedDelete();
                @this._innerPanel = null;
            }
            catch (NullReferenceException)
            {
                throw;
            }

            return;
        }

        if (@this._innerPanel is { } innerPanel && innerPanel.Children.Contains(child))
        {
            innerPanel.RemoveChild(child, dispose);

            return;
        }

        if (@this is Modal)
        {
            ApplicationContext.CurrentContext.Logger.LogTrace(
                "Removing {ChildName} (normal child) from Modal {Name} (Thread {ThreadId})",
                child.CanonicalName,
                @this.CanonicalName,
                Environment.CurrentManagedThreadId
            );
        }
        @this._children.Remove(child);
        @this.OnChildRemoved(child);

        if (dispose)
        {
            child.DelayedDelete();
        }
    }

    /// <summary>
    ///     Removes all children (and disposes them).
    /// </summary>
    public virtual void DeleteAllChildren()
    {
        // todo: probably shouldn't invalidate after each removal
        while (_children.Count > 0)
        {
            RemoveChild(_children[0], true);
        }
    }

    /// <summary>
    ///     Handler invoked when a child is added.
    /// </summary>
    /// <param name="child">Child added.</param>
    protected virtual void OnChildAdded(Base child)
    {
        child?.OnAttached(this);
        Invalidate();
    }

    /// <summary>
    ///     Handler invoked when a child is removed.
    /// </summary>
    /// <param name="child">Child removed.</param>
    protected virtual void OnChildRemoved(Base child)
    {
        child?.OnDetached();
        Invalidate();
    }

    /// <summary>
    ///     Moves the control by a specific amount.
    /// </summary>
    /// <param name="x">X-axis movement.</param>
    /// <param name="y">Y-axis movement.</param>
    public virtual void MoveBy(int x, int y) => SetBounds(X + x, Y + y, Width, Height);

    /// <summary>
    ///     Moves the control to a specific point.
    /// </summary>
    /// <param name="x">Target x coordinate.</param>
    /// <param name="y">Target y coordinate.</param>
    public virtual void MoveTo(float x, float y) => MoveTo((int) x, (int) y);

    /// <summary>
    ///     Moves the control to a specific point, clamping on parent bounds if RestrictToParent is set.
    /// </summary>
    /// <param name="x">Target x coordinate.</param>
    /// <param name="y">Target y coordinate.</param>
    /// <param name="aligning"></param>
    public virtual void MoveTo(int x, int y, bool aligning = false)
    {
        var ownWidth = Width;
        var ownHeight = Height;
        var ownMargin = Margin;

        if (RestrictToParent && Parent is {} parent)
        {
            var ownPadding = Parent.Padding;

            var alignmentDistanceLeft = aligning ? _alignmentPadding.Left : 0;
            var alignmentDistanceTop = aligning ? _alignmentPadding.Top : 0;
            var alignmentDistanceRight = aligning ? _alignmentPadding.Right : 0;
            var alignmentDistanceBottom = aligning ? _alignmentPadding.Bottom : 0;
            var parentWidth = parent.Width;
            var parentHeight = parent.Height;

            var xFromLeft = ownMargin.Left + ownPadding.Left + alignmentDistanceLeft;
            var xFromRight = parentWidth - ownMargin.Right - ownWidth - ownPadding.Right - alignmentDistanceRight;

            if (xFromLeft > xFromRight)
            {
                x = (int)((xFromLeft + xFromRight) / 2f);
            }
            else
            {
                x = Math.Clamp(x, xFromLeft, xFromRight);
            }

            var yFromTop = ownMargin.Top + ownPadding.Top + alignmentDistanceTop;
            var yFromBottom = parentHeight - ownMargin.Bottom - ownHeight - ownPadding.Bottom - alignmentDistanceBottom;

            if (yFromTop > yFromBottom)
            {
                y = (int)((yFromTop + yFromBottom) / 2f);
            }
            else
            {
                y = Math.Clamp(y, yFromTop, yFromBottom);
            }
        }
        else
        {
            x = Math.Max(x, ownMargin.Left);
            y = Math.Max(y, ownMargin.Top);
        }

        SetBounds(x, y, ownWidth, ownHeight);
    }

    /// <summary>
    ///     Sets the control position based on ImagePanel
    /// </summary>
    public virtual void SetPosition(Base _icon)
    {
        SetPosition((int)_icon.ToCanvas(new Point(0, 0)).X, (int)_icon.ToCanvas(new Point(0, 0)).Y);
    }

    /// <summary>
    ///     Sets the control position.
    /// </summary>
    /// <param name="x">Target x coordinate.</param>
    /// <param name="y">Target y coordinate.</param>
    public virtual void SetPosition(float x, float y)
    {
        SetPosition((int) x, (int) y);
    }

    /// <summary>
    ///     Sets the control position.
    /// </summary>
    /// <param name="x">Target x coordinate.</param>
    /// <param name="y">Target y coordinate.</param>
    public virtual void SetPosition(int x, int y) => SetBounds(x, y, Width, Height);

    public virtual void SetPosition(Point point) => SetBounds(
        point.X,
        point.Y,
        Width,
        Height
    );

    /// <summary>
    ///     Sets the control size.
    /// </summary>
    /// <param name="width">New width.</param>
    /// <param name="height">New height.</param>
    /// <returns>True if bounds changed.</returns>
    public virtual bool SetSize(int width, int height) => SetBounds(X, Y, width, height);

    public bool SetSize(Point point) => SetSize(point.X, point.Y);

    /// <summary>
    ///     Sets the control bounds.
    /// </summary>
    /// <param name="bounds">New bounds.</param>
    /// <returns>True if bounds changed.</returns>
    public virtual bool SetBounds(Rectangle bounds) => SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);

    public virtual bool SetBounds(Point position, Point size) => SetBounds(
        position.X,
        position.Y,
        size.X,
        size.Y
    );

    /// <summary>
    ///     Sets the control bounds.
    /// </summary>
    /// <param name="x">X.</param>
    /// <param name="y">Y.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <returns>
    ///     True if bounds changed.
    /// </returns>
    public virtual bool SetBounds(float x, float y, float width, float height) => SetBounds((int) x, (int) y, (int) width, (int) height);

    protected virtual void OnMaximumSizeChanged(Point oldSize, Point newSize)
    {
    }
    protected virtual void OnMinimumSizeChanged(Point oldSize, Point newSize)
    {
    }

    protected virtual void OnSizeChanged(Point oldSize, Point newSize)
    {
        Parent?.OnChildSizeChanged(this, oldSize, newSize);
    }

    /// <summary>
    ///     Sets the control bounds.
    /// </summary>
    /// <param name="x">X position.</param>
    /// <param name="y">Y position.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <returns>
    ///     True if bounds changed.
    /// </returns>
    public virtual bool SetBounds(int x, int y, int width, int height)
    {
        width = Math.Max(0, width);
        height = Math.Max(0, height);

        if (_bounds.X == x && _bounds.Y == y && _bounds.Width == width && _bounds.Height == height)
        {
            return false;
        }

        var oldBounds = Bounds;

        var newBounds = _bounds with
        {
            X = x,
            Y = y,
        };

        var maximumSize = MaximumSize;
        var minimumSize = MinimumSize;
        newBounds.Width = Math.Max(maximumSize.X > 0 ? Math.Min(maximumSize.X, width) : width, minimumSize.X);
        newBounds.Height = Math.Max(maximumSize.Y > 0 ? Math.Min(maximumSize.Y, height) : height, minimumSize.Y);

        if (newBounds.Width > 100000 || newBounds.Height > 100000)
        {
            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Extremely large component resize '{ComponentName}' {OldBounds} => {NewBounds}",
                ParentQualifiedName,
                oldBounds.Size,
                newBounds.Size
            );
        }

        _bounds = newBounds;

        var margin = _margin;
        Rectangle outerBounds = new(newBounds);
        outerBounds.X -= margin.Left;
        outerBounds.Y -= margin.Top;
        outerBounds.Width += margin.Left + margin.Right;
        outerBounds.Height += margin.Top + margin.Bottom;
        _outerBounds = outerBounds;

        OnBoundsChanged(oldBounds, newBounds);

        if (oldBounds.Position != newBounds.Position)
        {
            OnPositionChanged(oldBounds.Position, newBounds.Position);
            PositionChanged?.Invoke(
                this,
                new ValueChangedEventArgs<Point>
                {
                    Value = newBounds.Position,
                    OldValue = oldBounds.Position,
                }
            );
        }

        if (oldBounds.Size != newBounds.Size)
        {
            if (_cacheToTexture)
            {
                Skin.Renderer.Ctt.DisposeCachedTexture(this);
                Redraw();
            }

            OnSizeChanged(oldBounds.Size, newBounds.Size);
            SizeChanged?.Invoke(
                this,
                new ValueChangedEventArgs<Point>
                {
                    Value = newBounds.Size,
                    OldValue = oldBounds.Size,
                }
            );
            ProcessAlignments();
        }

        if (_dock != Pos.None)
        {
            InvalidateDock();
            InvalidateParentDock();
        }

        BoundsChanged?.Invoke(
            this,
            new ValueChangedEventArgs<Rectangle>
            {
                Value = newBounds,
                OldValue = oldBounds,
            }
        );

        return true;
    }

    public void InvalidateDock()
    {
        if (!_dockDirty)
        {
            _dockDirty = true;
        }

        Invalidate();
    }

    protected void InvalidateParentDock() => _parent?.InvalidateDock();

    /// <summary>
    /// Positions the control inside its parent.
    /// </summary>
    /// <param name="pos">Target position.</param>
    /// <param name="xPadding">X padding.</param>
    /// <param name="yPadding">Y padding.</param>
    public virtual void Position(Pos pos, int xPadding = 0, int yPadding = 0)
    {
        // Cache frequently used values.
        int parentWidth = Parent.Width;
        int parentHeight = Parent.Height;
        Padding padding = Parent.Padding;

        // Calculate the new X and Y positions using bitwise operations.
        int x = X;
        int y = Y;
        if ((pos & Pos.Left) != 0)
        {
            x = padding.Left + xPadding;
        }
        else if ((pos & Pos.Right) != 0)
        {
            x = parentWidth - Width - padding.Right - xPadding;
        }
        else if ((pos & Pos.CenterH) != 0)
        {
            x = (int)(padding.Left + xPadding + (parentWidth - Width - padding.Left - padding.Right) * 0.5f);
        }

        if ((pos & Pos.Top) != 0)
        {
            y = padding.Top + yPadding;
        }
        else if ((pos & Pos.Bottom) != 0)
        {
            y = parentHeight - Height - padding.Bottom - yPadding;
        }
        else if ((pos & Pos.CenterV) != 0)
        {
            y = (int)(padding.Top + yPadding + (parentHeight - Height - padding.Bottom - padding.Top) * 0.5f);
        }

        // Set the new position.
        SetPosition(x, y);
    }

    protected virtual void OnPositionChanged(Point oldPosition, Point newPosition)
    {
    }

    /// <summary>
    ///     Handler invoked when control's bounds change.
    /// </summary>
    /// <param name="oldBounds">Old bounds.</param>
    /// <param name="newBounds"></param>
    protected virtual void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        Parent?.OnChildBoundsChanged(this, oldBounds, newBounds);

        if (_bounds.Width != oldBounds.Width || _bounds.Height != oldBounds.Height)
        {
            Invalidate();
        }

        Redraw();
        UpdateRenderBounds();
    }

    /// <summary>
    ///     Handler invoked when control's scale changes.
    /// </summary>
    protected virtual void OnScaleChanged()
    {
        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].OnScaleChanged();
        }
    }

    /// <summary>
    ///     Handler invoked when a child component's bounds change.
    /// </summary>
    protected virtual void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        if ((child.Dock & ~Pos.Fill) != 0)
        {
            InvalidateDock();
        }
    }

    /// <summary>
    ///     Handler invoked when child component's size changes.
    /// </summary>
    protected virtual void OnChildSizeChanged(Base child, Point oldChildSize, Point newChildSize)
    {
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void Render(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Renders the control to a cache using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    /// <param name="master">Root parent.</param>
    protected virtual void DoCacheRender(Skin.Base skin, Base master)
    {
        var renderer = skin?.Renderer;
        var cache = renderer?.Ctt;

        // Return early if the cache is not available
        if (cache == null)
        {
            return;
        }

        // Save current render offset and clip region
        var oldRenderOffset = renderer.RenderOffset;
        var oldRegion = renderer.ClipRegion;

        // Set render offset and clip region based on whether this control is the root parent
        if (this != master)
        {
            renderer.AddRenderOffset(Bounds);
            renderer.AddClipRegion(Bounds);
        }
        else
        {
            renderer.RenderOffset = Point.Empty;
            renderer.ClipRegion = new Rectangle(0, 0, Width, Height);
        }

        // Render the control and its children if the cache is dirty and the clip region is visible
        if (_cacheTextureDirty && renderer.ClipRegionVisible)
        {
            if (ClipContents)
            {
                renderer.StartClip();
            }
            else
            {
                renderer.EndClip();
            }

            // Setup the cache texture if necessary
            if (ShouldCacheToTexture)
            {
                cache.SetupCacheTexture(this);
            }

            // Render the control
            Render(skin);

            var childrenToRender = OrderChildrenForRendering(_children.Where(IsNodeVisible));

            foreach (var child in childrenToRender)
            {
                child.DoCacheRender(skin, master);
            }

            // Finish the cache texture if necessary
            if (ShouldCacheToTexture)
            {
                cache.FinishCacheTexture(this);
                _cacheTextureDirty = false;
            }
        }

        // Restore the original clip region and render offset
        renderer.ClipRegion = oldRegion;
        renderer.StartClip();
        renderer.RenderOffset = oldRenderOffset;

        // Draw the cached control texture if necessary
        if (ShouldCacheToTexture)
        {
            cache.DrawCachedControlTexture(this);
        }
    }

    private static bool IsNodeVisible(Base node) => node.IsVisibleInTree;

    /// <summary>
    ///     Rendering logic implementation.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    internal virtual void DoRender(Skin.Base skin)
    {
        // If this control has a different skin,
        // then so does its children.
        if (_skin != null)
        {
            skin = _skin;
        }

        // Do think
        Think();

        UpdateColors();

        var render = skin.Renderer;

        if (render.Ctt != null && ShouldCacheToTexture)
        {
            DoCacheRender(skin, this);
            if (DrawDebugOutlines)
            {
                RenderDebugOutlinesRecursive(skin);
            }
        }
        else
        {
            RenderRecursive(skin, Bounds);
            if (DrawDebugOutlines)
            {
                skin.DrawDebugOutlines(this);
            }
        }
    }

    internal virtual void RenderDebugOutlinesRecursive(Skin.Base skin)
    {
        var oldRenderOffset = skin.Renderer.RenderOffset;
        skin.Renderer.AddRenderOffset(Bounds);
        foreach (var child in _children)
        {
            if (child.IsHidden)
            {
                continue;
            }

            child.RenderDebugOutlinesRecursive(skin);
        }
        skin.Renderer.RenderOffset = oldRenderOffset;

        skin.DrawDebugOutlines(this);
    }

    protected virtual Base[] OrderChildrenForRendering(IEnumerable<Base> children)
    {
        return children as Base[] ?? children.ToArray();
    }

    protected virtual void OnPostDraw(Skin.Base skin)
    {
    }

    protected virtual void OnPreDraw(Skin.Base skin)
    {
    }

    public void SkipRender() => _skipRender = true;

    /// <summary>
    ///     Recursive rendering logic.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    /// <param name="clipRect">Clipping rectangle.</param>
    protected virtual void RenderRecursive(Skin.Base skin, Rectangle clipRect)
    {
        if (_skipRender)
        {
            _skipRender = false;
            return;
        }

        OnPreDraw(skin);
        BeforeDraw?.Invoke(this, EventArgs.Empty);

        var render = skin.Renderer;
        var oldRenderOffset = render.RenderOffset;

        render.AddRenderOffset(clipRect);

        RenderUnder(skin);

        var oldRegion = render.ClipRegion;

        if (ShouldClip)
        {
            render.AddClipRegion(clipRect);

            if (!render.ClipRegionVisible)
            {
                render.RenderOffset = oldRenderOffset;
                render.ClipRegion = oldRegion;

                return;
            }

            render.StartClip();
        }

        //Render myself first
        Render(skin);

        var childrenToRender = OrderChildrenForRendering(_children.Where(IsNodeVisible));

        foreach (var child in childrenToRender)
        {
            if (child is Label label && (label.Text?.Contains("resources/gui/mainmenu_buttonregister.png") ?? false))
            {
                label.ToString();
            }

            child.DoRender(skin);
        }

        render.ClipRegion = oldRegion;
        render.StartClip();
        RenderOver(skin);

        RenderFocus(skin);

        render.RenderOffset = oldRenderOffset;

        OnPostDraw(skin);
        AfterDraw?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Sets the control's skin.
    /// </summary>
    /// <param name="skin">New skin.</param>
    /// <param name="doChildren">Deterines whether to change children skin.</param>
    public virtual void SetSkin(Skin.Base skin, bool doChildren = false)
    {
        if (_skin == skin)
        {
            return;
        }

        _skin = skin;
        Invalidate();
        Redraw();
        OnSkinChanged(skin);

        if (doChildren)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].SetSkin(skin, true);
            }
        }
    }

    /// <summary>
    ///     Handler invoked when control's skin changes.
    /// </summary>
    /// <param name="newSkin">New skin.</param>
    protected virtual void OnSkinChanged(Skin.Base newSkin)
    {
    }

    /// <summary>
    ///     Handler invoked on mouse wheel event.
    /// </summary>
    /// <param name="delta">Scroll delta.</param>
    protected virtual bool OnMouseWheeled(int delta)
    {
        if (_parent != null)
        {
            return _parent.OnMouseWheeled(delta);
        }

        return false;
    }

    /// <summary>
    ///     Invokes mouse wheeled event (used by input system).
    /// </summary>
    internal bool InputMouseWheeled(int delta)
    {
        if (IsDisabledByTree)
        {
            return false;
        }

        return OnMouseWheeled(delta);
    }

    /// <summary>
    ///     Handler invoked on mouse wheel event.
    /// </summary>
    /// <param name="delta">Scroll delta.</param>
    protected virtual bool OnMouseHWheeled(int delta)
    {
        if (_parent != null)
        {
            return _parent.OnMouseHWheeled(delta);
        }

        return false;
    }

    /// <summary>
    ///     Invokes mouse wheeled event (used by input system).
    /// </summary>
    internal bool InputMouseHWheeled(int delta)
    {
        if (IsDisabledByTree)
        {
            return false;
        }

        return OnMouseHWheeled(delta);
    }

    /// <summary>
    ///     Handler invoked on mouse moved event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="dx">X change.</param>
    /// <param name="dy">Y change.</param>
    protected virtual void OnMouseMoved(int x, int y, int dx, int dy)
    {
    }

    /// <summary>
    ///     Invokes mouse moved event (used by input system).
    /// </summary>
    internal void InputMouseMoved(int x, int y, int dx, int dy)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        OnMouseMoved(x, y, dx, dy);
    }

    internal void InputNonUserMouseClicked(MouseButton mouseButton, Point mousePosition, bool isPressed)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        OnMouseDoubleClicked(mouseButton, mousePosition, userAction: false);
        Clicked?.Invoke(this, new MouseButtonState(mouseButton, mousePosition, isPressed: isPressed));
    }

    internal void InputMouseDoubleClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        OnMouseDoubleClicked(mouseButton, mousePosition, userAction);
        DoubleClicked?.Invoke(this, new MouseButtonState(mouseButton, mousePosition, true));
    }

    protected bool PlaySound(string? name)
    {
        if (name == null || this.IsDisabled)
        {
            return false;
        }

        if (Canvas is not { } canvas)
        {
            return false;
        }

        name = GameContentManager.RemoveExtension(name).ToLower();
        if (GameContentManager.Current.GetSound(name) is not { } sound)
        {
            return false;
        }

        if (sound.CreateInstance() is not { } soundInstance)
        {
            return false;
        }

        canvas.PlayAndAddSound(soundInstance);
        return true;
    }

    public bool IsActive
    {
        get => IsMouseButtonActive(MouseButton.Left);
        set
        {
            if (value)
            {
                _mouseButtonPressed.Add(MouseButton.Left);
            }
            else
            {
                _mouseButtonPressed.Remove(MouseButton.Left);
            }
        }
    }

    public virtual ComponentState ComponentState
    {
        get
        {
            var componentState = ComponentState.Normal;
            if (IsDisabledByTree)
            {
                componentState = ComponentState.Disabled;
            }
            else if (IsActive)
            {
                componentState = ComponentState.Active;
            }
            else if (IsHovered)
            {
                componentState = ComponentState.Hovered;
            }

            return componentState;
        }
    }

    public bool IsMouseButtonActive(MouseButton mouseButton) => _mouseButtonPressed.Contains(mouseButton);

    private readonly HashSet<MouseButton> _mouseButtonPressed = [];

    public IReadOnlySet<MouseButton> MouseButtonPressed => _mouseButtonPressed;

    protected virtual void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
    }

    protected virtual void OnMouseDoubleClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
    }

    protected virtual void OnMouseDown(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
    }

    protected virtual void OnMouseUp(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
    }

    public bool KeepFocusOnMouseExit { get; set; }

    internal void InputMouseButtonState(MouseButton mouseButton, Point mousePosition, bool pressed, bool userAction = true)
    {
        var emitsEvents = !IsDisabledByTree;
        var mouseButtonStateArgs = new MouseButtonState(mouseButton, mousePosition.X, mousePosition.Y, pressed);
        var wasActive = IsMouseButtonActive(mouseButton);
        if (pressed)
        {
            _mouseButtonPressed.Add(mouseButton);
            InputHandler.MouseFocus = this;

            if (!wasActive)
            {
                OnMouseDown(mouseButton, mousePosition, userAction);
                if (emitsEvents)
                {
                    MouseDown?.Invoke(this, mouseButtonStateArgs);
                }
            }
        }
        else
        {
            if (IsHovered && wasActive)
            {
                OnMouseClicked(mouseButton, mousePosition, userAction);
                if (emitsEvents)
                {
                    Clicked?.Invoke(this, mouseButtonStateArgs);
                }
            }

            _mouseButtonPressed.Remove(mouseButton);

            if (_mouseButtonPressed.Count < 1)
            {
                // Only replace focus if no mouse buttons are pressed
                // ApplicationContext.CurrentContext.Logger.LogTrace(
                //     "Setting MouseFocus to null from {NodeName}",
                //     CanonicalName
                // );
                InputHandler.MouseFocus = null;
            }

            if (wasActive)
            {
                OnMouseUp(mouseButton, mousePosition, userAction);
                if (emitsEvents)
                {
                    MouseUp?.Invoke(this, mouseButtonStateArgs);
                }
            }
        }

        if (wasActive != pressed)
        {
            Redraw();
        }
    }

    /// <summary>
    ///     Handler invoked on mouse cursor entering control's bounds.
    /// </summary>
    protected virtual void OnMouseEntered()
    {
        HoverEnter?.Invoke(this, EventArgs.Empty);

        Redraw();
    }

    /// <summary>
    ///     Invokes mouse enter event (used by input system).
    /// </summary>
    internal void InputMouseEntered()
    {
        if (Tooltip != null)
        {
            ToolTip.Enable(this);
        }
        else if (Parent is { Tooltip: not null })
        {
            ToolTip.Enable(Parent);
        }

        if (IsDisabledByTree)
        {
            return;
        }

        OnMouseEntered();
    }

    /// <summary>
    ///     Handler invoked on mouse cursor leaving control's bounds.
    /// </summary>
    protected virtual void OnMouseLeft()
    {
        IsActive = false;

        HoverLeave?.Invoke(this, EventArgs.Empty);

        Redraw();
    }

    /// <summary>
    ///     Invokes mouse leave event (used by input system).
    /// </summary>
    internal void InputMouseLeft()
    {
        if (Tooltip != null)
        {
            ToolTip.Disable(this);
        }
        else if (Parent is { Tooltip: not null })
        {
            ToolTip.Disable(Parent);
        }

        if (IsDisabledByTree)
        {
            return;
        }

        OnMouseLeft();

        IsActive = false;
    }

    /// <summary>
    ///     Focuses the control.
    /// </summary>
    public virtual void Focus(bool moveMouse = false)
    {
        if (InputHandler.KeyboardFocus == this)
        {
            return;
        }

        if (InputHandler.KeyboardFocus != null)
        {
            InputHandler.KeyboardFocus.OnLostKeyboardFocus();
        }

        if (moveMouse)
        {
            InputHandler.Focus(FocusSource.Keyboard, this);
        }
        else
        {
            InputHandler.KeyboardFocus = this;
        }

        OnKeyboardFocus();
        Redraw();
    }

    /// <summary>
    ///     Unfocuses the control.
    /// </summary>
    public virtual void Blur()
    {
        if (InputHandler.KeyboardFocus != this)
        {
            return;
        }

        InputHandler.KeyboardFocus = null;
        OnLostKeyboardFocus();
        Redraw();
    }

    /// <summary>
    ///     Control has been clicked - invoked by input system. Windows use it to propagate activation.
    /// </summary>
    public virtual void Touch()
    {
        if (Parent != null)
        {
            Parent.OnChildTouched(this);
        }
    }

    protected virtual void OnChildTouched(Base control)
    {
        Touch();
    }

    /// <summary>
    ///     Gets a child by its coordinates.
    /// </summary>
    /// <param name="x">The local X coordinate to check.</param>
    /// <param name="y">The local Y coordinate to check.</param>
    /// <param name="filters"></param>
    /// <returns>Control or null if not found.</returns>
    public Base? GetComponentAt(int x, int y, NodeFilter filters = default)
    {
        // If it's out of our bounds, return null
        if (x < 0 || Width <= x || y < 0 || Height <= y)
        {
            return null;
        }

        // If we and/or an ancestor are hidden, return null if we aren't explicitly allowing hidden components
        if (IsHidden)
        {
            if (!filters.HasFlag(NodeFilter.IncludeHidden))
            {
                return null;
            }
        }

        var possibleNode = RunOnMainThread(FindComponentAt, new ComponentAtParams(new Point(x, y), filters), invalidate: false);
        if (possibleNode is not null)
        {
            return possibleNode;
        }

        if (this is Text)
        {
            return filters.HasFlag(NodeFilter.IncludeText) ? this : null;
        }

        // By default, we only return components that are mouse-input enabled, but if the filters include
        // those components explicitly, we can return them too. This is particularly useful for debugging.
        if (MouseInputEnabled || filters.HasFlag(NodeFilter.IncludeMouseInputDisabled))
        {
            return this;
        }

        return null;
    }

    private record struct ComponentAtParams(Point Position, NodeFilter Filters);

    private static Base? FindComponentAt(Base @this, ComponentAtParams componentAtParams)
    {
        var ((x, y), filters) = componentAtParams;
        // Check children in reverse order (last added first).
        for (int childIndex = @this._children.Count - 1; childIndex >= 0; childIndex--)
        {
            var child = @this._children[childIndex];
            if (child.GetComponentAt(x - child.X, y - child.Y, filters) is { } descendant)
            {
                return descendant;
            }
        }

        return null;
    }

    public Base? GetComponentAt(Point point, NodeFilter filters = default) => GetComponentAt(point.X, point.Y, filters);

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void Layout(Skin.Base skin)
    {
        UpdateColors();

        if (skin?.Renderer.Ctt != null && ShouldCacheToTexture)
        {
            skin.Renderer.Ctt.CreateControlCacheTexture(this);
        }
    }

    protected virtual bool ShouldSkipLayout => !(_visible || ToolTip.IsActiveTooltip(this));

    public int NodeCount => 1 + _children.Sum(child => child.NodeCount);

    protected virtual void DoPrelayout(Skin.Base skin)
    {

    }

    protected void DoLayoutIfNeeded(Skin.Base skin)
    {
        if (!_layoutDirty)
        {
            return;
        }

        _layoutDirty = false;
        Layout(skin);
    }

    private Point _requiredSizeForDockFillNodes;

    protected void InvokeThreadQueue()
    {
        if (_pendingThreadQueues < 1)
        {
            return;
        }

        if (_threadQueue.InvokePending())
        {
            UpdatePendingThreadQueues(-1);
        }

        var count = _children.Count;
        for (var index = 0; index < count; index++)
        {
            var child = _children[index];
            child.InvokeThreadQueue();
        }
    }

    /// <summary>
    ///     Recursively lays out the control's interior according to alignment, margin, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected void RecurseLayout(Skin.Base skin)
    {
        if (_skin != null)
        {
            skin = _skin;
        }

        if (ShouldSkipLayout)
        {
            return;
        }

        foreach (var child in _children)
        {
            child.DoPrelayout(skin);

            _preLayoutActionsParent.IsExecuting = true;
            PreLayout.InvokePending();
            _preLayoutActionsParent.IsExecuting = false;

            child.BeforeLayout?.Invoke(child, EventArgs.Empty);
        }

        var expectedSize = new Point(Math.Max(Width, MinimumSize.X), Math.Max(Height, MinimumSize.Y));
        if (expectedSize != Size)
        {
            Size = expectedSize;
        }

        DoLayoutIfNeeded(skin);

        if (_needsAlignment)
        {
            _needsAlignment = false;
            ProcessAlignments();
        }

        if (_dockDirty)
        {
            _dockDirty = false;

            var remainingBounds = RenderBounds;

            // Adjust bounds for padding
            remainingBounds.X += _padding.Left;
            remainingBounds.Width -= _padding.Left + _padding.Right;
            remainingBounds.Y += _padding.Top;
            remainingBounds.Height -= _padding.Top + _padding.Bottom;

            var dockChildSpacing = DockChildSpacing;

            var directionalDockChildren =
                _children.Where(child => !child.ShouldSkipLayout && !child.Dock.HasFlag(Pos.Fill)).ToArray();
            var dockFillChildren =
                _children.Where(child => !child.ShouldSkipLayout && child.Dock.HasFlag(Pos.Fill)).ToArray();

            foreach (var child in directionalDockChildren)
            {
                var childDock = child.Dock;

                var childMargin = child.Margin;
                var childMarginH = childMargin.Left + childMargin.Right;
                var childMarginV = childMargin.Top + childMargin.Bottom;

                var childOuterWidth = childMarginH + child.Width;
                var childOuterHeight = childMarginV + child.Height;

                var availableWidth = remainingBounds.Width - childMarginH;
                var availableHeight = remainingBounds.Height - childMarginV;

                var childFitsContentWidth = false;
                var childFitsContentHeight = false;

                switch (child)
                {
                    case ISmartAutoSizeToContents smartAutoSizeToContents:
                        childFitsContentWidth = smartAutoSizeToContents.AutoSizeToContentWidth;
                        childFitsContentHeight = smartAutoSizeToContents.AutoSizeToContentHeight;
                        break;
                    case IAutoSizeToContents { AutoSizeToContents: true }:
                        childFitsContentWidth = true;
                        childFitsContentHeight = true;
                        break;
                    case IFitHeightToContents { FitHeightToContents: true }:
                        childFitsContentHeight = true;
                        break;
                }

                if (childDock.HasFlag(Pos.Left))
                {
                    var height = childFitsContentHeight
                        ? child.Height
                        : availableHeight;

                    var y = remainingBounds.Y + childMargin.Top;
                    if (childDock.HasFlag(Pos.CenterV))
                    {
                        height = child.Height;
                        var extraY = Math.Max(0, availableHeight - height) / 2;
                        if (extraY != 0)
                        {
                            y += extraY;
                        }
                    }
                    else if (childDock.HasFlag(Pos.Bottom))
                    {
                        y = remainingBounds.Bottom - (childMargin.Bottom + child.Height);
                    }
                    else if (!childDock.HasFlag(Pos.Top))
                    {
                        var extraY = Math.Max(0, availableHeight - height) / 2;
                        if (extraY != 0)
                        {
                            y += extraY;
                        }
                    }

                    child.SetBounds(
                        remainingBounds.X + childMargin.Left,
                        y,
                        child.Width,
                        height
                    );

                    var boundsDeltaX = childOuterWidth + dockChildSpacing.Left;
                    remainingBounds.X += boundsDeltaX;
                    remainingBounds.Width -= boundsDeltaX;
                }

                if (childDock.HasFlag(Pos.Right))
                {
                    var height = childFitsContentHeight
                        ? child.Height
                        : availableHeight;

                    var y = remainingBounds.Y + childMargin.Top;
                    if (childDock.HasFlag(Pos.CenterV))
                    {
                        height = child.Height;
                        var extraY = Math.Max(0, availableHeight - height) / 2;
                        if (extraY != 0)
                        {
                            y += extraY;
                        }
                    }
                    else if (childDock.HasFlag(Pos.Bottom))
                    {
                        y = remainingBounds.Bottom - (childMargin.Bottom + child.Height);
                    }
                    else if (!childDock.HasFlag(Pos.Top))
                    {
                        var extraY = Math.Max(0, availableHeight - height) / 2;
                        if (extraY != 0)
                        {
                            y += extraY;
                        }
                    }

                    var offsetFromRight = child.Width + childMargin.Right;
                    child.SetBounds(
                        remainingBounds.X + remainingBounds.Width - offsetFromRight,
                        y,
                        child.Width,
                        height
                    );

                    var boundsDeltaX = childOuterWidth + dockChildSpacing.Right;
                    remainingBounds.Width -= boundsDeltaX;
                }

                if (childDock.HasFlag(Pos.Top) && !childDock.HasFlag(Pos.Left) && !childDock.HasFlag(Pos.Right))
                {
                    var width = childFitsContentWidth
                        ? child.Width
                        : availableWidth;

                    var x = remainingBounds.Left + childMargin.Left;

                    if (childDock.HasFlag(Pos.CenterH))
                    {
                        x = remainingBounds.Left + (remainingBounds.Width - child.OuterWidth) / 2;
                        width = child.Width;
                    }

                    child.SetBounds(
                        x,
                        remainingBounds.Top + childMargin.Top,
                        width,
                        child.Height
                    );

                    var boundsDeltaY = childOuterHeight + dockChildSpacing.Top;
                    remainingBounds.Y += boundsDeltaY;
                    remainingBounds.Height -= boundsDeltaY;
                }

                if (childDock.HasFlag(Pos.Bottom) && !childDock.HasFlag(Pos.Left) && !childDock.HasFlag(Pos.Right))
                {
                    var width = childFitsContentWidth
                        ? child.Width
                        : availableWidth;

                    var offsetFromBottom = child.Height + childMargin.Bottom;
                    var x = remainingBounds.Left + childMargin.Left;

                    if (childDock.HasFlag(Pos.CenterH))
                    {
                        x = remainingBounds.Left + (remainingBounds.Width - child.OuterWidth) / 2;
                        width = child.Width;
                    }

                    child.SetBounds(
                        x,
                        remainingBounds.Bottom - offsetFromBottom,
                        width,
                        child.Height
                    );

                    remainingBounds.Height -= childOuterHeight + dockChildSpacing.Bottom;
                }

                child.RecurseLayout(skin);
            }

            var boundsForFillNodes = remainingBounds;
            _innerBounds = remainingBounds;

            Point sizeToFitDockFillNodes = default;

            var largestDockFillSize = dockFillChildren.Aggregate(
                default(Point),
                (size, node) =>
                    new Point(Math.Max(size.X, node.Width), Math.Max(size.Y, node.Height))
            );

            int suggestedWidth, suggestedHeight;
            if (dockFillChildren.Length < 2)
            {
                suggestedWidth = remainingBounds.Width;
                suggestedHeight = remainingBounds.Height;
            }
            else if (largestDockFillSize.Y > largestDockFillSize.X)
            {
                if (largestDockFillSize.Y > remainingBounds.Height)
                {
                    suggestedWidth = Math.Max(largestDockFillSize.X, remainingBounds.Width);
                    suggestedHeight = remainingBounds.Height / dockFillChildren.Length;
                }
                else
                {
                    suggestedWidth = remainingBounds.Width / dockFillChildren.Length;
                    suggestedHeight = Math.Max(largestDockFillSize.Y, remainingBounds.Height);
                }
            }
            else if (largestDockFillSize.X > remainingBounds.Width)
            {
                suggestedWidth = remainingBounds.Width / dockFillChildren.Length;
                suggestedHeight = Math.Max(largestDockFillSize.Y, remainingBounds.Height);
            }
            else
            {
                suggestedWidth = Math.Max(largestDockFillSize.X, remainingBounds.Width);
                suggestedHeight = remainingBounds.Height / dockFillChildren.Length;
            }

            var fillHorizontal = suggestedHeight == remainingBounds.Height;

            //
            // Fill uses the left over space, so do that now.
            //
            foreach (var child in dockFillChildren)
            {
                var dock = child.Dock;

                var childMargin = child.Margin;
                var childMarginH = childMargin.Left + childMargin.Right;
                var childMarginV = childMargin.Top + childMargin.Bottom;

                Point newPosition = new(
                    remainingBounds.X + childMargin.Left,
                    remainingBounds.Y + childMargin.Top
                );

                Point newSize = new(
                    suggestedWidth - childMarginH,
                    suggestedHeight - childMarginV
                );

                var childMinimumSize = child.MinimumSize;
                var neededX = Math.Max(0, childMinimumSize.X - newSize.X);
                var neededY = Math.Max(0, childMinimumSize.Y - newSize.Y);

                bool exhaustSize = false;
                if (neededX > 0 || neededY > 0)
                {
                    exhaustSize = true;

                    if (sizeToFitDockFillNodes == default)
                    {
                        sizeToFitDockFillNodes = Size;
                    }

                    sizeToFitDockFillNodes.X += neededX;
                    sizeToFitDockFillNodes.Y += neededY;
                }
                else if (remainingBounds.Width < 1 || remainingBounds.Height < 1)
                {
                    if (sizeToFitDockFillNodes == default)
                    {
                        sizeToFitDockFillNodes = Size;
                    }

                    sizeToFitDockFillNodes.X += Math.Max(10, boundsForFillNodes.Width / dockFillChildren.Length);
                    sizeToFitDockFillNodes.Y += Math.Max(10, boundsForFillNodes.Height / dockFillChildren.Length);
                }

                newSize.X = Math.Max(childMinimumSize.X, newSize.X);
                newSize.Y = Math.Max(childMinimumSize.Y, newSize.Y);

                if (child is IAutoSizeToContents { AutoSizeToContents: true })
                {
                    if (Pos.Right == (dock & (Pos.Right | Pos.Left)))
                    {
                        var offsetFromRight = child.Width + childMargin.Right + dockChildSpacing.Right;
                        newPosition.X = remainingBounds.Right - offsetFromRight;
                    }

                    if (Pos.Bottom == (dock & (Pos.Bottom | Pos.Top)))
                    {
                        var offsetFromBottom = child.Height + childMargin.Bottom + dockChildSpacing.Bottom;
                        newPosition.Y = remainingBounds.Bottom - offsetFromBottom;
                    }

                    if (dock.HasFlag(Pos.CenterH))
                    {
                        newPosition.X = remainingBounds.X + (remainingBounds.Width - (childMarginH + child.Width)) / 2;
                    }

                    if (dock.HasFlag(Pos.CenterV))
                    {
                        newPosition.Y = remainingBounds.Y +
                                        (remainingBounds.Height - (childMarginV + child.Height)) / 2;
                    }

                    child.SetPosition(newPosition);

                    // TODO: Figure out how to adjust remaining bounds in the autosize case
                }
                else
                {
                    ApplyDockFill(child, newPosition, newSize);

                    var childOuterBounds = child.OuterBounds;
                    if (fillHorizontal)
                    {
                        var delta = childOuterBounds.Right - remainingBounds.X;
                        remainingBounds.X = childOuterBounds.Right;
                        remainingBounds.Width -= delta;
                    }
                    else
                    {
                        var delta = childOuterBounds.Bottom - remainingBounds.Y;
                        remainingBounds.Y = childOuterBounds.Bottom;
                        remainingBounds.Height -= delta;
                    }
                }

                if (exhaustSize)
                {
                    remainingBounds.X += remainingBounds.Width;
                    remainingBounds.Width = 0;
                    remainingBounds.Y += remainingBounds.Height;
                    remainingBounds.Height = 0;
                }

                child.RecurseLayout(skin);
            }
        }
        else
        {
            foreach (var child in _children)
            {
                if (child.ShouldSkipLayout)
                {
                    continue;
                }

                child.RecurseLayout(skin);
            }
        }

        DoPostlayout(skin);

        _postLayoutActionsParent.IsExecuting = true;
        PostLayout.InvokePending();
        _postLayoutActionsParent.IsExecuting = false;

        AfterLayout?.Invoke(this, EventArgs.Empty);

        // ReSharper disable once InvertIf
        if (_canvas is { } canvas)
        {
            if (IsTabable && !canvas._tabQueue.Contains(this))
            {
                canvas._tabQueue.AddLast(this);
            }

            if (InputHandler.KeyboardFocus == this && !canvas._tabQueue.Contains(this))
            {
                canvas._tabQueue.AddLast(this);
            }
        }
    }

    protected Point _dockFillSize;

    protected virtual void ApplyDockFill(Base child, Point position, Point size)
    {
        child._dockFillSize = size;
        child.SetBounds(position, size);
    }

    /// <summary>
    ///     Checks if the given control is a child of this instance.
    /// </summary>
    /// <param name="child">Control to examine.</param>
    /// <returns>True if the control is out child.</returns>
    public bool IsChild(Base child)
    {
        return _children.Contains(child);
    }

    public Point ToCanvas(int x, int y)
    {
        if (_host is not { } parent)
        {
            return new Point(x, y);
        }

        x += X;
        y += Y;

        // ReSharper disable once InvertIf
        if (parent._innerPanel is { } innerPanel && innerPanel.IsChild(this))
        {
            x += innerPanel.X;
            y += innerPanel.Y;
        }

        return _host.ToCanvas(x, y);
    }

    /// <summary>
    ///     Converts local coordinates to canvas coordinates.
    /// </summary>
    /// <param name="point">Local coordinates.</param>
    /// <returns>Canvas coordinates.</returns>
    public Point ToCanvas(Point point) => ToCanvas(point.X, point.Y);

    /// <summary>
    ///     Converts canvas coordinates to local coordinates.
    /// </summary>
    /// <param name="globalCoordinates">Canvas coordinates.</param>
    /// <returns>Local coordinates.</returns>
    public virtual Point CanvasPosToLocal(Point globalCoordinates) => ToLocal(globalCoordinates.X, globalCoordinates.Y);

    public virtual bool HasChild(Base component) => IsChild(component);

    public Point ToGlobal(Point point) => ToGlobal(point.X, point.Y);

    public virtual Point ToGlobal(int x, int y)
    {
        if (_host is not { } parent)
        {
            return new Point(x, y);
        }

        x += X;
        y += Y;

        if (parent._innerPanel is not { } innerPanel || !innerPanel.HasChild(this))
        {
            return parent.ToGlobal(x, y);
        }

        return innerPanel.ToGlobal(x, y);
    }

    public Point ToLocal(Point point) => ToLocal(point.X, point.Y);

    public virtual Point ToLocal(int x, int y)
    {
        if (_host is not {} parent)
        {
            return new Point(x, y);
        }

        x -= X;
        y -= Y;

        if (parent._innerPanel is not { } innerPanel || !innerPanel.HasChild(this))
        {
            return parent.ToLocal(x, y);
        }

        return innerPanel.ToLocal(x, y);
    }

    /// <summary>
    ///     Closes all menus recursively.
    /// </summary>
    public virtual void CloseMenus()
    {
        ////debug.print("Base.CloseMenus: {0}", this);

        // todo: not very efficient with the copying and recursive closing, maybe store currently open menus somewhere (canvas)?
        var childrenCopy = _children.FindAll(x => true);
        foreach (var child in childrenCopy)
        {
            child.CloseMenus();
        }
    }

    /// <summary>
    ///     Copies Bounds to RenderBounds.
    /// </summary>
    protected virtual void UpdateRenderBounds()
    {
        _renderBounds.X = 0;
        _renderBounds.Y = 0;

        _renderBounds.Width = _bounds.Width;
        _renderBounds.Height = _bounds.Height;
    }

    /// <summary>
    ///     Sets mouse cursor to current cursor.
    /// </summary>
    public virtual void UpdateCursor()
    {
        Platform.Neutral.SetCursor(_cursor);
    }

    // giver
    public virtual Package DragAndDrop_GetPackage(int x, int y)
    {
        return _dragPayload;
    }

    // giver
    public virtual bool DragAndDrop_Draggable()
    {
        if (_dragPayload == null)
        {
            return false;
        }

        return _dragPayload.IsDraggable;
    }

    // giver
    public virtual void DragAndDrop_SetPackage(bool draggable, string name = "", object userData = null)
    {
        if (_dragPayload == null)
        {
            _dragPayload = new Package();
            _dragPayload.IsDraggable = draggable;
            _dragPayload.Name = name;
            _dragPayload.UserData = userData;
        }
    }

    // giver
    public virtual bool DragAndDrop_ShouldStartDrag()
    {
        return true;
    }

    // giver
    public virtual void DragAndDrop_StartDragging(Package package, int x, int y)
    {
        package.HoldOffset = CanvasPosToLocal(new Point(x, y));
        package.DrawControl = this;
    }

    // giver
    public virtual void DragAndDrop_EndDragging(bool success, int x, int y)
    {
    }

    // receiver
    public virtual bool DragAndDrop_HandleDrop(Package p, int x, int y)
    {
        DragAndDrop.SourceControl.Parent = this;

        return true;
    }

    // receiver
    public virtual void DragAndDrop_HoverEnter(Package p, int x, int y)
    {
    }

    // receiver
    public virtual void DragAndDrop_HoverLeave(Package p)
    {
    }

    // receiver
    public virtual void DragAndDrop_Hover(Package p, int x, int y)
    {
    }

    // receiver
    public virtual bool DragAndDrop_CanAcceptPackage(Package p)
    {
        return false;
    }

    public record struct SizeToChildrenArgs(
        bool X = true,
        bool Y = true,
        bool Recurse = false,
        Point MinimumSize = default
    );

    private static void SizeChildrenToChildren(Base @this, SizeToChildrenArgs args)
    {
        foreach (var child in @this._children)
        {
            if (!child._visible)
            {
                continue;
            }

            child.SizeToChildren(args);
        }
    }

    public bool SizeToChildren(bool resizeX = true, bool resizeY = true, bool recursive = false) =>
        SizeToChildren(new SizeToChildrenArgs(resizeX, resizeY, recursive));

    /// <summary>
    ///     Resizes the control to fit its children.
    /// </summary>
    /// <param name="args"></param>
    /// <returns>True if bounds changed.</returns>
    public virtual bool SizeToChildren(SizeToChildrenArgs args)
    {
        if (args is { X: false, Y: false })
        {
            return false;
        }

        if (args.Recurse)
        {
            RunOnMainThread(SizeChildrenToChildren, this, args);
        }

        var childrenSize = GetChildrenSize();
        childrenSize.X = Math.Max(childrenSize.X, args.MinimumSize.X);
        childrenSize.Y = Math.Max(childrenSize.Y, args.MinimumSize.Y);

        var padding = Padding;
        var size = childrenSize;
        var paddingH = padding.Right + padding.Left;
        var paddingV = padding.Bottom + padding.Top;

        size.X = size.X < 0 ? Width : (size.X + paddingH);
        size.Y = size.Y < 0 ? Height : (size.Y + paddingV);

        if (_requiredSizeForDockFillNodes != default)
        {
            size.X = Math.Max(_requiredSizeForDockFillNodes.X, size.X);
            size.Y = Math.Max(_requiredSizeForDockFillNodes.Y, size.Y);
            _requiredSizeForDockFillNodes = default;
        }

        size.X = Math.Max(Math.Min(size.X, _maximumSize.X < 1 ? size.X : _maximumSize.X), _minimumSize.X);
        size.Y = Math.Max(Math.Min(size.Y, _maximumSize.Y < 1 ? size.Y : _maximumSize.Y), _minimumSize.Y);

        var newSize = new Point(args.X ? size.X : Width, args.Y ? size.Y : Height);

        if (Dock.HasFlag(Pos.Fill))
        {
            var dockFillSize = ApplyDockFillOnSizeToChildren(newSize, size);
            newSize = dockFillSize;
        }

        if (!SetSize(newSize))
        {
            return false;
        }

        ProcessAlignments();

        return true;
    }

    protected virtual Point ApplyDockFillOnSizeToChildren(Point size, Point internalSize)
    {
        return new Point(
            Math.Max(Width, size.X),
            Math.Max(Height, size.Y)
        );
    }

    /// <summary>
    ///     Returns the total width and height of all children.
    /// </summary>
    /// <remarks>
    ///     Default implementation InvalidateAlignmentreturns maximum size of children since the layout is unknown.
    ///     Implement this in derived compound controls to properly return their size.
    /// </remarks>
    /// <returns></returns>
    public virtual Point GetChildrenSize() => RunOnMainThread(GetChildrenSize);

    private static Point GetChildrenSize(Base @this)
    {
        Point min = new(int.MaxValue, int.MaxValue);
        Point max = default;

        foreach (var child in @this._children)
        {
            if (!child._visible)
            {
                continue;
            }

            var childBounds = child.OuterBounds;
            min.X = Math.Min(min.X, childBounds.Left);
            min.Y = Math.Min(min.Y, childBounds.Top);
            max.X = Math.Max(max.X, childBounds.Right);
            max.Y = Math.Max(max.Y, childBounds.Bottom);
        }

        var delta = max - min;
        return delta;
    }

    public virtual Point MeasureContent()
    {
        var contentSize = GetChildrenSize();
        contentSize.X += Padding.Left + Padding.Right;
        contentSize.Y += Padding.Top + Padding.Bottom;

        // ReSharper disable once InvertIf
        if (_innerPanel is { } innerPanel)
        {
            contentSize.X += innerPanel.Padding.Left + innerPanel.Padding.Right;
            contentSize.Y += innerPanel.Padding.Top + innerPanel.Padding.Bottom;
        }

        return contentSize;
    }

    /// <summary>
    ///     Handles keyboard accelerator.
    /// </summary>
    /// <param name="accelerator">Accelerator text.</param>
    /// <returns>True if handled.</returns>
    internal virtual bool HandleAccelerator(string accelerator)
    {
        if (InputHandler.KeyboardFocus == this || !AccelOnlyFocus)
        {
            if (_accelerators.ContainsKey(accelerator))
            {
                _accelerators[accelerator].Invoke(this, EventArgs.Empty);

                return true;
            }
        }

        return _children.Any(child => child.HandleAccelerator(accelerator));
    }

    /// <summary>
    ///     Adds keyboard accelerator.
    /// </summary>
    /// <param name="accelerator">Accelerator text.</param>
    /// <param name="handler">Handler.</param>
    public void AddAccelerator(string? accelerator, GwenEventHandler<EventArgs> handler)
    {
        accelerator = accelerator?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(accelerator))
        {
            return;
        }

        _accelerators[accelerator] = handler;
    }

    /// <summary>
    ///     Adds keyboard accelerator with a default handler.
    /// </summary>
    /// <param name="accelerator">Accelerator text.</param>
    public void AddAccelerator(string accelerator)
    {
        _accelerators[accelerator] = DefaultAcceleratorHandler;
    }

    /// <summary>
    ///     Function invoked after layout.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void DoPostlayout(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Re-renders the control, invalidates cached texture.
    /// </summary>
    public virtual void Redraw()
    {
        _cacheTextureDirty = true;
        _host?.Redraw();
    }

    /// <summary>
    ///     Updates control colors.
    /// </summary>
    /// <remarks>
    ///     Used in composite controls like lists to differentiate row colors etc.
    /// </remarks>
    public virtual void UpdateColors()
    {
    }

    /// <summary>
    ///     Invalidates control's parent.
    /// </summary>
    public void InvalidateParent() => _host?.Invalidate(alsoInvalidateParent: true);

    /// <summary>
    ///     Handler for keyboard events.
    /// </summary>
    /// <param name="key">Key pressed.</param>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyPressed(Key key, bool down = true)
    {
        var handled = false;
        switch (key)
        {
            case Key.Tab:
                handled = OnKeyTab(down);

                break;

            case Key.Space:
                handled = OnKeySpace(down);

                break;

            case Key.Home:
                handled = OnKeyHome(down);

                break;

            case Key.End:
                handled = OnKeyEnd(down);

                break;

            case Key.Return:
                handled = OnKeyReturn(down);

                break;

            case Key.Backspace:
                handled = OnKeyBackspace(down);

                break;

            case Key.Delete:
                handled = OnKeyDelete(down);

                break;

            case Key.Right:
                handled = OnKeyRight(down);

                break;

            case Key.Left:
                handled = OnKeyLeft(down);

                break;

            case Key.Up:
                handled = OnKeyUp(down);

                break;

            case Key.Down:
                handled = OnKeyDown(down);

                break;

            case Key.Escape:
                handled = OnKeyEscape(down);

                break;

            case Key.Invalid:
            case Key.Shift:
            case Key.Control:
            case Key.Alt:
            case Key.Count:
            default:
                break;
        }

        if (handled)
        {
            return true;
        }

        Parent?.OnKeyPressed(key, down);

        return false;
    }

    /// <summary>
    ///     Invokes key press event (used by input system).
    /// </summary>
    internal bool InputKeyPressed(Key key, bool down = true)
    {
        if (IsDisabledByTree)
        {
            return false;
        }

        return OnKeyPressed(key, down);
    }

    /// <summary>
    ///     Handler for keyboard events.
    /// </summary>
    /// <param name="key">Key pressed.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyReleaseed(Key key)
    {
        if (IsDisabledByTree)
        {
            return false;
        }

        return OnKeyPressed(key, false);
    }

    /// <summary>
    ///     Handler for Tab keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyTab(bool down, bool shift = false)
    {
        if (!down)
        {
            return true;
        }

        if (_canvas is not { } canvas)
        {
            return true;
        }

        Base? next;
        lock (canvas._tabQueue)
        {
            var hasValid = canvas._tabQueue.Any(IsNodeValidTabTarget);
            if (!hasValid)
            {
                return true;
            }

            var currentFocus = InputHandler.KeyboardFocus;
            var last = (shift ? canvas._tabQueue.First : canvas._tabQueue.Last)?.Value;
            if (currentFocus != default && canvas._tabQueue.Contains(currentFocus) && currentFocus != last)
            {
                while (canvas._tabQueue.Last?.Value != currentFocus)
                {
                    next = canvas._tabQueue.First?.Value;
                    if (next == default)
                    {
                        break;
                    }

                    canvas._tabQueue.RemoveFirst();
                    canvas._tabQueue.AddLast(next);
                }
            }

            do
            {
                next = (shift ? canvas._tabQueue.Last : canvas._tabQueue.First)?.Value;
                if (next == default)
                {
                    return true;
                }

                if (shift)
                {
                    canvas._tabQueue.RemoveLast();
                    canvas._tabQueue.AddFirst(next);
                }
                else
                {
                    canvas._tabQueue.RemoveFirst();
                    canvas._tabQueue.AddLast(next);
                }
            }
            while (!IsNodeValidTabTarget(next));
        }

        if (IsNodeValidTabTarget(next))
        {
            Console.WriteLine($"Focusing {next.ParentQualifiedName} ({next.GetFullishName()})");
            next.Focus(moveMouse: next is not TextBox);
        }

        Redraw();

        return true;
    }

    private static bool IsNodeValidTabTarget(Base? node) =>
        node is { IsDisabledByTree: false, IsHiddenByTree: false, IsTabable: true };

    /// <summary>
    ///     Handler for Space keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeySpace(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Return keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyReturn(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Backspace keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyBackspace(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Delete keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyDelete(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Right Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyRight(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Left Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyLeft(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Home keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyHome(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for End keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyEnd(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyUp(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyDown(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Escape keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyEscape(bool down)
    {
        return false;
    }

    /// <summary>
    ///     Handler for Paste event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected virtual void OnPaste(Base from, EventArgs args)
    {
    }

    /// <summary>
    ///     Handler for Copy event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected virtual void OnCopy(Base from, EventArgs args)
    {
    }

    /// <summary>
    ///     Handler for Cut event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected virtual void OnCut(Base from, EventArgs args)
    {
    }

    /// <summary>
    ///     Handler for Select All event.
    /// </summary>
    /// <param name="from">Source control.</param>
    protected virtual void OnSelectAll(Base from, EventArgs args)
    {
    }

    internal void InputCopy(Base from)
    {
        OnCopy(from, EventArgs.Empty);
    }

    internal void InputPaste(Base from)
    {
        if (IsDisabledByTree)
        {
            return;
        }

        OnPaste(from, EventArgs.Empty);
    }

    internal void InputCut(Base from)
    {
        if (IsDisabledByTree)
        {
            OnCopy(from, EventArgs.Empty);
            return;
        }

        OnCut(from, EventArgs.Empty);
    }

    internal void InputSelectAll(Base from)
    {
        OnSelectAll(from, EventArgs.Empty);
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void RenderFocus(Skin.Base skin)
    {
        if (InputHandler.KeyboardFocus != this)
        {
            return;
        }

        if (!IsTabable)
        {
            return;
        }

        skin.DrawKeyboardHighlight(this, RenderBounds, 3);
    }

    /// <summary>
    ///     Renders under the actual control (shadows etc).
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void RenderUnder(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Renders over the actual control (overlays).
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void RenderOver(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Called during rendering.
    /// </summary>
    public virtual void Think()
    {
    }

    /// <summary>
    ///     Handler for gaining keyboard focus.
    /// </summary>
    protected virtual void OnKeyboardFocus()
    {
    }

    /// <summary>
    ///     Handler for losing keyboard focus.
    /// </summary>
    protected virtual void OnLostKeyboardFocus()
    {
    }

    /// <summary>
    ///     Handler for character input event.
    /// </summary>
    /// <param name="chr">Character typed.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnChar(Char chr)
    {
        return false;
    }

    internal bool InputChar(Char chr)
    {
        if (IsDisabledByTree)
        {
            return false;
        }

        return OnChar(chr);
    }

    public virtual void Anim_WidthIn(float length, float delay = 0.0f, float ease = 1.0f)
    {
        Animation.Add(this, new Anim.Size.Width(0, Width, length, false, delay, ease));
        Width = 0;
    }

    public virtual void Anim_HeightIn(float length, float delay, float ease)
    {
        Animation.Add(this, new Anim.Size.Height(0, Height, length, false, delay, ease));
        Height = 0;
    }

    public virtual void Anim_WidthOut(float length, bool hide, float delay, float ease)
    {
        Animation.Add(this, new Anim.Size.Width(Width, 0, length, hide, delay, ease));
    }

    public virtual void Anim_HeightOut(float length, bool hide, float delay, float ease)
    {
        Animation.Add(this, new Anim.Size.Height(Height, 0, length, hide, delay, ease));
    }

    public void FitChildrenToSize()
    {
        foreach (var child in Children)
        {
            //push them back into view if they are outside it
            child.X = Math.Min(Bounds.Width, child.X + child.Width) - child.Width;
            child.Y = Math.Min(Bounds.Height, child.Y + child.Height) - child.Height;

            //Non-negative has priority, so do it second.
            child.X = Math.Max(0, child.X);
            child.Y = Math.Max(0, child.Y);
        }
    }

    protected bool SetIfChanged<T>(ref T field, T value) => SetIfChanged(ref field, value, out _);

    protected bool SetIfChanged(ref string? field, string? value) => SetIfChanged(ref field, value, out _);

    protected bool SetIfChanged(ref string? field, string? value, StringComparison stringComparison) =>
        SetIfChanged(ref field, value, stringComparison, out _);

    protected bool SetIfChanged<T>(ref T field, T value, out T oldValue)
    {
        oldValue = field;

        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        return true;
    }

    protected bool SetIfChanged(ref string? field, string? value, out string? oldValue) =>
        SetIfChanged(ref field, value, StringComparison.Ordinal, out oldValue);

    protected bool SetIfChanged(ref string? field, string? value, StringComparison stringComparison, out string? oldValue)
    {
        oldValue = field;

        if (string.Equals(field, value, stringComparison))
        {
            return false;
        }

        field = value;
        return true;
    }

    protected bool SetAndDoIfChanged<TValue>(ref TValue field, TValue value, Action<Base> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!SetIfChanged(ref field, value))
        {
            return false;
        }

        action(this);
        return true;
    }

    protected static bool SetAndDoIfChanged<TNode, TValue>(
        ref TValue field,
        TValue value,
        Action<TNode> action,
        TNode @this
    ) where TNode : Base
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!@this.SetIfChanged(ref field, value))
        {
            return false;
        }

        action(@this);
        return true;
    }

    protected static bool SetAndDoIfChanged<TNode, TValue>(
        ref TValue field,
        TValue value,
        Action<Base?, TNode> action,
        Base? parent,
        TNode @this
    ) where TNode : Base
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!@this.SetIfChanged(ref field, value))
        {
            return false;
        }

        action(parent, @this);
        return true;
    }

    protected static bool SetAndDoIfChanged<TNode, TValue>(
        ref TValue field,
        TValue value,
        Action<TNode, TValue> action,
        TNode @this
    ) where TNode : Base
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!@this.SetIfChanged(ref field, value))
        {
            return false;
        }

        action(@this, value);
        return true;
    }

    protected bool SetAndDoIfChanged<TValue>(ref TValue field, TValue value, Action<Base, TValue> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!SetIfChanged(ref field, value))
        {
            return false;
        }

        action(this, value);
        return true;
    }

    protected bool SetAndDoIfChanged(ref string? field, string? value, Action action) =>
        SetAndDoIfChanged(ref field, value, StringComparison.Ordinal, action);

    protected bool SetAndDoIfChanged(ref string? field, string? value, StringComparison stringComparison, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!SetIfChanged(ref field, value, stringComparison))
        {
            return false;
        }

        action();
        return true;

    }

    protected bool SetAndDoIfChanged<T>(ref T field, T value, ValueChangeHandler<T> valueChangeHandler)
    {
        ArgumentNullException.ThrowIfNull(valueChangeHandler);

        if (!SetIfChanged(ref field, value, out var oldValue))
        {
            return false;
        }

        valueChangeHandler(oldValue, value);
        return true;
    }

    protected bool SetAndDoIfChanged(
        ref string? field,
        string value,
        ValueChangeHandler<string?> valueChangeHandler
    ) => SetAndDoIfChanged(ref field, value, StringComparison.Ordinal, valueChangeHandler);

    protected bool SetAndDoIfChanged(
        ref string? field,
        string value,
        StringComparison stringComparison,
        ValueChangeHandler<string?> valueChangeHandler
    )
    {
        ArgumentNullException.ThrowIfNull(valueChangeHandler);

        if (!SetIfChanged(ref field, value, stringComparison, out var oldValue))
        {
            return false;
        }

        valueChangeHandler(oldValue, value);
        return true;
    }
}