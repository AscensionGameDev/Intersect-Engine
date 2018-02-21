using System;
using System.Collections.Generic;
using Intersect.Client.Classes.UI.Game.Crafting;
using Intersect.GameObjects;
using Intersect.Localization;
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
            mCraftWindow = new WindowControl(gameCanvas, Strings.Get("craftingbench", "title"), false,
                "CraftingWindow");
            mCraftWindow.DisableResizing();

            mItemContainer = new ScrollControl(mCraftWindow, "IngredientsContainer");

            //Labels
            mLblRecipes = new Label(mCraftWindow, "RecipesTitle");
            mLblRecipes.Text = Strings.Get("craftingbench", "recipes");

            mLblIngredients = new Label(mCraftWindow, "IngredientsTitle");
            mLblIngredients.Text = Strings.Get("craftingbench", "ingredients");

            mLblProduct = new Label(mCraftWindow, "ProductLabel");
            mLblProduct.Text = Strings.Get("craftingbench", "product");

            //Recepie list
            mRecipes = new ListBox(mCraftWindow, "RecipesList");

            //Progress Bar
            mBarContainer = new ImagePanel(mCraftWindow, "ProgressBarContainer");
            mBar = new ImagePanel(mBarContainer, "ProgressBar");

            //Load the craft button
            mCraft = new Button(mCraftWindow, "CraftButton");
            mCraft.SetText(Strings.Get("craftingbench", "craft"));
            mCraft.Clicked += craft_Clicked;

            Gui.LoadRootUiData(mCraftWindow, "InGame.xml");

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
            if (index >= Globals.GameBench.Crafts.Count) return; 
            var craft = Globals.GameBench.Crafts[index];
            if (craft == null) return;

            mCombinedItem = new RecipeItem(this, new CraftIngredient(craft.Item, 0))
            {
                Container = new ImagePanel(mCraftWindow, "CraftedItemContainer")
            };
            mCombinedItem.Setup("CraftedItemIcon");

            //TODO Made this more efficient.
            Gui.LoadRootUiData(mCombinedItem.Container, "InGame.xml");

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
                mItems[i].Container = new ImagePanel(mItemContainer, "IngredientItemContainer");
                mItems[i].Setup("IngredientItemIcon");

                Label lblTemp = new Label(mItems[i].Container, "IngredientItemValue");

                int onHand = 0;
                if (itemdict.ContainsKey(craft.Ingredients[i].Item))
                {
                    onHand = itemdict[craft.Ingredients[i].Item];
                }
                lblTemp.Text = onHand + "/" + craft.Ingredients[i].Quantity;
                mValues.Add(lblTemp);

                //TODO Made this more efficient.
                Gui.LoadRootUiData(mItems[i].Container, "InGame.xml");

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
                foreach (CraftIngredient c in Globals.GameBench.Crafts[mCraftIndex].Ingredients)
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
                    ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Get("craftingbench", "incorrectresources"),
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
            foreach (CraftIngredient c in Globals.GameBench.Crafts[mCraftIndex].Ingredients)
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
                ChatboxMsg.AddMessage(new ChatboxMsg(Strings.Get("craftingbench", "incorrectresources"),
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
                for (int i = 0; i < Globals.GameBench.Crafts.Count; i++)
                {
                    tmpRow = mRecipes.AddRow((i + 1) + ") " + ItemBase.GetName(Globals.GameBench.Crafts[i].Item));
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
                if (i > Globals.GameBench.Crafts[mCraftIndex].Time)
                {
                    i = Globals.GameBench.Crafts[mCraftIndex].Time;
                    Crafting = false;
                    mCraftWindow.IsClosable = true;
                    LoadCraftItems(mCraftIndex);
                }
                decimal width = Convert.ToDecimal(i) / Convert.ToDecimal(Globals.GameBench.Crafts[mCraftIndex].Time) *
                                mBarContainer.Width;
                mBar.Width = Convert.ToInt32(width);
            }
        }
    }
}