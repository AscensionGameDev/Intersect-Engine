using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Shop
{

    public class ShopItem
    {

        public ImagePanel Container;

        private int mCurrentItem = -2;

        private ItemDescWindow mDescWindow;

        private bool mIsEquipped;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;

        private int mMouseY = -1;

        //Slot info
        private int mMySlot;

        //Textures
        private GameRenderTexture mSfTex;

        //Drag/Drop References
        private ShopWindow mShopWindow;

        public ImagePanel Pnl;

        public ShopItem(ShopWindow shopWindow, int index)
        {
            mShopWindow = shopWindow;
            mMySlot = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "ShopItemIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += Pnl_RightClicked;
            Pnl.DoubleClicked += Pnl_RightClicked; //Allow buying via double click OR right click
        }

        private void Pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            //Confirm the purchase
            var item = ItemBase.Get(Globals.GameShop.SellingItems[mMySlot].ItemId);
            if (item != null)
            {
                if (item.IsStackable)
                {
                    var iBox = new InputBox(
                        Strings.Shop.buyitem, Strings.Shop.buyitemprompt.ToString(item.Name), true,
                        InputBox.InputType.NumericInput, BuyItemInputBoxOkay, null, mMySlot
                    );
                }
                else
                {
                    PacketSender.SendBuyItem(mMySlot, 1);
                }
            }
        }

        public void LoadItem()
        {
            var item = ItemBase.Get(Globals.GameShop.SellingItems[mMySlot].ItemId);
            if (item != null)
            {
                var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
                if (itemTex != null)
                {
                    Pnl.Texture = itemTex;
                    Pnl.RenderColor = item.Color;
                }
            }
        }

        private void BuyItemInputBoxOkay(object sender, EventArgs e)
        {
            var value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendBuyItem((int) ((InputBox) sender).UserData, value);
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

            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }

            var item = ItemBase.Get(Globals.GameShop.SellingItems[mMySlot].CostItemId);
            if (item != null && Globals.GameShop.SellingItems[mMySlot].Item != null)
            {
                mDescWindow = new ItemDescWindow(
                    Globals.GameShop.SellingItems[mMySlot].Item, 1, mShopWindow.X, mShopWindow.Y, item.StatsGiven, "",
                    Strings.Shop.costs.ToString(Globals.GameShop.SellingItems[mMySlot].CostItemQuantity, item.Name)
                );
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
        }

    }

}
