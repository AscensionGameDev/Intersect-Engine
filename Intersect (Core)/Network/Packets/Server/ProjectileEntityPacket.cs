using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ProjectileEntityPacket : EntityPacket
    {
        //Parameterless Constructor for MessagePack
        public ProjectileEntityPacket()
        {
        }

        [Key(24)]
        public Guid ProjectileId { get; set; }


        [Key(25)]
        public byte ProjectileDirection { get; set; }


        [Key(26)]
        public Guid TargetId { get; set; }


        [Key(27)]
        public Guid OwnerId { get; set; }

    }

}
