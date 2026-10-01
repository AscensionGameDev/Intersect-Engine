using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Crafting;

public partial class CraftingRecipeDescriptor : DatabaseObject<CraftingRecipeDescriptor>, IFolderable
{
    [NotMapped]
    public List<CraftingRecipeIngredient> Ingredients { get; set; } = [];

    [JsonConstructor]
    public CraftingRecipeDescriptor(Guid id) : base(id)
    {
        Name = "New Craft";
    }

    //Parameterless constructor for EF
    public CraftingRecipeDescriptor()
    {
        Name = "New Craft";
    }

    [JsonIgnore]
    [Column("Ingredients")]
    public string IngredientsJson
    {
        get => JsonConvert.SerializeObject(Ingredients, Formatting.None);
        protected set => Ingredients = JsonConvert.DeserializeObject<List<CraftingRecipeIngredient>>(value);
    }

    /// <summary>Profession this recipe belongs to (0 = none: classic recipe, no skill involved).</summary>
    [JsonProperty(Order = -10)]
    public int Profession { get; set; }

    /// <summary>Skill at which the recipe becomes available and always raises the skill (orange).</summary>
    [JsonProperty(Order = -9)]
    public int SkillOrange { get; set; }

    /// <summary>Skill from which the recipe is yellow (often raises the skill).</summary>
    [JsonProperty(Order = -8)]
    public int SkillYellow { get; set; }

    /// <summary>Skill from which the recipe is green (rarely raises the skill).</summary>
    [JsonProperty(Order = -7)]
    public int SkillGreen { get; set; }

    /// <summary>Skill from which the recipe is grey (never raises the skill).</summary>
    [JsonProperty(Order = -7)]
    public int SkillGray { get; set; }

    /// <summary>True: taught by the trainer as soon as the skill is high enough. False: must be learned (recipe item, quest, boss...).</summary>
    [JsonProperty(Order = -7)]
    public bool Trainable { get; set; } = true;

    /// <summary>WoW colour of this recipe for a player with the given skill.</summary>
    public RecipeDifficulty GetDifficulty(int skill)
    {
        if (skill >= SkillGray)
        {
            return RecipeDifficulty.Grey;
        }

        if (skill >= SkillGreen)
        {
            return RecipeDifficulty.Green;
        }

        return skill >= SkillYellow ? RecipeDifficulty.Yellow : RecipeDifficulty.Orange;
    }

    [JsonProperty(Order = -6)]
    public int ItemLossChance { get; set; } = 100;

    [JsonProperty(Order = -5)]
    public int FailureChance { get; set; } = 0;

    [JsonProperty(Order = -4)]
    public Guid ItemId { get; set; }

    [JsonProperty(Order = -3)]
    public int Quantity { get; set; } = 1;

    [JsonProperty(Order = -2)]
    public int Time { get; set; }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    [Column("Event")]
    [JsonProperty]
    public Guid EventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventDescriptor Event
    {
        get => EventDescriptor.Get(EventId);
        set => EventId = value?.Id ?? Guid.Empty;
    }

    [NotMapped]
    public ConditionLists CraftingRequirements { get; set; } = new();

    //Requirements
    [Column("CraftingRequirements")]
    [JsonIgnore]
    public string JsonCraftingRequirements
    {
        get => CraftingRequirements.Data();
        set => CraftingRequirements.Load(value ?? "[]");
    }
}