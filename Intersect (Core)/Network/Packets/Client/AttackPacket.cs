using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class AttackPacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public AttackPacket()
        {

        }
        
        public AttackPacket(Guid target)
        {
            Target = target;
        }

        [Key(3)]
        public Guid Target { get; set; }

    }

}
