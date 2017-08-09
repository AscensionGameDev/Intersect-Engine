using System.Collections.Generic;
using Intersect.Client.Classes.UI.Game.Shop;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class ShopWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;

        private ScrollControl _itemContainer;

        //Controls
        private WindowControl _shopWindow;

        public List<ShopWindowItem> Items = new List<ShopWindowItem>();

        //Init
        public ShopWindow(Canvas _gameCanvas)
        {
            _shopWindow = new WindowControl(_gameCanvas, Globals.GameShop.Name, false, "ShopWindow");
            _shopWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_shopWindow);

            _itemContainer = new ScrollControl(_shopWindow, "ItemContainer");
            _itemContainer.EnableScroll(false, true);

            Gui.LoadRootUIData(_shopWindow, "InGame.xml");

            InitItemContainer();
        }

        //Location
        public int X
        {
            get { return _shopWindow.X; }
        }

        public int Y
        {
            get { return _shopWindow.Y; }
        }

        public void Close()
        {
            _shopWindow.Close();
        }

        public bool IsVisible()
        {
            return !_shopWindow.IsHidden;
        }

        public void Hide()
        {
            _shopWindow.IsHidden = true;
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Globals.GameShop.SellingItems.Count; i++)
            {
                Items.Add(new ShopWindowItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer, "ShopItemContainer");
                Items[i].Setup();

                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].container, "InGame.xml");

                var xPadding = Items[i].container.Padding.Left + Items[i].container.Padding.Right;
                var yPadding = Items[i].container.Padding.Top + Items[i].container.Padding.Bottom;
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (Items[i].container.Width + xPadding))) *
                    (Items[i].container.Width + xPadding) + xPadding,
                    (i / (_itemContainer.Width / (Items[i].container.Width + xPadding))) *
                    (Items[i].container.Height + yPadding) + yPadding);
            }
        }
    }
}