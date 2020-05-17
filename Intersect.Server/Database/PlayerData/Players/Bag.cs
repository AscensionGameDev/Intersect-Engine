using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.GameObjects;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class Bag
    {

        public Bag()
        {
        }

        public Bag(int slots)
        {
            SlotCount = slots;
            for (var i = 0; i < slots; i++)
            {
                Slots.Add(new BagSlot(i));
            }
        }

        [JsonIgnore, NotMapped]
        public bool IsEmpty => Slots?.All(slot => slot?.ItemId == default || ItemBase.Get(slot.ItemId) == default) ?? true;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public int SlotCount { get; private set; }

        public virtual List<BagSlot> Slots { get; set; } = new List<BagSlot>();

        public static Bag GetBag(PlayerContext context, Guid id)
        {
            var bag = context.Bags.Where(p => p.Id == id).Include(p => p.Slots).SingleOrDefault();
            if (bag != null)
            {
                bag.Slots = bag.Slots.OrderBy(p => p.Slot).ToList();
            }

            return bag;
        }

    }

}
