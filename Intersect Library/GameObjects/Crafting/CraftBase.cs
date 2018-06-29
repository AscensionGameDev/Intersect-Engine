using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Crafting
{
    public class CraftBase : DatabaseObject<CraftBase>
    {
        [JsonIgnore]
        [Column("Ingredients")]
        public string IngredientsJson
        {
            get => JsonConvert.SerializeObject(Ingredients, Formatting.None);
            protected set => Ingredients = JsonConvert.DeserializeObject<List<CraftIngredient>>(value);
        }
        [NotMapped]
        public List<CraftIngredient> Ingredients = new List<CraftIngredient>();

        [JsonProperty(Order = -3)]
        public int Item { get; set; } = -1;
        [JsonProperty(Order = -2)]
        public int Time { get; set; }

        [JsonConstructor]
        public CraftBase(int index) : base(index)
        {
            Name = "New Craft";
        }

        //Parameterless constructor for EF
        public CraftBase()
        {
            Name = "New Craft";
        }
    }

    public class CraftIngredient
    {
        public int Item = -1;
        public int Quantity = 1;

        public CraftIngredient(int item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public ItemBase GetItem()
        {
            return ItemBase.Lookup.Get<ItemBase>(Item);
        }
    }
}