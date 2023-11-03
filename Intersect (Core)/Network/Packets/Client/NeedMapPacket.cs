using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class NeedMapPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public NeedMapPacket()
        {
        }

        public NeedMapPacket(params Guid[] mapIds)
        {
            MapIds = new List<Guid>(mapIds);
        }

        [Key(0)]
        public List<Guid> MapIds { get; set; }

    }

}
