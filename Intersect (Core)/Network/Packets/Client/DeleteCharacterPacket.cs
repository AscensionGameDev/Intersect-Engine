using System;

namespace Intersect.Network.Packets.Client
{

    public class DeleteCharacterPacket : CerasPacket
    {

        public DeleteCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }

        public Guid CharacterId { get; set; }

    }

}
