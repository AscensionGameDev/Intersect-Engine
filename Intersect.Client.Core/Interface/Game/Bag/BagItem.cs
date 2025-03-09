using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Interface.Game.Inventory;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Collections.Slotting;
using Intersect.Configuration;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Items;

namespace Intersect.Client.Interface.Game.Bag;

public partial class BagItem : SlotItem
{
    // Controls
    private readonly Label _quantityLabel;
    private readonly BagWindow _bagWindow;
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
    private readonly MenuItem _withdrawContextItem;

    public BagItem(BagWindow bagWindow, Base parent, int index, ContextMenu contextMenu) : base(parent, nameof(BagItem), index, contextMenu)
    {
        _bagWindow = bagWindow;
        TextureFilename = "bagitem.png";

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

        _contextMenu.ClearChildren();
        _withdrawContextItem = _contextMenu.AddItem(Strings.BagContextMenu.Withdraw);
        _withdrawContextItem.Clicked += _withdrawMenuItem_Clicked;
        _contextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
    }

    #region Context Menu

    public override void OpenContextMenu()
    {
        if (Globals.BagSlots is not { Length: > 0 } bagSlots)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(bagSlots[SlotIndex].ItemId, out var item))
        {
            return;
        }

        _withdrawContextItem.SetText(Strings.BagContextMenu.Withdraw.ToString(item.Name));
        base.OpenContextMenu();
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

    private void _iconImage_HoverEnter(Base? sender, EventArgs? arguments)
    {
        if (InputHandler.MouseFocus != default)
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

        if (_descWindow != default)
        {
            _descWindow.Dispose();
            _descWindow = default;
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
        _descWindow = new ItemDescriptionWindow(
            item.Descriptor,
            item.Quantity,
            _bagWindow.X,
            _bagWindow.Y,
            item.ItemProperties
        );
    }

    private void _iconImage_HoverLeave(Base sender, EventArgs arguments)
    {
        _mouseOver = false;
        _mouseX = -1;
        _mouseY = -1;

        if (_descWindow != default)
        {
            _descWindow.Dispose();
            _descWindow = default;
        }
    }

    private void _iconImage_Clicked(Base sender, MouseButtonState arguments)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (arguments.MouseButton)
        {
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

            case MouseButton.Left:
                _clickTime = Timing.Global.MillisecondsUtc + 500;
                break;
        }
    }

    private void _iconImage_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.InBag)
        {
            Globals.Me?.TryRetrieveItemFromBag(SlotIndex, -1);
        }
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
            _iconImage.Texture = default;
            _textureLoaded = default;
            return;
        }

        var bagSlot = bagSlots[SlotIndex];
        var descriptor = bagSlot.Descriptor;

        _quantityLabel.IsVisibleInParent = !IsDragging && descriptor.IsStackable && bagSlot.Quantity > 1;
        if (_quantityLabel.IsVisibleInParent)
        {
            _quantityLabel.Text = Strings.FormatQuantityAbbreviated(bagSlot.Quantity);
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
                        _clickTime = 0;
                    }
                }
                else
                {
                    if (_canDrag && Draggable.Active == default)
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
        else
        {
            if (_dragIcon?.Update() == true)
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

                //So we picked up an item and then dropped it. Lets see where we dropped it to.
                //Check inventory first.
                if (_bagWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    for (var i = 0; i < Globals.BagSlots.Length; i++)
                    {
                        var slot = _bagWindow.Items[i];
                        if (slot is not BagItem bagItem)
                        {
                            continue;
                        }

                        if (bagItem.RenderBounds().IntersectsWith(dragRect))
                        {
                            if (FloatRect.Intersect(bagItem.RenderBounds(), dragRect).Width *
                                FloatRect.Intersect(bagItem.RenderBounds(), dragRect).Height >
                                bestIntersect)
                            {
                                bestIntersect =
                                    FloatRect.Intersect(bagItem.RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(bagItem.RenderBounds(), dragRect).Height;

                                bestIntersectIndex = i;
                            }
                        }
                    }

                    if (bestIntersectIndex > -1)
                    {
                        if (SlotIndex != bestIntersectIndex)
                        {
                            //Try to swap....
                            PacketSender.SendMoveBagItems(SlotIndex, bestIntersectIndex);
                        }
                    }
                }
                else
                {
                    var invWindow = Interface.GameUi.GameMenu.GetInventoryWindow();

                    if (invWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.Instance.Player.MaxInventory; i++)
                        {
                            if(invWindow.Items[i] is not InventoryItem inventoryItem)
                            {
                                continue;
                            }

                            if (inventoryItem.RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(inventoryItem.RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(inventoryItem.RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(inventoryItem.RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(inventoryItem.RenderBounds(), dragRect).Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.TryRetrieveItemFromBag(SlotIndex, bestIntersectIndex);
                        }
                    }
                }

                _dragIcon.Dispose();
            }
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
