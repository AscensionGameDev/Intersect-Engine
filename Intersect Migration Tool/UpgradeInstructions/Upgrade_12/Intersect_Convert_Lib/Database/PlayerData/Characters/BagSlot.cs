using System;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class BagSlot : Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid ParentBagId { get; private set; }
        public virtual Bag ParentBag { get; private set; }
        public int Slot { get; private set; }

        public BagSlot()
        {

        }

        public BagSlot(int slot)
        {
            Slot = slot;
        }
    }
}
