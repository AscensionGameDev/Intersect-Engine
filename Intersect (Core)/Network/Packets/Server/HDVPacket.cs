using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
	public class HDVPacket : CerasPacket
	{
		public HDVPacket(Guid hdvid, HDVItemPacket[] hdvItems)
		{
			HdvID = hdvid;
			HdvItems = hdvItems;
		}

		public Guid HdvID { get; set; }
		public HDVItemPacket[] HdvItems { get; set; }
	}
}
