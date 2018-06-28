namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Network
{
    public interface IPacketFactory
    {
        bool CanCreatePacketType(PacketCode packetCode);

        IPacket Create(PacketCode packetCode, IConnection connection);
        IPacket Create(PacketCode packetCode, IConnection connection, params object[] args);

        TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection)
            where TPacketType : class, IPacket;

        TPacketType Create<TPacketType>(PacketCode packetCode, IConnection connection, params object[] args)
            where TPacketType : class, IPacket;
    }
}