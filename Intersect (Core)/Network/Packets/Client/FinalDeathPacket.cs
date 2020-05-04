using System;

namespace Intersect.Network.Packets.Client
{

    public class FinalDeathPacket : CerasPacket
    {

        public FinalDeathPacket(bool noRevive) 
        {
            NoRevive = noRevive;
        }

        public bool NoRevive { get; set; }
    }

}
