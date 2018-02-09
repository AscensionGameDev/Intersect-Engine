using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.UI.Game.Trades;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class TradingWindow
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;
        private Label mTheirOffer;

        //Trade button
        private Button mTrade;

        public List<TradeSegment> TradeSegment = new List<TradeSegment>();

        private WindowControl mTradeWindow;

        //Name tags
        private Label mYourOffer;

        //Trader
        public int EntityId;

        //Init
        public TradingWindow(Canvas gameCanvas, int entityId)
        {
            EntityId = entityId;

            mTradeWindow = new WindowControl(gameCanvas,
                Strings.Get("trading", "title", Globals.Entities[EntityId].MyName), false, "TradeWindow");
            mTradeWindow.DisableResizing();
            Gui.InputBlockingElements.Add(mTradeWindow);

            mYourOffer = new Label(mTradeWindow, "YourOfferLabel")
            {
                Text = Strings.Get("trading", "youroffer")
            };

            mTheirOffer = new Label(mTradeWindow, "TheirOfferLabel")
            {
                Text = Strings.Get("trading", "theiroffer")
            };

            mTrade = new Button(mTradeWindow, "TradeButton");
            mTrade.SetText(Strings.Get("trading", "accept"));
            mTrade.Clicked += trade_Clicked;

            for (int i = 0; i < 2; i++)
            {
                TradeSegment.Add(new TradeSegment(this, mTradeWindow, i));
            }
            Gui.LoadRootUiData(mTradeWindow, "InGame.xml");

            for (int i = 0; i < 2; i++)
            {
                TradeSegment[i].InitItemContainer(i);
            }
        }

        //Location
        public int X
        {
            get { return mTradeWindow.X; }
        }

        public int Y
        {
            get { return mTradeWindow.Y; }
        }

        public void Close()
        {
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
            int g = 0;

            if (mTradeWindow.IsHidden == true)
            {
                return;
            }
            for (int n = 0; n < 2; n++)
            {
                for (int i = 0; i < Options.MaxInvItems; i++)
                {
                    if (Globals.Trade[n, i] != null && Globals.Trade[n, i].ItemNum > -1)
                    {
                        var item = ItemBase.Lookup.Get<ItemBase>(Globals.Trade[n, i].ItemNum);
                        if (item != null)
                        {
                            g += (item.Price * Globals.Trade[n, i].ItemVal);
                            TradeSegment[n].Items[i].Pnl.IsHidden = false;
                            if (item.IsStackable())
                            {
                                TradeSegment[n].Values[i].IsHidden = false;
                                TradeSegment[n].Values[i].Text = Globals.Trade[n, i].ItemVal.ToString();
                            }
                            else
                            {
                                TradeSegment[n].Values[i].IsHidden = true;
                            }

                            if (TradeSegment[n].Items[i].IsDragging)
                            {
                                TradeSegment[n].Items[i].Pnl.IsHidden = true;
                                TradeSegment[n].Values[i].IsHidden = true;
                            }
                            TradeSegment[n].Items[i].Update();
                        }
                    }
                    else
                    {
                        TradeSegment[n].Items[i].Pnl.IsHidden = true;
                        TradeSegment[n].Values[i].IsHidden = true;
                    }
                }
                TradeSegment[n].GoldValue.Text = Strings.Get("trading", "value", g);
                TradeSegment[n].GoldValue.SetPosition(
                    4 +
                    (((2 * n) + 1) * (mTradeWindow.Width - mTradeWindow.Padding.Left -
                                      mTradeWindow.Padding.Right) / 4) -
                    (TradeSegment[n].GoldValue.Width / 2), 294);
                g = 0;
            }
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
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
            mTrade.Text = Strings.Get("trading", "pending");
            PacketSender.SendAcceptTrade();
        }
    }
}