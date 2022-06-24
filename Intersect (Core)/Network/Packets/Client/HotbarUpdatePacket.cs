using MessagePack;
using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class HotbarUpdatePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public HotbarUpdatePacket()
        {
        }

        public HotbarUpdatePacket(int slot, sbyte type, int itemIndex)
        {
            HotbarSlot = slot;
            Type = type;
            Index = itemIndex;
        }

        [Key(0)]
        public int HotbarSlot { get; set; } //Hotbar Slot

        [Key(1)]
        public sbyte Type { get; set; }

        [Key(2)]
        public int Index { get; set; } //Inv or Spell Index
    }

}
