﻿using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;

namespace Intersect.Client.Interface.Game.Crafting
{

    public class CraftingWindow
    {

        private static int sItemXPadding = 4;

        private static int sItemYPadding = 4;

        public bool Crafting;

        private ImagePanel mBar;

        private ImagePanel mBarContainer;

        private long mBarTimer;

        private RecipeItem mCombinedItem;

        private Label mCombinedValue;

        private Button mCraft;

        private ImagePanel mCraftedItemTemplate;

        private Guid mCraftId;

        //Controls
        private WindowControl mCraftWindow;

        private bool mInitialized = false;

        private ScrollControl mItemContainer;

        private List<RecipeItem> mItems = new List<RecipeItem>();

        private ImagePanel mItemTemplate;

        private Label mLblIngredients;

        private Label mLblProduct;

        private Label mLblRecipes;

        //Objects
        private ListBox mRecipes;

        private List<Label> mValues = new List<Label>();

        public CraftingWindow(Canvas gameCanvas)
        {
            mCraftWindow = new WindowControl(gameCanvas, Globals.ActiveCraftingTable.Name, false, "CraftingWindow");
            mCraftWindow.DisableResizing();

            mItemContainer = new ScrollControl(mCraftWindow, "IngredientsContainer");

            //Labels
            mLblRecipes = new Label(mCraftWindow, "RecipesTitle");
            mLblRecipes.Text = Strings.Crafting.recipes;

            mLblIngredients = new Label(mCraftWindow, "IngredientsTitle");
            mLblIngredients.Text = Strings.Crafting.ingredients;

            mLblProduct = new Label(mCraftWindow, "ProductLabel");
            mLblProduct.Text = Strings.Crafting.product;

            //Recepie list
            mRecipes = new ListBox(mCraftWindow, "RecipesList");

            //Progress Bar
            mBarContainer = new ImagePanel(mCraftWindow, "ProgressBarContainer");
            mBar = new ImagePanel(mBarContainer, "ProgressBar");

            //Load the craft button
            mCraft = new Button(mCraftWindow, "CraftButton");
            mCraft.SetText(Strings.Crafting.craft);
            mCraft.Clicked += craft_Clicked;

            mCraftWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            Interface.InputBlockingElements.Add(mCraftWindow);

            Globals.Me.InventoryUpdatedDelegate = () =>
            {
                //Refresh crafting window items
                LoadCraftItems(mCraftId);
            };
        }

        //Location
        public int X => mCraftWindow.X;

        public int Y => mCraftWindow.Y;

        private void LoadCraftItems(Guid id)
        {
            //Combined item
            mCraftId = id;
            if (mCombinedItem != null)
            {
                mCraftWindow.Children.Remove(mCombinedItem.Container);
            }

            //Clear the old item description box
            if (mCombinedItem != null && mCombinedItem.DescWindow != null)
            {
                mCombinedItem.DescWindow.Dispose();
            }

            if (!Globals.ActiveCraftingTable.Crafts.Contains(id))
            {
                return;
            }

            var craft = Globals.ActiveCraftingTable.Crafts.Get(id);

            if (craft == null)
            {
                return;
            }

            mCombinedItem = new RecipeItem(this, new CraftIngredient(craft.ItemId, 0))
            {
                Container = new ImagePanel(mCraftWindow, "CraftedItem")
            };

            mCombinedItem.Setup("CraftedItemIcon");
            mCombinedValue = new Label(mCombinedItem.Container, "CraftedItemQuantity");

            mCombinedItem.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

            mCombinedItem.LoadItem();
            mCombinedValue.Show();
            var quantity = Math.Max(craft.Quantity, 1);
            var itm = ItemBase.Get(craft.ItemId);
            if (itm == null || !itm.IsStackable)
            {
                quantity = 1;
            }

            mCombinedValue.Text = quantity.ToString();

            for (var i = 0; i < mItems.Count; i++)
            {
                //Clear the old item description box
                if (mItems[i].DescWindow != null)
                {
                    mItems[i].DescWindow.Dispose();
                }

                mItemContainer.RemoveChild(mItems[i].Container, true);
            }

            mItems.Clear();
            mValues.Clear();

            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            var itemdict = new Dictionary<Guid, int>();
            foreach (var item in Globals.Me.Inventory)
            {
                if (item != null)
                {
                    if (itemdict.ContainsKey(item.ItemId))
                    {
                        itemdict[item.ItemId] += item.Quantity;
                    }
                    else
                    {
                        itemdict.Add(item.ItemId, item.Quantity);
                    }
                }
            }

            for (var i = 0; i < craft.Ingredients.Count; i++)
            {
                mItems.Add(new RecipeItem(this, craft.Ingredients[i]));
                mItems[i].Container = new ImagePanel(mItemContainer, "CraftingIngredient");
                mItems[i].Setup("IngredientItemIcon");

                var lblTemp = new Label(mItems[i].Container, "IngredientItemValue");

                var onHand = 0;
                if (itemdict.ContainsKey(craft.Ingredients[i].ItemId))
                {
                    onHand = itemdict[craft.Ingredients[i].ItemId];
                }

                lblTemp.Text = onHand + "/" + craft.Ingredients[i].Quantity;
                mValues.Add(lblTemp);

                mItems[i].Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

                mItems[i].LoadItem();

                var xPadding = mItems[i].Container.Margin.Left + mItems[i].Container.Margin.Right;
                var yPadding = mItems[i].Container.Margin.Top + mItems[i].Container.Margin.Bottom;
                mItems[i]
                    .Container.SetPosition(
                        i %
                        ((mItemContainer.Width - mItemContainer.GetVerticalScrollBar().Width) /
                         (mItems[i].Container.Width + xPadding)) *
                        (mItems[i].Container.Width + xPadding) +
                        xPadding,
                        i /
                        ((mItemContainer.Width - mItemContainer.GetVerticalScrollBar().Width) /
                         (mItems[i].Container.Width + xPadding)) *
                        (mItems[i].Container.Height + yPadding) +
                        yPadding
                    );
            }

            //If crafting & we no longer have the items for the craft then stop!
            if (Crafting)
            {
                var cancraft = true;
                foreach (var c in CraftBase.Get(mCraftId).Ingredients)
                {
                    if (itemdict.ContainsKey(c.ItemId))
                    {
                        if (itemdict[c.ItemId] >= c.Quantity)
                        {
                            itemdict[c.ItemId] -= c.Quantity;
                        }
                        else
                        {
                            cancraft = false;

                            break;
                        }
                    }
                    else
                    {
                        cancraft = false;

                        break;
                    }
                }

                if (!cancraft)
                {
                    Crafting = false;
                    mCraftWindow.IsClosable = true;
                    mBar.Width = 0;
                    ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Crafting.incorrectresources, Color.Red));

                    return;
                }
            }
        }

        public void Close()
        {
            if (Crafting == false)
            {
                mCraftWindow.Close();
            }
        }

        public bool IsVisible()
        {
            return !mCraftWindow.IsHidden;
        }

        public void Hide()
        {
            if (Crafting == false)
            {
                mCraftWindow.IsHidden = true;
            }
        }

        //Load new recepie
        void tmpNode_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            if (Crafting == false)
            {
                LoadCraftItems((Guid) ((ListBoxRow) sender).UserData);
            }
        }

        //Craft the item
        void craft_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //This shouldn't be client side :(
            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            var availableItemQuantities = new Dictionary<Guid, int>();
            foreach (var item in Globals.Me.Inventory)
            {
                if (item != null)
                {
                    if (availableItemQuantities.ContainsKey(item.ItemId))
                    {
                        availableItemQuantities[item.ItemId] += item.Quantity;
                    }
                    else
                    {
                        availableItemQuantities.Add(item.ItemId, item.Quantity);
                    }
                }
            }

            var craftDescriptor = CraftBase.Get(mCraftId);
            var canCraft = craftDescriptor?.Ingredients != null;

            if (canCraft)
            {
                foreach (var ingredient in craftDescriptor.Ingredients)
                {
                    if (!availableItemQuantities.TryGetValue(ingredient.ItemId, out var availableQuantity))
                    {
                        canCraft = false;

                        break;
                    }

                    if (availableQuantity < ingredient.Quantity)
                    {
                        canCraft = false;

                        break;
                    }

                    availableItemQuantities[ingredient.ItemId] -= ingredient.Quantity;
                }
            }

            if (canCraft)
            {
                Crafting = true;
                mBarTimer = Globals.System.GetTimeMs();
                PacketSender.SendCraftItem(mCraftId);
                mCraftWindow.IsClosable = false;

                return;
            }

            ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Crafting.incorrectresources, Color.Red));
        }

        //Update the crafting bar
        public void Update()
        {
            if (!mInitialized)
            {
                for (var i = 0; i < Globals.ActiveCraftingTable?.Crafts?.Count; ++i)
                {
                    var activeCraft = CraftBase.Get(Globals.ActiveCraftingTable.Crafts[i]);
                    if (activeCraft == null)
                    {
                        continue;
                    }

                    var tmpRow = mRecipes?.AddRow(i + 1 + ") " + ItemBase.GetName(activeCraft.ItemId));
                    if (tmpRow == null)
                    {
                        continue;
                    }

                    tmpRow.UserData = Globals.ActiveCraftingTable.Crafts[i];
                    tmpRow.DoubleClicked += tmpNode_DoubleClicked;
                    tmpRow.Clicked += tmpNode_DoubleClicked;
                    tmpRow.SetTextColor(Color.White);
                    tmpRow.RenderColor = new Color(50, 255, 255, 255);
                }

                //Load the craft data
                if (Globals.ActiveCraftingTable?.Crafts?.Count > 0)
                {
                    LoadCraftItems(Globals.ActiveCraftingTable.Crafts[0]);
                }

                mInitialized = true;
            }

            if (!Crafting)
            {
                mBar.Width = 0;

                return;
            }

            var craft = CraftBase.Get(mCraftId);
            if (craft == null)
            {
                return;
            }

            var delta = Globals.System.GetTimeMs() - mBarTimer;
            if (delta > craft.Time)
            {
                delta = craft.Time;
                Crafting = false;
                if (mCraftWindow != null)
                {
                    mCraftWindow.IsClosable = true;
                }

                LoadCraftItems(mCraftId);
                mBar.Width = 0;
            }

            var ratio = craft.Time == 0 ? 0 : Convert.ToDecimal(delta) / Convert.ToDecimal(craft.Time);
            var width = ratio * mBarContainer?.Width ?? 0;

            if (mBar == null)
            {
                return;
            }

            mBar.SetTextureRect(
                0, 0, Convert.ToInt32(ratio * mBar.Texture?.GetWidth() ?? 0), mBar.Texture?.GetHeight() ?? 0
            );

            mBar.Width = Convert.ToInt32(width);
        }

    }

}
