using System.Collections.Generic;
using Intersect;
using Intersect.Client.Classes.UI.Game.Bank;
using Intersect.GameObjects;
using Intersect.Localization;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.UI.Game
{
    public class BankWindow
    {
        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        //Controls
        private WindowControl mBankWindow;

        private ScrollControl mItemContainer;
        private List<Label> mValues = new List<Label>();

        public List<BankItem> Items = new List<BankItem>();

        //Location
        public int X;

        public int Y;

        //Init
        public BankWindow(Canvas gameCanvas)
        {
            mBankWindow = new WindowControl(gameCanvas, Strings.Get("bank", "title"), false, "BankWindow");
            mBankWindow.DisableResizing();
            Gui.InputBlockingElements.Add(mBankWindow);

            mItemContainer = new ScrollControl(mBankWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            Gui.LoadRootUiData(mBankWindow, "InGame.xml");
            InitItemContainer();
        }

        public void Close()
        {
            mBankWindow.Close();
        }

        public bool IsVisible()
        {
            return !mBankWindow.IsHidden;
        }

        public void Hide()
        {
            mBankWindow.IsHidden = true;
        }

        public void Update()
        {
            if (mBankWindow.IsHidden == true)
            {
                return;
            }
            X = mBankWindow.X;
            Y = mBankWindow.Y;
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                if (Globals.Bank[i] != null && Globals.Bank[i].ItemNum > -1)
                {
                    var item = ItemBase.Lookup.Get<ItemBase>(Globals.Bank[i].ItemNum);
                    if (item != null)
                    {
                        Items[i].Pnl.IsHidden = false;
                        if (item.IsStackable())
                        {
                            mValues[i].IsHidden = false;
                            mValues[i].Text = Globals.Bank[i].ItemVal.ToString();
                        }
                        else
                        {
                            mValues[i].IsHidden = true;
                        }

                        if (Items[i].IsDragging)
                        {
                            Items[i].Pnl.IsHidden = true;
                            mValues[i].IsHidden = true;
                        }
                        Items[i].Update();
                    }
                }
                else
                {
                    Items[i].Pnl.IsHidden = true;
                    mValues[i].IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                Items.Add(new BankItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "BankItemContainer");
                Items[i].Setup();

                mValues.Add(new Label(mItemContainer));
                mValues[i].Text = "";

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

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = mBankWindow.LocalPosToCanvas(new Point(0, 0)).X - sItemXPadding / 2,
                Y = mBankWindow.LocalPosToCanvas(new Point(0, 0)).Y - sItemYPadding / 2,
                Width = mBankWindow.Width + sItemXPadding,
                Height = mBankWindow.Height + sItemYPadding
            };
            return rect;
        }
    }
}