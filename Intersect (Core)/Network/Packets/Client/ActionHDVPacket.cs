using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
	public class ActionHDVPacket : CerasPacket
	{
		public ActionHDVPacket(Guid hdvItemId, int action)
		{
			HdvItemId = hdvItemId;
			Action = action;
		}

		public Guid HdvItemId { get; set; }
		public int Action { get; set; }
	}
}
