using System;

namespace Intersect.Network.Packets.Client
{

    public class SelectCharacterPacket : CerasPacket
    {

        public SelectCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }

        public Guid CharacterId { get; set; }

    }

}
