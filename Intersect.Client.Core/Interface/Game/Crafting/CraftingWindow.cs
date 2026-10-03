using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Reflection;
using Intersect.GameObjects;
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
    private readonly List<Label> mIngredientNames = [];
    private Label? mBlockLabel;
    private Label? mEmptyLabel;
    private Label? mDescriptionLabel;
    private Label? mRequiresLabel;

    // Extra room compared to the layout JSON so long texts and recipe names stay inside the frame.
    private const int ExtraWidth = 150;
    private const int ExtraHeight = 80;
    private const int ListWidth = 290;
    private const int IconSize = 48;

    // Right-hand detail panel (WoW-style): product, description, requirement, components, stats.
    private int mRightX;
    private int mRightW;

    private static readonly Color GoldColor = new(255, 255, 209, 0);

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
        if (!CraftingRecipeDescriptor.TryGet(craftDescriptorId, out var craftDescriptor))
        {
            return;
        }

        LoadCraftRecipe(craftDescriptor);
    }

    private void LoadCraftRecipe(CraftingRecipeDescriptor craftDescriptor)
    {
        _craftRecipeDescriptorId = craftDescriptor.Id;

        if (_craftedItem is { } craftedItem)
        {
            if (craftedItem.Container is { } container)
            {
                RemoveChild(container, true);
            }

            Interface.GameUi.ItemDescriptionWindow?.Hide();
        }

        var craftedItemDescriptorId = craftDescriptor.ItemId;
        _craftedItem = new RecipeItem(this, new CraftingRecipeIngredient(craftedItemDescriptorId, 0))
        {
            Container = new ImagePanel(this, "CraftedItem"),
        };

        _craftedItem.Setup("CraftedItemIcon");
        mCombinedValue = new Label(_craftedItem.Container, "CraftedItemQuantity");

        _craftedItem.Container.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        _craftedItem.Container.SetPosition(mRightX, 6);

        _craftedItem.LoadItem();
        mCombinedValue.Show();
        var quantity = Math.Max(craftDescriptor.Quantity, 1);
        if (!ItemDescriptor.TryGet(craftedItemDescriptorId, out var craftedItemDescriptor) ||
            !craftedItemDescriptor.IsStackable)
        {
            quantity = 1;
        }

        mCombinedValue.Text = quantity.ToString();

        foreach (var recipeItem in mItems)
        {
            //Clear the old item description box
            Interface.GameUi.ItemDescriptionWindow?.Hide();
            if (recipeItem.Container is { } container)
            {
                mItemContainer.RemoveChild(container, true);
            }
        }

        mItems.Clear();
        mValues.Clear();
        foreach (var nameLabel in mIngredientNames)
        {
            mItemContainer.RemoveChild(nameLabel, true);
        }

        mIngredientNames.Clear();

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

            // One ingredient per row: icon, then its name coloured by whether the player has enough of it.
            Point iconPosition = new(xPadding, (ingredientIndex * (recipeItemContainer.Height + yPadding)) + yPadding);
            recipeItemContainer.SetPosition(iconPosition);

            var hasEnough = onHand >= craftingIngredient.Quantity;
            var nameLabel = new Label(mItemContainer, "IngredientName")
            {
                Text = ItemDescriptor.TryGet(craftingIngredient.ItemId, out var ingredientDescriptor)
                    ? $"{ingredientDescriptor.Name} x{craftingIngredient.Quantity}"
                    : "?",
                TextColor = hasEnough ? new Color(255, 120, 220, 120) : new Color(255, 230, 90, 90),
            };
            nameLabel.SetBounds(
                iconPosition.X + recipeItemContainer.Width + 10,
                iconPosition.Y + Math.Max(0, (recipeItemContainer.Height - 18) / 2),
                Math.Max(60, mItemContainer.Width - mItemContainer.VerticalScrollBar.Width - recipeItemContainer.Width - 24),
                20
            );
            mIngredientNames.Add(nameLabel);
        }

        //Show crafting time and chances
        mLblCraftingChance.Text = Strings.Crafting.CraftChance.ToString(craftDescriptor.FailureChance);
        mLblDestroyMaterialsChance.Text =
            Strings.Crafting.DestroyMaterialsChance.ToString(craftDescriptor.ItemLossChance);
        mLblCraftingTime.Text =
            $"Temps : {craftDescriptor.Time / 1000.0:0.#}s    Échec : {craftDescriptor.FailureChance}%    Perte des composants : {craftDescriptor.ItemLossChance}%";

        // Product name (rarity colour), description and requirement, WoW-style.
        var productName = craftedItemDescriptor?.Name ?? craftDescriptor.Name;
        mLblProduct.Text = productName;
        mLblProduct.TextColor = craftedItemDescriptor != null &&
                                CustomColors.Items.Rarities.TryGetValue(craftedItemDescriptor.Rarity, out var rarityColor)
            ? rarityColor
            : new Color(255, 120, 230, 120);
        if (mDescriptionLabel != null)
        {
            mDescriptionLabel.Text = craftedItemDescriptor?.Description ?? string.Empty;
        }

        if (mRequiresLabel != null)
        {
            mRequiresLabel.Text = craftDescriptor.Profession != 0
                ? $"Requiert : {Professions.GetName((Profession)craftDescriptor.Profession)} niveau {craftDescriptor.SkillOrange}"
                : string.Empty;
            mRequiresLabel.TextColor = GetBlockReason(craftDescriptor) != null
                ? new Color(255, 230, 90, 90)
                : new Color(255, 230, 230, 230);
        }

        //If crafting & we no longer have the items for the craft then stop!
        if (IsCrafting)
        {
            var cancraft = true;
            foreach (var c in CraftingRecipeDescriptor.Get(_craftRecipeDescriptorId).Ingredients)
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

        var blockReason = GetBlockReason(craftDescriptor);
        if (mBlockLabel != null)
        {
            mBlockLabel.Text = blockReason ?? string.Empty;
        }

        if (!IsCrafting)
        {
            mCraft.IsDisabled = blockReason != null;
        }

        if (blockReason != null)
        {
            craftableQuantity = 0;
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

        if (sender is not ListBoxRow { UserData: CraftingRecipeDescriptor craftDescriptor })
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                "Sender is not a {ListBoxRowTypeName} or the {UserDataPropertyName} is not a {CraftDescriptorTypeName}",
                typeof(ListBoxRow).GetName(true),
                nameof(UserData),
                typeof(CraftingRecipeDescriptor).GetName(true)
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

        var craftDescriptor = CraftingRecipeDescriptor.Get(_craftRecipeDescriptorId);
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

    protected override void DoPrelayout(Framework.Gwen.Skin.Base skin)
    {
        base.DoPrelayout(skin);

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

        var craft = CraftingRecipeDescriptor.Get(_craftRecipeDescriptorId);
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
        EnlargeLayout();

        if (Globals.ActiveCraftingTable.Crafts is not { Count: > 0 } craftRecipeDescriptorIds)
        {
            ShowEmptyMessage();
            return;
        }

        CraftingRecipeDescriptor? craftRecipeDescriptorToLoad = null;
        CraftingRecipeDescriptor? firstRecipe = null;
        foreach (var craftRecipeDescriptorId in craftRecipeDescriptorIds)
        {
            if (!CraftingRecipeDescriptor.TryGet(craftRecipeDescriptorId, out var craftRecipeDescriptor))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Failed to load craft recipe descriptor {CraftDescriptorId}",
                    craftRecipeDescriptorId
                );
                continue;
            }

            var locked = GetBlockReason(craftRecipeDescriptor) != null;
            var rowText = craftRecipeDescriptor.Profession != 0
                ? $"{craftRecipeDescriptor.Name}   (niv. {craftRecipeDescriptor.SkillOrange})"
                : craftRecipeDescriptor.Name;
            var craftingRecipeRow = mRecipes.AddRow(rowText);
            craftingRecipeRow.UserData = craftRecipeDescriptor;
            if (locked)
            {
                craftingRecipeRow.SetTextColor(new Color(255, 165, 165, 165));
            }
            else
            {
                ApplyDifficultyColor(craftingRecipeRow, craftRecipeDescriptor);
                craftRecipeDescriptorToLoad ??= craftRecipeDescriptor;
            }

            craftingRecipeRow.DoubleClicked += CraftingRecipeRowOnClicked;
            craftingRecipeRow.Clicked += CraftingRecipeRowOnClicked;

            firstRecipe ??= craftRecipeDescriptor;
        }

        // Prefer the first recipe the player can actually craft; otherwise show the first locked one.
        craftRecipeDescriptorToLoad ??= firstRecipe;
        if (craftRecipeDescriptorToLoad is not null)
        {
            LoadCraftRecipe(craftRecipeDescriptorToLoad);
        }
        else
        {
            ShowEmptyMessage();
        }
    }

    private void EnlargeLayout()
    {
        SetSize(Width + ExtraWidth, Height + ExtraHeight);
        Align.Center(this);

        mRecipes.SetBounds(mRecipes.X, mRecipes.Y, ListWidth, mRecipes.Height + ExtraHeight);

        mRightX = mRecipes.X + ListWidth + 16;
        mRightW = Width - mRightX - 44;

        // Bottom row: progress bar on the left, buttons on the right.
        var buttonY = mCraft.Y + ExtraHeight;
        mCraft.SetBounds(mRightX + mRightW - mCraft.Width, buttonY, mCraft.Width, mCraft.Height);
        mCraftAll.SetBounds(mCraft.X - mCraftAll.Width - 8, buttonY, mCraftAll.Width, mCraftAll.Height);
        mBarContainer.SetBounds(
            mRightX,
            buttonY + Math.Max(0, (mCraft.Height - mBarContainer.Height) / 2),
            Math.Max(40, Math.Min(mBarContainer.Width, mCraftAll.X - mRightX - 12)),
            mBarContainer.Height
        );

        // Product name next to the icon created in LoadCraftRecipe.
        mLblProduct.SetBounds(mRightX + IconSize + 12, 10, mRightW - IconSize - 12, 24);

        mDescriptionLabel = new Label(this, "DescriptionLabel")
        {
            TextColor = new Color(255, 235, 235, 235),
            WrappingBehavior = WrappingBehavior.Wrapped,
            Text = string.Empty,
        };
        mDescriptionLabel.SetBounds(mRightX, 66, mRightW, 56);

        mRequiresLabel = new Label(this, "RequiresLabel")
        {
            Text = string.Empty,
        };
        mRequiresLabel.SetBounds(mRightX, 128, mRightW, 22);

        // New labels use the default (larger) font; align the layout-JSON labels on it so every text reads the same.
        if (mDescriptionLabel.Font is { } readableFont)
        {
            mLblProduct.Font = readableFont;
            mLblIngredients.Font = readableFont;
            mLblCraftingTime.Font = readableFont;
        }

        mLblIngredients.Text = "Composants :";
        mLblIngredients.TextColor = GoldColor;
        mLblIngredients.SetBounds(mRightX, 156, mRightW, 22);

        var statsY = buttonY - 52;
        var blockY = buttonY - 28;
        mItemContainer.SetBounds(mRightX, 182, mRightW, Math.Max(60, statsY - 182 - 6));

        // The three stats are shown on one line (mLblCraftingTime); the separate labels are redundant.
        mLblCraftingChance.Hide();
        mLblDestroyMaterialsChance.Hide();
        mLblCraftingTime.SetBounds(mRightX, statsY, mRightW, 22);

        mBlockLabel = new Label(this, "BlockReasonLabel")
        {
            TextColor = new Color(255, 230, 150, 60),
            Text = string.Empty,
        };
        mBlockLabel.SetBounds(mRightX, blockY, mRightW, 22);
    }

    private void ShowEmptyMessage()
    {
        mEmptyLabel = new Label(this, "EmptyRecipesLabel")
        {
            TextColor = new Color(255, 200, 200, 200),
            Text = "Aucune recette disponible ici.\nApprends ce métier auprès du formateur.",
        };
        mEmptyLabel.SetBounds(mRecipes.X + 8, mRecipes.Y + 30, mRecipes.Width - 16, 60);
    }

    /// <summary>Why this recipe cannot be crafted by the local player yet, or null when it can.</summary>
    private static string? GetBlockReason(CraftingRecipeDescriptor recipe)
    {
        if (recipe.Profession == 0 || Globals.Me is not { } player)
        {
            return null;
        }

        var name = Professions.GetName((Profession)recipe.Profession);
        if (!player.ProfessionSkills.TryGetValue(recipe.Profession, out var skill))
        {
            return $"Apprends le métier {name} auprès d'un formateur.";
        }

        if (player.KnownRecipeIds.Contains(recipe.Id))
        {
            return null;
        }

        if (skill < recipe.SkillOrange)
        {
            return $"{name} niveau {recipe.SkillOrange} requis (actuel : {skill}).";
        }

        return recipe.Trainable ? null : "Recette à apprendre auprès d'un formateur.";
    }

    /// <summary>WoW colours: orange = always raises the skill, yellow = often, green = rarely, grey = never.</summary>
    private static void ApplyDifficultyColor(ListBoxRow row, CraftingRecipeDescriptor recipe)
    {
        if (recipe.Profession == 0 || Globals.Me == null)
        {
            return;
        }

        var skill = Globals.Me.ProfessionSkills.TryGetValue(recipe.Profession, out var value) ? value : 0;
        var color = recipe.GetDifficulty(skill) switch
        {
            RecipeDifficulty.Orange => new Color(255, 255, 140, 0),
            RecipeDifficulty.Yellow => new Color(255, 255, 215, 0),
            RecipeDifficulty.Green => new Color(255, 80, 200, 80),
            _ => new Color(255, 140, 140, 140),
        };

        row.SetTextColor(color);
    }

    protected override void Render(Framework.Gwen.Skin.Base skin)
    {
        base.Render(skin);
    }
}