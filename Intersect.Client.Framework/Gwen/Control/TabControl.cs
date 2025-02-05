using Intersect.Client.Framework.Gwen.ControlInternal;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     Control with multiple tabs that can be reordered and dragged.
/// </summary>
public partial class TabControl : Base
{
    private readonly ScrollBarButton[] _scrollbarButtons;

    private readonly TabStrip _tabStrip;

    private TabButton? _activeButton;

    private int _scrollOffset;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TabControl" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public TabControl(Base parent, string? name = default) : base(parent, name: name)
    {
        _scrollbarButtons = new ScrollBarButton[2];
        _scrollOffset = 0;

        _tabStrip = new TabStrip(this);
        _tabStrip.StripPosition = Pos.Top;

        // Make this some special control?
        _scrollbarButtons[0] = new ScrollBarButton(this);
        _scrollbarButtons[0].SetDirectionLeft();
        _scrollbarButtons[0].Clicked += ScrollPressedLeft;
        _scrollbarButtons[0].SetSize(14, 16);

        _scrollbarButtons[1] = new ScrollBarButton(this);
        _scrollbarButtons[1].SetDirectionRight();
        _scrollbarButtons[1].Clicked += ScrollPressedRight;
        _scrollbarButtons[1].SetSize(14, 16);

        _innerPanel = new TabControlInner(this)
        {
            Dock = Pos.Fill,
        };
        _innerPanel.SendToBack();

        IsTabable = false;
    }

    /// <summary>
    ///     Determines if tabs can be reordered by dragging.
    /// </summary>
    public bool AllowReorder
    {
        get => _tabStrip.AllowReorder;
        set => _tabStrip.AllowReorder = value;
    }

    /// <summary>
    ///     Currently active tab button.
    /// </summary>
    public TabButton SelectedTab => _activeButton;

    /// <summary>
    ///     Current tab strip position.
    /// </summary>
    public Pos TabStripPosition
    {
        get => _tabStrip.StripPosition;
        set => _tabStrip.StripPosition = value;
    }

    /// <summary>
    ///     Tab strip.
    /// </summary>
    public TabStrip TabStrip => _tabStrip;

    /// <summary>
    ///     Number of tabs in the control.
    /// </summary>
    public int TabCount => _tabStrip.Children.Count;

    /// <summary>
    ///     Invoked when a tab has been added.
    /// </summary>
    public event GwenEventHandler<EventArgs>? TabAdded;

    /// <summary>
    ///     Invoked when a tab has been removed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? TabRemoved;

    public event GwenEventHandler<TabChangeEventArgs>? TabChanged;

    public TabButton AddPage(string label, string? tabName, Base? page = null)
    {
        if (page == null)
        {
            page = new Base(this, name: tabName)
            {
                Dock = Pos.Fill,
            };
        }
        else
        {
            page.Name = tabName;
            page.Parent = this;
        }

        TabButton button = new(_tabStrip)
        {
            IsTabable = false,
            Page = page,
            Text = label,
        };

        AddPage(button);

        return button;
    }

    /// <summary>
    ///     Adds a new page/tab.
    /// </summary>
    /// <param name="label">Tab label.</param>
    /// <param name="page">Page contents.</param>
    /// <returns>Newly created control.</returns>
    public TabButton AddPage(string label, Base? page = null) => AddPage(label, null, page);

    /// <summary>
    ///     Adds a page/tab.
    /// </summary>
    /// <param name="button">Page to add. (well, it's a TabButton which is a parent to the page).</param>
    public void AddPage(TabButton button)
    {
        var page = button.Page;
        page.Parent = this;
        page.IsHidden = true;
        page.Margin = Margin.Four;
        page.Dock = Pos.Fill;

        button.Parent = _tabStrip;
        button.Dock = Pos.Left;
        button.SizeToContents();

        if (button.TabControl != this)
        {
            if (button.TabControl is { } otherTabControl)
            {
                button.Clicked -= otherTabControl.OnTabPressed;
            }

            button.TabControl = this;
            button.Clicked += OnTabPressed;
        }

        if (null == _activeButton)
        {
            button.Press();
        }

        TabAdded?.Invoke(this, EventArgs.Empty);

        Invalidate();
    }

    /// <summary>
    ///     Handler for tab selection.
    /// </summary>
    /// <param name="control">Event source (TabButton).</param>
    /// <param name="args"></param>
    internal virtual void OnTabPressed(Base control, EventArgs args)
    {
        if (control is not TabButton nextTab)
        {
            return;
        }

        if (nextTab.Page is not {} page)
        {
            return;
        }

        if (_activeButton == nextTab)
        {
            return;
        }

        if (_activeButton is {} previousTab)
        {
            if (_activeButton.Page is {} previousTabPage)
            {
                previousTabPage.IsVisible = false;
            }

            _activeButton.Redraw();
        }
        else
        {
            previousTab = null;
        }

        _activeButton = nextTab;

        page.IsVisible = true;

        TabChanged?.Invoke(control, new TabChangeEventArgs
        {
            PreviousTab = previousTab,
            ActiveTab = nextTab,
        });

        _tabStrip.Invalidate();
        Invalidate();
    }

    /// <summary>
    ///     Function invoked after layout.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void PostLayout(Skin.Base skin)
    {
        base.PostLayout(skin);
        HandleOverflow();
    }

    /// <summary>
    ///     Handler for tab removing.
    /// </summary>
    /// <param name="button"></param>
    internal virtual void OnLoseTab(TabButton button)
    {
        if (_activeButton == button)
        {
            _activeButton = null;
        }

        //TODO: Select a tab if any exist.

        if (TabRemoved != null)
        {
            TabRemoved.Invoke(this, EventArgs.Empty);
        }

        Invalidate();
    }

    private void HandleOverflow()
    {
        var tabsSize = _tabStrip.GetChildrenSize();

        // Only enable the scrollers if the tabs are at the top.
        // This is a limitation we should explore.
        // Really TabControl should have derivitives for tabs placed elsewhere where we could specialize
        // some functions like this for each direction.
        var needed = tabsSize.X > Width && _tabStrip.Dock == Pos.Top;

        _scrollbarButtons[0].IsHidden = !needed;
        _scrollbarButtons[1].IsHidden = !needed;

        if (!needed)
        {
            return;
        }

        _scrollOffset = Util.Clamp(_scrollOffset, 0, tabsSize.X - Width + 32);

#if false //
// This isn't frame rate independent.
// Could be better. Get rid of m_ScrollOffset and just use m_TabStrip.GetMargin().left ?
// Then get a margin animation type and do it properly!
// TODO!
//
    m_TabStrip.SetMargin( Margin( Gwen::Approach( m_TabStrip.GetMargin().left, m_iScrollOffset * -1, 2 ), 0, 0, 0 ) );
    InvalidateParent();
#else
        _tabStrip.Margin = new Margin(_scrollOffset * -1, 0, 0, 0);
#endif

        _scrollbarButtons[0].SetPosition(Width - 30, 5);
        _scrollbarButtons[1].SetPosition(_scrollbarButtons[0].Right, 5);
    }

    protected virtual void ScrollPressedLeft(Base control, EventArgs args)
    {
        _scrollOffset -= 120;
    }

    protected virtual void ScrollPressedRight(Base control, EventArgs args)
    {
        _scrollOffset += 120;
    }

}
