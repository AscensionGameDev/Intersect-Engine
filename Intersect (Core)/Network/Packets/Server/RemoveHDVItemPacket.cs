using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
	public class RemoveHDVItemPacket : CerasPacket
	{
		public RemoveHDVItemPacket(Guid id)
		{
			RemoveItemHDVId = id;
		}
		public Guid RemoveItemHDVId { get; set; }
	}
}
