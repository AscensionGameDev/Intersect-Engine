namespace Intersect.Network
{
    public interface IPacketHandler
    {
        bool Handle(IPacketSender packetSender, IPacket packet);
    }

    public interface IPacketHandler<TPacket> : IPacketHandler where TPacket : IPacket
    {
        bool Handle(IPacketSender packetSender, TPacket packet);
    }
}
