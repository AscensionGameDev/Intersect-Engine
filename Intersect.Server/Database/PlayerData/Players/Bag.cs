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

    public partial class Bag
    {

        public Bag()
        {
        }

        public Bag(int slots)
        {
            SlotCount = slots;
            ValidateSlots();
            Save();
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

                        //Remove any items from this bag that have been removed from the game
                        foreach (var itm in bag.Slots)
                        {
                            if (itm.ItemId != Guid.Empty && ItemBase.Get(itm.ItemId) == null)
                            {
                                itm.Set(Item.None);
                            }
                        }
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

        /// <summary>
        /// Finds all bag slots matching the desired item and quantity.
        /// </summary>
        /// <param name="itemId">The item Id to look for.</param>
        /// <param name="quantity">The quantity of the item to look for.</param>
        /// <returns>A list of <see cref="InventorySlot"/> containing the requested item.</returns>
        public List<BagSlot> FindBagItemSlots(Guid itemId, int quantity = 1)
        {
            var slots = new List<BagSlot>();

            for (var i = 0; i < SlotCount; i++)
            {
                var item = Slots[i];
                if (item?.ItemId != itemId)
                {
                    continue;
                }

                if (item.Quantity >= quantity)
                {
                    slots.Add(item);
                }
            }

            return slots;
        }

        /// <summary>
        /// Retrieves a list of open bag slots for this bag.
        /// </summary>
        /// <returns>A list of <see cref="BagSlot"/></returns>
        public List<BagSlot> FindOpenBagSlots()
        {
            var slots = new List<BagSlot>();

            for (var i = 0; i < SlotCount; i++)
            {
                var inventorySlot = Slots[i];

                if (inventorySlot != null && inventorySlot.ItemId == Guid.Empty)
                {
                    slots.Add(inventorySlot);
                }
            }
            return slots;
        }

        /// <summary>
        /// Finds the index of a given <see cref="BagSlot"/> within this bag's Slots.
        /// </summary>
        /// <param name="slot">The <see cref="BagSlot"/>to search for</param>
        /// <returns>The index if found, otherwise -1</returns>
        public int FindSlotIndex(BagSlot slot)
        {
            return Slots.FindIndex(sl => sl.Id == slot.Id);
        }

    }

}
