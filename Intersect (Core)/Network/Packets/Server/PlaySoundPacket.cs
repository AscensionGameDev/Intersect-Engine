using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PlaySoundPacket : CerasPacket
    {
        public string Sound { get; set; }

        public PlaySoundPacket(string sound)
        {
            Sound = sound;
        }
    }
}
