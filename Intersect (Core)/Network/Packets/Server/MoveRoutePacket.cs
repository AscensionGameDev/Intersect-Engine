using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MoveRoutePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MoveRoutePacket()
        {
        }

        public MoveRoutePacket(bool active)
        {
            Active = active;
        }

        [Key(0)]
        public bool Active { get; set; }

    }

}
