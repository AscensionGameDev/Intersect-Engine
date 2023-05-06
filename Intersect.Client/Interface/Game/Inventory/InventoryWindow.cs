using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory
{

    public partial class InventoryWindow
    {

        //Item List
        public List<InventoryItem> Items = new List<InventoryItem>();

        //Initialized Items?
        private bool mInitializedItems = false;

        //Controls
        private WindowControl mInventoryWindow;

        private ScrollControl mItemContainer;

        private List<Label> mValues = new List<Label>();

        // Context menu
        private Framework.Gwen.Control.Menu mContextMenu;

        private MenuItem mUseItemContextItem;

        private MenuItem mActionItemContextItem;

        private MenuItem mDropItemContextItem;

        //Init
        public InventoryWindow(Canvas gameCanvas)
        {
            mInventoryWindow = new WindowControl(gameCanvas, Strings.Inventory.Title, false, "InventoryWindow");
            mInventoryWindow.DisableResizing();

            mItemContainer = new ScrollControl(mInventoryWindow, "ItemsContainer");
            mItemContainer.EnableScroll(false, true);
            mInventoryWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            // Generate our context menu with basic options.
            mContextMenu = new Framework.Gwen.Control.Menu(gameCanvas, "InventoryContextMenu");
            mContextMenu.IsHidden = true;
            mContextMenu.IconMarginDisabled = true;
            //TODO: Is this a memory leak?
            mContextMenu.Children.Clear();
            mUseItemContextItem = mContextMenu.AddItem(Strings.ItemContextMenu.Use);
            mUseItemContextItem.Clicked += MUseItemContextItem_Clicked;
            mDropItemContextItem = mContextMenu.AddItem(Strings.ItemContextMenu.Drop);
            mDropItemContextItem.Clicked += MDropItemContextItem_Clicked;
            mActionItemContextItem = mContextMenu.AddItem(Strings.ItemContextMenu.Bank);
            mActionItemContextItem.Clicked += MActionItemContextItem_Clicked;
            mContextMenu.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        }

        public void OpenContextMenu(int slot)
        {
            // Clear out the old options.
            mContextMenu.RemoveChild(mUseItemContextItem, false);
            mContextMenu.RemoveChild(mActionItemContextItem, false);
            mContextMenu.RemoveChild(mDropItemContextItem, false);
            mContextMenu.Children.Clear();

            var item = ItemBase.Get(Globals.Me.Inventory[slot].ItemId);

            // No point showing a menu for blank space.
            if (item == null)
            {
                return;
            }

            // Add our use Item prompt, assuming we have a valid usecase.
            switch (item.ItemType)
            {
                case Enums.ItemType.Spell:
                    mContextMenu.AddChild(mUseItemContextItem);
                    mUseItemContextItem.SetText(item.QuickCast
                        ? Strings.ItemContextMenu.Cast.ToString(item.Name)
                        : Strings.ItemContextMenu.Learn.ToString(item.Name));
                    break;

                case Enums.ItemType.Event:
                case Enums.ItemType.Consumable:
                    mContextMenu.AddChild(mUseItemContextItem);
                    mUseItemContextItem.SetText(Strings.ItemContextMenu.Use.ToString(item.Name));
                    break;

                case Enums.ItemType.Bag:
                    mContextMenu.AddChild(mUseItemContextItem);
                    mUseItemContextItem.SetText(Strings.ItemContextMenu.Open.ToString(item.Name));
                    break;

                case Enums.ItemType.Equipment:
                    mContextMenu.AddChild(mUseItemContextItem);
                    // Show the correct equip/unequip prompts.
                    if (Globals.Me.MyEquipment.Contains(slot))
                    {
                        mUseItemContextItem.SetText(Strings.ItemContextMenu.Unequip.ToString(item.Name));
                    }
                    else
                    {
                        mUseItemContextItem.SetText(Strings.ItemContextMenu.Equip.ToString(item.Name));
                    }
                    
                    break;
            }

            // Set up the correct contextual additional action.
            if (Globals.InBag && item.CanBag)
            {
                mContextMenu.AddChild(mActionItemContextItem);
                mActionItemContextItem.SetText(Strings.ItemContextMenu.Bag.ToString(item.Name));
            }
            else if (Globals.InBank && (item.CanBank || item.CanGuildBank))
            {
                mContextMenu.AddChild(mActionItemContextItem);
                mActionItemContextItem.SetText(Strings.ItemContextMenu.Bank.ToString(item.Name));
            }
            else if (Globals.InTrade && item.CanTrade)
            {
                mContextMenu.AddChild(mActionItemContextItem);
                mActionItemContextItem.SetText(Strings.ItemContextMenu.Trade.ToString(item.Name));
            }
            else if (Globals.GameShop != null && item.CanSell)
            {
                mContextMenu.AddChild(mActionItemContextItem);
                mActionItemContextItem.SetText(Strings.ItemContextMenu.Sell.ToString(item.Name));
            }

            // Can we drop this item? if so show the user!
            if (item.CanDrop)
            {
                mContextMenu.AddChild(mDropItemContextItem);
                mDropItemContextItem.SetText(Strings.ItemContextMenu.Drop.ToString(item.Name));
            }

            // Set our Inventory slot as userdata for future reference.
            mContextMenu.UserData = slot;

            // Display our menu.. If we have anything to display.
            if (mContextMenu.Children.Count > 0)
            {
                mContextMenu.SizeToChildren();
                mContextMenu.Open(Framework.Gwen.Pos.None);
            }
        }

        private void MUseItemContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int)sender.Parent.UserData;
            Globals.Me.TryUseItem(slot);
        }

        private void MActionItemContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int)sender.Parent.UserData;
            if (Globals.GameShop != null)
            {
                Globals.Me.TrySellItem(slot);
            }
            else if (Globals.InBank)
            {
                Globals.Me.TryDepositItem(slot);
            }
            else if (Globals.InBag)
            {
                Globals.Me.TryStoreBagItem(slot, -1);
            }
            else if (Globals.InTrade)
            {
                Globals.Me.TryTradeItem(slot);
            }
        }

        private void MDropItemContextItem_Clicked(Base sender, Framework.Gwen.Control.EventArguments.ClickedEventArgs arguments)
        {
            var slot = (int) sender.Parent.UserData;
            Globals.Me.TryDropItem(slot);
        }

        //Location
        //Location
        public int X => mInventoryWindow.X;

        public int Y => mInventoryWindow.Y;

        //Methods
        public void Update()
        {
            if (!mInitializedItems)
            {
                mInitializedItems = true;
                InitItemContainer();
            }

            if (mInventoryWindow.IsHidden)
            {
                return;
            }

            mInventoryWindow.IsClosable = Globals.CanCloseInventory;

            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                var item = ItemBase.Get(Globals.Me.Inventory[i].ItemId);
                if (item != null)
                {
                    Items[i].Pnl.IsHidden = false;
                    if (item.IsStackable)
                    {
                        mValues[i].IsHidden = Globals.Me.Inventory[i].Quantity <= 1;
                        mValues[i].Text = Strings.FormatQuantityAbbreviated(Globals.Me.Inventory[i].Quantity);
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
                else
                {
                    Items[i].Pnl.IsHidden = true;
                    mValues[i].IsHidden = true;
                }
            }
        }

        private void InitItemContainer()
        {
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                Items.Add(new InventoryItem(this, i));
                Items[i].Container = new ImagePanel(mItemContainer, "InventoryItem");
                Items[i].Setup();

                mValues.Add(new Label(Items[i].Container, "InventoryItemValue"));
                mValues[i].Text = "";

                Items[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

                if (Items[i].EquipPanel.Texture == null)
                {
                    Items[i].EquipPanel.Texture = Graphics.Renderer.GetWhiteTexture();
                }

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
            mInventoryWindow.IsHidden = false;
        }

        public bool IsVisible()
        {
            return !mInventoryWindow.IsHidden;
        }

        public void Hide()
        {
            if (!Globals.CanCloseInventory)
            {
                return;
            }

            mContextMenu?.Close();
            mInventoryWindow.IsHidden = true;
        }

        public FloatRect RenderBounds()
        {
            var rect = new FloatRect()
            {
                X = mInventoryWindow.LocalPosToCanvas(new Point(0, 0)).X -
                    (Items[0].Container.Padding.Left + Items[0].Container.Padding.Right) / 2,
                Y = mInventoryWindow.LocalPosToCanvas(new Point(0, 0)).Y -
                    (Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom) / 2,
                Width = mInventoryWindow.Width + Items[0].Container.Padding.Left + Items[0].Container.Padding.Right,
                Height = mInventoryWindow.Height + Items[0].Container.Padding.Top + Items[0].Container.Padding.Bottom
            };

            return rect;
        }

    }

}
