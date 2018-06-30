using System.Collections.Generic;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class CraftBase : DatabaseObject<CraftBase>
    {
        public List<CraftIngredient> Ingredients = new List<CraftIngredient>();
        [JsonProperty(Order = -3)]
        public int Item = -1;
        [JsonProperty(Order = -2)]
        public int Time = 1;

        [JsonConstructor]
        public CraftBase(int index) : base(index)
        {
            Name = "New Craft";
        }

        public void Load(ByteBuffer bf)
        {
            Item = bf.ReadInteger();
            Time = bf.ReadInteger();
            Ingredients.Clear();
            var count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                var craftIngredient = new CraftIngredient(bf.ReadInteger(), bf.ReadInteger());
                Ingredients.Add(craftIngredient);
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(Item);
            bf.WriteInteger(Time);
            bf.WriteInteger(Ingredients.Count);
            for (int i = 0; i < Ingredients.Count; i++)
            {
                bf.WriteInteger(Ingredients[i].Item);
                bf.WriteInteger(Ingredients[i].Quantity);
            }
            return bf.ToArray();
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
    }
}