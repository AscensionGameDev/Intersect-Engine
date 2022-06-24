using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Models;
using Intersect.GameObjects.Events;

using Newtonsoft.Json;
using Intersect.GameObjects.Conditions;

namespace Intersect.GameObjects.Crafting
{

    public partial class CraftBase : DatabaseObject<CraftBase>, IFolderable
    {

        [NotMapped] public List<CraftIngredient> Ingredients = new List<CraftIngredient>();

        [JsonConstructor]
        public CraftBase(Guid id) : base(id)
        {
            Name = "New Craft";
        }

        //Parameterless constructor for EF
        public CraftBase()
        {
            Name = "New Craft";
        }

        [JsonIgnore]
        [Column("Ingredients")]
        public string IngredientsJson
        {
            get => JsonConvert.SerializeObject(Ingredients, Formatting.None);
            protected set => Ingredients = JsonConvert.DeserializeObject<List<CraftIngredient>>(value);
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
        public string Folder { get; set; } = "";

        [Column("Event")]
        [JsonProperty]
        public Guid EventId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EventBase Event
        {
            get => EventBase.Get(EventId);
            set => EventId = value?.Id ?? Guid.Empty;
        }

        [NotMapped] public ConditionLists CraftingRequirements = new ConditionLists();

        //Requirements
        [Column("CraftingRequirements")]
        [JsonIgnore]
        public string JsonCraftingRequirements
        {
            get => CraftingRequirements.Data();
            set => CraftingRequirements.Load(value ?? "[]");
        }

    }

    public partial class CraftIngredient
    {

        public Guid ItemId;

        public int Quantity = 1;

        public CraftIngredient(Guid itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        public ItemBase GetItem()
        {
            return ItemBase.Get(ItemId);
        }

    }

}
