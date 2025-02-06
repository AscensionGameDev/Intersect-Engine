using System.Collections;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.ControlInternal;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.Framework.Gwen.Control;


/// <summary>
///     Popup menu.
/// </summary>
public partial class Menu : ScrollControl
{

    private string mBackgroundTemplateFilename;

    private GameTexture mBackgroundTemplateTex;

    private bool mDeleteOnClose;

    private bool mDisableIconMargin;

    private GameFont mItemFont;

    //Menu Item Stuff
    private string mItemFontInfo;

    protected Color mItemHoverTextColor;

    protected Color mItemNormalTextColor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Menu" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public Menu(Base parent, string? name = default) : base(parent, name)
    {
        Size = new Point(10, 10);
        Padding = Padding.Two;
        IconMarginDisabled = false;

        DeleteOnClose = false;
        Name = name;
    }

    internal override bool IsMenuComponent => true;

    public bool IconMarginDisabled
    {
        get => mDisableIconMargin;
        set => mDisableIconMargin = value;
    }

    /// <summary>
    ///     Determines whether the menu should be disposed on close.
    /// </summary>
    public bool DeleteOnClose
    {
        get => mDeleteOnClose;
        set => mDeleteOnClose = value;
    }

    /// <summary>
    ///     Determines whether the menu should open on mouse hover.
    /// </summary>
    protected virtual bool ShouldHoverOpenMenu => true;

    public MenuItem[] MenuItems => Children.OfType<MenuItem>().ToArray();

    /// <summary>C:\Users\JC Snider\Desktop\AGD\Intersect-Engine\Intersect.Client.Framework\Gwen\Control\Button.cs
    ///     Renders the control using specified skin.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Render(Skin.Base skin)
    {
        skin.DrawMenu(this, IconMarginDisabled);
    }

    /// <summary>
    ///     Renders under the actual control (shadows etc).
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void RenderUnder(Skin.Base skin)
    {
        base.RenderUnder(skin);
        skin.DrawShadow(this);
    }

    /// <summary>
    ///     Opens the menu.
    /// </summary>
    /// <param name="pos">Unused.</param>
    public void Open(Pos pos)
    {
        IsHidden = false;
        BringToFront();
        var mouse = Input.InputHandler.MousePosition;

        var x = mouse.X;
        var y = mouse.Y;
        if (x + Width > Canvas.Width)
        {
            x -= Width;
        }

        if (y + Height > Canvas.Height)
        {
            y -= Height;
        }

        SetPosition(x, y);
    }

    /// <summary>
    ///     Lays out the control's interior according to alignment, padding, dock etc.
    /// </summary>
    /// <param name="skin">Skin to use.</param>
    protected override void Layout(Skin.Base skin)
    {
        var childrenHeight = Children.Sum(child => child != null ? child.Height : 0);

        if (InnerPanel.MaximumSize.Y > 0 && childrenHeight > InnerPanel.MaximumSize.Y)
        {
            InnerPanel.MaximumSize = new Point(InnerPanel.MaximumSize.X, childrenHeight);
        }

        var canvasHeight = Canvas?.Height ?? Y + childrenHeight;
        if (Y + childrenHeight > canvasHeight)
        {
            childrenHeight = canvasHeight - Y;
        }

        SetSize(Width, childrenHeight);

        base.Layout(skin);
    }

    public override void Invalidate()
    {
        base.Invalidate();
    }

    /// <summary>
    ///     Adds a new menu item.
    /// </summary>
    /// <param name="text">Item text.</param>
    /// <returns>Newly created control.</returns>
    public virtual MenuItem AddItem(string text)
    {
        return AddItem(text, null);
    }

    /// <summary>
    ///     Adds a new menu item.
    /// </summary>
    /// <param name="text">Item text.</param>
    /// <param name="iconTexture"></param>
    /// <param name="textureFilename"></param>
    /// <param name="accelerator">Accelerator for this item.</param>
    /// <param name="font"></param>
    /// <returns>Newly created control.</returns>
    public virtual MenuItem AddItem(
        string text,
        GameTexture? iconTexture,
        string? textureFilename = default,
        string? accelerator = default,
        GameFont? font = default
    )
    {
        var newMenuItem = new MenuItem(this)
        {
            Padding = Padding.Four,
            Text = text,
            Font = font,
        };
        newMenuItem.SetStateTexture(iconTexture, textureFilename, Button.ControlState.Normal);
        newMenuItem.SetAccelerator(accelerator);

        OnAddItem(newMenuItem);

        return newMenuItem;
    }

    /// <summary>
    ///     Add item handler.
    /// </summary>
    /// <param name="menuItem">Item added.</param>
    protected virtual void OnAddItem(MenuItem menuItem)
    {
        menuItem.TextPadding = new Padding(IconMarginDisabled ? 0 : 24, 0, 16, 0);
        menuItem.Dock = Pos.Top;
        menuItem.SizeToContents();
        menuItem.TextAlign = Pos.CenterV | Pos.Left;
        menuItem.HoverEnter += OnHoverItem;

        Width = Math.Max(Width, menuItem.Width + 10 + 32);

        UpdateItemStyles();
    }

    /// <summary>
    ///     Closes all submenus.
    /// </summary>
    public virtual void CloseAll()
    {
        foreach (var menuItem in Children.OfType<MenuItem>())
        {
            menuItem.CloseMenu();
        }
    }

    /// <summary>
    ///     Indicates whether any (sub)menu is open.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsMenuOpen()
    {
        return Children.Any(
            child =>
            {
                if (child is MenuItem)
                {
                    return (child as MenuItem).IsMenuOpen;
                }

                return false;
            }
        );
    }

    /// <summary>
    ///     Mouse hover handler.
    /// </summary>
    /// <param name="control">Event source.</param>
    protected virtual void OnHoverItem(Base control, EventArgs args)
    {
        if (!ShouldHoverOpenMenu)
        {
            return;
        }

        var item = control as MenuItem;
        if (null == item)
        {
            return;
        }

        if (item.IsMenuOpen)
        {
            return;
        }

        CloseAll();
        item.OpenMenu();
    }

    /// <summary>
    ///     Closes the current menu.
    /// </summary>
    public virtual void Close()
    {
        if (IsHidden)
        {
            return;
        }

        IsHidden = true;
        if (DeleteOnClose)
        {
            DelayedDelete();
        }
    }

    /// <summary>
    ///     Closes all submenus and the current menu.
    /// </summary>
    public override void CloseMenus()
    {
        base.CloseMenus();
        CloseAll();
        Close();
    }

    /// <summary>
    ///     Adds a divider menu item.
    /// </summary>
    public virtual void AddDivider()
    {
        var divider = new MenuDivider(this);
        divider.Dock = Pos.Top;
        divider.Margin = new Margin(IconMarginDisabled ? 0 : 24, 0, 4, 0);
    }

    public override bool SizeToChildren(bool width = true, bool height = true)
    {
        base.SizeToChildren(width, height);
        if (width)
        {
            var maxWidth = 0;
            foreach (var child in Children)
            {
                if (child.Width > maxWidth)
                {
                    maxWidth = child.Width;
                }
            }

            this.SetSize(maxWidth, Height);
        }

        return true;
    }

    public override JObject? GetJson(bool isRoot = false, bool onlySerializeIfNotEmpty = false)
    {
        var serializedProperties = base.GetJson(isRoot, onlySerializeIfNotEmpty);
        if (serializedProperties is null)
        {
            return null;
        }

        serializedProperties.Add("BackgroundTemplate", mBackgroundTemplateFilename);
        serializedProperties.Add("ItemTextColor", Color.ToString(mItemNormalTextColor));
        serializedProperties.Add("ItemHoveredTextColor", Color.ToString(mItemHoverTextColor));
        serializedProperties.Add("ItemFont", mItemFontInfo);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken obj, bool isRoot = default)
    {
        base.LoadJson(obj);
        if (obj["BackgroundTemplate"] != null)
        {
            SetBackgroundTemplate(
                GameContentManager.Current.GetTexture(
                    Content.TextureType.Gui, (string)obj["BackgroundTemplate"]
                ), (string)obj["BackgroundTemplate"]
            );
        }

        if (obj["ItemTextColor"] != null)
        {
            mItemNormalTextColor = Color.FromString((string)obj["ItemTextColor"]);
        }

        if (obj["ItemHoveredTextColor"] != null)
        {
            mItemHoverTextColor = Color.FromString((string)obj["ItemHoveredTextColor"]);
        }

        if (obj["ItemFont"] != null && obj["ItemFont"].Type != JTokenType.Null)
        {
            var fontArr = ((string)obj["ItemFont"]).Split(',');
            mItemFontInfo = (string)obj["ItemFont"];
            mItemFont = GameContentManager.Current.GetFont(fontArr[0], int.Parse(fontArr[1]));
        }

        UpdateItemStyles();
    }

    private void UpdateItemStyles()
    {
        var menuItems = Children.Where(x => x is MenuItem).ToArray();
        foreach (var item in menuItems)
        {
            var itm = (MenuItem)item;
            if (mItemFont != null)
            {
                itm.Font = mItemFont;
            }

            itm.SetTextColor(mItemNormalTextColor, Label.ControlState.Normal);
            itm.SetTextColor(mItemHoverTextColor, Label.ControlState.Hovered);
        }
    }

    public GameTexture GetTemplate()
    {
        return mBackgroundTemplateTex;
    }

    public void SetBackgroundTemplate(GameTexture texture, string fileName)
    {
        if (texture == null && !string.IsNullOrWhiteSpace(fileName))
        {
            texture = GameContentManager.Current?.GetTexture(Content.TextureType.Gui, fileName);
        }

        mBackgroundTemplateFilename = fileName;
        mBackgroundTemplateTex = texture;
    }

}
