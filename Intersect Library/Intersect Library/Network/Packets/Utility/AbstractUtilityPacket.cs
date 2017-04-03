namespace Intersect.Network.Packets.Utility
{
    public abstract class AbstractUtilityPacket : AbstractExtendedPacket<UtilityPackets>
    {
        protected AbstractUtilityPacket(IConnection connection, UtilityPackets type)
            : base(connection, PacketGroups.Utility, type)
        {
        }
    }
}