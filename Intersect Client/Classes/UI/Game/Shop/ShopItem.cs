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
        private int _currentItem = -2;
        private ItemDescWindow _descWindow;
        private bool _isEquipped;

        //Slot info
        private int _mySlot;

        //Drag/Drop References
        private ShopWindow _shopWindow;

        public ImagePanel container;

        //Mouse Event Variables
        private bool MouseOver;

        private int MouseX = -1;
        private int MouseY = -1;
        public ImagePanel pnl;

        //Textures
        private GameRenderTexture sfTex;

        public ShopWindowItem(ShopWindow shopWindow, int index)
        {
            _shopWindow = shopWindow;
            _mySlot = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container, "ShopItemIcon");
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.DoubleClicked += Pnl_DoubleClicked;
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[_mySlot].ItemNum);
            if (item != null)
            {
                GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Pic);
                if (itemTex != null)
                {
                    pnl.Texture = itemTex;
                }
            }
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            //Confirm the purchase
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[_mySlot].ItemNum);
            if (item != null)
            {
                if (item.IsStackable())
                {
                    InputBox iBox = new InputBox(Strings.Get("shop", "buyitem"),
                        Strings.Get("shop", "buyitemprompt", item.Name), true, InputBox.InputType.TextInput,
                        BuyItemInputBoxOkay, null, _mySlot);
                }
                else
                {
                    PacketSender.SendBuyItem(_mySlot, 1);
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
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            var item = ItemBase.Lookup.Get<ItemBase>(Globals.GameShop.SellingItems[_mySlot].CostItemNum);
            if (item != null)
                _descWindow = new ItemDescWindow(Globals.GameShop.SellingItems[_mySlot].ItemNum, 1, _shopWindow.X - 255,
                    _shopWindow.Y, item.StatsGiven, "",
                    Strings.Get("shop", "costs", Globals.GameShop.SellingItems[_mySlot].CostItemVal, item.Name));
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
                Width = pnl.Width,
                Height = pnl.Height
            };
            return rect;
        }

        public void Update()
        {
        }
    }
}