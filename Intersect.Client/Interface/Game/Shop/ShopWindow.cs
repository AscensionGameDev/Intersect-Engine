using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game.Shop
{

    public class ShopWindow
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public List<ShopItem> Items = new List<ShopItem>();

        private ScrollControl mItemContainer;

        //Controls
        private WindowControl mShopWindow;

        //Init
        public ShopWindow(Canvas gameCanvas)
        {
            mShopWindow = new WindowControl(gameCanvas, Globals.GameShop.Name, false, "ShopWindow");
            mShopWindow.DisableResizing();
            Interface.InputBlockingElements.Add(mShopWindow);

            mItemContainer = new ScrollControl(mShopWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            mShopWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            InitItemContainer();
        }

        //Location
        public int X => mShopWindow.X;

        public int Y => mShopWindow.Y;

        public void Close()
        {
            mShopWindow.Close();
        }

        public bool IsVisible()
        {
            return !mShopWindow.IsHidden;
        }

        public void Hide()
        {
            mShopWindow.IsHidden = true;
        }

        private void InitItemContainer()
        {
            for (var i = 0; i < Globals.GameShop.SellingItems.Count; i++)
            {
                Items.Add(new ShopItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "ShopItem");
                Items[i].Setup();

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

                Items[i].LoadItem();

                var xPadding = Items[i].Container.Margin.Left + Items[i].Container.Margin.Right;
                var yPadding = Items[i].Container.Margin.Top + Items[i].Container.Margin.Bottom;
                Items[i]
                    .Container.SetPosition(
                        i %
                        (mItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Width + xPadding) +
                        xPadding,
                        i /
                        (mItemContainer.Width / (Items[i].Container.Width + xPadding)) *
                        (Items[i].Container.Height + yPadding) +
                        yPadding
                    );
            }
        }

    }

}
