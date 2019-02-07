using System;
using System.Collections.Generic;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Client.UI.Game.Chat;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;

namespace Intersect.Client.UI.Game.Crafting
{
    public class CraftingWindow
    {
        private static int sItemXPadding = 4;
        private static int sItemYPadding = 4;
        private ImagePanel mBar;

        private ImagePanel mBarContainer;

        private RecipeItem mCombinedItem;
        private Button mCraft;
        private ImagePanel mCraftedItemTemplate;

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

        private long mBarTimer;
        private Guid mCraftId;
        public bool Crafting;

        public CraftingWindow(Canvas gameCanvas)
        {
            mCraftWindow = new WindowControl(gameCanvas, Strings.Crafting.title, false, "CraftingWindow");
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

            mCraftWindow.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());

            Gui.InputBlockingElements.Add(mCraftWindow);

            Globals.Me.InventoryUpdatedDelegate = () =>
            {
                //Refresh crafting window items
                LoadCraftItems(mCraftId);
            };
        }

        //Location
        public int X
        {
            get { return mCraftWindow.X; }
        }

        public int Y
        {
            get { return mCraftWindow.Y; }
        }

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
            if (!Globals.ActiveCraftingTable.Crafts.Contains(id)) return; 
            var craft = Globals.ActiveCraftingTable.Crafts.Get(id);
            if (craft == null) return;
            
            mCombinedItem = new RecipeItem(this, new CraftIngredient(craft.ItemId, 0))
            {
                Container = new ImagePanel(mCraftWindow, "CraftedItem")
            };
            mCombinedItem.Setup("CraftedItemIcon");

            mCombinedItem.Container.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
            mCombinedItem.LoadItem();

            for (int i = 0; i < mItems.Count; i++)
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
            Dictionary<Guid, int> itemdict = new Dictionary<Guid, int>();
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

            for (int i = 0; i < craft.Ingredients.Count; i++)
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

                mItems[i].Container.LoadJsonUi(GameContentManager.UI.InGame, GameGraphics.Renderer.GetResolutionString());
                mItems[i].LoadItem();

                var xPadding = mItems[i].Container.Padding.Left + mItems[i].Container.Padding.Right;
                var yPadding = mItems[i].Container.Padding.Top + mItems[i].Container.Padding.Bottom;
                mItems[i].Container.SetPosition(
                    (i % ((mItemContainer.Width - mItemContainer.GetVerticalScrollBar().Width) /
                          (mItems[i].Container.Width + xPadding))) * (mItems[i].Container.Width + xPadding) + xPadding,
                    (i / ((mItemContainer.Width - mItemContainer.GetVerticalScrollBar().Width) /
                          (mItems[i].Container.Width + xPadding))) * (mItems[i].Container.Height + yPadding) +
                    yPadding);
            }

            //If crafting & we no longer have the items for the craft then stop!
            if (Crafting)
            {
                var cancraft = true;
                foreach (CraftIngredient c in CraftBase.Get(mCraftId).Ingredients)
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
                    ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Crafting.incorrectresources,
                        Framework.GenericClasses.Color.Red));
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
                LoadCraftItems((Guid)((ListBoxRow) sender).UserData);
            }
        }

        //Craft the item
        void craft_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //This shouldn't be client side :(
            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            Dictionary<Guid, int> itemdict = new Dictionary<Guid, int>();
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

            var cancraft = true;
            foreach (CraftIngredient c in CraftBase.Get(mCraftId).Ingredients)
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
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Crafting.incorrectresources,
                    Framework.GenericClasses.Color.Red));
                return;
            }

            Crafting = true;
            mBarTimer = Globals.System.GetTimeMs();
            PacketSender.SendCraftItem(mCraftId);
            mCraftWindow.IsClosable = false;
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

                    var tmpRow = mRecipes?.AddRow((i + 1) + ") " + ItemBase.GetName(activeCraft.ItemId));
                    if (tmpRow == null)
                    {
                        continue;
                    }

                    tmpRow.UserData = Globals.ActiveCraftingTable.Crafts[i];
                    tmpRow.DoubleClicked += tmpNode_DoubleClicked;
                    tmpRow.Clicked += tmpNode_DoubleClicked;
                    tmpRow.SetTextColor(Framework.GenericClasses.Color.White);
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
            }

            var ratio = craft.Time == 0 ? 0 : Convert.ToDecimal(delta) / Convert.ToDecimal(craft.Time);
            var width = ratio * mBarContainer?.Width ?? 0;

            if (mBar == null)
            {
                return;
            }

            mBar.SetTextureRect(0, 0, Convert.ToInt32(ratio * mBar.Texture?.GetWidth() ?? 0), mBar.Texture?.GetHeight() ?? 0);
            mBar.Width = Convert.ToInt32(width);
        }
    }
}