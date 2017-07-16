using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.ControlInternal;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Trades
{
    public class TradeSegment
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        public ScrollControl _itemContainer;
        public Label GoldValue;
        public List<TradeItem> Items = new List<TradeItem>();

        public int MyIndex;
        private TradingWindow Parent;
        public List<Label> Values = new List<Label>();
        private ImagePanel _itemTemplate;

        public TradeSegment(TradingWindow parent, WindowControl _tradeWindow, int index)
        {
            MyIndex = index;
            Parent = parent;

            var xmlPrefix = "Your";
            if (MyIndex == 1)
            {
                xmlPrefix = "Their";
            }

            _itemContainer = new ScrollControl(_tradeWindow, xmlPrefix + "ItemContainer");
            _itemContainer.EnableScroll(false, true);

            GoldValue = new Label(_tradeWindow, xmlPrefix + "GoldValue")
            {
                Text = Strings.Get("trading", "value", 0)
            };

        }

        public void InitItemContainer(int index)
        {
            var xmlPrefix = "Your";
            if (MyIndex == 1)
            {
                xmlPrefix = "Their";
            }
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new TradeItem(Parent, i, index));
                Items[i].container = new ImagePanel(_itemContainer,xmlPrefix + "TradeContainer");
                Items[i].Setup();

                Values.Add(new Label(Items[i].container, xmlPrefix + "TradeValue"));
                Values[i].Text = "";


                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].container, "InGame.xml");

                var xPadding = Items[i].container.Padding.Left + Items[i].container.Padding.Right;
                var yPadding = Items[i].container.Padding.Top + Items[i].container.Padding.Bottom;
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Width + xPadding) + xPadding,
                    (i / (_itemContainer.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Height + yPadding) + yPadding);
            }
        }
    }
}
