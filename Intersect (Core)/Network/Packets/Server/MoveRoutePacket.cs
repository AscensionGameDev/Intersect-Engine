namespace Intersect.Network.Packets.Server
{

    public class MoveRoutePacket : CerasPacket
    {

        public MoveRoutePacket(bool active)
        {
            Active = active;
        }

        public bool Active { get; set; }

    }

}
