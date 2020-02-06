using System;

namespace Intersect.Network.Packets.Client
{
    public class DeleteCharacterPacket : CerasPacket
    {
        public Guid CharacterId { get; set; }

        public DeleteCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }
    }
}
