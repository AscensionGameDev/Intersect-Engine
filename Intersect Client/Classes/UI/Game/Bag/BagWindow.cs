using System.Collections.Generic;
using Intersect.Client.Classes.UI.Game.Bag;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.UI.Game
{
    public class BagWindow
    {
        private static int ItemXPadding = 4;

        private static int ItemYPadding = 4;

        //Controls
        private WindowControl _bagWindow;

        private ScrollControl _itemContainer;
        private List<Label> _values = new List<Label>();

        public List<BagItem> Items = new List<BagItem>();

        //Init
        public BagWindow(Canvas _gameCanvas)
        {
            _bagWindow = new WindowControl(_gameCanvas, Strings.Get("bags", "title"), false, "BagWindow");
            _bagWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_bagWindow);

            _itemContainer = new ScrollControl(_bagWindow, "ItemContainer");
            _itemContainer.EnableScroll(false, true);

            Gui.LoadRootUIData(_bagWindow, "InGame.xml");

            InitItemContainer();
        }

        //Location
        //Location
        public int X
        {
            get { return _bagWindow.X; }
        }

        public int Y
        {
            get { return _bagWindow.Y; }
        }

        public void Close()
        {
            _bagWindow.Close();
        }

        public bool IsVisible()
        {
            return !_bagWindow.IsHidden;
        }

        public void Hide()
        {
            _bagWindow.IsHidden = true;
        }

        public void Update()
        {
            if (_bagWindow.IsHidden == true || Globals.Bag == null)
            {
                return;
            }
            for (int i = 0; i < Globals.Bag.Length; i++)
            {
                if (Globals.Bag[i] != null && Globals.Bag[i].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(Globals.Bag[i].ItemNum);
                    if (item != null)
                    {
                        Items[i].pnl.IsHidden = false;

                        if (item.IsStackable())
                        {
                            _values[i].IsHidden = false;
                            _values[i].Text = Globals.Bag[i].ItemVal.ToString();
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
            for (int i = 0; i < Globals.Bag.Length; i++)
            {
                Items.Add(new BagItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer, "BagItemContainer");
                Items[i].Setup();

                _values.Add(new Label(Items[i].container, "BagItemValue"));
                _values[i].Text = "";

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

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _bagWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _bagWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _bagWindow.Width + ItemXPadding,
                Height = _bagWindow.Height + ItemYPadding
            };
            return rect;
        }
    }
}