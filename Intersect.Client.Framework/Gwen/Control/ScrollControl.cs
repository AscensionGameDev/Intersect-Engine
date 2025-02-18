using Intersect.Client.Framework.GenericClasses;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Base for controls whose interior can be scrolled.
/// </summary>
public partial class ScrollControl : Base
{
    protected readonly ScrollPanel _scrollPanel;

    private bool _updatingScrollbars;

    private OverflowBehavior _overflowX;
    private OverflowBehavior _overflowY;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScrollControl" /> class.
    /// </summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="name">The name of the control.</param>
    public ScrollControl(Base parent, string? name = default) : base(parent, name ?? nameof(ScrollControl))
    {
        VerticalScrollBar = new VerticalScrollBar(this)
        {
            Dock = Pos.Right,
            NudgeAmount = 30,
        };
        VerticalScrollBar.BarMoved += VBarMoved;

        HorizontalScrollBar = new HorizontalScrollBar(this)
        {
            Dock = Pos.Bottom,
            NudgeAmount = 30,
        };
        HorizontalScrollBar.BarMoved += HBarMoved;

        _scrollPanel = new ScrollPanel(this, name: nameof(_scrollPanel));
        _scrollPanel.SendToBack();

        _innerPanel = _scrollPanel;

        _overflowX = OverflowBehavior.Hidden;
        _overflowY = OverflowBehavior.Auto;
        MouseInputEnabled = false;
    }

    public Padding InnerPanelPadding
    {
        get => _scrollPanel.Padding;
        set => _scrollPanel.Padding = value;
    }

    public int VerticalScroll
    {
        get => _scrollPanel.Y;
        set => VerticalScrollBar.ScrollAmount = value / (float)_scrollPanel.Height;
    }

    public int HorizontalScroll
    {
        get => _scrollPanel.X;
        set => _scrollPanel.X = value;
    }

    public Base InnerPanel => _scrollPanel;

    /// <summary>
    ///     Indicates whether the control can be scrolled horizontally.
    /// </summary>
    public bool CanScrollH => _overflowX is OverflowBehavior.Auto or OverflowBehavior.Scroll;

    /// <summary>
    ///     Indicates whether the control can be scrolled vertically.
    /// </summary>
    public bool CanScrollV => _overflowY is OverflowBehavior.Auto or OverflowBehavior.Scroll;

    /// <summary>
    ///     Determines whether the scroll bars should be hidden if not needed.
    /// </summary>
    public bool AutoHideBars
    {
        get => _overflowX == OverflowBehavior.Auto || _overflowY == OverflowBehavior.Auto;
        set => SetOverflow(_overflowX.SetAutoHide(value), _overflowY.SetAutoHide(value));
    }

    public ScrollBar VerticalScrollBar { get; }

    public ScrollBar HorizontalScrollBar { get; }

    public OverflowBehavior Overflow
    {
        set => SetOverflow(value, value);
    }

    public OverflowBehavior OverflowX
    {
        get => _overflowX;
        set => SetOverflow(value, _overflowY);
    }

    public OverflowBehavior OverflowY
    {
        get => _overflowY;
        set => SetOverflow(_overflowX, value);
    }

    public void SetOverflow(OverflowBehavior overflowX, OverflowBehavior overflowY)
    {
        if (_overflowX == overflowX && _overflowY == overflowY)
        {
            return;
        }

        var enableScrollX = overflowX.AllowsScrolling();
        var enableScrollY = overflowY.AllowsScrolling();

        _overflowX = overflowX;
        _overflowY = overflowY;

        EnableScroll(enableScrollX, enableScrollY, true);

        Invalidate();
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add(nameof(OverflowX), _overflowX.ToString());
        serializedProperties.Add(nameof(OverflowY), _overflowY.ToString());
        serializedProperties.Add(nameof(HorizontalScrollBar), HorizontalScrollBar.GetJson());
        serializedProperties.Add(nameof(VerticalScrollBar), VerticalScrollBar.GetJson());

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token, isRoot);

        if (token is not JObject obj)
        {
            return;
        }

        var overflowX = _overflowX;
        if (obj.TryGetValue(nameof(OverflowX), out var tokenOverflowX) &&
            tokenOverflowX is JValue { Type: JTokenType.String } valueOverflowX)
        {
            _ = Enum.TryParse(valueOverflowX.Value<string>(), out overflowX);
        }

        var overflowY = _overflowY;
        if (obj.TryGetValue(nameof(OverflowY), out var tokenOverflowY) &&
            tokenOverflowY is JValue { Type: JTokenType.String } valueOverflowY)
        {
            _ = Enum.TryParse(valueOverflowY.Value<string>(), out overflowY);
        }

        SetOverflow(overflowX, overflowY);

        if (obj[nameof(HorizontalScrollBar)] is {} horizontalScrollbarToken)
        {
            HorizontalScrollBar.LoadJson(horizontalScrollbarToken);
        }

        if (obj[nameof(VerticalScrollBar)] is {} verticalScrollbarToken)
        {
            VerticalScrollBar.LoadJson(verticalScrollbarToken);
        }
    }

    /// <summary>
    ///     Enables or disables inner scrollbars.
    /// </summary>
    /// <param name="horizontal">Determines whether the horizontal scrollbar should be enabled.</param>
    /// <param name="vertical">Determines whether the vertical scrollbar should be enabled.</param>
    public void EnableScroll(bool horizontal, bool vertical) => EnableScroll(horizontal, vertical, false);

    protected virtual void EnableScroll(bool horizontal, bool vertical, bool internalSet)
    {
        if (!internalSet)
        {
            _overflowX = _overflowX.SetAllowScrolling(horizontal);
            _overflowY = _overflowY.SetAllowScrolling(vertical);
            Invalidate();
        }

        UpdateScrollBars();
    }

    public virtual void SetInnerSize(int width, int height) => _scrollPanel.SetSize(width, height);

    protected virtual void VBarMoved(Base control, EventArgs args) => Invalidate();

    protected virtual void HBarMoved(Base control, EventArgs args) => Invalidate();

    /// <summary>
    ///     Handler invoked when control children's bounds change.
    /// </summary>
    /// <param name="child"></param>
    /// <param name="oldChildBounds"></param>
    /// <param name="newChildBounds"></param>
    protected override void OnChildBoundsChanged(Base child, Rectangle oldChildBounds, Rectangle newChildBounds)
    {
        if (_innerPanel == null)
        {
            return;
        }

        UpdateScrollBars();
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        UpdateScrollBars();
        base.Layout(skin);
    }

    /// <summary>
    ///     Handler invoked on mouse wheel event.
    /// </summary>
    /// <param name="delta">Scroll delta.</param>
    /// <returns></returns>
    protected override bool OnMouseWheeled(int delta)
    {
        if (CanScrollV && VerticalScrollBar.IsVisibleInTree)
        {
            var scrollAmount = VerticalScrollBar.ScrollAmount - VerticalScrollBar.NudgeAmount * (delta / 60.0f);
            if (VerticalScrollBar.SetScrollAmount(scrollAmount))
            {
                return true;
            }
        }

        if (!CanScrollH || !HorizontalScrollBar.IsVisibleInTree)
        {
            return false;
        }

        return HorizontalScrollBar.SetScrollAmount(
            HorizontalScrollBar.ScrollAmount - HorizontalScrollBar.NudgeAmount * (delta / 60.0f), true
        );
    }

    /// <summary>
    ///     Handler invoked on mouse wheel event.
    /// </summary>
    /// <param name="delta">Scroll delta.</param>
    /// <returns></returns>
    protected override bool OnMouseHWheeled(int delta)
    {
        if (!CanScrollH || !HorizontalScrollBar.IsVisibleInTree)
        {
            return false;
        }

        var scrollValue = HorizontalScrollBar.ScrollAmount - HorizontalScrollBar.NudgeAmount * (delta / 60.0f);
        return HorizontalScrollBar.SetScrollAmount(scrollValue, true);
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
#if false // Debug render - this shouldn't render ANYTHING REALLY - it should be up to the parent!
Gwen::Rect rect = GetRenderBounds();
Gwen::Renderer::Base* render = skin->GetRender();

render->SetDrawColor( Gwen::Color( 255, 255, 0, 100 ) );
render->DrawFilledRect( rect );

render->SetDrawColor( Gwen::Color( 255, 0, 0, 100 ) );
render->DrawFilledRect( m_InnerPanel->GetBounds() );

render->RenderText( skin->GetDefaultFont(), Gwen::Point( 0, 0 ), Utility::Format( L"Offset: %i %i", m_InnerPanel->X(), m_InnerPanel->Y() ) );
#endif
    }

    private static void UpdateScrollbar(ScrollBar scrollBar, bool show, float ratio)
    {
        if (show)
        {
            scrollBar.IsDisabled = ratio > 1;
            if (scrollBar.IsVisibleInTree)
            {
                return;
            }

            scrollBar.IsVisibleInTree = true;
            scrollBar.SetScrollAmount(0, forceUpdate: true);
        }
        else
        {
            scrollBar.IsVisibleInTree = false;
            scrollBar.IsDisabled = true;
        }
    }

    public virtual void UpdateScrollBars()
    {
        if (_updatingScrollbars)
        {
            return;
        }

        _updatingScrollbars = true;

        //Get the max size of all our children together
        var childrenWidth = Children.Count > 0 ? Children.Max(x => x.Right) : 0;
        var childrenHeight = Children.Count > 0 ? Children.Max(x => x.Bottom) : 0;

        var canScrollH = CanScrollH;
        var canScrollV = CanScrollV;

        var verticalScrollbarWidth = VerticalScrollBar.IsHidden ? 0 : VerticalScrollBar.Width;
        var availableWidth = Width - verticalScrollbarWidth;

        var horizontalScrollbarHeight = HorizontalScrollBar.IsHidden ? 0 : HorizontalScrollBar.Height;
        var availableHeight = Height - horizontalScrollbarHeight;

        var updatedWidth = canScrollH ? Math.Max(InnerWidth, childrenWidth) : availableWidth;
        var updatedHeight = canScrollV ? Math.Max(InnerHeight, childrenHeight) : availableHeight;

        var innerPanelMargin = _scrollPanel.Margin;
        var innerPanelWidth = updatedWidth - (innerPanelMargin.Left + innerPanelMargin.Right);
        var innerPanelHeight = updatedHeight - (innerPanelMargin.Top + innerPanelMargin.Bottom);

        _scrollPanel.SetSize(innerPanelWidth, innerPanelHeight);

        var widthRatio = Width / (float)(childrenWidth + verticalScrollbarWidth);
        var heightRatio = Height / (float)(childrenHeight + horizontalScrollbarHeight);

        VerticalScrollBar.ContentSize = _scrollPanel.Height;
        VerticalScrollBar.ViewableContentSize = availableHeight;

        HorizontalScrollBar.ContentSize = _scrollPanel.Width;
        HorizontalScrollBar.ViewableContentSize = availableWidth;

        var showScrollH = _overflowX == OverflowBehavior.Scroll || _overflowX == OverflowBehavior.Auto && widthRatio < 1;
        UpdateScrollbar(HorizontalScrollBar, showScrollH, widthRatio);

        var showScrollV = _overflowY == OverflowBehavior.Scroll || _overflowY == OverflowBehavior.Auto && heightRatio < 1;
        UpdateScrollbar(VerticalScrollBar, showScrollV, heightRatio);

        var newInnerPanelPosX = 0;
        var newInnerPanelPosY = 0;

        _pixelsPerScrollAmountY = -(_scrollPanel.Height - Height + horizontalScrollbarHeight);
        if (showScrollV)
        {
            newInnerPanelPosY = (int)(_pixelsPerScrollAmountY * VerticalScrollBar.ScrollAmount);
        }

        _pixelsPerScrollAmountX = -(_scrollPanel.Width - Width + verticalScrollbarWidth);
        if (showScrollH)
        {
            newInnerPanelPosX = (int)(_pixelsPerScrollAmountX * HorizontalScrollBar.ScrollAmount);
        }

        _scrollPanel.SetPosition(newInnerPanelPosX, newInnerPanelPosY);
        _updatingScrollbars = false;

        // ApplicationContext.CurrentContext.Logger.LogTrace(
        //     "Updated {ControlTypeName} '{ControlCanonicalName}' scrollbars Available=({AvailableSize}) Content=({ContentSize}) Show Scrollbars ({ShowScrollbarX}, {ShowScrollbarY})",
        //     GetType().GetName(qualified: true),
        //     CanonicalName,
        //     new Point(availableWidth, availableHeight),
        //     new Point(updatedWidth, updatedHeight),
        //     showScrollH,
        //     showScrollV
        // );
    }

    private float _pixelsPerScrollAmountX;
    private float _pixelsPerScrollAmountY;

    public void ScrollToX(int x)
    {
        var scrollAmount =  x / _pixelsPerScrollAmountX;
        HorizontalScrollBar.ScrollAmount = scrollAmount;
    }

    public void ScrollToY(int y)
    {
        var scrollAmount = y / _pixelsPerScrollAmountY;
        VerticalScrollBar.ScrollAmount = scrollAmount;
    }

    protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.OnBoundsChanged(oldBounds, newBounds);
    }

    public override Point GetChildrenSize()
    {
        var childrenSize = base.GetChildrenSize();
        return childrenSize;
    }

    public override void Invalidate()
    {
        base.Invalidate();
    }

    public virtual void ScrollToBottom()
    {
        if (!CanScrollV)
        {
            return;
        }

        UpdateScrollBars();
        VerticalScrollBar.ScrollToBottom();
    }

    public virtual void ScrollToTop()
    {
        if (!CanScrollV)
        {
            return;
        }

        UpdateScrollBars();
        VerticalScrollBar.ScrollToTop();
    }

    public virtual void ScrollToLeft()
    {
        if (!CanScrollH)
        {
            return;
        }

        UpdateScrollBars();
        VerticalScrollBar.ScrollToLeft();
    }

    public virtual void ScrollToRight()
    {
        if (!CanScrollH)
        {
            return;
        }

        UpdateScrollBars();
        VerticalScrollBar.ScrollToRight();
    }

    public virtual void DeleteAll() => _scrollPanel.DeleteAllChildren();
}
