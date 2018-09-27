using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.UI.Game.Bank
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
            mBankWindow = new WindowControl(gameCanvas, Strings.Bank.title, false, "BankWindow");
            mBankWindow.DisableResizing();
            Gui.InputBlockingElements.Add(mBankWindow);

            mItemContainer = new ScrollControl(mBankWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            mBankWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
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
                if (Globals.Bank[i] != null && Globals.Bank[i].ItemId != Guid.Empty)
                {
                    var item = ItemBase.Get(Globals.Bank[i].ItemId);
                    if (item != null)
                    {
                        Items[i].Pnl.IsHidden = false;
                        if (item.IsStackable())
                        {
                            mValues[i].IsHidden = false;
                            mValues[i].Text = Globals.Bank[i].Quantity.ToString();
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
                Items[i].Container = new ImagePanel(mItemContainer, "BankItem");
                Items[i].Setup();

                mValues.Add(new Label(mItemContainer));
                mValues[i].Text = "";

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());

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
                X = mBankWindow.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).X - sItemXPadding / 2,
                Y = mBankWindow.LocalPosToCanvas(new Framework.GenericClasses.Point(0, 0)).Y - sItemYPadding / 2,
                Width = mBankWindow.Width + sItemXPadding,
                Height = mBankWindow.Height + sItemYPadding
            };
            return rect;
        }
    }
}