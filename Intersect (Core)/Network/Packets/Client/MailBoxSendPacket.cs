using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
	public class MailBoxSendPacket : CerasPacket
	{
		public MailBoxSendPacket(string to, string title, string message, Guid itemid, int quantity)
		{
			To = to;
			Title = title;
			Message = message;
			ItemID = itemid;
			Quantity = quantity;
		}

		public string To { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public Guid ItemID { get; set; }
		public int Quantity { get; set; }
	}
}
