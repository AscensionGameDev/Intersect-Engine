using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Gwen.ControlInternal;
using IntersectClientExtras.Gwen.Input;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect.Client.Classes.UI.Game.Inventory;

namespace Intersect_Client.Classes.UI.Game
{
    public class InventoryWindow
    {
        //Controls
        private WindowControl _inventoryWindow;
        private ScrollControl _itemContainer;
        private List<Label> _values = new List<Label>();

        //Item List
        public List<InventoryItem> Items = new List<InventoryItem>();

        //Initialized Items?
        private bool _initializedItems = false;

        //Location
        //Location
        public int X
        {
            get { return _inventoryWindow.X; }
        }

        public int Y
        {
            get { return _inventoryWindow.Y; }
        }

        //Init
        public InventoryWindow(Canvas _gameCanvas)
        {
            _inventoryWindow = new WindowControl(_gameCanvas, Strings.Get("inventory", "title"), false, "InventoryWindow");
            _inventoryWindow.DisableResizing();

            _itemContainer = new ScrollControl(_inventoryWindow, "ItemsContainer");
            _itemContainer.EnableScroll(false, true);
        }

        //Methods
        public void Update()
        {
            if (!_initializedItems)
            {
                _initializedItems = true;
                InitItemContainer();
            }
            if (_inventoryWindow.IsHidden == true)
            {
                return;
            }
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                var item = ItemBase.Lookup.Get<ItemBase>(Globals.Me.Inventory[i].ItemNum);
                if (item != null)
                {
                    Items[i].pnl.IsHidden = false;
                    if (item.IsStackable())
                    {
                        _values[i].IsHidden = false;
                        _values[i].Text = Globals.Me.Inventory[i].ItemVal.ToString();
                    }
                    else
                    {
                        _values[i].IsHidden = true;
                    }

                    if (Items[i].IsDragging)
                    {
                        Items[i].pnl.IsHidden = true;
                        _values[i].IsHidden = true;
                    }
                    Items[i].Update();
                }
                else
                {
                    Items[i].pnl.IsHidden = true;
                    _values[i].IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new InventoryItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer,"InventoryItemContainer");
                Items[i].Setup();

                _values.Add(new Label(Items[i].container, "InventoryItemValue"));
                _values[i].Text = "";

                //TODO Made this more efficient.
                Gui.LoadRootUIData(Items[i].container, "InGame.xml");

                var xPadding = Items[i].container.Padding.Left + Items[i].container.Padding.Right;
                var yPadding = Items[i].container.Padding.Top + Items[i].container.Padding.Bottom;
                Items[i].container.SetPosition(
                    (i % (_itemContainer.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Width + xPadding) + xPadding,
                    (i / (_itemContainer.Width / (Items[i].container.Width + xPadding))) * (Items[i].container.Height + yPadding) + yPadding);
            }
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

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _inventoryWindow.LocalPosToCanvas(new Point(0, 0)).X - (Items[0].container.Padding.Left + Items[0].container.Padding.Right) / 2,
                Y = _inventoryWindow.LocalPosToCanvas(new Point(0, 0)).Y - (Items[0].container.Padding.Top + Items[0].container.Padding.Bottom) / 2,
                Width = _inventoryWindow.Width + (Items[0].container.Padding.Left + Items[0].container.Padding.Right),
                Height = _inventoryWindow.Height + (Items[0].container.Padding.Top + Items[0].container.Padding.Bottom)
            };
            return rect;
        }
    }
}