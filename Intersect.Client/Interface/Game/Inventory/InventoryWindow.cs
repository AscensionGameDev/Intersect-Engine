using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game.Inventory
{

    public class InventoryWindow
    {

        //Item List
        public List<InventoryItem> Items = new List<InventoryItem>();

        //Initialized Items?
        private bool mInitializedItems = false;

        //Controls
        private WindowControl mInventoryWindow;

        private ScrollControl mItemContainer;

        private List<Label> mValues = new List<Label>();

        //Init
        public InventoryWindow(Canvas gameCanvas)
        {
            mInventoryWindow = new WindowControl(gameCanvas, Strings.Inventory.title, false, "InventoryWindow");
            mInventoryWindow.DisableResizing();

            mItemContainer = new ScrollControl(mInventoryWindow, "ItemsContainer");
            mItemContainer.EnableScroll(false, true);
            mInventoryWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
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
                        mValues[i].IsHidden = false;
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

        public bool Hide()
        {
            if (!Globals.CanCloseInventory)
            {
                return false;
            }

            mInventoryWindow.IsHidden = true;
            return true;
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
