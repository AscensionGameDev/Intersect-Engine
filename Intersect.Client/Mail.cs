using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Client
{
	public class Mail
	{
		public Mail(Guid mailId, string name, string message, string senderName, Guid item, int quantity = 0)
		{
			MailID = mailId;
			Name = name;
			Message = message;
			SenderName = senderName;
			Item = item;
			Quantity = quantity;
		}

		public Guid MailID { get; set; }

		public string Name { get; set; }

		public string Message { get; set; }

		public string SenderName { get; set; }

		public Guid Item { get; set; } = Guid.Empty;

		public int Quantity { get; set; }
	}
}
