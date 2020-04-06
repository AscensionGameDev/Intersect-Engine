namespace Intersect.Network.Packets.Server
{
	public class MailBoxsUpdatePacket : CerasPacket
	{
		public MailBoxsUpdatePacket(MailBoxUpdatePacket[] mailboxs)
		{
			Mails = mailboxs;
		}

		public MailBoxUpdatePacket[] Mails { get; set; }
	}
}
