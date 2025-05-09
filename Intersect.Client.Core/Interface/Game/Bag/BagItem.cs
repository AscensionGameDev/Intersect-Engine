using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Bag;

public partial class BagItem : SlotItem
{
    // Controls
    private readonly Label _quantityLabel;
    private readonly BagWindow _bagWindow;

    // Context Menu Handling
    private readonly MenuItem _withdrawContextItem;

    public BagItem(BagWindow bagWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(BagItem), index, contextMenu)
    {
        _bagWindow = bagWindow;
        TextureFilename = "bagitem.png";

        Icon.HoverEnter += Icon_HoverEnter;
        Icon.HoverLeave += Icon_HoverLeave;
        Icon.Clicked += Icon_Clicked;
        Icon.DoubleClicked += Icon_DoubleClicked;

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
        _withdrawContextItem = contextMenu.AddItem(Strings.BagContextMenu.Withdraw);
        _withdrawContextItem.Clicked += _withdrawMenuItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(bagSlots[SlotIndex].ItemId, out var item))
        {
            return;
        }

        // Clear the context menu and add the withdraw item with updated item name
        contextMenu.ClearChildren();
        _withdrawContextItem.SetText(Strings.BagContextMenu.Withdraw.ToString(item.Name));
        contextMenu.AddChild(_withdrawContextItem);
        base.OnContextMenuOpening(contextMenu);
    }

    private void _withdrawMenuItem_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.InBag)
        {
            Globals.Me?.TryRetrieveItemFromBag(SlotIndex, -1);
        }
    }

    #endregion

    #region Mouse Events

    private void Icon_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != default)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        if (bagSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            return;
        }

        var item = bagSlots[SlotIndex];
        Interface.GameUi.ItemDescriptionWindow?.Show(item.Descriptor, item.Quantity, item.ItemProperties);
    }

    private void Icon_HoverLeave(Base sender, EventArgs arguments)
    {
        Interface.GameUi.ItemDescriptionWindow?.Hide();
    }

    private void Icon_Clicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton is MouseButton.Right)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                OpenContextMenu();
            }
            else
            {
                Icon_DoubleClicked(sender, arguments);
            }
        }
    }

    private void Icon_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton is MouseButton.Left)
        {
            Globals.Me?.TryRetrieveItemFromBag(SlotIndex, -1);
        }
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        var targetNode = Interface.FindComponentUnderCursor();

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case BagItem bagItem:
                    PacketSender.SendMoveBagItems(SlotIndex, bagItem.SlotIndex);
                    return true;

                case InventoryItem inventoryItem:
                    Globals.Me?.TryRetrieveItemFromBag(SlotIndex, inventoryItem.SlotIndex);
                    return true;

                default:
                    targetNode = targetNode.Parent;
                    break;
            }
        }

        // If we've reached the top of the tree, we can't drop here, so cancel drop
        return false;
    }

    #endregion

    public override void Update()
    {
        if (Globals.Me == default)
        {
            return;
        }

        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        if (bagSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            _quantityLabel.IsVisibleInParent = false;
            Icon.Texture = default;
            return;
        }

        var bagSlot = bagSlots[SlotIndex];
        var descriptor = bagSlot.Descriptor;

        _quantityLabel.IsVisibleInParent = !Icon.IsDragging && descriptor.IsStackable && bagSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bagSlot.Quantity);
        }

        if (Icon.TextureFilename == descriptor.Icon)
        {
            return;
        }

        var itemTexture = GameContentManager.Current.GetTexture(Framework.Content.TextureType.Item, descriptor.Icon);
        if (itemTexture != default)
        {
            Icon.Texture = itemTexture;
            Icon.RenderColor = descriptor.Color;
            Icon.IsVisibleInParent = true;
        }
        else
        {
            if (Icon.Texture != default)
            {
                Icon.Texture = default;
                Icon.IsVisibleInParent = false;
            }
        }
    }
}
