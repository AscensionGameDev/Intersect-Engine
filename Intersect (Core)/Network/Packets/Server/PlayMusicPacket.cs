using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class PlayMusicPacket : CerasPacket
    {
        public string BGM { get; set; }

        public PlayMusicPacket(string bgm)
        {
            BGM = bgm;
        }
    }
}
