using Intersect.Logging;
using Intersect.Network.Packets.Ping;

namespace Intersect.Network.Handlers
{
    public class PingHandler
    {
        public bool HandlePing(IPacket packet)
        {
            var ping = (PingPacket)packet;

            Log.Debug($"Handling PingPacket (sender = {ping.Connection.Guid}, requestPong = {ping.RequestPong}).");

            if (ping.RequestPong)
            {
                var pong = new PingPacket(null);
                ping.Connection.Send(pong);
            }

            return true;
        }
    }
}