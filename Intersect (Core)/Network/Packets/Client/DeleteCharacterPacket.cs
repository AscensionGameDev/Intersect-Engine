using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class DeleteCharacterPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public DeleteCharacterPacket()
        {
        }

        public DeleteCharacterPacket(Guid charId)
        {
            CharacterId = charId;
        }

        [Key(0)]
        public Guid CharacterId { get; set; }

    }

}
