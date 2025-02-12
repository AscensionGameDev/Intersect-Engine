using System.Collections.Concurrent;
using System.Diagnostics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Anim;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
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
using Intersect.Framework.Reflection;
using Intersect.Framework.Serialization;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Base control class.
/// </summary>
public partial class Base : IDisposable
{
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

    public delegate void GwenEventHandler<in TArgs>(Base sender, TArgs arguments) where TArgs : EventArgs;

    public delegate void GwenEventHandler<in TSender, in TArgs>(TSender sender, TArgs arguments)
        where TSender : Base where TArgs : EventArgs;

    /// <summary>
    ///     Accelerator map.
    /// </summary>
    private readonly Dictionary<string, GwenEventHandler<EventArgs>> mAccelerators;

    /// <summary>
    ///     Real list of children.
    /// </summary>
    private readonly List<Base> mChildren;

    private Canvas? _canvas;

    /// <summary>
    ///     This is the panel's actual parent - most likely the logical
    ///     parent's InnerPanel (if it has one). You should rarely need this.
    /// </summary>
    private Base? mActualParent;

    private List<Alignments> _alignments = [];
    private Padding _alignmentPadding;
    private Point _alignmentTranslation;

    private Rectangle mBounds;
    private Rectangle mBoundsOnDisk;

    private bool mCacheTextureDirty;

    private bool mCacheToTexture;

    private Color mColor;

    private Cursor mCursor;

    private bool _disabled;

    private bool mDisposed;

    private Pos _dock;
    private Padding _dockChildSpacing;

    private Package mDragAndDrop_package;

    private bool mDrawBackground = true;

    private bool mDrawDebugOutlines;

    private bool mHidden;
    private bool _skipRender;

    private bool mHideToolTip;

    private Rectangle _outerBounds;
    private Rectangle mInnerBounds;

    /// <summary>
    ///     If the innerpanel exists our children will automatically become children of that
    ///     instead of us - allowing us to move them all around by moving that panel (useful for scrolling etc).
    /// </summary>
    protected Base? _innerPanel;

    private bool mKeyboardInputEnabled;

    private Margin mMargin;

    private Point _maximumSize = default;

    private Point _minimumSize = default;

    private bool mMouseInputEnabled;

    private string? _name;

    private bool _needsAlignment;
    private bool _needsLayout;

    private Padding mPadding;

    protected Modal? mModal;
    private Base? mOldParent;
    private Base? mParent { get; set; }

    private Rectangle mRenderBounds;

    private bool mRestrictToParent;

    private Skin.Base mSkin;

    private bool mTabable;

    private Base? _tooltip;

    private string? _tooltipBackgroundName;

    private GameTexture? _tooltipBackground { get; set; }

    private Color? _tooltipTextColor;

    public virtual string? TooltipFontName
    {
        get => _tooltipFont?.Name;
        set
        {
            if (value == TooltipFontName)
            {
                return;
            }

            TooltipFont = GameContentManager.Current.GetFont(value, TooltipFont?.Size ?? 10);
        }
    }

    public virtual int TooltipFontSize
    {
        get => _tooltipFont?.Size ?? 10;
        set
        {
            if (value == TooltipFontSize)
            {
                return;
            }

            TooltipFont = GameContentManager.Current.GetFont(TooltipFont?.Name, value);
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
            GameTexture? texture = null;
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

    private GameFont? _tooltipFont;

    private string? _tooltipFontInfo;

    private object mUserData;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Base" /> class.
    /// </summary>
    /// <param name="parent">parent control</param>
    /// <param name="name">name of this control</param>
    public Base(Base? parent = default, string? name = default)
    {
        if (this is Canvas canvas)
        {
            _canvas = canvas;
        }

        _name = name ?? string.Empty;
        mChildren = [];
        mAccelerators = [];

        Parent = parent;

        mHidden = false;
        mBounds = new Rectangle(0, 0, 10, 10);
        mPadding = Padding.Zero;
        mMargin = Margin.Zero;
        mColor = Color.White;
        _alignmentPadding = Padding.Zero;

        RestrictToParent = false;

        MouseInputEnabled = true;
        KeyboardInputEnabled = false;

        Invalidate();
        Cursor = Cursors.Default;

        //ToolTip = null;
        IsTabable = false;
        ShouldDrawBackground = true;
        _disabled = false;
        mCacheTextureDirty = true;
        mCacheToTexture = false;

        BoundsOutlineColor = Color.Red;
        MarginOutlineColor = Color.Green;
        PaddingOutlineColor = Color.Blue;
    }

    /// <summary>
    ///     Font.
    /// </summary>
    public GameFont? TooltipFont
    {
        get => _tooltipFont;
        set
        {
            _tooltipFont = value;
            _tooltipFontInfo = value == null ? null : $"{value.GetName()},{value.GetSize()}";
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
    public List<Base> Children => _innerPanel?.Children ?? mChildren;

    /// <summary>
    ///     The logical parent. It's usually what you expect, the control you've parented it to.
    /// </summary>
    public Base? Parent
    {
        get => mParent;
        set
        {
            if (mParent == value)
            {
                return;
            }

            if (mParent is { } oldParent)
            {
                NotifyDetachingFromRoot(oldParent.Root);
                NotifyDetaching(oldParent);
                oldParent.RemoveChild(this, false);
            }

            PropagateCanvas(value?._canvas);

            mParent = value;
            mActualParent = default;

            if (mParent is not { } newParent)
            {
                return;
            }

            newParent.AddChild(this);

            NotifyAttachingToRoot(newParent.Root);
            NotifyAttaching(newParent);
        }
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
            OnAttaching(parent);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking OnAttaching() for {NodeName}",
                CanonicalName
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
            OnDetaching(parent);
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking OnDetaching() for {NodeName}",
                CanonicalName
            );
        }
    }

    protected virtual void OnDetachingFromRoot(Base root) {}

    private void NotifyDetachingFromRoot(Base root)
    {
        try
        {
            Root.RemoveUpdatableDataProviders(_updatableDataProviders.Keys);

            try
            {
                OnDetachingFromRoot(root);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Error occurred while invoking OnDetachingFromRoot() for {NodeName}",
                    CanonicalName
                );
            }

            foreach (var child in mChildren)
            {
                child.NotifyDetachingFromRoot(root);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking NotifyDetachingFromRoot() for {NodeName} from {RootName}",
                CanonicalName,
                root.CanonicalName
            );
        }
    }

    protected virtual void OnAttachingToRoot(Base root) { }

    private void NotifyAttachingToRoot(Base root)
    {
        try
        {
            Root.AddUpdatableDataProviders(_updatableDataProviders.Keys);

            try
            {
                OnAttachingToRoot(root);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Error occurred while invoking OnAttachingToRoot() for {NodeName}",
                    CanonicalName
                );
            }

            foreach (var child in mChildren)
            {
                child.NotifyAttachingToRoot(root);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Error occurred while invoking NotifyAttachingToRoot() for {NodeName} to {RootName}",
                CanonicalName,
                root.CanonicalName
            );
        }
    }

    private void PropagateCanvas(Canvas? canvas)
    {
        try
        {
            _canvas = canvas;

            var children = mChildren.ToArray();
            foreach (var child in children)
            {
                child.PropagateCanvas(canvas);
            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning(
                exception,
                "Exception thrown while invoking PropagateCanvas() for {NodeName} with {CanvasName}",
                CanonicalName,
                canvas?.CanonicalName ?? "null"
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

            Invalidate();
            InvalidateParent();
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

    protected bool HasSkin => mSkin != null || (mParent?.HasSkin ?? false);

    /// <summary>
    ///     Current skin.
    /// </summary>
    public Skin.Base Skin
    {
        get
        {
            if (mSkin != null)
            {
                return mSkin;
            }

            if (mParent != null)
            {
                return mParent.Skin;
            }

            throw new InvalidOperationException("GetSkin: null");
        }
    }

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
                        CanonicalName,
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
            if (mParent == null)
            {
                return false;
            }

            return mParent.IsMenuComponent;
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
        get => mPadding;
        set
        {
            if (mPadding == value)
            {
                return;
            }

            mPadding = value;
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
        get => mColor;
        set
        {
            if (mColor == value)
            {
                return;
            }

            mColor = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Current margin - outer spacing.
    /// </summary>
    public Margin Margin
    {
        get => mMargin;
        set
        {
            if (mMargin == value)
            {
                return;
            }

            mMargin = value;
            Invalidate();
            InvalidateParent();
        }
    }

    /// <summary>
    ///     Indicates whether the control is on top of its parent's children.
    /// </summary>
    public virtual bool IsOnTop

        // todo: validate
        => this == Parent.mChildren.First();

    /// <summary>
    ///     User data associated with the control.
    /// </summary>
    public object? UserData
    {
        get => mUserData;
        set => mUserData = value;
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
        set => SetAndDoIfChanged(ref _disabled, value, Invalidate);
    }

    /// <summary>
    ///     Indicates whether the control is hidden.
    /// </summary>
    public virtual bool IsHidden
    {
        get => ((_inheritParentEnablementProperties && Parent is {} parent) ? parent.IsHidden : mHidden);
        set
        {
            if (value == mHidden)
            {
                // ApplicationContext.CurrentContext.Logger.LogTrace(
                //     "{ComponentTypeName} (\"{ComponentName}\") set to same visibility ({Visibility})",
                //     GetType().GetName(qualified: true),
                //     CanonicalName,
                //     !value
                // );
                return;
            }

            var hidden = _inheritParentEnablementProperties ? (Parent?.IsHidden ?? value) : value;
            mHidden = hidden;

            VisibilityChangedEventArgs eventArgs = new()
            {
                IsVisible = !hidden,
            };

            OnVisibilityChanged(this, eventArgs);
            VisibilityChanged?.Invoke(this, eventArgs);

            Invalidate();
            InvalidateParent();
        }
    }

    protected virtual void OnVisibilityChanged(object? sender, VisibilityChangedEventArgs eventArgs)
    {

    }

    protected virtual Point InnerPanelSizeFrom(Point size) => size;

    public virtual bool IsHiddenByTree => mHidden || (Parent?.IsHiddenByTree ?? false);

    public virtual bool IsDisabledByTree => _disabled || (Parent?.IsDisabledByTree ?? false);

    /// <summary>
    ///     Determines whether the control's position should be restricted to parent's bounds.
    /// </summary>
    public bool RestrictToParent
    {
        get => mRestrictToParent;
        set => mRestrictToParent = value;
    }

    /// <summary>
    ///     Determines whether the control receives mouse input events.
    /// </summary>
    public bool MouseInputEnabled
    {
        get => mMouseInputEnabled;
        set
        {
            if (value == mMouseInputEnabled)
            {
                return;
            }

            mMouseInputEnabled = value;
        }
    }

    /// <summary>
    ///     Determines whether the control receives keyboard input events.
    /// </summary>
    public bool KeyboardInputEnabled
    {
        get => mKeyboardInputEnabled;
        set => mKeyboardInputEnabled = value;
    }

    /// <summary>
    ///     Gets or sets the mouse cursor when the cursor is hovering the control.
    /// </summary>
    public Cursor Cursor
    {
        get => mCursor;
        set => mCursor = value;
    }

    public int GlobalX => X + (Parent?.GlobalX ?? 0);

    public int GlobalY => Y + (Parent?.GlobalY ?? 0);

    public Point PositionGlobal => new Point(X, Y) + (mActualParent?.PositionGlobal ?? Point.Empty);

    public Rectangle GlobalBounds => new(PositionGlobal, Size);

    /// <summary>
    ///     Indicates whether the control is tabable (can be focused by pressing Tab).
    /// </summary>
    public bool IsTabable
    {
        get => mTabable;
        set => mTabable = value;
    }

    /// <summary>
    ///     Indicates whether control's background should be drawn during rendering.
    /// </summary>
    public bool ShouldDrawBackground
    {
        get => mDrawBackground;
        set => mDrawBackground = value;
    }

    /// <summary>
    ///     Indicates whether the renderer should cache drawing to a texture to improve performance (at the cost of memory).
    /// </summary>
    public bool ShouldCacheToTexture
    {
        get => mCacheToTexture;
        set => mCacheToTexture = value;
    }

    /// <summary>
    ///     Gets the control's internal canonical name.
    /// </summary>
    public string CanonicalName => mParent == null ? Name : mParent.Name + "." + Name;

    /// <summary>
    ///     Gets or sets the control's internal name.
    /// </summary>
    public string? Name
    {
        get => _name;
        set => _name = value;
    }

    /// <summary>
    ///     Control's size and position relative to the parent.
    /// </summary>
    public Rectangle Bounds => mBounds;

    public Rectangle OuterBounds => _outerBounds;

    public bool ClipContents { get; set; } = true;

    /// <summary>
    ///     Bounds for the renderer.
    /// </summary>
    public Rectangle RenderBounds => mRenderBounds;

    /// <summary>
    ///     Bounds adjusted by padding.
    /// </summary>
    public Rectangle InnerBounds => mInnerBounds;

    /// <summary>
    ///     Size restriction.
    /// </summary>
    public Point MinimumSize
    {
        get => _minimumSize;
        set
        {
            _minimumSize = value;
            if (_innerPanel != null)
            {
                _innerPanel.MinimumSize = InnerPanelSizeFrom(value);
            }
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
            _maximumSize = value;
            if (_innerPanel != null)
            {
                _innerPanel.MaximumSize = InnerPanelSizeFrom(value);
            }
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
    public bool IsVisible
    {
        get
        {
            if (IsHidden)
            {
                return false;
            }

            return Parent is not { } parent || parent.IsVisible || ToolTip.IsActiveTooltip(parent);
        }
        set => IsHidden = !value;
    }

    public bool IsVisibleInParent
    {
        get => !IsHidden || Parent is not { } parent || ToolTip.IsActiveTooltip(parent);
        set => IsHidden = false;
    }

    /// <summary>
    ///     Leftmost coordinate of the control.
    /// </summary>
    public int X
    {
        get => mBounds.X;
        set => SetPosition(value, Y);
    }

    /// <summary>
    ///     Topmost coordinate of the control.
    /// </summary>
    public int Y
    {
        get => mBounds.Y;
        set => SetPosition(X, value);
    }

    // todo: Bottom/Right includes margin but X/Y not?

    public int Width
    {
        get => mBounds.Width;
        set => SetSize(value, Height);
    }

    public int Height
    {
        get => mBounds.Height;
        set => SetSize(Width, value);
    }

    public int OuterWidth => Width + Margin.Left + Margin.Right;

    public int OuterHeight => Height + Margin.Top + Margin.Bottom;

    public Point Size
    {
        get => mBounds.Size;
        set => SetSize(value.X, value.Y);
    }

    public int InnerWidth => mBounds.Width - (mPadding.Left + mPadding.Right);

    public int MaximumInnerWidth => _maximumSize.X - (mPadding.Left + mPadding.Right);

    public int InnerHeight => mBounds.Height - (mPadding.Top + mPadding.Bottom);

    public int MaximumInnerHeight => _maximumSize.Y - (mPadding.Top + mPadding.Bottom);

    public int Bottom => mBounds.Bottom + mMargin.Bottom;

    public int Right => mBounds.Right + mMargin.Right;

    /// <summary>
    ///     Determines whether margin, padding and bounds outlines for the control will be drawn. Applied recursively to all
    ///     children.
    /// </summary>
    public bool DrawDebugOutlines
    {
        get => mDrawDebugOutlines;
        set
        {
            mDrawDebugOutlines = value;
            if (_innerPanel is { } innerPanel)
            {
                innerPanel.DrawDebugOutlines = value;
            }
            foreach (var child in mChildren)
            {
                child.DrawDebugOutlines = value;
            }
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
    public virtual void Dispose()
    {
        ////debug.print("Control.Base: Disposing {0} {1:X}", this, GetHashCode());
        if (mDisposed)
        {
            return;
        }

        ICacheToTexture cache = default;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            cache = Skin.Renderer.Ctt;
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

        // [Fix]: "InvalidOperationException: Collection was modified (during iteration); enumeration operation may not execute".
        // (Creates an array copy of the children to avoid modifying the collection during iteration).
        var children = mChildren.ToArray();
        foreach (var child in children)
        {
            child.Dispose();
        }

        mChildren?.Clear();

        _innerPanel?.Dispose();

        mDisposed = true;
        GC.SuppressFinalize(this);
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

    public virtual JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        JObject children = new();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var component in mChildren)
        {
            if (component == _innerPanel)
            {
                continue;
            }

            if (component == Tooltip)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(component.Name) && children[component.Name] == null)
            {
                children.Add(component.Name, component.GetJson());
            }
        }

        if (onlySerializeIfNotEmpty && !children.HasValues)
        {
            return null;
        }

        isRoot |= Parent == default;

        var boundsToWrite = isRoot
            ? new Rectangle(mBoundsOnDisk.X, mBoundsOnDisk.Y, mBounds.Width, mBounds.Height)
            : mBounds;

        var serializedProperties = new JObject(
            new JProperty(nameof(Bounds), Rectangle.ToString(boundsToWrite)),
            new JProperty(nameof(Dock), Dock.ToString()),
            new JProperty(nameof(Padding), Padding.ToString(mPadding)),
            new JProperty("AlignmentEdgeDistances", Padding.ToString(_alignmentPadding)),
            new JProperty("AlignmentTransform", _alignmentTranslation.ToString()),
            new JProperty(nameof(Margin), mMargin.ToString()),
            new JProperty(nameof(RenderColor), Color.ToString(mColor)),
            new JProperty(nameof(Alignments), string.Join(",", _alignments.ToArray())),
            new JProperty("DrawBackground", mDrawBackground),
            new JProperty(nameof(MinimumSize), _minimumSize.ToString()),
            new JProperty(nameof(MaximumSize), _maximumSize.ToString()),
            new JProperty("Disabled", _disabled),
            new JProperty("Hidden", mHidden),
            new JProperty(nameof(RestrictToParent), mRestrictToParent),
            new JProperty(nameof(MouseInputEnabled), mMouseInputEnabled),
            new JProperty("HideToolTip", mHideToolTip),
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

        if (children.HasValues)
        {
            serializedProperties.Add(nameof(Children), children);
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
                    }
                }
                catch (Exception exception)
                {
                    //Log JSON UI Loading Error
                    throw new Exception("Error loading json ui for " + CanonicalName, exception);
                }
            }

            if (!cacheHit && saveOutput)
            {
                GameContentManager.Current?.SaveUIJson(stage, Name, GetJsonUI(true), resolution);
            }
        });
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

        if (obj[nameof(Bounds)] != null)
        {
            mBoundsOnDisk = Rectangle.FromString((string)obj[nameof(Bounds)]);
            isRoot = isRoot || Parent == default;
            if (isRoot)
            {
                SetSize(mBoundsOnDisk.Width, mBoundsOnDisk.Height);
            }
            else
            {
                SetBounds(ValidateJsonBounds(mBoundsOnDisk));
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

        if (obj[nameof(MinimumSize)] != null)
        {
            MinimumSize = Point.FromString((string) obj[nameof(MinimumSize)]);
        }

        if (obj[nameof(MaximumSize)] != null)
        {
            MaximumSize = Point.FromString((string) obj[nameof(MaximumSize)]);
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
            mHideToolTip = true;
            SetToolTipText(null);
        }

        if (obj["ToolTipBackground"] is JValue { Type: JTokenType.String } tooltipBackgroundName)
        {
            var fileName = tooltipBackgroundName.Value<string>();
            GameTexture texture = null;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                texture = GameContentManager.Current?.GetTexture(Content.TextureType.Gui, fileName);
            }

            _tooltipBackgroundName = fileName;
            _tooltipBackground = texture;
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
                TooltipFont = GameContentManager.Current?.GetFont(fontName, fontSize);
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

        if (obj[nameof(Children)] != null)
        {
            var children = obj[nameof(Children)];
            foreach (JProperty tkn in children)
            {
                var name = tkn.Name;
                foreach (var ctrl in mChildren)
                {
                    if (ctrl.Name == name)
                    {
                        ctrl.LoadJson(tkn.First);
                    }
                }
            }
        }

        Invalidate(alsoInvalidateParent: true);
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

    private bool HasNamedChildren()
    {
        return mChildren?.Any(ctrl => !string.IsNullOrEmpty(ctrl?.Name)) ?? false;
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

    private readonly ConcurrentQueue<Action> _deferredActions = [];

    public void Defer(Action action)
    {
        _deferredActions.Enqueue(action);
        Invalidate();
    }

    public override string ToString()
    {
        if (this is MenuItem)
        {
            return "[MenuItem: " + (this as MenuItem).Text + "]";
        }

        if (this is Label)
        {
            return "[Label: " + (this as Label).Text + "]";
        }

        if (this is Text)
        {
            return "[Text: " + (this as Text).DisplayedText + "]";
        }

        return GetType().ToString();
    }

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

        if (mHideToolTip || string.IsNullOrWhiteSpace(text))
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
                Font = _tooltipFont ?? GameContentManager.Current?.GetFont("sourcesansproblack", 10),
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
        for (int i = 0; i < mChildren.Count; i++)
        {
            mChildren[i]?.Invalidate();
            if (recursive)
            {
                mChildren[i]?.InvalidateChildren(true);
            }
        }

        if (_innerPanel != null)
        {
            foreach (var child in _innerPanel.mChildren)
            {
                child?.Invalidate();
                if (recursive)
                {
                    child?.InvalidateChildren(true);
                }
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
        if (!_needsLayout)
        {
            _needsLayout = true;
        }

        if (!mCacheTextureDirty)
        {
            mCacheTextureDirty = true;
        }
    }

    public void Invalidate(bool alsoInvalidateParent)
    {
        Invalidate();
        if (alsoInvalidateParent)
        {
            InvalidateParent();
        }
    }

    public virtual void MoveBefore(Base other)
    {
        if (other == this)
        {
            return;
        }

        if (other.Parent is not {} otherParent)
        {
            // If the other component has no parent we can't move before it
            return;
        }

        if (Parent is { } parent && parent != otherParent)
        {
            // If we have a parent and the parent is not the same as the other component, we should not move
            // This won't be hit if we have no parent, which will be treated as an insertion
            return;
        }

        var parentChildren = otherParent.Children;
        var otherIndex = parentChildren.IndexOf(other);
        if (otherIndex < 0)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Possible race condition detected: Component '{Other}' is not in its parent's ({Parent}) list of children",
                string.IsNullOrWhiteSpace(other.Name) ? other.GetType().GetName(qualified: true) : other.Name,
                string.IsNullOrWhiteSpace(otherParent.Name) ? otherParent.GetType().GetName(qualified: true) : otherParent.Name
            );
            return;
        }

        var ownIndex = parentChildren.IndexOf(this);
        if (ownIndex < 0)
        {
            // If we have no parent yet, add it
            ownIndex = otherParent.AddChild(this);
        }

        var insertionIndex = otherIndex;
        if (ownIndex < insertionIndex)
        {
            // If the component being moved is already ordered before the other component the insertion
            // index will decrease by one after we Remove() below, so we need to decrement it
            --insertionIndex;

            if (ownIndex == insertionIndex)
            {
                // Skip this since we're not actually moving it
                return;
            }
        }

        _ = parentChildren.Remove(this);
        parentChildren.Insert(insertionIndex, this);
    }

    public virtual void MoveAfter(Base other)
    {
        if (other == this)
        {
            return;
        }

        if (other.Parent is not {} otherParent)
        {
            // If the other component has no parent we can't move before it
            return;
        }

        if (Parent is { } parent && parent != otherParent)
        {
            // If we have a parent and the parent is not the same as the other component, we should not move
            // This won't be hit if we have no parent, which will be treated as an insertion
            return;
        }

        var parentChildren = otherParent.Children;
        var otherIndex = parentChildren.IndexOf(other);
        if (otherIndex < 0)
        {
            ApplicationContext.Context.Value?.Logger.LogError(
                "Possible race condition detected: Component '{Other}' is not in its parent's ({Parent}) list of children",
                string.IsNullOrWhiteSpace(other.Name) ? other.GetType().GetName(qualified: true) : other.Name,
                string.IsNullOrWhiteSpace(otherParent.Name) ? otherParent.GetType().GetName(qualified: true) : otherParent.Name
            );
            return;
        }

        var ownIndex = parentChildren.IndexOf(this);
        if (ownIndex < 0)
        {
            // If we have no parent yet, add it
            ownIndex = otherParent.AddChild(this);
        }

        var insertionIndex = otherIndex;
        if (ownIndex > insertionIndex)
        {
            // If the component being moved is already ordered after the other component the insertion
            // index will decrease by one after we Remove() below, so we need to decrement it
            ++insertionIndex;

            if (ownIndex == insertionIndex)
            {
                // Skip this since we're not actually moving it
                return;
            }
        }

        _ = parentChildren.Remove(this);
        parentChildren.Insert(insertionIndex, this);
    }

    /// <summary>
    ///     Sends the control to the bottom of paren't visibility stack.
    /// </summary>
    public virtual void SendToBack()
    {
        if (mActualParent == null)
        {
            return;
        }

        if (mActualParent.mChildren.Count == 0)
        {
            return;
        }

        if (mActualParent.mChildren.First() == this)
        {
            return;
        }

        mActualParent.mChildren.Remove(this);
        mActualParent.mChildren.Insert(0, this);

        InvalidateParent();
    }

    /// <summary>
    ///     Brings the control to the top of paren't visibility stack.
    /// </summary>
    public virtual void BringToFront()
    {
        if (mParent != null && mParent is Modal modal)
        {
            modal.BringToFront();
        }

        var last = mActualParent?.mChildren.LastOrDefault();
        if (last == default || last == this)
        {
            return;
        }

        mActualParent.mChildren.Remove(this);
        mActualParent.mChildren.Add(this);
        InvalidateParent();
        Redraw();
    }

    public virtual void BringNextToControl(Base child, bool behind)
    {
        if (null == mActualParent)
        {
            return;
        }

        mActualParent.mChildren.Remove(this);

        // todo: validate
        var idx = mActualParent.mChildren.IndexOf(child);
        if (idx == mActualParent.mChildren.Count - 1)
        {
            BringToFront();

            return;
        }

        if (behind)
        {
            ++idx;

            if (idx == mActualParent.mChildren.Count - 1)
            {
                BringToFront();

                return;
            }
        }

        mActualParent.mChildren.Insert(idx, this);
        InvalidateParent();
    }

    /// <summary>
    /// Finds the first child that matches the predicate.
    /// </summary>
    /// <param name="predicate">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
    /// <param name="recurse">Whether or not the search will recurse through the element tree.</param>
    /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <see cref="Base" />.</returns>
    public virtual Base Find(Predicate<Base> predicate, bool recurse = false)
    {
        var child = mChildren.Find(predicate);
        if (child != null)
        {
            return child;
        }

        return recurse
            ? mChildren.Select(selectChild => selectChild?.Find(predicate, true)).FirstOrDefault()
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

        children.AddRange(mChildren.FindAll(predicate));

        if (recurse)
        {
            children.AddRange(mChildren.SelectMany(selectChild => selectChild?.FindAll(predicate, true)));
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
        if (mModal != null)
        {
            return;
        }

        mModal = new Modal(Canvas)
        {
            ShouldDrawBackground = dim,
        };

        mOldParent = Parent;
        Parent = mModal;
    }

    public void RemoveModal()
    {
        if (mModal == null)
        {
            return;
        }

        Parent = mOldParent;
        Canvas?.RemoveChild(mModal, false);
        mModal = null;
    }

    /// <summary>
    ///     Attaches specified control as a child of this one.
    /// </summary>
    /// <remarks>
    ///     If InnerPanel is not null, it will become the parent.
    /// </remarks>
    /// <param name="child">Control to be added as a child.</param>
    /// <param name="setParent"></param>
    public virtual int AddChild(Base child)
    {
        int childIndex;

        if (_innerPanel == null)
        {
            childIndex = mChildren.Count;
            mChildren.Add(child);
            child.mActualParent = this;
        }
        else
        {
            childIndex = _innerPanel.AddChild(child);
        }

        child.DrawDebugOutlines = DrawDebugOutlines;

        OnChildAdded(child);

        return childIndex;
    }

    /// <summary>
    ///     Detaches specified control from this one.
    /// </summary>
    /// <param name="child">Child to be removed.</param>
    /// <param name="dispose">Determines whether the child should be disposed (added to delayed delete queue).</param>
    public virtual void RemoveChild(Base child, bool dispose)
    {
        // If we removed our innerpanel
        // remove our pointer to it
        if (_innerPanel == child)
        {
            mChildren.Remove(_innerPanel);
            _innerPanel.DelayedDelete();
            _innerPanel = null;

            return;
        }

        if (_innerPanel != null && _innerPanel.Children.Contains(child))
        {
            _innerPanel.RemoveChild(child, dispose);

            return;
        }

        mChildren.Remove(child);
        OnChildRemoved(child);

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
        while (mChildren.Count > 0)
        {
            RemoveChild(mChildren[0], true);
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
        SetPosition((int)_icon.LocalPosToCanvas(new Point(0, 0)).X, (int)_icon.LocalPosToCanvas(new Point(0, 0)).Y);
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

        if (mBounds.X == x && mBounds.Y == y && mBounds.Width == width && mBounds.Height == height)
        {
            return false;
        }

        var oldBounds = Bounds;

        var newBounds = mBounds with
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
                CanonicalName,
                oldBounds.Size,
                newBounds.Size
            );
        }

        mBounds = newBounds;

        var margin = mMargin;
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
        //Anything that needs to update on size changes
        //Iterate my children and tell them I've changed
        Parent?.OnChildBoundsChanged(this, oldBounds, newBounds);

        if (mBounds.Width != oldBounds.Width || mBounds.Height != oldBounds.Height)
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
        for (int i = 0; i < mChildren.Count; i++)
        {
            mChildren[i].OnScaleChanged();
        }
    }

    /// <summary>
    ///     Handler invoked when a child component's bounds change.
    /// </summary>
    protected virtual void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
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
        if (mCacheTextureDirty && renderer.ClipRegionVisible)
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

            var childrenToRender = mChildren.ToArray();
            childrenToRender = OrderChildrenForRendering(childrenToRender.Where(child => child.IsVisible));

            foreach (var child in childrenToRender)
            {
                child.DoCacheRender(skin, master);
            }

            // Finish the cache texture if necessary
            if (ShouldCacheToTexture)
            {
                cache.FinishCacheTexture(this);
                mCacheTextureDirty = false;
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

    /// <summary>
    ///     Rendering logic implementation.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    internal virtual void DoRender(Skin.Base skin)
    {
        // If this control has a different skin,
        // then so does its children.
        if (mSkin != null)
        {
            skin = mSkin;
        }

        // Do think
        Think();

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
        foreach (var child in mChildren)
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

        var childrenToRender = mChildren.ToArray();
        childrenToRender = OrderChildrenForRendering(childrenToRender.Where(child => child.IsVisible));

        foreach (var child in childrenToRender)
        {
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
        if (mSkin == skin)
        {
            return;
        }

        mSkin = skin;
        Invalidate();
        Redraw();
        OnSkinChanged(skin);

        if (doChildren)
        {
            for (int i = 0; i < mChildren.Count; i++)
            {
                mChildren[i].SetSkin(skin, true);
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
        if (mActualParent != null)
        {
            return mActualParent.OnMouseWheeled(delta);
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
        if (mActualParent != null)
        {
            return mActualParent.OnMouseHWheeled(delta);
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
        get => _mouseButtonPressed.GetValueOrDefault(MouseButton.Left, false);
        set => _mouseButtonPressed[MouseButton.Left] = value;
    }

    public bool IsMouseButtonActive(MouseButton mouseButton) =>
        _mouseButtonPressed.GetValueOrDefault(mouseButton, false);

    private readonly Dictionary<MouseButton, bool> _mouseButtonPressed = [];

    public IReadOnlyDictionary<MouseButton, bool> MouseButtonPressed => _mouseButtonPressed;

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
            _mouseButtonPressed[mouseButton] = true;
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

            _mouseButtonPressed[mouseButton] = false;
            InputHandler.MouseFocus = null;

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

        var children = mChildren.ToArray();
        // Check children in reverse order (last added first).
        for (int childIndex = children.Length - 1; childIndex >= 0; childIndex--)
        {
            var child = children[childIndex];
            if (child.GetComponentAt(x - child.X, y - child.Y, filters) is { } descendant)
            {
                return descendant;
            }
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

    protected virtual bool ShouldSkipLayout => IsHidden && !ToolTip.IsActiveTooltip(this);

    public int NodeCount => 1 + mChildren.Sum(child => child.NodeCount);

    protected virtual void Prelayout(Skin.Base skin)
    {

    }

    protected void DoLayoutIfNeeded(Skin.Base skin)
    {
        if (!_needsLayout)
        {
            return;
        }

        _needsLayout = false;
        Layout(skin);
    }

    /// <summary>
    ///     Recursively lays out the control's interior according to alignment, margin, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void RecurseLayout(Skin.Base skin)
    {
        if (mSkin != null)
        {
            skin = mSkin;
        }

        if (ShouldSkipLayout)
        {
            return;
        }

        var children = mChildren.ToArray();
        foreach (var child in children)
        {
            child.Prelayout(skin);
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

        var remainingBounds = RenderBounds;

        // Adjust bounds for padding
        remainingBounds.X += mPadding.Left;
        remainingBounds.Width -= mPadding.Left + mPadding.Right;
        remainingBounds.Y += mPadding.Top;
        remainingBounds.Height -= mPadding.Top + mPadding.Bottom;

        var dockChildSpacing = DockChildSpacing;

        var directionalDockChildren =
            children.Where(child => !child.ShouldSkipLayout && !child.Dock.HasFlag(Pos.Fill)).ToArray();
        var dockFillChildren =
            children.Where(child => !child.ShouldSkipLayout && child.Dock.HasFlag(Pos.Fill)).ToArray();

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

            var childFitsContents = child is IAutoSizeToContents { AutoSizeToContents: true };

            if (childDock.HasFlag(Pos.Left))
            {
                var height = childFitsContents
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
                var height = childFitsContents
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
                var width = childFitsContents
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
                var width = childFitsContents
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

        mInnerBounds = remainingBounds;

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
                    newPosition.Y = remainingBounds.Y + (remainingBounds.Height - (childMarginV + child.Height)) / 2;
                }

                child.SetPosition(newPosition);
            }
            else
            {
                Point newSize = new(
                    remainingBounds.Width - childMarginH,
                    remainingBounds.Height - childMarginV
                );

                ApplyDockFill(child, newPosition, newSize);
            }

            child.RecurseLayout(skin);
        }

        PostLayout(skin);
        AfterLayout?.Invoke(this, EventArgs.Empty);

        while (_deferredActions.TryDequeue(out var deferredAction))
        {
            deferredAction();
        }

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

    protected virtual void ApplyDockFill(Base child, Point position, Point size)
    {
        child.SetBounds(position, size);
    }

    /// <summary>
    ///     Checks if the given control is a child of this instance.
    /// </summary>
    /// <param name="child">Control to examine.</param>
    /// <returns>True if the control is out child.</returns>
    public bool IsChild(Base child)
    {
        return mChildren.Contains(child);
    }

    /// <summary>
    ///     Converts local coordinates to canvas coordinates.
    /// </summary>
    /// <param name="pnt">Local coordinates.</param>
    /// <returns>Canvas coordinates.</returns>
    public virtual Point LocalPosToCanvas(Point pnt)
    {
        if (mParent == null)
        {
            return pnt;
        }

        var x = pnt.X + X;
        var y = pnt.Y + Y;

        // If our parent has an innerpanel and we're a child of it
        // add its offset onto us.
        if (mParent._innerPanel != null && mParent._innerPanel.IsChild(this))
        {
            x += mParent._innerPanel.X;
            y += mParent._innerPanel.Y;
        }

        return mParent.LocalPosToCanvas(new Point(x, y));
    }

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
        if (mParent is not { } parent)
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
        if (mParent is not {} parent)
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
        var childrenCopy = mChildren.FindAll(x => true);
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
        mRenderBounds.X = 0;
        mRenderBounds.Y = 0;

        mRenderBounds.Width = mBounds.Width;
        mRenderBounds.Height = mBounds.Height;
    }

    /// <summary>
    ///     Sets mouse cursor to current cursor.
    /// </summary>
    public virtual void UpdateCursor()
    {
        Platform.Neutral.SetCursor(mCursor);
    }

    // giver
    public virtual Package DragAndDrop_GetPackage(int x, int y)
    {
        return mDragAndDrop_package;
    }

    // giver
    public virtual bool DragAndDrop_Draggable()
    {
        if (mDragAndDrop_package == null)
        {
            return false;
        }

        return mDragAndDrop_package.IsDraggable;
    }

    // giver
    public virtual void DragAndDrop_SetPackage(bool draggable, string name = "", object userData = null)
    {
        if (mDragAndDrop_package == null)
        {
            mDragAndDrop_package = new Package();
            mDragAndDrop_package.IsDraggable = draggable;
            mDragAndDrop_package.Name = name;
            mDragAndDrop_package.UserData = userData;
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

    /// <summary>
    ///     Resizes the control to fit its children.
    /// </summary>
    /// <param name="resizeX">Determines whether to change control's width.</param>
    /// <param name="resizeY">Determines whether to change control's height.</param>
    /// <param name="recursive"></param>
    /// <returns>True if bounds changed.</returns>
    public virtual bool SizeToChildren(bool resizeX = true, bool resizeY = true, bool recursive = false)
    {
        if (recursive)
        {
            var children = mChildren.ToArray();
            foreach (var child in children)
            {
                if (child.mHidden)
                {
                    continue;
                }

                child.SizeToChildren(resizeX: resizeX, resizeY: resizeY, recursive: recursive);
            }
        }

        var childrenSize = GetChildrenSize();
        var padding = Padding;
        var size = childrenSize;
        var paddingH = padding.Right + padding.Left;
        var paddingV = padding.Bottom + padding.Top;

        size.X = size.X < 0 ? Width : (size.X + paddingH);
        size.Y = size.Y < 0 ? Height : (size.Y + paddingV);

        size.X = Math.Max(Math.Min(size.X, _maximumSize.X < 1 ? size.X : _maximumSize.X), _minimumSize.X);
        size.Y = Math.Max(Math.Min(size.Y, _maximumSize.Y < 1 ? size.Y : _maximumSize.Y), _minimumSize.Y);

        var newSize = new Point(resizeX ? size.X : Width, resizeY ? size.Y : Height);

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
    public virtual Point GetChildrenSize()
    {
        Point min = new(int.MaxValue, int.MaxValue);
        Point max = default;

        var children = mChildren.ToArray();
        foreach (var child in children)
        {
            if (!child.IsVisible)
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
            if (mAccelerators.ContainsKey(accelerator))
            {
                mAccelerators[accelerator].Invoke(this, EventArgs.Empty);

                return true;
            }
        }

        return mChildren.Any(child => child.HandleAccelerator(accelerator));
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

        mAccelerators[accelerator] = handler;
    }

    /// <summary>
    ///     Adds keyboard accelerator with a default handler.
    /// </summary>
    /// <param name="accelerator">Accelerator text.</param>
    public void AddAccelerator(string accelerator)
    {
        mAccelerators[accelerator] = DefaultAcceleratorHandler;
    }

    /// <summary>
    ///     Function invoked after layout.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected virtual void PostLayout(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Re-renders the control, invalidates cached texture.
    /// </summary>
    public virtual void Redraw()
    {
        UpdateColors();
        mCacheTextureDirty = true;
        mParent?.Redraw();
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
    public void InvalidateParent() => mParent?.Invalidate(alsoInvalidateParent: true);

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
            var hasValid = canvas._tabQueue.Any(
                control => control is { IsDisabledByTree: false, IsHiddenByTree: false }
            );
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
            while (next.IsHiddenByTree || next.IsDisabledByTree || !next.IsTabable);
        }

        if (next is { IsTabable: true, IsDisabledByTree: false, IsHiddenByTree: false })
        {
            Console.WriteLine($"Focusing {next.CanonicalName} ({next.GetFullishName()})");
            next.Focus(moveMouse: next is not TextBox);
        }

        Redraw();

        return true;
    }

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

    protected bool SetAndDoIfChanged<T>(ref T field, T value, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!SetIfChanged(ref field, value))
        {
            return false;
        }

        action();
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

public delegate void ValueChangeHandler<TValue>(TValue oldValue, TValue newValue);
