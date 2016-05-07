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
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;

using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.UI.Game
{
    public class BankWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        //Controls
        private WindowControl _bankWindow;
        private ScrollControl _itemContainer;

        //Location
        public int X;
        public int Y;

        public List<BankItem> Items = new List<BankItem>();
        private List<Label> _values = new List<Label>();

        //Init
        public BankWindow(Canvas _gameCanvas)
        {
            _bankWindow = new WindowControl(_gameCanvas, "Bank");
            _bankWindow.SetSize(400, 400);
            _bankWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200, GameGraphics.Renderer.GetScreenHeight() / 2 - 200);
            _bankWindow.DisableResizing();
            _bankWindow.Margin = Margin.Zero;
            _bankWindow.Padding = Padding.Zero;
            X = _bankWindow.X;
            Y = _bankWindow.Y;

            _itemContainer = new ScrollControl(_bankWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_bankWindow.Width, _bankWindow.Height - 24);
            _itemContainer.EnableScroll(false, true);
            InitItemContainer();
        }

        public void Close()
        {
            _bankWindow.Close();
        }
        public bool IsVisible()
        {
            return !_bankWindow.IsHidden;
        }
        public void Hide()
        {
            _bankWindow.IsHidden = true;
        }

        public void Update()
        {
            if (_bankWindow.IsHidden == true) { return; }
            X = _bankWindow.X;
            Y = _bankWindow.Y;
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                if (Globals.Bank[i] != null &&  Globals.Bank[i].ItemNum > -1)
                {
                    var item = ItemBase.GetItem(Globals.Bank[i].ItemNum);
                    if (item != null)
                    {
                        Items[i].pnl.IsHidden = false;

                        if (item.ItemType == (int)ItemTypes.Consumable || //Allow Stacking on Currency, Consumable, Spell, and item types of none.
                            item.ItemType == (int)ItemTypes.Currency ||
                            item.ItemType == (int)ItemTypes.None ||
                            item.ItemType == (int)ItemTypes.Spell)
                        {
                            _values[i].IsHidden = false;
                            _values[i].Text = Globals.Bank[i].ItemVal.ToString();
                        }
                        else
                        {
                            _values[i].IsHidden = true;
                        }

                        if (Items[i].IsDragging)
                        {
                            Items[i].pnl.IsHidden = true;
                            _values[i].IsHidden = true;
                        }
                        Items[i].Update();
                    }
                }
                else
                {
                    Items[i].pnl.IsHidden = true;
                    _values[i].IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                Items.Add(new BankItem(this, i));
                Items[i].pnl = new ImagePanel(_itemContainer);
                Items[i].pnl.SetSize(32, 32);
                Items[i].pnl.SetPosition((i % (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemXPadding) + ItemXPadding, (i / (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemYPadding) + ItemYPadding);
                Items[i].pnl.IsHidden = false;
                Items[i].Setup();

                _values.Add(new Label(_itemContainer));
                _values[i].Text = "";
                _values[i].SetPosition((i % (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemXPadding) + ItemXPadding, (i / (_itemContainer.Width / (32 + ItemXPadding))) * (32 + ItemYPadding) + ItemYPadding + 24);
                _values[i].TextColor = Color.Black;
                _values[i].MakeColorDark();
                _values[i].IsHidden = true;
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect();
            rect.X = _bankWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2;
            rect.Y = _bankWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2;
            rect.Width = _bankWindow.Width + ItemXPadding;
            rect.Height = _bankWindow.Height + ItemYPadding;
            return rect;
        }
    }

    public class BankItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        public ImagePanel pnl;
        private ItemDescWindow _descWindow;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;
        private long ClickTime = 0;

        //Dragging
        private bool CanDrag = false;
        private Draggable dragIcon;
        public bool IsDragging;

        //Slot info
        private int _mySlot;
        private bool _isEquipped;
        private int _currentItem = -2;

        //Drag/Drop References
        private BankWindow _bankWindow;


        public BankItem(BankWindow bankWindow, int index)
        {
            _bankWindow = bankWindow;
            _mySlot = index;
        }

        public void Setup()
        {
            pnl.HoverEnter += pnl_HoverEnter;
            pnl.HoverLeave += pnl_HoverLeave;
            pnl.RightClicked += pnl_RightClicked;
            pnl.DoubleClicked += Pnl_DoubleClicked;
            pnl.Clicked += pnl_Clicked;
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.InBank)
            {
                Globals.Me.TryWithdrawItem(_mySlot);
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryDropItem(_mySlot);
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
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left)) { CanDrag = false; return; }
            if (_descWindow != null) { _descWindow.Dispose(); _descWindow = null; }
            if (Globals.Bank[_mySlot] != null)
            {
                _descWindow = new ItemDescWindow(Globals.Bank[_mySlot].ItemNum, Globals.Bank[_mySlot].ItemVal, _bankWindow.X - 220, _bankWindow.Y, Globals.Bank[_mySlot].StatBoost);
            }
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
            bool equipped = false;
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.Equipment[i] == _mySlot)
                {
                    equipped = true;
                }
            }
            if (Globals.Bank[_mySlot].ItemNum != _currentItem || equipped != _isEquipped)
            {
                _currentItem = Globals.Bank[_mySlot].ItemNum;
                _isEquipped = equipped;
                var item = ItemBase.GetItem(Globals.Bank[_mySlot].ItemNum);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Pic);
                    if (itemTex != null)
                    {
                        pnl.Texture =  itemTex;
                    }
                    else
                    {
                        if (pnl.Texture != null)
                        {
                            pnl.Texture = null;
                        }
                    }
                }
                else
                {
                    if (pnl.Texture != null)
                    {
                        pnl.Texture = null;
                    }
                }
            }
            if (!IsDragging)
            {
                if (MouseOver)
                {
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        CanDrag = true;
                        MouseX = -1;
                        MouseY = -1;
                        if (Globals.System.GetTimeMS() < ClickTime)
                        {
                            //Globals.Me.TryUseItem(_mySlot);
                            ClickTime = 0;
                        }
                    }
                    else
                    {
                        if (CanDrag)
                        {
                            if (MouseX == -1 || MouseY == -1)
                            {
                                MouseX = InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X;
                                MouseY = InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y;

                            }
                            else
                            {
                                int xdiff = MouseX - (InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X);
                                int ydiff = MouseY - (InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseX, pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseY, pnl.Texture);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (dragIcon.Update())
                {
                    //Drug the item and now we stopped
                    IsDragging = false;
                    FloatRect dragRect = new FloatRect(dragIcon.x - ItemXPadding / 2, dragIcon.y - ItemYPadding / 2, ItemXPadding / 2 + 32, ItemYPadding / 2 + 32);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (_bankWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_bankWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (_mySlot != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendMoveBankItems(bestIntersectIndex, _mySlot);
                                //Globals.Me.SwapItems(bestIntersectIndex, _mySlot);
                            }
                        }
                    }
                    dragIcon.Dispose();
                }
            }
        }
    }
}
