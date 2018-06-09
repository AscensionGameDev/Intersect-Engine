using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.GameObjects.Crafting
{
    public class CraftIngredient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        public ItemBase Item { get; set; }
        public int Quantity { get; set; }

        public CraftIngredient() : this(null, -1)
        {

        }

        public CraftIngredient(ItemBase item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}
