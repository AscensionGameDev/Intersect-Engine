using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class DirectionPacket : CerasPacket
    {
        public byte Direction { get; set; }

        public DirectionPacket(byte dir)
        {
            Direction = dir;
        }
    }
}
