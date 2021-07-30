using Intersect.GameObjects;
using System;

namespace Intersect.Client.Framework.Items
{
    public interface IItem
    {
        Guid? BagId { get; set; }
        ItemBase Base { get; }
        Guid ItemId { get; set; }
        int Quantity { get; set; }
        int[] StatBuffs { get; set; }

        IItem Clone();
        void Load(Guid id, int quantity, Guid? bagId, int[] statBuffs);
    }
}