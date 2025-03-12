using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Bag;
using Intersect.Client.Interface.Game.Bank;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Framework.Core;
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
    private Draggable? _dragIcon;
    private ItemDescriptionWindow? _descWindow;

    // Drag Handling
    public bool IsDragging;
    private bool _canDrag;
    private long _clickTime;
    private bool _mouseOver;
    private int _mouseX = -1;
    private int _mouseY = -1;

    // Data control
    private string _textureLoaded = string.Empty;

    // Context Menu Handling
    private readonly MenuItem _useItemMenuItem;
    private readonly MenuItem _actionItemMenuItem;
    private readonly MenuItem _dropItemMenuItem;

    public InventoryItem(InventoryWindow inventoryWindow, Base parent, int index, ContextMenu contextMenu) : base(parent, nameof(InventoryItem), index, contextMenu)
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

        _contextMenu.ClearChildren();
        _useItemMenuItem = _contextMenu.AddItem(Strings.ItemContextMenu.Use);
        _useItemMenuItem.Clicked += _useItemContextItem_Clicked;
        _dropItemMenuItem = _contextMenu.AddItem(Strings.ItemContextMenu.Drop);
        _dropItemMenuItem.Clicked += _dropItemContextItem_Clicked;
        _actionItemMenuItem = _contextMenu.AddItem(Strings.ItemContextMenu.Bank);
        _actionItemMenuItem.Clicked += _actionItemContextItem_Clicked;
        _contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        if (Globals.Me is { } player)
        {
            player.InventoryUpdated += PlayerOnInventoryUpdated;
        }
    }

    #region Context Menu

    public override void OpenContextMenu()
    {
        // Clear out the old options since we might not show all of them
        _contextMenu.ClearChildren();

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
                _contextMenu.AddChild(_useItemMenuItem);
                var useItemLabel = descriptor.QuickCast ? Strings.ItemContextMenu.Cast : Strings.ItemContextMenu.Learn;
                _useItemMenuItem.Text = useItemLabel.ToString(descriptor.Name);
                break;

            case ItemType.Event:
            case ItemType.Consumable:
                _contextMenu.AddChild(_useItemMenuItem);
                _useItemMenuItem.Text = Strings.ItemContextMenu.Use.ToString(descriptor.Name);
                break;

            case ItemType.Bag:
                _contextMenu.AddChild(_useItemMenuItem);
                _useItemMenuItem.Text = Strings.ItemContextMenu.Open.ToString(descriptor.Name);
                break;

            case ItemType.Equipment:
                _contextMenu.AddChild(_useItemMenuItem);
                var equipItemLabel = Globals.Me.MyEquipment.Contains(SlotIndex) ? Strings.ItemContextMenu.Unequip : Strings.ItemContextMenu.Equip;
                _useItemMenuItem.Text = equipItemLabel.ToString(descriptor.Name);
                break;
        }

        // Set up the correct contextual additional action.
        if (Globals.InBag && descriptor.CanBag)
        {
            _contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Bag.ToString(descriptor.Name));
        }
        else if (Globals.InBank && (descriptor.CanBank || descriptor.CanGuildBank))
        {
            _contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Bank.ToString(descriptor.Name));
        }
        else if (Globals.InTrade && descriptor.CanTrade)
        {
            _contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Trade.ToString(descriptor.Name));
        }
        else if (Globals.GameShop != null && descriptor.CanSell)
        {
            _contextMenu.AddChild(_actionItemMenuItem);
            _actionItemMenuItem.SetText(Strings.ItemContextMenu.Sell.ToString(descriptor.Name));
        }

        // Can we drop this item? if so show the user!
        if (descriptor.CanDrop)
        {
            _contextMenu.AddChild(_dropItemMenuItem);
            _dropItemMenuItem.SetText(Strings.ItemContextMenu.Drop.ToString(descriptor.Name));
        }

        base.OpenContextMenu();
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
        switch (arguments.MouseButton)
        {
            case MouseButton.Left:
                _clickTime = Timing.Global.MillisecondsUtc + 500;
                break;

            case MouseButton.Right:
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
                break;
        }
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _mouseOver = false;
        _mouseX = -1;
        _mouseY = -1;

        if (_descWindow != null)
        {
            _descWindow.Dispose();
            _descWindow = null;
        }
    }

    void _iconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        _mouseOver = true;
        _canDrag = true;

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            _canDrag = false;
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
        _textureLoaded = string.Empty;
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = _iconImage.ToCanvas(new Point(0, 0)).X,
            Y = _iconImage.ToCanvas(new Point(0, 0)).Y,
            Width = _iconImage.Width,
            Height = _iconImage.Height
        };

        return rect;
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
        _equipImageBackground.IsVisibleInParent = !IsDragging && equipped;
        _equipLabel.IsVisibleInParent = !IsDragging && equipped;

        _quantityLabel.IsVisibleInParent = !IsDragging && descriptor.IsStackable && inventorySlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(inventorySlot.Quantity);
        }

        _cooldownLabel.IsVisibleInParent = !IsDragging && Globals.Me.IsItemOnCooldown(SlotIndex);
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

        if (_textureLoaded != descriptor.Icon)
        {
            var itemTex = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, descriptor.Icon);
            if (itemTex != null)
            {
                _iconImage.Texture = itemTex;
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

            _textureLoaded = descriptor.Icon;

            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
                _iconImage_HoverEnter(null, null);
            }
        }

        if (!IsDragging)
        {
            if (_mouseOver)
            {
                if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
                {
                    _canDrag = true;
                    _mouseX = -1;
                    _mouseY = -1;

                    if (Timing.Global.MillisecondsUtc < _clickTime)
                    {
                        _clickTime = 0;
                    }
                }
                else
                {
                    if (_canDrag && Draggable.Active == null)
                    {
                        if (_mouseX == -1 || _mouseY == -1)
                        {
                            _mouseX = InputHandler.MousePosition.X - _iconImage.ToCanvas(new Point(0, 0)).X;
                            _mouseY = InputHandler.MousePosition.Y - _iconImage.ToCanvas(new Point(0, 0)).Y;
                        }
                        else
                        {
                            var xdiff = _mouseX -
                                        (InputHandler.MousePosition.X - _iconImage.ToCanvas(new Point(0, 0)).X);

                            var ydiff = _mouseY -
                                        (InputHandler.MousePosition.Y - _iconImage.ToCanvas(new Point(0, 0)).Y);

                            if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                            {
                                IsDragging = true;
                                _iconImage.IsVisibleInParent = false;

                                _dragIcon = new Draggable(
                                    _iconImage.ToCanvas(new Point(0, 0)).X + _mouseX,
                                    _iconImage.ToCanvas(new Point(0, 0)).X + _mouseY, _iconImage.Texture, _iconImage.RenderColor
                                );
                            }
                        }
                    }
                }
            }
        }
        else if (_dragIcon?.Update() == true)
        {
            //Drug the item and now we stopped
            IsDragging = false;
            _iconImage.IsVisibleInParent = true;

            var dragRect = new FloatRect(
                _dragIcon.X - (Padding.Left + Padding.Right) / 2f,
                _dragIcon.Y - (Padding.Top + Padding.Bottom) / 2f,
                (Padding.Left + Padding.Right) / 2f + _iconImage.Width,
                (Padding.Top + Padding.Bottom) / 2f + _iconImage.Height
            );

            float bestIntersect = 0;
            var bestIntersectIndex = -1;

            //So we picked up an item and then dropped it. Lets see where we dropped it to.
            //Check inventory first.
            if (_inventoryWindow.RenderBounds().IntersectsWith(dragRect))
            {
                var inventorySlotComponents = _inventoryWindow.Items.ToArray();
                var inventorySlotLimit = Math.Min(Options.Instance.Player.MaxInventory, inventorySlotComponents.Length);
                for (var inventoryIndex = 0; inventoryIndex < inventorySlotLimit; inventoryIndex++)
                {
                    var inventorySlotComponent = inventorySlotComponents[inventoryIndex];
                    var inventoryRenderBounds = ((InventoryItem)inventorySlotComponent).RenderBounds();

                    if (!inventoryRenderBounds.IntersectsWith(dragRect))
                    {
                        continue;
                    }

                    var intersection = FloatRect.Intersect(inventoryRenderBounds, dragRect);
                    if (!(intersection.Width * intersection.Height > bestIntersect))
                    {
                        continue;
                    }

                    bestIntersect = intersection.Width * intersection.Height;
                    bestIntersectIndex = inventoryIndex;
                }

                if (bestIntersectIndex > -1)
                {
                    if (SlotIndex != bestIntersectIndex)
                    {
                        Globals.Me.SwapItems(SlotIndex, bestIntersectIndex);
                    }
                }
            }
            else if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
            {
                var hotbarSlotComponents = Interface.GameUi.Hotbar.Items.ToArray();
                var hotbarSlotLimit = Math.Min(
                    Options.Instance.Player.HotbarSlotCount,
                    hotbarSlotComponents.Length
                );
                for (var hotbarSlotIndex = 0; hotbarSlotIndex < hotbarSlotLimit; hotbarSlotIndex++)
                {
                    var hotbarSlotComponent = hotbarSlotComponents[hotbarSlotIndex];
                    var hotbarSlotRenderBounds = hotbarSlotComponent.RenderBounds();
                    if (!hotbarSlotRenderBounds.IntersectsWith(dragRect))
                    {
                        continue;
                    }

                    var intersection = FloatRect.Intersect(hotbarSlotRenderBounds, dragRect);
                    if (intersection.Width * intersection.Height <= bestIntersect)
                    {
                        continue;
                    }

                    bestIntersect = intersection.Width * intersection.Height;
                    bestIntersectIndex = hotbarSlotIndex;
                }

                if (bestIntersectIndex > -1)
                {
                    Globals.Me.AddToHotbar((byte)bestIntersectIndex, 0, SlotIndex);
                }
            }
            else if (Globals.InBag && Globals.BagSlots != default)
            {
                var bagWindow = Interface.GameUi.GetBagWindow();
                if (bagWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    var bagSlotComponents = bagWindow.Items.ToArray();
                    var bagSlotLimit = Math.Min(Globals.BagSlots.Length, bagSlotComponents.Length);
                    for (var bagSlotIndex = 0; bagSlotIndex < bagSlotLimit; bagSlotIndex++)
                    {
                        if (bagSlotComponents[bagSlotIndex] is not BagItem bagSlotComponent)
                        {
                            continue;
                        }

                        var bagSlotRenderBounds = bagSlotComponent.RenderBounds();
                        if (!bagSlotRenderBounds.IntersectsWith(dragRect))
                        {
                            continue;
                        }

                        var intersection = FloatRect.Intersect(bagSlotRenderBounds, dragRect);
                        if (intersection.Width * intersection.Height <= bestIntersect)
                        {
                            continue;
                        }

                        bestIntersect = intersection.Width * intersection.Height;
                        bestIntersectIndex = bagSlotIndex;
                    }

                    if (bestIntersectIndex > -1)
                    {
                        Globals.Me.TryStoreItemInBag(SlotIndex, bestIntersectIndex);
                    }
                }
            }
            else if (Globals.InBank && Globals.BankSlots != default)
            {
                if (Interface.GameUi.GetBankWindow() is not BankWindow bankWindow)
                {
                    return;
                }

                if (bankWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    var bankSlotComponents = bankWindow.Items.ToArray();
                    var bankSlotLimit = Math.Min(
                        Math.Min(Globals.BankSlots.Length, Globals.BankSlotCount),
                        bankSlotComponents.Length
                    );

                    for (var bankSlotIndex = 0; bankSlotIndex < bankSlotLimit; bankSlotIndex++)
                    {
                        if (bankSlotComponents[bankSlotIndex] is not BankItem bankSlotComponent)
                        {
                            continue;
                        }

                        var bankSlotRenderBounds = bankSlotComponent.RenderBounds();
                        if (!bankSlotRenderBounds.IntersectsWith(dragRect))
                        {
                            continue;
                        }

                        var intersection = FloatRect.Intersect(bankSlotRenderBounds, dragRect);
                        if (!(intersection.Width * intersection.Height > bestIntersect))
                        {
                            continue;
                        }

                        bestIntersect = intersection.Width * intersection.Height;
                        bestIntersectIndex = bankSlotIndex;
                    }

                    if (bestIntersectIndex > -1)
                    {
                        var slot = Globals.Me.Inventory[SlotIndex];
                        Globals.Me.TryStoreItemInBank(
                            SlotIndex,
                            bankSlotIndex: bestIntersectIndex,
                            quantityHint: slot.Quantity,
                            skipPrompt: true
                        );
                    }
                }
            }
            else if (!Globals.Me.IsBusy)
            {
                PacketSender.SendDropItem(SlotIndex, Globals.Me.Inventory[SlotIndex].Quantity);
            }

            _dragIcon.Dispose();
        }
    }

    private void _reset()
    {
        _iconImage.IsVisibleInParent = false;
        _equipImageBackground.IsVisibleInParent = false;
        _quantityLabel.IsVisibleInParent = false;
        _equipLabel.IsVisibleInParent = false;
        _cooldownLabel.IsVisibleInParent = false;
        _textureLoaded = string.Empty;

        if (_dragIcon != default)
        {
            _dragIcon.Dispose();
            _dragIcon = default;
        }

        if (_descWindow != default)
        {
            _descWindow.Dispose();
            _descWindow = default;
        }
    }
}
