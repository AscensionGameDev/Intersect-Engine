using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.DragDrop;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Bank;

public partial class BankItem : SlotItem
{
    // Controls
    private readonly Label _quantityLabel;
    private BankWindow _bankWindow;

    // Context Menu Handling
    private MenuItem _withdrawContextItem;

    public BankItem(BankWindow bankWindow, Base parent, int index, ContextMenu contextMenu) :
        base(parent, nameof(BankItem), index, contextMenu)
    {
        _bankWindow = bankWindow;
        TextureFilename = "bankitem.png";

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
        _withdrawContextItem = contextMenu.AddItem(Strings.BankContextMenu.Withdraw);
        _withdrawContextItem.Clicked += _withdrawMenuItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        // Clear out the old options since we might not show all of them
        contextMenu.ClearChildren();

        if (Globals.BankSlots[SlotIndex] is not { } bankSlot)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(bankSlot.ItemId, out var item))
        {
            return;
        }

        // update context menu
        _withdrawContextItem.SetText(Strings.BankContextMenu.Withdraw.ToString(item.Name));
        contextMenu.AddChild(_withdrawContextItem);
        base.OnContextMenuOpening(contextMenu);
    }

    private void _withdrawMenuItem_Clicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryRetrieveItemFromBank(SlotIndex);
    }

    #endregion

    #region Mouse Events

    private void Icon_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }

        if (Globals.BankSlots is not { Length: > 0 } bankSlots)
        {
            return;
        }

        if (bankSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            return;
        }

        var item = bankSlots[SlotIndex];
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
        if (arguments.MouseButton is not MouseButton.Left)
        {
            return;
        }

        if (Globals.BankSlots is not { Length: > 0 } bankSlots)
        {
            return;
        }

        if (bankSlots[SlotIndex] is not { Quantity: > 0 } slot)
        {
            return;
        }

        if (Globals.InputManager.IsKeyDown(Keys.Shift))
        {
            Globals.Me?.TryRetrieveItemFromBank(SlotIndex, skipPrompt: true);
        }
        else
        {
            Globals.Me?.TryRetrieveItemFromBank(
                SlotIndex,
                slot,
                quantityHint: slot.Quantity,
                skipPrompt: false
            );
        }
    }

    #endregion

    #region Drag and Drop

    public override bool DragAndDrop_HandleDrop(Package package, int x, int y)
    {
        if (Globals.Me is not { } player)
        {
            return false;
        }

        var rank = player.GuildRank;
        var isInGuild = !string.IsNullOrWhiteSpace(player.Guild);

        if (Globals.IsGuildBank)
        {
            if (!isInGuild || (player.Rank != 0 && rank?.Permissions.BankDeposit == false))
            {
                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        Strings.Guilds.NotAllowedSwap.ToString(player.Guild),
                        CustomColors.Alerts.Error,
                        ChatMessageType.Bank
                    )
                );

                return false;
            }
        }

        var targetNode = Interface.FindComponentUnderCursor();

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case BankItem bankItem:
                    PacketSender.SendMoveBankItems(SlotIndex, bankItem.SlotIndex);
                    return true;

                case InventoryItem inventoryItem:

                    if (Globals.BankSlots is not { Length: > 0 } bankSlots)
                    {
                        return false;
                    }

                    if (bankSlots[SlotIndex] is not { Quantity: > 0 } slot)
                    {
                        return false;
                    }

                    player.TryRetrieveItemFromBank(
                        SlotIndex,
                        inventorySlotIndex: inventoryItem.SlotIndex,
                        quantityHint: slot.Quantity,
                        skipPrompt: true
                    );
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

        if (Globals.BankSlots is not { Length: > 0 } bankSlots)
        {
            return;
        }

        if (bankSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            _quantityLabel.IsVisibleInParent = false;
            Icon.Texture = default;
            return;
        }

        var bankSlot = bankSlots[SlotIndex];
        var descriptor = bankSlot.Descriptor;

        _quantityLabel.IsVisibleInParent = !Icon.IsDragging && descriptor.IsStackable && bankSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bankSlot.Quantity);
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
