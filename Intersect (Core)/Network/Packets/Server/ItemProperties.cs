using System;
using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class ItemProperties
    {
        public ItemProperties()
        {
        }

        public ItemProperties(ItemProperties other)
        {
            if (other == default)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Array.Copy(other.StatModifiers, StatModifiers, Enum.GetValues<Stat>().Length);
        }

        [Key(0)]
        public int[] StatModifiers { get; set; } = new int[Enum.GetValues<Stat>().Length];
    }
}
