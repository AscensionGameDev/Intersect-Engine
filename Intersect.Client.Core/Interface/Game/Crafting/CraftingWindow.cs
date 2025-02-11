using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Game.Crafting;

public partial class CraftingWindow : Window
{
    private readonly ImagePanel mBar;

    private readonly ImagePanel mBarContainer;

    private readonly Button mCraft;

    private readonly Button mCraftAll;

    private readonly ScrollControl mItemContainer;

    private readonly List<RecipeItem> mItems = [];

    private readonly Label mLblCraftingChance;

    private readonly Label mLblCraftingTime;

    private readonly Label mLblDestroyMaterialsChance;

    private readonly Label mLblIngredients;

    private readonly Label mLblProduct;

    private readonly Label mLblRecipes;

    //Objects
    private readonly ListBox mRecipes;

    private readonly List<Label> mValues = [];
    private Guid _automaticCraftingDescriptorId;

    private RecipeItem? _craftedItem;

    private Guid _craftRecipeDescriptorId;

    private long mBarTimer;

    private Label mCombinedValue;

    private int mRemainingCrafts;

    public CraftingWindow(Canvas gameCanvas, bool journalMode) : base(
        gameCanvas,
        Globals.ActiveCraftingTable.Name,
        false,
        nameof(CraftingWindow)
    )
    {
        IsResizable = false;

        SkipRender();

        mItemContainer = new ScrollControl(this, "IngredientsContainer");

        mJournalMode = journalMode;

        //Labels
        mLblRecipes = new Label(this, "RecipesTitle");
        mLblRecipes.Text = Strings.Crafting.Recipes;

        mLblIngredients = new Label(this, "IngredientsTitle");
        mLblIngredients.Text = Strings.Crafting.Ingredients;

        mLblProduct = new Label(this, "ProductLabel");
        mLblProduct.Text = Strings.Crafting.Product;

        mLblCraftingChance = new Label(this, "ProductChanceLabel");
        mLblCraftingChance.Text = Strings.Crafting.CraftChance.ToString(0);

        mLblDestroyMaterialsChance = new Label(this, "DestroyMaterialsChanceLabel");
        mLblDestroyMaterialsChance.Text = Strings.Crafting.DestroyMaterialsChance.ToString(0);

        mLblCraftingTime = new Label(this, "CraftingTimeLabel");
        mLblCraftingTime.Text = Strings.Crafting.CraftingTime.ToString(0);

        mRecipes = new ListBox(this, "RecipesList")
        {
            CellSpacing = default, InnerPanelPadding = default,
        };

        //Progress Bar
        mBarContainer = new ImagePanel(this, "ProgressBarContainer");
        mBar = new ImagePanel(mBarContainer, "ProgressBar");

        //Load the craft button
        mCraft = new Button(this, "CraftButton");
        mCraft.SetText(Strings.Crafting.Craft);
        mCraft.Clicked += craft_Clicked;

        //Craft all button
        mCraftAll = new Button(this, "CraftAllButton");
        mCraftAll.SetText(Strings.Crafting.CraftAll.ToString(1));
        mCraftAll.Clicked += craftAll_Clicked;

        if (mJournalMode)
        {
            mCraft.Hide();
            mCraftAll.Hide();
        }

        Interface.InputBlockingComponents.Add(this);

        if (Globals.Me is { } player)
        {
            player.InventoryUpdated += PlayerOnInventoryUpdated;
        }
    }

    public bool IsCrafting => mRemainingCrafts > 0;

    private bool mJournalMode { get; }

    protected override bool CanClose => !IsCrafting;

    private void PlayerOnInventoryUpdated(Player player, int slotIndex)
    {
        LoadCraftRecipeById(_craftRecipeDescriptorId);
    }

    private void LoadCraftRecipeById(Guid craftDescriptorId)
    {
        if (!CraftBase.TryGet(craftDescriptorId, out var craftDescriptor))
        {
            return;
        }

        LoadCraftRecipe(craftDescriptor);
    }

    private void LoadCraftRecipe(CraftBase craftDescriptor)
    {
        _craftRecipeDescriptorId = craftDescriptor.Id;

        if (_craftedItem is { } craftedItem)
        {
            if (craftedItem.Container is { } container)
            {
                RemoveChild(container, true);
            }

            if (craftedItem.DescWindow is { } descriptionWindow)
            {
                descriptionWindow.Dispose();
            }
        }

        var craftedItemDescriptorId = craftDescriptor.ItemId;
        _craftedItem = new RecipeItem(this, new CraftIngredient(craftedItemDescriptorId, 0))
        {
            Container = new ImagePanel(this, "CraftedItem"),
        };

        _craftedItem.Setup("CraftedItemIcon");
        mCombinedValue = new Label(_craftedItem.Container, "CraftedItemQuantity");

        _craftedItem.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        _craftedItem.LoadItem();
        mCombinedValue.Show();
        var quantity = Math.Max(craftDescriptor.Quantity, 1);
        if (!ItemBase.TryGet(craftedItemDescriptorId, out var craftedItemDescriptor) ||
            !craftedItemDescriptor.IsStackable)
        {
            quantity = 1;
        }

        mCombinedValue.Text = quantity.ToString();

        foreach (var recipeItem in mItems)
        {
            //Clear the old item description box
            recipeItem.DescWindow?.Dispose();
            if (recipeItem.Container is { } container)
            {
                mItemContainer.RemoveChild(container, true);
            }
        }

        mItems.Clear();
        mValues.Clear();

        if (Globals.Me is not { } player)
        {
            return;
        }

        // Quickly Look through the inventory and create a catalog of what items we have, and how many
        Dictionary<Guid, int> inventoryItemsByDescriptorId = [];
        foreach (var item in player.Inventory)
        {
            var inventoryItemDescriptorId = item.ItemId;
            var currentQuantity = inventoryItemsByDescriptorId.GetValueOrDefault(inventoryItemDescriptorId, 0);
            inventoryItemsByDescriptorId[inventoryItemDescriptorId] = currentQuantity + item.Quantity;
        }

        var craftableQuantity = -1;

        for (var ingredientIndex = 0; ingredientIndex < craftDescriptor.Ingredients.Count; ingredientIndex++)
        {
            var craftingIngredient = craftDescriptor.Ingredients[ingredientIndex];
            var recipeItem = new RecipeItem(this, craftingIngredient);
            mItems.Add(recipeItem);
            var recipeItemContainer = new ImagePanel(mItemContainer, "CraftingIngredient");
            recipeItem.Container = recipeItemContainer;
            recipeItem.Setup("IngredientItemIcon");

            var lblTemp = new Label(recipeItemContainer, "IngredientItemValue");

            var onHand = inventoryItemsByDescriptorId.GetValueOrDefault(craftingIngredient.ItemId, 0);

            lblTemp.Text = onHand + "/" + craftingIngredient.Quantity;

            var possibleToCraft = (int)Math.Floor(onHand / (double)craftingIngredient.Quantity);

            if (craftableQuantity == -1 || possibleToCraft < craftableQuantity)
            {
                craftableQuantity = possibleToCraft;
            }

            mValues.Add(lblTemp);

            recipeItemContainer.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
            recipeItem.LoadItem();

            var xPadding = recipeItemContainer.Margin.Left + recipeItemContainer.Margin.Right;
            var yPadding = recipeItemContainer.Margin.Top + recipeItemContainer.Margin.Bottom;

            var availableWidth = mItemContainer.Width - mItemContainer.VerticalScrollBar.Width;
            var widthPerRecipeItemIcon = recipeItemContainer.Width + xPadding;
            var itemsPerRow = Math.Max(1, availableWidth / Math.Max(1, widthPerRecipeItemIcon));

            var column = ingredientIndex % itemsPerRow;
            var row = ingredientIndex / itemsPerRow;

            Point iconPosition = new(
                (column * widthPerRecipeItemIcon) + xPadding,
                (row * (recipeItemContainer.Height + yPadding)) + yPadding
            );
            recipeItemContainer.SetPosition(iconPosition);
        }

        //Show crafting time and chances
        mLblCraftingTime.Text = Strings.Crafting.CraftingTime.ToString(craftDescriptor.Time / 1000.0);
        mLblCraftingChance.Text = Strings.Crafting.CraftChance.ToString(craftDescriptor.FailureChance);
        mLblDestroyMaterialsChance.Text =
            Strings.Crafting.DestroyMaterialsChance.ToString(craftDescriptor.ItemLossChance);

        //If crafting & we no longer have the items for the craft then stop!
        if (IsCrafting)
        {
            var cancraft = true;
            foreach (var c in CraftBase.Get(_craftRecipeDescriptorId).Ingredients)
            {
                if (inventoryItemsByDescriptorId.ContainsKey(c.ItemId))
                {
                    if (inventoryItemsByDescriptorId[c.ItemId] >= c.Quantity)
                    {
                        inventoryItemsByDescriptorId[c.ItemId] -= c.Quantity;
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
                mRemainingCrafts = 0;
                IsClosable = true;
                mBar.Width = 0;
                ChatboxMsg.AddMessage(
                    new ChatboxMsg(
                        Strings.Crafting.IncorrectResources,
                        CustomColors.Alerts.Error,
                        ChatMessageType.Crafting
                    )
                );

                return;
            }
        }

        mCraftAll.IsHidden = mJournalMode || craftableQuantity < 2;
        if (!mCraftAll.IsHidden)
        {
            mCraftAll.SetText(Strings.Crafting.CraftAll.ToString(craftableQuantity));
            mCraftAll.UserData = craftableQuantity;
            mCraftAll.IsDisabled = IsCrafting;
        }
    }

    public override void Hide()
    {
        if (IsCrafting)
        {
            return;
        }

        base.Hide();
    }

    //Load new recepie
    private void CraftingRecipeRowOnClicked(Base sender, MouseButtonState arguments)
    {
        if (IsCrafting)
        {
            return;
        }

        if (sender is not ListBoxRow { UserData: CraftBase craftDescriptor })
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Sender is not a {ListBoxRowTypeName} or the {UserDataPropertyName} is not a {CraftDescriptorTypeName}",
                typeof(ListBoxRow).GetName(true),
                nameof(UserData),
                typeof(CraftBase).GetName(true)
            );
            return;
        }

        LoadCraftRecipe(craftDescriptor);
    }

    private bool CanCraft()
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

        var craftDescriptor = CraftBase.Get(_craftRecipeDescriptorId);
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

        return canCraft;
    }

    private void DoCraft(int count)
    {
        if (IsCrafting)
        {
            PacketSender.SendCraftItem(default, default);
            mRemainingCrafts = 0;
            IsClosable = true;
            mBar.Width = 0;
            _automaticCraftingDescriptorId = default;

            LoadCraftRecipeById(_craftRecipeDescriptorId);

            return;
        }

        if (CanCraft())
        {
            mRemainingCrafts = count;
            mBarTimer = Timing.Global.Milliseconds;
            PacketSender.SendCraftItem(_craftRecipeDescriptorId, count);
            IsClosable = false;
            mCraftAll.IsDisabled = true;

            return;
        }

        ChatboxMsg.AddMessage(
            new ChatboxMsg(Strings.Crafting.IncorrectResources, CustomColors.Alerts.Error, ChatMessageType.Crafting)
        );
    }

    //Craft the item
    private void craft_Clicked(Base sender, MouseButtonState arguments)
    {
        DoCraft(1);
    }

    //Craft all the items
    private void craftAll_Clicked(Base sender, MouseButtonState arguments)
    {
        if (CanCraft())
        {
            DoCraft((int)mCraftAll.UserData);
            _automaticCraftingDescriptorId = _craftRecipeDescriptorId;
            mCraftAll.Disable();
        }
        else
        {
            ChatboxMsg.AddMessage(
                new ChatboxMsg(Strings.Crafting.IncorrectResources, CustomColors.Alerts.Error, ChatMessageType.Crafting)
            );
        }
    }

    protected override void Prelayout(Framework.Gwen.Skin.Base skin)
    {
        base.Prelayout(skin);

        if (IsCrafting)
        {
            mCraft.SetText(Strings.Crafting.CraftStop);
        }
        else
        {
            mCraft.SetText(Strings.Crafting.Craft);
            mBar.Width = 0;
            return;
        }

        var craft = CraftBase.Get(_craftRecipeDescriptorId);
        if (craft == null)
        {
            return;
        }

        var delta = Timing.Global.Milliseconds - mBarTimer;
        if (delta > craft.Time)
        {
            delta = craft.Time;
            mRemainingCrafts--;
            if (mRemainingCrafts < 1)
            {
                if (this != null)
                {
                    IsClosable = true;
                }

                mBar.Width = 0;
            }
            else
            {
                mBarTimer = Timing.Global.Milliseconds;
            }
        }

        var ratio = craft.Time == 0 ? 0 : Convert.ToDecimal(delta) / Convert.ToDecimal(craft.Time);
        var width = ratio * mBarContainer?.Width ?? 0;

        if (mBar == null)
        {
            return;
        }

        mBar.SetTextureRect(0, 0, Convert.ToInt32(ratio * mBar.Texture?.Width ?? 0), mBar.Texture?.Height ?? 0);

        mBar.Width = Convert.ToInt32(width);
    }

    protected override void EnsureInitialized()
    {
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());

        if (Globals.ActiveCraftingTable.Crafts is not { Count: > 0 } craftRecipeDescriptorIds)
        {
            return;
        }

        CraftBase? craftRecipeDescriptorToLoad = null;
        foreach (var craftRecipeDescriptorId in craftRecipeDescriptorIds)
        {
            if (!CraftBase.TryGet(craftRecipeDescriptorId, out var craftRecipeDescriptor))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Failed to load craft recipe descriptor {CraftDescriptorId}",
                    craftRecipeDescriptorId
                );
                continue;
            }

            var craftNumber = Math.Max(1, mRecipes.RowCount + 1);
            var craftingRecipeRow = mRecipes.AddRow(
                Strings.Crafting.RecipeListEntry.ToString(craftNumber, craftRecipeDescriptor.Name)
            );
            craftingRecipeRow.UserData = craftRecipeDescriptor;
            craftingRecipeRow.DoubleClicked += CraftingRecipeRowOnClicked;
            craftingRecipeRow.Clicked += CraftingRecipeRowOnClicked;

            craftRecipeDescriptorToLoad ??= craftRecipeDescriptor;
        }

        if (craftRecipeDescriptorToLoad is not null)
        {
            LoadCraftRecipe(craftRecipeDescriptorToLoad);
        }
    }

    protected override void Render(Framework.Gwen.Skin.Base skin)
    {
        base.Render(skin);
    }
}