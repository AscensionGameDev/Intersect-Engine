using System.Collections;

namespace Intersect.Collections.Slotting;

public class SlotList<TSlot> : IList<TSlot> where TSlot : ISlot
{
    private readonly SortedList<int, TSlot> _slots;
    private readonly Func<int, TSlot> _factory;

    public SlotList(int capacity, Func<int, TSlot> factory)
    {
        _slots = new SortedList<int, TSlot>(capacity);
        _factory = factory;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TSlot> GetEnumerator() => _slots.Values.GetEnumerator();

    public void Add(TSlot slot)
    {
        if (_slots.TryGetValue(slot.Slot, out var existingSlot))
        {
            if (existingSlot.IsEmpty)
            {
                _slots[slot.Slot] = slot;
            }
            else
            {
                throw new ArgumentException($"Slot {slot.Slot} is already filled with a non-empty slot.", nameof(slot));
            }
        }
        else
        {
            _slots.Add(slot.Slot, slot);
        }
    }

    public void Clear() => _slots.Clear();

    public bool Contains(TSlot slot) => _slots.ContainsValue(slot);

    public void CopyTo(TSlot[] array, int arrayIndex) =>
        _slots.Select(kvp => kvp.Value).ToArray().CopyTo(array, arrayIndex);

    public bool Remove(TSlot slot) => _slots.Remove(slot.Slot);

    public int Capacity
    {
        get => _slots.Capacity;
        set => _slots.Capacity = value;
    }

    public int Count => _slots.Count;

    public bool IsReadOnly => ((IDictionary)_slots).IsReadOnly;

    public int IndexOf(TSlot slot) => _slots.IndexOfValue(slot);

    public void Insert(int slotIndex, TSlot slot)
    {
        if (slotIndex != slot.Slot)
        {
            throw new ArgumentException($"Tried to insert slot {slot.Slot} to slot index {slotIndex}", nameof(slotIndex));
        }

        _slots.Add(slotIndex, slot);
    }

    public void RemoveAt(int slotIndex) => _slots.Remove(slotIndex);

    public TSlot this[int slotIndex]
    {
        get
        {
            if (_slots.TryGetValue(slotIndex, out var slot))
            {
                return slot;
            }

            slot = _factory(slotIndex);
            _slots[slotIndex] = slot;
            return slot;
        }
        set => _slots[slotIndex] = value;
    }

    public int FindIndex(Func<TSlot, bool> predicate)
    {
        var copy = this.ToArray();
        for (var index = 0; index < copy.Length; ++index)
        {
            if (predicate(copy[index]))
            {
                return index;
            }
        }

        return -1;
    }
}