using Intersect.Client.Framework.GenericClasses;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Base for controls whose interior can be scrolled.
/// </summary>
public partial class ScrollControl : Base
{
    private bool _autoHideBars;

    private bool _canScrollH;

    private bool _canScrollV;

    private bool mUpdatingScrollbars;

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

        _innerPanel = new Base(this)
        {
            X = 0,
            Y = 0,
            Margin = Margin.Five,
            MouseInputEnabled = false,
        };
        _innerPanel.SendToBack();

        _autoHideBars = false;
        _canScrollH = true;
        _canScrollV = true;
        MouseInputEnabled = false;
    }

    public int VerticalScroll => _innerPanel?.Y ?? 0;

    public int HorizontalScroll => _innerPanel?.X ?? 0;

    public Base InnerPanel => _innerPanel ??
                              throw new InvalidOperationException($"{nameof(ScrollControl)} had the inner panel unset");

    /// <summary>
    ///     Indicates whether the control can be scrolled horizontally.
    /// </summary>
    public bool CanScrollH => _canScrollH;

    /// <summary>
    ///     Indicates whether the control can be scrolled vertically.
    /// </summary>
    public bool CanScrollV => _canScrollV;

    /// <summary>
    ///     Determines whether the scroll bars should be hidden if not needed.
    /// </summary>
    public bool AutoHideBars
    {
        get => _autoHideBars;
        set => _autoHideBars = value;
    }

    public ScrollBar VerticalScrollBar { get; }

    public ScrollBar HorizontalScrollBar { get; }

    protected bool HScrollRequired
    {
        set
        {
            if (value)
            {
                HorizontalScrollBar.SetScrollAmount(0, true);
                HorizontalScrollBar.IsDisabled = true;
                if (_autoHideBars)
                {
                    HorizontalScrollBar.IsHidden = true;
                }
            }
            else
            {
                HorizontalScrollBar.IsHidden = false;
                HorizontalScrollBar.IsDisabled = false;
            }
        }
    }

    protected bool VScrollRequired
    {
        set
        {
            if (value)
            {
                if (!VerticalScrollBar.IsVisible)
                {
                    return;
                }

                VerticalScrollBar.SetScrollAmount(0, true);
                VerticalScrollBar.IsDisabled = true;
                VerticalScrollBar.IsHidden = _autoHideBars;
            }
            else
            {
                VerticalScrollBar.IsHidden = false;
                VerticalScrollBar.IsDisabled = false;
            }
        }
    }

    public override JObject GetJson(bool isRoot = default)
    {
        var obj = base.GetJson(isRoot);
        obj.Add("CanScrollH", _canScrollH);
        obj.Add("CanScrollV", _canScrollV);
        obj.Add("AutoHideBars", _autoHideBars);
        obj.Add("InnerPanel", _innerPanel.GetJson());
        obj.Add("HorizontalScrollBar", HorizontalScrollBar.GetJson());
        obj.Add("VerticalScrollBar", VerticalScrollBar.GetJson());

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["CanScrollH"] != null)
        {
            _canScrollH = (bool)obj["CanScrollH"];
        }

        if (obj["CanScrollV"] != null)
        {
            _canScrollV = (bool)obj["CanScrollV"];
        }

        if (obj["AutoHideBars"] != null)
        {
            _autoHideBars = (bool)obj["AutoHideBars"];
        }

        if (obj["InnerPanel"] != null)
        {
            _innerPanel.LoadJson(obj["InnerPanel"]);
        }

        if (obj["HorizontalScrollBar"] != null)
        {
            HorizontalScrollBar.LoadJson(obj["HorizontalScrollBar"]);
        }

        if (obj["VerticalScrollBar"] != null)
        {
            VerticalScrollBar.LoadJson(obj["VerticalScrollBar"]);
        }
    }

    /// <summary>
    ///     Enables or disables inner scrollbars.
    /// </summary>
    /// <param name="horizontal">Determines whether the horizontal scrollbar should be enabled.</param>
    /// <param name="vertical">Determines whether the vertical scrollbar should be enabled.</param>
    public virtual void EnableScroll(bool horizontal, bool vertical)
    {
        _canScrollV = vertical;
        _canScrollH = horizontal;
        VerticalScrollBar.IsHidden = !_canScrollV;
        HorizontalScrollBar.IsHidden = !_canScrollH;
    }

    public virtual void SetInnerSize(int width, int height)
    {
        _innerPanel.SetSize(width, height);
    }

    protected virtual void VBarMoved(Base control, EventArgs args)
    {
        Invalidate();
    }

    protected virtual void HBarMoved(Base control, EventArgs args)
    {
        Invalidate();
    }

    /// <summary>
    ///     Handler invoked when control children's bounds change.
    /// </summary>
    /// <param name="oldChildBounds"></param>
    /// <param name="child"></param>
    protected override void OnChildBoundsChanged(Rectangle oldChildBounds, Base child)
    {
        //UpdateScrollBars();
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
        if (CanScrollV && VerticalScrollBar.IsVisible)
        {
            if (VerticalScrollBar.SetScrollAmount(
                VerticalScrollBar.ScrollAmount - VerticalScrollBar.NudgeAmount * (delta / 60.0f), true
            ))
            {
                return true;
            }
        }

        if (!CanScrollH || !HorizontalScrollBar.IsVisible)
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
        if (!CanScrollH || !HorizontalScrollBar.IsVisible)
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

    public virtual void UpdateScrollBars()
    {
        if (_innerPanel == null)
        {
            return;
        }

        if (mUpdatingScrollbars)
        {
            return;
        }

        mUpdatingScrollbars = true;

        //Get the max size of all our children together
        var childrenWidth = Children.Count > 0 ? Children.Max(x => x.Right) : 0;
        var childrenHeight = Children.Count > 0 ? Children.Max(x => x.Bottom) : 0;


        var updatedWidth = _canScrollH
            ? Math.Max(Width, childrenWidth)
            : Width - (VerticalScrollBar.IsHidden ? 0 : VerticalScrollBar.Width);
        var updatedHeight = Math.Max(Height, childrenHeight);

        _innerPanel.SetSize(updatedWidth, updatedHeight);

        var wPercent = Width /
                       (float)(childrenWidth + (VerticalScrollBar.IsHidden ? 0 : VerticalScrollBar.Width));

        var hPercent = Height /
                       (float)(childrenHeight +
                               (HorizontalScrollBar.IsHidden ? 0 : HorizontalScrollBar.Height));

        if (_canScrollV)
        {
            VScrollRequired = hPercent >= 1;
        }
        else
        {
            VerticalScrollBar.IsHidden = true;
        }

        if (_canScrollH)
        {
            HScrollRequired = wPercent >= 1;
        }
        else
        {
            HorizontalScrollBar.IsHidden = true;
        }

        VerticalScrollBar.ContentSize = _innerPanel.Height;
        VerticalScrollBar.ViewableContentSize =
            Height - (HorizontalScrollBar.IsHidden ? 0 : HorizontalScrollBar.Height);

        HorizontalScrollBar.ContentSize = _innerPanel.Width;
        HorizontalScrollBar.ViewableContentSize =
            Width - (VerticalScrollBar.IsHidden ? 0 : VerticalScrollBar.Width);

        var newInnerPanelPosX = 0;
        var newInnerPanelPosY = 0;

        if (CanScrollV && !VerticalScrollBar.IsHidden)
        {
            newInnerPanelPosY =
                (int)(-(_innerPanel.Height -
                        Height +
                        (HorizontalScrollBar.IsHidden ? 0 : HorizontalScrollBar.Height)) *
                      VerticalScrollBar.ScrollAmount);
        }

        if (CanScrollH && !HorizontalScrollBar.IsHidden)
        {
            newInnerPanelPosX =
                (int)(-(_innerPanel.Width -
                        Width +
                        (VerticalScrollBar.IsHidden ? 0 : VerticalScrollBar.Width)) *
                      HorizontalScrollBar.ScrollAmount);
        }

        _innerPanel.SetPosition(newInnerPanelPosX, newInnerPanelPosY);
        mUpdatingScrollbars = false;
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

    public virtual void DeleteAll()
    {
        _innerPanel?.DeleteAllChildren();
    }
}
