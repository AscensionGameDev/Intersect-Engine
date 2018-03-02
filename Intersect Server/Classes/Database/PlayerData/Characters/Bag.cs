using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Bag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
        public int SlotCount { get; set; }

        public List<BagItem> Items { get; set; } = new List<BagItem>();

        public Bag()
        {
            
        }

        public Bag(int slots)
        {
            SlotCount = slots;
            for (int i = 0; i < slots; i++)
            {
                Items.Add(new BagItem());
            }
        }
    }
}
