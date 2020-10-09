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
        private const int MAX_LOOT_ITEMS = 10;

        //Item List
        public List<MapItemIcon> Items = new List<MapItemIcon>();
        private List<Label> mValues = new List<Label>();

        //Controls
        private WindowControl mMapItemWindow;

        private ScrollControl mItemContainer;

        private Button mBtnLootAll;

        //Init
        public MapItemWindow(Canvas gameCanvas)
        {
            mMapItemWindow = new WindowControl(gameCanvas, Strings.MapItemWindow.Title, false, "MapItemWindow");
            mMapItemWindow.DisableResizing();

            mItemContainer = new ScrollControl(mMapItemWindow, "ItemsContainer");
            mItemContainer.EnableScroll(false, true);

            mBtnLootAll = new Button(mMapItemWindow, "LootAllButton");
            mBtnLootAll.Text = Strings.MapItemWindow.LootButton;
            mBtnLootAll.Clicked += MBtnLootAll_Clicked;

            mMapItemWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            mMapItemWindow.IsClosable = false;

            CreateItemContainer();
        }

        //Location
        public int X => mMapItemWindow.X;

        public int Y => mMapItemWindow.Y;

        //Methods
        public void Update()
        {
            var location = new Point(Globals.Me.X, Globals.Me.Y);
            var mapItems = Globals.Me.MapInstance.MapItems;
            if ((!mapItems.ContainsKey(location) || mapItems[location].Count < 1) && !mMapItemWindow.IsHidden)
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

            for (var i = 0; i < MAX_LOOT_ITEMS; i++)
            {
                if (i < mapItems[location].Count)
                {
                    var item = ItemBase.Get(mapItems[location][i].ItemId);
                    if (item != null)
                    {
                        Items[i].MyItem = mapItems[location][i];
                        Items[i].Pnl.IsHidden = false;
                        if (item.IsStackable)
                        {
                            mValues[i].IsHidden = false;
                            mValues[i].Text = Strings.FormatQuantityAbbreviated(mapItems[location][i].Quantity);
                        }
                        else
                        {
                            mValues[i].IsHidden = true;
                        }
                    }
                    else
                    {
                        Items[i].MyItem = null;
                        Items[i].Pnl.IsHidden = true;
                        mValues[i].IsHidden = true;
                    }
                }
                else
                {
                    Items[i].MyItem = null;
                    Items[i].Pnl.IsHidden = true;
                    mValues[i].IsHidden = true;
                }
                
                Items[i].Update();
            }
        }

        private void CreateItemContainer()
        {
            for (var i = 0; i < MAX_LOOT_ITEMS; i++)
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

        public void Show()
        {
            mMapItemWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mMapItemWindow.IsHidden;
        }

        public bool Hide()
        {
            if (!Globals.CanCloseInventory)
            {
                return false;
            }

            mMapItemWindow.IsHidden = true;
            return true;
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect() {
                X = mMapItemWindow.LocalPosToCanvas(new Point(0, 0)).X -
                    (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right) / 2,
                Y = mMapItemWindow.LocalPosToCanvas(new Point(0, 0)).Y -
                    (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom) / 2,
                Width = mMapItemWindow.Width + Items[0].Container.Padding.Left + Items[0].Container.Padding.Right,
                Height = mMapItemWindow.Height + Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom
            };

            return rect;
        }

        private void MBtnLootAll_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var location = new Point(Globals.Me.X, Globals.Me.Y);
            if (Globals.Me.MapInstance.MapItems.ContainsKey(location))
            {
                foreach (var item in Globals.Me.MapInstance.MapItems[location])
                {
                    Globals.Me.TryPickupItem(item.UniqueId);
                }
            }
            
        }

    }

}
