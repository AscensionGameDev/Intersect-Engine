using Lidgren.Network;

namespace Intersect.Network.Packets.Ping
{
    public class PingPacket : AbstractPacket
    {
        public bool RequestPong { get; set; }
    
        public PingPacket(NetConnection connection)
            : base(connection, PacketGroups.Ping)
        {
            RequestPong = false;
        }

        public override bool Read(ref NetIncomingMessage message)
        {
            RequestPong = message.ReadBoolean();

            return true;
        }

        public override bool Write(ref NetOutgoingMessage message)
        {
            message.Write(RequestPong);

            return false;
        }
    }
}