using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Intersect.Client.Framework.Input;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;

/// <summary>
///     ComboBox control.
/// </summary>
public partial class ComboBox : Button
{
    private readonly Base _arrowIcon;
    private readonly Menu _menu;

    private Point _itemMaximumSize;

    private string mCloseMenuSound;

    private string mHoverItemSound;

    private string mHoverMenuSound;

    private bool mMenuAbove;

    //Sound Effects
    private string mOpenMenuSound;

    private MenuItem? mSelectedItem;

    private string mSelectItemSound;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ComboBox" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public ComboBox(Base parent, string? name = default) : base(parent, name)
    {
        Size = new Point(100, 20);
        _menu = new Menu(this, name: nameof(_menu))
        {
            IsHidden = true,
            IconMarginDisabled = true,
            IsTabable = false,
            MaximumSize = new Point(100, 200),
        };

        _arrowIcon = new DownArrow(this, name: nameof(_arrowIcon))
        {
            Margin = new Margin(4, 0, 0, 0),
        };

        TextAlign = Pos.Left | Pos.CenterV;
        Text = string.Empty;

        AutoSizeToContents = true;
        IsTabable = true;
        KeyboardInputEnabled = true;
    }

    /// <summary>
    ///     Indicates whether the combo menu is open.
    /// </summary>
    public bool IsOpen => _menu.IsVisible;

    /// <summary>
    ///     Selected item.
    /// </summary>
    /// <remarks>Not just String property, because items also have internal names.</remarks>
    public MenuItem? SelectedItem
    {
        get => mSelectedItem;
        set
        {
            if (value == mSelectedItem)
            {
                return;
            }

            if (value == null)
            {
                mSelectedItem = null;
                // TODO: Event?
                return;
            }

            if (value.Parent == _menu)
            {
                mSelectedItem = value;
                OnItemSelected(mSelectedItem, new ItemSelectedEventArgs(value, true, mSelectedItem.UserData));
                return;
            }

            ApplicationContext.CurrentContext.Logger.LogWarning(
                "Tried to set selected item of {ComponentTypeName} '{ComponentName}' to '{SelectionName}' ({SelectionType})",
                GetType().GetName(qualified: true),
                CanonicalName,
                value.Name,
                value.GetType().GetName(qualified: true)
            );

            mSelectedItem = null;
        }
    }

    internal override bool IsMenuComponent => true;

    /// <summary>
    ///     Invoked when the selected item has changed.
    /// </summary>
    public event GwenEventHandler<ItemSelectedEventArgs>? ItemSelected;

    public void SetMenuAbove()
    {
        mMenuAbove = true;
        if (IsOpen)
        {
            Open();
        }
    }

    public void SetMenuBelow()
    {
        mMenuAbove = false;
        if (IsOpen)
        {
            Open();
        }
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var obj = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (obj is null)
        {
            return null;
        }

        obj.Add("MenuAbove", mMenuAbove);
        obj.Add("DropDownButton", _arrowIcon.GetJson());
        obj.Add("OpenMenuSound", mOpenMenuSound);
        obj.Add("CloseMenuSound", mCloseMenuSound);
        obj.Add("HoverMenuSound", mHoverMenuSound);
        obj.Add("ItemHoverSound", mHoverItemSound);
        obj.Add("SelectItemSound", mSelectItemSound);

        return base.FixJson(obj);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["MenuAbove"] != null)
        {
            mMenuAbove = (bool)obj["MenuAbove"];
        }

        if (obj["Menu"] != null)
        {
            _menu.LoadJson(obj["Menu"]);
        }

        if (obj["DropDownButton"] != null)
        {
            _arrowIcon.LoadJson(obj["DropDownButton"]);
        }

        if (obj["OpenMenuSound"] != null)
        {
            mOpenMenuSound = (string)obj["OpenMenuSound"];
        }

        if (obj["CloseMenuSound"] != null)
        {
            mCloseMenuSound = (string)obj["CloseMenuSound"];
        }

        if (obj["HoverMenuSound"] != null)
        {
            mHoverMenuSound = (string)obj["HoverMenuSound"];
        }

        if (obj["ItemHoverSound"] != null)
        {
            mHoverItemSound = (string)obj["ItemHoverSound"];
        }

        if (obj["SelectItemSound"] != null)
        {
            mSelectItemSound = (string)obj["SelectItemSound"];
        }

        foreach (var child in Children)
        {
            if (child is Menu menu)
            {
                foreach (var menuChild in menu.Children)
                {
                    if (menuChild is MenuItem menuItem)
                    {
                        menuItem.SetHoverSound(mHoverItemSound);
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Adds a new item.
    /// </summary>
    /// <param name="label">Item label (displayed).</param>
    /// <param name="name">Item name.</param>
    /// <returns>Newly created control.</returns>
    public virtual MenuItem AddItem(string label, string? name = default, object? userData = default)
    {
        var item = _menu.AddItem(label, null, "", "", Font);
        item.Name = name;
        item.Selected += OnItemSelected;
        item.UserData = userData;
        item.SetTextColor(GetTextColor(ComponentState.Normal), ComponentState.Normal);
        item.SetTextColor(GetTextColor(ComponentState.Hovered), ComponentState.Hovered);
        item.SetTextColor(GetTextColor(ComponentState.Active), ComponentState.Active);
        item.SetTextColor(GetTextColor(ComponentState.Disabled), ComponentState.Disabled);
        item.SetHoverSound(mHoverItemSound);

        UpdateItemMaximumSize(label);

        if (mSelectedItem == null)
        {
            OnItemSelected(item, new ItemSelectedEventArgs(null, true, selectedUserData: null));
        }

        return item;
    }

    private void UpdateItemMaximumSize(string item)
    {
        var itemSize = Skin.Renderer.MeasureText(Font, item);
        _itemMaximumSize.X = Math.Max(_itemMaximumSize.X, itemSize.X);
        _itemMaximumSize.Y = Math.Max(_itemMaximumSize.Y, itemSize.Y);
    }

    protected override void OnFontChanged(Base sender, GameFont? oldFont, GameFont? newFont)
    {
        _itemMaximumSize = default;
        foreach (var item in _menu.MenuItems)
        {
            UpdateItemMaximumSize(item.Text ?? string.Empty);
        }
    }

    /// <summary>
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawComboBox(this, IsActive, IsOpen);
    }

    public override void Disable()
    {
        base.Disable();
        Canvas?.CloseMenus();
    }

    /// <summary>
    ///     Internal Pressed implementation.
    /// </summary>
    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        var canvas = Canvas;

        if (IsOpen)
        {
            canvas?.CloseMenus();

            return;
        }

        var wasMenuHidden = _menu.IsHidden;
        canvas?.CloseMenus();

        if (wasMenuHidden)
        {
            Open();
        }

        base.OnMouseClicked(mouseButton, mousePosition, userAction);
    }

    /// <summary>
    ///     Removes all items.
    /// </summary>
    public virtual void DeleteAll()
    {
        _itemMaximumSize = default;
        _menu.DeleteAll();
    }

    /// <summary>
    ///     Internal handler for item selected event.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void OnItemSelected(Base control, ItemSelectedEventArgs args)
    {
        if (!IsDisabled)
        {
            //Convert selected to a menu item
            var item = control as MenuItem;
            if (null == item)
            {
                return;
            }

            mSelectedItem = item;
            Text = mSelectedItem.Text;
            _menu.IsHidden = true;

            ItemSelected?.Invoke(this, args);

            if (!args.Automated)
            {
                base.PlaySound(mSelectItemSound);
            }

            Focus();
            Invalidate();
        }
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        _arrowIcon.Position(Pos.Right | Pos.CenterV, 4, 0);

        base.Layout(skin);
    }

    /// <summary>
    ///     Handler for losing keyboard focus.
    /// </summary>
    protected override void OnLostKeyboardFocus()
    {
        if (GetTextColor(ComponentState.Normal) != null)
        {
            TextColor = GetTextColor(ComponentState.Normal);

            return;
        }

        TextColor = Color.Black;
    }

    /// <summary>
    ///     Handler for gaining keyboard focus.
    /// </summary>
    protected override void OnKeyboardFocus()
    {
        //Until we add the blue highlighting again
        if (GetTextColor(ComponentState.Normal) != null)
        {
            TextColor = GetTextColor(ComponentState.Normal);

            return;
        }

        TextColor = Color.White;
    }

    /// <summary>
    ///     Opens the combo.
    /// </summary>
    public virtual void Open()
    {
        if (IsDisabledByTree)
        {
            return;
        }

        _menu.Parent = GetCanvas();
        _menu.IsHidden = false;
        _menu.BringToFront();

        var p = LocalPosToCanvas(Point.Empty);
        var height = _menu.Children.OfType<MenuItem>().Sum(child => child.Height);
        var width = Width;
        _menu.MaximumSize = _menu.MaximumSize with { X = width };
        if (mMenuAbove)
        {
            _menu.RestrictToParent = false;
            _menu.SetBounds(new Rectangle(p.X, p.Y - _menu.Height, width, height));
            _menu.RestrictToParent = true;
        }
        else
        {
            _menu.SetBounds(new Rectangle(p.X, p.Y + Height, width, height));
        }

        base.PlaySound(mOpenMenuSound);
    }

    /// <summary>
    ///     Closes the combo.
    /// </summary>
    public virtual void Close()
    {
        _menu.Hide();

        base.PlaySound(mCloseMenuSound);
    }

    /// <summary>
    ///     Handler for Down Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyDown(bool down)
    {
        if (!down)
        {
            return true;
        }

        var it = _menu.Children.FindIndex(x => x == mSelectedItem);
        if (it + 1 >= _menu.Children.Count)
        {
            return true;
        }

        var selectedItem = _menu.Children[it + 1];
        OnItemSelected(this, new ItemSelectedEventArgs(selectedItem, selectedUserData: selectedItem.UserData));

        return true;
    }

    /// <summary>
    ///     Handler for Up Arrow keyboard event.
    /// </summary>
    /// <param name="down">Indicates whether the key was pressed or released.</param>
    /// <returns>
    ///     True if handled.
    /// </returns>
    protected override bool OnKeyUp(bool down)
    {
        if (!down)
        {
            return true;
        }

        var it = _menu.Children.FindLastIndex(x => x == mSelectedItem);
        if (it - 1 < 0)
        {
            return true;
        }

        var selectedItem = _menu.Children[it - 1];
        OnItemSelected(this, new ItemSelectedEventArgs(selectedItem, selectedUserData: selectedItem.UserData));

        return true;
    }

    /// <summary>
    ///     Renders the focus overlay.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderFocus(Skin.Base skin)
    {
    }

    /// <summary>
    ///     Selects the first menu item with the given text it finds.
    ///     If a menu item can not be found that matches input, nothing happens.
    /// </summary>
    /// <param name="label">The label to look for, this is what is shown to the user.</param>
    public bool SelectByText(string text)
    {
        foreach (MenuItem item in _menu.Children)
        {
            if (item.Text == text)
            {
                SelectedItem = item;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Selects the first menu item with the given internal name it finds.
    ///     If a menu item can not be found that matches input, nothing happens.
    /// </summary>
    /// <param name="name">The internal name to look for. To select by what is displayed to the user, use "SelectByText".</param>
    public bool SelectByName(string name)
    {
        foreach (MenuItem item in _menu.Children)
        {
            if (item.Name == name)
            {
                SelectedItem = item;

                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Selects the first menu item with the given user data it finds.
    ///     If a menu item can not be found that matches input, nothing happens.
    /// </summary>
    /// <param name="userdata">
    ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
    ///     If null is passed in, it will look for null/unset UserData.
    /// </param>
    public virtual bool SelectByUserData(object? userdata)
    {
        foreach (MenuItem item in _menu.Children)
        {
            if (userdata == null)
            {
                if (item.UserData == null)
                {
                    SelectedItem = item;

                    return true;
                }
            }
            else if (userdata.Equals(item.UserData))
            {
                SelectedItem = item;

                return true;
            }
        }

        return false;
    }

    public void SetMenuBackgroundColor(Color clr)
    {
        _menu.RenderColor = clr;
    }

    public void SetMenuMaxSize(int w, int h)
    {
        _menu.MaximumSize = new Point(w, h);
    }

    protected override Point GetContentSize() => _itemMaximumSize == default ? base.GetContentSize() : _itemMaximumSize + new Point(4, 0);

    protected override Padding GetContentPadding()
    {
        var padding = base.GetContentPadding();
        padding.Right += _arrowIcon.OuterWidth;
        return padding;
    }

    public override void SetTextColor(Color clr, ComponentState state)
    {
        base.SetTextColor(clr, state);
        foreach (MenuItem itm in _menu.Children)
        {
            itm.SetTextColor(clr, state);
        }
    }

    protected override void OnMouseEntered()
    {
        base.OnMouseEntered();

        //Play Mouse Entered Sound
        if (!IsOpen && ShouldDrawHover)
        {
            base.PlaySound(mHoverMenuSound);
        }
    }

}
