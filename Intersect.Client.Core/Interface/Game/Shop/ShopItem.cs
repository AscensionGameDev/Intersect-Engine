using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game.Shop;

public partial class ShopItem : ImagePanel
{
    private readonly int _mySlot;
    private readonly ShopWindow _shopWindow;
    public ImagePanel _iconImage;
    private readonly ContextMenu _contextMenu;
    private readonly MenuItem _buyMenuItem;
    private ItemDescriptionWindow? _itemDescWindow;

    public ShopItem(ShopWindow shopWindow, Base parent, int index) : base(parent, nameof(ShopItem))
    {
        _shopWindow = shopWindow;
        _mySlot = index;

        MinimumSize = new Point(34, 34);
        Margin = new Margin(4, 4, 4, 4);
        MouseInputEnabled = true;
        TextureFilename = "shopitem.png";

        _iconImage = new ImagePanel(this, "ShopItemIcon")
        {
            MinimumSize = new Point(32, 32),
            MouseInputEnabled = true,
            Alignment = [Alignments.Center],
            HoverSound = "octave-tap-resonant.wav",
        };
        _iconImage.HoverEnter += _iconImage_HoverEnter;
        _iconImage.HoverLeave += _iconImage_HoverLeave;
        _iconImage.Clicked += _iconImage_RightClicked;
        _iconImage.DoubleClicked += _iconImage_DoubleClicked;

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        // Generate our context menu with basic options.
        // TODO: Refactor so shop only has 1 context menu shared between all items
        _contextMenu = new ContextMenu(Interface.CurrentInterface.Root, "ShopContextMenu")
        {
            IsVisibleInParent = false,
            IconMarginDisabled = true,
            ItemFont = GameContentManager.Current.GetFont(name: "sourcesansproblack"),
            ItemFontSize = 10,
        };

        //TODO: Is this a memory leak?
        _contextMenu.ClearChildren();
        _buyMenuItem = _contextMenu.AddItem(Strings.ShopContextMenu.Buy);
        _buyMenuItem.Clicked += _buyMenuItem_Clicked;
        _contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        LoadItem();
    }

    private void _iconImage_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (_itemDescWindow != default)
        {
            _itemDescWindow.Dispose();
            _itemDescWindow = default;
        }

        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        if (!ItemBase.TryGet(gameShop.SellingItems[_mySlot].CostItemId, out var item))
        {
            return;
        }

        if (gameShop.SellingItems[_mySlot].Item != default)
        {
            ItemProperties itemProperty = new ItemProperties()
            {
                StatModifiers = item.StatsGiven,
            };

            _itemDescWindow = new ItemDescriptionWindow(
                item: gameShop.SellingItems[_mySlot].Item,
                amount: 1,
                x: _shopWindow.X,
                y: _shopWindow.Y,
                itemProperties: itemProperty,
                valueLabel: Strings.Shop.Costs.ToString(gameShop.SellingItems[_mySlot].CostItemQuantity, item.Name)
            );
        }
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        if (_itemDescWindow != null)
        {
            _itemDescWindow.Dispose();
            _itemDescWindow = null;
        }
    }

    private void _iconImage_RightClicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton != MouseButton.Right)
        {
            return;
        }

        if (ClientConfiguration.Instance.EnableContextMenus)
        {
            OpenContextMenu(_mySlot);
        }
        else
        {
            _iconImage_DoubleClicked(sender, arguments);
        }
    }

    private void _iconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryBuyItem(_mySlot);
    }

    private void _buyMenuItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.MouseButtonState arguments)
    {
        if (sender.Parent?.UserData is not int slot)
        {
            return;
        }

        Globals.Me?.TryBuyItem(slot);
    }

    protected override void Dispose(bool disposing)
    {
        _contextMenu?.Close();
        base.Dispose(disposing);
    }

    public void OpenContextMenu(int slot)
    {
        if (Globals.GameShop?.SellingItems[slot] == default)
        {
            return;
        }

        if (!ItemBase.TryGet(Globals.GameShop.SellingItems[slot].ItemId, out var item))
        {
            return;
        }

        _buyMenuItem.SetText(Strings.ShopContextMenu.Buy.ToString(item.Name));

        _contextMenu.UserData = slot;
        _contextMenu.SizeToChildren();
        _contextMenu.Open(Pos.None);
    }

    public void LoadItem()
    {
        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        if (!ItemBase.TryGet(gameShop.SellingItems[_mySlot].ItemId, out var itemDescriptor))
        {
            return;
        }

        var itemTex = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, itemDescriptor.Icon);
        if (itemTex != null)
        {
            _iconImage.Texture = itemTex;
            _iconImage.RenderColor = itemDescriptor.Color;
        }
    }
}
