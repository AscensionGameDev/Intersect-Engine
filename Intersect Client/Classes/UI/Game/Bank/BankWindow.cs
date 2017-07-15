using System;
using System.Collections.Generic;
using System.IO;
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
using Intersect_Client.Classes.Networking;
using Color = IntersectClientExtras.GenericClasses.Color;
using Point = IntersectClientExtras.GenericClasses.Point;
using Intersect.Client.Classes.UI.Game.Bank;

namespace Intersect_Client.Classes.UI.Game
{
    public class BankWindow
    {
        private static int ItemXPadding = 4;
        private static int ItemYPadding = 4;
        //Controls
        private WindowControl _bankWindow;
        private ScrollControl _itemContainer;
        private List<Label> _values = new List<Label>();

        public List<BankItem> Items = new List<BankItem>();

        //Location
        public int X;
        public int Y;

        //Init
        public BankWindow(Canvas _gameCanvas)
        {
            _bankWindow = new WindowControl(_gameCanvas, Strings.Get("bank", "title"),false,"BankWindow");
            _bankWindow.DisableResizing();
            Gui.InputBlockingElements.Add(_bankWindow);

            _itemContainer = new ScrollControl(_bankWindow,"ItemContainer");
            _itemContainer.EnableScroll(false, true);

            Gui.LoadRootUIData(_bankWindow, "InGame.xml");
            InitItemContainer();
        }

        public void Close()
        {
            _bankWindow.Close();
        }

        public bool IsVisible()
        {
            return !_bankWindow.IsHidden;
        }

        public void Hide()
        {
            _bankWindow.IsHidden = true;
        }

        public void Update()
        {
            if (_bankWindow.IsHidden == true)
            {
                return;
            }
            X = _bankWindow.X;
            Y = _bankWindow.Y;
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                if (Globals.Bank[i] != null && Globals.Bank[i].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(Globals.Bank[i].ItemNum);
                    if (item != null)
                    {
                        Items[i].pnl.IsHidden = false;
                        if (item.IsStackable())
                        {
                            _values[i].IsHidden = false;
                            _values[i].Text = Globals.Bank[i].ItemVal.ToString();
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
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                Items.Add(new BankItem(this, i));
                Items[i].container = new ImagePanel(_itemContainer,"BankItemContainer");
                Items[i].Setup();

                _values.Add(new Label(_itemContainer));
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

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = _bankWindow.LocalPosToCanvas(new Point(0, 0)).X - ItemXPadding / 2,
                Y = _bankWindow.LocalPosToCanvas(new Point(0, 0)).Y - ItemYPadding / 2,
                Width = _bankWindow.Width + ItemXPadding,
                Height = _bankWindow.Height + ItemYPadding
            };
            return rect;
        }
    }
}