namespace Intersect.Network.Packets.Client
{

    public class HotbarUpdatePacket : CerasPacket
    {

        public HotbarUpdatePacket(byte slot, sbyte type, int itemIndex)
        {
            HotbarSlot = slot;
            Type = type;
            Index = itemIndex;
        }

        public byte HotbarSlot { get; set; } //Hotbar Slot

        public sbyte Type { get; set; }

        public int Index { get; set; } //Inv or Spell Index

    }

}
