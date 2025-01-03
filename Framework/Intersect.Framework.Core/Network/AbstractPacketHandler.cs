namespace Intersect.Network;

public abstract class AbstractPacketHandler<TPacket> : IPacketHandler<TPacket> where TPacket : class, IPacket
{
    public abstract bool Handle(IPacketSender packetSender, TPacket packet);

    public virtual bool Handle(IPacketSender packetSender, IPacket packet)
    {
        return packet is TPacket typedPacket && Handle(packetSender, typedPacket);
    }
}