using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
	public class AddHDVPacket : CerasPacket
	{

		public AddHDVPacket(int slot, int quantity, int price)
		{
			Slot = slot;
			Quantity = quantity;
			Price = price;
		}

		public int Slot { get; set; }
		public int Quantity { get; set; }
		public int Price { get; set; }
	}
}
