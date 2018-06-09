using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Crafting
{
    public class Craft
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        public List<CraftIngredient> Ingredients { get; set; } = new List<CraftIngredient>();

        [JsonProperty(Order = -3)]
        public ItemBase Item { get; set; }

        [JsonProperty(Order = -2)]
        public int Time { get; set; } = 1;

        public void Load(ByteBuffer bf)
        {
            Item = ItemBase.Lookup.Get<ItemBase>(bf.ReadGuid());
            Time = bf.ReadInteger();
            Ingredients.Clear();
            var count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                var craftIngredient = new CraftIngredient(ItemBase.Lookup.Get<ItemBase>(bf.ReadGuid()), bf.ReadInteger());
                Ingredients.Add(craftIngredient);
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(Item.Id);
            bf.WriteInteger(Time);
            bf.WriteInteger(Ingredients.Count);
            for (int i = 0; i < Ingredients.Count; i++)
            {
                bf.WriteGuid(Ingredients[i].Item.Id);
                bf.WriteInteger(Ingredients[i].Quantity);
            }
            return bf.ToArray();
        }
    }
}
