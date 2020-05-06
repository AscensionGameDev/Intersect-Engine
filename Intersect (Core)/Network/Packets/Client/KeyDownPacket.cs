using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Client
{
    public class KeyDownPacket : CerasPacket
    {
        public KeyDownPacket(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
