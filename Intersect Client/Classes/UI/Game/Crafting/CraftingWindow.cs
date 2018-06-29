using System;
using System.Collections.Generic;
using Intersect.Client.Classes.UI.Game.Crafting;
using Intersect.GameObjects;
using Intersect.Client.Classes.Localization;
using Intersect.GameObjects.Crafting;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game.Chat;

namespace Intersect_Client.Classes.UI.Game
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
        private int mCraftIndex;
        public bool Crafting;

        public CraftingWindow(Canvas gameCanvas)
        {
            mCraftWindow = new WindowControl(gameCanvas, Strings.Crafting.title, false,
                "CraftingWindow");
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

            mCraftWindow.LoadJsonUi(GameContentManager.UI.InGame);

            Gui.InputBlockingElements.Add(mCraftWindow);

            Globals.Me.InventoryUpdatedDelegate = () =>
            {
                //Refresh crafting window items
                LoadCraftItems(mCraftIndex);
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

        private void LoadCraftItems(int index)
        {
            //Combined item
            mCraftIndex = index;
            if (mCombinedItem != null)
            {
                mCraftWindow.Children.Remove(mCombinedItem.Container);
            }
            //Clear the old item description box
            if (mCombinedItem != null && mCombinedItem.DescWindow != null)
            {
                mCombinedItem.DescWindow.Dispose();
            }
            if (index >= Globals.ActiveCraftingTable.Crafts.Count) return; 
            var craft = CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[index]);
            if (craft == null) return;
            
            mCombinedItem = new RecipeItem(this, new CraftIngredient(craft.Item, 0))
            {
                Container = new ImagePanel(mCraftWindow, "CraftedItemContainer")
            };
            mCombinedItem.Setup("CraftedItemIcon");

            mCombinedItem.Container.LoadJsonUi(GameContentManager.UI.InGame);
            mCombinedItem.LoadItem();

            for (int i = 0; i < mItems.Count; i++)
            {
                //Clear the old item description box
                if (mItems[i].DescWindow != null)
                {
                    mItems[mCraftIndex].DescWindow.Dispose();
                }
                mItemContainer.RemoveChild(mItems[i].Container, true);
            }
            mItems.Clear();
            mValues.Clear();

            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            Dictionary<int, int> itemdict = new Dictionary<int, int>();
            foreach (var item in Globals.Me.Inventory)
            {
                if (item != null)
                {
                    if (itemdict.ContainsKey(item.ItemNum))
                    {
                        itemdict[item.ItemNum] += item.ItemVal;
                    }
                    else
                    {
                        itemdict.Add(item.ItemNum, item.ItemVal);
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
                if (itemdict.ContainsKey(craft.Ingredients[i].Item))
                {
                    onHand = itemdict[craft.Ingredients[i].Item];
                }
                lblTemp.Text = onHand + "/" + craft.Ingredients[i].Quantity;
                mValues.Add(lblTemp);

                mItems[i].Container.LoadJsonUi(GameContentManager.UI.InGame);
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
                foreach (CraftIngredient c in CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[mCraftIndex]).Ingredients)
                {
                    if (itemdict.ContainsKey(c.Item))
                    {
                        if (itemdict[c.Item] >= c.Quantity)
                        {
                            itemdict[c.Item] -= c.Quantity;
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
                        Color.Red));
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
                LoadCraftItems(Convert.ToInt32(((ListBoxRow) sender).UserData));
            }
        }

        //Craft the item
        void craft_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //This shouldn't be client side :(
            //Quickly Look through the inventory and create a catalog of what items we have, and how many
            Dictionary<int, int> itemdict = new Dictionary<int, int>();
            foreach (var item in Globals.Me.Inventory)
            {
                if (item != null)
                {
                    if (itemdict.ContainsKey(item.ItemNum))
                    {
                        itemdict[item.ItemNum] += item.ItemVal;
                    }
                    else
                    {
                        itemdict.Add(item.ItemNum, item.ItemVal);
                    }
                }
            }

            var cancraft = true;
            foreach (CraftIngredient c in CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[mCraftIndex]).Ingredients)
            {
                if (itemdict.ContainsKey(c.Item))
                {
                    if (itemdict[c.Item] >= c.Quantity)
                    {
                        itemdict[c.Item] -= c.Quantity;
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
                    Color.Red));
                return;
            }

            Crafting = true;
            mBarTimer = Globals.System.GetTimeMs();
            PacketSender.SendCraftItem(mCraftIndex);
            mCraftWindow.IsClosable = false;
        }

        //Update the crafting bar
        public void Update()
        {
            if (!mInitialized)
            {
                ListBoxRow tmpRow;
                for (int i = 0; i < Globals.ActiveCraftingTable.Crafts.Count; i++)
                {
                    tmpRow = mRecipes.AddRow((i + 1) + ") " + ItemBase.GetName(CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[i]).Item));
                    tmpRow.UserData = i;
                    tmpRow.DoubleClicked += tmpNode_DoubleClicked;
                    tmpRow.Clicked += tmpNode_DoubleClicked;
                    tmpRow.SetTextColor(Color.White);
                }

                //Load the craft data
                LoadCraftItems(0);
                mInitialized = true;
            }
            if (Crafting == true)
            {
                long i = Globals.System.GetTimeMs() - mBarTimer;
                if (i > CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[mCraftIndex]).Time)
                {
                    i = CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[mCraftIndex]).Time;
                    Crafting = false;
                    mCraftWindow.IsClosable = true;
                    LoadCraftItems(mCraftIndex);
                }
                decimal width = Convert.ToDecimal(i) / Convert.ToDecimal(CraftBase.Lookup.Get<CraftBase>(Globals.ActiveCraftingTable.Crafts[mCraftIndex]).Time) *
                                mBarContainer.Width;
                mBar.Width = Convert.ToInt32(width);
            }
        }
    }
}