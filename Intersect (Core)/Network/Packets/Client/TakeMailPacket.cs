using System;

namespace Intersect.Network.Packets.Client
{
	public class TakeMailPacket : CerasPacket
	{
		public TakeMailPacket(Guid mailId)
		{
			MailID = mailId;
		}

		public Guid MailID { get; private set; }
	}
}
