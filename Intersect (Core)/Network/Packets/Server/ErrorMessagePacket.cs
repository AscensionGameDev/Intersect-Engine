using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ErrorMessagePacket : CerasPacket
    {
        public string Header { get; set; }
        public string Error { get; set; }

        public ErrorMessagePacket(string header, string error)
        {
            Header = header;
            Error = error;
        }
    }
}
