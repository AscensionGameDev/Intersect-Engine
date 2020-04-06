namespace Intersect.Network.Packets.Server
{

	public class MailBoxPacket : CerasPacket
	{

		public MailBoxPacket(bool close, bool send)
		{
			Close = close;
			Send = send;
		}

		public bool Close { get; set; }

		public bool Send { get; set; }

	}

}

