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
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        private Label _theirOffer;

        //Trade button
        private Button _trade;

        public List<TradeSegment> _tradeSegment = new List<TradeSegment>();

        private WindowControl _tradeWindow;

        //Name tags
        private Label _yourOffer;

        //Trader
        public int EntityID;

        //Init
        public TradingWindow(Canvas _gameCanvas, int entityID)
        {
            EntityID = entityID;

            _tradeWindow = new WindowControl(_gameCanvas,
                Strings.Get("trading", "title", Globals.Entities[EntityID].MyName), false, "TradeWindow");
            _tradeWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_tradeWindow);

            _yourOffer = new Label(_tradeWindow, "YourOfferLabel")
            {
                Text = Strings.Get("trading", "youroffer")
            };

            _theirOffer = new Label(_tradeWindow, "TheirOfferLabel")
            {
                Text = Strings.Get("trading", "theiroffer")
            };

            _trade = new Button(_tradeWindow, "TradeButton");
            _trade.SetText(Strings.Get("trading", "accept"));
            _trade.Clicked += trade_Clicked;

            for (int i = 0; i < 2; i++)
            {
                _tradeSegment.Add(new TradeSegment(this, _tradeWindow, i));
            }
            Gui.LoadRootUIData(_tradeWindow, "InGame.xml");

            for (int i = 0; i < 2; i++)
            {
                _tradeSegment[i].InitItemContainer(i);
            }
        }

        //Location
        public int X
        {
            get { return _tradeWindow.X; }
        }

        public int Y
        {
            get { return _tradeWindow.Y; }
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

            if (_tradeWindow.IsHidden == true)
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
                _tradeSegment[n].GoldValue.Text = Strings.Get("trading", "value", g);
                _tradeSegment[n].GoldValue.SetPosition(
                    4 +
                    (((2 * n) + 1) * (_tradeWindow.Width - _tradeWindow.Padding.Left -
                                      _tradeWindow.Padding.Right) / 4) -
                    (_tradeSegment[n].GoldValue.Width / 2), 294);
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
}