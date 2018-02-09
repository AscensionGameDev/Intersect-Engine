using System.Collections.Generic;
using Intersect.Client.Classes.UI.Game.Shop;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class ShopWindow
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;

        private ScrollControl mItemContainer;

        //Controls
        private WindowControl mShopWindow;

        public List<ShopWindowItem> Items = new List<ShopWindowItem>();

        //Init
        public ShopWindow(Canvas gameCanvas)
        {
            mShopWindow = new WindowControl(gameCanvas, Globals.GameShop.Name, false, "ShopWindow");
            mShopWindow.DisableResizing();
            Gui.InputBlockingElements.Add(mShopWindow);

            mItemContainer = new ScrollControl(mShopWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            Gui.LoadRootUiData(mShopWindow, "InGame.xml");

            InitItemContainer();
        }

        //Location
        public int X
        {
            get { return mShopWindow.X; }
        }

        public int Y
        {
            get { return mShopWindow.Y; }
        }

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
            for (int i = 0; i < Globals.GameShop.SellingItems.Count; i++)
            {
                Items.Add(new ShopWindowItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "ShopItemContainer");
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUiData(Items[i].Container, "InGame.xml");

                var xPadding = Items[i].Container.Padding.Left + Items[i].Container.Padding.Right;
                var yPadding = Items[i].Container.Padding.Top + Items[i].Container.Padding.Bottom;
                Items[i].Container.SetPosition(
                    (i % (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Width + xPadding) + xPadding,
                    (i / (mItemContainer.Width / (Items[i].Container.Width + xPadding))) *
                    (Items[i].Container.Height + yPadding) + yPadding);
            }
        }
    }
}