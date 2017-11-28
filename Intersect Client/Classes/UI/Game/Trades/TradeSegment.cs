using System.Collections.Generic;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.UI;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Trades
{
    public class TradeSegment
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;

        public ScrollControl ItemContainer;
        private ImagePanel mItemTemplate;
        public Label GoldValue;
        public List<TradeItem> Items = new List<TradeItem>();

        public int MyIndex;
        private TradingWindow mParent;
        public List<Label> Values = new List<Label>();

        public TradeSegment(TradingWindow parent, WindowControl tradeWindow, int index)
        {
            MyIndex = index;
            mParent = parent;

            var xmlPrefix = "Your";
            if (MyIndex == 1)
            {
                xmlPrefix = "Their";
            }

            ItemContainer = new ScrollControl(tradeWindow, xmlPrefix + "ItemContainer");
            ItemContainer.EnableScroll(false, true);

            GoldValue = new Label(tradeWindow, xmlPrefix + "GoldValue")
            {
                Text = Strings.Trading.value.ToString( 0)
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
                Items.Add(new TradeItem(mParent, i, index));
                Items[i].Container = new ImagePanel(ItemContainer, xmlPrefix + "TradeContainer");
                Items[i].Setup();

                Values.Add(new Label(Items[i].Container, xmlPrefix + "TradeValue"));
                Values[i].Text = "";

                //TODO Made this more efficient.
                Gui.LoadRootUiData(Items[i].Container, "InGame.xml");

                var xPadding = Items[i].Container.Padding.Left + Items[i].Container.Padding.Right;
                var yPadding = Items[i].Container.Padding.Top + Items[i].Container.Padding.Bottom;
                Items[i].Container.SetPosition(
                    (i % (ItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Width + xPadding) + xPadding,
                    (i / (ItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Height + yPadding) + yPadding);
            }
        }
    }
}