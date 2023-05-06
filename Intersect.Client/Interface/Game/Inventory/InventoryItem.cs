using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Inventory
{

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

        private string mTexLoaded = "";

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
            Pnl.RightClicked += pnl_RightClicked;
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

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.GameShop != null)
            {
                Globals.Me.TrySellItem(mMySlot);
            }
            else if (Globals.InBank)
            {
                Globals.Me.TryDepositItem(mMySlot);
            }
            else if (Globals.InBag)
            {
                Globals.Me.TryStoreBagItem(mMySlot, -1);
            }
            else if (Globals.InTrade)
            {
                Globals.Me.TryTradeItem(mMySlot);
            }
            else
            {
                Globals.Me.TryUseItem(mMySlot);
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Timing.Global.MillisecondsUtc + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                mInventoryWindow.OpenContextMenu(mMySlot);
            }
            else
            {
                if (Globals.GameShop != null)
                {
                    Globals.Me.TrySellItem(mMySlot);
                }
                else if (Globals.InBank)
                {
                    Globals.Me.TryDepositItem(mMySlot);
                }
                else if (Globals.InBag)
                {
                    Globals.Me.TryStoreBagItem(mMySlot, -1);
                }
                else if (Globals.InTrade)
                {
                    Globals.Me.TryTradeItem(mMySlot);
                }
                else
                {
                    Globals.Me.TryDropItem(mMySlot);
                }
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
            if (Globals.InputManager.MouseButtonDown(MouseButtons.Left))
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
                            Strings.Shop.sellsfor.ToString(shopItem.CostItemQuantity, hoveredItem.Name)
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
                            Strings.Shop.sellsfor.ToString(invItem.Base.Price.ToString(), costItem.Name)
                        );
                    }
                }
                else
                {
                    if (invItem?.Base != null)
                    {
                        mDescWindow = new ItemDescriptionWindow(
                            invItem.Base, invItem.Quantity, mInventoryWindow.X, mInventoryWindow.Y, invItem.ItemProperties,
                            "", Strings.Shop.wontbuy
                        );
                    }
                }
            }
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };

            return rect;
        }

        public void Update()
        {
            var equipped = false;
            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
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
                        mCooldownLabel.IsHidden = false;
                        var secondsRemaining = (float) Globals.Me.GetItemRemainingCooldown(mMySlot) / 1000f;
                        if (secondsRemaining > 10f)
                        {
                            mCooldownLabel.Text = Strings.Inventory.Cooldown.ToString(secondsRemaining.ToString("N0"));
                        }
                        else
                        {
                            mCooldownLabel.Text = Strings.Inventory.Cooldown.ToString(
                                secondsRemaining.ToString("N1").Replace(".", Strings.Numbers.dec)
                            );
                        }
                    }
                }
                else
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }

                    mTexLoaded = "";
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
                    if (!Globals.InputManager.MouseButtonDown(MouseButtons.Left))
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
                                mMouseX = InputHandler.MousePosition.X - Pnl.LocalPosToCanvas(new Point(0, 0)).X;
                                mMouseY = InputHandler.MousePosition.Y - Pnl.LocalPosToCanvas(new Point(0, 0)).Y;
                            }
                            else
                            {
                                var xdiff = mMouseX -
                                            (InputHandler.MousePosition.X - Pnl.LocalPosToCanvas(new Point(0, 0)).X);

                                var ydiff = mMouseY -
                                            (InputHandler.MousePosition.Y - Pnl.LocalPosToCanvas(new Point(0, 0)).Y);

                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    mDragIcon = new Draggable(
                                        Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseX,
                                        Pnl.LocalPosToCanvas(new Point(0, 0)).X + mMouseY, Pnl.Texture, Pnl.RenderColor
                                    );
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (mDragIcon.Update())
                {
                    //Drug the item and now we stopped
                    IsDragging = false;
                    var dragRect = new FloatRect(
                        mDragIcon.X - (Container.Padding.Left + Container.Padding.Right) / 2,
                        mDragIcon.Y - (Container.Padding.Top + Container.Padding.Bottom) / 2,
                        (Container.Padding.Left + Container.Padding.Right) / 2 + Pnl.Width,
                        (Container.Padding.Top + Container.Padding.Bottom) / 2 + Pnl.Height
                    );

                    float bestIntersect = 0;
                    var bestIntersectIndex = -1;

                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (mInventoryWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (mInventoryWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(mInventoryWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(mInventoryWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(mInventoryWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(mInventoryWindow.Items[i].RenderBounds(), dragRect).Height;

                                    bestIntersectIndex = i;
                                }
                            }
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
                        for (var i = 0; i < Options.Instance.PlayerOpts.HotbarSlotCount; i++)
                        {
                            if (Interface.GameUi.Hotbar.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(
                                            Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect
                                        )
                                        .Width *
                                    FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                        .Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Width *
                                        FloatRect.Intersect(Interface.GameUi.Hotbar.Items[i].RenderBounds(), dragRect)
                                            .Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            Globals.Me.AddToHotbar((byte) bestIntersectIndex, 0, mMySlot);
                        }
                    }
                    else if (Globals.InBag)
                    {
                        var bagWindow = Interface.GameUi.GetBagWindow();
                        if (bagWindow.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (var i = 0; i < Globals.Bag.Length; i++)
                            {
                                if (bagWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (FloatRect.Intersect(bagWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(bagWindow.Items[i].RenderBounds(), dragRect).Height >
                                        bestIntersect)
                                    {
                                        bestIntersect =
                                            FloatRect.Intersect(bagWindow.Items[i].RenderBounds(), dragRect).Width *
                                            FloatRect.Intersect(bagWindow.Items[i].RenderBounds(), dragRect).Height;

                                        bestIntersectIndex = i;
                                    }
                                }
                            }

                            if (bestIntersectIndex > -1)
                            {
                                Globals.Me.TryStoreBagItem(mMySlot, bestIntersectIndex);
                            }
                        }
                    }
                    else if (Globals.InBank)
                    {
                        var bankWindow = Interface.GameUi.GetBankWindow();
                        if (bankWindow.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (var i = 0; i < Globals.Bank.Length; i++)
                            {
                                if (bankWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (FloatRect.Intersect(bankWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(bankWindow.Items[i].RenderBounds(), dragRect).Height >
                                        bestIntersect)
                                    {
                                        bestIntersect =
                                            FloatRect.Intersect(bankWindow.Items[i].RenderBounds(), dragRect).Width *
                                            FloatRect.Intersect(bankWindow.Items[i].RenderBounds(), dragRect).Height;

                                        bestIntersectIndex = i;
                                    }
                                }
                            }

                            if (bestIntersectIndex > -1)
                            {
                                Globals.Me.TryDepositItem(mMySlot, bestIntersectIndex);
                            }
                        }
                    }

                    mDragIcon.Dispose();
                }
            }
        }

    }

}
