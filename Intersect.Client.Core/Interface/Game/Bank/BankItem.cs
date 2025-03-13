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
using Intersect.Client.Interface.Game.DescriptionWindows;
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
    private ItemDescriptionWindow? _descWindow;

    // Context Menu Handling
    private MenuItem _withdrawContextItem;

    public BankItem(BankWindow bankWindow, Base parent, int index, ContextMenu contextMenu) :
        base(parent, nameof(BankItem), index, contextMenu)
    {
        _bankWindow = bankWindow;
        TextureFilename = "bankitem.png";

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
        _withdrawContextItem = contextMenu.AddItem(Strings.BankContextMenu.Withdraw);
        _withdrawContextItem.Clicked += _withdrawMenuItem_Clicked;
        contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    protected override void OnContextMenuOpening(ContextMenu contextMenu)
    {
        if (Globals.BankSlots is not { Length: > 0 } bankSlots)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(bankSlots[SlotIndex].ItemId, out var item))
        {
            return;
        }

        // Clear the context menu and add the withdraw item with updated item name
        contextMenu.ClearChildren();
        contextMenu.AddChild(_withdrawContextItem);
        _withdrawContextItem.SetText(Strings.BankContextMenu.Withdraw.ToString(item.Name));

        base.OnContextMenuOpening(contextMenu);
    }

    private void _withdrawMenuItem_Clicked(Base sender, MouseButtonState arguments)
    {
        Globals.Me?.TryRetrieveItemFromBank(SlotIndex);
    }

    #endregion

    #region Mouse Events

    private void IconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            return;
        }


        _descWindow?.Dispose();
        _descWindow = null;

        if (Globals.BankSlots is not { Length: > 0 } bankSlots)
        {
            return;
        }

        if (bankSlots[SlotIndex] is not { Descriptor: not null } or { Quantity: <= 0 })
        {
            return;
        }

        var item = bankSlots[SlotIndex];
        _descWindow = new ItemDescriptionWindow(
            item.Descriptor,
            item.Quantity,
            _bankWindow.X,
            _bankWindow.Y,
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
        if (!Globals.InBank)
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
        var targetNode = Interface.FindComponentUnderCursor(NodeFilter.None);

        // Find the first parent acceptable in that tree that can accept the package
        while (targetNode != default)
        {
            switch (targetNode)
            {
                case BankItem bankItem:
                    if (Globals.IsGuildBank)
                    {
                        if (Globals.Me is not { } player)
                        {
                            return false;
                        }

                        var rank = player.GuildRank;
                        var isInGuild = !string.IsNullOrWhiteSpace(player.Guild);
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

                    Globals.Me?.TryRetrieveItemFromBank(
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
            
            // If we've reached the top of the tree, we can't drop here, so cancel drop
            if (targetNode == null)
            {
                return false;
            }
        }

        return false;
    }

    #endregion

    public new void Update()
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
            IconImage.Texture = default;
            return;
        }

        var bankSlot = bankSlots[SlotIndex];
        var descriptor = bankSlot.Descriptor;

        //TODO: dont show when is dragging
        _quantityLabel.IsVisibleInParent = descriptor.IsStackable && bankSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bankSlot.Quantity);
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
