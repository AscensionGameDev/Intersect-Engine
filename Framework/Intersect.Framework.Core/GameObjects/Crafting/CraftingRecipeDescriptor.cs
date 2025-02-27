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