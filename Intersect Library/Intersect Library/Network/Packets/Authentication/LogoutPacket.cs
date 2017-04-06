using Intersect.Memory;

namespace Intersect.Network.Packets.Authentication
{
    public class LogoutPacket : AbstractAuthenticationPacket
    {
        public LogoutPacket(IConnection connection)
            : base(connection, AuthenticationPackets.Login)
        {
        }
    }
}