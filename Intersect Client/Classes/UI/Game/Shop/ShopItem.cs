using System;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Shop
{
    public class ShopWindowItem
    {
        private int mCurrentItem = -2;
        private ItemDescWindow mDescWindow;
        private bool mIsEquipped;

        //Slot info
        private int mMySlot;

        //Drag/Drop References
        private ShopWindow mShopWindow;

        public ImagePanel Container;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;
        private int mMouseY = -1;
        public ImagePanel Pnl;

        //Textures
        private GameRenderTexture mSfTex;

        public ShopWindowItem(ShopWindow shopWindow, int index)
        {
            mShopWindow = shopWindow;
            mMySlot = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "ShopItemIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.DoubleClicked += Pnl_DoubleClicked;
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[mMySlot].ItemNum);
            if (item != null)
            {
                GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Pic);
                if (itemTex != null)
                {
                    Pnl.Texture = itemTex;
                }
            }
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            //Confirm the purchase
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[mMySlot].ItemNum);
            if (item != null)
            {
                if (item.IsStackable())
                {
                    InputBox iBox = new InputBox(Strings.Get("shop", "buyitem"),
                        Strings.Get("shop", "buyitemprompt", item.Name), true, InputBox.InputType.TextInput,
                        BuyItemInputBoxOkay, null, mMySlot);
                }
                else
                {
                    PacketSender.SendBuyItem(mMySlot, 1);
                }
            }
        }

        private void BuyItemInputBoxOkay(object sender, EventArgs e)
        {
            int value = (int) ((InputBox) sender).Value;
            if (value > 0)
            {
                PacketSender.SendBuyItem(((InputBox) sender).UserData, value);
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
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[mMySlot].CostItemNum);
            if (item != null)
                mDescWindow = new ItemDescWindow(Globals.GameShop.SellingItems[mMySlot].ItemNum, 1, mShopWindow.X - 255,
                    mShopWindow.Y, item.StatsGiven, "",
                    Strings.Get("shop", "costs", Globals.GameShop.SellingItems[mMySlot].CostItemVal, item.Name));
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
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