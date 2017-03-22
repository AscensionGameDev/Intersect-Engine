using System;
using System.Collections.Generic;
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
using Intersect_Library;
using Color = IntersectClientExtras.GenericClasses.Color;

using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect_Library.GameObjects;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.UI.Game
{
    public class TradingWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        private WindowControl _tradeWindow;

        public List<TradeSegment> _tradeSegment = new List<TradeSegment>();

        //Name tags
        private Label _yourOffer;
        private Label _theirOffer;

        //Trade button
        private Button _trade;

        //Location
        public int X;
        public int Y;

        //Trader
        public int EntityID;

        //Init
        public TradingWindow(Canvas _gameCanvas, int entityID)
        {
            EntityID = entityID;

            _tradeWindow = new WindowControl(_gameCanvas, Strings.Get("trading","title", Globals.Entities[EntityID].MyName));
            _tradeWindow.SetSize(434, 402);
            _tradeWindow.SetPosition(GameGraphics.Renderer.GetScreenWidth() / 2 - 200, GameGraphics.Renderer.GetScreenHeight() / 2 - 200);
            _tradeWindow.DisableResizing();
            _tradeWindow.Margin = Margin.Zero;
            _tradeWindow.Padding = new Padding(8, 5, 9, 11);
            X = _tradeWindow.X;
            Y = _tradeWindow.Y;
            Gui.InputBlockingElements.Add(_tradeWindow);

            _tradeWindow.SetTitleBarHeight(24);
            _tradeWindow.SetCloseButtonSize(20, 20);
            _tradeWindow.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "tradeactive.png"), WindowControl.ControlState.Active);
            _tradeWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closenormal.png"), Button.ControlState.Normal);
            _tradeWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closehover.png"), Button.ControlState.Hovered);
            _tradeWindow.SetCloseButtonImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "closeclicked.png"), Button.ControlState.Clicked);
            _tradeWindow.SetFont(Globals.ContentManager.GetFont(Gui.DefaultFont, 14));
            _tradeWindow.SetTextColor(new Color(255, 220, 220, 220), WindowControl.ControlState.Active);

            _yourOffer = new Label(_tradeWindow)
            {
                Text = Strings.Get("trading", "youroffer")
            };
            _yourOffer.SetPosition(4 + ((_tradeWindow.Width - _tradeWindow.Padding.Left - _tradeWindow.Padding.Right) / 4) - (_yourOffer.Width / 2), 4);
            _yourOffer.TextColorOverride = Color.White;

            _theirOffer = new Label(_tradeWindow)
            {
                Text = Strings.Get("trading", "theiroffer")
            };
            _theirOffer.SetPosition(4 + (3 * (_tradeWindow.Width - _tradeWindow.Padding.Left - _tradeWindow.Padding.Right) / 4) - (_yourOffer.Width / 2), 4);
            _theirOffer.TextColorOverride = Color.White;

            _trade = new Button(_tradeWindow);
            _trade.SetSize(100, 42);
            _trade.SetText(Strings.Get("trading", "accept"));
            _trade.SetPosition(_tradeWindow.Width - 120, _tradeWindow.Height - 82);
            _trade.Clicked += trade_Clicked;
            _trade.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonnormal.png"), Button.ControlState.Normal);
            _trade.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonhover.png"), Button.ControlState.Hovered);
            _trade.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "smallbuttonclicked.png"), Button.ControlState.Clicked);
            _trade.SetTextColor(new Color(255, 30, 30, 30), Label.ControlState.Normal);
            _trade.SetTextColor(new Color(255, 20, 20, 20), Label.ControlState.Hovered);
            _trade.SetTextColor(new Color(255, 215, 215, 215), Label.ControlState.Clicked);
            _trade.Font = Globals.ContentManager.GetFont(Gui.DefaultFont, 12);

            for (int i = 0; i < 2; i++)
            {
                _tradeSegment.Add(new TradeSegment(this, _tradeWindow, i));
            }
        }

        public void Close()
        {
            _tradeWindow.Close();
        }
        public bool IsVisible()
        {
            return !_tradeWindow.IsHidden;
        }
        public void Hide()
        {
            _tradeWindow.IsHidden = true;
        }

        public void Update()
        {
            int g = 0;

            if (_tradeWindow.IsHidden == true) { return; }
            X = _tradeWindow.X;
            Y = _tradeWindow.Y;
            for (int n = 0; n < 2; n++)
            {
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Globals.Trade[n, i] != null && Globals.Trade[n, i].ItemNum > -1)
                    {
                        var item = ItemBase.GetItem(Globals.Trade[n, i].ItemNum);
                        g += (item.Price * Globals.Trade[n, i].ItemVal);
                        if (item != null)
                        {
                            _tradeSegment[n].Items[i].pnl.IsHidden = false;
                            if (item.IsStackable())
                            {
                                _tradeSegment[n].Values[i].IsHidden = false;
                                _tradeSegment[n].Values[i].Text = Globals.Trade[n, i].ItemVal.ToString();
                            }
                            else
                            {
                                _tradeSegment[n].Values[i].IsHidden = true;
                            }

                            if (_tradeSegment[n].Items[i].IsDragging)
                            {
                                _tradeSegment[n].Items[i].pnl.IsHidden = true;
                                _tradeSegment[n].Values[i].IsHidden = true;
                            }
                            _tradeSegment[n].Items[i].Update();
                        }
                    }
                    else
                    {
                        _tradeSegment[n].Items[i].pnl.IsHidden = true;
                        _tradeSegment[n].Values[i].IsHidden = true;
                    }
                }
                _tradeSegment[n].GoldValue.Text = Strings.Get("trading", "value",g);
                _tradeSegment[n].GoldValue.SetPosition(4 + (((2 * n) + 1) * (_tradeWindow.Width - _tradeWindow.Padding.Left - _tradeWindow.Padding.Right) / 4) - (_tradeSegment[n].GoldValue.Width / 2), 294);
                g = 0;
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _tradeWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _tradeWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _tradeWindow.Width + ItemXPadding,
                Height = _tradeWindow.Height + ItemYPadding
            };
            return rect;
        }

        //Trade the item
        void trade_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _trade.Text = Strings.Get("trading", "pending");
            PacketSender.SendAcceptTrade();
        }
    }

    public class TradeItem
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        public ImagePanel container;
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
        private int _mySide;
        private int _currentItem = -2;

        //Drag/Drop References
        private TradingWindow _tradeWindow;


        public TradeItem(TradingWindow tradeWindow, int index, int side)
        {
            _tradeWindow = tradeWindow;
            _mySlot = index;
            _mySide = side;
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
            if (Globals.InTrade)
            {
                Globals.Me.TryRevokeItem(_mySlot);
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClickTime = Globals.System.GetTimeMS() + 500;
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            Globals.Me.TryRevokeItem(_mySlot);
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
            if (ItemBase.Get(Globals.Trade[_mySide, _mySlot].ItemNum) != null)
            {
                _descWindow = new ItemDescWindow(Globals.Trade[_mySide, _mySlot].ItemNum, Globals.Trade[_mySide, _mySlot].ItemVal, _tradeWindow.X - 255, _tradeWindow.Y, Globals.Trade[_mySide, _mySlot].StatBoost);
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
            int n = _mySide;
            if (Globals.Trade[n, _mySlot].ItemNum != _currentItem)
            {
                _currentItem = Globals.Trade[n, _mySlot].ItemNum;
                var item = ItemBase.GetItem(Globals.Trade[n, _mySlot].ItemNum);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Pic);
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
                        if (n == 0) { CanDrag = true; } //Only be able to drag your trade box
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
                    if (_tradeWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (_tradeWindow._tradeSegment[n].Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(_tradeWindow._tradeSegment[n].Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_tradeWindow._tradeSegment[n].Items[i].RenderBounds(), dragRect).Height > bestIntersect)
                                {
                                    bestIntersect = FloatRect.Intersect(_tradeWindow._tradeSegment[n].Items[i].RenderBounds(), dragRect).Width * FloatRect.Intersect(_tradeWindow._tradeSegment[n].Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (_mySlot != bestIntersectIndex)
                            {
                                //Trade
                                //PacketSender.SendTradeConfirm(bestIntersectIndex);
                            }
                        }
                    }
                    dragIcon.Dispose();
                }
            }
        }
    }

    public class TradeSegment
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        public ScrollControl _itemContainer;
        public List<TradeItem> Items = new List<TradeItem>();
        public List<Label> Values = new List<Label>();
        public Label GoldValue;

        public int MyIndex;
        private TradingWindow Parent;

        public TradeSegment(TradingWindow parent, WindowControl _tradeWindow, int index)
        {
            MyIndex = index;
            Parent = parent;

            _itemContainer = new ScrollControl(_tradeWindow);
            _itemContainer.SetPosition((_tradeWindow.Width / 2) * index - (8 * index), 20);
            _itemContainer.SetSize((_tradeWindow.Width - _tradeWindow.Padding.Left - _tradeWindow.Padding.Right) / 2, 270);
            _itemContainer.EnableScroll(false, true);
            _itemContainer.AutoHideBars = false;

            var scrollbar = _itemContainer.GetVerticalScrollBar();
            scrollbar.RenderColor = new Color(200, 40, 40, 40);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarnormal.png"), Dragger.ControlState.Normal);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarhover.png"), Dragger.ControlState.Hovered);
            scrollbar.SetScrollBarImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "scrollbarclicked.png"), Dragger.ControlState.Clicked);

            var upButton = scrollbar.GetScrollBarButton(Pos.Top);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrownormal.png"), Button.ControlState.Normal);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowclicked.png"), Button.ControlState.Clicked);
            upButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "uparrowhover.png"), Button.ControlState.Hovered);

            var downButton = scrollbar.GetScrollBarButton(Pos.Bottom);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrownormal.png"), Button.ControlState.Normal);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowclicked.png"), Button.ControlState.Clicked);
            downButton.SetImage(Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "downarrowhover.png"), Button.ControlState.Hovered);

            GoldValue = new Label(_tradeWindow)
            {
                Text = Strings.Get("trading", "value", 0)
            };
            GoldValue.SetPosition(4 + (((2 * index) + 1) * (_tradeWindow.Width - _tradeWindow.Padding.Left - _tradeWindow.Padding.Right) / 4) - (GoldValue.Width / 2), 294);
            GoldValue.TextColorOverride = Color.White;

            InitItemContainer(index);
        }

        private void InitItemContainer(int index)
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new TradeItem(Parent, i, index));
                Items[i].container = new ImagePanel(_itemContainer);
                Items[i].container.SetSize(34, 34);
                Items[i].container.SetPosition((i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding, (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding);
                Items[i].container.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Gui, "tradeitem.png");
                Items[i].Setup();

                Values.Add(new Label(_itemContainer));
                Values[i].Text = "";
                Values[i].SetPosition((i % (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemXPadding) + ItemXPadding + 2, (i / (_itemContainer.Width / (34 + ItemXPadding))) * (34 + ItemYPadding) + ItemYPadding + 20);
                Values[i].TextColorOverride = Color.White;
                Values[i].IsHidden = true;
            }
        }
    }
}
