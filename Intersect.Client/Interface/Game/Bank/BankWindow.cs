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

        private bool mOpen;

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
            Close();
        }

        public void Close()
        {
            mBankWindow.IsHidden = true;
            mOpen = false;
        }

        public void Open()
        {
            // Hide unavailable bank slots
            var currentBankSlots = Math.Max(0, Globals.BankSlots);
            // For any slot beyond the current bank's maximum slots
            for (var i = currentBankSlots; i < Options.Instance.Bank.MaxSlots; i++)
            {
                var bankItem = Items[i];
                var bankLabel = mValues[i];

                bankItem.Container.Hide();
                bankLabel.Hide();

                // Position this invisible BankItem at 0,0 so the scrollbar doesn't think we have more slots than we do
                SetItemPosition(i);
            }

            mBankWindow.IsHidden = false;
            mOpen = true;
        }

        public bool IsVisible()
        {
            return !mBankWindow.IsHidden;
        }

        public void Update()
        {
            if (mBankWindow.IsHidden == true)
            {
                if (mOpen)
                {
                    Interface.GameUi.NotifyCloseBank();
                }

                return;
            }

            X = mBankWindow.X;
            Y = mBankWindow.Y;
            for (var i = 0; i < Math.Min(Globals.BankSlots, Options.Instance.Bank.MaxSlots); i++)
            {
                var bankItem = Items[i];
                var bankLabel = mValues[i];
                var globalBankItem = Globals.Bank[i];

                bankItem.Container.Show();
                SetItemPosition(i);
                if (globalBankItem != null && globalBankItem.ItemId != Guid.Empty)
                {
                    var item = ItemBase.Get(globalBankItem.ItemId);
                    if (item != null)
                    {
                        bankItem.Pnl.IsHidden = false;
                        if (item.IsStackable)
                        {
                            bankLabel.IsHidden = globalBankItem.Quantity <= 1;
                            bankLabel.Text = Strings.FormatQuantityAbbreviated(globalBankItem.Quantity);
                        }
                        else
                        {
                            bankLabel.IsHidden = true;
                        }

                        if (bankItem.IsDragging)
                        {
                            bankItem.Pnl.IsHidden = true;
                            bankLabel.IsHidden = true;
                        }

                        bankItem.Update();
                    }
                }
                else
                {
                    bankItem.Pnl.IsHidden = true;
                    bankLabel.IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (var slotIndex = 0; slotIndex < Options.Instance.Bank.MaxSlots; slotIndex++)
            {
                var bankItem = new BankItem(this, slotIndex);

                bankItem.Container = new ImagePanel(mItemContainer, "BankItem");
                bankItem.Setup();

                var bankLabel = new Label(bankItem.Container, "BankItemValue");
                bankLabel.Text = "";

                bankItem.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
                
                Items.Add(bankItem);
                mValues.Add(bankLabel);
            }
        }

        /// <summary>
        /// Sets the item's position based on whether it's hidden or not
        /// </summary>
        /// <param name="i">Index of the item slot</param>
        private void SetItemPosition(int i)
        {
            var item = Items[i];

            // If the item is hidden, the player doesn't have that slot unlocked - move it to the top so that the scrollbar doesn't lie to the player
            if (item.Container.IsHidden)
            {
                item.Container.SetPosition(0, 0);
                return;
            }

            var xPadding = item.Container.Margin.Left + item.Container.Margin.Right;
            var yPadding = item.Container.Margin.Top + item.Container.Margin.Bottom;

            item.Container.SetPosition(
                i %
                (mItemContainer.Width / (item.Container.Width + xPadding)) *
                (item.Container.Width + xPadding) +
                xPadding,
                i /
                (mItemContainer.Width / (item.Container.Width + xPadding)) *
                (item.Container.Height + yPadding) +
                yPadding
            );
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
