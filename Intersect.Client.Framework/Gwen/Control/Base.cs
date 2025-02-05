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
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Base control class.
/// </summary>
public partial class Base : IDisposable
{

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

    /// <summary>
    ///     Delegate used for all control event handlers.
    /// </summary>
    /// <param name="control">Event source.</param>
    /// <param name="args">Additional arguments. May be empty (EventArgs.Empty).</param>
    public delegate void GwenEventHandler<in T>(Base sender, T arguments) where T : EventArgs;

    /// <summary>
    ///     Accelerator map.
    /// </summary>
    private readonly Dictionary<string, GwenEventHandler<EventArgs>> mAccelerators;

    /// <summary>
    ///     Real list of children.
    /// </summary>
    private readonly List<Base> mChildren;

    /// <summary>
    ///     This is the panel's actual parent - most likely the logical
    ///     parent's InnerPanel (if it has one). You should rarely need this.
    /// </summary>
    private Base? mActualParent;

    private Padding mAlignmentDistance;

    private List<Alignments> mAlignments = new List<Alignments>();

    private Point mAlignmentTransform;

    private Rectangle mBounds;
    private Rectangle mBoundsOnDisk;

    private bool mCacheTextureDirty;

    private bool mCacheToTexture;

    private Color mColor;

    private Cursor mCursor;

    private bool _disabled;

    private bool mDisposed;

    private Pos mDock;

    private Package mDragAndDrop_package;

    private bool mDrawBackground;

    private bool mDrawDebugOutlines;

    private bool mHidden;

    private bool mHideToolTip;

    private Rectangle mInnerBounds;

    /// <summary>
    ///     If the innerpanel exists our children will automatically become children of that
    ///     instead of us - allowing us to move them all around by moving that panel (useful for scrolling etc).
    /// </summary>
    protected Base? _innerPanel;

    private bool mKeyboardInputEnabled;

    private Margin mMargin;

    private Point mMaximumSize = default;

    private Point mMinimumSize = default;

    private bool mMouseInputEnabled;

    private string? _name;

    private bool mNeedsLayout;

    private Padding mPadding;

    private Base? mParent;

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
        get => mToolTipFont?.Name;
        set
        {
            if (value == TooltipFontName)
            {
                return;
            }

            ToolTipFont = GameContentManager.Current.GetFont(value, ToolTipFont?.Size ?? 10);
        }
    }

    public virtual int TooltipFontSize
    {
        get => mToolTipFont?.Size ?? 10;
        set
        {
            if (value == TooltipFontSize)
            {
                return;
            }

            ToolTipFont = GameContentManager.Current.GetFont(ToolTipFont?.Name, value);
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

    private GameFont mToolTipFont;

    private string mToolTipFontInfo;

    private object mUserData;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Base" /> class.
    /// </summary>
    /// <param name="parent">parent control</param>
    /// <param name="name">name of this control</param>
    public Base(Base? parent = default, string? name = default)
    {
        _name = name ?? string.Empty;
        mChildren = new List<Base>();
        mAccelerators = new Dictionary<string, GwenEventHandler<EventArgs>>();

        Parent = parent;

        mHidden = false;
        mBounds = new Rectangle(0, 0, 10, 10);
        mPadding = Padding.Zero;
        mMargin = Margin.Zero;
        mColor = Color.White;
        mAlignmentDistance = Padding.Zero;

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
    public GameFont ToolTipFont
    {
        get => mToolTipFont;
        set
        {
            mToolTipFont = value;
            mToolTipFontInfo = $"{value?.GetName()},{value?.GetSize()}";
        }
    }

    public List<Alignments> CurAlignments => mAlignments;

    /// <summary>
    ///     Returns true if any on click events are set.
    /// </summary>
    internal bool ClickEventAssigned =>
        Clicked != null || RightClicked != null || DoubleClicked != null || DoubleRightClicked != null;

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

            if (mParent != default)
            {
                OnDetaching(mParent);
            }

            mParent?.RemoveChild(this, false);

            mParent = value;
            mActualParent = default;

            mParent?.AddChild(this);
            if (mParent != default)
            {
                OnAttaching(mParent);
            }
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
        get => mAlignments.ToArray();
        set
        {
            mAlignments = value.ToList();
            ProcessAlignments();
        }
    }

    /// <summary>
    ///     Dock position.
    /// </summary>
    public Pos Dock
    {
        get => mDock;
        set
        {
            if (mDock == value)
            {
                return;
            }

            mDock = value;

            Invalidate();
            InvalidateParent();
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
    public Padding AlignmentDistance
    {
        get => mAlignmentDistance;
        set
        {
            if (mAlignmentDistance == value)
            {
                return;
            }

            mAlignmentDistance = value;
            ProcessAlignments();
            Invalidate();
            InvalidateParent();
        }
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
    public object UserData
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
    public virtual bool IsDisabled
    {
        get => (_inheritParentEnablementProperties && Parent != default) ? Parent.IsDisabled : _disabled;
        set
        {
            if (value == _disabled)
            {
                return;
            }

            if (_inheritParentEnablementProperties)
            {
                _disabled = Parent?.IsDisabled ?? value;
            }
            else
            {
                _disabled = value;
            }

            Invalidate();
        }
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

    public virtual bool IsHiddenByTree => mHidden || (Parent?.IsHidden ?? false);

    public virtual bool IsDisabledByTree => _disabled || (Parent?.IsDisabled ?? false);

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

    public Point PositionGlobal => new Point(X, Y) + (Parent?.PositionGlobal ?? Point.Empty);

    public Rectangle BoundsGlobal => new Rectangle(PositionGlobal, Size);

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
        get => mMinimumSize;
        set => mMinimumSize = value;
    }

    /// <summary>
    ///     Size restriction.
    /// </summary>
    public Point MaximumSize
    {
        get => mMaximumSize;
        set
        {
            mMaximumSize = value;
            if (_innerPanel != null)
            {
                _innerPanel.MaximumSize = value;
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

            return Parent == null || Parent.IsVisible || ToolTip.IsActiveTooltip(Parent);
        }
        set => IsHidden = !value;
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

    public Point Size
    {
        get => mBounds.Size;
        set => SetSize(value.X, value.Y);
    }

    public int InnerWidth => mBounds.Width - (mPadding.Left + mPadding.Right);

    public int InnerHeight => mBounds.Height - (mPadding.Top + mPadding.Bottom);

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
            foreach (var child in Children)
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
    public Canvas? Canvas => GetCanvas();

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
    public event GwenEventHandler<EventArgs> BeforeDraw;

    /// <summary>
    ///     Invoked after this control is drawn
    /// </summary>
    public event GwenEventHandler<EventArgs> AfterDraw;

    public void AddAlignment(Alignments alignment)
    {
        if (mAlignments?.Contains(alignment) ?? true)
        {
            return;
        }

        mAlignments.Add(alignment);
    }

    public void RemoveAlignments()
    {
        mAlignments.Clear();
    }

    public virtual string GetJsonUI(bool isRoot = false)
    {
        return JsonConvert.SerializeObject(GetJson(isRoot), Formatting.Indented);
    }

    public virtual JObject GetJson(bool isRoot = default)
    {
        var alignments = new List<string>();
        foreach (var alignment in mAlignments)
        {
            alignments.Add(alignment.ToString());
        }

        isRoot |= Parent == default;

        var boundsToWrite = isRoot
            ? new Rectangle(mBoundsOnDisk.X, mBoundsOnDisk.Y, mBounds.Width, mBounds.Height)
            : mBounds;

        var serializedProperties = new JObject(
            new JProperty("Bounds", Rectangle.ToString(boundsToWrite)),
            new JProperty("Padding", Padding.ToString(mPadding)),
            new JProperty("AlignmentEdgeDistances", Padding.ToString(mAlignmentDistance)),
            new JProperty("AlignmentTransform", mAlignmentTransform.ToString()),
            new JProperty("Margin", mMargin.ToString()),
            new JProperty("RenderColor", Color.ToString(mColor)),
            new JProperty("Alignments", string.Join(",", alignments.ToArray())),
            new JProperty("DrawBackground", mDrawBackground),
            new JProperty("MinimumSize", mMinimumSize.ToString()),
            new JProperty("MaximumSize", mMaximumSize.ToString()),
            new JProperty("Disabled", _disabled),
            new JProperty("Hidden", mHidden),
            new JProperty("RestrictToParent", mRestrictToParent),
            new JProperty("MouseInputEnabled", mMouseInputEnabled),
            new JProperty("HideToolTip", mHideToolTip),
            new JProperty("ToolTipBackground", _tooltipBackgroundName),
            new JProperty("ToolTipFont", mToolTipFontInfo),
            new JProperty(
                nameof(TooltipTextColor),
                _tooltipTextColor == null ? JValue.CreateNull() : Color.ToString(_tooltipTextColor)
            )
        );

        JObject children = new();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var component in mChildren)
        {
            if (component == Tooltip)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(component.Name) && children[component.Name] == null)
            {
                children.Add(component.Name, component.GetJson());
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
        var children = json.Property("Children");
        if (children != null)
        {
            json.Remove("Children");
            json.Add(children);
        }

        return json;
    }

    public void LoadJsonUi(GameContentManager.UI stage, string? resolution, bool saveOutput = true)
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

    public virtual void LoadJson(JToken obj, bool isRoot = default)
    {
        if (obj["Alignments"] != null)
        {
            RemoveAlignments();
            var alignments = ((string) obj["Alignments"]).Split(',');
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

        if (obj["Bounds"] != null)
        {
            mBoundsOnDisk = Rectangle.FromString((string)obj["Bounds"]);
            isRoot = isRoot || Parent == default;
            if (isRoot)
            {
                SetSize(mBoundsOnDisk.Width, mBoundsOnDisk.Height);
            }
            else
            {
                SetBounds(mBoundsOnDisk);
            }
        }

        if (obj["Padding"] != null)
        {
            Padding = Padding.FromString((string) obj["Padding"]);
        }

        if (obj["AlignmentEdgeDistances"] != null)
        {
            mAlignmentDistance = Padding.FromString((string) obj["AlignmentEdgeDistances"]);
        }

        if (obj["AlignmentTransform"] != null)
        {
            mAlignmentTransform = Point.FromString((string) obj["AlignmentTransform"]);
        }

        if (obj["Margin"] != null)
        {
            Margin = Margin.FromString((string) obj["Margin"]);
        }

        if (obj["RenderColor"] != null)
        {
            RenderColor = Color.FromString((string) obj["RenderColor"]);
        }

        if (obj["DrawBackground"] != null)
        {
            ShouldDrawBackground = (bool) obj["DrawBackground"];
        }

        if (obj["MinimumSize"] != null)
        {
            MinimumSize = Point.FromString((string) obj["MinimumSize"]);
        }

        if (obj["MaximumSize"] != null)
        {
            MaximumSize = Point.FromString((string) obj["MaximumSize"]);
        }

        if (obj["Disabled"] != null)
        {
            IsDisabled = (bool) obj["Disabled"];
        }

        if (obj["Hidden"] != null)
        {
            IsHidden = (bool) obj["Hidden"];
        }

        if (obj["RestrictToParent"] != null)
        {
            RestrictToParent = (bool) obj["RestrictToParent"];
        }

        if (obj["MouseInputEnabled"] != null)
        {
            MouseInputEnabled = (bool) obj["MouseInputEnabled"];
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

        if (obj["ToolTipFont"] != null && obj["ToolTipFont"].Type != JTokenType.Null)
        {
            var fontArr = ((string) obj["ToolTipFont"]).Split(',');
            mToolTipFontInfo = (string) obj["ToolTipFont"];
            mToolTipFont = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
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

        if (HasNamedChildren())
        {
            if (obj["Children"] != null)
            {
                var children = obj["Children"];
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
        }
    }

    public virtual void ProcessAlignments()
    {
        foreach (var alignment in mAlignments)
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

        MoveTo(X + mAlignmentTransform.X, Y + mAlignmentTransform.Y, true);
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
    public event GwenEventHandler<EventArgs> HoverEnter;

    /// <summary>
    ///     Invoked when mouse pointer leaves the control.
    /// </summary>
    public event GwenEventHandler<EventArgs> HoverLeave;

    /// <summary>
    ///     Invoked when control's bounds have been changed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? BoundsChanged;

    /// <summary>
    ///     Invoked when the control has been left-clicked.
    /// </summary>
    public virtual event GwenEventHandler<ClickedEventArgs> Clicked;

    /// <summary>
    ///     Invoked when the control has been double-left-clicked.
    /// </summary>
    public virtual event GwenEventHandler<ClickedEventArgs> DoubleClicked;

    /// <summary>
    ///     Invoked when the control has been right-clicked.
    /// </summary>
    public virtual event GwenEventHandler<ClickedEventArgs> RightClicked;

    /// <summary>
    ///     Invoked when the control has been double-right-clicked.
    /// </summary>
    public virtual event GwenEventHandler<ClickedEventArgs> DoubleRightClicked;

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
    ///     Gets the canvas (root parent) of the control.
    /// </summary>
    /// <returns></returns>
    public virtual Canvas? GetCanvas() => mParent?.GetCanvas();

    /// <summary>
    ///     Enables the control.
    /// </summary>
    public void Enable()
    {
        IsDisabled = false;
    }

    /// <summary>
    ///     Disables the control.
    /// </summary>
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

            labelTooltip = new Label(this, name: "Tooltip")
            {
                AutoSizeToContents = true,
                Font = mToolTipFont ?? GameContentManager.Current?.GetFont("sourcesansproblack", 10),
                MaximumSize = new Point(300, 0),
                Padding = new Padding(
                    5,
                    3,
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
            if (mToolTipFont != null)
            {
                tooltip.Font = mToolTipFont;
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
        mNeedsLayout = true;
        mCacheTextureDirty = true;
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
    public virtual Base FindChildByName(string name, bool recursive = false) =>
        Find(child => string.Equals(child?.Name, name));

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

    protected virtual void OnAttached(Base parent)
    {
    }

    protected virtual void OnAttaching(Base newParent)
    {
    }

    protected virtual void OnDetached()
    {
    }

    protected virtual void OnDetaching(Base oldParent)
    {
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
    ///     Moves the control to a specific point, clamping on paren't bounds if RestrictToParent is set.
    /// </summary>
    /// <param name="x">Target x coordinate.</param>
    /// <param name="y">Target y coordinate.</param>
    public virtual void MoveTo(int x, int y, bool aligning = false)
    {
        if (RestrictToParent && Parent != null)
        {
            var parent = Parent;
            if (x - Padding.Left - (aligning ? mAlignmentDistance.Left : 0) < parent.Margin.Left)
            {
                x = parent.Margin.Left + Padding.Left + (aligning ? mAlignmentDistance.Left : 0);
            }

            if (y - Padding.Top - (aligning ? mAlignmentDistance.Top : 0) < parent.Margin.Top)
            {
                y = parent.Margin.Top + Padding.Top + (aligning ? mAlignmentDistance.Top : 0);
            }

            if (x + Width + Padding.Right + (aligning ? mAlignmentDistance.Right : 0) >
                parent.Width - parent.Margin.Right)
            {
                x = parent.Width -
                    parent.Margin.Right -
                    Width -
                    Padding.Right -
                    (aligning ? mAlignmentDistance.Right : 0);
            }

            if (y + Height + Padding.Bottom + (aligning ? mAlignmentDistance.Bottom : 0) >
                parent.Height - parent.Margin.Bottom)
            {
                y = parent.Height -
                    parent.Margin.Bottom -
                    Height -
                    Padding.Bottom -
                    (aligning ? mAlignmentDistance.Bottom : 0);
            }
        }

        SetBounds(x, y, Width, Height);
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
        if (mBounds.X == x && mBounds.Y == y && mBounds.Width == width && mBounds.Height == height)
        {
            return false;
        }

        var oldBounds = Bounds;

        mBounds.X = x;
        mBounds.Y = y;

        var maximumSize = MaximumSize;
        mBounds.Width = maximumSize.X > 0 ? Math.Min(MaximumSize.X, width) : width;
        mBounds.Height = maximumSize.Y > 0 ? Math.Min(MaximumSize.Y, height) : height;

        if (oldBounds.Size != mBounds.Size)
        {
            ProcessAlignments();
        }

        OnBoundsChanged(oldBounds);

        BoundsChanged?.Invoke(this, EventArgs.Empty);

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

    /// <summary>
    ///     Handler invoked when control's bounds change.
    /// </summary>
    /// <param name="oldBounds">Old bounds.</param>
    protected virtual void OnBoundsChanged(Rectangle oldBounds)
    {
        //Anything that needs to update on size changes
        //Iterate my children and tell them I've changed
        Parent?.OnChildBoundsChanged(oldBounds, this);

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
    ///     Handler invoked when control children's bounds change.
    /// </summary>
    protected virtual void OnChildBoundsChanged(Rectangle oldChildBounds, Base child)
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

            // Render the children (Reverse).
            for (int i = 0; i < mChildren.Count; i++)
            {
                var child = mChildren[i];
                if (!child.IsHidden)
                {
                    child.DoCacheRender(skin, master);
                }
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

    /// <summary>
    ///     Recursive rendering logic.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    /// <param name="clipRect">Clipping rectangle.</param>
    protected virtual void RenderRecursive(Skin.Base skin, Rectangle clipRect)
    {
        if (BeforeDraw != null)
        {
            BeforeDraw.Invoke(this, EventArgs.Empty);
        }

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
            if (!child.IsVisible)
            {
                continue;
            }

            child.DoRender(skin);
        }

        render.ClipRegion = oldRegion;
        render.StartClip();
        RenderOver(skin);

        RenderFocus(skin);

        render.RenderOffset = oldRenderOffset;

        if (AfterDraw != null)
        {
            AfterDraw.Invoke(this, EventArgs.Empty);
        }
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
        OnMouseMoved(x, y, dx, dy);
    }

    /// <summary>
    ///     Handler invoked on mouse click (left) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="down">If set to <c>true</c> mouse button is down.</param>
    protected virtual void OnMouseClickedLeft(int x, int y, bool down, bool automated = false)
    {
        if (down && Clicked != null)
        {
            Clicked(this, new ClickedEventArgs(x, y, down));
        }
    }

    /// <summary>
    ///     Invokes left mouse click event (used by input system).
    /// </summary>
    internal void InputMouseClickedLeft(int x, int y, bool down, bool automated = false)
    {
        OnMouseClickedLeft(x, y, down, automated);
    }

    /// <summary>
    ///     Handler invoked on mouse click (right) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="down">If set to <c>true</c> mouse button is down.</param>
    protected virtual void OnMouseClickedRight(int x, int y, bool down)
    {
        if (down && RightClicked != null)
        {
            RightClicked(this, new ClickedEventArgs(x, y, down));
        }
    }

    /// <summary>
    ///     Invokes right mouse click event (used by input system).
    /// </summary>
    internal void InputMouseClickedRight(int x, int y, bool down)
    {
        OnMouseClickedRight(x, y, down);
    }

    /// <summary>
    ///     Handler invoked on mouse double click (left) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    protected virtual void OnMouseDoubleClickedLeft(int x, int y)
    {
        // [omeg] should this be called?
        // [halfofastaple] Maybe. Technically, a double click is still technically a single click. However, this shouldn't be called here, and
        //					Should be called by the event handler.
        OnMouseClickedLeft(x, y, true);

        DoubleClicked?.Invoke(this, new ClickedEventArgs(x, y, true));
    }

    /// <summary>
    ///     Invokes left double mouse click event (used by input system).
    /// </summary>
    internal void InputMouseDoubleClickedLeft(int x, int y)
    {
        OnMouseDoubleClickedLeft(x, y);
    }

    /// <summary>
    ///     Handler invoked on mouse double click (right) event.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    protected virtual void OnMouseDoubleClickedRight(int x, int y)
    {
        // [halfofastaple] See: OnMouseDoubleClicked for discussion on triggering single clicks in a double click event
        OnMouseClickedRight(x, y, true);

        DoubleRightClicked?.Invoke(this, new ClickedEventArgs(x, y, true));
    }

    /// <summary>
    ///     Invokes right double mouse click event (used by input system).
    /// </summary>
    internal void InputMouseDoubleClickedRight(int x, int y)
    {
        OnMouseDoubleClickedRight(x, y);
    }

    /// <summary>
    ///     Handler invoked on mouse cursor entering control's bounds.
    /// </summary>
    protected virtual void OnMouseEntered()
    {
        if (HoverEnter != null)
        {
            HoverEnter.Invoke(this, EventArgs.Empty);
        }

        if (Tooltip != null)
        {
            ToolTip.Enable(this);
        }
        else if (Parent != null && Parent.Tooltip != null)
        {
            ToolTip.Enable(Parent);
        }

        Redraw();
    }

    protected void PlaySound(string filename)
    {
        if (filename == null || this.IsDisabled)
        {
            return;
        }

        filename = GameContentManager.RemoveExtension(filename).ToLower();
        var sound = GameContentManager.Current.GetSound(filename);
        if (sound != null)
        {
            var soundInstance = sound.CreateInstance();
            if (soundInstance != null)
            {
                Canvas.PlayAndAddSound(soundInstance);
            }
        }
    }

    /// <summary>
    ///     Invokes mouse enter event (used by input system).
    /// </summary>
    internal void InputMouseEntered()
    {
        OnMouseEntered();
    }

    /// <summary>
    ///     Handler invoked on mouse cursor leaving control's bounds.
    /// </summary>
    protected virtual void OnMouseLeft()
    {
        if (HoverLeave != null)
        {
            HoverLeave.Invoke(this, EventArgs.Empty);
        }

        if (Tooltip != null)
        {
            ToolTip.Disable(this);
        }
        else if (Parent != null && Parent.Tooltip != null)
        {
            ToolTip.Disable(Parent);
        }

        Redraw();
    }

    /// <summary>
    ///     Invokes mouse leave event (used by input system).
    /// </summary>
    internal void InputMouseLeft()
    {
        OnMouseLeft();
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
    /// <param name="x">Child X.</param>
    /// <param name="y">Child Y.</param>
    /// <returns>Control or null if not found.</returns>
    public virtual Base GetControlAt(int x, int y)
    {
        // Return null if control is hidden or coordinates are outside the control's bounds.
        if (IsHidden || x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return null;
        }

        // Check children in reverse order (last added first).
        for (int i = mChildren.Count - 1; i >= 0; i--)
        {
            var child = mChildren[i];
            var found = child.GetControlAt(x - child.X, y - child.Y);
            if (found != null)
            {
                return found;
            }
        }

        // Return control if it is mouse input enabled, otherwise return null.
        return MouseInputEnabled ? this : null;
    }

    public virtual Base GetControlAt(Point point) => GetControlAt(point.X, point.Y);

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

    protected virtual void Prelayout(Skin.Base skin)
    {

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

        foreach (var child in mChildren)
        {
            child.Prelayout(skin);
        }

        var expectedSize = new Point(Math.Max(Width, MinimumSize.X), Math.Max(Height, MinimumSize.Y));
        if (expectedSize != Size)
        {
            Size = expectedSize;
        }

        if (mNeedsLayout)
        {
            mNeedsLayout = false;
            Layout(skin);
        }

        var ownBounds = RenderBounds;

        // Adjust bounds for padding
        ownBounds.X += mPadding.Left;
        ownBounds.Width -= mPadding.Left + mPadding.Right;
        ownBounds.Y += mPadding.Top;
        ownBounds.Height -= mPadding.Top + mPadding.Bottom;

        foreach (var child in mChildren)
        {
            if (child.ShouldSkipLayout)
            {
                continue;
            }

            var childDock = child.Dock;

            if (childDock.HasFlag(Pos.Fill))
            {
                continue;
            }

            var childMargin = child.Margin;
            var childMarginH = childMargin.Left + childMargin.Right;
            var childMarginV = childMargin.Top + childMargin.Bottom;

            var childOuterWidth = childMarginH + child.Width;
            var childOuterHeight = childMarginV + child.Height;

            if (childDock.HasFlag(Pos.Top))
            {
                child.SetBounds(
                    ownBounds.X + childMargin.Left,
                    ownBounds.Y + childMargin.Top,
                    ownBounds.Width - childMarginH,
                    child.Height
                );

                ownBounds.Y += childOuterHeight;
                ownBounds.Height -= childOuterHeight;
            }

            if (childDock.HasFlag(Pos.Left))
            {
                child.SetBounds(
                    ownBounds.X + childMargin.Left,
                    ownBounds.Y + childMargin.Top,
                    child.Width,
                    ownBounds.Height - childMarginV
                );

                ownBounds.X += childOuterWidth;
                ownBounds.Width -= childOuterWidth;
            }

            if (childDock.HasFlag(Pos.Right))
            {
                child.SetBounds(
                    ownBounds.X + ownBounds.Width - child.Width - childMargin.Right,
                    ownBounds.Y + childMargin.Top,
                    child.Width,
                    ownBounds.Height - childMarginV
                );

                ownBounds.Width -= childOuterWidth;
            }

            if (childDock.HasFlag(Pos.Bottom))
            {
                if (child.Name?.StartsWith("BottomBar") ?? false)
                {
                    child.Margin.ToString();
                }

                child.SetBounds(
                    ownBounds.Left + childMargin.Left,
                    ownBounds.Bottom - (child.Height + childMargin.Bottom),
                    ownBounds.Width - childMarginH,
                    child.Height
                );

                ownBounds.Height -= childOuterHeight;
            }

            child.RecurseLayout(skin);
        }

        mInnerBounds = ownBounds;

        //
        // Fill uses the left over space, so do that now.
        //
        foreach (var child in mChildren)
        {
            if (child.ShouldSkipLayout)
            {
                continue;
            }

            var dock = child.Dock;

            if (!dock.HasFlag(Pos.Fill))
            {
                continue;
            }

            var childMargin = child.Margin;
            var childMarginH = childMargin.Left + childMargin.Right;
            var childMarginV = childMargin.Top + childMargin.Bottom;

            Point newPosition = new(
                ownBounds.X + childMargin.Left,
                ownBounds.Y + childMargin.Top
            );

            if (child is IAutoSizeToContents { AutoSizeToContents: true })
            {
                if (Pos.Right == (dock & (Pos.Right | Pos.Left)))
                {
                    newPosition.X = ownBounds.Right - (childMargin.Right + child.Width);
                }

                if (Pos.Bottom == (dock & (Pos.Bottom | Pos.Top)))
                {
                    newPosition.Y = ownBounds.Bottom - (childMargin.Bottom + child.Height);
                }

                if (dock.HasFlag(Pos.CenterH))
                {
                    newPosition.X = ownBounds.X + (ownBounds.Width - (childMarginH + child.Width)) / 2;
                }

                if (dock.HasFlag(Pos.CenterV))
                {
                    newPosition.Y = ownBounds.Y + (ownBounds.Height - (childMarginV + child.Height)) / 2;
                }

                child.SetPosition(newPosition);
            }
            else
            {
                Point newSize = new(
                    ownBounds.Width - childMarginH,
                    ownBounds.Height - childMarginV
                );

                child.SetBounds(newPosition, newSize);
            }

            child.RecurseLayout(skin);
        }

        PostLayout(skin);

        var canvas = GetCanvas();
        // ReSharper disable once InvertIf
        if (canvas != default)
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
    /// <param name="pnt">Canvas coordinates.</param>
    /// <returns>Local coordinates.</returns>
    public virtual Point CanvasPosToLocal(Point pnt)
    {
        if (mParent == null)
        {
            return pnt;
        }

        var x = pnt.X - X;
        var y = pnt.Y - Y;

        // If our parent has an innerpanel and we're a child of it
        // add its offset onto us.
        if (mParent._innerPanel != null && mParent._innerPanel.IsChild(this))
        {
            x -= mParent._innerPanel.X;
            y -= mParent._innerPanel.Y;
        }

        return mParent.CanvasPosToLocal(new Point(x, y));
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
    /// <param name="width">Determines whether to change control's width.</param>
    /// <param name="height">Determines whether to change control's height.</param>
    /// <returns>True if bounds changed.</returns>
    public virtual bool SizeToChildren(bool width = true, bool height = true)
    {
        var size = GetChildrenSize();
        size.X += Padding.Right;
        size.Y += Padding.Bottom;

        if (!SetSize(width ? size.X : Width, height ? size.Y : Height))
        {
            return false;
        }

        ProcessAlignments();

        return true;
    }

    /// <summary>
    ///     Returns the total width and height of all children.
    /// </summary>
    /// <remarks>
    ///     Default implementation returns maximum size of children since the layout is unknown.
    ///     Implement this in derived compound controls to properly return their size.
    /// </remarks>
    /// <returns></returns>
    public virtual Point GetChildrenSize()
    {
        var size = Point.Empty;

        for (int i = 0; i < mChildren.Count; i++)
        {
            if (mChildren[i].IsHidden)
            {
                continue;
            }

            size.X = Math.Max(size.X, mChildren[i].Right);
            size.Y = Math.Max(size.Y, mChildren[i].Bottom);
        }

        return size;
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
    public void AddAccelerator(string accelerator, GwenEventHandler<EventArgs> handler)
    {
        accelerator = accelerator.Trim().ToUpperInvariant();
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
    public void InvalidateParent()
    {
        mParent?.Invalidate();
        mParent?.InvalidateParent();
    }

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
        return OnKeyPressed(key, down);
    }

    /// <summary>
    ///     Handler for keyboard events.
    /// </summary>
    /// <param name="key">Key pressed.</param>
    /// <returns>True if handled.</returns>
    protected virtual bool OnKeyReleaseed(Key key)
    {
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

        var canvas = GetCanvas();
        if (canvas == default)
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
        OnPaste(from, EventArgs.Empty);
    }

    internal void InputCut(Base from)
    {
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

    protected bool SetIfChanged<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        return true;
    }

    protected bool SetIfChanged<T>(ref T field, T value, out T oldValue)
    {
        oldValue = field;
        return SetIfChanged(ref field, value);
    }

    protected bool SetAndDoIfChanged<T>(ref T field, T value, Action action)
    {
        if (default == action)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (SetIfChanged(ref field, value))
        {
            action();
            return true;
        }

        return false;
    }

    protected bool SetAndDoIfChanged<T>(ref T field, T value, ValueChangedHandler<T> valueChangedHandle)
    {
        if (default == valueChangedHandle)
        {
            throw new ArgumentNullException(nameof(valueChangedHandle));
        }

        if (SetIfChanged(ref field, value, out var oldValue))
        {
            valueChangedHandle(oldValue, field);
            return true;
        }

        return false;
    }
}

public delegate void ValueChangedHandler<T>(T oldValue, T newValue);
