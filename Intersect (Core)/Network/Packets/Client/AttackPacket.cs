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
        
        public AttackPacket(Guid target, bool targetOnFocus)
        {
            Target = target;
            TargetOnFocus = targetOnFocus;
        }

        [Key(3)]
        public Guid Target { get; set; }
        
        [Key(4)]
        public bool TargetOnFocus { get; set; }

    }

}
