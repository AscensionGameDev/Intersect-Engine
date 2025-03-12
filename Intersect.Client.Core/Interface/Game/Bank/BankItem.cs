using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
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
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Bank;

public partial class BankItem : SlotItem
{
    // Controls
    private readonly Label _quantityLabel;
    private BankWindow _bankWindow;
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
    private string? _textureLoaded;

    // Context Menu Handling
    private MenuItem _withdrawContextItem;

    public BankItem(BankWindow bankWindow, Base parent, int index, ContextMenu contextMenu) :
        base(parent, nameof(BankItem), index, contextMenu)
    {
        _bankWindow = bankWindow;
        TextureFilename = "bankitem.png";

        _iconImage.HoverEnter += _iconImage_HoverEnter;
        _iconImage.HoverLeave += _iconImage_HoverLeave;
        _iconImage.Clicked += _iconImage_Clicked;
        _iconImage.DoubleClicked += _iconImage_DoubleClicked;

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

    #region Mouse Events

    private void _iconImage_HoverEnter(Base? sender, EventArgs? arguments)
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
                    _iconImage_DoubleClicked(sender, arguments);
                }
                break;
        }
    }

    private void _iconImage_DoubleClicked(Base sender, MouseButtonState arguments)
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
            _iconImage.Texture = default;
            _textureLoaded = default;
            return;
        }

        var bankSlot = bankSlots[SlotIndex];
        var descriptor = bankSlot.Descriptor;

        _quantityLabel.IsVisibleInParent = !IsDragging && descriptor.IsStackable && bankSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bankSlot.Quantity);
        }

        if (_textureLoaded != descriptor.Icon)
        {
            var itemTex = Globals.ContentManager?.GetTexture(Framework.Content.TextureType.Item, descriptor.Icon);
            if (itemTex != default)
            {
                _iconImage.Texture = itemTex;
                _iconImage.RenderColor = descriptor.Color;
                _iconImage.IsVisibleInParent = true;
            }
            else
            {
                if (_iconImage.Texture != default)
                {
                    _iconImage.Texture = default;
                    _iconImage.IsVisibleInParent = false;
                }
            }

            _textureLoaded = descriptor.Icon;

            if (_descWindow != default)
            {
                _descWindow.Dispose();
                _descWindow = default;
                _iconImage_HoverEnter(default, default);
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
                        //Globals.Me.TryUseItem(_mySlot);
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
            var dragRect = new FloatRect(
                _dragIcon.X - (Padding.Left + Padding.Right) / 2f,
                _dragIcon.Y - (Padding.Top + Padding.Bottom) / 2f,
                (Padding.Left + Padding.Right) / 2f + _iconImage.Width,
                (Padding.Top + Padding.Bottom) / 2f + _iconImage.Height
            );

            float bestIntersect = 0;
            var bestIntersectIndex = -1;

            // So we picked up an item and then dropped it. Lets see where we dropped it to.
            if (_bankWindow.RenderBounds().IntersectsWith(dragRect))
            {
                var bankSlotComponents = _bankWindow.Items.ToArray();
                var bankSlotLimit = Math.Min(Globals.BankSlotCount, bankSlotComponents.Length);
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
                    if (intersection.Width * intersection.Height <= bestIntersect)
                    {
                        continue;
                    }

                    bestIntersect = intersection.Width * intersection.Height;
                    bestIntersectIndex = bankSlotIndex;
                }

                if (bestIntersectIndex > -1)
                {
                    if (SlotIndex != bestIntersectIndex)
                    {
                        var allowed = true;

                        //Permission Check
                        if (Globals.IsGuildBank)
                        {
                            var rank = Globals.Me.GuildRank;
                            if (string.IsNullOrWhiteSpace(Globals.Me.Guild) ||
                                (rank?.Permissions.BankDeposit == false && Globals.Me.Rank != 0))
                            {
                                ChatboxMsg.AddMessage(
                                    new ChatboxMsg(
                                        Strings.Guilds.NotAllowedSwap.ToString(Globals.Me.Guild),
                                        CustomColors.Alerts.Error,
                                        ChatMessageType.Bank
                                    )
                                );
                                allowed = false;
                            }
                        }

                        if (allowed)
                        {
                            PacketSender.SendMoveBankItems(SlotIndex, bestIntersectIndex);
                        }
                    }
                }
            }
            else
            {
                var invWindow = Interface.GameUi.GameMenu.GetInventoryWindow();
                if (invWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    var inventorySlots = invWindow.Items.ToArray();
                    var inventorySlotLimit = Math.Min(
                        Globals.Me?.Inventory.Length ?? inventorySlots.Length,
                        inventorySlots.Length
                    );
                    for (var inventoryIndex = 0; inventoryIndex < inventorySlotLimit; inventoryIndex++)
                    {
                        if (inventorySlots[inventoryIndex] is not InventoryItem inventorySlotComponent)
                        {
                            continue;
                        }

                        var inventorySlotRenderBounds = inventorySlotComponent.RenderBounds();
                        if (!inventorySlotRenderBounds.IntersectsWith(dragRect))
                        {
                            continue;
                        }

                        var intersection = FloatRect.Intersect(inventorySlotRenderBounds, dragRect);
                        if (intersection.Width * intersection.Height <= bestIntersect)
                        {
                            continue;
                        }

                        bestIntersect = intersection.Width * intersection.Height;
                        bestIntersectIndex = inventoryIndex;
                    }

                    if (bestIntersectIndex > -1)
                    {
                        var slot = Globals.BankSlots[SlotIndex];
                        Globals.Me?.TryRetrieveItemFromBank(
                            SlotIndex,
                            inventorySlotIndex: bestIntersectIndex,
                            quantityHint: slot.Quantity,
                            skipPrompt: true
                        );
                    }
                }
            }

            _dragIcon.Dispose();
        }
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
}
