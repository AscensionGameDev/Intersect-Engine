using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class InventoryUpdatePacket : CerasPacket
    {

        public InventoryUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs, Dictionary<string, int> tags, Dictionary<string, string> stringTags)
		{
            Slot = slot;
            ItemId = id;
            BagId = bagId;
            Quantity = quantity;
            StatBuffs = statBuffs;
			Tags = tags;
			StringTags = stringTags;
		}

        public int Slot { get; set; }

        public Guid ItemId { get; set; }

        public Guid? BagId { get; set; }

        public int Quantity { get; set; }

        public int[] StatBuffs { get; set; }

		public Dictionary<string, int> Tags { get; set; }
		public Dictionary<string, string> StringTags { get; set; }

	}

}
