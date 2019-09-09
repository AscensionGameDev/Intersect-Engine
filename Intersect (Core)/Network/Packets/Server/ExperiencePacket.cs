using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class ExperiencePacket : CerasPacket
    {
        public long Experience { get; set; }
        public long ExperienceToNextLevel { get; set; }

        public ExperiencePacket(long exp, long tnl)
        {
            Experience = exp;
            ExperienceToNextLevel = tnl;
        }

    }
}
