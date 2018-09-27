using System;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.GameObjects;

namespace Intersect.Client.UI.Game.Trades
{
    public class TradeItem
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;
        private Guid mCurrentItemId;
        private ItemDescWindow mDescWindow;
        private int mMySide;

        //Slot info
        private int mMySlot;

        //Drag/Drop References
        private TradingWindow mTradeWindow;

        //Dragging
        private bool mCanDrag;

        private long mClickTime;
        public ImagePanel Container;
        private Draggable mDragIcon;
        public bool IsDragging;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;
        private int mMouseY = -1;
        public ImagePanel Pnl;

        public TradeItem(TradingWindow tradeWindow, int index, int side)
        {
            mTradeWindow = tradeWindow;
            mMySlot = index;
            mMySide = side;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "TradeIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += Pnl_DoubleClicked;  //Revoke with right click or double click
            Pnl.DoubleClicked += Pnl_DoubleClicked;
            Pnl.Clicked += pnl_Clicked;
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.InTrade)
            {
                Globals.Me.TryRevokeItem(mMySlot);
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Globals.System.GetTimeMs() + 500;
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
            if (ItemBase.Get(Globals.Trade[mMySide, mMySlot].ItemId) != null)
            {
                mDescWindow = new ItemDescWindow(Globals.Trade[mMySide, mMySlot].Item,
                    Globals.Trade[mMySide, mMySlot].Quantity, mTradeWindow.X - 255, mTradeWindow.Y,
                    Globals.Trade[mMySide, mMySlot].StatBoost);
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };
            return rect;
        }

        public void Update()
        {
            int n = mMySide;
            if (Globals.Trade[n, mMySlot].ItemId != mCurrentItemId)
            {
                mCurrentItemId = Globals.Trade[n, mMySlot].ItemId;
                var item = ItemBase.Get(Globals.Trade[n, mMySlot].ItemId);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Icon);
                    if (itemTex != null)
                    {
                        Pnl.Texture = itemTex;
                    }
                    else
                    {
                        if (Pnl.Texture != null)
                        {
                            Pnl.Texture = null;
                        }
                    }
                }
                else
                {
                    if (Pnl.Texture != null)
                    {
                        Pnl.Texture = null;
                    }
                }
            }
            if (!IsDragging)
            {
                if (mMouseOver)
                {
                    if (!Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
                    {
                        if (n == 0)
                        {
                            mCanDrag = true;
                        } //Only be able to drag your trade box
                        mMouseX = -1;
                        mMouseY = -1;
                        if (Globals.System.GetTimeMs() < mClickTime)
                        {
                            //Globals.Me.TryUseItem(_mySlot);
                            mClickTime = 0;
                        }
                    }
                    else
                    {
                        if (mCanDrag)
                        {
                            if (mMouseX == -1 || mMouseY == -1)
                            {
                                mMouseX = InputHandler.MousePosition.X -
                                         Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X;
                                mMouseY = InputHandler.MousePosition.Y -
                                         Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y;
                            }
                            else
                            {
                                int xdiff = mMouseX -
                                            (InputHandler.MousePosition.X -
                                             Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0))
                                                 .X);
                                int ydiff = mMouseY -
                                            (InputHandler.MousePosition.Y -
                                             Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0))
                                                 .Y);
                                if (Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)) > 5)
                                {
                                    IsDragging = true;
                                    mDragIcon = new Draggable(
                                        Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X +
                                        mMouseX,
                                        Pnl.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X +
                                        mMouseY, Pnl.Texture);
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
                    FloatRect dragRect = new FloatRect(mDragIcon.X - sItemXPadding / 2, mDragIcon.Y - sItemYPadding / 2,
                        sItemXPadding / 2 + 32, sItemYPadding / 2 + 32);

                    float bestIntersect = 0;
                    int bestIntersectIndex = -1;
                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (mTradeWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Options.MaxInvItems; i++)
                        {
                            if (mTradeWindow.TradeSegment[n].Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (
                                    FloatRect.Intersect(mTradeWindow.TradeSegment[n].Items[i].RenderBounds(), dragRect)
                                        .Width *
                                    FloatRect.Intersect(mTradeWindow.TradeSegment[n].Items[i].RenderBounds(), dragRect)
                                        .Height > bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(mTradeWindow.TradeSegment[n].Items[i].RenderBounds(),
                                            dragRect).Width *
                                        FloatRect.Intersect(mTradeWindow.TradeSegment[n].Items[i].RenderBounds(),
                                            dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (mMySlot != bestIntersectIndex)
                            {
                                //Trade
                                //PacketSender.SendTradeConfirm(bestIntersectIndex);
                            }
                        }
                    }
                    mDragIcon.Dispose();
                }
            }
        }
    }
}