using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class RequestPasswordResetPacket : CerasPacket
    {
        public string NameOrEmail { get; set; }

        public RequestPasswordResetPacket(string nameOrEmail)
        {
            NameOrEmail = nameOrEmail;
        }
    }
}
