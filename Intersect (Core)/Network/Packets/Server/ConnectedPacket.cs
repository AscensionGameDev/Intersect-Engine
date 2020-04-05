using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
	public class ConnectedPacket : CerasPacket
	{
		public ConnectedPacket(string[] online)
		{
			Online = online;
		}
		public string[] Online { get; set; }
	}
}
