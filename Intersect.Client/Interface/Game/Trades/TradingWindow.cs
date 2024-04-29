﻿using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Trades
{

    public partial class TradingWindow
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        private Label mTheirOffer;

        //Trade button
        private Button mTrade;

        private WindowControl mTradeWindow;

        //Name tags
        private Label mYourOffer;

        public List<TradeSegment> TradeSegment = new List<TradeSegment>();

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mWithdrawContextItem;

        //Init
        public TradingWindow(Canvas gameCanvas, string traderName)
        {
            mTradeWindow = new WindowControl(
                gameCanvas, Strings.Trading.title.ToString(traderName), false, "TradeWindow"
            );

            mTradeWindow.DisableResizing();
            Interface.InputBlockingElements.Add(mTradeWindow);

            mYourOffer = new Label(mTradeWindow, "YourOfferLabel")
            {
                Text = Strings.Trading.youroffer
            };

            mTheirOffer = new Label(mTradeWindow, "TheirOfferLabel")
            {
                Text = Strings.Trading.theiroffer
            };

            mTrade = new Button(mTradeWindow, "TradeButton");
            mTrade.SetText(Strings.Trading.accept);
            mTrade.Clicked += trade_Clicked;

            for (var i = 0; i < 2; i++)
            {
                TradeSegment.Add(new TradeSegment(this, mTradeWindow, i));
            }

            mTradeWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            for (var i = 0; i < 2; i++)
            {
                TradeSegment[i].InitItemContainer(i);
            }

            // Generate our context menu with basic options.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "TradeContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();
            mWithdrawContextItem = mContextMenu.AddItem(Strings.TradeContextMenu.Withdraw);
            mWithdrawContextItem.Clicked += MWithdrawContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        private void MWithdrawContextItem_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var slot = (int) sender.Parent.UserData;
            Globals.Me.TryRevokeItem(slot);
        }

        public void OpenContextMenu(int side, int slot)
        {
            var item = ItemBase.Get(Globals.Trade[side, slot].ItemId);

            // No point showing a menu for blank space.
            if (item == null)
            {
                return;
            }

            mWithdrawContextItem.SetText(Strings.TradeContextMenu.Withdraw.ToString(item.Name));

            // Set our spell slot as userdata for future reference.
            mContextMenu.UserData = slot ;

            mContextMenu.IsHidden = false;
            mContextMenu.SetSize(mContextMenu.Width, mContextMenu.Height);
            mContextMenu.SizeToChildren();
            mContextMenu.Open(Framework.Gwen.Pos.None);
            mContextMenu.MoveTo(mContextMenu.X, mContextMenu.Y);
        }

        //Location
        public int X => mTradeWindow.X;

        public int Y => mTradeWindow.Y;

        public void Close()
        {
            mContextMenu?.Close();
            mTradeWindow.Close();
        }

        public bool IsVisible()
        {
            return !mTradeWindow.IsHidden;
        }

        public void Hide()
        {
            mTradeWindow.IsHidden = true;
        }

        public void Update()
        {
            var g = 0;

            if (mTradeWindow.IsHidden == true)
            {
                return;
            }

            for (var n = 0; n < 2; n++)
            {
                for (var i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Globals.Trade[n, i] != null && Globals.Trade[n, i].ItemId != Guid.Empty)
                    {
                        var item = ItemBase.Get(Globals.Trade[n, i].ItemId);
                        if (item == null)
                        {
                            continue;
                        }

                        g += item.Price * Globals.Trade[n, i].Quantity;
                        TradeSegment[n].Items[i].Pnl.IsHidden = false;
                        if (item.IsStackable)
                        {
                            TradeSegment[n].Values[i].IsHidden = Globals.Trade[n, i].Quantity <= 1;
                            TradeSegment[n].Values[i].Text =
                                Strings.FormatQuantityAbbreviated(Globals.Trade[n, i].Quantity);
                        }
                        else
                        {
                            TradeSegment[n].Values[i].IsHidden = true;
                        }

                        TradeSegment[n].Items[i].Update();
                    }
                    else
                    {
                        TradeSegment[n].Items[i].Pnl.IsHidden = true;
                        TradeSegment[n].Values[i].IsHidden = true;
                    }
                }

                TradeSegment[n].GoldValue.Text = Strings.Trading.value.ToString(g);
                g = 0;
            }
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = mTradeWindow.LocalPosToCanvas(new Point(0, 0)).X - sItemXPadding / 2,
                Y = mTradeWindow.LocalPosToCanvas(new Point(0, 0)).Y - sItemYPadding / 2,
                Width = mTradeWindow.Width + sItemXPadding,
                Height = mTradeWindow.Height + sItemYPadding
            };

            return rect;
        }

        //Trade the item
        void trade_Clicked(Base sender, ClickedEventArgs arguments)
        {
            mTrade.Text = Strings.Trading.pending;
            mTrade.IsDisabled = true;
            PacketSender.SendAcceptTrade();
        }

    }

}
