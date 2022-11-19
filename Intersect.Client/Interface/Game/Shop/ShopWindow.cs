using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Shop
{

    public partial class ShopWindow
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public List<ShopItem> Items = new List<ShopItem>();

        private ScrollControl mItemContainer;

        //Controls
        private WindowControl mShopWindow;

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mBuyContextItem;

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

            // Generate our context menu with basic options.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "ShopContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();
            mBuyContextItem = mContextMenu.AddItem(Strings.ShopContextMenu.Buy);
            mBuyContextItem.Clicked += MBuyContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        public void OpenContextMenu(int slot)
        {
            var item = ItemBase.Get(Globals.GameShop.SellingItems[slot].ItemId);

            // No point showing a menu for blank space.
            if (item == null)
            {
                return;
            }

            mBuyContextItem.SetText(Strings.ShopContextMenu.Buy.ToString(item.Name));

            // Set our spell slot as userdata for future reference.
            mContextMenu.UserData = slot;

            mContextMenu.SizeToChildren();
            mContextMenu.Open(Framework.Gwen.Pos.None);
        }

        private void MBuyContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int) sender.Parent.UserData;
            Globals.Me.TryBuyItem(slot);
        }

        //Location
        public int X => mShopWindow.X;

        public int Y => mShopWindow.Y;

        public void Close()
        {
            mContextMenu?.Close();
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
