using System;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.UI.Game.Bag
{
    public class BagItem
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;

        //Drag/Drop References
        private BagWindow mBagWindow;

        private Guid mCurrentItemId;
        private ItemDescWindow mDescWindow;

        //Slot info
        private int mMySlot;

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

        public BagItem(BagWindow bagWindow, int index)
        {
            mBagWindow = bagWindow;
            mMySlot = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "BagItemIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += Pnl_DoubleClicked; //Allow withdrawing via double click OR right click
            Pnl.DoubleClicked += Pnl_DoubleClicked;
            Pnl.Clicked += pnl_Clicked;
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.InBag)
            {
                Globals.Me.TryRetreiveBagItem(mMySlot);
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
            if (Globals.Bag[mMySlot] != null)
            {
                mDescWindow = new ItemDescWindow(Globals.Bag[mMySlot].Item, Globals.Bag[mMySlot].Quantity, mBagWindow.X, mBagWindow.Y, Globals.Bag[mMySlot].StatBoost);
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
            if (Globals.Bag[mMySlot].ItemId != mCurrentItemId)
            {
                mCurrentItemId = Globals.Bag[mMySlot].ItemId;
                var item = ItemBase.Get(Globals.Bag[mMySlot].ItemId);
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
                        mCanDrag = true;
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
                    if (mBagWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (int i = 0; i < Globals.Bag.Length; i++)
                        {
                            if (mBagWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(mBagWindow.Items[i].RenderBounds(), dragRect).Height;
                                    bestIntersectIndex = i;
                                }
                            }
                        }
                        if (bestIntersectIndex > -1)
                        {
                            if (mMySlot != bestIntersectIndex)
                            {
                                //Try to swap....
                                PacketSender.SendMoveBagItems(bestIntersectIndex, mMySlot);
                            }
                        }
                    }
                    mDragIcon.Dispose();
                }
            }
        }
    }
}