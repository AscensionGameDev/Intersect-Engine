using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Bag;
using Intersect.Client.Interface.Game.Bank;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Interface.Game.Hotbar;
using Intersect.Client.Interface.Game.Shop;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Inventory;

public partial class InventoryItem : SlotItem
{
    // Controls
    private readonly Label _quantityLabel;
    private readonly Label _equipLabel;
    private readonly Label _cooldownLabel;
    private readonly ImagePanel _equipImageBackground;
    private readonly InventoryWindow _inventoryWindow;
    private ItemDescriptionWindow? _descWindow;

    // Context Menu Handling
    private readonly MenuItem _useItemMenuItem;
    private readonly MenuItem _actionItemMenuItem;
    private readonly MenuItem _dropItemMenuItem;

    public InventoryItem(InventoryWindow inventoryWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(InventoryItem), index, contextMenu)
    {
        _inventoryWindow = inventoryWindow;
        TextureFilename = "inventoryitem.png";

        _iconImage.HoverEnter += _iconImage_HoverEnter;
        _iconImage.HoverLeave += _iconImage_HoverLeave;
        _iconImage.Clicked += _iconImage_Clicked;
        _iconImage.DoubleClicked += _iconImage_DoubleClicked;

        _equipImageBackground = new ImagePanel(this, "EquippedIcon")
        {
            Texture = Graphics.Renderer.WhitePixel,
        };

        _equipLabel = new Label(this, "EquippedLabel")
        {
            IsVisibleInParent = false,
            Text = Strings.Inventory.EquippedSymbol,
            FontName = "sourcesansproblack",
            FontSize = 8,
            TextColor = new Color(0, 255, 255, 255),
            Alignment = [Alignments.Right, Alignments.Top],
            BackgroundTemplateName = "equipped.png",
            Padding = new Padding(2),
        };

        _cooldownLabel = new Label(this, "CooldownLabel")
        {
            IsVisibleInParent = false,
            FontName = "sourcesansproblack",
            FontSize = 8,
            TextColor = new Color(0, 255, 255, 255),
            Alignment = [Alignments.Center],
            BackgroundTemplateName = "quantity.png",
            Padding = new Padding(2),
        };

        _quantityLabel = new Label(this, "Quantity")
        {
            Alignment = [Alignments.Bottom, Alignments.Right],
            BackgroundTemplateName = "quantity.png",
            FontName = "sourcesansproblack",
            FontSize = 8,
            Padding = new Padding(2),
        };

        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        contextMenu.ClearChildren();
        _useItemMenuItem = contextMenu.AddItem(Strings.ItemContextMenu.Use);
        _useItemMenuItem.Clicked += _useItemContextItem_Clicked;
        _dropItemMenuItem = contextMenu.AddItem(Strings.ItemContextMenu.Drop);
        _dropItemMenuItem.Clicked += _dropItemContextItem_Clicked;
        _actionItemMenuItem = contextMenu.AddItem(Strings.ItemContextMenu.Bank);
        _actionItemMenuItem.Clicked += _actionItemContextItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        if (Globals.Me is { } player)
        {
            player.InventoryUpdated += PlayerOnInventoryUpdated;
        }
    }

    #region Context Menu

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        // Clear out the old options since we might not show all of them
        contextMenu.ClearChildren();

        if (Globals.Me?.Inventory[SlotIndex] is not { } inventorySlot)
        {
            return;
        }

        // No point showing a menu for blank space.
        if (!ItemDescriptor.TryGet(inventorySlot.ItemId, out var descriptor))
        {
            return;
        }

        // Add our use Item prompt, assuming we have a valid usecase.
        switch (descriptor.ItemType)
        {
            case ItemType.Spell:
                contextMenu.AddChild(_useItemMenuItem);
                var useItemLabel = descriptor.QuickCast ? Strings.ItemContextMenu.Cast : Strings.ItemContextMenu.Learn;
                _useItemMenuItem.Text = useItemLabel.ToString(descriptor.Name);
                break;

            case ItemType.Event:
            case ItemType.Consumable:
                contextMenu.AddChild(_useItemMenuItem);
                _useItemMenuItem.Text = Strings.ItemContextMenu.Use.ToString(descriptor.Name);
                break;

            case ItemType.Bag:
                contextMenu.AddChild(_useItemMenuItem);
                _useItemMenuItem.Text = Strings.ItemContextMenu.Open.ToString(descriptor.Name);
                break;

            case ItemType.Equipment:
                contextMenu.AddChild(_useItemMenuItem);
                var equipItemLabel = Globals.Me.MyEquipment.Contains(SlotIndex) ? Strings.ItemContextMenu.Unequip : Strings.ItemContextMenu.Equip;
                _useItemMenuItem.Text = equipItemLabel.ToString(descriptor.Name);
                break;
        }

        // Set up the correct contextual additional action.
        if (Globals.InBag && descriptor.CanBag)
        {
            contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Bag.ToString(descriptor.Name));
        }
        else if (Globals.InBank && (descriptor.CanBank || descriptor.CanGuildBank))
        {
            contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Bank.ToString(descriptor.Name));
        }
        else if (Globals.InTrade && descriptor.CanTrade)
        {
            contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Trade.ToString(descriptor.Name));
        }
        else if (Globals.GameShop != null && descriptor.CanSell)
        {
            contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Sell.ToString(descriptor.Name));
        }

        // Can we drop this item? if so show the user!
        if (descriptor.CanDrop)
        {
            contextMenu.AddChild(_dropItemMenuItem);
            _dropItemMenuItem.SetText(Strings.ItemContextMenu.Drop.ToString(descriptor.Name));
        }

        base.OnContextMenuOpening(contextMenu);
    }

    private void _useItemContextItem_Clicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryUseItem(SlotIndex);
    }

    private void _actionItemContextItem_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.GameShop != null)
        {
            Globals.Me?.TrySellItem(SlotIndex);
        }
        else if (Globals.InBank)
        {
            Globals.Me?.TryStoreItemInBank(SlotIndex);
        }
        else if (Globals.InBag)
        {
            Globals.Me?.TryStoreItemInBag(SlotIndex, -1);
        }
        else if (Globals.InTrade)
        {
            Globals.Me?.TryOfferItemToTrade(SlotIndex);
        }
    }

    private void _dropItemContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.MouseButtonState arguments)
    {
        Globals.Me?.TryDropItem(SlotIndex);
    }

    #endregion

    #region Mouse Events

    private void _iconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.Me == default)
        {
            return;
        }

        if (Globals.GameShop != null)
        {
            Globals.Me.TrySellItem(SlotIndex);
        }
        else if (Globals.InBank)
        {
            if (Globals.InputManager.IsKeyDown(Framework.GenericClasses.Keys.Shift))
            {
                Globals.Me.TryStoreItemInBank(
                    SlotIndex,
                    skipPrompt: true
                );
            }
            else
            {
                var slot = Globals.Me.Inventory[SlotIndex];
                Globals.Me.TryStoreItemInBank(
                    SlotIndex,
                    slot,
                    quantityHint: slot.Quantity,
                    skipPrompt: false
                );
            }
        }
        else if (Globals.InBag)
        {
            Globals.Me.TryStoreItemInBag(SlotIndex, -1);
        }
        else if (Globals.InTrade)
        {
            Globals.Me.TryOfferItemToTrade(SlotIndex);
        }
        else
        {
            Globals.Me.TryUseItem(SlotIndex);
        }
    }

    private void _iconImage_Clicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton is MouseButton.Right)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                OpenContextMenu();
            }
            else
            {
                if (Globals.GameShop != null)
                {
                    Globals.Me?.TrySellItem(SlotIndex);
                }
                else if (Globals.InBank)
                {
                    Globals.Me?.TryStoreItemInBank(SlotIndex);
                }
                else if (Globals.InBag)
                {
                    Globals.Me?.TryStoreItemInBag(SlotIndex, -1);
                }
                else if (Globals.InTrade)
                {
                    Globals.Me?.TryOfferItemToTrade(SlotIndex);
                }
                else
                {
                    Globals.Me?.TryDropItem(SlotIndex);
                }
            }
        }
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _descWindow?.Dispose();
        _descWindow = null;
    }

    void _iconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (_descWindow != null)
        {
            _descWindow.Dispose();
            _descWindow = null;
        }

        if (Globals.Me?.Inventory[SlotIndex] is not { } inventorySlot)
        {
            return;
        }

        var inventorySlotDescriptor = inventorySlot.Descriptor;
        if (inventorySlotDescriptor is null)
        {
            return;
        }

        if (Globals.GameShop == null)
        {
            _descWindow = new ItemDescriptionWindow(
                inventorySlotDescriptor,
                inventorySlot.Quantity,
                _inventoryWindow.X,
                _inventoryWindow.Y,
                inventorySlot.ItemProperties
            );
        }
        else
        {
            ShopItemDescriptor? shopItemDescriptor = default;
            for (var i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
            {
                var tmpShop = Globals.GameShop.BuyingItems[i];
                if (inventorySlot.ItemId == tmpShop.ItemId)
                {
                    shopItemDescriptor = tmpShop;
                    break;
                }
            }

            if (Globals.GameShop.BuyingWhitelist && shopItemDescriptor != default)
            {
                if (!ItemDescriptor.TryGet(shopItemDescriptor.CostItemId, out var hoveredItem))
                {
                    return;
                }

                _descWindow = new ItemDescriptionWindow(
                    inventorySlotDescriptor,
                    inventorySlot.Quantity,
                    _inventoryWindow.X,
                    _inventoryWindow.Y,
                    inventorySlot.ItemProperties,
                    "",
                    Strings.Shop.SellsFor.ToString(shopItemDescriptor.CostItemQuantity, hoveredItem.Name)
                );
            }
            else if (shopItemDescriptor == null)
            {
                var costItem = Globals.GameShop.DefaultCurrency;
                if (inventorySlotDescriptor != null && costItem != null)
                {
                    _descWindow = new ItemDescriptionWindow(
                        inventorySlotDescriptor,
                        inventorySlot.Quantity,
                        _inventoryWindow.X,
                        _inventoryWindow.Y,
                        inventorySlot.ItemProperties,
                        "",
                        Strings.Shop.SellsFor.ToString(inventorySlotDescriptor.Price.ToString(), costItem.Name)
                    );
                }
            }
            else
            {
                _descWindow = new ItemDescriptionWindow(
                    inventorySlotDescriptor,
                    inventorySlot.Quantity,
                    _inventoryWindow.X,
                    _inventoryWindow.Y,
                    inventorySlot.ItemProperties,
                    "",
                    Strings.Shop.WontBuy
                );
            }
        }
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        if (Globals.Me?.Inventory is not { } inventory)
        {
            return false;
        }

        if (inventory[SlotIndex] is not { } inventorySlot)
        {
            return false;
        }

        if (!Interface.DoesMouseHitInterface() && !Globals.Me.IsBusy)
        {
            PacketSender.SendDropItem(SlotIndex, inventorySlot.Quantity);
            return true;
        }

        var targetNode = Interface.FindComponentUnderCursor(NodeFilter.None);

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case InventoryItem inventoryItem:
                    if (inventoryItem.SlotIndex == SlotIndex)
                    {
                        return false;
                    }

                    Globals.Me?.SwapItems(SlotIndex, inventoryItem.SlotIndex);
                    return true;

                case BagItem bagItem:
                    Globals.Me?.TryStoreItemInBag(SlotIndex, bagItem.SlotIndex);
                    return true;

                case BankItem bankItem:
                    Globals.Me?.TryStoreItemInBank(
                        SlotIndex,
                        bankSlotIndex: bankItem.SlotIndex,
                        quantityHint: inventorySlot.Quantity,
                        skipPrompt: true
                    );
                    return true;

                case HotbarItem hotbarItem:
                    Globals.Me?.AddToHotbar(hotbarItem.SlotIndex, 0, SlotIndex);
                    return true;

                case ShopWindow:
                    Globals.Me?.TrySellItem(SlotIndex);
                    return true;

                default:
                    targetNode = targetNode.Parent;
                    break;
            }

            // If we've reached the top of the tree, we can't drop here, so return false
            if (targetNode == null)
            {
                return false;
            }
        }

        return false;
    }

    #endregion

    private void PlayerOnInventoryUpdated(Player player, int slotIndex)
    {
        if (player != Globals.Me)
        {
            return;
        }

        if (slotIndex != SlotIndex)
        {
            return;
        }

        if (Globals.Me.Inventory[SlotIndex] == default)
        {
            return;
        }

        // empty texture to reload on update
        _iconImage.Texture = default;
    }

    public override void Update()
    {
        if (Globals.Me == default)
        {
            return;
        }

        if (Globals.Me.Inventory[SlotIndex] is not { } inventorySlot)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(inventorySlot.ItemId, out var descriptor))
        {
            _reset();
            return;
        }

        var equipped = Globals.Me.MyEquipment.Any(s => s == SlotIndex);
        //Todo: hide when dragging
        _equipImageBackground.IsVisibleInParent = equipped;
        //Todo: hide when dragging
        _equipLabel.IsVisibleInParent = equipped;

        //Todo: hide when dragging
        _quantityLabel.IsVisibleInParent = descriptor.IsStackable && inventorySlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(inventorySlot.Quantity);
        }

        //Todo: hide when dragging
        _cooldownLabel.IsVisibleInParent = Globals.Me.IsItemOnCooldown(SlotIndex);
        if (_cooldownLabel.IsVisibleInParent)
        {
            var itemCooldownRemaining = Globals.Me.GetItemRemainingCooldown(SlotIndex);
            _cooldownLabel.Text = TimeSpan.FromMilliseconds(itemCooldownRemaining).WithSuffix("0.0");
            _iconImage.RenderColor.A = 100;
        }
        else
        {
            _iconImage.RenderColor.A = descriptor.Color.A;
        }

        if (_iconImage.TextureFilename == descriptor.Icon)
        {
            return;
        }

        var itemTexture = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, descriptor.Icon);
        if (itemTexture != null)
        {
            _iconImage.Texture = itemTexture;
            _iconImage.RenderColor = Globals.Me.IsItemOnCooldown(SlotIndex)
                ? new Color(100, descriptor.Color.R, descriptor.Color.G, descriptor.Color.B)
                : descriptor.Color;
            _iconImage.IsVisibleInParent = true;
        }
        else
        {
            if (_iconImage.Texture != null)
            {
                _iconImage.Texture = null;
                _iconImage.IsVisibleInParent = false;
            }
        }

        if (_descWindow != null)
        {
            _descWindow.Dispose();
            _descWindow = null;
            _iconImage_HoverEnter(null, null);
        }
    }

    private void _reset()
    {
        _iconImage.IsVisibleInParent = false;
        _iconImage.Texture = default;
        _equipImageBackground.IsVisibleInParent = false;
        _quantityLabel.IsVisibleInParent = false;
        _equipLabel.IsVisibleInParent = false;
        _cooldownLabel.IsVisibleInParent = false;
        _descWindow?.Dispose();
        _descWindow = default;
    }
}
