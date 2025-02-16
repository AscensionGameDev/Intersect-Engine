using Intersect.Client.Core;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Inventory;


public partial class InventoryItem
{

    public ImagePanel Container;

    public Label EquipLabel;

    public ImagePanel EquipPanel;

    public bool IsDragging;

    //Dragging
    private bool mCanDrag;

    private long mClickTime;

    private Label mCooldownLabel;

    private int mCurrentAmt = 0;

    private Guid mCurrentItemId;

    private ItemDescriptionWindow mDescWindow;

    private Draggable mDragIcon;

    private bool mIconCd;

    //Drag/Drop References
    private InventoryWindow mInventoryWindow;

    private bool mIsEquipped;

    //Mouse Event Variables
    private bool mMouseOver;

    private int mMouseX = -1;

    private int mMouseY = -1;

    //Slot info
    private int mMySlot;

    private string mTexLoaded = string.Empty;

    public ImagePanel Pnl;

    public InventoryItem(InventoryWindow inventoryWindow, int index)
    {
        mInventoryWindow = inventoryWindow;
        mMySlot = index;
    }

    public void Setup()
    {
        Pnl = new ImagePanel(Container, "InventoryItemIcon");
        Pnl.HoverEnter += pnl_HoverEnter;
        Pnl.HoverLeave += pnl_HoverLeave;
        Pnl.Clicked += pnl_Clicked;
        Pnl.DoubleClicked += Pnl_DoubleClicked;
        EquipPanel = new ImagePanel(Pnl, "InventoryItemEquippedIcon");
        EquipPanel.Texture = Graphics.Renderer.GetWhiteTexture();
        EquipLabel = new Label(Pnl, "InventoryItemEquippedLabel");
        EquipLabel.IsHidden = true;
        EquipLabel.Text = Strings.Inventory.EquippedSymbol;
        EquipLabel.TextColor = new Color(0, 255, 255, 255);
        mCooldownLabel = new Label(Pnl, "InventoryItemCooldownLabel");
        mCooldownLabel.IsHidden = true;
        mCooldownLabel.TextColor = new Color(0, 255, 255, 255);
    }

    private void Pnl_DoubleClicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.GameShop != null)
        {
            Globals.Me.TrySellItem(mMySlot);
        }
        else if (Globals.InBank)
        {
            if (Globals.InputManager.IsKeyDown(Keys.Shift))
            {
                Globals.Me.TryStoreItemInBank(
                    mMySlot,
                    skipPrompt: true
                );
            }
            else
            {
                var slot = Globals.Me.Inventory[mMySlot];
                Globals.Me.TryStoreItemInBank(
                    mMySlot,
                    slot,
                    quantityHint: slot.Quantity,
                    skipPrompt: false
                );
            }
        }
        else if (Globals.InBag)
        {
            Globals.Me.TryStoreItemInBag(mMySlot, -1);
        }
        else if (Globals.InTrade)
        {
            Globals.Me.TryOfferItemToTrade(mMySlot);
        }
        else
        {
            Globals.Me.TryUseItem(mMySlot);
        }
    }

    void pnl_Clicked(Base sender, MouseButtonState arguments)
    {
        switch (arguments.MouseButton)
        {
            case MouseButton.Left:
                mClickTime = Timing.Global.MillisecondsUtc + 500;
                break;

            case MouseButton.Right:
                if (ClientConfiguration.Instance.EnableContextMenus)
                {
                    mInventoryWindow.OpenContextMenu(mMySlot);
                }
                else
                {
                    if (Globals.GameShop != null)
                    {
                        Globals.Me?.TrySellItem(mMySlot);
                    }
                    else if (Globals.InBank)
                    {
                        Globals.Me?.TryStoreItemInBank(mMySlot);
                    }
                    else if (Globals.InBag)
                    {
                        Globals.Me?.TryStoreItemInBag(mMySlot, -1);
                    }
                    else if (Globals.InTrade)
                    {
                        Globals.Me?.TryOfferItemToTrade(mMySlot);
                    }
                    else
                    {
                        Globals.Me?.TryDropItem(mMySlot);
                    }
                }
                break;
        }
    }

    void pnl_HoverLeave(Base sender, EventArgs arguments)
    {
        mMouseOver = false;
        mMouseX = -1;
        mMouseY = -1;
        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }
    }

    void pnl_HoverEnter(Base sender, EventArgs arguments)
    {
        if (InputHandler.MouseFocus != null)
        {
            return;
        }

        mMouseOver = true;
        mCanDrag = true;
        if (Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
        {
            mCanDrag = false;

            return;
        }

        if (mDescWindow != null)
        {
            mDescWindow.Dispose();
            mDescWindow = null;
        }

        if (Globals.GameShop == null)
        {
            if (Globals.Me.Inventory[mMySlot]?.Base != null)
            {
                mDescWindow = new ItemDescriptionWindow(
                    Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity, mInventoryWindow.X,
                    mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].ItemProperties
                );
            }
        }
        else
        {
            var invItem = Globals.Me.Inventory[mMySlot];
            ShopItem shopItem = null;
            for (var i = 0; i < Globals.GameShop.BuyingItems.Count; i++)
            {
                var tmpShop = Globals.GameShop.BuyingItems[i];

                if (invItem.ItemId == tmpShop.ItemId)
                {
                    shopItem = tmpShop;

                    break;
                }
            }

            if (Globals.GameShop.BuyingWhitelist && shopItem != null)
            {
                var hoveredItem = ItemBase.Get(shopItem.CostItemId);
                if (hoveredItem != null && Globals.Me.Inventory[mMySlot]?.Base != null)
                {
                    mDescWindow = new ItemDescriptionWindow(
                        Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity,
                        mInventoryWindow.X, mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].ItemProperties, "",
                        Strings.Shop.SellsFor.ToString(shopItem.CostItemQuantity, hoveredItem.Name)
                    );
                }
            }
            else if (shopItem == null)
            {
                var costItem = Globals.GameShop.DefaultCurrency;
                if (invItem.Base != null && costItem != null && Globals.Me.Inventory[mMySlot]?.Base != null)
                {
                    mDescWindow = new ItemDescriptionWindow(
                        Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity,
                        mInventoryWindow.X, mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].ItemProperties, "",
                        Strings.Shop.SellsFor.ToString(invItem.Base.Price.ToString(), costItem.Name)
                    );
                }
            }
            else
            {
                if (invItem?.Base != null)
                {
                    mDescWindow = new ItemDescriptionWindow(
                        invItem.Base, invItem.Quantity, mInventoryWindow.X, mInventoryWindow.Y, invItem.ItemProperties,
                        "", Strings.Shop.WontBuy
                    );
                }
            }
        }
    }

    public FloatRect RenderBounds()
    {
        var rect = new FloatRect()
        {
            X = Pnl.ToCanvas(new Point(0, 0)).X,
            Y = Pnl.ToCanvas(new Point(0, 0)).Y,
            Width = Pnl.Width,
            Height = Pnl.Height
        };

        return rect;
    }

    public void Update()
    {
        var equipped = false;
        for (var i = 0; i < Options.Instance.Equipment.Slots.Count; i++)
        {
            if (Globals.Me.MyEquipment[i] == mMySlot)
            {
                equipped = true;

                break;
            }
        }

        var item = ItemBase.Get(Globals.Me.Inventory[mMySlot].ItemId);
        if (Globals.Me.Inventory[mMySlot].ItemId != mCurrentItemId ||
            Globals.Me.Inventory[mMySlot].Quantity != mCurrentAmt ||
            equipped != mIsEquipped ||
            item == null && mTexLoaded != "" ||
            item != null && mTexLoaded != item.Icon ||
            mIconCd != Globals.Me.IsItemOnCooldown(mMySlot) ||
            Globals.Me.IsItemOnCooldown(mMySlot))
        {
            mCurrentItemId = Globals.Me.Inventory[mMySlot].ItemId;
            mCurrentAmt = Globals.Me.Inventory[mMySlot].Quantity;
            mIsEquipped = equipped;
            EquipPanel.IsHidden = !mIsEquipped;
            EquipLabel.IsHidden = !mIsEquipped;
            mCooldownLabel.IsHidden = true;
            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    Pnl.Texture = itemTex;
                    if (Globals.Me.IsItemOnCooldown(mMySlot))
                    {
                        Pnl.RenderColor = new Color(100, item.Color.R, item.Color.G, item.Color.B);
                    }
                    else
                    {
                        Pnl.RenderColor = item.Color;
                    }
                }
                else
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }
                }

                mTexLoaded = item.Icon;
                mIconCd = Globals.Me.IsItemOnCooldown(mMySlot);
                if (mIconCd)
                {
                    var itemCooldownRemaining = Globals.Me.GetItemRemainingCooldown(mMySlot);
                    mCooldownLabel.IsHidden = false;
                    mCooldownLabel.Text = TimeSpan.FromMilliseconds(itemCooldownRemaining).WithSuffix("0.0");
                }
            }
            else
            {
                if (Pnl.Texture != null)
                {
                    Pnl.Texture = null;
                }

                mTexLoaded = string.Empty;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
                pnl_HoverEnter(null, null);
            }
        }

        if (!IsDragging)
        {
            if (mMouseOver)
            {
                if (!Globals.InputManager.IsMouseButtonDown(MouseButton.Left))
                {
                    mCanDrag = true;
                    mMouseX = -1;
                    mMouseY = -1;
                    if (Timing.Global.MillisecondsUtc < mClickTime)
                    {
                        mClickTime = 0;
                    }
                }
                else
                {
                    if (mCanDrag && Draggable.Active == null)
                    {
                        if (mMouseX == -1 || mMouseY == -1)
                        {
                            mMouseX = InputHandler.MousePosition.X - Pnl.ToCanvas(new Point(0, 0)).X;
                            mMouseY = InputHandler.MousePosition.Y - Pnl.ToCanvas(new Point(0, 0)).Y;
                        }
                        else
                        {
                            var xdiff = mMouseX -
                                        (InputHandler.MousePosition.X - Pnl.ToCanvas(new Point(0, 0)).X);

                            var ydiff = mMouseY -
                                        (InputHandler.MousePosition.Y - Pnl.ToCanvas(new Point(0, 0)).Y);

                            if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                            {
                                IsDragging = true;
                                mDragIcon = new Draggable(
                                    Pnl.ToCanvas(new Point(0, 0)).X + mMouseX,
                                    Pnl.ToCanvas(new Point(0, 0)).X + mMouseY, Pnl.Texture, Pnl.RenderColor
                                );
                            }
                        }
                    }
                }
            }
        }
        else if (mDragIcon.Update())
        {
            //Drug the item and now we stopped
            IsDragging = false;

            var dragRect = new FloatRect(
                mDragIcon.X - (Container.Padding.Left + Container.Padding.Right) / 2f,
                mDragIcon.Y - (Container.Padding.Top + Container.Padding.Bottom) / 2f,
                (Container.Padding.Left + Container.Padding.Right) / 2f + Pnl.Width,
                (Container.Padding.Top + Container.Padding.Bottom) / 2f + Pnl.Height
            );

            float bestIntersect = 0;
            var bestIntersectIndex = -1;

            //So we picked up an item and then dropped it. Lets see where we dropped it to.
            //Check inventory first.
            if (mInventoryWindow.RenderBounds().IntersectsWith(dragRect))
            {
                var inventorySlotComponents = mInventoryWindow.Items.ToArray();
                var inventorySlotLimit = Math.Min(Options.Instance.Player.MaxInventory, inventorySlotComponents.Length);
                for (var inventoryIndex = 0; inventoryIndex < inventorySlotLimit; inventoryIndex++)
                {
                    var inventorySlotComponent = inventorySlotComponents[inventoryIndex];
                    var inventoryRenderBounds = inventorySlotComponent.RenderBounds();

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
                    if (mMySlot != bestIntersectIndex)
                    {
                        Globals.Me.SwapItems(mMySlot, bestIntersectIndex);
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
                    Globals.Me.AddToHotbar((byte)bestIntersectIndex, 0, mMySlot);
                }
            }
            else if (Globals.InBag)
            {
                var bagWindow = Interface.GameUi.GetBagWindow();
                if (bagWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    var bagSlotComponents = bagWindow.Items.ToArray();
                    var bagSlotLimit = Math.Min(Globals.Bag.Length, bagSlotComponents.Length);
                    for (var bagSlotIndex = 0; bagSlotIndex < bagSlotLimit; bagSlotIndex++)
                    {
                        var bagSlotComponent = bagSlotComponents[bagSlotIndex];
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
                        Globals.Me.TryStoreItemInBag(mMySlot, bestIntersectIndex);
                    }
                }
            }
            else if (Globals.InBank)
            {
                var bankWindow = Interface.GameUi.GetBankWindow();
                if (bankWindow.RenderBounds().IntersectsWith(dragRect))
                {
                    var bankSlotComponents = bankWindow.Items.ToArray();
                    var bankSlotLimit = Math.Min(
                        Math.Min(Globals.Bank.Length, Globals.BankSlots),
                        bankSlotComponents.Length
                    );

                    for (var bankSlotIndex = 0; bankSlotIndex < bankSlotLimit; bankSlotIndex++)
                    {
                        var bankSlotComponent = bankSlotComponents[bankSlotIndex];
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
                        var slot = Globals.Me.Inventory[mMySlot];
                        Globals.Me.TryStoreItemInBank(
                            mMySlot,
                            bankSlotIndex: bestIntersectIndex,
                            quantityHint: slot.Quantity,
                            skipPrompt: true
                        );
                    }
                }
            }
            else if (!Globals.Me.IsBusy)
            {
                PacketSender.SendDropItem(mMySlot, Globals.Me.Inventory[mMySlot].Quantity);
            }

            mDragIcon.Dispose();
        }
    }

}
