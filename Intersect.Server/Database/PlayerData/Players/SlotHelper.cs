using System;
using System.Collections.Generic;

namespace Intersect.Server.Database.PlayerData.Players
{

    public static class SlotHelper
    {

        public static List<TSlot> Sort<TSlot>(List<TSlot> slots) where TSlot : ISlot
        {
            slots.Sort(CompareSlotIndex);

            return slots;
        }

        public static int CompareSlotIndex<TSlot>(TSlot a, TSlot b) where TSlot : ISlot
        {
            if (a == null || b == null)
            {
                return 0;
            }

            return a.Slot - b.Slot;
        }

        public static TSlot ConstructSlot<TSlot>(int slotIndex) where TSlot : ISlot
        {
            var type = typeof(TSlot);
            var constructor = type.GetConstructor(new[] {typeof(int)});

            if (constructor == null)
            {
                throw new MissingMethodException($@"No constructor matching the signature {type.FullName}(int).");
            }

            try
            {
                return (TSlot) constructor.Invoke(new object[] {slotIndex});
            }
            catch (Exception exception)
            {
                throw new Exception($@"Exception occurred creating slot with index {slotIndex}.", exception);
            }
        }

        public static bool ValidateSlots<TSlot>(
            List<TSlot> slots,
            int targetCount,
            Func<int, TSlot> factory = null
        ) where TSlot : ISlot
        {
            if (slots.Count >= targetCount)
            {
                return false;
            }

            Sort(slots);

            for (var slotIndex = 0; slotIndex < targetCount; ++slotIndex)
            {
                if (ValidateSlot(slots, slotIndex))
                {
                    continue;
                }

                slots.Add(factory == null ? ConstructSlot<TSlot>(slotIndex) : factory.Invoke(slotIndex));
            }

            return true;
        }

        public static bool ValidateSlot<TSlot>(List<TSlot> slots, int slotIndex) where TSlot : ISlot
        {
            // If we are outside the bounds of the slot collection
            // we don't expect to find anything, and realistically
            // in the case that this return is hit this method
            // never really needed to be called in the first place.
            if (slots.Count <= slotIndex || slotIndex < 0)
            {
                return false;
            }

            // If somehow there is a null value in the slot, or if
            // the index is mismatched there is a chance we may have
            // detected data corruption and we absolutely want to
            // blow up. This is the real reason this method exists.
            if ((slots[slotIndex]?.Slot ?? -1) == slotIndex)
            {
                return true;
            }

            throw new InvalidOperationException(
                $@"Slot {slotIndex} was was expected to be filled but is either empty or mismatched. Potential data corruption."
            );
        }

    }

}
