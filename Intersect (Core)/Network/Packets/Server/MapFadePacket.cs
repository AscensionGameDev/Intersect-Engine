using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapFadePacket : IntersectPacket
    {
        public MapFadePacket()
        {
            FadeIn = false;
        }

        public MapFadePacket(bool fadeIn)
        {
            FadeIn = fadeIn;
        }

        [Key(0)]
        public bool FadeIn { get; set; }
    }
}
