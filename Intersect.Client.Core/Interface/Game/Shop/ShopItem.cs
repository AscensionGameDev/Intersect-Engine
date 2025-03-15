using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Shop;

public partial class ShopItem : SlotItem
{
    private readonly int _mySlot;
    private readonly ShopWindow _shopWindow;
    private readonly MenuItem _buyMenuItem;
    private ItemDescriptionWindow? _descriptionWindow;

    public ShopItem(ShopWindow shopWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(ShopItem), index, contextMenu)
    {
        _shopWindow = shopWindow;
        _mySlot = index;
        TextureFilename = "shopitem.png";

        Icon.HoverEnter += Icon_HoverEnter;
        Icon.HoverLeave += Icon_HoverLeave;
        Icon.Clicked += Icon_RightClicked;
        Icon.DoubleClicked += Icon_DoubleClicked;

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        contextMenu.ClearChildren();
        _buyMenuItem = contextMenu.AddItem(Strings.ShopContextMenu.Buy);
        _buyMenuItem.Clicked += _buyMenuItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        LoadItem();
    }

    private void Icon_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (_descriptionWindow != default)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = default;
        }

        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(gameShop.SellingItems[_mySlot].CostItemId, out var item))
        {
            return;
        }

        if (gameShop.SellingItems[_mySlot].Item != default)
        {
            ItemProperties itemProperty = new ItemProperties()
            {
                StatModifiers = item.StatsGiven,
            };

            _descriptionWindow = new ItemDescriptionWindow(
                item: gameShop.SellingItems[_mySlot].Item,
                amount: 1,
                x: _shopWindow.X,
                y: _shopWindow.Y,
                itemProperties: itemProperty,
                valueLabel: Strings.Shop.Costs.ToString(gameShop.SellingItems[_mySlot].CostItemQuantity, item.Name)
            );
        }
    }

    private void Icon_HoverLeave(Base sender, EventArgs arguments)
    {
        if (_descriptionWindow != null)
        {
            _descriptionWindow.Dispose();
            _descriptionWindow = null;
        }
    }

    private void Icon_RightClicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton != MouseButton.Right)
        {
            return;
        }

        if (ClientConfiguration.Instance.EnableContextMenus)
        {
            OpenContextMenu();
        }
        else
        {
            Icon_DoubleClicked(sender, arguments);
        }
    }

    private void Icon_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryBuyItem(_mySlot);
    }

    private void _buyMenuItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.MouseButtonState arguments)
    {
        Globals.Me?.TryBuyItem(_mySlot);
    }

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(gameShop.SellingItems[SlotIndex].ItemId, out var item))
        {
            return;
        }

        contextMenu.ClearChildren();
        contextMenu.AddChild(_buyMenuItem);
        _buyMenuItem.SetText(Strings.ShopContextMenu.Buy.ToString(item.Name));
        base.OnContextMenuOpening(contextMenu);
    }

    public void LoadItem()
    {
        if (Globals.GameShop is not { SellingItems.Count: > 0 } gameShop)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(gameShop.SellingItems[_mySlot].ItemId, out var itemDescriptor))
        {
            return;
        }

        var itemTex = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, itemDescriptor.Icon);
        if (itemTex != null)
        {
            Icon.Texture = itemTex;
            Icon.RenderColor = itemDescriptor.Color;
        }
    }

    public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
    {
        var targetNode = Interface.FindComponentUnderCursor();

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            if (targetNode is not InventoryWindow)
            {
                targetNode = targetNode.Parent;
                continue;
            }

            Globals.Me?.TryBuyItem(_mySlot);
            return true;
        }

        // If we've reached the top of the tree, we can't drop here, so cancel drop
        return false;
    }
}
