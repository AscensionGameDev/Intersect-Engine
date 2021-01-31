using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.GameObjects;
using Intersect.Logging;
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
            ValidateSlots();
        }

        [JsonIgnore, NotMapped]
        public bool IsEmpty => Slots?.All(slot => slot?.ItemId == default || ItemBase.Get(slot.ItemId) == default) ?? true;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public int SlotCount { get; private set; }

        public virtual List<BagSlot> Slots { get; set; } = new List<BagSlot>();

        public void ValidateSlots(bool checkItemExistence = true)
        {
            if (Slots == null)
            {
                Slots = new List<BagSlot>(SlotCount);
            }

            var slots = Slots
                .Where(bagSlot => bagSlot != null)
                .OrderBy(bagSlot => bagSlot.Slot)
                .Select(
                    bagSlot =>
                    {
                        if (checkItemExistence && (bagSlot.ItemId == Guid.Empty || bagSlot.Descriptor == null))
                        {
                            bagSlot.Set(new Item());
                        }

                        return bagSlot;
                    }
                )
                .ToList();

            for (var slotIndex = 0; slotIndex < SlotCount; ++slotIndex)
            {
                if (slotIndex < slots.Count)
                {
                    var slot = slots[slotIndex];
                    if (slot != null)
                    {
                        if (slot.Slot != slotIndex)
                        {
                            slots.Insert(slotIndex, new BagSlot(slotIndex));
                        }
                    }
                    else
                    {
                        slots[slotIndex] = new BagSlot(slotIndex);
                    }
                }
                else
                {
                    slots.Add(new BagSlot(slotIndex));
                }
            }

            Slots = slots;
        }

        public static Bag GetBag(Guid id)
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext())
                {
                    var bag = context.Bags.Where(p => p.Id == id).Include(p => p.Slots).SingleOrDefault();
                    if (bag != null)
                    {
                        bag.Slots = bag.Slots.OrderBy(p => p.Slot).ToList();
                        bag.ValidateSlots();
                    }

                    return bag;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        public void Save ()
        {
            try
            {
                using (var context = DbInterface.CreatePlayerContext(readOnly: false))
                {
                    context.Bags.Update(this);
                    context.ChangeTracker.DetectChanges();
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

    }

}
