using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
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
    private ItemDescriptionWindow? _descWindow;

    // Context Menu Handling
    private readonly MenuItem _withdrawContextItem;

    public BagItem(BagWindow bagWindow, Base parent, int index, ContextMenu contextMenu)
        : base(parent, nameof(BagItem), index, contextMenu)
    {
        _bagWindow = bagWindow;
        TextureFilename = "bagitem.png";

        IconImage.HoverEnter += IconImage_HoverEnter;
        IconImage.HoverLeave += IconImage_HoverLeave;
        IconImage.Clicked += IconImage_Clicked;
        IconImage.DoubleClicked += IconImage_DoubleClicked;

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

    private void IconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != default)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        _descWindow?.Dispose();
        _descWindow = default;

        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        if (bagSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            return;
        }

        var item = bagSlots[SlotIndex];
        _descWindow = new ItemDescriptionWindow(
            item.Descriptor,
            item.Quantity,
            _bagWindow.X,
            _bagWindow.Y,
            item.ItemProperties
        );
    }

    private void IconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _descWindow?.Dispose();
        _descWindow = default;
    }

    private void IconImage_Clicked(Base sender, MouseButtonState arguments)
    {
        if (arguments.MouseButton is MouseButton.Right)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                OpenContextMenu();
            }
            else
            {
                IconImage_DoubleClicked(sender, arguments);
            }
        }
    }

    private void IconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.InBag)
        {
            Globals.Me?.TryRetrieveItemFromBag(SlotIndex, -1);
        }
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        var targetNode = Interface.FindComponentUnderCursor(NodeFilter.None);

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
            
            // If we've reached the top of the tree, we can't drop here, so cancel drop
            if (targetNode == null)
            {
                return false;
            }
        }

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
            IconImage.Texture = default;
            return;
        }

        var bagSlot = bagSlots[SlotIndex];
        var descriptor = bagSlot.Descriptor;

        //TODO: dont show when is dragging
        _quantityLabel.IsVisibleInParent = descriptor.IsStackable && bagSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bagSlot.Quantity);
        }

        if (IconImage.TextureFilename == descriptor.Icon)
        {
            return;
        }

        var itemTexture = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, descriptor.Icon);
        if (itemTexture != default)
        {
            IconImage.Texture = itemTexture;
            IconImage.RenderColor = descriptor.Color;
            IconImage.IsVisibleInParent = true;
        }
        else
        {
            if (IconImage.Texture != default)
            {
                IconImage.Texture = default;
                IconImage.IsVisibleInParent = false;
            }
        }

        _descWindow?.Dispose();
        _descWindow = default;
    }
}
