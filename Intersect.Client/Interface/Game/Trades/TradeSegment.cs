using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Localization;

namespace Intersect.Client.Interface.Game.Trades
{

    public class TradeSegment
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public Label GoldValue;

        public ScrollControl ItemContainer;

        public List<TradeItem> Items = new List<TradeItem>();

        private ImagePanel mItemTemplate;

        private TradingWindow mParent;

        public int MyIndex;

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
                Text = Strings.Trading.value.ToString(0)
            };
        }

        public void InitItemContainer(int index)
        {
            var prefix = "Your";
            if (MyIndex == 1)
            {
                prefix = "Their";
            }

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new TradeItem(mParent, i, index));
                Items[i].Container = new ImagePanel(ItemContainer, prefix + "TradeItem");
                Items[i].Setup();

                Values.Add(new Label(Items[i].Container, "TradeValue"));
                Values[i].Text = "";

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

                var xPadding = Items[i].Container.Margin.Left + Items[i].Container.Margin.Right;
                var yPadding = Items[i].Container.Margin.Top + Items[i].Container.Margin.Bottom;
                Items[i]
                    .Container.SetPosition(
                        i %
                        (ItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Width + xPadding) +
                        xPadding,
                        i /
                        (ItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Height + yPadding) +
                        yPadding
                    );
            }
        }

    }

}
