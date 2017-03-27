using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class BankWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        //Controls
        private WindowControl _bankWindow;
        private ScrollControl _itemContainer;
        private List<Label> _values = new List<Label>();

        public List<BankItem> Items = new List<BankItem>();

        //Location
        public int X;
        public int Y;

        //Init
        public BankWindow(Canvas _gameCanvas)
        {
            _bankWindow = new WindowControl(_gameCanvas, Strings.Get("bank", "title"));
            _bankWindow.SetSize(415, 424);
            _bankWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200,
                GameGraphics.Renderer.GetScreenHeight() / 2 - 200);
            _bankWindow.DisableResizing();
            _bankWindow.Margin = Margin.Zero;
            _bankWindow.Padding = new Padding(8, 5, 9, 11);
            X = _bankWindow.X;
            Y = _bankWindow.Y;
            Gui.InputBlockingElements.Add(_bankWindow);

            _bankWindow.SetTitleBarHeight(24);
            _bankWindow.SetCloseButtonSize(20, 20);
            _bankWindow.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "bankactive.png"),
                WindowControl.ControlState.Active);
            _bankWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"),
                Button.ControlState.Normal);
            _bankWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"),
                Button.ControlState.Hovered);
            _bankWindow.SetCloseButtonImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"),
                Button.ControlState.Clicked);
            _bankWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _bankWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _itemContainer = new ScrollControl(_bankWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_bankWindow.Width - _bankWindow.Padding.Left - _bankWindow.Padding.Right,
                _bankWindow.Height - 24 - _bankWindow.Padding.Top - _bankWindow.Padding.Bottom);
            _itemContainer.EnableScroll(false, true);
            _itemContainer.AutoHideBars = false;

            var scrollbar = _itemContainer.GetVerticalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"),
                Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"),
                Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"),
                Dragger.ControlState.Clicked);

            var upButton = scrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"),
                Button.ControlState.Normal);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"),
                Button.ControlState.Clicked);
            upButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"),
                Button.ControlState.Hovered);
            var downButton = scrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"),
                Button.ControlState.Normal);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"),
                Button.ControlState.Clicked);
            downButton.SetImage(
                Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"),
                Button.ControlState.Hovered);
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
            if (_bankWindow.IsHidden == true)
            {
                return;
            }
            X = _bankWindow.X;
            Y = _bankWindow.Y;
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                if (Globals.Bank[i] != null && Globals.Bank[i].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get(Globals.Bank[i].ItemNum);
                    if (item != null)
                    {
                        Items[i].pnl.IsHidden = false;
                        if (item.IsStackable())
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
                Items[i].container = new ImagePanel(_itemContainer);
                Items[i].container.SetSize(34, 34);
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding,
                    (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding);
                Items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui,
                    "bankitem.png");
                Items[i].Setup();

                _values.Add(new Label(_itemContainer));
                _values[i].Text = "";
                _values[i].SetPosition(
                    (i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding + 2,
                    (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding + 20);
                _values[i].TextColorOverride = Color.White;
                _values[i].IsHidden = true;
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _bankWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _bankWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _bankWindow.Width + ItemXPadding,
                Height = _bankWindow.Height + ItemYPadding
            };
            return rect;
        }
    }

    public class BankItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        //Drag/Drop References
        private BankWindow _bankWindow;
        private int _currentItem = -2;
        private ItemDescWindow _descWindow;

        //Slot info
        private int _mySlot;

        //Dragging
        private bool CanDrag = false;
        private long ClickTime = 0;
        public ImagePanel container;
        private Draggable dragIcon;
        public bool IsDragging;

        //Mouse Event Variables
        private bool MouseOver = false;
        private int MouseX = -1;
        private int MouseY = -1;
        public ImagePanel pnl;

        public BankItem(BankWindow bankWindow, int index)
        {
            _bankWindow = bankWindow;
            _mySlot = index;
        }

        public void Setup()
        {
            pnl = new ImagePanel(container);
            pnl.SetSize(32, 32);
            pnl.SetPosition(1, 1);
            pnl.IsHidden = true;
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
            MouseOver = true;
            CanDrag = true;
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                CanDrag = false;
                return;
            }
            if (_descWindow != null)
            {
                _descWindow.Dispose();
                _descWindow = null;
            }
            if (Globals.Bank[_mySlot] != null)
            {
                _descWindow = new ItemDescWindow(Globals.Bank[_mySlot].ItemNum, Globals.Bank[_mySlot].ItemVal,
                    _bankWindow.X - 255, _bankWindow.Y, Globals.Bank[_mySlot].StatBoost);
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = pnl.LocalPosToCanvas(new Point(0, 0)).X,
                Y = pnl.LocalPosToCanvas(new Point(0, 0)).Y,
                Width = pnl.Width,
                Height = pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            if (Globals.Bank[_mySlot].ItemNum != _currentItem)
            {
                _currentItem = Globals.Bank[_mySlot].ItemNum;
                var item = ItemBase.Lookup.Get(Globals.Bank[_mySlot].ItemNum);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Pic);
                    if (itemTex != null)
                    {
                        pnl.Texture = itemTex;
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
                                int xdiff = MouseX -
                                            (InputHandler.MousePosition.X - pnl.LocalPosToCanvas(new Point(0, 0)).X);
                                int ydiff = MouseY -
                                            (InputHandler.MousePosition.Y - pnl.LocalPosToCanvas(new Point(0, 0)).Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    dragIcon = new Draggable(pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseX,
                                        pnl.LocalPosToCanvas(new Point(0, 0)).X + MouseY, pnl.Texture);
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
                    FloatRect dragRect = new FloatRect(dragIcon.x - ItemXPadding / 2, dragIcon.y - ItemYPadding / 2,
                        ItemXPadding / 2 + 32, ItemYPadding / 2 + 32);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (_bankWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxBankSlots; i++)
                        {
                            if (_bankWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(_bankWindow.Items[i].RenderBounds(), dragRect).Height;
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