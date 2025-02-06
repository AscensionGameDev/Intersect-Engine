using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Menu item.
/// </summary>
public partial class MenuItem : Button
{
    private bool _checkable;
    private bool _checked;
    private bool _onStrip;

    private Label? _accelerator;
    private Menu? _submenu;
    private Base? _submenuArrow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MenuItem" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    public MenuItem(Base parent) : base(parent)
    {
        AutoSizeToContents = true;
        _onStrip = false;
        IsTabable = false;
        IsCheckable = false;
        IsChecked = false;

        _accelerator = new Label(this);
    }

    /// <summary>
    ///     Indicates whether the item is on a menu strip.
    /// </summary>
    public bool IsOnStrip
    {
        get => _onStrip;
        set => _onStrip = value;
    }

    /// <summary>
    ///     Determines if the menu item is checkable.
    /// </summary>
    public bool IsCheckable
    {
        get => _checkable;
        set => _checkable = value;
    }

    /// <summary>
    ///     Indicates if the parent menu is open.
    /// </summary>
    public bool IsMenuOpen
    {
        get
        {
            if (_submenu == null)
            {
                return false;
            }

            return !_submenu.IsHidden;
        }
    }

    /// <summary>
    ///     Gets or sets the check value.
    /// </summary>
    public bool IsChecked
    {
        get => _checked;
        set
        {
            if (value == _checked)
            {
                return;
            }

            _checked = value;

            if (CheckChanged != null)
            {
                CheckChanged.Invoke(this, EventArgs.Empty);
            }

            if (value)
            {
                if (Checked != null)
                {
                    Checked.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (UnChecked != null)
                {
                    UnChecked.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///     Gets the parent menu.
    /// </summary>
    public Menu Menu
    {
        get
        {
            if (null != _submenu)
            {
                return _submenu;
            }

            _submenu = new Menu(Canvas ?? throw new InvalidOperationException("No canvas"));
            _submenu.IsHidden = true;

            if (!_onStrip)
            {
                _submenuArrow?.Dispose();
                _submenuArrow = new RightArrow(this);
                _submenuArrow.SetSize(15, 15);
            }

            Invalidate();

            return _submenu;
        }
    }

    /// <summary>
    ///     Invoked when the item is selected.
    /// </summary>
    public event GwenEventHandler<ItemSelectedEventArgs>? Selected;

    /// <summary>
    ///     Invoked when the item is checked.
    /// </summary>
    public event GwenEventHandler<EventArgs>? Checked;

    /// <summary>
    ///     Invoked when the item is unchecked.
    /// </summary>
    public event GwenEventHandler<EventArgs>? UnChecked;

    /// <summary>
    ///     Invoked when the item's check value is changed.
    /// </summary>
    public event GwenEventHandler<EventArgs>? CheckChanged;

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawMenuItem(this, IsMenuOpen, _checkable ? _checked : false);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        _submenuArrow?.Position(Pos.Right | Pos.CenterV, 4, 0);

        base.Layout(skin);
    }

    /// <summary>
    ///     Internal OnPressed implementation.
    /// </summary>
    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        if (_submenu != null)
        {
            ToggleMenu();
        }
        else if (!_onStrip)
        {
            IsChecked = !IsChecked;
            Selected?.Invoke(this, new ItemSelectedEventArgs(this, selectedUserData: UserData));
            Canvas?.CloseMenus();
        }

        base.OnMouseClicked(mouseButton, mousePosition, userAction);
    }

    /// <summary>
    ///     Toggles the menu open state.
    /// </summary>
    public void ToggleMenu()
    {
        if (IsMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    /// <summary>
    ///     Opens the menu.
    /// </summary>
    public void OpenMenu()
    {
        if (null == _submenu)
        {
            return;
        }

        _submenu.IsHidden = false;
        _submenu.BringToFront();

        var p = LocalPosToCanvas(Point.Empty);

        // Strip menus open downwards
        if (_onStrip)
        {
            _submenu.SetPosition(p.X, p.Y + Height + 1);
        }

        // Submenus open sidewards
        else
        {
            _submenu.SetPosition(p.X + Width, p.Y);
        }

        // TODO: Option this.
        // TODO: Make sure on screen, open the other side of the
        // parent if it's better...
    }

    /// <summary>
    ///     Closes the menu.
    /// </summary>
    public void CloseMenu()
    {
        if (null == _submenu)
        {
            return;
        }

        _submenu.Close();
        _submenu.CloseAll();
    }

    protected override Point GetContentSize()
    {
        var contentSize = base.GetContentSize();
        if (_accelerator is { } accelerator)
        {
            contentSize.X += accelerator.Width;
        }

        if (Parent is not { } parent)
        {
            return contentSize;
        }

        var parentWidth = parent.Width;
        contentSize.X = Math.Max(contentSize.X, parentWidth);
        return contentSize;
    }

    public override bool SizeToContents()
    {
        _accelerator?.SizeToContents();

        var sizeChanged = base.SizeToContents();

        if (_accelerator != null)
        {
            _accelerator.TextAlign = Pos.Left;
        }

        return sizeChanged;
    }

    public MenuItem SetAction(
        GwenEventHandler<EventArgs> handler,
        GwenEventHandler<ItemSelectedEventArgs> selHandler
    )
    {
        if (_accelerator != null)
        {
            AddAccelerator(_accelerator.Text, handler);
        }

        Selected += selHandler;

        return this;
    }

    public void SetAccelerator(string? accelerator)
    {
        if (string.IsNullOrWhiteSpace(accelerator))
        {
            _accelerator = null;
            return;
        }

        _accelerator = new Label(this)
        {
            TextAlign = Pos.Right | Pos.CenterV,
            Dock = Pos.Right,
            Margin = new Margin(
                0,
                0,
                16,
                0
            ),
            Text = accelerator,
        };
    }
}
