using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class Bag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public int SlotCount { get; private set; }

        public virtual List<BagSlot> Slots { get; set; } = new List<BagSlot>();

        public Bag()
        {
            
        }

        public Bag(int slots)
        {
            SlotCount = slots;
            for (int i = 0; i < slots; i++)
            {
                Slots.Add(new BagSlot(i));
            }
        }

        public static Bag GetBag(PlayerContext context, Guid id)
        {
            var bag = context.Bags.Where(p => p.Id == id).Include(p => p.Slots).SingleOrDefault();
            if (bag != null)
                bag.Slots = bag.Slots.OrderBy(p => p.Slot).ToList();
            return bag;
        }
    }
}
