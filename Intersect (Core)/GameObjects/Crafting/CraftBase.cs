using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Models;
using Intersect.GameObjects.Events;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Crafting
{

    public class CraftBase : DatabaseObject<CraftBase>, IFolderable
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

    }

    public class CraftIngredient
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
