using System.Collections.Generic;
using Intersect.Client.Classes.Localization;
using IntersectClientExtras.File_Management;
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

            var prefix = "Your";
            if (MyIndex == 1)
            {
                prefix = "Their";
            }

            ItemContainer = new ScrollControl(tradeWindow, prefix + "ItemContainer");
            ItemContainer.EnableScroll(false, true);

            GoldValue = new Label(tradeWindow, prefix + "GoldValue")
            {
                Text = Strings.Trading.value.ToString( 0)
            };
        }

        public void InitItemContainer(int index)
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new TradeItem(Parent, i, index));
                Items[i].Container = new ImagePanel(ItemContainer, "TradeItem");
                Items[i].Setup();

                Values.Add(new Label(Items[i].Container, "TradeValue"));
                Values[i].Text = "";

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame);

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