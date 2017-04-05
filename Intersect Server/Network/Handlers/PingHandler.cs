using Intersect.Network;
using Intersect.Network.Packets.Ping;

namespace Intersect.Server.Network.Handlers
{
    public class PingHandler
    {
        public bool HandlePing(IPacket packet)
        {
            var ping = (PingPacket)packet;
            if (ping.RequestPong)
            {
                var pong = new PingPacket(null);
                ping.Connection.Send(pong);
            }
            return true;
        }
    }
}