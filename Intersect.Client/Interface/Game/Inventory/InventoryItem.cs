using System;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory
{

    public class InventoryItem
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

        private ItemDescWindow mDescWindow;

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
            EquipPanel = new ImagePanel(Pnl, "InventoryItemEquippedIcon");
            EquipPanel.Texture = Graphics.Renderer.GetWhiteTexture();
            EquipLabel = new Label(Pnl, "InventoryItemEquippedLabel");
            EquipLabel.IsHidden = true;
            EquipLabel.Text = Strings.Inventory.equippedicon;
            EquipLabel.TextColor = new Color(0, 255, 255, 255);
            mCooldownLabel = new Label(Pnl, "InventoryItemCooldownLabel");
            mCooldownLabel.IsHidden = true;
            mCooldownLabel.TextColor = new Color(0, 255, 255, 255);
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Globals.System.GetTimeMs() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
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
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
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
                    mDescWindow = new ItemDescWindow(
                        Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity, mInventoryWindow.X,
                        mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].StatBuffs
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
                        mDescWindow = new ItemDescWindow(
                            Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity,
                            mInventoryWindow.X, mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].StatBuffs, "",
                            Strings.Shop.sellsfor.ToString(shopItem.CostItemQuantity, hoveredItem.Name)
                        );
                    }
                }
                else if (shopItem == null)
                {
                    var costItem = Globals.GameShop.DefaultCurrency;
                    if (invItem.Base != null && costItem != null && Globals.Me.Inventory[mMySlot]?.Base != null)
                    {
                        mDescWindow = new ItemDescWindow(
                            Globals.Me.Inventory[mMySlot].Base, Globals.Me.Inventory[mMySlot].Quantity,
                            mInventoryWindow.X, mInventoryWindow.Y, Globals.Me.Inventory[mMySlot].StatBuffs, "",
                            Strings.Shop.sellsfor.ToString(invItem.Base.Price.ToString(), costItem.Name)
                        );
                    }
                }
                else
                {
                    if (invItem?.Base != null)
                    {
                        mDescWindow = new ItemDescWindow(
                            invItem.Base, invItem.Quantity, mInventoryWindow.X, mInventoryWindow.Y, invItem.StatBuffs,
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
                mIconCd != Globals.Me.ItemOnCd(mMySlot) ||
                Globals.Me.ItemOnCd(mMySlot))
            {
                mCurrentItemId = Globals.Me.Inventory[mMySlot].ItemId;
                mCurrentAmt = Globals.Me.Inventory[mMySlot].Quantity;
                mIsEquipped = equipped;
                EquipPanel.IsHidden = !mIsEquipped;
                EquipLabel.IsHidden = !mIsEquipped;
                mCooldownLabel.IsHidden = true;
                if (item != null)
                {
                    var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
                    if (itemTex != null)
                    {
                        Pnl.Texture = itemTex;
                        if (Globals.Me.ItemOnCd(mMySlot))
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
                    mIconCd = Globals.Me.ItemOnCd(mMySlot);
                    if (mIconCd)
                    {
                        mCooldownLabel.IsHidden = false;
                        var secondsRemaining = (float) Globals.Me.ItemCdRemainder(mMySlot) / 1000f;
                        if (secondsRemaining > 10f)
                        {
                            mCooldownLabel.Text = Strings.Inventory.cooldown.ToString(secondsRemaining.ToString("N0"));
                        }
                        else
                        {
                            mCooldownLabel.Text = Strings.Inventory.cooldown.ToString(
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
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        mCanDrag = true;
                        mMouseX = -1;
                        mMouseY = -1;
                        if (Globals.System.GetTimeMs() < mClickTime)
                        {
                            Globals.Me.TryUseItem(mMySlot);
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
                                //Try to swap....
                                PacketSender.SendSwapInvItems(bestIntersectIndex, mMySlot);
                                Globals.Me.SwapItems(bestIntersectIndex, mMySlot);
                            }
                        }
                    }
                    else if (Interface.GameUi.Hotbar.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Options.MaxHotbar; i++)
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
                        var bagWindow = Interface.GameUi.GetBag();
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

                    mDragIcon.Dispose();
                }
            }
        }

    }

}
