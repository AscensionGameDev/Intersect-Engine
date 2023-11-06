using System;

using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Game.DescriptionWindows;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Bank
{

    public partial class BankItem
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public ImagePanel Container;

        public bool IsDragging;

        //Drag/Drop References
        private BankWindow mBankWindow;

        //Dragging
        private bool mCanDrag;

        private long mClickTime;

        private Guid mCurrentItemId;

        private ItemDescriptionWindow mDescWindow;

        private Draggable mDragIcon;

        //Mouse Event Variables
        private bool mMouseOver;

        private int mMouseX = -1;

        private int mMouseY = -1;

        //Slot info
        private int mMySlot;

        public ImagePanel Pnl;

        public BankItem(BankWindow bankWindow, int index)
        {
            mBankWindow = bankWindow;
            mMySlot = index;
        }

        public void Setup()
        {
            Pnl = new ImagePanel(Container, "BankItemIcon");
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += Pnl_RightClicked;
            Pnl.DoubleClicked += Pnl_DoubleClicked;
            Pnl.Clicked += pnl_Clicked;
        }

        private void Pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            if (ClientConfiguration.Instance.EnableContextMenus)
            {
                mBankWindow.OpenContextMenu(mMySlot);
            }
            else
            {
                Pnl_DoubleClicked(sender, arguments);
            }
        }

        private void Pnl_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Globals.InBank)
            {
                if (Globals.InputManager.KeyDown(Keys.Shift))
                {
                    Globals.Me.TryWithdrawItem(
                        mMySlot,
                        skipPrompt: true
                    );
                }
                else
                {
                    var slot = Globals.Bank[mMySlot];
                    Globals.Me.TryWithdrawItem(
                        mMySlot,
                        slot,
                        quantityHint: slot.Quantity,
                        skipPrompt: false
                    );
                }
            }
        }

        void pnl_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mClickTime = Timing.Global.MillisecondsUtc + 500;
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
            if (Globals.InputManager.MouseButtonDown(MouseButtons.Left))
            {
                mCanDrag = false;

                return;
            }

            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }

            if (Globals.Bank[mMySlot]?.Base != null)
            {
                mDescWindow = new ItemDescriptionWindow(
                    Globals.Bank[mMySlot].Base, Globals.Bank[mMySlot].Quantity, mBankWindow.X, mBankWindow.Y,
                    Globals.Bank[mMySlot].ItemProperties
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
            if (Globals.Bank[mMySlot].ItemId != mCurrentItemId)
            {
                mCurrentItemId = Globals.Bank[mMySlot].ItemId;
                var item = ItemBase.Get(Globals.Bank[mMySlot].ItemId);
                if (item != null)
                {
                    var itemTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Item, item.Icon);
                    if (itemTex != null)
                    {
                        Pnl.Texture = itemTex;
                        Pnl.RenderColor = item.Color;
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
                    if (!Globals.InputManager.MouseButtonDown(MouseButtons.Left))
                    {
                        mCanDrag = true;
                        mMouseX = -1;
                        mMouseY = -1;
                        if (Timing.Global.MillisecondsUtc < mClickTime)
                        {
                            //Globals.Me.TryUseItem(_mySlot);
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
                        mDragIcon.X - sItemXPadding / 2, mDragIcon.Y - sItemYPadding / 2, sItemXPadding / 2 + 32,
                        sItemYPadding / 2 + 32
                    );

                    float bestIntersect = 0;
                    var bestIntersectIndex = -1;

                    //So we picked up an item and then dropped it. Lets see where we dropped it to.
                    //Check inventory first.
                    if (mBankWindow.RenderBounds().IntersectsWith(dragRect))
                    {
                        for (var i = 0; i < Globals.BankSlots; i++)
                        {
                            if (mBankWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                            {
                                if (FloatRect.Intersect(mBankWindow.Items[i].RenderBounds(), dragRect).Width *
                                    FloatRect.Intersect(mBankWindow.Items[i].RenderBounds(), dragRect).Height >
                                    bestIntersect)
                                {
                                    bestIntersect =
                                        FloatRect.Intersect(mBankWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(mBankWindow.Items[i].RenderBounds(), dragRect).Height;

                                    bestIntersectIndex = i;
                                }
                            }
                        }

                        if (bestIntersectIndex > -1)
                        {
                            if (mMySlot != bestIntersectIndex)
                            {
                                var allowed = true;

                                //Permission Check
                                if (Globals.GuildBank)
                                {
                                    var rank = Globals.Me.GuildRank;
                                    if (string.IsNullOrWhiteSpace(Globals.Me.Guild) || (!rank.Permissions.BankDeposit && Globals.Me.Rank != 0))
                                    {
                                        ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Guilds.NotAllowedSwap.ToString(Globals.Me.Guild), CustomColors.Alerts.Error, ChatMessageType.Bank));
                                        allowed = false;
                                    }
                                }

                                if (allowed)
                                {
                                    PacketSender.SendMoveBankItems(mMySlot, bestIntersectIndex);
                                }

                                //Globals.Me.SwapItems(bestIntersectIndex, _mySlot);
                            }
                        }
                    }
                    else
                    {
                        var invWindow = Interface.GameUi.GameMenu.GetInventoryWindow();
                        if (invWindow.RenderBounds().IntersectsWith(dragRect))
                        {
                            for (var i = 0; i < Globals.Me.Inventory.Length; i++)
                            {
                                if (invWindow.Items[i].RenderBounds().IntersectsWith(dragRect))
                                {
                                    if (FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Width *
                                        FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Height >
                                        bestIntersect)
                                    {
                                        bestIntersect =
                                            FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Width *
                                            FloatRect.Intersect(invWindow.Items[i].RenderBounds(), dragRect).Height;

                                        bestIntersectIndex = i;
                                    }
                                }
                            }

                            if (bestIntersectIndex > -1)
                            {
                                var slot = Globals.Bank[mMySlot];
                                Globals.Me.TryWithdrawItem(
                                    mMySlot,
                                    inventorySlotIndex: bestIntersectIndex,
                                    quantityHint: slot.Quantity,
                                    skipPrompt: true
                                );
                            }
                        }
                    }

                    mDragIcon.Dispose();
                }
            }
        }

    }

}
