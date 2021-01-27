using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Utilities;

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

        private bool mFoundItems;

        private int mScanTimer = 250;   // How often do we scan for items in Milliseconds?

        private long mLastItemScan;

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

            // Are we allowed to scan for items?
            if (mLastItemScan < Timing.Global.Milliseconds)
            {
                // Reset if we've found items
                mFoundItems = false;

                // Find all valid locations near our location and iterate through them to find items we can display.
                var itemSlot = 0;
                foreach (var map in FindSurroundingTiles(Globals.Me.X, Globals.Me.Y, Options.Loot.MaximumLootWindowDistance))
                {
                    var mapItems = map.Key.MapItems;
                    var tiles = map.Value;

                    // iterate through all locations on this map to see if we've got items there.
                    foreach (var tileIndex in tiles)
                    {
                        // When no items have ever been on this location, just skip straight away.
                        if (!mapItems.ContainsKey(tileIndex))
                        {
                            continue;
                        }

                        // Go through each item up to our display limit and add them to our window.
                        foreach (var mapItem in mapItems[tileIndex])
                        {
                            // Skip rendering this item if we're already past the cap we are allowed to display.
                            if (itemSlot > Options.Loot.MaximumLootWindowItems - 1)
                            {
                                continue;
                            }

                            var finalItem = mapItem.Base;
                            if (finalItem != null)
                            {
                                Items[itemSlot].TileIndex = tileIndex;
                                Items[itemSlot].MapId = map.Key.Id;
                                Items[itemSlot].MyItem = mapItem;
                                Items[itemSlot].Pnl.IsHidden = false;
                                if (finalItem.IsStackable)
                                {
                                    mValues[itemSlot].IsHidden = false;
                                    mValues[itemSlot].Text = Strings.FormatQuantityAbbreviated(mapItem.Quantity);
                                }
                                else
                                {
                                    mValues[itemSlot].IsHidden = true;
                                }

                                mFoundItems = true;
                                itemSlot++;
                            }
                            else
                            {
                                Items[itemSlot].TileIndex = -1;
                                Items[itemSlot].MyItem = null;
                                Items[itemSlot].Pnl.IsHidden = true;
                                mValues[itemSlot].IsHidden = true;
                            }
                        }
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

                // Set up our timer
                mLastItemScan = Timing.Global.Milliseconds + mScanTimer;
            }

            // Do we display our window?
            if (!mFoundItems)
            {
                mMapItemWindow.Hide();
            }
            else
            {
                mMapItemWindow.Show();
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
            if (Globals.Me.MapInstance == null)
            {
                return;
            }

            // Try and pick up everything on our location.
            Globals.Me.TryPickupItem(Globals.Me.MapInstance.Id, Globals.Me.Y * Options.MapWidth + Globals.Me.X);
            
        }

        private Dictionary<MapInstance, List<int>> FindSurroundingTiles(int myX, int myY, int distance)
        {
            // Loop through all locations surrounding us to get valid tiles.
            var locations = new Dictionary<MapInstance, List<int>>();
            for (var x = 0 - distance; x <= distance; x++)
            {
                for (var y = 0 - distance; y <= distance; y++)
                {
                    // Use these to keep track of our translation.
                    var currentMap = Globals.Me.MapInstance;
                    var currentX = myX + x;
                    var currentY = myY + y;

                    // Are we on a valid map at all?
                    if (currentMap == null)
                    {
                        break;
                    }

                    // Are we going to the map on our left?
                    if (currentX < 0)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Left != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Left);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentX = (Options.MapWidth + 1) + x;
                        }
                    }

                    // Are we going to the map on our right?
                    if (currentX >= Options.MapWidth)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Right != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Right);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentX = -1 + x;
                        }
                    }

                    // Are we going to the map up from us?
                    if (currentY < 0)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Up != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Up);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentY = (Options.MapHeight + 1) + y;
                        }
                    }

                    // Are we going to the map down from us?
                    if (currentY >= Options.MapHeight)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Down != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Down);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentY = -1 + y;
                        }
                    }

                    if (!locations.ContainsKey(currentMap))
                    {
                        locations.Add(currentMap, new List<int>());
                    }
                    locations[currentMap].Add(currentY * Options.MapWidth + currentX);
                }
            }

            return locations;
        }

    }
}
