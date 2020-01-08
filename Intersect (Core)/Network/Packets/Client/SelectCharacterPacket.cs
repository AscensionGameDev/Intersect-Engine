using System;

namespace Intersect.Network.Packets.Client
{
    public class SelectCharacterPacket : CerasPacket
    {
        public Guid CharacterId { get; set; }

        public SelectCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }
    }
}
