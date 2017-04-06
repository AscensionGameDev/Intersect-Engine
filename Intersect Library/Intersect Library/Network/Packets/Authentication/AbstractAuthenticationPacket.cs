namespace Intersect.Network.Packets.Authentication
{
    public abstract class AbstractAuthenticationPacket : AbstractExtendedPacket<AuthenticationPackets>
    {
        protected AbstractAuthenticationPacket(IConnection connection, AuthenticationPackets type)
            : base(connection, PacketGroups.Authentication, type)
        {
        }
    }
}