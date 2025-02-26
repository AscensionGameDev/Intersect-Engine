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

    private IGameTexture mBackgroundTemplateTex;

    private bool mDeleteOnClose;

    private bool mDisableIconMargin;

    //Menu Item Stuff
    protected Color mItemHoverTextColor;

    protected Color mItemNormalTextColor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Menu" /> class.
    /// </summary>
    /// <param name="parent">Parent control.</param>
    /// <param name="name"></param>
    public Menu(Base parent, string? name = default) : base(parent, name)
    {
        _scrollPanel.Padding = default;

        Size = new Point(10, 10);
        Padding = Padding.Two;
        IconMarginDisabled = false;

        DeleteOnClose = false;
    }

    #region Font Handling

    private IFont? _itemFont;

    public IFont? ItemFont
    {
        get => _itemFont;
        set => SetItemFont(value, value?.Name);
    }

    private string? _itemFontName;

    public string? ItemFontName
    {
        get => _itemFontName;
        set => SetItemFont(GameContentManager.Current.GetFont(_itemFontName), _itemFontName);
    }

    private int _itemFontSize;

    public int ItemFontSize
    {
        get => _itemFontSize;
        set
        {
            if (value == _itemFontSize)
            {
                return;
            }

            var oldValue = _itemFontSize;
            _itemFontSize = value;
            OnFontSizeChanged(this, value, oldValue);
        }
    }

    private void SetItemFont(IFont? itemFont, string? itemFontName)
    {
        var oldValue = _itemFont;
        _itemFont = itemFont;
        _itemFontName = itemFontName;

        if (itemFont != oldValue)
        {
            OnFontChanged(this, itemFont, oldValue);
        }
    }

    protected virtual void OnFontChanged(Base sender, IFont? newFont, IFont? oldFont)
    {
        UpdateItemStyles();
    }

    protected virtual void OnFontSizeChanged(Base sender, int newSize, int oldSize)
    {
        UpdateItemStyles();
    }

    #endregion

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
    public void Open(Pos pos) => RunOnMainThread(Open, this, pos);

    private static void Open(Menu @this, Pos position)
    {
        @this.IsVisibleInParent = true;
        @this.BringToFront();

        var mouse = Input.InputHandler.MousePosition;

        // Subtract a few pixels to it's absolutely clear the mouse is on a menu item
        var x = mouse.X - 4;
        var y = mouse.Y - 4;

        @this.OnPositioningBeforeOpen();

        if (@this.Canvas is { } canvas)
        {
            var canvasSize = canvas.Size;
            var size = @this.Size;
            x = Math.Min(x, Math.Max(0, canvasSize.X - size.X));
            y = Math.Min(y, Math.Max(0, canvasSize.Y - size.Y));
        }

        @this.SetPosition(x, y);

        @this.OnOpen();
    }

    protected virtual void OnPositioningBeforeOpen()
    {

    }

    protected virtual void OnOpen()
    {

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
        IGameTexture? iconTexture,
        string? textureFilename = default,
        string? accelerator = default,
        IFont? font = default
    )
    {
        var newMenuItem = new MenuItem(this)
        {
            Font = font,
            Text = text,
            Padding = new Padding(8),
        };
        newMenuItem.SetStateTexture(iconTexture, textureFilename, ComponentState.Normal);
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
        menuItem.Padding = new Padding(IconMarginDisabled ? 8 : 32, 4, 8, 4);
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

    public override bool SizeToChildren(SizeToChildrenArgs args)
    {
        var resized = base.SizeToChildren(args);

        if (!args.X)
        {
            return resized;
        }

        var maxWidth = 0;
        foreach (var child in Children)
        {
            if (child.Width > maxWidth)
            {
                maxWidth = child.Width;
            }
        }

        return this.SetSize(maxWidth, Height);
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
        serializedProperties.Add(nameof(ItemFontName), ItemFontName);
        serializedProperties.Add(nameof(ItemFontSize), ItemFontSize);

        return base.FixJson(serializedProperties);
    }

    public override void LoadJson(JToken token, bool isRoot = default)
    {
        base.LoadJson(token, isRoot);

        if (token is not JObject obj)
        {
            return;
        }

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

        string? itemFontName = null;
        int? itemFontSize = null;

        if (obj.TryGetValue(nameof(ItemFont), out var tokenItemFont) &&
            tokenItemFont is JValue { Type: JTokenType.String } valueItemFont)
        {
            var stringItemFont = valueItemFont.Value<string>()?.Trim();
            if (!string.IsNullOrWhiteSpace(stringItemFont))
            {
                var parts = stringItemFont.Split(',');
                itemFontName = parts.FirstOrDefault();

                if (parts.Length > 1 && int.TryParse(parts[1], out var size))
                {
                    itemFontSize = size;
                }
            }
        }

        if (obj.TryGetValue(nameof(ItemFontName), out var tokenItemFontName) &&
            tokenItemFontName is JValue { Type: JTokenType.String } valueItemFontName)
        {
            itemFontName = valueItemFontName.Value<string>();
        }

        if (obj.TryGetValue(nameof(ItemFontSize), out var tokenItemFontSize) &&
            tokenItemFontSize is JValue { Type: JTokenType.Integer } valueItemFontSize)
        {
            itemFontSize = valueItemFontSize.Value<int>();
        }

        if (itemFontSize.HasValue)
        {
            ItemFontSize = itemFontSize.Value;
        }

        itemFontName = itemFontName?.Trim();
        if (!string.IsNullOrWhiteSpace(itemFontName))
        {
            ItemFontName = itemFontName;
        }

        UpdateItemStyles();
    }

    private void UpdateItemStyles()
    {
        var menuItems = Children.OfType<MenuItem>().ToArray();
        foreach (var item in menuItems)
        {
            if (_itemFont != null)
            {
                item.Font = _itemFont;
            }

            item.FontSize = _itemFontSize;
            item.SetTextColor(mItemNormalTextColor, ComponentState.Normal);
            item.SetTextColor(mItemHoverTextColor, ComponentState.Hovered);
        }
    }

    public IGameTexture GetTemplate()
    {
        return mBackgroundTemplateTex;
    }

    public void SetBackgroundTemplate(IGameTexture texture, string fileName)
    {
        if (texture == null && !string.IsNullOrWhiteSpace(fileName))
        {
            texture = GameContentManager.Current?.GetTexture(Content.TextureType.Gui, fileName);
        }

        mBackgroundTemplateFilename = fileName;
        mBackgroundTemplateTex = texture;
    }
}
