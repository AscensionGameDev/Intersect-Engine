using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Bank
{

    public partial class BankWindow
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

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mWithdrawContextItem;

        //Init
        public BankWindow(Canvas gameCanvas)
        {
            // Create a new window to display the contents of the bank.
            mBankWindow = new WindowControl(gameCanvas,
                Globals.GuildBank
                    ? Strings.Guilds.Bank.ToString(Globals.Me?.Guild)
                    : Strings.Bank.title.ToString(),
                false, "BankWindow");

            // Disable resizing and add to the list of input-blocking elements.
            mBankWindow.DisableResizing();
            Interface.InputBlockingElements.Add(mBankWindow);

            // Create a new scroll control for the items in the bank.
            mItemContainer = new ScrollControl(mBankWindow, "ItemContainer");
            mItemContainer.EnableScroll(false, true);

            // Initialize the bank window and item container.
            mBankWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            InitItemContainer();

            // Create a context menu for the bank items.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "BankContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;

            // Clear the children of the context menu and add a "Withdraw" option.
            mContextMenu.Children.Clear();
            mWithdrawContextItem = mContextMenu.AddItem(Strings.BankContextMenu.Withdraw);
            mWithdrawContextItem.Clicked += MWithdrawContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            // Close the window.
            Close();
        }

        public void OpenContextMenu(int slot)
        {
            var item = ItemBase.Get(Globals.Bank[slot].ItemId);

            // No point showing a menu for blank space.
            if (item == null)
            {
                return;
            }

            mWithdrawContextItem.SetText(Strings.BankContextMenu.Withdraw.ToString(item.Name));

            // Set our spell slot as userdata for future reference.
            mContextMenu.UserData = slot;

            mContextMenu.SizeToChildren();
            mContextMenu.Open(Framework.Gwen.Pos.None);
        }

        private void MWithdrawContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int)sender.Parent.UserData;
            Globals.Me.TryWithdrawItem(slot);
        }

        public void Close()
        {
            mBankWindow.IsHidden = true;
            mOpen = false;
            mContextMenu?.Close();
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
            if (mBankWindow.IsHidden)
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
