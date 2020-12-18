using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory
{

    public class MapItemWindow
    {
        //Item List
        public List<MapItemIcon> Items = new List<MapItemIcon>();
        private List<Label> mValues = new List<Label>();

        //Controls
        private ImagePanel mMapItemWindow;

        private Label mMenuHeader;

        private ScrollControl mItemContainer;

        private Button mBtnLootAll;

        //Init
        public MapItemWindow(Canvas gameCanvas)
        {
            mMapItemWindow = new ImagePanel(gameCanvas, "MapItemWindow");
            mMenuHeader = new Label(mMapItemWindow, "Title");
            mMenuHeader.SetText(Strings.MapItemWindow.Title);

            mItemContainer = new ScrollControl(mMapItemWindow, "ItemsContainer");
            mItemContainer.EnableScroll(false, true);

            mBtnLootAll = new Button(mMapItemWindow, "LootAllButton");
            mBtnLootAll.Text = Strings.MapItemWindow.LootButton;
            mBtnLootAll.Clicked += MBtnLootAll_Clicked;

            mMapItemWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            CreateItemContainer();
        }

        //Location
        public int X => mMapItemWindow.X;

        public int Y => mMapItemWindow.Y;

        //Methods
        public void Update()
        {
            // Is this disabled from the server config? If so, skip doing anything.
            if (!Options.Loot.EnableLootWindow)
            {
                mMapItemWindow.Hide();
                return;
            }

            var location = new Point(Globals.Me.X, Globals.Me.Y);
            var mapItems = Globals.Me.MapInstance?.MapItems;
            if (mapItems == null || ((!mapItems.ContainsKey(location) || mapItems[location].Count < 1) && !mMapItemWindow.IsHidden))
            {
                mMapItemWindow.Hide();
            }
            else if (mapItems.ContainsKey(location) && mapItems[location].Count > 0 && mMapItemWindow.IsHidden)
            {
                mMapItemWindow.Show();
            }

            if (mMapItemWindow.IsHidden)
            {
                return;
            }

            var itemSlot = 0;
            foreach(var mapItem in mapItems[location])
            {
                // Skip rendering this item if we're already past the cap we are allowed to display.
                if (itemSlot > Options.Loot.MaximumLootWindowItems - 1)
                {
                    continue;
                }

                // Are we allowed to see and pick this item up?
                if (!mapItem.VisibleToAll && mapItem.Owner != Globals.Me.Id && !Globals.Me.IsInMyParty(mapItem.Owner))
                {
                    // This item does not apply to us!
                    continue;
                }

                var itemBase = ItemBase.Get(mapItem.ItemId);
                if (itemBase != null)
                {
                    Items[itemSlot].MyItem = mapItem;
                    Items[itemSlot].Pnl.IsHidden = false;
                    if (itemBase.IsStackable)
                    {
                        mValues[itemSlot].IsHidden = false;
                        mValues[itemSlot].Text = Strings.FormatQuantityAbbreviated(mapItem.Quantity);
                    }
                    else
                    {
                        mValues[itemSlot].IsHidden = true;
                    }

                    itemSlot++;
                }
                else
                {
                    Items[itemSlot].MyItem = null;
                    Items[itemSlot].Pnl.IsHidden = true;
                    mValues[itemSlot].IsHidden = true;
                } 
            }

            // Update our UI and hide our unused icons.
            for (var slot = 0; slot < Options.Loot.MaximumLootWindowItems; slot++)
            {
                if (slot > itemSlot - 1)
                {
                    Items[slot].MyItem = null;
                    Items[slot].Pnl.IsHidden = true;
                    mValues[slot].IsHidden = true;
                }
                
                Items[slot].Update();
            }
        }

        private void CreateItemContainer()
        {
            for (var i = 0; i < Options.Loot.MaximumLootWindowItems; i++)
            {
                Items.Add(new MapItemIcon(this));
                Items[i].Container = new ImagePanel(mItemContainer, "MapItemIcon");
                Items[i].Setup();

                mValues.Add(new Label(Items[i].Container, "MapItemValue"));
                mValues[i].Text = "";

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

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

        private void MBtnLootAll_Clicked(Base sender, ClickedEventArgs arguments)
        {
            // Try and pick up everything on our location.
            Globals.Me.TryPickupItem(Globals.Me.X, Globals.Me.Y);
            
        }

    }

}
