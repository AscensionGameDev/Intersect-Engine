using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Bank
{

    public class BankWindow
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public List<BankItem> Items = new List<BankItem>();

        //Controls
        private WindowControl mBankWindow;

        private ScrollControl mItemContainer;

        private List<Label> mValues = new List<Label>();

        //Location
        public int X;

        public int Y;

        //Init
        public BankWindow(Canvas gameCanvas)
        {
            mBankWindow = new WindowControl(gameCanvas, Globals.GuildBank ? Strings.Guilds.Bank.ToString(Globals.Me?.Guild) : Strings.Bank.title.ToString(), false, "BankWindow");
            mBankWindow.DisableResizing();
            Interface.InputBlockingElements.Add(mBankWindow);

            mItemContainer = new ScrollControl(mBankWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            mBankWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
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
            for (var i = 0; i < Globals.BankSlots; i++)
            {
                if (Globals.Bank[i] != null && Globals.Bank[i].ItemId != Guid.Empty)
                {
                    var item = ItemBase.Get(Globals.Bank[i].ItemId);
                    if (item != null)
                    {
                        Items[i].Pnl.IsHidden = false;
                        if (item.IsStackable)
                        {
                            mValues[i].IsHidden = false;
                            mValues[i].Text = Strings.FormatQuantityAbbreviated(Globals.Bank[i].Quantity);
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
            for (var i = 0; i < Globals.BankSlots; i++)
            {
                Items.Add(new BankItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "BankItem");
                Items[i].Setup();

                mValues.Add(new Label(Items[i].Container, "BankItemValue"));
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

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
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
