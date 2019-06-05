using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class HotbarUpdatePacket : CerasPacket
    {
        public byte HotbarSlot { get; set; } //Hotbar Slot
        public sbyte Type { get; set; }
        public int Index { get; set; } //Inv or Spell Index

        public HotbarUpdatePacket(byte slot, sbyte type, int itemIndex)
        {
            HotbarSlot = slot;
            Type = type;
            Index = itemIndex;
        }
    }
}
