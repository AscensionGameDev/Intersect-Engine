using Gwen;
using Gwen.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.UI.Game
{
    public class InventoryWindow : IGUIElement
    {
        //Controls
        private WindowControl _inventoryWindow;
        private ScrollControl _itemContainer;

        //Item List
        private int ItemXPadding = 4;
        private int ItemYPadding = 4;
        private List<ImagePanel> _items = new List<ImagePanel>();

        //Init
        public InventoryWindow(Canvas _gameCanvas)
        {
            _inventoryWindow = new WindowControl(_gameCanvas, "Inventory");
            _inventoryWindow.SetSize(200, 300);
            _inventoryWindow.SetPosition(Graphics.ScreenWidth -210, Graphics.ScreenHeight - 500);
            _inventoryWindow.DisableResizing();
            _inventoryWindow.Margin = Margin.Zero;
            _inventoryWindow.Padding = Padding.Zero;
            _inventoryWindow.IsHidden = true;

            _itemContainer = new ScrollControl(_inventoryWindow);
            _itemContainer.SetPosition(0, 0);
            _itemContainer.SetSize(_inventoryWindow.Width, _inventoryWindow.Height - 24);
            _itemContainer.EnableScroll(false, true);
            InitItemContainer();
        }

        //Methods
        public void Update()
        {

        }
        private void InitItemContainer()
        {
            int x = ItemXPadding;
            int y = ItemYPadding;

            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                _items.Add(new ImagePanel(_itemContainer) { ImageName = "Resources/Items/1.png" });
                _items[i].SetSize(32, 32);
                _items[i].SetPosition(x, y);
                _items[i].Clicked += InventoryWindow_Clicked;
                x += 32 + ItemXPadding;
                if (x + 32 + 4 > _itemContainer.Width)
                {
                    x = ItemXPadding;
                    y = y + 32 + ItemYPadding;
                }
            }
        }

        void InventoryWindow_Clicked(Base sender, ClickedEventArgs arguments)
        {
            
        }
        public void Show()
        {
            _inventoryWindow.IsHidden = false;
        }
        public bool IsVisible()
        {
            return !_inventoryWindow.IsHidden;
        }
        public void Hide()
        {
            _inventoryWindow.IsHidden = true;
        }
    }
}
