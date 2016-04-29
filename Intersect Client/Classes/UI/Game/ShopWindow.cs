/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Library;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class ShopWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        //Controls
        private WindowControl _shopWindow;
        private ScrollControl _itemContainer;

        //Location
        public int X;
        public int Y;

        public List<ShopWindowItem> Items = new List<ShopWindowItem>();

        //Init
        public ShopWindow(Canvas _gameCanvas)
        {
            _shopWindow = new WindowControl(_gameCanvas, Globals.GameShop.Name);
            _shopWindow.SetSize(400, 400);
            _shopWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200, GameGraphics.Renderer.GetScreenHeight() /2 - 200);
            _shopWindow.DisableResizing();
            _shopWindow.Margin = Margin.Zero;
            _shopWindow.Padding = Padding.Zero;
            X = _shopWindow.X;
            Y = _shopWindow.Y;

            _itemContainer = new ScrollControl(_shopWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_shopWindow.Width, _shopWindow.Height - 24);
            _itemContainer.EnableScroll(false, true);
            InitItemContainer();
        }

        public void Close()
        {
            _shopWindow.Close();
        }
        public bool IsVisible()
        {
            return !_shopWindow.IsHidden;
        }
        public void Hide()
        {
            _shopWindow.IsHidden = true;
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Globals.GameShop.SellingItems.Count; i++)
            {
                Items.Add(new ShopWindowItem(this, i));
                Items[i].pnl = new ImagePanel(_itemContainer);
                Items[i].pnl.SetSize(32, 32);
                Items[i].pnl.SetPosition((i % (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemXPadding) + ItemXPadding, (i / (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemYPadding) + ItemYPadding);
                Items[i].pnl.IsHidden = false;
                Items[i].Setup();
            }
        }
    }

    public class ShopWindowItem
    {
        public ImagePanel pnl;
        private ItemDescWindow _descWindow;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;

        //Slot info
        private int _mySlot;
        private bool _isEquipped;
        private int _currentItem = -2;

        //Textures
        private GameRenderTexture sfTex;

        //Drag/Drop References
        private ShopWindow _shopWindow;


        public ShopWindowItem(ShopWindow shopWindow, int index)
        {
            _shopWindow = shopWindow;
            _mySlot = index;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.DoubleClicked += Pnl_DoubleClicked;
            GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Pic);
            if (itemTex != null)
            {
                pnl.Texture = itemTex;
            }
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            //Confirm the purchase
            if (Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Type == (int) ItemTypes.Consumable ||
                Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Type == (int) ItemTypes.Currency ||
                Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Type == (int) ItemTypes.None ||
                Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Type == (int) ItemTypes.Spell)
            {
                InputBox iBox = new InputBox("Buy Item", "How many " + Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].ItemNum].Name + "(s) would you like to buy?", true, BuyItemInputBoxOkay, null, -1, true);
            }
            else
            {
                PacketSender.SendBuyItem(_mySlot, 1);
            }
        }
        private void BuyItemInputBoxOkay(Object sender, EventArgs e)
        {
            int value = (int)((InputBox)sender).Value;
            if (value > 0)
            {
                PacketSender.SendSellItem(((InputBox)sender).Slot, value);
            }
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            MouseOver = false;
            MouseX = -1;
            MouseY = -1;
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            _descWindow = new ItemDescWindow(Globals.GameShop.SellingItems[_mySlot].ItemNum, 1, _shopWindow.X - 220, _shopWindow.Y, null,"", "Costs " + Globals.GameShop.SellingItems[_mySlot].CostItemVal + " " +  Globals.GameItems[Globals.GameShop.SellingItems[_mySlot].CostItemNum].Name + "(s)");
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = pnl.LocalPosToCanvas(new Point(0, 0)).X;
            rect.Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y;
            rect.Width = pnl.Width;
            rect.Height = pnl.Height;
            return rect;
        }

        public void Update()
        {

        }
    }
}
